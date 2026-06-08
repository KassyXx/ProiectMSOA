using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect_MSOA___nr_5
{
    public class BazaDeDateContext : DbContext
    {
        public BazaDeDateContext() : base("name=BazaDeDateContext")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BazaDeDateContext>());
        }
        public DbSet<Utilizator> Utilizatori { get; set; }
        public DbSet<ProdusServiciu> ProduseServicii { get; set; } 
        public DbSet<Programare> Programari { get; set; }
        public DbSet<Plata> Plati { get; set; }
        public DbSet<Raport> Rapoarte { get; set; }
        public DbSet<MesajComunicare> MesajeComunicare { get; set; }
    }
}
