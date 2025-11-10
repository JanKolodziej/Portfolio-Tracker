
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Diagnostics;


namespace Biblioteka_Klas
{
    /// <summary>
    /// Przechowuje kwoty dywidend z podziałem na symbole np XTB.PL , 50 zł
    /// </summary>
     public class Dywidendy
    {
        
        public string Symbol { get; set;}
        public decimal Kwota { get; set;}
        

        public Dywidendy(string symbol, decimal kwota)
        {
            Symbol = symbol;
            Kwota = kwota;
        }
    }
    /// <summary>
    /// Reprezentuje konto klienta w xtb
    /// </summary>
    public class Konto
    {
        public static List<Konto> ListaKont = new();  //Lista wszystkich kont
        public decimal Wplaty { get; set; }
        public decimal Odsetki { get; set; }
        //Listy Pozycji/Operacji ***************
        public List<OperacjeGotowkowe> ListaOperacjiGotowkowych { get; set; } = new();

        public List<ZamknietaPozycja> ListaZamknietychPozycji { get; set; } = new();
        public List<OtwartaPozycja> ListaOtwartychPozycji{ get; set; } = new();
        public  List<Dywidendy> ListaKwotDywidend { get; set; } = new();

        public List<RekordTabelaZysku> ListaRekordówTabeliZysku { get; set; } = new();
        public List<RekordTabelaZysku> ListaZamknietychPozycjiW_Tabeli { get; set; } = new();
        
        //**********************************

        //Zyski na pozycjach
        public decimal ZyskNaZamknietychPozycjach { get; set; } 
        public decimal ZyskNaOtwartychPozycjach { get; set; } = new();
        public decimal ZyskNaOtwartychPozycjachProcent { get; set; } = new();
        public decimal WartoscKonta { get; set; } = 0;
        public decimal WynikKonta { get; set; } = 0; //Wynik konta w procentach

        public string Nazwa { get; set; } = "";
        public Konto(List<OperacjeGotowkowe> listaoperacji, List<ZamknietaPozycja> zamkniete,List<OtwartaPozycja> otwarte, string nazwa)
        {
            Nazwa= nazwa;
            ListaOperacjiGotowkowych = listaoperacji;
            ListaZamknietychPozycji = zamkniete;
            ListaOtwartychPozycji = otwarte;
            SegregacjaOperacjiGotowkowych(this);
            Przelicz_Zysk_Na_Otwartych_Pozycjach();
            Przelicz_Zysk_Na_Zamknietych_Pozycjach();
            WartoscKonta = Wartosc_Konta();
            WynikKonta=Wynik_Konta();
            
            Dodaj_Rekordy_Do_Tabeli_Zysku();
            Policz_Zyski_Tabela();
            
            ListaKont.Add(this);

        }
        /// <summary>
        /// Przelicza zysk na otwartych pozycjach
        /// </summary>
        public void Przelicz_Zysk_Na_Otwartych_Pozycjach()
        {
            decimal zyskNaOtwartychPozycjach = 0;
            decimal zainwestowanaKwota = 0;
            decimal zyskNaOtwartychPozycjachProcent = 0;
            foreach (OtwartaPozycja pozycja in ListaOtwartychPozycji)
            {
                zyskNaOtwartychPozycjach += pozycja.Profit;
                zainwestowanaKwota += pozycja.PurchasePrice;
            }
            if (zainwestowanaKwota != 0)
            {
                zyskNaOtwartychPozycjachProcent = zyskNaOtwartychPozycjach / zainwestowanaKwota * 100;
            }


            ZyskNaOtwartychPozycjach = zyskNaOtwartychPozycjach;
            ZyskNaOtwartychPozycjachProcent = zyskNaOtwartychPozycjachProcent;

        }

        /// <summary>
        /// Przelicza zysk na zamknietych pozycjach
        /// </summary>
        public void Przelicz_Zysk_Na_Zamknietych_Pozycjach()
        {
            ZyskNaZamknietychPozycjach = 0;
            foreach (ZamknietaPozycja pozycja in ListaZamknietychPozycji)
            {
                ZyskNaZamknietychPozycjach += pozycja.Profit;
            }
        }

        

        /// <summary>
        /// Oblicza wartość konta na podstawie wzoru:
        /// Wpłaty + Odsetki + ZyskNaZamknietychPozycjach + ZyskNaOtwartychPozycjach + SumaDywidend (uwzglednia podatki)
        /// </summary>
        /// <returns>wartość konta(decimal)</returns>
        public decimal Wartosc_Konta()
        {
            decimal wartoscKonta = Wplaty + Odsetki + ZyskNaZamknietychPozycjach + ZyskNaOtwartychPozycjach + ListaKwotDywidend.Sum(x => x.Kwota);
            return wartoscKonta;
        }

