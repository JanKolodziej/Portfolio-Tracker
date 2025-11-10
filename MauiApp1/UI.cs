using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteka_Klas;
using System.Diagnostics;

namespace MauiApp1
{
    /// <summary>
    /// Znajdują się tu wszystkie pola, metody związane z UI
    /// </summary>
    internal class UI
    {
        public static async Task<string[]> ReadCsvAsync(string path)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(path);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            // podziel tekst na linie
            var lines = content.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

            return lines;
        }


        public static async Task Wczytaj_Z_Pliku_SP()
        {
            int pom = 0;
            //Dane w pliku w postaci Date,Open,High,Low,Close,Volume
            try
            {
                string[] linie = await ReadCsvAsync("HistoriaSP.csv");
                string[] linieDolar = await ReadCsvAsync("HistoriaKursDolara.csv");
                for (int i = 1; i < linie.Count(); i++) //Pomijamy pierwszy wiersz z nagłówkami
                {
                    string[] wartosci = linie[i].Split(',');
                    DateTime data = DateTime.Parse(wartosci[0]);
                    decimal cenaSrednia = (decimal.Parse(wartosci[2].Replace('.', ',')) + decimal.Parse(wartosci[3].Replace('.', ',')) + decimal.Parse(wartosci[4].Replace('.', ','))) / 3; //Liczymy Typical price (TP) jako średnią z  High, Low, Close

                    //Znajdź kurs dolara z tego samego dnia
                    decimal kursDolara = 0;


                    for (int j = i; j < linieDolar.Count(); j++)
                    {
                        string[] wartosciDolar = linieDolar[j].Split(',');
                        DateTime dataDolar = DateTime.Parse(wartosciDolar[0]);
                        if (dataDolar == data)
                        {
                            kursDolara = decimal.Parse(wartosciDolar[1].Replace('.', ','));
                            break;
                        }
                    }

                    if (kursDolara == 0) //Nie wiadomo czemu(najprawdopodobniej wynika to z dat świąt), ale w pliku ze strony NBP,
                    {                 //brakuje kursów dla niektórych dni, więc dla tych których brakuje, przypiszemy kurs z dnia poprzedniego
                                      //Na dzień pisania tego kodu tj. 10.10.2025 174 pozycje nie miały swojego kursu


                        SP500Pozycja pozycjaDnia = new(SP500Pozycja.ListaSP500PozycjaDnia.Last().KursDolara, data, cenaSrednia);
                        SP500Pozycja.ListaSP500PozycjaDnia.Add(pozycjaDnia);
                        pom++;

                    }
                    else
                    {
                        SP500Pozycja pozycjaDnia = new(kursDolara, data, cenaSrednia);
                        SP500Pozycja.ListaSP500PozycjaDnia.Add(pozycjaDnia);
                    }

                }

                Debug.WriteLine(pom + " Pozycji bez kursu dolara");
            }
            catch
            {
                //Plik nie istnieje
                Debug.WriteLine("Plik" + "HistoriaSP.csv" + " nie istnieje");

            }

        }
    }
}
