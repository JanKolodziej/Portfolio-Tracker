using Biblioteka_Klas;

namespace MauiApp1.Test
{
    public class TestyPrzekrojowe
    {
        /// <summary>
        /// Test przekrojowy na podstawie pliku Plik_xtb_do_testow.xlsx
        /// </summary>
        [Fact]
        public async Task Test_Przkrojowy_Zamkniete_Pozycje() 
        {
            List<ZamknietaPozycja> zamknietaPozycja = ZamknietaPozycja.Wczytaj_Dane_Z_Excela("Plik_xtb_do_testow.xlsx");
            List<OtwartaPozycja> otwartaPozycja = OtwartaPozycja.Wczytaj_Dane_Z_Excela("Plik_xtb_do_testow.xlsx");
            List<OperacjeGotowkowe> operacjeGotowkowe = OperacjeGotowkowe.Wczytaj_Dane_Z_Excela("Plik_xtb_do_testow.xlsx");
            Konto konto = new(operacjeGotowkowe, zamknietaPozycja, otwartaPozycja, "Konto Testowe");

            SP500Pozycja.ListaSP500PozycjaDnia = await SQLiteDane.WczytajSP500();

            Assert.Equal(-271.4080000m, konto.ZyskNaZamknietychPozycjach);
            Assert.Equal(8500, konto.Wplaty);
            Assert.Equal(1330, konto.ZyskNaOtwartychPozycjach);
            Assert.Equal(12.31m, konto.Odsetki);
            List<Dywidendy> testoweLista = new() { new("PKO.PL", 202.5m) };
            Assert.Equal(testoweLista[0].Kwota, konto.ListaKwotDywidend[0].Kwota);
            Assert.Equal(testoweLista[0].Symbol, konto.ListaKwotDywidend[0].Symbol);
            Assert.Equal(202.5m - 271.408m + 8500m + 1330m + 12.31m, konto.WartoscKonta);
            Assert.Equal((202.5m - 271.408m + 1330m + 12.31m) / 85, konto.WynikKonta);

            Assert.Equal(39.44305387m, konto.Zarobek_Na_SP500(),7);






        }
    }
}
