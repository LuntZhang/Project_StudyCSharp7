using DataLib;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Linq;

namespace EnumerableSample
{
    class GroupingSamples
    {
        internal static void Register(CommandLineApplication app)
        {
            app.Command("group", cmd =>
            {
                var methodOption = new CommandOption("-m", CommandOptionType.NoValue);
                var variableOption = new CommandOption("-v", CommandOptionType.NoValue);
                var anonymousOption = new CommandOption("-a", CommandOptionType.NoValue);
                var nestedOption = new CommandOption("-n", CommandOptionType.NoValue);
                var nestedMethodOption = new CommandOption("-nm", CommandOptionType.NoValue);
                cmd.Options.AddRange(new[] { methodOption, variableOption, anonymousOption, nestedOption, nestedMethodOption });
                cmd.Description = "group -[m|v|a|n|nm]";
                cmd.OnExecute(() =>
                {
                    if (methodOption.HasValue())
                    {
                        GroupingWithMethods();
                    }
                    else if (variableOption.HasValue())
                    {
                        GroupingWithVariables();
                    }
                    else if (anonymousOption.HasValue())
                    {
                        GroupingWithAnonymousTypes();
                    }
                    else if (nestedOption.HasValue())
                    {
                        GroupingAndNestedObjects();
                    }
                    else if (nestedMethodOption.HasValue())
                    {
                        GroupingAndNestedObjectsWithMethods();
                    }
                    else
                    {
                        Grouping();
                    }
                    return 0;
                });
            });
        }

        // 分组
        public static void Grouping()
        {
            /*
             * 子句group r by r.Country into g根据Country属性组合所有的赛车手
             * 并定义一个新的标识符g，它以后用于访问分组的结果信息。
             * 
             * group子句的结果根据应用到的分组结果上的扩展方法Count()来排序，
             * 如果冠军数相同，就根据关键字来排序，该关键字分组所用的关键字国家。
             * 
             * where子句根据至少有两项的分组来筛选结果，
             * select子句创建一个带Country和Count属性的匿名类型
             */
             
            var countries =
                from r in Formula1.GetChampions()
                group r by r.Country into g
                orderby g.Count() descending, g.Key
                where g.Count() > 2
                select new
                {
                    Country = g.Key,
                    Count = g.Count()
                };

            foreach (var item in countries)
            {
                Console.WriteLine($"{item.Country,-10} {item.Count}");
            }

        }

        public static void GroupingWithMethods()
        {
            var countries = Formula1.GetChampions()
              .GroupBy(r => r.Country)
              .OrderByDescending(g => g.Count())
              .ThenBy(g => g.Key)
              .Where(g => g.Count() >= 2)
              .Select(g => new
              {
                  Country = g.Key,
                  Count = g.Count()
              });


            foreach (var item in countries)
            {
                Console.WriteLine($"{item.Country,-10} {item.Count}");
            }

        }

        public static void GroupingWithVariables()
        {
           /*
            * 在上面编写的Linq查询中，Count方法调用了多次。
            * 使用let子句可以改变这种方式。
            * let允许在linq查询中定义变量
            */

            var countries = from r in Formula1.GetChampions()
                            group r by r.Country into g
                            let count = g.Count()
                            orderby count descending, g.Key
                            where count >= 2
                            select new
                            {
                                Country = g.Key,
                                Count = count
                            };

            foreach (var item in countries)
            {
                Console.WriteLine($"{item.Country,-10} {item.Count}");
            }

        }

        public static void GroupingWithAnonymousTypes()
        {
            /*
             * 为了定义传递给下一个方法的额外数据，可以使用Select方法来创建匿名类型。
             * 这里创建了一个带Group和Count属性的匿名类型。
             * 带有这些属性的一组项传递给OrderByDescending方法，基于匿名类型的Count属性排序。
             */
            var countries = Formula1.GetChampions()
              .GroupBy(r => r.Country)
              .Select(g => new { Group = g, Count = g.Count() })
              .OrderByDescending(g => g.Count)
              .ThenBy(g => g.Group.Key)
              .Where(g => g.Count >= 2)
              .Select(g => new
              {
                  Country = g.Group.Key,
                  g.Count
              });

            foreach (var item in countries)
            {
                Console.WriteLine($"{item.Country,-10} {item.Count}");
            }
        }

        public static void GroupingAndNestedObjects()
        {
            var countries = from r in Formula1.GetChampions()
                            group r by r.Country into g
                            let count = g.Count()
                            orderby count descending, g.Key
                            where count >= 2
                            select new
                            {
                                Country = g.Key,
                                Count = count,
                                Racers = from r1 in g
                                         orderby r1.LastName
                                         select r1.FirstName + " " + r1.LastName
                            };
            foreach (var item in countries)
            {
                Console.WriteLine($"{item.Country,-10} {item.Count}");
                foreach (var name in item.Racers)
                {
                    Console.Write($"{name}; ");
                }
                Console.WriteLine();
            }
        }

        public static void GroupingAndNestedObjectsWithMethods()
        {
            var countries = Formula1.GetChampions()
                .GroupBy(r => r.Country)
                .Select(g => new
                {
                    Group = g,
                    g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Key)
                .Where(g => g.Count >= 2)
                .Select(g => new
                {
                    Country = g.Key,
                    g.Count,
                    Racers = g.Group.OrderBy(r => r.LastName).Select(r => r.FirstName + " " + r.LastName)
                });

            foreach (var item in countries)
            {
                Console.WriteLine($"{item.Country,-10} {item.Count}");
                foreach (var name in item.Racers)
                {
                    Console.Write($"{name}; ");
                }
                Console.WriteLine();
            }
        }
    }
}
