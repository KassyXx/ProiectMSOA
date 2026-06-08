using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect_MSOA___nr_5
{
    public class MesajComunicare
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public string Expeditor { get; set; }
        public string TextMesaj { get; set; }
        public DateTime DataTrimitere { get; set; }
    }
}

