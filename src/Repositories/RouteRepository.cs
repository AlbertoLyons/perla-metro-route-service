using Neo4j.Driver;
using perla_metro_route_service.src.Interfaces;
using perla_metro_route_service.src.data;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly DataContext _context;
        public RouteRepository(DataContext context)
        {
            _context = context;
        }
        private async Task CreateConstraintsAsync()
        {
            var query = @"
                CREATE CONSTRAINT route_id_unique IF NOT EXISTS
                FOR (r:Route)
                REQUIRE r.Id IS UNIQUE
            ";
            using var session = _context.GetSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query);
            });
        }

        public async Task CreateRouteAsync(Route route)
        {
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
                using var session = _context.GetSession();
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, new
                    {
                        Id = route.Id.ToString(),
                        route.OriginStation,
                        route.DestinationStation,
                        DepartureTime = route.DepartureTime.Hours + ":" + route.DepartureTime.Minutes,
                        ArrivalTime = route.ArrivalTime.Hours + ":" + route.ArrivalTime.Minutes,
                        InterludeTimes = route.InterludeTimes.Select(t => t.Hours + ":" + t.Minutes).ToList(),
                        route.IsActive
                    });
                });
            }
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

        public async Task<List<Route>> GetAllRoutesAsync()
        {
            var query = @"MATCH (r:Route) RETURN r";
            try
            {
                using var session = _context.GetSession();
                return await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query);
                    var records = await cursor.ToListAsync();

                    var routes = new List<Route>();

                    foreach (var record in records)
                    {
                        var node = record["r"].As<INode>();

                        var route = new Route
                        {
                            Id = Guid.Parse(node.Properties["Id"].As<string>()),
                            OriginStation = node.Properties["OriginStation"].As<string>(),
                            DestinationStation = node.Properties["DestinationStation"].As<string>(),
                            DepartureTime = TimeSpan.Parse(node.Properties["DepartureTime"].As<string>()),
                            ArrivalTime = TimeSpan.Parse(node.Properties["ArrivalTime"].As<string>()),
                            InterludeTimes = node.Properties.ContainsKey("InterludeTimes")
                                ? node.Properties["InterludeTimes"].As<List<object>>()
                                    .Select(x => TimeSpan.Parse(x.ToString()!))
                                    .ToList()
                                : new List<TimeSpan>(),
                            IsActive = node.Properties["IsActive"].As<bool>()
                        };
                        routes.Add(route);
                    }
                    return routes;
                });
            }
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

        public async Task<Route?> GetRouteByIdAsync(Guid id)
        {
            var query = @"
                MATCH (r:Route { Id: $id })
                RETURN r
            ";

            try
            {
                using var session = _context.GetSession();
                return await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, new { id = id.ToString() });
                    var records = await cursor.ToListAsync();

                    if (records.Count == 0) return null;

                    var node = records[0]["r"].As<INode>();

                    return new Route
                    {
                        Id = Guid.Parse(node.Properties["Id"].As<string>()),
                        OriginStation = node.Properties["OriginStation"].As<string>(),
                        DestinationStation = node.Properties["DestinationStation"].As<string>(),
                        DepartureTime = TimeSpan.Parse(node.Properties["DepartureTime"].As<string>()),
                        ArrivalTime = TimeSpan.Parse(node.Properties["ArrivalTime"].As<string>()),
                        InterludeTimes = node.Properties["InterludeTimes"].As<List<object>>()
                            .Select(x => TimeSpan.Parse(x.ToString()!))
                            .ToList(),
                        IsActive = node.Properties["IsActive"].As<bool>()
                    };
                });
            } catch (Exception ex)
            {
                if (ex.Message.Contains("No such label"))
                {
                    Console.WriteLine($"No route found with Id {id}.");
                    return null;
                }
                throw;
            }
        }

        public async Task UpdateRouteAsync(Route route)
        {
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

                using var session = _context.GetSession();
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, new
                    {
                        route.Id,
                        route.OriginStation,
                        route.DestinationStation,
                        DepartureTime = route.DepartureTime.ToString("o"),
                        ArrivalTime = route.ArrivalTime.ToString("o"),
                        InterludeTimes = route.InterludeTimes.Select(t => t.ToString("o")).ToList(),
                        route.IsActive
                    });
                });
            } catch (Exception ex) {
                if (ex.Message.Contains("No such label"))
                {
                    Console.WriteLine($"No route found with Id {route.Id}.");
                    return;
                }
                throw;
            }
        }
        public async Task DeleteRouteAsync(Guid id)
        {
            var query = @"
                MATCH (r:Route { Id: $id })
                SET r.IsActive = $IsActive
                RETURN r
            ";

            try
            {

                using var session = _context.GetSession();
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, new { id = id.ToString(), IsActive = false });
                });
            }
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
        public void Dispose()
        {
            _context?.Dispose();
        }

        Task IRouteRepository.CreateConstraintsAsync()
        {
            return CreateConstraintsAsync();
        }
    }
}