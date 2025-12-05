using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Biblioteka_Klas
{
    //Struktura pliku Json chart/result/
    // -> /Timestamp (czas w Unix)
    // -> /Indicators/ quote(high,close,low)
    public class YahooQuote
    {
        [JsonPropertyName("high")]
        public List<double?> High { get; set; }
        [JsonPropertyName("close")]
        public List<double?> Close { get; set; }
        [JsonPropertyName("low")]
        public List<double?> Low { get; set; }
    }
    public class YahooIndicators
    {
        [JsonPropertyName("quote")]
        public List<YahooQuote> Quote { get; set; }
    }
    public class YahooResult
    {
        [JsonPropertyName("indicators")]
        public YahooIndicators Indicators { get; set; }

        [JsonPropertyName("timestamp")]
        public List<long> TimeStamp {  get; set; }
    }
    public class YahooChart
    {
        [JsonPropertyName("result")]
        public List<YahooResult> Results { get; set; }
    }
    public class YahooResponse
    {
        [JsonPropertyName("chart")]
        public YahooChart Chart { get; set; }
    }
}
