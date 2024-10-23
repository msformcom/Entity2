using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.DAO
{
    public interface ITimeStamp
    {
        public DateTime DateCreation { get; set; }
        public DateTime DateModification { get; set; }
    }
}
