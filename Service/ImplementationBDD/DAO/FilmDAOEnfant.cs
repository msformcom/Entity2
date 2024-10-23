using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.DAO
{
    public class FilmDAOEnfant : FilmDAO
    {
        public override ICollection<GenreDAO> Genres { get => base.Genres; set => base.Genres = value; }
    }
}
