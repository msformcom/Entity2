
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Service;
using Service.ImplementationBDD;
using Service.ImplementationBDD.ClassesConcretes;
using Service.ImplementationBDD.DAO;
using Service.Models;
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

            #region Configuration de la journalisation

            // Création et configuration d'un logFactory
            var loggerFactory = LoggerFactory.Create(options =>
            {
                options.AddDebug();
            });
            // Configuration globale du logging
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            #endregion

            #region Config de CinemaContext

            // Ajout des options nécessaires à la construction du FilmServiceBDD
            // Par l'utilisation d'un DbConTextOtionsBuilder
            var optionsBuilder = new DbContextOptionsBuilder<CinemaContext>();
            // Permet de configurer les options pour utiliser le provider de sql server
            optionsBuilder.UseSqlServer(config.GetConnectionString("CinemaBDD")
                
                );
            // Permet de configurer les options de logging
            optionsBuilder.UseLoggerFactory(loggerFactory);

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
                    options.HasKey(c => c.Id);
                    options.ToTable("TBL_Films");
                    options.HasIndex(c => c.Imdb).IsUnique();


                    options.Property(c => c.Id).HasColumnName("PK_Film");
                    options.Property(c => c.Title).HasMaxLength(200).HasColumnName("Titre");
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

                #region Seed

                var F1 = new FilmDAO()
                {
                    Title = "Rien que pour vos cheveux",
                    DateSortie = new DateTime(2005, 11, 11),
                    DateCreation = DateTime.Now,
                    DateModification = DateTime.Now,
                    Duree = 90,
                    Imdb = "987987"
                };
                var F2 = new FilmDAO()
                {
                    Title = "L'odeur de la papaye verte",
                    DateSortie = new DateTime(2005, 11, 11),
                    DateCreation = DateTime.Now,
                    DateModification = DateTime.Now,
                    Duree = 90,
                    Imdb = "8768"
                };



                var G1 = new GenreDAO() { AgeMinimum = 12, Libelle = "Action" };
                var G2 = new GenreDAO() { AgeMinimum = 12, Libelle = "Comédie" };
                var G3 = new GenreDAO() { AgeMinimum = 12, Libelle = "SF" };


                m.Entity<GenreDAO>().HasData(G1, G2, G3);
                m.Entity<FilmDAO>().HasData(F1, F2);



                #endregion




            });
            #endregion


            #region Config du IFilmService
            // FilmServiceBDD sera utilisée pour répondre à une demande de type IFilmService

            // Une seule instance du service sera crée
            //services.AddSingleton<IFilmService, FilmServiceBDD>();

            // Une instance par demande
            //services.AddTransient<IFilmService, FilmServiceBDD>();

            // Une instance par scope
            services.AddScoped<IFilmService, FilmServiceBDD>();

     



            #endregion

            #region Config des mappings entre DAO et les classes concretes
            services.AddAutoMapper(options =>
            {
                // CreateMap() => mappings auto quand les propriétés sont identique
                options.CreateMap<FilmDAO, Film>()

                // Pour la propriété Titre du Film prendre la propriété Title du DAO
                .ForMember(c => c.Titre, o => o.MapFrom(c => c.Title))
                .ReverseMap();

                //var mapping = options.CreateMap<Film, FilmDAO>()
                //mapping.ForAllMembers(o => o.Ignore());
                //mapping.ForMember(c => c.Title, o => o.Ignore());
                //mapping.ForMember(c => c.Duree, o => o.MapFrom(c => c.Duree))
                //         .ForMember(c => c.Id, o => o.MapFrom(c => c.Id))
                //         .ForMember(c=>c.DateModification, o=>o.MapFrom(c=>DateTime.Now))

                ;

            });

            #endregion



            // Je créé un provider 
            Provider = services.BuildServiceProvider();
        }


    }
}
