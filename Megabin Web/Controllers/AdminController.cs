using Megabin_Web.Features.Address.CreateAddress;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Megabin_Web.Shared.DTOs.Addresses;
using Megabin_Web.Shared.DTOs.Drivers;
using Megabin_Web.Shared.DTOs.Routing;
using Megabin_Web.Shared.DTOs.ScheduleContracts;
using Megabin_Web.Shared.DTOs.Users;
using Megabin_Web.Shared.Infrastructure.OpenRouteService;
using Megabin_Web.Shared.Infrastructure.PasswordService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly IRouteOptimizationService _routeOptimizationService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            AppDbContext dbContext,
            IPasswordService passwordService,
            IRouteOptimizationService routeOptimizationService,
            ILogger<AdminController> logger
        )
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _routeOptimizationService = routeOptimizationService;
            _logger = logger;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<GetUser>>> GetAllUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            var userList = new List<GetUser>();
            foreach (var user in users)
            {
                userList.Add(
                    new GetUser
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role,
                        PhoneNumber = user.PhoneNumber,
                    }
                );
            }
            return Ok(userList);
        }

        [HttpPost("ResetUserPassword")]
        public async Task<ActionResult> ResetUserPassword(ResetPassword resetPassword)
        {
            var user = await _dbContext.Users.FindAsync(resetPassword.UserId);
            if (user == null)
                return NotFound();
            await _passwordService.ResetPassword(resetPassword.UserId, resetPassword.NewPassword);
            return Ok();
        }

        [HttpGet("GetUser/{userId}")]
        public async Task<ActionResult<GetUser>> GetUser(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            var userDto = new GetUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
            };
            return Ok(userDto);
        }

        [HttpPost("UpdateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUser updateUser)
        {
            var user = await _dbContext.Users.FindAsync(updateUser.UserId);
            if (user == null)
                return NotFound();

            user.Name = updateUser.Name;
            user.Email = updateUser.Email;
            user.Role = updateUser.Role;
            user.PhoneNumber = updateUser.PhoneNumber;

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            // Load user with all related entities
            var user = await _dbContext
                .Users.Include(u => u.Addresss)
                    .ThenInclude(a => a.Schedules)
                .Include(u => u.ApiUsageTracker)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            // 1. Delete all ScheduleContracts for all user addresses
            foreach (var address in user.Addresss)
            {
                _dbContext.ScheduledContract.RemoveRange(address.Schedules);
            }

            // 2. Delete Driver profile if user is a driver
            var driver = await _dbContext.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver != null)
            {
                _dbContext.Drivers.Remove(driver);
            }

            // 3. Delete ScheduledCollections where user is assigned as driver
            var scheduledCollections = await _dbContext
                .ScheduledCollections.Where(sc => sc.UserId == userId)
                .ToListAsync();
            _dbContext.ScheduledCollections.RemoveRange(scheduledCollections);

            // 4. Delete all APIUsageTracker records
            _dbContext.APIUsageTrackers.RemoveRange(user.ApiUsageTracker);

            // 5. Delete all Addresses (already loaded via Include)
            _dbContext.Addresses.RemoveRange(user.Addresss);

            // 6. Finally, delete the user
            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("AddUserAddress")]
        public async Task<ActionResult<CreateAddressResponseDto>> AddUserAddress(
            CreateAddressCommand addUserAddress
        )
        {
            var user = await _dbContext
                .Users.Include(x => x.Addresss)
                .FirstOrDefaultAsync(x => x.Id == addUserAddress.UserId);
            if (user == null)
                return NotFound();

            var newAddress = new Addresses
            {
                Address = addUserAddress.Address.Label,
                Lat = addUserAddress.Address.Location.Latitude,
                Long = addUserAddress.Address.Location.Longitude,
                TotalBins = addUserAddress.TotalBins,
                AddressNotes = addUserAddress.AddressNotes,
                UserId = user.Id,
                User = user,
                Status = addUserAddress.Status,
            };

            user.Addresss.Add(newAddress);
            await _dbContext.SaveChangesAsync();
            var res = new CreateAddressResponseDto { AddressId = newAddress.Id };
            return Ok(res);
        }

        [HttpPost("UpdateUserAddress")]
        public async Task<ActionResult> UpdateUserAddress(UpdateAddress updateUserAddress)
        {
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(x =>
                x.Id == updateUserAddress.AddressId
            );
            if (address == null)
                return NotFound();
            address.TotalBins = updateUserAddress.TotalBins;
            address.AddressNotes = updateUserAddress.AddressNotes;
            address.Status = updateUserAddress.Status;
            address.Long = updateUserAddress.Location.Longitude;
            address.Lat = updateUserAddress.Location.Latitude;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("GetAllUserAddresses/{userId}")]
        public async Task<ActionResult<List<GetAddress>>> GetAllUserAddresses(Guid userId)
        {
            var user = await _dbContext
                .Users.Include(a => a.Addresss)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return NotFound();
            var addressList = new List<GetAddress>();
            foreach (var address in user.Addresss)
            {
                addressList.Add(
                    new GetAddress
                    {
                        Id = address.Id,
                        Address = address.Address,
                        AddressNotes = address.AddressNotes ?? string.Empty,
                        TotalBins = address.TotalBins,
                        AddressStatus = address.Status,
                        Location = new Location(address.Long, address.Lat),
                    }
                );
            }
            return Ok(addressList);
        }

        [HttpPost("AddScheduleContract")]
        public async Task<ActionResult> AddScheduleContract(CreateScheduleContract createSchedule)
        {
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(x =>
                x.Id == createSchedule.AddressId
            );
            if (address == null)
                return NotFound();
            _dbContext.ScheduledContract.Add(
                new Shared.Domain.Entities.ScheduleContract
                {
                    AddressesId = createSchedule.AddressId,
                    Addresses = address,
                    DayOfWeek = createSchedule.DayOfWeek,
                    Frequency = createSchedule.Frequency,
                    Active = true,
                    ApprovedExternally = true,
                }
            );
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("GetScheduledContracts/{addressId}")]
        public async Task<ActionResult<List<GetScheduleContract>>> GetScheduledContracts(
            Guid addressId
        )
        {
            var contracts = await _dbContext
                .ScheduledContract.Where(x => x.AddressesId == addressId)
                .Select(c => new GetScheduleContract
                {
                    Id = c.Id,
                    Frequency = c.Frequency,
                    DayOfWeek = c.DayOfWeek,
                    StartingDate = c.StartingDate,
                    LastCollected = c.LastCollected,
                    Active = c.Active,
                    ApprovedExternally = c.ApprovedExternally,
                    AddressesId = c.AddressesId,
                })
                .ToListAsync();

            if (contracts == null || contracts.Count == 0)
                return NotFound();

            return Ok(contracts);
        }

        [HttpPost("UpdateScheduleContract")]
        public async Task<ActionResult> UpdateScheduleContract(
            UpdateScheduleContract updateSchedule
        )
        {
            var contract = await _dbContext.ScheduledContract.FindAsync(updateSchedule.ContractId);
            if (contract == null)
                return NotFound();

            contract.Frequency = updateSchedule.Frequency;
            contract.DayOfWeek = updateSchedule.DayOfWeek;
            contract.Active = updateSchedule.Active;
            contract.ApprovedExternally = updateSchedule.ApprovedExternally;

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("DeleteScheduleContract")]
        public async Task<ActionResult> DeleteScheduleContract(Guid contractId)
        {
            var contract = await _dbContext.ScheduledContract.FindAsync(contractId);
            if (contract == null)
                return NotFound();
            _dbContext.ScheduledContract.Remove(contract);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        /*
        [HttpPost("GenerateDailyRoutes")]
        public async Task<ActionResult<DailyOptimizationResult>> GenerateDailyRoutes(
            DateTime? targetDate = null
        )
        {
            var optimizationDate = targetDate ?? DateTime.Today;

            _logger.LogInformation(
                "Starting manual route optimization for date {Date}",
                optimizationDate
            );

            // Clear existing schedules for the target date
            var existingSchedules = await _dbContext
                .ScheduledCollections.Where(sc => sc.ScheduledFor.Date == optimizationDate.Date)
                .ToListAsync();

            if (existingSchedules.Count > 0)
            {
                _logger.LogInformation(
                    "Clearing {Count} existing schedules for {Date}",
                    existingSchedules.Count,
                    optimizationDate
                );
                _dbContext.ScheduledCollections.RemoveRange(existingSchedules);
            }

            // Get day of week for the target date
            var dayOfWeekEnum = optimizationDate.DayOfWeek;

            // Get all active schedule contracts that need collection on this day of week
            var activeContracts = await _dbContext
                .ScheduledContract.Include(sc => sc.Addresses)
                .Where(sc =>
                    sc.Active && sc.ApprovedExternally && (DayOfWeek)sc.DayOfWeek == dayOfWeekEnum
                )
                .ToListAsync();

            if (activeContracts.Count == 0)
            {
                _logger.LogInformation(
                    "No active schedule contracts found for {DayOfWeek}",
                    dayOfWeekEnum
                );
                return Ok(
                    new DailyOptimizationResult(
                        new List<DriverRoute>(),
                        new List<CollectionJob>(),
                        0,
                        0
                    )
                );
            }

            _logger.LogInformation(
                "Found {Count} active schedule contracts for {DayOfWeek}",
                activeContracts.Count,
                dayOfWeekEnum
            );

            // Get all active drivers with their home addresses
            var drivers = await _dbContext
                .Drivers.Include(d => d.HomeAddress)
                .Where(d => d.Active)
                .ToListAsync();

            if (drivers.Count == 0)
            {
                _logger.LogWarning("No active drivers found. Cannot optimize routes.");
                return BadRequest("No active drivers available for route optimization.");
            }

            _logger.LogInformation("Found {Count} active drivers", drivers.Count);

            // Define depot locations (using hardcoded Johannesburg depot)
            var depots = new List<DepotLocation>
            {
                new DepotLocation(
                    "depot-1",
                    new Location(28.0473, -26.2041), // Johannesburg placeholder
                    "Main Depot, Johannesburg"
                ),
            };

            // Build collection jobs from schedule contracts
            var jobs = activeContracts
                .Select(sc => new CollectionJob(
                    sc.Id.ToString(),
                    new Location(sc.Addresses.Long, sc.Addresses.Lat),
                    sc.Addresses.Address
                ))
                .ToList();

            // Build driver vehicles
            var vehicles = drivers
                .Select(d => new DriverVehicle(
                    d.Id.ToString(),
                    new Location(d.HomeAddress.Long, d.HomeAddress.Lat),
                    d.VehicleCapacity
                ))
                .ToList();

            // Optimize routes
            _logger.LogInformation(
                "Calling route optimization for {JobCount} jobs and {DriverCount} drivers",
                jobs.Count,
                drivers.Count
            );

            var result = await _routeOptimizationService.OptimizeMultiVehicleRoutesAsync(
                jobs,
                vehicles,
                depots
            );

            _logger.LogInformation(
                "Route optimization complete: {RouteCount} routes created, {UnassignedCount} unassigned jobs, Total distance: {Distance}m",
                result.Routes.Count,
                result.UnassignedJobs.Count,
                result.TotalDistanceMeters
            );

            // Save optimized routes to database as ScheduledCollections
            foreach (var route in result.Routes)
            {
                var driverId = Guid.Parse(route.DriverId);
                var driver = drivers.First(d => d.Id == driverId);

                // Create scheduled collections for each stop in the route (excluding depot stops)
                foreach (var stop in route.Stops.Where(s => s.Type == StopType.Collection))
                {
                    var scheduleContractId = Guid.Parse(stop.JobId!);

                    var scheduledCollection = new ScheduledCollections
                    {
                        Id = Guid.NewGuid(),
                        ScheduledFor = optimizationDate,
                        UserId = driverId, // Note: This field stores the driver ID
                        User = driver.User,
                        Collected = false,
                        Notes = $"Route order: {route.Stops.IndexOf(stop) + 1}/{route.Stops.Count}",
                    };

                    _dbContext.ScheduledCollections.Add(scheduledCollection);
                }
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Manual route optimization completed successfully. Created {Count} scheduled collections.",
                result.Routes.Sum(r => r.Stops.Count(s => s.Type == StopType.Collection))
            );

            return Ok(result);
        }
        */

        [HttpPost("CreateDriver")]
        public async Task<IActionResult> CreateDriver(CreateDriver createDriver)
        {
            var user = await _dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == createDriver.UserId);
            if (user == null)
                return NotFound();
            if (user.DriverProfile != null)
                return BadRequest("User is already a driver.");

            var driver = new Driver
            {
                Active = createDriver.Active,
                LicenseNumber = createDriver.LicenseNumber,
                UserId = user.Id,
                User = user,
                HomeAddressLabel = createDriver.HomeAddressLabel,
                HomeAddressLong = createDriver.HomeAddressLong,
                HomeAddressLat = createDriver.HomeAddressLat,
                DropoffLocationLabel = createDriver.DropoffLocationLabel,
                DropoffLocationLong = createDriver.DropoffLocationLong,
                DropoffLocationLat = createDriver.DropoffLocationLat,
                VehicleCapacity = createDriver.VehicleCapacity,
            };
            _dbContext.Drivers.Add(driver);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("UpdateDriver")]
        public async Task<IActionResult> UpdateDriver(UpdateDriver updateDriver)
        {
            var driver = await _dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == updateDriver.UserId);
            if (driver == null || driver.DriverProfile == null)
                return NotFound();
            driver.DriverProfile.Active = updateDriver.Active;
            driver.DriverProfile.LicenseNumber = updateDriver.LicenseNumber;
            driver.DriverProfile.VehicleCapacity = updateDriver.VehicleCapacity;
            driver.DriverProfile.HomeAddressLabel = updateDriver.HomeAddressLabel;
            driver.DriverProfile.HomeAddressLong = updateDriver.HomeAddressLong;
            driver.DriverProfile.HomeAddressLat = updateDriver.HomeAddressLat;
            driver.DriverProfile.DropoffLocationLabel = updateDriver.DropoffLocationLabel;
            driver.DriverProfile.DropoffLocationLong = updateDriver.DropoffLocationLong;
            driver.DriverProfile.DropoffLocationLat = updateDriver.DropoffLocationLat;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> DeleteDriver(Guid userId)
        {
            var driver = await _dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (driver == null || driver.DriverProfile == null)
                return NotFound();

            _dbContext.Drivers.Remove(driver.DriverProfile);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("DisableDriver")]
        public async Task<IActionResult> DisableDriver(Guid userId)
        {
            var driver = await _dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (driver == null || driver.DriverProfile == null)
                return NotFound();

            driver.DriverProfile.Active = false;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("GetDriver")]
        public async Task<ActionResult<GetDriver>> GetDriver(Guid userId)
        {
            var driver = await _dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (driver == null || driver.DriverProfile == null)
                return NotFound();
            var driverDto = new GetDriver
            {
                DriverId = driver.DriverProfile.Id,
                HomeAddressLabel = driver.DriverProfile.HomeAddressLabel,
                HomeAddressLong = driver.DriverProfile.HomeAddressLong,
                HomeAddressLat = driver.DriverProfile.HomeAddressLat,
                DropoffLocationLabel = driver.DriverProfile.DropoffLocationLabel,
                DropoffLocationLong = driver.DriverProfile.DropoffLocationLong,
                DropoffLocationLat = driver.DriverProfile.DropoffLocationLat,
                VehicleCapacity = driver.DriverProfile.VehicleCapacity,
                LicenseNumber = driver.DriverProfile.LicenseNumber,
                Active = driver.DriverProfile.Active,
                UserId = driver.Id,
            };
            return Ok(driverDto);
        }

        //Now in the FE - we need to conditionally trigger these things.
        // If the user has a state of Driver - fetch driver
        // else do nothing
        // If the user gets set to a driver - fetch driver
        // If found - load the driver details into the form thats amended because the user is now a driver
        // If user clicks update, call update driver and pass the driver details
        // if the user gets set to non-driver - call disable driver
        // if the the user is not found - assume it's a new drive and call create driver rather than update driver.
        // if it's loaded again --> Get Driver --> Based on that info, either populate the form and call update driver, or show an empty form and call create driver.
    }
}
