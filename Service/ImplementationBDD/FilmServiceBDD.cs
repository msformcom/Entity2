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
        private readonly DbContextOptions options;

        // La création d'un context necéssite de connaitre les options DbContextOptions
        // Les options sero,t enregistrées  dans le provider
        public FilmServiceBDD(DbContextOptions options)
        {
            this.options = options;
        }
        public Task<IFilm> ReadAsync(string Imdb)
        {
            throw new NotImplementedException();
        }

        public Task<IFilm> ReadAsync(ISearchResult searchResult)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ISearchResult>> SearchFilmAsync(ISearchFilm? critere)
        {
            var context = new CinemaContext(this.options);
          
                // A partir de context.Films, je manipule une requete
                IQueryable<FilmDAO> query = context.Films;// SELECT * FROM Tbl_Films   
                if (critere != null)
                {
                    if ((critere.Texte != null))
                    {
                        query = query.Where(c => c.Titre.Contains(critere.Texte));
                        // SELECT * FROM Tbl_Films  WHERE Titre LIKE '%---%'
                    }
                }
                // Mappage des données de la table en SerachResult
                var resultats = query.Select(c => new SearchResult()
                {
                    Libelle = c.Titre,
                    Description = $"Film de {c.Duree} minutes"
                });
                // On renvoit un IEnumerable => La requete n'est pas encore envoyée
                // Avantage : C'est seulement si le code appelant enumere que la requete sera envoyée
                // Inconvénient => Il faut gérer de manière élégante la fermeture du context
                return Task.FromResult(resultats as IEnumerable<ISearchResult>);
            

     
        }
    }
}
