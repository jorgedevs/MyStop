using Newtonsoft.Json;

namespace MyStop
{
    [JsonObject("Stop")]
    public class StopDTO
    {
        [JsonProperty("StopNo")]
        public string StopNo { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("BayNo")]
        public string BayNo { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("OnStreet")]
        public string OnStreet { get; set; }

        [JsonProperty("AtStreet")]
        public string AtStreet { get; set; }

        [JsonProperty("Latitude")]
        public string Latitude { get; set; }

        [JsonProperty("Longitude")]
        public string Longitude { get; set; }

        [JsonProperty("WheelchairAccess")]
        public string WheelchairAccess { get; set; }

        [JsonProperty("Distance")]
        public string Distance { get; set; }

        [JsonProperty("Routes")]
        public string Routes { get; set; }

        public StopDTO ()
        {
        }
    }
}

