using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Service.ImplementationBDD.ClassesConcretes;
using Service.ImplementationBDD.DAO;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD
{
    // Cette classe est instanciée via DI

    public class FilmServiceBDD : IFilmService
    {
        private readonly DbContextOptions<CinemaContext> options;
        private readonly ModelBuilderDelegate modelSpecs;
        private readonly IMapper mapper;
        private readonly CinemaContext context;

        // La création d'un context necéssite de connaitre les options DbContextOptions
        // Les options sero,t enregistrées  dans le provider
        public FilmServiceBDD(DbContextOptions<CinemaContext> options,
                ModelBuilderDelegate modelSpecs,
                IMapper mapper

            )
        {
            this.options = options;
            this.modelSpecs = modelSpecs;
            this.mapper = mapper;
            // Création de la BDD si elle n'existe pas
            context = new CinemaContext(options, this.modelSpecs);
            context.Database.EnsureCreated();


        }
        public Task<IFilm> ReadAsync(string Imdb)
        {
            throw new NotImplementedException();
        }

        public async Task<IFilm> ReadAsync(ISearchResult searchResult)
        {
            // Je récupère l'id du SearchResult
            var id = ((SearchResult)searchResult).Id;
            var entries = context.ChangeTracker.Entries();
            // Je charge l'entité sans faire appel au tracking
            var dao = await context.Films
                 .AsNoTracking()
                .FirstAsync(c => c.Id == id);

            entries = context.ChangeTracker.Entries();
            var poco = mapper.Map<Film>(dao);
            return poco;
            // Super fastidieux
            //var film=new Film()
            //{
            //    Imdb=dao.Imdb,
            //    Duree=dao.Duree,
            //}

        }

        public Task<IEnumerable<ISearchResult>> SearchFilmAsync(ISearchFilm? critere)
        {

            // A partir de context.Films, je manipule une requete
            IQueryable<FilmDAO> query = context.Films;// SELECT * FROM Tbl_Films   
            if (critere != null)
            {
                if ((critere.Texte != null))
                {
                    query = query.Where(c => c.Title.Contains(critere.Texte));
                    // SELECT * FROM Tbl_Films  WHERE Titre LIKE '%---%'
                }
            }
            // Mappage des données de la table en SerachResult
            var resultats = query.Select(c => new SearchResult()
            {
                Id = c.Id,
                Libelle = c.Title,
                Description = $"Film de {c.Duree} minutes"
            });

            // On renvoit un IEnumerable => La requete n'est pas encore envoyée
            // Avantage : C'est seulement si le code appelant enumere que la requete sera envoyée
            // Inconvénient => Il faut gérer de manière élégante la fermeture du context
            return Task.FromResult(resultats.ToList() as IEnumerable<ISearchResult>);



        }

        public async Task<IFilm> UpdateAsync(IFilm film)
        {
            // Je caste le IFilm en Film
            var poco = ((Film)film);
            var id = ((Film)film).Id;

            // L'enregistrement existe deja dans le tracker
            var entreeExistante = context.ChangeTracker.Entries<FilmDAO>()
                .FirstOrDefault(c => c.Entity.Id == id);

            if (entreeExistante != null)
            {
                // Il n'y a pas l'option AsNoTracking
                // Solution 1 : L'enlever du tracker
                //entreeExistante.State = EntityState.Detached;


                // Solution 2 : Modifier la version existante
                // le mapper peut limiter les propriétés injectées dans entreeExistante
                mapper.Map(poco, entreeExistante.Entity);

                //entreeExistante.Entity.Duree = poco.Duree;
                entreeExistante.Property(c => c.Title).IsModified = false;
                entreeExistante.Entity.DateModification = DateTime.Now;

            }

            else
            {
                // L'enregistrement n'est pas tracke
                // Obtenir le DAO
                var dao = mapper.Map<FilmDAO>(poco);
                // TODO: Ajouter un interceptor au context
                // Qui surveille les MAJ et change la DateModification
                dao.DateModification = DateTime.Now;
                context.Update(dao);
                // Je dis de manière explicite que la colonne title ne doit pas être mise à jour
                context.Entry(dao) .Property(c => c.Title).IsModified = false;
            }

            await context.SaveChangesAsync();
            return film;
        }
    }
}
