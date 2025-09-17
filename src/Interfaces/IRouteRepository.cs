using perla_metro_route_service.src.DTO;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Interfaces
{
    public interface IRouteRepository
    {
        Task CreateConstraintsAsync();
        Task CreateRouteAsync(Route route);
        Task<List<Route>> GetAllRoutesAsync();
        Task<Route?> GetRouteByIdAsync(Guid id);
        Task UpdateRouteAsync(Route route);
        Task DeleteRouteAsync(Guid id);
        Task<bool> ExistsStationAsync(string stationName);
    }
}