using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteka_Klas;
using Metalama.Patterns.Observability;


namespace MauiApp1
{
    [Observable]
    public partial class ViewModelInformacjeOKoncie
    {
        public string WplatyLabelText { get; set; } = string.Empty;
        public string OdsetkiLabelText { get; set; } = string.Empty;
        public string ZyskLabelText { get; set; } = string.Empty;
        public string WynikKontaLabelText { get; set; } = string.Empty;


        public void Ustaw_Informacje_O_Koncie(Konto konto)
        {
            WplatyLabelText = konto.Wplaty.ToString() + " zł";
            OdsetkiLabelText = konto.Odsetki.ToString() + " zł"; ;
            ZyskLabelText = konto.ZyskNaZamknietychPozycjach.ToString();
            WynikKontaLabelText = konto.WynikKonta.ToString("0.00") + " %";
        }
    }
}
