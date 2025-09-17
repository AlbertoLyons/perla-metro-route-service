using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using perla_metro_route_service.src.DTO;
using perla_metro_route_service.src.Interfaces;
using perla_metro_route_service.src.Mappers;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Controllers
{
    /// <summary>
    /// Controller for managing perla metro routes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        /// <summary>
        /// Repository for accessing route data.
        /// </summary>
        private readonly IRouteRepository _routeRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteController"/> class.
        /// </summary>
        /// <param name="routeRepository">The route repository.</param>
        public RouteController(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }
        /// <summary>
        /// Gets all routes, including if is active or not. This endpoint is used for admin purposes.
        /// </summary>
        /// <returns>A list of all routes.</returns>
        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                // Fetch all routes from the repository
                var routes = await _routeRepository.GetAllRoutesAsync();
                // Return the list of routes with a 200 OK status
                return Ok(routes);
            }
            // Handle any exceptions that may occur
            catch (Exception e)
            {
                // Return a 400 Bad Request status with the exception message
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Creates a new route by the specific DTO.
        /// </summary>
        /// <param name="createRoute"></param>
        /// <returns>The created route</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRoute createRoute)
        {
            try
            {
                // Check if either the origin or destination station already exists
                if (await _routeRepository.ExistsStationAsync(createRoute.OriginStation) ||
                    await _routeRepository.ExistsStationAsync(createRoute.DestinationStation))
                {
                    return BadRequest("One or both stations exists. Please choose different stations.");
                }
                // Check if origin and destination stations are the same
                if (createRoute.OriginStation == createRoute.DestinationStation)
                {
                    return BadRequest("Origin and destination stations cannot be the same.");
                }
                // Check if departure time is earlier than arrival time
                if (createRoute.DepartureTime >= createRoute.ArrivalTime)
                {
                    return BadRequest("Departure time must be earlier than arrival time.");
                }
                // Check if interlude times are between departure and arrival times
                foreach (var interlude in createRoute.InterludeTimes)
                {
                    if (interlude <= createRoute.DepartureTime || interlude >= createRoute.ArrivalTime)
                    {
                        return BadRequest("Interlude times must be between departure and arrival times.");
                    }
                }
                // Map the CreateRoute DTO to a Route entity
                var route = createRoute.CreatedDTOToRoute();
                // Save the new route to the repository
                await _routeRepository.CreateRouteAsync(route);
                // Return the created route with a 200 OK status
                return Ok(route);
            }
            // Handle any exceptions that may occur
            catch (Exception e)
            {
                // Return a 400 Bad Request status with the exception message
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Gets a route by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier in UUID V4 format of the route.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(Guid id)
        {
            try
            {
                // Fetch the route by its ID from the repository
                var route = await _routeRepository.GetRouteByIdAsync(id);
                // If the route is not found, return a 404 Not Found status
                if (route == null)
                {
                    return NotFound();
                }
                // Map the Route entity to a RouteDTO
                var routeDTO = route.RouteToGetByIdDTO();
                // Return the route DTO with a 200 OK status
                return Ok(routeDTO);
            }
            // Handle any exceptions that may occur
            catch (Exception e)
            {
                // Return a 400 Bad Request status with the exception message
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Updates an existing route by its unique identifier and the update params.
        /// </summary>
        /// <param name="id">The unique identifier in UUID V4 format of the route to update.</param>
        /// <param name="updateRoute">The updated route data.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] UpdateRoute updateRoute)
        {
            try
            {
                // Fetch the existing route by its ID from the repository
                var existingRoute = await _routeRepository.GetRouteByIdAsync(id);
                // If the route is not found, return a 404 Not Found status
                if (existingRoute == null)
                {
                    return NotFound();
                }
                // Check if either the origin or destination station already exists
                if (await _routeRepository.ExistsStationAsync(updateRoute.OriginStation) ||
                    await _routeRepository.ExistsStationAsync(updateRoute.DestinationStation))
                {
                    return BadRequest("One or both stations exists. Please choose different stations.");
                }
                // Check if origin and destination stations are the same
                if (updateRoute.OriginStation == updateRoute.DestinationStation)
                {
                    return BadRequest("Origin and destination stations cannot be the same.");
                }
                // Check if departure time is earlier than arrival time
                if (updateRoute.DepartureTime >= updateRoute.ArrivalTime)
                {
                    return BadRequest("Departure time must be earlier than arrival time.");
                }
                // Check if interlude times are between departure and arrival times
                foreach (var interlude in updateRoute.InterludeTimes)
                {
                    if (interlude <= updateRoute.DepartureTime || interlude >= updateRoute.ArrivalTime)
                    {
                        return BadRequest("Interlude times must be between departure and arrival times.");
                    }
                }
                // Map the UpdateRoute DTO to the existing Route entity
                var updatedRoute = existingRoute.UpdatedToRoute(updateRoute);
                // Save the updated route to the repository
                await _routeRepository.UpdateRouteAsync(updatedRoute);
                // Return the updated route with a 200 OK status
                return Ok(updatedRoute);
            }
            // Handle any exceptions that may occur
            catch (Exception e)
            {
                // Return a 400 Bad Request status with the exception message
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// Deletes a route with Soft Delete by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier in UUID V4 format of the route to delete.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            try
            {
                // Fetch the existing route by its ID from the repository
                var existingRoute = await _routeRepository.GetRouteByIdAsync(id);
                // If the route is not found, return a 404 Not Found status
                if (existingRoute == null)
                {
                    return NotFound();
                }
                // Delete the route using soft delete by only setting IsActive to false
                await _routeRepository.DeleteRouteAsync(id);
                // Return a success message with a 200 OK status
                return Ok("Route deleted successfully.");
            }
            // Handle any exceptions that may occur
            catch (Exception e)
            {
                // Return a 400 Bad Request status with the exception message
                return BadRequest(e.Message);
            }
        }
    }
}