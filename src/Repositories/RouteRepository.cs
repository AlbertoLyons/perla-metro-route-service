using Neo4j.Driver;
using perla_metro_route_service.src.Interfaces;
using perla_metro_route_service.src.data;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.repositories
{
    /// <summary>
    /// Implementation of IRouteRepository for managing Route entities in the Neo4j database.
    /// </summary>
    public class RouteRepository : IRouteRepository
    {
        /// <summary>
        /// The DataContext for database interactions.
        /// </summary>
        private readonly DataContext _context;
        /// <summary>
        /// Initializes a new instance of the RouteRepository class.
        /// </summary>
        /// <param name="context">The DataContext to use for database operations.</param>
        public RouteRepository(DataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Creates necessary constraints in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CreateConstraintsAsync()
        {
            // Ensure the uniqueness of the Route Id property
            var query = @"
                CREATE CONSTRAINT route_id_unique IF NOT EXISTS
                FOR (r:Route)
                REQUIRE r.Id IS UNIQUE
            ";
            // Create the constraint in the database
            using var session = _context.GetSession();
            // Execute the query in a write transaction
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query);
            });
        }
        /// <summary>
        /// Creates a new route in the database.
        /// </summary>
        /// <param name="route"></param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateRouteAsync(Route route)
        {
            // Cypher query to create a new Route node
            var query = @"
                CREATE (r:Route {
                    Id: $Id,
                    OriginStation: $OriginStation,
                    DestinationStation: $DestinationStation,
                    DepartureTime: $DepartureTime,
                    ArrivalTime: $ArrivalTime,
                    InterludeTimes: $InterludeTimes,
                    IsActive: $IsActive
                })
                RETURN r
            ";
            try
            {
                // Create the route in the database
                using var session = _context.GetSession();
                await session.ExecuteWriteAsync(async tx =>
                {
                    // Execute the query with parameters
                    await tx.RunAsync(query, new
                    {
                        // Ensures the Id is stored as a string because Neo4j does not support Guid natively
                        Id = route.Id.ToString(),
                        route.OriginStation,
                        route.DestinationStation,
                        // Store TimeSpan as string in "HH:mm" format
                        DepartureTime = route.DepartureTime.Hours + ":" + route.DepartureTime.Minutes,
                        ArrivalTime = route.ArrivalTime.Hours + ":" + route.ArrivalTime.Minutes,
                        InterludeTimes = route.InterludeTimes.Select(t => t.Hours + ":" + t.Minutes).ToList(),
                        route.IsActive
                    });
                });
            }
            // Handle exceptions, such as constraint violations
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    Console.WriteLine($"Route with Id {route.Id} already exists.");
                    return;
                }
                throw;
            }
        }
        /// <summary>
        /// Retrieves all routes from the database.
        /// </summary>
        /// <returns>A list of all Route entities.</returns>
        public async Task<List<Route>> GetAllRoutesAsync()
        {
            // Cypher query to retrieve all Route nodes
            var query = @"MATCH (r:Route) RETURN r";
            try
            {
                // Retrieve the routes from the database
                using var session = _context.GetSession();
                return await session.ExecuteReadAsync(async tx =>
                {
                    // Execute the query
                    var cursor = await tx.RunAsync(query);
                    var records = await cursor.ToListAsync();
                    // Map the records to Route entities
                    var routes = new List<Route>();
                    // Convert each record to a Route object
                    foreach (var record in records)
                    {
                        var node = record["r"].As<INode>();
                        var route = new Route
                        {
                            // Parse the Id back to Guid
                            Id = Guid.Parse(node.Properties["Id"].As<string>()),
                            OriginStation = node.Properties["OriginStation"].As<string>(),
                            DestinationStation = node.Properties["DestinationStation"].As<string>(),
                            // Parse the Departure and Arrival times back to TimeSpan
                            DepartureTime = TimeSpan.Parse(node.Properties["DepartureTime"].As<string>()),
                            ArrivalTime = TimeSpan.Parse(node.Properties["ArrivalTime"].As<string>()),
                            InterludeTimes = node.Properties.ContainsKey("InterludeTimes")
                                ? node.Properties["InterludeTimes"].As<List<object>>()
                                    .Select(x => TimeSpan.Parse(x.ToString()!))
                                    .ToList()
                                : new List<TimeSpan>(),
                            IsActive = node.Properties["IsActive"].As<bool>()
                        };
                        // Add the route to the list
                        routes.Add(route);
                    }
                    // Return the list of routes
                    return routes;
                });
            }
            // Handle exceptions, such as no routes found
            catch (Exception ex)
            {
                if (ex.Message.Contains("No such label"))
                {
                    Console.WriteLine("No routes found in the database.");
                    return new List<Route>();
                }
                throw;
            }
        }
        /// <summary>
        /// Retrieves a route by its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Route entity if found; otherwise, null.</returns>
        public async Task<Route?> GetRouteByIdAsync(Guid id)
        {
            // Cypher query to retrieve a Route node by Id
            var query = @"
                MATCH (r:Route { Id: $id })
                RETURN r
            ";
            try
            {
                // Retrieve the route from the database
                using var session = _context.GetSession();
                return await session.ExecuteReadAsync(async tx =>
                {
                    // Execute the query
                    var cursor = await tx.RunAsync(query, new { id = id.ToString() });
                    var records = await cursor.ToListAsync();

                    // If no records found, return null
                    if (records.Count == 0) return null;

                    // Convert the record to a Route object
                    var node = records[0]["r"].As<INode>();
                    if (node.Properties["IsActive"].As<bool>() == false) return null;
                    if (node == null) return null;

                    // Map the node properties to a Route object
                    return new Route
                    {
                        // Parse the Id back to Guid
                        Id = Guid.Parse(node.Properties["Id"].As<string>()),
                        OriginStation = node.Properties["OriginStation"].As<string>(),
                        DestinationStation = node.Properties["DestinationStation"].As<string>(),
                        // Parse the Departure and Arrival times back to TimeSpan
                        DepartureTime = TimeSpan.Parse(node.Properties["DepartureTime"].As<string>()),
                        ArrivalTime = TimeSpan.Parse(node.Properties["ArrivalTime"].As<string>()),
                        InterludeTimes = node.Properties.ContainsKey("InterludeTimes")
                            ? node.Properties["InterludeTimes"].As<List<object>>()
                                .Select(x => TimeSpan.Parse(x.ToString()!))
                                .ToList()
                            : new List<TimeSpan>(),
                        IsActive = node.Properties["IsActive"].As<bool>()
                    };
                });
            }
            // Handle exceptions, such as no route found
            catch (Exception ex)
            {
                if (ex.Message.Contains("No such label"))
                {
                    Console.WriteLine($"No route found with Id {id}.");
                    return null;
                }
                throw;
            }
        }
        /// <summary>
        /// Updates an existing route in the database.
        /// </summary>
        /// <param name="route"></param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateRouteAsync(Route route)
        {
            // Cypher query to update an existing Route node
            var query = @"
                MATCH (r:Route { Id: $Id })
                SET r.OriginStation = $OriginStation,
                    r.DestinationStation = $DestinationStation,
                    r.DepartureTime = $DepartureTime,
                    r.ArrivalTime = $ArrivalTime,
                    r.InterludeTimes = $InterludeTimes,
                    r.IsActive = $IsActive
                RETURN r
            ";
            try
            {
                // Get a session for database interaction
                using var session = _context.GetSession();
                // Update the route in the database
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, new
                    {
                        // Ensure the Id is stored as a string
                        Id = route.Id.ToString(),
                        route.OriginStation,
                        route.DestinationStation,
                        // Store TimeSpan as string in "HH:mm" format
                        DepartureTime = route.DepartureTime.Hours + ":" + route.DepartureTime.Minutes,
                        ArrivalTime = route.ArrivalTime.Hours + ":" + route.ArrivalTime.Minutes,
                        InterludeTimes = route.InterludeTimes.Select(t => t.Hours + ":" + t.Minutes).ToList(),
                        route.IsActive
                    });
                });
            }
            // Handle exceptions, such as route not found
            catch (Exception ex)
            {
                if (ex.Message.Contains("No such label"))
                {
                    Console.WriteLine($"No route found with Id {route.Id}.");
                    return;
                }
                throw;
            }
        }
        /// <summary>
        /// Deletes (deactivates) a route by its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteRouteAsync(Guid id)
        {
            // Cypher query to "delete" a Route node by setting IsActive to false (Soft Delete)
            var query = @"
                MATCH (r:Route { Id: $id })
                SET r.IsActive = $IsActive
                RETURN r
            ";
            try
            {
                // Get a session for database interaction
                using var session = _context.GetSession();
                // "Delete" the route by setting IsActive to false
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, new { id = id.ToString(), IsActive = false });
                });
            }
            // Handle exceptions, such as route not found
            catch (Exception ex)
            {
                if (ex.Message.Contains("No such label"))
                {
                    Console.WriteLine($"No route found with Id {id}.");
                    return;
                }
                throw;
            }
        }
        /// <summary>
        /// Checks if any route exists with the given station name as either origin or destination.
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns>True if such a route exists; otherwise, false.</returns>
        public async Task<bool> ExistsStationAsync(string stationName)
        {
            // Cypher query to check if any Route node exists with the given station name
            var query = @"
                MATCH (r:Route)
                WHERE r.OriginStation = $stationName OR r.DestinationStation = $stationName
                RETURN COUNT(r) > 0 AS stationExists
            ";
            // Get a session for database interaction
            using var session = _context.GetSession();
            // Execute the query and return the result
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { stationName });
                var record = await cursor.SingleAsync();
                return record["stationExists"].As<bool>();
            });
        }
        /// <summary>
        /// Disposes the DataContext when the repository is disposed.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
        }
        /// <summary>
        /// Creates necessary constraints in the database.
        /// </summary>
        Task IRouteRepository.CreateConstraintsAsync()
        {
            return CreateConstraintsAsync();
        }
    }
}