using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace perla_metro_route_service.src.DTO
{
    public class CreateRoute
    {
        public required string OriginStation { get; set; }
        public required string DestinationStation { get; set; }
        public required TimeSpan DepartureTime { get; set; }
        public required TimeSpan ArrivalTime { get; set; }
        public required List<TimeSpan> InterludeTimes { get; set; } = new List<TimeSpan>();
    }
}