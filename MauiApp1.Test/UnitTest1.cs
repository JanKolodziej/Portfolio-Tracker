using Biblioteka_Klas;
namespace MauiApp1.Test
{
    public class UnitTest1
    {
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
                new OperacjeGotowkowe(TypOperacjiGotowkowej.deposit,DateTime.Now.AddDays(-7) , 5000,""),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.deposit,DateTime.Now.AddDays(-8) , 5000,""),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.withdrawal,DateTime.Now.AddDays(-7) , 5000,""),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.DIVIDENT,DateTime.Now.AddDays(-7) , 50,"XTB.PL"),
                new OperacjeGotowkowe(TypOperacjiGotowkowej.Withholding_Tax,DateTime.Now.AddDays(-7) , 5,"XTB.PL"),
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
    }
}
