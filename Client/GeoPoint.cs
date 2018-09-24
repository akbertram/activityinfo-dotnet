using System;
using Newtonsoft.Json;

namespace ActivityInfo
{
    public class GeoPoint
    {
        [JsonProperty("latitude")]
        public double Latitute { get; set;  }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        public GeoPoint()
        {
        }

        public GeoPoint(double latitute, double longitude)
        {
            Latitute = latitute;
            Longitude = longitude;
        }
    }
}
