using Biblioteka_Klas;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Metalama.Patterns.Observability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    [Observable]
    public partial class WykresSP500ViewModel
    {
        //Wykres Sp500
        public IEnumerable<ISeries> SeriesSP500 { get; set; } = new ISeries[0];
        public Axis[] XAxesSP500 { get; set; } = new Axis[0];
        public Axis[] YAxesSP500 { get; set; } = new Axis[0];

        public void Ustaw_Dane_Do_SP500(Konto konto)
        {
            (SeriesSP500, XAxesSP500, YAxesSP500) = UstawienieWykresow.Ustawienie_Wykres_SP500(konto.ListaOperacjiGotowkowych);
        }
    }
}
