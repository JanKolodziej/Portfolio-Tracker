using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka_Klas
{
    public class KontoSumaryczne : Konto
    {

        public KontoSumaryczne(List<OperacjeGotowkowe> listaoperacji, List<ZamknietaPozycja> zamkniete, List<OtwartaPozycja> otwarte)
            :base(listaoperacji,zamkniete, otwarte,"Konto Sumaryczne")
        {
            
        }
    

        public static void Zsumuj_Wszystkie_Konta(List<OperacjeGotowkowe> operacjeGotowkowe,List<OtwartaPozycja> otwartaPozycja, List<ZamknietaPozycja> zamknietaPozycja)
        {
            operacjeGotowkowe.Clear();
            otwartaPozycja.Clear();
            zamknietaPozycja.Clear();
            foreach(Konto konto in ListaKont)
            {
                if(konto is not KontoSumaryczne)
                {
                    operacjeGotowkowe.AddRange(konto.ListaOperacjiGotowkowych);
                    zamknietaPozycja.AddRange(konto.ListaZamknietychPozycji);
                    otwartaPozycja.AddRange(konto.ListaOtwartychPozycji);
                }
            }
            
        }
        private void Przelicz_Dywidendy()
        {
             List<Dywidendy> posegregowaneDywidendy = new();

            foreach (Dywidendy dywidendy in ListaKwotDywidend)
            {
                if(posegregowaneDywidendy.Any(x => x.Symbol == dywidendy.Symbol)) //Sprawdzamy czy dany symbol znajduje się już na liście 
                {
                    //Jeżeli tak to go szukamy i dodajemy kwote do jego kwoty
                    for (int i = 0; i < posegregowaneDywidendy.Count; i++)
                    {
                        if (posegregowaneDywidendy[i].Symbol == dywidendy.Symbol)
                        {
                            posegregowaneDywidendy[i].Kwota += dywidendy.Kwota;
                        }
                    }
                }
                else //Jeżeli nie to dodajemy nowy symbol do listy
                {
                    Dywidendy nowaDywidenda = new Dywidendy(dywidendy.Symbol, dywidendy.Kwota);
                    posegregowaneDywidendy.Add(nowaDywidenda);
                }
            }
            ListaKwotDywidend = posegregowaneDywidendy;

        }



    }
}
