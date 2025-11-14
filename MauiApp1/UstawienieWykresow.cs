using Biblioteka_Klas;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System.Diagnostics;
using System.Globalization;

namespace MauiApp1
{
    /// <summary>
    /// Statyczna klasa do ustawnienia wykresów 
    /// </summary>
    public static class UstawienieWykresow
    {

        /// <summary>
        /// Tworzy Dane do wykresu SP500
        /// </summary>
        /// <param name="listaOperacjiGotowkowych"></param>
        ///  <returns>Sp500 Series, X Axis, Y Axis </returns>
        public static (IEnumerable<ISeries>, Axis[], Axis[]) Ustawienie_Wykres_SP500(List<OperacjeGotowkowe> listaOperacjiGotowkowych)
        {

            var listaSp = new List<DateTimePoint>();
            decimal ostatniaCenaSP = SP500Pozycja.ListaSP500PozycjaDnia.Last().CenaSrednia;
            decimal ostatniKursDolara = SP500Pozycja.ListaSP500PozycjaDnia.Last().KursDolara;
            decimal liczbaPozycjiNaSP = 0;


            foreach (var operacja in listaOperacjiGotowkowych.OrderBy(date => date.Date))
            {
                if (operacja.Type == TypOperacjiGotowkowej.deposit || operacja.Type == TypOperacjiGotowkowej.withdrawal ||
                    operacja.Type == TypOperacjiGotowkowej.Subaccount_Transfer || operacja.Type == TypOperacjiGotowkowej.IKE_Deposit)
                {
                    if (listaSp.Count > 0)
                    {
                        if (operacja.Date.Date == listaSp.Last().DateTime.Date)  //Jeżeli data jest ta sama co ostatnio musimy nadpisać ostatni punkt
                        {
                            listaSp.RemoveAt(listaSp.Count - 1);
                        }
                        else if (operacja.Date.Date > listaSp.Last().DateTime.Date.AddDays(1))
                        {
                            for (int i = 1; i < (operacja.Date.Date - listaSp.Last().DateTime.Date).Days; i++)
                            {
                                SP500Pozycja SP = SP500Pozycja.Znajdz_Najblizszy_Sp(listaSp.Last().DateTime.Date.AddDays(1));
                                listaSp.Add(new DateTimePoint(listaSp.Last().DateTime.Date.AddDays(1), Math.Round(Convert.ToDouble(liczbaPozycjiNaSP * SP.KursDolara * SP.CenaSrednia), 2)));

                            }
                        }
                    }


                    SP500Pozycja dzienSp = SP500Pozycja.Znajdz_Najblizszy_Sp(operacja.Date);
                    liczbaPozycjiNaSP += operacja.Amount / dzienSp.KursDolara / dzienSp.CenaSrednia;
                    listaSp.Add(new DateTimePoint(operacja.Date.Date, Math.Round(Convert.ToDouble(liczbaPozycjiNaSP * dzienSp.KursDolara * dzienSp.CenaSrednia), 2)));
                }

            }


            IEnumerable<ISeries> seriesP500 = new ISeries[]
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "S&P500",
                    Values = listaSp,
                    Fill = null,
                    GeometrySize = 0
                }
            };

            Axis[] XAxes = new Axis[]
            {
                new DateTimeAxis(TimeSpan.FromDays(30),dt => dt.ToString("MM.yyyy"))
                {
                    MinLimit = listaSp.First().DateTime.Ticks
                }
            };


            Axis[] YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Wartość [PLN]",
                    MinLimit = 0,
                }
            };

            return (seriesP500, XAxes, YAxes);


        }

        /// <summary>
        /// Tworzy Dane do wykresu Kolowego z procentowymi wartosciami
        /// </summary>
        /// <param name="listaRekordTabelaZysku"></param>
        public static IEnumerable<ISeries> Ustawienie_Wykresu_Kolowego_Wartosci_Procentowych(List<RekordTabelaZysku> listaRekordTabelaZysku)
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
            return listaSeriiKolowego;


        }


        /// <summary>
        /// Tworzy wykres kołowy z dywidendami poszczególnych akcji
        /// </summary>
        public static IEnumerable<ISeries> Ustawienie_Wykresu_Kolowego_dywidend(List<Dywidendy> listaKwotDywidend)
        {
            IEnumerable<ISeries> seriesKolowyDywidendy = listaKwotDywidend.Select(g => new PieSeries<decimal>
            {
                Name = g.Symbol,
                Values = new decimal[] { g.Kwota }
            }).ToArray();
            return seriesKolowyDywidendy;
        }

        /// <summary>
        /// tworzy wykres słupkowy dywidend poszczególnych akcji z podziałęm na miesiące
        /// </summary>
         /// <returns>Sp500 Series, X Axis, Y Axis </returns>
        public static (IEnumerable<ISeries>, Axis[], Axis[]) Ustawienie_Wykresu_Slupki_Miesiace(List<OperacjeGotowkowe> listaOperacjiGotowkowych)
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

            // oś X — miesiące
            Axis[] xaxesSlupkiMiesiace = new Axis[]
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
            Axis[] yaxesSlupkiMiesiace = new Axis[]
            {
                        new Axis
                        {
                            Name = "Kwota [PLN]",
                            MinLimit = 0,
                        }
            };
            return (seriesList, xaxesSlupkiMiesiace,yaxesSlupkiMiesiace);
        }

        /// <summary>
        /// Tworzy wykres słupkowy z dywidendami posortowanymi według roku
        /// </summary>
        public static (IEnumerable<ISeries>, Axis[], Axis[]) Ustawienie_Wykresu_Slupki_Lata(List<OperacjeGotowkowe> listaOperacjiGotowkowych)
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
            var seriesSlupki = new ISeries[]
            {
                new ColumnSeries<decimal>
                {
                    Name = "Dywidendy",
                    Values = dywidendyPoRoku.Select(g => g.Suma).ToArray(),
                    MaxBarWidth = 1000,
                    Padding = 50
                }
            };

            Axis[] xAxesSlupek = new Axis[]
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
            Axis[] yAxesSlupek = new Axis[]
            {
                new Axis
                {
                    Name = "Kwota [PLN]",
                    MinLimit = 0,
                }
            };
            return (seriesSlupki, xAxesSlupek, yAxesSlupek);

        }
    }
}
