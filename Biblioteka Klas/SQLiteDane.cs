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
                DateTime startDateUtc = new DateTime(Dzisiaj.Year, Dzisiaj.Month, Dzisiaj.Day, 0, 0, 0, DateTimeKind.Utc);
                if (DateTime.Compare(Dzisiaj, OstatniSP.Data.Date.AddDays(2)) <= 0)
                {
                    Debug.WriteLine("Baza Aktualna");
                    return;
                }
                List<SP500Pozycja> nowePozycje = await ObslugaAPI.Pobieranie_Nowych_Pozycji_SP500(OstatniSP, startDateUtc);
                if (nowePozycje.Count() > 0)
                {
                    string insertQuery = "INSERT INTO SP500Dolar (Data, KursDolara, CenaSrednia) VALUES (@Data, @KursDolara, @CenaSrednia)";
                    await connection.ExecuteAsync(insertQuery, nowePozycje);
                    Debug.WriteLine($"Baza Zaktualizowana o {nowePozycje.Count()} pozycji");
                }
            }
        }
    }
}
