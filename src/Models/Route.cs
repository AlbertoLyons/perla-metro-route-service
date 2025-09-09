using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace perla_metro_route_service.src.Models
{
    public class Route
    {
        public string Id { get; set; } = "";
        public string OriginStation { get; set; } = "";
        public string DestinationStation { get; set; } = "";
        public DateTime DepartureTime { get; set; } = DateTime.MinValue;
        public DateTime ArrivalTime { get; set; } = DateTime.MinValue;
        public List<DateTime> interludeTimes { get; set; } = new List<DateTime>();
        public bool IsActive { get; set; } = true;
    }
}