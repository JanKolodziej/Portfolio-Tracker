using ClosedXML.Excel;

namespace Biblioteka_Klas
{
    /// <summary>
    /// Reprezentuje otwartą pozycje giełdową
    /// </summary>
    public class OtwartaPozycja
    {
        public string Symbol { get; set; }  //Np: XTB.PL
        public decimal Volume { get; set; } //Ilość pozycji
        public DateTime OpenTime { get; set; } //Data zawarcia pozycji
        public decimal OpenPrice { get; set; }//Cena otwarcia 1 pozycji
        public decimal MarketPrice { get; set; } //Aktualna Cena
        public decimal PurchasePrice { get; set; } //Całkowita cena zakupu
        public decimal Profit { get; set; } // Zysk ale również strata



        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="volume"></param>
        /// <param name="openTime"></param>
        /// <param name="openPrice"></param>
        /// <param name="marketPrice"></param>
        /// <param name="purchasePrice"></param>
        /// <param name="profit"></param>
        public OtwartaPozycja(string symbol, decimal volume, DateTime openTime, decimal openPrice, decimal marketPrice, decimal purchasePrice, decimal profit)
        {

            Symbol = symbol;

            Volume = volume;
            OpenTime = openTime;
            OpenPrice = openPrice;
            MarketPrice = marketPrice;
            PurchasePrice = purchasePrice;
            Profit = profit;
        }



        /// <summary>
        /// Wczytuje dane z pliku excel wygenerowanego przez xtb i zapisuje dane w ListaOtwartychPozycji
        /// </summary>
        /// <param name="path"></param>
        public static List<OtwartaPozycja> Wczytaj_Dane_Z_Excela(string path)
        {

            List<OtwartaPozycja> ListaOtwartychPozycji = new();
            var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheet(2);
            bool Flaga = false; //Zmienna pomocnicza ustalana na false, zmienia się na tru po rozpoczęciu się tabeli
            foreach (var row in worksheet.RowsUsed()) //Iterujemy po każdym rzędzie Pliku tworząc w każdym(w którym są dane), obiekt naszej klasy i dodając do listy
            {
                if (Flaga)
                {
                    if (row.Cell("C").GetValue<string>() != "")
                    {
                        OtwartaPozycja nowapozycja = new(row.Cell("C").GetValue<string>(), row.Cell("E").GetValue<decimal>(), row.Cell("F").GetValue<DateTime>(),
                        row.Cell("G").GetValue<decimal>(), row.Cell("H").GetValue<decimal>(), row.Cell("I").GetValue<decimal>(), row.Cell("P").GetValue<decimal>());
                        ListaOtwartychPozycji.Add(nowapozycja);



                    }
                }
                else
                {
                    if (row.Cell("B").GetValue<string>() == "Position")
                    {
                        Flaga = true; //Zmiana flagi na true bo rozpoczęcie tabeli
                    }
                }
            }

            return ListaOtwartychPozycji;
        }





    }
}
