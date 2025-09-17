using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace perla_metro_route_service.src.Models
{
    public class Route
    {
        /// <summary>
        /// Unique identifier for the route using UUID V4.
        /// </summary>
        public required Guid Id { get; set; }
        /// <summary>
        /// The station where the route begins.
        /// </summary>
        public required string OriginStation { get; set; }
        /// <summary>
        /// The station where the route ends.
        /// </summary>
        public required string DestinationStation { get; set; }
        /// <summary>
        /// The time the route departs from the origin station.
        /// </summary>
        public required TimeSpan DepartureTime { get; set; }
        /// <summary>
        /// The time the route arrives at the destination station.
        /// </summary>
        public required TimeSpan ArrivalTime { get; set; }
        /// <summary>
        /// List of interlude times at intermediate stations.
        /// </summary>
        public required List<TimeSpan> InterludeTimes { get; set; } = new List<TimeSpan>();
        /// <summary>
        /// Indicates whether the route is currently active. If inactive, it should not be considered for scheduling.
        /// </summary>
        public required bool IsActive { get; set; } = true;
    }
}