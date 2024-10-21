using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.DAO
{
    public class CinemaContext : DbContext
    {
        // DbContextOptions est un objet qui permet au context de connaître
        // - Son provider
        // Les options de connection (serveur,nom de bdd, authentification)
        public CinemaContext(DbContextOptions options) : base(options)
        {
            
        }
        // Ajouter une table pour les films
        internal DbSet<FilmDAO> Films { get; set; }
    }
}
