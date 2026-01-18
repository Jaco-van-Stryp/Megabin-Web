using Megabin_Web.Data;
using Megabin_Web.DTOs.Addresses;
using Megabin_Web.DTOs.Routing;
using Megabin_Web.DTOs.ScheduleContracts;
using Megabin_Web.DTOs.Users;
using Megabin_Web.Interfaces;
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

        public AdminController(AppDbContext dbContext, IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
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
        public async Task<ActionResult<CreateAddressResponse>> AddUserAddress(
            CreateAddress addUserAddress
        )
        {
            var user = await _dbContext
                .Users.Include(x => x.Addresss)
                .FirstOrDefaultAsync(x => x.Id == addUserAddress.UserId);
            if (user == null)
                return NotFound();

            var newAddress = new Entities.Addresses
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
            var res = new CreateAddressResponse { AddressId = newAddress.Id };
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
                new Entities.ScheduleContract
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
        public async Task<ActionResult<List<GetScheduleContract>>> GetScheduledContracts(Guid addressId)
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
                    AddressesId = c.AddressesId
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
    }
}
