using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using perla_metro_route_service.src.DTO;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Mappers
{
    /// <summary>
    /// Maps Route objects to and from various DTOs.
    /// </summary>
    public static class RouteMapper
    {
        /// <summary>
        /// Converts a CreateRoute DTO to a Route model.
        /// </summary>
        /// <param name="route">The CreateRoute DTO to convert.</param>
        /// <returns>A new Route model.</returns>
        public static Route CreatedDTOToRoute(this CreateRoute route)
        {
            return new Route
            {
                Id = Guid.NewGuid(),
                OriginStation = route.OriginStation,
                DestinationStation = route.DestinationStation,
                DepartureTime = route.DepartureTime,
                ArrivalTime = route.ArrivalTime,
                InterludeTimes = route.InterludeTimes,
                IsActive = route.IsActive
            };
        }
        /// <summary>
        /// Converts a Route model to a GetRouteById DTO.
        /// </summary>
        /// <param name="route">The Route model to convert.</param>
        public static GetRouteById RouteToGetByIdDTO(this Route route)
        {
            return new GetRouteById
            {
                Id = route.Id,
                OriginStation = route.OriginStation,
                DestinationStation = route.DestinationStation,
                DepartureTime = route.DepartureTime,
                ArrivalTime = route.ArrivalTime,
            };
        }
        /// <summary>
        /// Updates an existing Route model with data from an UpdateRoute DTO.
        /// </summary>
        /// <param name="route">The existing Route model to update.</param>
        /// <param name="updateRoute">The UpdateRoute DTO containing updated data.</param>
        /// <returns>An updated Route model.</returns>
        public static Route UpdatedToRoute(this Route route, UpdateRoute updateRoute)
        {
            return new Route
            {
                Id = route.Id,
                OriginStation = updateRoute.OriginStation,
                DestinationStation = updateRoute.DestinationStation,
                DepartureTime = updateRoute.DepartureTime,
                ArrivalTime = updateRoute.ArrivalTime,
                InterludeTimes = updateRoute.InterludeTimes,
                IsActive = route.IsActive
            };
        }
    }
}