        /// <summary>
        /// Oblicza Wynik konta w procentach na podstawie wzoru:
        /// ((WartoscKonta - Wplaty)/Wplaty) * 100%
        /// </summary>
        /// <returns>Wynik konta w procentach</returns>
        public decimal Wynik_Konta()
        {
            decimal wynikKonta = (WartoscKonta - Wplaty)/Wplaty * 100;
            return wynikKonta;
        }

        /// <summary>
        /// Liczy ile zarobilibyśmy na SP 500 gdybyśmy za każdą wpłate kupowali SP 500
        /// </summary>
        public decimal Zarobek_Na_SP500()
        {
            decimal ostatniaCenaSP = SP500Pozycja.ListaSP500PozycjaDnia.Last().CenaSrednia;
            decimal ostatniKursDolara = SP500Pozycja.ListaSP500PozycjaDnia.Last().KursDolara;
            decimal liczbaPozycjiNaSP = 0;


            foreach (var operacja in ListaOperacjiGotowkowych)
            {
                if (operacja.Type == TypOperacjiGotowkowej.deposit || operacja.Type == TypOperacjiGotowkowej.withdrawal ||
                    operacja.Type == TypOperacjiGotowkowej.Subaccount_Transfer || operacja.Type == TypOperacjiGotowkowej.IKE_Deposit)
                {
                    SP500Pozycja dzienSp = SP500Pozycja.Znajdz_Najblizszy_Sp(operacja.Date);
                    liczbaPozycjiNaSP += operacja.Amount / dzienSp.KursDolara / dzienSp.CenaSrednia;
                }

            }
            return (liczbaPozycjiNaSP * ostatniaCenaSP*ostatniKursDolara -Wplaty)/Wplaty*100;
        }



        /// <summary>
        /// Przechodzi po wszystkich listach, otwarte pozycje, zamkniete i dywidendy i dodaje je do listy ListaRekordówTabela
        /// </summary>
        protected void Dodaj_Rekordy_Do_Tabeli_Zysku()
        {
            foreach (OtwartaPozycja pozycja in ListaOtwartychPozycji)
            {
                if(!ListaRekordówTabeliZysku.Any(x => x.Symbol == pozycja.Symbol))//Jeżeli nie ma jeszcze rekordu z danym symbolem to dodajemy nowy
                {
                    RekordTabelaZysku rekordTabeli = new(pozycja.Symbol,pozycja.Volume,pozycja.MarketPrice,pozycja.OpenPrice,pozycja.Profit,pozycja.PurchasePrice);
                    ListaRekordówTabeliZysku.Add(rekordTabeli);
                }
                else//Jeżeli jest to aktualizujemy istniejący rekord
                {
                    foreach(RekordTabelaZysku rekord in ListaRekordówTabeliZysku)
                    {
                        if(rekord.Symbol== pozycja.Symbol)
                        {
                            rekord.CenaZakupu += pozycja.PurchasePrice; //Sumujemy koszt zakupu
                            rekord.IloscAkcji += pozycja.Volume;
                            rekord.Zysk += pozycja.Profit;


                        }
                    }
                }
            }

            foreach(ZamknietaPozycja zamknietaPozycja in ListaZamknietychPozycji)
            {
                if(!ListaRekordówTabeliZysku.Any(x => x.Symbol == zamknietaPozycja.Symbol))
                {
                    if(!ListaZamknietychPozycjiW_Tabeli.Any(x => x.Symbol == zamknietaPozycja.Symbol))
                    {
                        RekordTabelaZysku rekord = new(zamknietaPozycja.Symbol, 0, 0, 0, 0, 0, 0, 0, 0, zamknietaPozycja.Profit);
                        rekord.KosztZamknietychPozycji = zamknietaPozycja.TotalPurchasePrice;
                        ListaZamknietychPozycjiW_Tabeli.Add(rekord);
                    }
                    else
                    {
                        foreach(RekordTabelaZysku rekordTabela in ListaZamknietychPozycjiW_Tabeli)
                        {
                            if(rekordTabela.Symbol == zamknietaPozycja.Symbol)
                            {
                                rekordTabela.ZrealizowanyZysk += zamknietaPozycja.Profit;
                                rekordTabela.KosztZamknietychPozycji += zamknietaPozycja.TotalPurchasePrice;
                            }
                        }
                    }
                    

                }
                else
                {
                    foreach(RekordTabelaZysku rekordTabela in ListaRekordówTabeliZysku)
                    {
                        if(rekordTabela.Symbol == zamknietaPozycja.Symbol)
                        {
                            rekordTabela.ZrealizowanyZysk += zamknietaPozycja.Profit;
                            rekordTabela.KosztZamknietychPozycji += zamknietaPozycja.TotalPurchasePrice;
                        }
                    }
                }
            }
            foreach(Dywidendy dywidendy in ListaKwotDywidend)
            {
                
                if (ListaRekordówTabeliZysku.Any(x => x.Symbol == dywidendy.Symbol))
                {
                    foreach (RekordTabelaZysku rekordTabela in ListaRekordówTabeliZysku)
                    {
                        if (rekordTabela.Symbol == dywidendy.Symbol)
                        {
                            rekordTabela.Dywidendy += dywidendy.Kwota;
                        }
                    }
                }
                else if(ListaZamknietychPozycjiW_Tabeli.Any(x => x.Symbol == dywidendy.Symbol))
                {
                    foreach (RekordTabelaZysku rekordTabela in ListaZamknietychPozycjiW_Tabeli)
                    {
                        if (rekordTabela.Symbol == dywidendy.Symbol)
                        {
                            rekordTabela.Dywidendy += dywidendy.Kwota;
                        }
                    }

                }
                
            }
        }

