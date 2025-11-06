using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka_Klas
{
    public class SP500PozycjaDnia
    {
        public decimal KursDolara {get; set; }
        public DateTime Data { get; set; }
        public decimal CenaSrednia { get; set; }
        public static List<SP500PozycjaDnia> ListaSP500PozycjaDnia = new();

       

        public SP500PozycjaDnia(decimal kursDolara, DateTime data, decimal cenaSrednia)
        {
            KursDolara = kursDolara;
            Data = data;
            CenaSrednia = cenaSrednia;
        }

        /// <summary>
        /// Wczytuje dane z plików do ListySP500PozycjaDnia
        /// </summary>
        
    }

    
}
