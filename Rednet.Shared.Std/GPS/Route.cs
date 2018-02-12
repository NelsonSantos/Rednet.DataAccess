using System.Collections.Generic;

namespace Rednet.Shared.GPS
{
    public class Route
    {
        public Bounds Bounds { get; set; }
        public string Copyrights { get; set; }
        public List<Leg> Legs { get; set; }
        public OverviewPolyline OverviewPolyline { get; set; }
        public string Summary { get; set; }
        public List<string> Warnings { get; set; }
        public List<object> WaypointOrder { get; set; }
    }
}