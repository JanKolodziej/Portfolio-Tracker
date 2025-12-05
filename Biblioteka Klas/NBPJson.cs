using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Biblioteka_Klas
{
    public class NBPRates
    {
        [JsonPropertyName("effectiveDate")]
        public string EffectiveDate { get; set; }

        [JsonPropertyName("mid")]
        public decimal Mid { get; set; }


    }

    public class NBPResponse
    {
        [JsonPropertyName("rates")]
        public List<NBPRates> Rates { get; set; }
    }
}
