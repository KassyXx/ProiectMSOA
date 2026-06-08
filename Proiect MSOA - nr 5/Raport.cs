using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect_MSOA___nr_5
{
    public class Raport
    {
        public int Id { get; set; }
        public DateTime DataGenerare { get; set; }
        public decimal TotalIncasari { get; set; }
        public int NumarProgramari { get; set; }
        public string TipRaport { get; set; }
    }
}
