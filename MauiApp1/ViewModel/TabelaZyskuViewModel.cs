using Biblioteka_Klas;
using Metalama.Patterns.Observability;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    [Observable]
    public partial class TabelaZyskuViewModel
    {
        //Tabela
        public ObservableCollection<object> ListaZysku { get; set; } = new ObservableCollection<object>();
        //Stopka tabeli
        public decimal ZyskSuma { get; set; } //Zysk ze wszystkich otwartych pozycji
        public decimal ZyskSumaProcent { get; set; }
        //Zmienne pomocnicze do kolorowania tekstu w UI
        public bool WiekszyOdZeraSumaProcent => ZyskSumaProcent > 0;
        public bool MniejszyOdZeraSumaProcent => ZyskSumaProcent < 0;
        //*********************************
        public decimal LacznyZrealizowanyZyskSuma { get; set; } //Zysk ze wszystkich zamkniętych pozycji
        //*********************************
        public decimal CalkowityZyskSuma { get; set; } //Zysk całkowity (otwarte + zamknięte pozycje + dywidendy)
        public decimal CalkowityZyskSumaProcent { get; set; }
        //Zmienne pomocnicze do kolorowania tekstu w UI
        public bool WiekszyOdZeraCalkowitySumaProcent => CalkowityZyskSumaProcent > 0;
        public bool MniejszyOdZeraCalkowitySumaProcent => CalkowityZyskSumaProcent < 0;
        //*********************************




        /// <summary>
        /// Ustawia dane do tabeli zysku i stopkę tabeli
        /// </summary>
        public void Ustawienie_Tabeli_Zysku(Konto konto)
        {
            ListaZysku = new ObservableCollection<object>
            {
                new TabelaGrupa("Otwarte Pozycje",  konto.ListaRekordówTabeliZysku ),
                new TabelaGrupa( "Zamkniete Pozycje", konto.ListaZamknietychPozycjiW_Tabeli )
            };
            ZyskSuma = konto.ZyskNaOtwartychPozycjach;
            ZyskSumaProcent = konto.ZyskNaOtwartychPozycjachProcent;
            LacznyZrealizowanyZyskSuma = konto.ZyskNaZamknietychPozycjach;
            CalkowityZyskSuma = konto.WartoscKonta - konto.Wplaty;
            CalkowityZyskSumaProcent = konto.WynikKonta;

        }
    }
}
