using Microsoft.Extensions.Configuration;
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
        //[TestInitialize]
        //void Initialize()
        //{
        //    #region S'assurer que la BDD est créée
            


        //    #endregion
        //}

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
            // results est un IEnumerable 
            // Peut etre une liste avec les resultat
            // Peut être un IEnumerable qui va générer les résultat
          
            var resultats=results.ToList();

            Assert.AreEqual(resultats.Count(), 2);

            // PremierItem est ici déclaré en tant que ISerachResult
            // => Pas d'id
            // La classe à l'intérieur de cette variable est un SearchResult => Id
            var PremierItem = resultats.First();
           
            var PremierFilm = await service.ReadAsync(PremierItem);

            PremierFilm.Duree += 1;

            await service.UpdateAsync(PremierFilm);

            var  PremierFilmEnBDD = await service.ReadAsync(PremierItem);
        }
    }
}
