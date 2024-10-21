using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public interface ISearchResult
    {
        public string Libelle { get; set; }
        public string Description { get; set; }
    }
}
