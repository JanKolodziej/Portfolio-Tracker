using Biblioteka_Klas;
namespace MauiApp1;

public partial class TabelaZysku : ContentView
{
	private readonly TabelaZyskuViewModel _viewModel;
	public TabelaZysku()
	{
		InitializeComponent();
		_viewModel = new TabelaZyskuViewModel();
		BindingContext = _viewModel;
	}
	public void Ustaw_Tabele(Konto konto)
	{
		_viewModel.Ustawienie_Tabeli_Zysku(konto);

    }
}