using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public interface IFilm
    {
        // Clé métier
        public string Imdb { get; set; }
        public string Titre { get; set; }
        public IEnumerable<IGenre> Genre { get; set; }
        public int Duree { get; set; }
        public DateTime DateSortie { get; set; }
    }
}
