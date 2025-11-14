using Biblioteka_Klas;
namespace MauiApp1.Test
{
    public class UnitTest1
    {
        /// <summary>
        /// Tworzy Konto z przykładowymi danymi do testów
        /// </summary>
        /// <returns>Konto do testu</returns>
        private Konto Stworz_Przykladowe_Konto()
        {
            List<ZamknietaPozycja> zamknietePpozycje = new()
            {
                new ZamknietaPozycja("XTB.PL", 10, DateTime.Now.AddDays(-10), DateTime.Now, 100, 110, 1000, 1100, 100),
                new ZamknietaPozycja("ABC.PL", 5, DateTime.Now.AddDays(-5), DateTime.Now, 200, 190, 1000, 950, -50)
            };
            List<OtwartaPozycja> otwartaPozycjas = new()
            {
                new OtwartaPozycja("XYZ.PL", 8, DateTime.Now.AddDays(-3), 150, 160, 1200, 80),
                new OtwartaPozycja("DEF.PL", 12, DateTime.Now.AddDays(-1), 300, 290, 3600, -120)
            };
            List<OperacjeGotowkowe> operacjeGotowkowes = new()
            {
                new OperacjeGotowkowe(TypOperacjiGotowkowej.deposit,DateTime.Now.AddDays(-9) , 5000,""),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.deposit,DateTime.Now.AddDays(-8) , 5000,""),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.withdrawal,DateTime.Now.AddDays(-7) , -5000,""),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.DIVIDENT,DateTime.Now.AddDays(-7) , 50,"XTB.PL"),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.Withholding_Tax,DateTime.Now.AddDays(-7) , -5,"XTB.PL"),
            };
            Konto konto = new Konto(operacjeGotowkowes, zamknietePpozycje, otwartaPozycjas, "Testy");
            return konto;


        }
        [Fact]
        public void Test_Przelicz_Zysk_Na_Zamknietych_Pozycjach()
        {

            Konto konto = Stworz_Przykladowe_Konto();


            Assert.Equal(50, konto.ZyskNaZamknietychPozycjach);
        }

        [Fact]
        public void Test_Przelicz_Zarobek_Na_SP500()
        {
            Konto konto = Stworz_Przykladowe_Konto();
            SP500Pozycja.ListaSP500PozycjaDnia = new List<SP500Pozycja>
            {
                new SP500Pozycja(4.0m, DateTime.Now.AddDays(-10), 4000m),
                new SP500Pozycja(4.1m, DateTime.Now.AddDays(-9), 4100m),
                new SP500Pozycja(4.2m, DateTime.Now.AddDays(-8), 4100m),
                new SP500Pozycja(4.3m, DateTime.Now.AddDays(-7), 4190m),
                new SP500Pozycja(4.4m, DateTime.Now.AddDays(-6), 4100m),
                new SP500Pozycja(4.5m, DateTime.Now.AddDays(-5), 4120m),
                new SP500Pozycja(4.4m, DateTime.Now.AddDays(-4), 4130m),
                new SP500Pozycja(4.3m, DateTime.Now.AddDays(-3), 4200m),
                new SP500Pozycja(4.2m, DateTime.Now.AddDays(-2), 4100m),
                new SP500Pozycja(4.1m, DateTime.Now.AddDays(-1), 4300m),
                new SP500Pozycja(4.0m, DateTime.Now, 4400m)
            };
            Assert.Equal(9.220800702442883399813593720m, konto.Zarobek_Na_SP500());
        }

    }
}
