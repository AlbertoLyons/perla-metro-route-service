using Bogus;
using perla_metro_route_service.src.Interfaces;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Data
{
    /// <summary>
    /// Class responsible for seeding initial data into the database.
    /// </summary>
    public class DataSeeder
    {
        /// <summary>
        /// Initializes the database with seed data if it is empty.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency injection.</param>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Create the database schema
            using var scope = serviceProvider.CreateScope();
            // Get the route repository from the service provider
            var repository = scope.ServiceProvider.GetRequiredService<IRouteRepository>();
            // Create database constraints
            await repository.CreateConstraintsAsync();
            // Check if there are any existing routes
            var routes = await repository.GetAllRoutesAsync();
            // If no routes exist, seed the database with fake data
            if (routes == null || routes.Count == 0)
            {
                var routeFaker = new Faker<Route>()
                    .RuleFor(r => r.Id, (f, r) => Guid.NewGuid())
                    .RuleFor(r => r.OriginStation, f => f.Address.City())
                    .RuleFor(r => r.DestinationStation, f => f.Address.City())
                    .RuleFor(r => r.DepartureTime, f => f.Date.Future().TimeOfDay)
                    .RuleFor(r => r.ArrivalTime, f => f.Date.Future().TimeOfDay + TimeSpan.FromHours(f.Random.Int(1, 5)))
                    .RuleFor(r => r.InterludeTimes, f => f.Make(3, () => f.Date.Future().TimeOfDay).ToList())
                    .RuleFor(r => r.IsActive, f => f.Random.Bool());
                // Generate 100 fake routes
                var routesF = routeFaker.Generate(100);
                // Save each generated route to the repository
                foreach (var route in routesF)
                {
                    await repository.CreateRouteAsync(route);
                }
            }
        }
    }
}
