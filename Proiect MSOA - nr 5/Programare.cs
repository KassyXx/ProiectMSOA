using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect_MSOA___nr_5
{
    public class Programare
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public int IdAngajat { get; set; }
        public int IdServiciu { get; set; }
        public DateTime DataOra { get; set; }
        public string Status { get; set; }
    }
}
