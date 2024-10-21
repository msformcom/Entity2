using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public interface IGenre
    {
        public string Libelle { get; set; }
        public int AgeMinimum { get; set; }

        public IEnumerable<IFilm> Films { get; set; }
    }
}
