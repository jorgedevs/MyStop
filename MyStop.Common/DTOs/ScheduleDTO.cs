using System;
using Newtonsoft.Json;

namespace MyStop
{
    [JsonObject("Schedule")]
    public class ScheduleDTO
    {
        [JsonProperty("Pattern")]
        public string Pattern { get; set; }

        [JsonProperty("Destination")]
        public string Destination { get; set; }

        [JsonProperty("ExpectedLeaveTime")]
        public string ExpectedLeaveTime { get; set; }

        [JsonProperty("ExpectedCountdown")]
        public int ExpectedCountdown { get; set; }

        [JsonProperty("ScheduleStatus")]
        public string ScheduleStatus { get; set; }

        [JsonProperty("CancelledTrip")]
        public bool CancelledTrip { get; set; }

        [JsonProperty("CancelledStop")]
        public bool CancelledStop { get; set; }

        [JsonProperty("AddedTrip")]
        public bool AddedTrip { get; set; }

        [JsonProperty("AddedStop")]
        public bool AddedStop { get; set; }

        [JsonProperty("LastUpdate")]
        public string LastUpdate { get; set; }
    }
}
