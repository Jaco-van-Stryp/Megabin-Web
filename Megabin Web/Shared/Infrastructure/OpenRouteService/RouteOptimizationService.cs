using System.Text;
using System.Text.Json;
using Megabin_Web.Shared.DTOs.OpenRouteService;
using Megabin_Web.Shared.DTOs.Routing;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Shared.Infrastructure.OpenRouteService
{
    /// <summary>
    /// Implementation of multi-vehicle route optimization using OpenRouteService VROOM engine.
    /// Uses OpenRouteService cloud API for daily collection route planning with capacity constraints.
    /// </summary>
    public class RouteOptimizationService : IRouteOptimizationService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenRouteServiceOptions _options;
        private readonly ILogger<RouteOptimizationService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the RouteOptimizationService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making requests to OpenRouteService API.</param>
        /// <param name="options">Configuration options for ORS base URL and timeout.</param>
        /// <param name="logger">Logger for debugging and monitoring optimization operations.</param>
        public RouteOptimizationService(
            HttpClient httpClient,
            IOptions<OpenRouteServiceOptions> options,
            ILogger<RouteOptimizationService> logger
        )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

            // Add API key to Authorization header for cloud API
            if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization",
                    _options.ApiKey
                );
            }

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };
        }

        /// <inheritdoc/>
        public async Task<DailyOptimizationResult> OptimizeMultiVehicleRoutesAsync(
            IReadOnlyList<CollectionJob> jobs,
            IReadOnlyList<DriverVehicle> vehicles,
            IReadOnlyList<DepotLocation> depots,
            CancellationToken cancellationToken = default
        )
        {
            if (jobs == null || jobs.Count == 0)
            {
                _logger.LogWarning("OptimizeMultiVehicleRoutesAsync called with no jobs");
                return new DailyOptimizationResult(
                    new List<DriverRoute>(),
                    new List<CollectionJob>(),
                    0,
                    0
                );
            }

            if (vehicles == null || vehicles.Count == 0)
            {
                _logger.LogWarning("OptimizeMultiVehicleRoutesAsync called with no vehicles");
                return new DailyOptimizationResult(new List<DriverRoute>(), jobs.ToList(), 0, 0);
            }

            if (depots == null || depots.Count == 0)
            {
                throw new ArgumentException(
                    "At least one depot location is required",
                    nameof(depots)
                );
            }

            try
            {
                _logger.LogInformation(
                    "Starting route optimization: {JobCount} jobs, {VehicleCount} vehicles, {DepotCount} depots",
                    jobs.Count,
                    vehicles.Count,
                    depots.Count
                );

                // Validate inputs
                ValidateInputs(jobs, vehicles, depots);

                // Assign each driver to nearest depot
                var vehicleDepotAssignments = AssignVehiclesToDepots(vehicles, depots);

                // Build VROOM request
                var vroomRequest = BuildVroomRequest(jobs, vehicles, vehicleDepotAssignments);

                // Call VROOM API
                var vroomResponse = await CallVroomApiAsync(vroomRequest, cancellationToken);

                // Parse and build result
                var result = BuildOptimizationResult(
                    vroomResponse,
                    jobs,
                    vehicles,
                    depots,
                    vehicleDepotAssignments
                );

                _logger.LogInformation(
                    "Optimization complete: {AssignedRoutes} routes, {UnassignedJobs} unassigned jobs, "
                        + "Total distance: {Distance:F2}m, Total duration: {Duration:F2}s",
                    result.Routes.Count,
                    result.UnassignedJobs.Count,
                    result.TotalDistanceMeters,
                    result.TotalDurationSeconds
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during route optimization");
                throw new InvalidOperationException("Failed to optimize routes", ex);
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Validates input parameters for optimization.
        /// </summary>
        private void ValidateInputs(
            IReadOnlyList<CollectionJob> jobs,
            IReadOnlyList<DriverVehicle> vehicles,
            IReadOnlyList<DepotLocation> depots
        )
        {
            foreach (var job in jobs)
            {
                if (string.IsNullOrWhiteSpace(job.Id))
                    throw new ArgumentException("All jobs must have a valid ID");
                if (!IsValidCoordinate(job.Location))
                    throw new ArgumentException($"Invalid coordinates for job {job.Id}");
            }

            foreach (var vehicle in vehicles)
            {
                if (string.IsNullOrWhiteSpace(vehicle.Id))
                    throw new ArgumentException("All vehicles must have a valid ID");
                if (!IsValidCoordinate(vehicle.StartLocation))
                    throw new ArgumentException(
                        $"Invalid start coordinates for vehicle {vehicle.Id}"
                    );
                if (vehicle.Capacity <= 0)
                    throw new ArgumentException($"Vehicle {vehicle.Id} must have capacity > 0");
            }

            foreach (var depot in depots)
            {
                if (!IsValidCoordinate(depot.Location))
                    throw new ArgumentException($"Invalid coordinates for depot {depot.Id}");
            }
        }

        /// <summary>
        /// Checks if coordinates are within valid ranges.
        /// </summary>
        private bool IsValidCoordinate(Location location)
        {
            return location.Longitude >= -180
                && location.Longitude <= 180
                && location.Latitude >= -90
                && location.Latitude <= 90;
        }

        /// <summary>
        /// Assigns each vehicle to the nearest depot based on straight-line distance.
        /// </summary>
        private Dictionary<string, DepotLocation> AssignVehiclesToDepots(
            IReadOnlyList<DriverVehicle> vehicles,
            IReadOnlyList<DepotLocation> depots
        )
        {
            var assignments = new Dictionary<string, DepotLocation>();

            foreach (var vehicle in vehicles)
            {
                var nearestDepot = depots
                    .OrderBy(d => CalculateDistance(vehicle.StartLocation, d.Location))
                    .First();

                assignments[vehicle.Id] = nearestDepot;

                _logger.LogDebug(
                    "Assigned vehicle {VehicleId} to depot {DepotId}",
                    vehicle.Id,
                    nearestDepot.Id
                );
            }

            return assignments;
        }

        /// <summary>
        /// Calculates straight-line distance between two locations (Haversine formula).
        /// </summary>
        private double CalculateDistance(Location loc1, Location loc2)
        {
            const double EarthRadiusKm = 6371.0;

            var dLat = DegreesToRadians(loc2.Latitude - loc1.Latitude);
            var dLon = DegreesToRadians(loc2.Longitude - loc1.Longitude);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(DegreesToRadians(loc1.Latitude))
                    * Math.Cos(DegreesToRadians(loc2.Latitude))
                    * Math.Sin(dLon / 2)
                    * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c * 1000; // Convert to meters
        }

        private double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

        /// <summary>
        /// Builds VROOM optimization request from inputs.
        /// </summary>
        private VroomRequest BuildVroomRequest(
            IReadOnlyList<CollectionJob> jobs,
            IReadOnlyList<DriverVehicle> vehicles,
            Dictionary<string, DepotLocation> vehicleDepotAssignments
        )
        {
            var vroomJobs = jobs.Select(j => new VroomJob
                {
                    Id = j.Id,
                    Location = new List<double> { j.Location.Longitude, j.Location.Latitude },
                    Amount = new List<int> { 1 }, // Each collection consumes 1 capacity unit
                })
                .ToList();

            var vroomVehicles = vehicles
                .Select(v => new VroomVehicle
                {
                    Id = v.Id,
                    Start = new List<double>
                    {
                        v.StartLocation.Longitude,
                        v.StartLocation.Latitude,
                    },
                    End = new List<double>
                    {
                        vehicleDepotAssignments[v.Id].Location.Longitude,
                        vehicleDepotAssignments[v.Id].Location.Latitude,
                    },
                    Capacity = new List<int> { v.Capacity },
                })
                .ToList();

            return new VroomRequest
            {
                Jobs = vroomJobs,
                Vehicles = vroomVehicles,
                Options = new VroomOptions { IncludeGeometry = true },
            };
        }

        /// <summary>
        /// Calls the VROOM optimization API.
        /// </summary>
        private async Task<VroomResponse> CallVroomApiAsync(
            VroomRequest request,
            CancellationToken cancellationToken
        )
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("Sending VROOM request: {RequestJson}", json);

            var response = await _httpClient.PostAsync("/optimization", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "VROOM API returned status {StatusCode}: {ErrorContent}",
                    response.StatusCode,
                    errorContent
                );
                throw new InvalidOperationException(
                    $"VROOM API request failed with status {response.StatusCode}: {errorContent}"
                );
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Received VROOM response: {ResponseJson}", responseContent);

            var vroomResponse = JsonSerializer.Deserialize<VroomResponse>(
                responseContent,
                _jsonOptions
            );

            if (vroomResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize VROOM response");
            }

            return vroomResponse;
        }

        /// <summary>
        /// Builds the final optimization result from VROOM response.
        /// </summary>
        private DailyOptimizationResult BuildOptimizationResult(
            VroomResponse vroomResponse,
            IReadOnlyList<CollectionJob> jobs,
            IReadOnlyList<DriverVehicle> vehicles,
            IReadOnlyList<DepotLocation> depots,
            Dictionary<string, DepotLocation> vehicleDepotAssignments
        )
        {
            var jobLookup = jobs.ToDictionary(j => j.Id);
            var driverRoutes = new List<DriverRoute>();

            foreach (var vroomRoute in vroomResponse.Routes)
            {
                if (vroomRoute.Vehicle == null)
                    continue;

                var stops = new List<RouteStop>();

                foreach (var step in vroomRoute.Steps)
                {
                    if (step.Type == "start")
                    {
                        // Skip start steps, they're just the vehicle starting position
                        continue;
                    }
                    else if (step.Type == "job" && step.Job != null)
                    {
                        // Collection stop
                        if (jobLookup.TryGetValue(step.Job, out var job))
                        {
                            stops.Add(
                                new RouteStop(
                                    StopType.Collection,
                                    job.Location,
                                    job.Address,
                                    job.Id
                                )
                            );
                        }
                    }
                    else if (step.Type == "end")
                    {
                        // Check if this is a depot stop (mid-route) or final end
                        // If there are more steps after this end, it's a depot dump
                        var stepIndex = vroomRoute.Steps.IndexOf(step);
                        var hasMoreJobsAfter = vroomRoute
                            .Steps.Skip(stepIndex + 1)
                            .Any(s => s.Type == "job");

                        if (
                            hasMoreJobsAfter
                            && vehicleDepotAssignments.TryGetValue(
                                vroomRoute.Vehicle,
                                out var depot
                            )
                        )
                        {
                            stops.Add(
                                new RouteStop(StopType.Depot, depot.Location, depot.Address, null)
                            );
                        }
                        // If it's the final end with no more jobs, we don't add it as a stop
                    }
                }

                var driverRoute = new DriverRoute(
                    vroomRoute.Vehicle,
                    stops,
                    vroomRoute.Distance,
                    vroomRoute.Duration
                );

                driverRoutes.Add(driverRoute);

                _logger.LogDebug(
                    "Driver {DriverId}: {StopCount} stops, {Distance:F2}m, {Duration:F2}s",
                    vroomRoute.Vehicle,
                    stops.Count,
                    vroomRoute.Distance,
                    vroomRoute.Duration
                );
            }

            // Collect unassigned jobs
            var unassignedJobs = vroomResponse
                .Unassigned.Where(u => jobLookup.ContainsKey(u.Id))
                .Select(u => jobLookup[u.Id])
                .ToList();

            if (unassignedJobs.Count > 0)
            {
                _logger.LogWarning(
                    "Found {UnassignedCount} unassigned jobs: {UnassignedIds}",
                    unassignedJobs.Count,
                    string.Join(", ", unassignedJobs.Select(j => j.Id))
                );
            }

            return new DailyOptimizationResult(
                driverRoutes,
                unassignedJobs,
                vroomResponse.Summary?.Distance ?? 0,
                vroomResponse.Summary?.Duration ?? 0
            );
        }

        #endregion
    }
}
