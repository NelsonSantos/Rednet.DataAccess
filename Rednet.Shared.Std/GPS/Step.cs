using Newtonsoft.Json;

namespace Rednet.Shared.GPS
{
    public class Step
    {
        public Distance Distance { get; set; }
        public Duration Duration { get; set; }
        [JsonProperty("end_location")]
        public Location EndLocation { get; set; }
        [JsonProperty("html_instructions")]
        public string HtmlInstructions { get; set; }
        public Polyline Polyline { get; set; }
        [JsonProperty("start_location")]
        public Location StartLocation { get; set; }
        [JsonProperty("travel_mode")]
        public string TravelMode { get; set; }
        public string Maneuver { get; set; }
    }
}