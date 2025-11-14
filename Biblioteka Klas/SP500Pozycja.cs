namespace Biblioteka_Klas
{
    public class SP500Pozycja
    {
        public decimal KursDolara { get; set; }
        public DateTime Data { get; set; }
        public decimal CenaSrednia { get; set; }

        public static List<SP500Pozycja> ListaSP500PozycjaDnia = new();

        public SP500Pozycja() { }

        public SP500Pozycja(decimal kursDolara, DateTime data, decimal cenaSrednia)
        {
            KursDolara = kursDolara;
            Data = data;
            CenaSrednia = cenaSrednia;
        }

        /// <summary>
        /// Znajduje najbliższą pozycje SP500 dla zadanej daty
        /// </summary>
        /// <param name="dataDoSprawdzenia"></param>
        /// <returns></returns>
        public static SP500Pozycja Znajdz_Najblizszy_Sp(DateTime dataDoSprawdzenia)
        {
            int wynikDateCompare = -1;
            foreach (var dzienSp in SP500Pozycja.ListaSP500PozycjaDnia)
            {
                wynikDateCompare = DateTime.Compare(dzienSp.Data, dataDoSprawdzenia.Date); //Porównujemy daty dla daty tej samej funkcja zwraca 0, dla mniejszej -1, dla większej +1
                if (wynikDateCompare == 0 || wynikDateCompare > 0)                          // Daty w liście są pokolei więc jeżeli nie istnieje data ta sama
                {                                                                          // To z -1 przeskoczymy do +1 
                    return dzienSp;
                }

            }
            return SP500Pozycja.ListaSP500PozycjaDnia.Last();           //Jedyny przypadek dla któego poprzedni warunek nie zadziała to gdy wpłacimy "dzisiaj"
                                                                        //A ostatni kurs Sp jest z "wczoraj", więc zwracamy ostatni isniejący kurs

        }

    }


}
