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
        
      
        public string PodajPlikLabelText { get; set; } = "Witaj Użytkowniku, potrzebuje żebyś podał mi plik, abym działał poprawnie";
        public string PrzyciskPlikText { get; set; } = " Plik z excela";



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

        //*********************************

        public WykresSP500ViewModel wykresSP500ViewModel { get; set; }
        public TabelaZyskuViewModel tabelaZyskuViewModel { get; set; }
        public ViewModelInformacjeOKoncie viewModelInformacjeOKoncie { get; set; }

        public ObservableCollection<Konto> KontoList { get; set; } = Konto.ListaKont;




        [NotObservable]
        private Konto? _wybraneKonto;
        public Konto? WybraneKonto
        {
            get => _wybraneKonto;
            set
            {
                if(_wybraneKonto != value && value != null)
                {
                    _wybraneKonto = value;
                    OnPropertyChanged(nameof(WybraneKonto));
                    Ustaw_Wyglad(_wybraneKonto);
                }
            }
        }

        public MainPageViewModel()
        {
            wykresSP500ViewModel = new WykresSP500ViewModel();
            tabelaZyskuViewModel = new TabelaZyskuViewModel();
            viewModelInformacjeOKoncie = new ViewModelInformacjeOKoncie();

        }

        public void Ustaw_Wyglad(Konto konto)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Ustaw_Dane_Wykresow(konto);
            Ustaw_Dane_W_Innych_VM(konto);


            PodajPlikLabelText = "Jeżeli chcesz, Dodaj kolejny konto do porównania";
            PrzyciskPlikText = "Dodaj Kolejny Plik";

            stopwatch.Stop();
            Debug.WriteLine($"Tworzenie danych do wykresow : {stopwatch.ElapsedMilliseconds} ms");
        }
        private void Ustaw_Dane_Wykresow(Konto konto)
        {
            SeriesKolowyOtwarte = UstawienieWykresow.Ustawienie_Wykresu_Kolowego_Wartosci_Procentowych(konto.ListaRekordówTabeliZysku);
            SeriesKolowyDywidendy = UstawienieWykresow.Ustawienie_Wykresu_Kolowego_dywidend(konto.ListaKwotDywidend);
            (SeriesSlupkiMiesiace, XAxesSlupkiMiesiace, YAxesSlupkiMiesiace) = UstawienieWykresow.Ustawienie_Wykresu_Slupki_Miesiace(konto.ListaOperacjiGotowkowych);
            (SeriesSlupki, XAxesSlupek, YAxesSlupek) = UstawienieWykresow.Ustawienie_Wykresu_Slupki_Lata(konto.ListaOperacjiGotowkowych);
        }

        private void Ustaw_Dane_W_Innych_VM(Konto konto)
        {
            wykresSP500ViewModel.Ustaw_Dane_Do_SP500(konto);
            tabelaZyskuViewModel.Ustawienie_Tabeli_Zysku(konto);
            viewModelInformacjeOKoncie.Ustaw_Informacje_O_Koncie(konto);
        }


    }
}
