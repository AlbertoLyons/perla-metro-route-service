using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace perla_metro_route_service.src.DTO
{
    /// <summary>
    /// Data Transfer Object for creating a new route.
    /// </summary>
    public class CreateRoute
    {
        /// <summary>
        /// Gets or sets the origin station of the route.
        /// </summary>
        public required string OriginStation { get; set; }
        /// <summary>
        /// Gets or sets the destination station of the route.
        /// </summary>
        public required string DestinationStation { get; set; }
        /// <summary>
        /// Gets or sets the departure time of the route.
        /// </summary>
        public required TimeSpan DepartureTime { get; set; }
        /// <summary>
        /// Gets or sets the arrival time of the route.
        /// </summary>
        public required TimeSpan ArrivalTime { get; set; }
        /// <summary>
        /// Gets or sets the list of interlude times for the route.
        /// </summary>
        public required List<TimeSpan> InterludeTimes { get; set; } = new List<TimeSpan>();
        /// <summary>
        /// Gets or sets a value indicating whether the route is active.
        /// </summary>
        public required bool IsActive { get; set; }
    }
}