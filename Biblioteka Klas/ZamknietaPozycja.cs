using ClosedXML.Excel;

namespace Biblioteka_Klas
{
    public class ZamknietaPozycja
    {

        public string Symbol { get; set; }  //Np: XTB.PL
        public decimal Volume { get; set; } //Ilość pozycji
        public DateTime OpenTime { get; set; } //Data zawarcia pozycji
        public DateTime CloseTime { get; set; } //Data zamkniecia pozycji  

        public decimal OpenPrice { get; set; }//Cena otwarcia 1 pozycji
        public decimal ClosePrice { get; set; } //Cena zamknięcia 1 pozycji
        public decimal TotalPurchasePrice { get; set; } //Całkowita cena zakupu
        public decimal TotalSalePrice { get; set; } //Całkowita cena sprzedaży
        public decimal Profit { get; set; } // Zysk ale również strata




        public ZamknietaPozycja(string symbol, decimal volume, DateTime openTime, DateTime closeTime, decimal openPrice, decimal closePrice, decimal totalPurchasePrice, decimal totalSalePrice, decimal profit)
        {
            Symbol = symbol;
            Volume = volume;
            OpenTime = openTime;
            CloseTime = closeTime;
            OpenPrice = openPrice;
            ClosePrice = closePrice;
            TotalPurchasePrice = totalPurchasePrice;
            TotalSalePrice = totalSalePrice;
            Profit = profit;


        }





        /// <summary>
        /// Wczytuje dane z excela i dodaje je do ListaZamnietychPozycji
        /// </summary>
        /// <param name="path"></param>
        public static List<ZamknietaPozycja> Wczytaj_Dane_Z_Excela(string path)
        {
            List<ZamknietaPozycja> ListaZamknietychPozycji = new();
            var workbook = new XLWorkbook(path);

            var worksheet = workbook.Worksheet(1);
            bool Flaga = false; //Zmienna pomocnicza ustalana na false, zmienia się na tru po rozpoczęciu się tabeli
            foreach (var row in worksheet.RowsUsed()) //Iterujemy po każdym rzędzie Pliku tworząc w każdym(w którym są dane), obiekt naszej klasy i dodając do listy
            {

                if (Flaga)
                {
                    if (row.Cell("C").GetValue<string>() != "" && row.Cell("L").GetValue<string>() != "")
                    {
                        ZamknietaPozycja nowaoperacja = new(row.Cell("C").GetValue<string>(), row.Cell("E").GetValue<decimal>(),
                            row.Cell("F").GetValue<DateTime>(), row.Cell("H").GetValue<DateTime>(), row.Cell("G").GetValue<decimal>(),
                            row.Cell("I").GetValue<decimal>(), row.Cell("L").GetValue<decimal>(), row.Cell("M").GetValue<decimal>(), row.Cell("T").GetValue<decimal>());

                        ListaZamknietychPozycji.Add(nowaoperacja);
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
            return ListaZamknietychPozycji;
        }
    }
}
