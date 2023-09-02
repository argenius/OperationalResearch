namespace KnapsackProblem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
                 zaino = Zaino(39)
    valoriEPesi = [
        [52, 10],
        [27, 6],
        [50, 15],
        [60, 22],
        [31, 17],
        [11, 14]
    ]
             */
            var items = new List<KnapsackItem>()
            {
                new KnapsackItem(1, 52,10),
                new KnapsackItem(2, 27,6),
                new KnapsackItem(3, 50,15),
                new KnapsackItem(4, 60,22),
                new KnapsackItem(5, 31,17),
                new KnapsackItem(6, 11,14)
            };
            var problem = new KnapsackProblem(39, items);
            problem.Resolve();
        }
    }
}