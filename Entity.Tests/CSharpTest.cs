namespace Entity.Tests
{
    [TestClass]
    public class CSharpTest
    {
        [TestMethod]
        public void IEnumerableTest()
        {
            var entiers=new List<int> { 1, 2, 3, 4, 10,7,4,7 };

            // Generator
            var petitsEntiers = entiers
                            .Where(c => 
                            c< 10) // Filtrage
                            .Distinct(); // Dédoublonnage => RAM => Zéro

            //Iterator
            foreach (var e in petitsEntiers) {
                Console.WriteLine(e);
            }
             

        }
    }
}