using System;

namespace AnonymousMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            string mid = ",middle part,";
            Func<string, string> anonDel = delegate (string param)
             {
                 param += mid;
                 param += " and this was added to the string";
                 return param;
             };
            Console.WriteLine(anonDel("start of string"));
            Console.ReadKey();
        }
    }
}
