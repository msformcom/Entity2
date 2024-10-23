using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IFilmService
    {
        // SCRUD
        Task<IEnumerable<ISearchResult>> SearchFilmAsync(ISearchFilm? critere=null);

        public Task<IFilm> ReadAsync(string Imdb);
        public Task<IFilm>  ReadAsync(ISearchResult searchResult);

        public Task<IFilm> UpdateAsync(IFilm film);

    }
}
