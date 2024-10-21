namespace Entity.Tests
{
    [TestClass]
    public class CSharpTest
    {
        [TestMethod]
        public void IEnumerableTest()
        {
            // SOurce des données
            var entiers=new List<int> { 1, 2, 3, 4, 10,7,4,7 };

            // Linq to object => Lonq to Entities => travaille sur une table
            // Generator
            var petitsEntiers = entiers
                            .Where(c => 
                            c< 10) // Filtrage
                            .Distinct(); // Dédoublonnage => RAM => Zéro

            var c = petitsEntiers.Count();

            //Iterator
            foreach (var e in petitsEntiers) {
                Console.WriteLine(e);
            }

            var enumerator=petitsEntiers.GetEnumerator(); // Position : -1
            while (enumerator.MoveNext()) { 
                var e=enumerator.Current;
                Console.WriteLine(e);
            }   

        }

        IEnumerable<int> EntiersPositifs()
        {
            int i = 0;
           while(true)
            {
                yield return i;
                i++;
            }

        }
        [TestMethod]
        public void YieldReturn()
        {
            var elements = EntiersPositifs();

            var iterator1 = elements.GetEnumerator();
            var iterator2 = elements.GetEnumerator();

            iterator2.MoveNext();
            iterator2.MoveNext();
            var i = iterator2.Current;


            //elements = Enumerable.Range(0, 10);
            // est-il possible d'avoir un objet qui représente tous les entiers positifs ?
            foreach (var e in elements.Take(10))
            {
                Console.WriteLine(e);

            }
        }

        async Task<IEnumerable<int>> GetIntsAsync()
        {
            await Task.Delay(3000);
            return new List<int>() { 1, 2, 3 }.AsEnumerable();
        }

        async IAsyncEnumerable<int> GetInts()
        {
            var i = 0;
            while (true)
            {
                await Task.Delay(3000);
                yield return i;
                i++;
            }

        }

        [TestMethod]
        public async Task IAsyncEnumerableTest()
        {
            // L'attente se situe au niveau de l'obtention du IEnumerable
            var ints = await GetIntsAsync();
            var enumerator = ints.GetEnumerator();
            while (enumerator.MoveNext()) { 
            
            }
            // L'attente se situe au niveau de l'enumeration
            var ints2 = GetInts();
            var asynEnumerator = ints2.GetAsyncEnumerator();
            while (await asynEnumerator.MoveNextAsync())
            {

            }
        }
    }
}