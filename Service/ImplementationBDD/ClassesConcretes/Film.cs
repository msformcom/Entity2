using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.ClassesConcretes
{
    public class Film : IFilm   // : Inherits ou Implements
    {
        public Guid Id { get; set; }
        public string Imdb { get; set; }
        public string Titre { get; set; }
        public IEnumerable<IGenre> Genre { get; set; }
        public int Duree { get; set; }
        public DateTime DateSortie { get; set; }
    }
}
