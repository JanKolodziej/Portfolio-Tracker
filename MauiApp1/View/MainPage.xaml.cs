using Biblioteka_Klas;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {

        private readonly MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainPageViewModel();
            BindingContext = viewModel;


        }
        /// <summary>
        /// Metoda wywoływana przez WybierzPlikButton, wczytuje plik i go przetwarza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WybierzPlik(object sender, EventArgs e)
        {

            var konto = await new ObsługaPliku().Wczytaj_Plik();

            if (konto != null)
            {
                viewModel.Ustaw_Wyglad(konto);
                KontoSumaryczne.Tworzenie_Konta_Sumarycznego();
                PickerKonto.IsVisible = true;
            }

        }
    }

}
