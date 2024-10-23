using Service.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.DAO
{
    // Classe => mappée à une table
    public  class FilmDAO : ITimeStamp
    {
        // Propriétés => mappées aux colonnes
        public Guid Id { get; set; } = Guid.NewGuid();


        public string Title { get; set; }
        public string Imdb { get; set; }

        public int Duree { get; set; }
        public DateTime DateSortie { get; set; }

        public DateTime DateCreation { get; set; }
        public DateTime DateModification { get; set; }

        //public GenreDAO Genre { get; set; }
        public virtual ICollection<GenreDAO> Genres { get; set; } = new HashSet<GenreDAO>();

    }
}
