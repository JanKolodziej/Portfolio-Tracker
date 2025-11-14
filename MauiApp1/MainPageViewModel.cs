using Biblioteka_Klas;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Metalama.Patterns.Observability;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    [Observable]
    public partial class MainPageViewModel 
    {
        //Wykres Sp500
        public IEnumerable<ISeries> SeriesSP500 { get; set; } = new ISeries[0];
        public Axis[] XAxesSP500 { get; set; } = new Axis[0];
        public Axis[] YAxesSP500 { get; set; } = new Axis[0];
        //Wykres kołowy
        public IEnumerable<ISeries> SeriesKolowyDywidendy { get; set; } = new ISeries[0];
        public IEnumerable<ISeries> SeriesKolowyOtwarte { get; set; } = new ISeries[0];
        //*********************************
        //Wykres słupkowy z podziałem na lata
        public IEnumerable<ISeries> SeriesSlupki { get; set; } = new ISeries[0];
        public Axis[] XAxesSlupek { get; set; } = new Axis[0];
        public Axis[] YAxesSlupek { get; set; } = new Axis[0];
        //*********************************
        //Wykres słupkowy z podziałem na miesiące
        public IEnumerable<ISeries> SeriesSlupkiMiesiace { get; set; } = new ISeries[0];
        public Axis[] XAxesSlupkiMiesiace { get; set; } = new Axis[0];
        public Axis[] YAxesSlupkiMiesiace { get; set; } = new Axis[0];
        //public required Func<ChartPoint, string> YToolTipLabelFormatter { get; set; }
        //*********************************
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

        //*********************************




        /// <summary>
        /// Ustawia dane do tabeli zysku i stopkę tabeli
        /// </summary>
        private void Ustawienie_Tabeli_Zysku(Konto konto)
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

        public void Ustaw_Wyglad(Konto konto)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            (SeriesSP500, XAxesSP500, YAxesSP500) = UstawienieWykresow.Ustawienie_Wykres_SP500(konto.ListaOperacjiGotowkowych);
            SeriesKolowyOtwarte = UstawienieWykresow.Ustawienie_Wykresu_Kolowego_Wartosci_Procentowych(konto.ListaRekordówTabeliZysku);
            SeriesKolowyDywidendy = UstawienieWykresow.Ustawienie_Wykresu_Kolowego_dywidend(konto.ListaKwotDywidend);
            (SeriesSlupkiMiesiace, XAxesSlupkiMiesiace, YAxesSlupkiMiesiace) = UstawienieWykresow.Ustawienie_Wykresu_Slupki_Miesiace(konto.ListaOperacjiGotowkowych);
            (SeriesSlupki, XAxesSlupek, YAxesSlupek) = UstawienieWykresow.Ustawienie_Wykresu_Slupki_Lata(konto.ListaOperacjiGotowkowych);
            Ustawienie_Tabeli_Zysku(konto);
            stopwatch.Stop();
            Debug.WriteLine($"Tworzenie danych do wykresow : {stopwatch.ElapsedMilliseconds} ms");
        }


    }
}
