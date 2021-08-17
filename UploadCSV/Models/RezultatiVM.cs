using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadCSV.Models
{
    public class IzboriVM
    {
        public class Rezultati
        {
            public string IzbornaJedinica { get; set; }
            public long? DT { get; set; }
            public long? HC { get; set; }
            public long? JB { get; set; }
            public long? JFK { get; set; }
            public long? JR { get; set; }
        }

        public class RezultatiView
        {
            public string IzbornaJedinica { get; set; }
            public string Kandidat { get; set; }
            public string BrojGlasova { get; set; }
            public string Procenat { get; set; }
            public string Error { get; set; }
        }

        public IList<RezultatiView> rezultati { get; set; }
        public IList<string> IzborneJedinice { get; set; }
    }
}
