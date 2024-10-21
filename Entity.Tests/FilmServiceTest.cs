using Microsoft.Extensions.DependencyInjection;
using Service;
using Service.ImplementationBDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Tests
{
    [TestClass]
    public class FilmServiceTest
    {

        [TestMethod]
        public async Task SearchTest()
        {
            var chaineRecherche = "Rien que";

            // Attention pas de couplage fort
            // var service = new FilmServiceBDD();

            // Obtention du service par l'intermédiare du provider
           // IFilmService service = (IFilmService) DI.Provider.GetService(typeof(IFilmService));
            
       
           var service = DI.Provider.GetRequiredService<IFilmService>();

            var results = await service.SearchFilmAsync();

          


        }
    }
}
