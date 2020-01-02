using System;

namespace LambdaExpressions
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleDemos();
        }

        static void SimpleDemos()
        {
            Console.WriteLine(nameof(SimpleDemos));
            Func<string, string> oneParm = s => $"change uppercase {s.ToUpper()}";
            Console.WriteLine(oneParm("test"));

            Func<double, double, double> twoParams = (x, y) => x * y;
            Console.WriteLine(twoParams(3,2));

            Func<double, double, double> twoParamsWithTypes = (double x, double y) => x * y;
            Console.WriteLine(twoParamsWithTypes(4, 2));

        }

        static void ClosureWithModification()
        {
            Console.WriteLine(nameof(ClosureWithModification));
            int someVal = 5;
            Func<int, int> f = x => x + someVal;

            someVal = 7;

            Console.WriteLine(f(3));
            Console.WriteLine();
        }

    }
}
