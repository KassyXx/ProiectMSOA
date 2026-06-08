using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect_MSOA___nr_5
{
    public class Plata
    {
        public int Id { get; set; }
        public int IdProgramare { get; set; }
        public decimal Suma { get; set; }
        public DateTime DataPlatii { get; set; }
        public string MetodaPlata { get; set; }
    }
}
