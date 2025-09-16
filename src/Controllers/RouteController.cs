using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using perla_metro_route_service.src.Interfaces;

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
    }
}