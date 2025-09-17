using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using perla_metro_route_service.src.DTO;
using perla_metro_route_service.src.Interfaces;
using perla_metro_route_service.src.Mappers;
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
                if (await _routeRepository.ExistsStationAsync(createRoute.OriginStation) ||
                    await _routeRepository.ExistsStationAsync(createRoute.DestinationStation))
                {
                    return BadRequest("One or both stations exists. Please choose different stations.");
                }

                if (createRoute.OriginStation == createRoute.DestinationStation)
                {
                    return BadRequest("Origin and destination stations cannot be the same.");
                }

                if (createRoute.DepartureTime >= createRoute.ArrivalTime)
                {
                    return BadRequest("Departure time must be earlier than arrival time.");
                }

                foreach (var interlude in createRoute.InterludeTimes)
                {
                    if (interlude <= createRoute.DepartureTime || interlude >= createRoute.ArrivalTime)
                    {
                        return BadRequest("Interlude times must be between departure and arrival times.");
                    }
                }

                var route = createRoute.CreatedDTOToRoute();
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
                var routeDTO = route.RouteToGetByIdDTO();
                return Ok(routeDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] UpdateRoute updateRoute)
        {
            try
            {
                var existingRoute = await _routeRepository.GetRouteByIdAsync(id);
                if (existingRoute == null)
                {
                    return NotFound();
                }
                if (await _routeRepository.ExistsStationAsync(updateRoute.OriginStation) ||
                    await _routeRepository.ExistsStationAsync(updateRoute.DestinationStation))
                {
                    return BadRequest("One or both stations exists. Please choose different stations.");
                }

                if (updateRoute.OriginStation == updateRoute.DestinationStation)
                {
                    return BadRequest("Origin and destination stations cannot be the same.");
                }

                if (updateRoute.DepartureTime >= updateRoute.ArrivalTime)
                {
                    return BadRequest("Departure time must be earlier than arrival time.");
                }

                foreach (var interlude in updateRoute.InterludeTimes)
                {
                    if (interlude <= updateRoute.DepartureTime || interlude >= updateRoute.ArrivalTime)
                    {
                        return BadRequest("Interlude times must be between departure and arrival times.");
                    }
                }

                var updatedRoute = existingRoute.UpdatedToRoute(updateRoute);
                await _routeRepository.UpdateRouteAsync(updatedRoute);
                return Ok(updatedRoute);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            try
            {
                var existingRoute = await _routeRepository.GetRouteByIdAsync(id);
                if (existingRoute == null)
                {
                    return NotFound();
                }
                await _routeRepository.DeleteRouteAsync(id);
                return Ok("Route deleted successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}