using ClosedXML.Excel;


namespace Biblioteka_Klas
{
    /// <summary>
    /// Wszystkie typy operacji gotówkowych występujących w xtb
    /// </summary>
    public enum TypOperacjiGotowkowej //Nazwy są brzydkie ale są zbliżone do tych w pliku xtb z róznicą że zamiast "_" jest " "
    {
        deposit,
        withdrawal,
        IKE_Deposit,
        Free_funds_Interest,
        Free_funds_Interest_Tax, //Free-funds Interest Tax
        DIVIDENT,
        Withholding_Tax,
        Stock_purchase,
        close_trade, //Pokazuje ile zarobiliśmy na danej pozycji
        Stock_sale, //Pokazuje za ile sprzedaliśmy daną pozycje
        Subaccount_Transfer,
        tax_IFTT

    }
    public class OperacjeGotowkowe
    {
        public TypOperacjiGotowkowej Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Symbol { get; set; }


        public OperacjeGotowkowe(TypOperacjiGotowkowej type, DateTime date, decimal amount, string symbol)
        {
            Type = type;
            Date = date;
            Amount = amount;
            Symbol = symbol;
        }

        /// <summary>
        /// Wczytuje dane o operacjach gotówkowych do ListaOperacjiGotowkowych
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<OperacjeGotowkowe> Wczytaj_Dane_Z_Excela(string path)
        {
            //konto.Wplaty = 0;
            //konto.Odsetki = 0;

            var workbook = new XLWorkbook(path);
            var worksheet = workbook.Worksheet(4);
            List<OperacjeGotowkowe> ListaOperacjiGotowkowych = new();
            bool Flaga = false; //Zmienna pomocnicza ustalana na false, zmienia się na tru po rozpoczęciu się tabeli
            foreach (var row in worksheet.RowsUsed()) //Iterujemy po każdym rzędzie Pliku tworząc w każdym(w którym są dane), obiekt naszej klasy i dodając do listy
            {
                if (row.Cell("C").GetValue<string>() != "")
                {


                    if (Flaga)
                    {
                        TypOperacjiGotowkowej typ;
                        string TypString = row.Cell("C").GetValue<string>();
                        TypString = TypString.Replace(" ", "_");
                        TypString = TypString.Replace("-", "_");
                        typ = Enum.Parse<TypOperacjiGotowkowej>(TypString);



                        OperacjeGotowkowe nowaoperacja = new(typ, row.Cell("D").GetValue<DateTime>(), row.Cell("G").GetValue<decimal>(), row.Cell("F").GetValue<string>());
                        ListaOperacjiGotowkowych.Add(nowaoperacja);



                    }
                    else
                    {
                        if (row.Cell("B").GetValue<string>() == "ID")
                        {
                            Flaga = true; //Zmiana flagi na true bo rozpoczęcie tabeli
                        }
                    }


                }

            }

            return ListaOperacjiGotowkowych;

        }


    }
}
