namespace Biblioteka_Klas
{
    public class KontoSumaryczne : Konto
    {

        public KontoSumaryczne(List<OperacjeGotowkowe> listaoperacji, List<ZamknietaPozycja> zamkniete, List<OtwartaPozycja> otwarte)
            : base(listaoperacji, zamkniete, otwarte, "Konto Sumaryczne")
        {

        }


        public static void Zsumuj_Wszystkie_Konta(List<OperacjeGotowkowe> operacjeGotowkowe, List<OtwartaPozycja> otwartaPozycja, List<ZamknietaPozycja> zamknietaPozycja)
        {
            operacjeGotowkowe.Clear();
            otwartaPozycja.Clear();
            zamknietaPozycja.Clear();
            foreach (Konto konto in ListaKont)
            {
                if (konto is not KontoSumaryczne)
                {
                    operacjeGotowkowe.AddRange(konto.ListaOperacjiGotowkowych);
                    zamknietaPozycja.AddRange(konto.ListaZamknietychPozycji);
                    otwartaPozycja.AddRange(konto.ListaOtwartychPozycji);
                }
            }

        }
        
        public static void Tworzenie_Konta_Sumarycznego()
        {
            if (Konto.ListaKont.Count >= 2)
            {

                foreach (Konto k in Konto.ListaKont)
                {
                    if (k is KontoSumaryczne)
                    {
                        Konto.ListaKont.Remove(k);
                        break;
                    }
                }
                List<OperacjeGotowkowe> listaoperacji = new();
                List<ZamknietaPozycja> zamkniete = new();
                List<OtwartaPozycja> otwarte = new();
                KontoSumaryczne.Zsumuj_Wszystkie_Konta(listaoperacji, otwarte, zamkniete);
                KontoSumaryczne kontoSumaryczne = new KontoSumaryczne(listaoperacji, zamkniete, otwarte);

            }

        }


    }
}
