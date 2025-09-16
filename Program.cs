using DotNetEnv;
using perla_metro_route_service.src.data;
using perla_metro_route_service.src.Interfaces;
using perla_metro_route_service.src.repositories;
using perla_metro_route_service.src.Data;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IRouteRepository, RouteRepository>(sp =>
{
    var context = sp.GetRequiredService<DataContext>();
    return new RouteRepository(context);
});

builder.Services.AddSingleton(sp =>
    new DataContext(
        Environment.GetEnvironmentVariable("NEO4J_URI") ?? "neo4j://localhost:7687",
        Environment.GetEnvironmentVariable("NEO4J_USER") ?? "neo4j",
        Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "12345678"
    )
);


builder.Services.AddScoped<DataSeeder>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyHost", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    // Seed initial data
    await DataSeeder.Initialize(services);
}

app.UseCors("AllowAnyHost");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