        protected void Policz_Zyski_Tabela()
        {
            foreach(RekordTabelaZysku rekord in ListaRekordówTabeliZysku)
            {
               
                
             rekord.CenaOtwarcia = rekord.CenaZakupu / rekord.IloscAkcji;
             rekord.CalkowityZysk = rekord.Zysk + rekord.Dywidendy + rekord.ZrealizowanyZysk;
             rekord.ZyskProcent = rekord.Zysk / rekord.CenaZakupu * 100;
             rekord.CalkowityZyskProcent = rekord.CalkowityZysk / (rekord.CenaZakupu + rekord.KosztZamknietychPozycji) * 100;
            }
            foreach(RekordTabelaZysku rekord in ListaZamknietychPozycjiW_Tabeli)
            {
                rekord.CalkowityZysk = rekord.ZrealizowanyZysk + rekord.Dywidendy;
                rekord.CalkowityZyskProcent = rekord.CalkowityZysk / rekord.KosztZamknietychPozycji * 100;
            }
        }

        /// <summary>
        /// Przechodzi po wszystkich operacja gotowkowych i dodaje je do odpowiednich pól, obiektu klasy Konto
        /// </summary>
        /// <param name="konto"></param>
        protected static void SegregacjaOperacjiGotowkowych(Konto konto)
        {


            foreach (var operacja in konto.ListaOperacjiGotowkowych)
            {
                if (operacja.Type == TypOperacjiGotowkowej.deposit || operacja.Type == TypOperacjiGotowkowej.withdrawal ||
                operacja.Type == TypOperacjiGotowkowej.Subaccount_Transfer || operacja.Type == TypOperacjiGotowkowej.IKE_Deposit)
                {
                    konto.Wplaty += operacja.Amount;
                }
                else if (operacja.Type == TypOperacjiGotowkowej.Free_funds_Interest || operacja.Type == TypOperacjiGotowkowej.Free_funds_Interest_Tax)
                {
                    konto.Odsetki += operacja.Amount;
                }
                else if (operacja.Type == TypOperacjiGotowkowej.DIVIDENT || operacja.Type == TypOperacjiGotowkowej.Withholding_Tax)
                {
                    if (konto.ListaKwotDywidend.Any(x => x.Symbol == operacja.Symbol)) //Sprawdzamy czy dany symbol znajduje się już na liście 
                    {
                        //Jeżeli tak to go szukamy i dodajemy kwote do jego kwoty
                        for (int i = 0; i < konto.ListaKwotDywidend.Count; i++)
                        {
                            if (konto.ListaKwotDywidend[i].Symbol == operacja.Symbol)
                            {
                                konto.ListaKwotDywidend[i].Kwota += operacja.Amount;

                            }
                        }
                    }
                    else //Jeżeli nie to dodajemy nowy symbol do listy
                    {
                        Dywidendy dywidendy = new Dywidendy(operacja.Symbol, operacja.Amount);
                        konto.ListaKwotDywidend.Add(dywidendy);
                    }
                }

            }
        }
    }
}


