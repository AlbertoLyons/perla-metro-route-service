using perla_metro_route_service.src.DTO;
using Route = perla_metro_route_service.src.Models.Route;

namespace perla_metro_route_service.src.Interfaces
{
    /// <summary>
    /// Interface for route repository operations.
    /// </summary>
    public interface IRouteRepository
    {
        /// <summary>
        /// Creates the necessary constraints for the route repository.
        /// </summary>
        Task CreateConstraintsAsync();
        /// <summary>
        /// Creates a new route.
        /// </summary>
        /// <param name="route">The route to create.</param>
        Task CreateRouteAsync(Route route);
        /// <summary>
        /// Retrieves all routes.
        /// </summary>
        Task<List<Route>> GetAllRoutesAsync();
        /// <summary>
        /// Retrieves a route by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier in UUID V4 format of the route.</param>
        Task<Route?> GetRouteByIdAsync(Guid id);
        /// <summary>
        /// Updates an existing route.
        /// </summary>
        /// <param name="route">The route to update.</param>
        Task UpdateRouteAsync(Route route);
        /// <summary>
        /// Deletes a route by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier in UUID V4 format of the route.</param>
        Task DeleteRouteAsync(Guid id);
        /// <summary>
        /// Verifies if the station of a route exists.
        /// </summary>
        /// <param name="stationName">The name of the station to check.</param>
        /// <returns>True if the station exists, otherwise false.</returns>
        Task<bool> ExistsStationAsync(string stationName);
    }
}