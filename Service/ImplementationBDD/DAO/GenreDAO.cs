using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.DAO
{
    public class GenreDAO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Libelle { get; set; }
        public int AgeMinimum { get; set; }

        // Un GenreDAO est associée à un ensemble de Film
        public ICollection<FilmDAO> Films { get; set; }

    }
}
