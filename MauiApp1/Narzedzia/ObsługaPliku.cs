using Biblioteka_Klas;

namespace MauiApp1
{
    /// <summary>
    /// Tworzy instancje klasy konto na podstawie danych z pliku
    /// </summary>
    internal class ObsługaPliku
    {
        public async Task<Konto?> Wczytaj_Plik()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "com.microsoft.excel.xlsx", "org.openxmlformats.spreadsheetml.sheet" } },
                { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" } },
                { DevicePlatform.WinUI, new[] { ".xlsx", ".xls" } },
                { DevicePlatform.MacCatalyst, new[] { "org.openxmlformats.spreadsheetml.sheet" } }
            });

            // Jeśli lista SP500 nie jest jeszcze wczytana, wczytaj ją asynchronicznie
            if (SP500Pozycja.ListaSP500PozycjaDnia.Count == 0)
            {
                SP500Pozycja.ListaSP500PozycjaDnia = await Biblioteka_Klas.SQLiteDane.WczytajSP500();
            }

            // Otwórz okno wyboru pliku
            var plik = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz plik pobrany z XTB",
                FileTypes = customFileType,
            });

            // Użytkownik anulował
            if (plik == null)
                return null;

            string sciezka = plik.FullPath;

            //  (opcjonalnie) sprawdzenie, czy już wczytano ten plik:
            /*
            foreach (Konto konto1 in Konto.ListaKont)
            {
                if (konto1.Path == sciezka)
                {
                    await DisplayAlert("Błąd", "Ten plik jest już wczytany do programu.", "OK");
                    return null;
                }
            }
            */

            // Wczytaj dane z Excela (jeśli te metody są synchroniczne, nie musisz ich awaitować)
            var otwarte = OtwartaPozycja.Wczytaj_Dane_Z_Excela(sciezka);
            var zamkniete = ZamknietaPozycja.Wczytaj_Dane_Z_Excela(sciezka);
            var gotowkowe = OperacjeGotowkowe.Wczytaj_Dane_Z_Excela(sciezka);

            // Utwórz konto z danymi
            Konto konto = new(gotowkowe, zamkniete, otwarte, sciezka);

            return konto;
        }

    }

}
