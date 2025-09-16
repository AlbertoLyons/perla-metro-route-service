using Bogus;
using perla_metro_route_service.src.Interfaces;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Data
{
    public class DataSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRouteRepository>();

            await repository.CreateConstraintsAsync();


            var routes = await repository.GetAllRoutesAsync();
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

                var routesF = routeFaker.Generate(100);

                foreach (var route in routesF)
                {
                    await repository.CreateRouteAsync(route);
                }
            }
        }
    }
}
