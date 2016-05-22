namespace Rednet.Shared.GPS
{
    public class Step
    {
        public Distance Distance { get; set; }
        public Duration Duration { get; set; }
        public Location EndLocation { get; set; }
        public string HtmlInstructions { get; set; }
        public Polyline Polyline { get; set; }
        public Location StartLocation { get; set; }
        public string TravelMode { get; set; }
        public string Maneuver { get; set; }
    }
}