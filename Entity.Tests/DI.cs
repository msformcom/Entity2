
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service;
using Service.ImplementationBDD;
using Service.ImplementationBDD.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Tests
{
    // Injection de dépendance de mon application
    public class DI
    {
        public static IServiceProvider Provider;
        static DI()
        {
            // Créer et configurer le provider

            var services = new ServiceCollection();

            #region Configuration de la config
            // Objectif : permettre à l'injection de dépendance de savoir où est la config
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            var config = configBuilder.Build();
            // Ajout de la config générée à l'injection de dépendance
            services.AddSingleton<IConfiguration>(config);

            #endregion


            #region Config du IFilmService
            // FilmServiceBDD sera utilisée pour répondre à une demande de type IFilmService

            // Une seule instance du service sera crée
            //services.AddSingleton<IFilmService, FilmServiceBDD>();

            // Une instance par demande
            //services.AddTransient<IFilmService, FilmServiceBDD>();

            // Une instance par scope
            services.AddScoped<IFilmService, FilmServiceBDD>();

            // Ajout des options nécessaires à la construction du FilmServiceBDD
            // Par l'utilisation d'un DbConTextOtionsBuilder
            var optionsBuilder = new DbContextOptionsBuilder<CinemaContext>();
            // Permet de configurer les options pour utiliser le provider de sql server
            optionsBuilder.UseSqlServer(config.GetConnectionString("CinemaBDD"));

            // Ici : Configurer le noms des tables...


            var optionsContext = optionsBuilder.Options;
            services.AddSingleton<DbContextOptions<CinemaContext>>(optionsContext);

            // Passage des specification du model sous la forme d'une fonction
            services.AddSingleton<ModelBuilderDelegate>(m =>
            {
                // m est du type ModelBuilder
                // Options concernant la table associée à FilmDAO
                m.Entity<FilmDAO>(options =>
                {
                    options.HasKey(c=>c.Id);
                    options.ToTable("TBL_Films");

                    options.Property(c => c.Id).HasColumnName("PK_Film");
                    options.Property(c => c.Titre).HasMaxLength(200);
                    options.Property(c => c.Imdb).HasMaxLength(20);
                    options.Property(c => c.DateSortie).HasColumnType("DATE");

                    var tableCorrespondance = options.HasMany(c => c.Genres).WithMany(c => c.Films);
                    tableCorrespondance.UsingEntity(
                    "TBL_Films_Genres",
                    l => l.HasOne(typeof(GenreDAO)).WithMany().HasForeignKey("FK_Genre").HasPrincipalKey(nameof(GenreDAO.Id)),
                    r => r.HasOne(typeof(FilmDAO)).WithMany().HasForeignKey("FK_Film").HasPrincipalKey(nameof(FilmDAO.Id)),
                    j => j.HasKey("FK_Film", "FK_Genre"));


                });
                m.Entity<GenreDAO>(options =>
                {
                    options.HasKey(c => c.Id);
                    options.ToTable("TBL_Genres");

                    options.Property(c => c.Id).HasColumnName("PK_Genre");
                    options.Property(c => c.Libelle).HasMaxLength(100);

                });


            });



            #endregion





            // Je créé un provider 
            Provider = services.BuildServiceProvider();
        }


    }
}
