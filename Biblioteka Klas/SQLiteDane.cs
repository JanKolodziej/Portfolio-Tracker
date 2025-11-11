
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Diagnostics;


namespace Biblioteka_Klas
{
    public class SQLiteDane
    {
        
        // Ścieżka do pliku bazy danych SQLite 
        public static string sciezkaDoBazy = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Baza_SP500.db"); // 

        // Ciąg połączenia
        public static string connectionString = $"Data Source={sciezkaDoBazy}";


        /// <summary>
        /// Asynchronicznie wczytuje dane SP500 z bazy SQLite i zwraca listę pozycji
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SP500Pozycja>> WczytajSP500()
        {
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
