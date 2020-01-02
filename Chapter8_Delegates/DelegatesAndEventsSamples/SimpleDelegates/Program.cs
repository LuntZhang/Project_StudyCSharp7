using System;

namespace SimpleDelegates
{
    delegate double DoubleOp(double x);
    class Program
    {
        static void Main(string[] args)
        {
            // 委托数组
            DoubleOp[] operations =
            {
                MathOperations.MultiplyByTwo,
                MathOperations.Square
            };
            // Func<double, double>[] operations =
            //{
            //     MathOperations.MultiplyByTwo,
            //     MathOperations.Square
            // };
            for (int i = 0; i < operations.Length; i++)
            {
                Console.WriteLine($"Using operations[{i}]:");
                ProcessAndDisplayNumber(operations[i], 2.0);
                ProcessAndDisplayNumber(operations[i], 7.90);
                ProcessAndDisplayNumber(operations[i], 1.414);
                Console.WriteLine();
                Console.ReadKey();
            }
        }

        static void ProcessAndDisplayNumber(DoubleOp action, double value)
        {
            double result = action(value);
            Console.WriteLine($"value is {value},result of operation is {result}");
            Console.ReadKey();
        }
        //static void ProcessAndDisplayNumber(Func<double,double> action, double value)
        //{
        //    double result = action(value);
        //    Console.WriteLine($"value is {value},result of operation is {result}");
        //    Console.ReadKey();
        //}
    }
}
