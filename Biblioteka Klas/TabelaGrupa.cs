namespace Biblioteka_Klas
{
    public class TabelaGrupa : List<RekordTabelaZysku>
    {
        public string NazwaGrupy { get; set; }

        public TabelaGrupa(string nazwa, List<RekordTabelaZysku> pozycje) : base(pozycje)
        {
            NazwaGrupy = nazwa;
        }
    }
}
