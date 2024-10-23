using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImplementationBDD.DAO
{
    // Intercepter les commandes sortantes et ajouter
    // OPTIONS(RECOMPILE) à la fin de la requete
    // si commentaire -- OPTION RECOMPILE
    public class RecompileInterceptor : DbCommandInterceptor
    {
        // Iterceptions pour la date de modification
        // et la date de creation
        public override DbCommand CommandInitialized(CommandEndEventData eventData, DbCommand result)
        {
            var context = eventData.Context;
            // recherche des insertions dans le changetracker
            var insertions=context.ChangeTracker.Entries()
                    .Where(c => c.State == Microsoft.EntityFrameworkCore.EntityState.Added);
            foreach (var e in insertions) { 
                if(e is ITimeStamp)
                {
                    ((ITimeStamp)e).DateCreation=DateTime.Now;
                }
            }

            var maj = context.ChangeTracker.Entries()
        .Where(c => c.State == Microsoft.EntityFrameworkCore.EntityState.Modified);
            foreach (var e in maj)
            {
                if (e is ITimeStamp)
                {
                    ((ITimeStamp)e).DateModification = DateTime.Now;
                    e.Property("DateCreation").IsModified = false;
                }
            }
            return base.CommandInitialized(eventData, result);
        }
        // Avant qu'une commande ramenant des enregistrement ne soit exécuté
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            if(command.CommandText.StartsWith("-- OPTION RECOMPILE"))
            {
                command.CommandText += " OPTION (RECOMPILE)";
            }
            return base.ReaderExecuting(command, eventData, result);
        }
    }
}
