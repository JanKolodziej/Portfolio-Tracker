using Biblioteka_Klas;

namespace MauiApp1;

public partial class WykresSP500 : ContentView
{ 
    private readonly WykresSP500ViewModel viewModel;
    public WykresSP500()
	{
		InitializeComponent();
        viewModel = new WykresSP500ViewModel();
        BindingContext = viewModel;
        
	}
	public void Ustaw_Wyglad_SP500(Konto konto)
	{
        WynikSPLabel.Text = "%" + konto.Zarobek_Na_SP500().ToString("0.00");
        LabelOstatniaAktualizacja.Text = "Ostatnia aktualizacja: ";
        LabelOstatniaAktualizacja.Text += SP500Pozycja.ListaSP500PozycjaDnia.Last().Data.Date.ToShortDateString();
        viewModel.Ustaw_Dane_Do_SP500(konto);
    }
}