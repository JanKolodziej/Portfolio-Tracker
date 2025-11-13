using Biblioteka_Klas;
using DocumentFormat.OpenXml.Wordprocessing;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;


namespace MauiApp1
{
    public partial class MainPage : ContentPage
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
        public required Func<ChartPoint, string> YToolTipLabelFormatter { get; set; }
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

        private Konto? Konto_uzytkownika { get; set; }

        public MainPage()
        {

            
            InitializeComponent();

            
        }
        /// <summary>
        /// Ustawia dane do wykresu SP500
        /// </summary>
        /// <param name="listaOperacjiGotowkowych"></param>
        private void Ustawienie_Wykres_SP500(List<OperacjeGotowkowe> listaOperacjiGotowkowych)
        {
            var listaSp = new List<DateTimePoint>();
            decimal ostatniaCenaSP = SP500Pozycja.ListaSP500PozycjaDnia.Last().CenaSrednia;
            decimal ostatniKursDolara = SP500Pozycja.ListaSP500PozycjaDnia.Last().KursDolara;
            decimal liczbaPozycjiNaSP = 0;


            foreach (var operacja in listaOperacjiGotowkowych.OrderBy(date=>date.Date))
            {
                if (operacja.Type == TypOperacjiGotowkowej.deposit || operacja.Type == TypOperacjiGotowkowej.withdrawal ||
                    operacja.Type == TypOperacjiGotowkowej.Subaccount_Transfer || operacja.Type == TypOperacjiGotowkowej.IKE_Deposit)
                {
                    if(listaSp.Count>0)
                    {
                        if (operacja.Date.Date== listaSp.Last().DateTime.Date)  //Jeżeli data jest ta sama co ostatnio musimy nadpisać ostatni punkt
                        {
                            listaSp.RemoveAt(listaSp.Count - 1);
                        }
                        else if (operacja.Date.Date > listaSp.Last().DateTime.Date.AddDays(1))
                        {
                            for(int i =1; i<(operacja.Date.Date- listaSp.Last().DateTime.Date).Days;i++)
                            {
                                SP500Pozycja SP = SP500Pozycja.Znajdz_Najblizszy_Sp(listaSp.Last().DateTime.Date.AddDays(1));
                                listaSp.Add(new DateTimePoint(listaSp.Last().DateTime.Date.AddDays(1), Math.Round( Convert.ToDouble(liczbaPozycjiNaSP * SP.KursDolara*SP.CenaSrednia),2)));

                            }
                        }
                    }
                    
                    
                    SP500Pozycja dzienSp = SP500Pozycja.Znajdz_Najblizszy_Sp(operacja.Date);
                    liczbaPozycjiNaSP += operacja.Amount / dzienSp.KursDolara / dzienSp.CenaSrednia;
                    listaSp.Add(new DateTimePoint(operacja.Date.Date, Math.Round(Convert.ToDouble(liczbaPozycjiNaSP * dzienSp.KursDolara * dzienSp.CenaSrednia),2)));  
                }

            }
            
            
            SeriesSP500 = new ISeries[]
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "S&P500",
                    Values = listaSp,
                    Fill = null,
                    GeometrySize = 0
                }
            };

            XAxesSP500 = new Axis[]
            {
                new DateTimeAxis(TimeSpan.FromDays(30),dt => dt.ToString("MM.yyyy"))
                {
                    MinLimit = listaSp.First().DateTime.Ticks
                }
            };
            

            YAxesSP500 = new Axis[]
            {
                new Axis
                {
                    Name = "Wartość [PLN]",
                    MinLimit = 0,
                }
            };

            OnPropertyChanged(nameof(SeriesSP500));
            OnPropertyChanged(nameof(XAxesSP500));
            OnPropertyChanged(nameof(YAxesSP500));

        }


        
        
        /// <summary>
        /// Metoda wywoływana przez WybierzPlikButton, wczytuje plik i go przetwarza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WybierzPlik(object sender, EventArgs e)
        {
            
            var konto = await new ObsługaPliku().Wczytaj_Plik();

            if(konto != null)
            {
                Ustaw_Wygląd_MainPage(konto);
            }
            
        }

        /// <summary>
        /// Ustawia wygląd MainPage 
        /// </summary>
        /// <param name="konto"></param>
        private  void Ustaw_Wygląd_MainPage(Konto konto)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Konto_uzytkownika = konto;
            if (Konto_uzytkownika == null)
                return;

            WplatyLabel.Text = konto.Wplaty.ToString() +" zł";
            OdsetkiLabel.Text = konto.Odsetki.ToString() + " zł"; ;
            ZyskLabel.Text = konto.ZyskNaZamknietychPozycjach.ToString();
            WynikKontaLabel.Text = konto.WynikKonta.ToString("0.00") + " %";
            WynikSPLabel.Text = "%"+ konto.Zarobek_Na_SP500().ToString("0.00") ;
            if (konto.WynikKonta > 0)
            {
                KomorkaWynik.Background=Colors.Green;
            }
            else
            {
                KomorkaWynik.Background = Colors.Red;
            }
            LabelOstatniaAktualizacja.Text= "Ostatnia aktualizacja: ";
            LabelOstatniaAktualizacja.Text += SP500Pozycja.ListaSP500PozycjaDnia.Last().Data.Date.ToShortDateString();


           Ustawienie_Wykres_SP500(konto.ListaOperacjiGotowkowych);
            Ustawienie_Wykresu_Kolowego_Wartosci_Konta(konto.ListaRekordówTabeliZysku);
             Ustawienie_Wykresu_Kolowego_dywidend(konto.ListaKwotDywidend);
            Ustawienie_Wykresu_Slupki_Miesiace(konto.ListaOperacjiGotowkowych);
             Ustawienie_Wykresu_Slupki_Lata(konto.ListaOperacjiGotowkowych);
            Ustawienie_Tabeli_Zysku();
             Ustawienie_Wykresu_Slupki_Lata(konto.ListaOperacjiGotowkowych);
            

            //Zmiany tekstów po załadowaniu pliku
            PodajPlikLabel.Text = "Jeżeli chcesz, Dodaj kolejny konto do porównania";
            PrzyciskPlik.Text = "Dodaj Kolejny Plik";
            PickerKonto.IsVisible = true;
            
           
            if (Konto.ListaKont.Count >= 2)
            {

                foreach (Konto k in Konto.ListaKont)
                {
                    if (k is KontoSumaryczne)
                    {
                        Konto.ListaKont.Remove(k);
                        break;
                    }
                }
                List<OperacjeGotowkowe> listaoperacji = new();
                List<ZamknietaPozycja> zamkniete = new();
                List<OtwartaPozycja> otwarte = new();
                KontoSumaryczne.Zsumuj_Wszystkie_Konta(listaoperacji, otwarte, zamkniete);
                KontoSumaryczne kontoSumaryczne = new KontoSumaryczne(listaoperacji,zamkniete,otwarte);
                

            }




            PickerKonto.ItemsSource = Konto.ListaKont.Select((k, index) => $"Konto {index + 1} - {System.IO.Path.GetFileName(k.Nazwa)}").ToList();

            for (int i = 0; i < Konto.ListaKont.Count; i++)
            {
                if (Konto.ListaKont[i] == konto)
                {
                    PickerKonto.SelectedIndex = i;
                    break;
                }

            }

            BindingContext = this;

            stopwatch.Stop();
            Debug.WriteLine($"Czas tworzenia konta : {stopwatch.ElapsedMilliseconds} ms");
        }
        /// <summary>
        /// Ustawienie wykresu kołowego procentowego udziału w koncie
        /// </summary>
        private void Ustawienie_Wykresu_Kolowego_Wartosci_Konta(List<RekordTabelaZysku> listaRekordTabelaZysku)
        {
            
            decimal wartoscOtawrtychPozycji = 0;
            foreach (var element in listaRekordTabelaZysku)
            {
                wartoscOtawrtychPozycji += element.CenaAktualna * element.IloscAkcji;
            }
            double procentPozostalych = 0;
            var listaSeriiKolowego = new List<ISeries>();
            foreach (var element in listaRekordTabelaZysku)
            {
                double wartoscProcentowa = Convert.ToDouble(Math.Round(100 * element.CenaAktualna * element.IloscAkcji / wartoscOtawrtychPozycji));
                if (wartoscProcentowa < 2)
                {
                    procentPozostalych += wartoscProcentowa;
                }
                else
                {
                    listaSeriiKolowego.Add(new PieSeries<double>
                    {
                        Name = element.Symbol,
                        Values = new[] { wartoscProcentowa }
                        //DataLabelsFormatter = point => $"{point.Coordinate.SecondaryValue}: {point.Coordinate.PrimaryValue:0.0} "



                    });
                }

            }
            if (procentPozostalych > 0)
            {
                listaSeriiKolowego.Add(new PieSeries<double>
                {
                    Name = "Pozostałe",
                    Values = new[] { procentPozostalych }

                });
            }
            SeriesKolowyOtwarte = listaSeriiKolowego;

            OnPropertyChanged(nameof(SeriesKolowyOtwarte));
        }
        /// <summary>
        /// tworzy wykres słupkowy dywidend poszczególnych akcji z podziałęm na miesiące
        /// </summary>
        private void Ustawienie_Wykresu_Slupki_Miesiace(List<OperacjeGotowkowe> listaOperacjiGotowkowych)
        {
            //Tworzenie wykresu słupkowego z zestackowanymi dywidendami w podziale na miesiące, o zadanym roku(na początku tylko 2025)
            List<string> Miesiace = new List<string>()
                    {
                        "styczeń", "luty", "marzec", "kwiecień", "maj", "czerwiec", "lipiec", "sierpień", "wrzesień", "październik", "listopad", "Grudzień"
                    };


            //Gdzie typ operacji to dywidenda lub podatek od dywidendy, grupujemy po miesiącu i symbolu i sumujemy kwoty
            var dywidendyWMiesiacach = listaOperacjiGotowkowych
                .Where(g => ((g.Type == TypOperacjiGotowkowej.Withholding_Tax || g.Type == TypOperacjiGotowkowej.DIVIDENT) && g.Date.Year == DateTime.Now.Year))
                .GroupBy(g => new { Miesiac = g.Date.ToString("MMMM", new CultureInfo("pl-PL")), Symbol = g.Symbol })
                .OrderBy(g => g.Key.Miesiac)
                .ThenBy(g => g.Key.Symbol)
                .Select(g => new
                {
                    Data = g.Key.Miesiac,
                    Symbol = g.Key.Symbol,
                    Suma = g.Sum(x => x.Amount)



                }).ToList();

            //Lista unikalnych symboli
            var unikalneSymbole = dywidendyWMiesiacach.Select(g => g.Symbol).Distinct().OrderBy(g => g).ToList();


            var seriesList = new List<ISeries>();

            //Dla każdego symbolu tworzymy listę wartości dla każdego miesiąca, jeśli w danym miesiącu nie było dywidendy z danego symbolu, to dodajemy null
            //Wymaga tego od nas LIveCharts aby Lista wartosci wygladala np tak  [null, null, 23.5, null, 45.6, null, null, null, null, null, null, null] dla symbolu który miał dywidendę tylko w marcu i maju
            // Na koniec tworzymy serię słupków dla danego symbolu i dodajemy do listy serii
            foreach (var symbol in unikalneSymbole)
            {
                List<double?> lista = new();

                foreach (string miesiac in Miesiace)
                {
                    bool czyCosZostaloDodane = false;
                    foreach (var dywidenda in dywidendyWMiesiacach)
                    {
                        if (dywidenda.Symbol == symbol && dywidenda.Data == miesiac)
                        {
                            lista.Add(Convert.ToDouble(dywidenda.Suma));
                            czyCosZostaloDodane = true;
                        }

                    }
                    if (!czyCosZostaloDodane)
                    {
                        lista.Add(null);
                    }

                }
                seriesList.Add(new StackedColumnSeries<double?>
                {
                    Name = symbol,
                    Values = lista,




                });


            }
            SeriesSlupkiMiesiace = seriesList;

            // oś X — miesiące
            XAxesSlupkiMiesiace = new Axis[]
            {
                        new Axis
                        {
                            Labels = Miesiace,
                            Name = "Miesiace",
                            UnitWidth = 1,
                            MinStep = 1,
                            ForceStepToMin = true
                        }
            };
            // oś Y — kwoty
            YAxesSlupkiMiesiace = new Axis[]
            {
                        new Axis
                        {
                            Name = "Kwota [PLN]",
                            MinLimit = 0,
                        }
            };



            OnPropertyChanged(nameof(SeriesSlupkiMiesiace)); //Odświeżenie wykresu
        }
        /// <summary>
        /// Tworzy wykres słupkowy z dywidendami posortowanymi według roku
        /// </summary>
        private void Ustawienie_Wykresu_Slupki_Lata(List<OperacjeGotowkowe> listaOperacjiGotowkowych)
        {
            //Gdzie typ operacji to dywidenda lub podatek od dywidendy, grupujemy po roku i sumujemy kwoty 
            var dywidendyPoRoku = listaOperacjiGotowkowych
                .Where(g => g.Type == TypOperacjiGotowkowej.DIVIDENT || g.Type == TypOperacjiGotowkowej.Withholding_Tax)
                .GroupBy(g => g.Date.Year)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Rok = g.Key,

                    Suma = g.Sum(x => x.Amount)
                }).ToList();




            SeriesSlupki = new ISeries[]
            {
                new ColumnSeries<decimal>
                {
                    Name = "Dywidendy",
                    Values = dywidendyPoRoku.Select(g => g.Suma).ToArray(),
                    MaxBarWidth = 1000,
                    Padding = 50
                }
            };

            XAxesSlupek = new Axis[]
            {
                    new Axis
                    {
                        Labels = dywidendyPoRoku.Select(g => g.Rok.ToString()).ToArray(),
                        Name = "Rok",
                        UnitWidth = 1,
                        MinStep = 1,
                        ForceStepToMin = true
                    }
            };
            // oś Y — kwoty
            YAxesSlupek = new Axis[]
            {
                    new Axis
                    {
                        Name = "Kwota [PLN]",
                        MinLimit = 0,
                    }
            };

            OnPropertyChanged(nameof(SeriesSlupki));
        }

        /// <summary>
        /// Tworzy wykres kołowy z dywidendami poszczególnych akcji
        /// </summary>
        private void Ustawienie_Wykresu_Kolowego_dywidend(List<Dywidendy> listaKwotDywidend)
        {
            if (Konto_uzytkownika != null)
            {
                SeriesKolowyDywidendy = listaKwotDywidend.Select(g => new PieSeries<decimal>
                {
                    Name = g.Symbol,
                    Values = new decimal[] { g.Kwota }
                }).ToArray();

                OnPropertyChanged(nameof(SeriesKolowyDywidendy));
            }
        }
        /// <summary>
        /// Ustawia dane do tabeli zysku i stopkę tabeli
        /// </summary>
        private void Ustawienie_Tabeli_Zysku()
        {
            if(Konto_uzytkownika == null)
            { return; }
            
            ListaZysku = new ObservableCollection<object>
            {
                new TabelaGrupa("Otwarte Pozycje",  Konto_uzytkownika.ListaRekordówTabeliZysku ),
                new TabelaGrupa( "Zamkniete Pozycje", Konto_uzytkownika.ListaZamknietychPozycjiW_Tabeli )
            };
            ZyskSuma = Konto_uzytkownika.ZyskNaOtwartychPozycjach;
            ZyskSumaProcent = Konto_uzytkownika.ZyskNaOtwartychPozycjachProcent;
            LacznyZrealizowanyZyskSuma = Konto_uzytkownika.ZyskNaZamknietychPozycjach;
            CalkowityZyskSuma = Konto_uzytkownika.WartoscKonta - Konto_uzytkownika.Wplaty;
            CalkowityZyskSumaProcent = Konto_uzytkownika.WynikKonta;

            OnPropertyChanged(nameof(ListaZysku));
            OnPropertyChanged(nameof(ZyskSuma));
            OnPropertyChanged(nameof(ZyskSumaProcent));
            OnPropertyChanged(nameof(LacznyZrealizowanyZyskSuma));
            OnPropertyChanged(nameof(CalkowityZyskSuma));
            OnPropertyChanged(nameof(CalkowityZyskSumaProcent));



        }
        /// <summary>
        /// Metoda obsługująca wybór pozycji w Picker Konta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickerKonto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Konto.ListaKont[PickerKonto.SelectedIndex] != Konto_uzytkownika)
            {
                Ustaw_Wygląd_MainPage(Konto.ListaKont[PickerKonto.SelectedIndex]);
            }
            
            
            
        }
    }
}
