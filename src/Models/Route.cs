using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace perla_metro_route_service.src.Models
{
    public class Route
    {
        public required string Id { get; set; }
        public required string OriginStation { get; set; }
        public required string DestinationStation { get; set; }
        public required DateTime DepartureTime { get; set; }
        public required DateTime ArrivalTime { get; set; }
        public required List<DateTime> InterludeTimes { get; set; } = new List<DateTime>();
        public required bool IsActive { get; set; } = true;
    }
}