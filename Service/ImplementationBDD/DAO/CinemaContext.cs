using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.DAO
{
    // Delegue : Type associé à une Signature de fonction
    // de délégué represente une fonction qui permet d'agir sur le modelBuilder
    public delegate void ModelBuilderDelegate(ModelBuilder modelBuilder);


    public class CinemaContext : DbContext
    {
        private readonly ModelBuilderDelegate modelSpecs;

        // DbContextOptions est un objet qui permet au context de connaître
        // - Son provider
        // Les options de connection (serveur,nom de bdd, authentification)
        public CinemaContext(DbContextOptions<CinemaContext> options,
                ModelBuilderDelegate modelSpecs
            
            ) : base(options)
        {
            this.modelSpecs = modelSpecs;
        }
        // Ajouter une table pour les films
        internal DbSet<FilmDAO> Films { get; set; }
        internal DbSet<GenreDAO> Genres { get; set; }

        // Cette méthode permet de donner les specification de la BDD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // En exécutant la fonction passée en paramètre de construction
            // je vais modifier le model
            this.modelSpecs(modelBuilder);

        }

    }
}
