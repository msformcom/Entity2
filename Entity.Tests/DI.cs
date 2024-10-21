
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service;
using Service.ImplementationBDD;
using Service.ImplementationBDD.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Tests
{
    // Injection de dépendance de mon application
    public class DI
    {
        public static IServiceProvider Provider;
        static DI()
        {
            // Créer et configurer le provider

            var services = new ServiceCollection();
            // FilmServiceBDD sera utilisée pour répondre à une demande de type IFilmService
            
            // Une seule instance du service sera crée
            //services.AddSingleton<IFilmService, FilmServiceBDD>();

            // Une instance par demande
            //services.AddTransient<IFilmService, FilmServiceBDD>();

            // Une instance par scope
            services.AddScoped<IFilmService, FilmServiceBDD>();

            // Ajout des options nécessaires à la construction du FilmServiceBDD
            // Par l'utilisation d'un DbConTextOtionsBuilder
            var optionsBuilder = new DbContextOptionsBuilder();
            // Permet de configurer les options pour utiliser le provider de sql server
            optionsBuilder.UseSqlServer()



            // Je créé un provider 
            Provider = services.BuildServiceProvider();
        }


    }
}
