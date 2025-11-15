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

        public void Ustaw_Wyglad(Konto konto)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            SeriesKolowyOtwarte = UstawienieWykresow.Ustawienie_Wykresu_Kolowego_Wartosci_Procentowych(konto.ListaRekordówTabeliZysku);
            SeriesKolowyDywidendy = UstawienieWykresow.Ustawienie_Wykresu_Kolowego_dywidend(konto.ListaKwotDywidend);
            (SeriesSlupkiMiesiace, XAxesSlupkiMiesiace, YAxesSlupkiMiesiace) = UstawienieWykresow.Ustawienie_Wykresu_Slupki_Miesiace(konto.ListaOperacjiGotowkowych);
            (SeriesSlupki, XAxesSlupek, YAxesSlupek) = UstawienieWykresow.Ustawienie_Wykresu_Slupki_Lata(konto.ListaOperacjiGotowkowych);
            
            stopwatch.Stop();
            Debug.WriteLine($"Tworzenie danych do wykresow : {stopwatch.ElapsedMilliseconds} ms");
        }


    }
}
