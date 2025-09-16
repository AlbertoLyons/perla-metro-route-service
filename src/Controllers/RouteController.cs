using Microsoft.AspNetCore.Mvc;
using perla_metro_route_service.src.DTO;
using perla_metro_route_service.src.Interfaces;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteRepository _routeRepository;

        public RouteController(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                var routes = await _routeRepository.GetAllRoutesAsync();
                return Ok(routes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRoute createRoute)
        {
            try
            {
                var route = new Route
                {
                    Id = Guid.NewGuid(),
                    OriginStation = createRoute.OriginStation,
                    DestinationStation = createRoute.DestinationStation,
                    DepartureTime = createRoute.DepartureTime,
                    ArrivalTime = createRoute.ArrivalTime,
                    InterludeTimes = createRoute.InterludeTimes,
                    IsActive = createRoute.IsActive
                };
                await _routeRepository.CreateRouteAsync(route);
                return Ok(route);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(Guid id)
        {
            try
            {
                var route = await _routeRepository.GetRouteByIdAsync(id);
                if (route == null)
                {
                    return NotFound();
                }
                return Ok(route);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}