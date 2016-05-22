using System.Collections.Generic;

namespace Rednet.Shared.GPS
{
    public class Leg
    {
        public Distance Distance { get; set; }
        public Duration Duration { get; set; }
        public string EndAddress { get; set; }
        public Location EndLocation { get; set; }
        public string StartAddress { get; set; }
        public Location StartLocation { get; set; }
        public List<Step> Steps { get; set; }
        public List<object> ViaWaypoint { get; set; }
    }
}