using Neo4j.Driver;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Data
{
    public class DataContext : IDisposable
    {
        private readonly IDriver _driver;
        public DataContext(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
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
            using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new
                {
                    route.Id,
                    route.OriginStation,
                    route.DestinationStation,
                    DepartureTime = route.DepartureTime.ToString("o"),
                    ArrivalTime = route.ArrivalTime.ToString("o"),
                    InterludeTimes = route.interludeTimes.Select(t => t.ToString("o")).ToList(),
                    route.IsActive
                });
            });
        }

        public async Task<List<Route>> GetAllRoutesAsync()
        {
            var query = @"MATCH (r:Route) RETURN r";

            using var session = _driver.AsyncSession();
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
                        Id = node.Properties["Id"].As<string>(),
                        OriginStation = node.Properties["OriginStation"].As<string>(),
                        DestinationStation = node.Properties["DestinationStation"].As<string>(),
                        DepartureTime = DateTime.Parse(node.Properties["DepartureTime"].As<string>()),
                        ArrivalTime = DateTime.Parse(node.Properties["ArrivalTime"].As<string>()),
                        interludeTimes = node.Properties.ContainsKey("InterludeTimes")
                            ? node.Properties["InterludeTimes"].As<List<object>>()
                                .Select(x => DateTime.Parse(x.ToString()!))
                                .ToList()
                            : new List<DateTime>(),
                        IsActive = node.Properties["IsActive"].As<bool>()
                    };
                    routes.Add(route);
                }
                return routes;
            });
        }

        public async Task<Route?> GetRouteByIdAsync(string id)
        {
            var query = @"
                MATCH (r:Route { Id: $id })
                RETURN r
            ";

            using var session = _driver.AsyncSession();
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, new { id });
                var records = await cursor.ToListAsync();

                if (records.Count == 0) return null;

                var node = records[0]["r"].As<INode>();

                return new Route
                {
                    Id = node.Properties["Id"].As<string>(),
                    OriginStation = node.Properties["OriginStation"].As<string>(),
                    DestinationStation = node.Properties["DestinationStation"].As<string>(),
                    DepartureTime = DateTime.Parse(node.Properties["DepartureTime"].As<string>()),
                    ArrivalTime = DateTime.Parse(node.Properties["ArrivalTime"].As<string>()),
                    interludeTimes = node.Properties["InterludeTimes"].As<List<object>>()
                        .Select(x => DateTime.Parse(x.ToString()!))
                        .ToList(),
                    IsActive = node.Properties["IsActive"].As<bool>()
                };
            });
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

            using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new
                {
                    route.Id,
                    route.OriginStation,
                    route.DestinationStation,
                    DepartureTime = route.DepartureTime.ToString("o"),
                    ArrivalTime = route.ArrivalTime.ToString("o"),
                    InterludeTimes = route.interludeTimes.Select(t => t.ToString("o")).ToList(),
                    route.IsActive
                });
            });
        }
        public async Task DeleteRouteAsync(string id)
        {
            var query = @"
                MATCH (r:Route { Id: $id })
                SET r.IsActive = $IsActive
                RETURN r
            ";

            using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { id });
            });
        }
        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}