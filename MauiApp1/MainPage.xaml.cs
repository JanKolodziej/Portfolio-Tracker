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
                Ustaw_Wygląd_MainPage(konto);
            }

        }


        /// <summary>
        /// Ustawia wygląd MainPage 
        /// </summary>
        /// <param name="konto"></param>
        private void Ustaw_Wygląd_MainPage(Konto konto)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            WplatyLabel.Text = konto.Wplaty.ToString() + " zł";
            OdsetkiLabel.Text = konto.Odsetki.ToString() + " zł"; ;
            ZyskLabel.Text = konto.ZyskNaZamknietychPozycjach.ToString();
            WynikKontaLabel.Text = konto.WynikKonta.ToString("0.00") + " %";
            WynikSPLabel.Text = "%" + konto.Zarobek_Na_SP500().ToString("0.00");
            if (konto.WynikKonta > 0)
            {
                KomorkaWynik.Background = Colors.Green;
            }
            else
            {
                KomorkaWynik.Background = Colors.Red;
            }
            LabelOstatniaAktualizacja.Text = "Ostatnia aktualizacja: ";
            LabelOstatniaAktualizacja.Text += SP500Pozycja.ListaSP500PozycjaDnia.Last().Data.Date.ToShortDateString();


            
            //Zmiany tekstów po załadowaniu pliku
            PodajPlikLabel.Text = "Jeżeli chcesz, Dodaj kolejny konto do porównania";
            PrzyciskPlik.Text = "Dodaj Kolejny Plik";
            PickerKonto.IsVisible = true;


            KontoSumaryczne.Tworzenie_Konta_Sumarycznego();

            PickerKonto.ItemsSource = Konto.ListaKont.Select((k, index) => $"Konto {index + 1} - {System.IO.Path.GetFileName(k.Nazwa)}").ToList();

            for (int i = 0; i < Konto.ListaKont.Count; i++)
            {
                if (Konto.ListaKont[i] == konto)
                {
                    PickerKonto.SelectedIndex = i;
                    break;
                }

            }
            stopwatch.Stop();
            Debug.WriteLine($"Czas tworzenia konta : {stopwatch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Metoda obsługująca wybór pozycji w Picker Konta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickerKonto_SelectedIndexChanged(object sender, EventArgs e)
        {
                Ustaw_Wygląd_MainPage(Konto.ListaKont[PickerKonto.SelectedIndex]);
            
        }
    }
}
