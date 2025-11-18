
using Dapper;
using Microsoft.Data.Sqlite;
using System.Diagnostics;


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


                // Wykonaj zapytanie za pomocą Dapper i mapuj na listę SP500Dolar
                var wyniki = await connection.QueryAsync<SP500Pozycja>("Select * From SP500Dolar");
                return wyniki.AsList();
            }


        }

    }
}
