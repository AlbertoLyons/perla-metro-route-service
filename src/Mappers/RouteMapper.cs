using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using perla_metro_route_service.src.DTO;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Mappers
{
    public static class RouteMapper
    {
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