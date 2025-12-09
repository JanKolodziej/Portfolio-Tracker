using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biblioteka_Klas
{
    internal static class ObslugaAPI
    {
        public static async Task<List<SP500Pozycja>> Pobieranie_Nowych_Pozycji_SP500(SP500Pozycja ostatniSP, DateTime dzisiaj)
        {
            using (var client = new HttpClient())
            {
                List<SP500Pozycja> nowePozycje = new();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                DateTime ostatniaDataUtc= new DateTime(ostatniSP.Data.Year, ostatniSP.Data.Month, ostatniSP.Data.Day, 0, 0, 0, DateTimeKind.Utc);
                NBPResponse? nbpData = await Laczenienie_Z_NBP_Dolar(ostatniaDataUtc, dzisiaj, client);
                if (nbpData == null) return nowePozycje;
                var kursyDolar = nbpData.Rates.ToDictionary(k => DateTime.Parse(k.EffectiveDate), v => v.Mid);

                YahooResponse? yahooData = await Laczenie_Z_Yahoo_SP500(ostatniaDataUtc, dzisiaj, client);

                if (yahooData == null || yahooData.Chart == null) return nowePozycje;
                var wynik = yahooData.Chart.Results.FirstOrDefault();
                if (wynik == null || wynik.TimeStamp == null) return nowePozycje;

                decimal ostatniKursDolara = ostatniSP.KursDolara;

                
                for (int i = 0; i < wynik.TimeStamp.Count(); i++)
                {
                    DateTime dataKursu = DateTimeOffset.FromUnixTimeSeconds(wynik.TimeStamp[i]).DateTime.Date;
                    double? low = wynik.Indicators.Quote[0].Low[i];
                    double? high = wynik.Indicators.Quote[0].High[i];
                    double? close = wynik.Indicators.Quote[0].Close[i];
                    if (low != null && high != null && close != null)
                    {
                        decimal cenaSrednia = (decimal)(low + high + close) / 3;
                        if (kursyDolar.ContainsKey(dataKursu))
                        {
                            ostatniKursDolara = kursyDolar[dataKursu];
                        }
                        SP500Pozycja sP500 = new(ostatniKursDolara, dataKursu, cenaSrednia);
                        nowePozycje.Add(sP500);
                        
                    }
                }
                return nowePozycje;
            }
        }



        /// <summary>
        /// Laczy sie z Yahoo Finance i pobiera kurs SP500
        /// </summary>
        /// <param name="ostatniaData"></param>
        /// <param name="dzisiaj"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task<YahooResponse?> Laczenie_Z_Yahoo_SP500(DateTime ostatniaData, DateTime dzisiaj, HttpClient client)
        {
            try
            { 
                long OstatniaDataUnix = ((DateTimeOffset)(ostatniaData.Date.AddDays(1))).ToUnixTimeSeconds();
                long DataDzisiejszaUnix = ((DateTimeOffset)(dzisiaj.Date.AddDays(-1))).ToUnixTimeSeconds();
                string yahooUrl = $"https://query1.finance.yahoo.com/v8/finance/chart/%5EGSPC?period1={OstatniaDataUnix}&period2={DataDzisiejszaUnix}&interval=1d";
                var yahooJson = await client.GetStringAsync(yahooUrl);
                YahooResponse? yahooData = JsonSerializer.Deserialize<YahooResponse>(yahooJson);
                return yahooData;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Wyjatek przy Laczenie Z Yahoo {ex}");
                return null;
            }

}

        /// <summary>
        /// Laczy się z NBP i pobiera kurs dolara
        /// </summary>
        /// <param name="ostatniaData"></param>
        /// <param name="dzisiaj"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task<NBPResponse?> Laczenienie_Z_NBP_Dolar(DateTime ostatniaData, DateTime dzisiaj, HttpClient client)
        {
            try
            {
                //Api daje kursy wlacznie, a poniewaz kurs Ostatni juz mamy, a kurs dzisiejszy jeszcze nie jest ustalony(rynek otwarty) to musimy zawęzić
                string nbpUrl = $"https://api.nbp.pl/api/exchangerates/rates/a/usd/{ostatniaData.AddDays(1):yyyy-MM-dd}/{dzisiaj.AddDays(-1):yyyy-MM-dd}/?format=json";
                var nbpJson = await client.GetStringAsync(nbpUrl);
                var nbpData = JsonSerializer.Deserialize<NBPResponse>(nbpJson);
                return nbpData;

            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Wyjatek przy Laczenie Z NBP {ex}");
                return null;
            }
            

        }
    }
}
