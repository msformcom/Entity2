using Microsoft.Extensions.DependencyInjection;
using Service.ImplementationBDD.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entity.Tests
{
    [TestClass]
    public class CinemaContextTest
    {
        [TestInitialize]
        public void InitializeBDD()
        {
            using (var scope = DI.Provider.CreateScope())
            {

                // La création du scope me permet de gérer les resources utilisées 
                // par tout objet obtenu via le scope
                var context = scope.ServiceProvider.GetRequiredService<CinemaContext>();
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();


                context.Database.Migrate();


                var film = context.Films.FirstOrDefault();

                // Ajouter un genre au film
                var genreSF = context.Genres.FirstOrDefault(c => c.Libelle == "SF");
                film.Genres.Add(genreSF);


                var genreComedie = context.Genres.FirstOrDefault(c => c.Libelle == "Comédie");
                film.Genres.Add(genreComedie);



                var film2 = context.Films.Skip(1).FirstOrDefault();
                film2.Genres.Add(genreComedie);

                context.SaveChanges();
                // Création de la fonction dans la BDD
                context.Database.ExecuteSqlRaw("CREATE OR ALTER FUNCTION FilmsPourAge(@age INT)\r\nRETURNS TABLE\r\nAS RETURN  SELECT DISTINCT dbo.TBL_Films.*\r\nFROM   dbo.TBL_Films INNER JOIN\r\n             dbo.TBL_Films_Genres ON dbo.TBL_Films.PK_Film = dbo.TBL_Films_Genres.FK_Film INNER JOIN\r\n             dbo.TBL_Genres ON dbo.TBL_Films_Genres.FK_Genre = dbo.TBL_Genres.PK_Genre\r\n\t\t\tWHERE AgeMinimum <= @age");
                context.Database.ExecuteSqlRaw("CREATE OR ALTER FUNCTION EuroFrancs(@prix Decimal(18,2))\r\nRETURNS Decimal(18,2)\r\nAS \r\nBEGIN\r\nRETURN @prix*6.55957\r\nEND");
            }
        }

        [TestMethod]
        public void NavigationTest()
        {
            // Scope de l'injection


            // Chargement propriétés de navigation 
            // Chargement eager => with Include
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {
                    var film = context.Films.Include(c => c.Genres).FirstOrDefault();
                    var genres = film.Genres.ToArray();
                }
            }


            // Chargement propriétés de navigation 
            // Chargement eager => with Select
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {
                    var film = context.Films.Select(c =>
                        new
                        {
                            Titre = c.Title,
                            Duree = c.Duree,
                            Genres = c.Genres.Select(c => c.Libelle)
                        }).FirstOrDefault();
                }
            }


            // Chargement propriétés de navigation 
            // Chargement explicit 
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {
                    var film = context.Films.First();
                    if (true)
                    {
                        // Explicit loading => je veux les genres
                        context.Entry(film) // Entrée correspondant au film dans le changetracker
                                .Navigation("Genres").Load();
                        // Si le genre était unique pour un films
                        //context.Entry(film) // Entrée correspondant au film dans le changetracker
                        //       .Reference("Genre").Load();
                        //context.Entry(film) // Entrée correspondant au film dans le changetracker
                        //        .Collection("Genre").Load();

                        Assert.AreEqual(film.Genres.Count(), 2);
                    }



                }
            }


            // Chargement propriétés de navigation 
            // Chargement lasy
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {
                    var film = context.Films.FirstOrDefault();
                    // Je vais utiliser un package LasyLoadingProxies
                    // je vais obtenir un objet de type FilmProxy =>
                    // 1) Herite de FilmDAO => FilmDAO pas sealed
                    // 2) le getter de la propriété de navigation Genres est réécrite 
                    //    la propriete Genres doit être marquée avec virtual
                    //    => dans le getter => Select à la BDD

                    // Le SELECT des genres ca être envoyé maintenant
                    var genres = film.Genres.ToArray();
                }
            }

            // Chargement propriétés de navigation 
            // Chargement lasy
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {
                    // Avec Lasy Loading
                    var film = context.Films.Include(c => c.Genres).ToArray(); // SELECT * FROM Films => 1000 films
                    foreach (var f in film)
                    {
                        var a = f.Genres.Count();  // SELECT.... FROM ... WHERE PK_Film=1
                    }

                }
            }

        }

        [TestMethod]
        public void SplitQueryTest()
        {
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {
                    var query = context.Genres.AsSplitQuery().Include(c => c.Films).AsSplitQuery();


                    var resultats = query.ToArray();
                }
            }
        }

        [TestMethod]
        public void UseFunctionOnServer()
        {
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {


                    var query = context.Genres.Select(c => FonctionsServer.EuroFrancs(c.AgeMinimum));
                    var resultat = query.ToArray();

                    // Le premier genre avec tous les films 
                    // aux personnes dont l'age leur permet de voir
                    var genre = context.Genres.Select(c => new
                    {
                        libelle = c.Libelle,
                        films = FonctionsServer.FilmsPourAge(c.AgeMinimum)
                    }).First();

                }
            }
        }


        [TestMethod]
        public void OptionRecompile()
        {
            using (var scope = DI.Provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<CinemaContext>())
                {
                    // Ajouter un DbCommandInterceptor pour ajouter l'option Recompile
                    // L'intercepteur va agir si il existe un comment -- OPTION RECOMPILE
                    var query = context.Genres.TagWith("OPTION RECOMPILE")
                        .Join(context.Films,g=>g.Id,f=>f.Id,(g,f)=>new
                    {
                        g.Libelle,
                        f.Title
                    });

                    var query2 = context.Genres
                        .AsNoTracking()
                        //.AsSplitQuery()
                        .TagWith("OPTION RECOMPILE").Select(g => new
                    {
                        g.Libelle,
                        films = g.Films
                    });
                    var resultat2=query2.ToArray();

                    var resultats= query.ToArray(); 
                }
            }
        }
    }
}
