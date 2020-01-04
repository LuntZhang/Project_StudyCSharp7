using DataLib;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Linq;

namespace EnumerableSample
{
    public class FilteringSamples
    {
        internal static void Register(CommandLineApplication app)
        {
            app.Command("filter", cmd =>
            {
                var invokeMethodOption = new CommandOption("-m", CommandOptionType.NoValue);
                var indexOption = new CommandOption("-i", CommandOptionType.NoValue);
                var typeOption = new CommandOption("-t", CommandOptionType.NoValue);
                cmd.Options.AddRange(new[] { invokeMethodOption, indexOption, typeOption });
                cmd.Description = "filter -[m|i|t]";
                cmd.OnExecute(() =>
                {
                    if (invokeMethodOption.HasValue())
                    {
                        FilteringWithMethods();
                    }
                    else if (indexOption.HasValue())
                    {
                        FilteringWithIndex();
                    }
                    else if (typeOption.HasValue())
                    {
                        TypeFiltering();
                    }
                    else
                    {
                        Filtering();
                    }
                    return 0;
                });
            });
        }

        public static void Filtering()
        {
            var racers = from r in Formula1.GetChampions()
                         where r.Wins > 15 && (r.Country == "Brazil" || r.Country == "Austria")
                         select r;

            foreach (var r in racers)
            {
                Console.WriteLine($"{r:A}");
            }
        }

        // 用索引筛选
        public static void FilteringWithIndex()
        {
            // 使用索引返回姓氏以A开头、索引为偶数的赛车手
            var racers = Formula1.GetChampions()
                .Where((r,index)=>r.LastName.StartsWith("A") && index % 2 !=0);
            foreach (var r in racers)
            {
                Console.WriteLine($"{r:A}");
            }
        }

        public static void FilteringWithMethods()
        {
            var racers = Formula1.GetChampions()
                .Where(r => r.Wins > 15 && (r.Country == "Brazil" || r.Country == "Austria"));

            foreach (var r in racers)
            {
                Console.WriteLine($"{r:A}");
            }
        }

        // 类型筛选
        public static void TypeFiltering()
        {
            object[] data = { "one", 2, 3, "four", "five", 6 };
            // 使用OfType()把string类传给泛型参数，就从集合中仅返回字符串
            var query = data.OfType<string>();
            foreach (var s in query)
            {
                Console.WriteLine(s);
            }
        }
    }
}
