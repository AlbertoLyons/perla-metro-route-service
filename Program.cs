using DotNetEnv;
using perla_metro_route_service.src.data;
using perla_metro_route_service.src.Interfaces;
using perla_metro_route_service.src.repositories;
using perla_metro_route_service.src.Data;

// Load environment variables from .env file
Env.Load();
// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);
// Add controllers and OpenAPI services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Register the RouteRepository as a singleton service
builder.Services.AddSingleton<IRouteRepository, RouteRepository>(sp =>
{
    var context = sp.GetRequiredService<DataContext>();
    return new RouteRepository(context);
});
// Register the DataContext as a singleton service with configuration from environment variables
builder.Services.AddSingleton(sp =>
new DataContext(
    Environment.GetEnvironmentVariable("NEO4J_URI") ?? "neo4j://localhost:7687",
    Environment.GetEnvironmentVariable("NEO4J_USER") ?? "neo4j",
    Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "12345678"
)
);
// Register the DataSeeder as a scoped service
builder.Services.AddScoped<DataSeeder>();
// Configure CORS to allow any origin, header, and method
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyHost", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Build the web application
var app = builder.Build();
// Configure the HTTP request pipeline for development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// Create a scope to seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    // Seed initial data
    await DataSeeder.Initialize(services);
}
// Use CORS policy, HTTPS redirection, and authorization middleware
app.UseCors("AllowAnyHost");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
