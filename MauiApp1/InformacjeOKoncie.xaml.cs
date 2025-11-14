using Biblioteka_Klas;

namespace MauiApp1;

public partial class InformacjeOKoncie : ContentView
{
	public InformacjeOKoncie()
	{
		InitializeComponent();
	}
    public void Ustaw_Wyglag_Kafelki(Konto konto)
	{
        WplatyLabel.Text = konto.Wplaty.ToString() + " z³";
        OdsetkiLabel.Text = konto.Odsetki.ToString() + " z³"; ;
        ZyskLabel.Text = konto.ZyskNaZamknietychPozycjach.ToString();
        WynikKontaLabel.Text = konto.WynikKonta.ToString("0.00") + " %";
    }
}