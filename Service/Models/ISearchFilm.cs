using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public interface ISearchFilm
    {
        public string? Texte { get; set; }
        public int? AgeMinimum { get; set; }
    }
}
