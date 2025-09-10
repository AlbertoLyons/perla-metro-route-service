using perla_metro_route_service.src.Data;
using Route = perla_metro_route_service.src.Models.Route;

DataContext dataContext = new DataContext("neo4j://localhost:7687", "neo4j", "12345678");

await dataContext.DeleteRouteAsync("828");

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
