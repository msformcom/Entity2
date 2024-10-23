using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.ClassesConcretes
{
    public class SearchResult : ISearchResult
    {
        public Guid Id { get; set; }
        public string Libelle { get; set; }
        public string Description { get; set; }
    }
}
