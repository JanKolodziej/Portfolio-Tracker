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
            await Biblioteka_Klas.SQLiteDane.Aktualizacja_Danych(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Baza_SP500.db"));

            if (SP500Pozycja.ListaSP500PozycjaDnia.Count == 0)
            {
                SP500Pozycja.ListaSP500PozycjaDnia = await Biblioteka_Klas.SQLiteDane.WczytajSP500(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Baza_SP500.db"));
            }


            var plik = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz plik pobrany z XTB",
                FileTypes = customFileType,
            });


            if (plik == null)
                return null;

            string sciezka = plik.FullPath;



            var otwarte = OtwartaPozycja.Wczytaj_Dane_Z_Excela(sciezka);
            var zamkniete = ZamknietaPozycja.Wczytaj_Dane_Z_Excela(sciezka);
            var gotowkowe = OperacjeGotowkowe.Wczytaj_Dane_Z_Excela(sciezka);


            Konto konto = new(gotowkowe, zamkniete, otwarte, sciezka);

            return konto;
        }

    }

}
