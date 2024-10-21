using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Tests
{
    public static class MesExtensions
    {
        // Fonction extension
        public static string Ellipsis(this string value, int maxLength=20) { 
            if(value.Length <= maxLength) return value;
            return $"{value.Substring(0,maxLength-3)}..."; 
        }
    }

    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod] 
        public void EllisisTest()
        {
            var chaine = "La vie est un long fleuve tranquille";
            // "La vie est un l...";
         
            var chaineRaccourcie = MesExtensions.Ellipsis(chaine,10);
            // Syntaxic sugar
            chaineRaccourcie = chaine.Ellipsis(10); // => MesExtensions.Ellipsis(chaine,10);
        }
    }
}
