using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Text.Json;


namespace Biblioteka_Klas
{
    public class SQLiteDane
    {
        /// <summary>
        /// Asynchronicznie wczytuje dane SP500 z bazy SQLite i zwraca listę pozycji
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SP500Pozycja>> WczytajSP500(string sciezkaDoBazy )
        {
            string connectionString = $"Data Source={sciezkaDoBazy}";
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();
                Debug.WriteLine("Połączono z bazą danych SQLite.");


                var wyniki = await connection.QueryAsync<SP500Pozycja>("Select * From SP500Dolar");
                return wyniki.AsList();
            }


        }

        /// <summary>
        /// Aktualizuje Dane w Bazie na podstawie API
        /// </summary>
        /// <param name="sciezkaDoBazy"></param>
        /// <returns></returns>
        public static async Task Aktualizacja_Danych(string sciezkaDoBazy)
        {
            string connectionString = $"Data Source={sciezkaDoBazy}";
            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();
                Debug.WriteLine("Połączono z bazą danych SQLite.");

                var OstatniSP = await connection.QuerySingleAsync<SP500Pozycja?>("SELECT * FROM SP500Dolar ORDER BY Data DESC LIMIT 1");
                if (OstatniSP == null) return;

                 DateTime Dzisiaj = DateTime.Now.Date;

                
                if(OstatniSP.Data == Dzisiaj)
                {
                    Debug.WriteLine("Baza Aktualna");
                    return;
                }
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                    //NBP
                    string nbpUrl = $"https://api.nbp.pl/api/exchangerates/rates/a/usd/{OstatniSP.Data:yyyy-MM-dd}/{Dzisiaj:yyyy-MM-dd}/?format=json";
                    var nbpJson = await client.GetStringAsync(nbpUrl);
                    var nbpData = JsonSerializer.Deserialize<NBPResponse>(nbpJson);
                    if (nbpData.Rates == null) return;
                    var kursyDolar = nbpData.Rates.ToDictionary(k => DateTime.Parse(k.EffectiveDate), v => v.Mid);

                    //Yahoo
                    long OstatniaDataUnix = ((DateTimeOffset)OstatniSP.Data).ToUnixTimeSeconds();
                    long DataDzisiejszaUnix = ((DateTimeOffset)Dzisiaj).ToUnixTimeSeconds();
                    string yahooUrl = $"https://query1.finance.yahoo.com/v8/finance/chart/%5EGSPC?period1={OstatniaDataUnix}&period2={DataDzisiejszaUnix}&interval=1d";
                    var yahooJson = await client.GetStringAsync(yahooUrl);
                    YahooResponse? yahooData = JsonSerializer.Deserialize<YahooResponse>(yahooJson);
                    //YahooResponse? yahooData = Laczenie_Z_Yahoo(OstatniaData, DataDzisiejsza, client);
                    if (yahooData == null) return;
                    var wynik = yahooData.Chart.Results.FirstOrDefault();
                    if(wynik == null || wynik.TimeStamp ==null ) return;

                    decimal ostatniKursDolara = OstatniSP.KursDolara;

                    List<SP500Pozycja> nowePozycje = new();
                    for (int i = 0; i < wynik.TimeStamp.Count(); i++)
                    {
                        DateTime dataKursu = DateTimeOffset.FromUnixTimeSeconds(wynik.TimeStamp[i]).DateTime.Date;
                        double? low = wynik.Indicators.Quote[0].Low[i];
                        double? high = wynik.Indicators.Quote[0].High[i];
                        double? close = wynik.Indicators.Quote[0].Close[i];
                        if(low != null && high != null && close != null)
                        {
                            decimal cenaSrednia = (decimal)(low + high + close) / 3;
                            if(!kursyDolar.ContainsKey(dataKursu))
                            {
                                dataKursu = OstatniSP.Data;
                            }
                            SP500Pozycja sP500 = new(kursyDolar[dataKursu],dataKursu,cenaSrednia);
                            nowePozycje.Add(sP500);
                            OstatniSP.Data = dataKursu;
                            ostatniKursDolara = kursyDolar[dataKursu];
                        }
                    }
                    if(nowePozycje.Count() > 0)
                    {
                        string insertQuery = "INSERT INTO SP500Dolar (Data, KursDolara, CenaSrednia) VALUES (@Data, @KursDolara, @CenaSrednia)";
                        await connection.ExecuteAsync(insertQuery, nowePozycje);
                        Debug.WriteLine("Baza Zaktualizowana");
                    }


                }
                
                
            }

        }

        //public static async Task<YahooResponse?> Laczenie_Z_Yahoo(DateTime ostatniaData, DateTime dataDzisiejsza,HttpClient client)
        //{
        //    long OstatniaDataUnix = ((DateTimeOffset)ostatniaData).ToUnixTimeSeconds();
        //    long DataDzisiejszaUnix = ((DateTimeOffset)dataDzisiejsza).ToUnixTimeSeconds();
        //    string yahooUrl = $"https://query1.finance.yahoo.com/v8/finance/chart/%5EGSPC?period1={OstatniaDataUnix}&period2={DataDzisiejszaUnix}&interval=1d";
        //    var yahooJson = await client.GetStringAsync(yahooUrl);
        //    YahooResponse? yahooData = JsonSerializer.Deserialize<YahooResponse>(yahooJson);
        //    return yahooData;
        //}
        
    }
}
