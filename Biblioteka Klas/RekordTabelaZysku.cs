namespace Biblioteka_Klas
{
    /// <summary>
    /// Klasa reprezentująca rekord w tabeli zysku.
    /// </summary>
    public class RekordTabelaZysku
    {
        public string Symbol { get; set; }
        public decimal IloscAkcji { get; set; }
        public decimal CenaAktualna { get; set; }
        public decimal CenaOtwarcia { get; set; }
        public decimal CenaZakupu { get; set; }
        public decimal KosztZamknietychPozycji { get; set; }


        public decimal Dywidendy { get; set; }
        public decimal Zysk { get; set; }
        public decimal ZyskProcent { get; set; }
        // Właściwość sprawdzająca, czy ZyskProcent jest większy od zera, wartośc pomocnicza do kolorowania tekstu w UI
        public bool WiekszyOdZeraProcent => ZyskProcent > 0;
        public bool MniejszyOdZeraProcent => ZyskProcent < 0;
        //****************************************************************************

        public decimal ZrealizowanyZysk { get; set; }
        public decimal CalkowityZysk { get; set; }
        public decimal CalkowityZyskProcent { get; set; }
        // Właściwość sprawdzająca, czy CalkowityZyskProcent jest większy od zera, wartośc pomocnicza do kolorowania tekstu w UI
        public bool WiekszyOdZeraCalkowityProcent => CalkowityZyskProcent > 0;
        public bool MniejszyOdZeraCalkowityProcent => CalkowityZyskProcent < 0;
        //****************************************************************************

        public RekordTabelaZysku(string symbol, decimal iloscAkcji, decimal cenaAktualna, decimal cenaOtwarcia, decimal zysk, decimal cenakupna, decimal zyskProcent = 0, decimal dywidendy = 0, decimal calkowityZysk = 0, decimal zrealizowanyZysk = 0)
        {
            Symbol = symbol;
            IloscAkcji = iloscAkcji;
            CenaAktualna = cenaAktualna;
            CenaOtwarcia = cenaOtwarcia;
            Zysk = zysk;
            CenaZakupu = cenakupna;
            ZyskProcent = zyskProcent;
            Dywidendy = dywidendy;
            CalkowityZysk = calkowityZysk;
            ZrealizowanyZysk = zrealizowanyZysk;
        }

    }
}
