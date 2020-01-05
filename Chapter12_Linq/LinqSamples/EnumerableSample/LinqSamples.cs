using DataLib;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EnumerableSample
{
    internal class LinqSamples
    {
        internal static void Register(CommandLineApplication app)
        {
            MethodInfo[] methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Name == nameof(LinqSamples))
                .Single()
                .GetMethods()
                .Where(m => m.IsPublic && m.IsStatic)
                .ToArray();

            foreach (var method in methods)
            {
                app.Command(method.Name.ToLower(), cmd =>
                {
                    cmd.Description = method.Name;
                    cmd.OnExecute(() => { method.Invoke(null, null); return 0; });
                });
            }
        }

        public static void Except()
        {
            var racers = Formula1.GetChampionships().SelectMany(cs => new List<RacerInfo>()
               {
                 new RacerInfo {
                   Year = cs.Year,
                   Position = 1,
                   FirstName = cs.First.FirstName(),
                   LastName = cs.First.LastName()
                 },
                 new RacerInfo {
                   Year = cs.Year,
                   Position = 2,
                   FirstName = cs.Second.FirstName(),
                   LastName = cs.Second.LastName()
                 },
                 new RacerInfo {
                   Year = cs.Year,
                   Position = 3,
                   FirstName = cs.Third.FirstName(),
                   LastName = cs.Third.LastName()
                 }
               });

            var nonChampions = racers.Select(r =>
              new
              {
                  r.FirstName,
                  r.LastName
              }).Except(Formula1.GetChampions().Select(r =>
                new
                {
                    r.FirstName,
                    r.LastName
                }));

            foreach (var r in nonChampions)
            {
                Console.WriteLine($"{r.FirstName} {r.LastName}");
            }
        } 

        // 12.2.13 合并
        public static void ZipOperation()
        {
            var racerNames = from r in Formula1.GetChampions()
                             where r.Country == "Italy"
                             orderby r.Wins descending
                             select new
                             {
                                 Name = r.FirstName + " " + r.LastName
                             };

            var racerNamesAndStarts = from r in Formula1.GetChampions()
                                      where r.Country == "Italy"
                                      orderby r.Wins descending
                                      select new
                                      {
                                          r.LastName,
                                          r.Starts
                                      };
            /*
             * Zip()对于合并，第一个集合中的第一项和第二个集合中的第一项合并，
             * 第一个集合中第二项会与第二个集合中的第二项合并，以此内推。
             * 如果两个序列的项数不同，就在到达较小集合的末尾时停止。
             * 
             * 通过参数first接收第一个集合的元素，通过参数second接收第二个集合的元素。
             */
            var racers = racerNames.Zip(racerNamesAndStarts, (first, second) => first.Name + ", starts: " + second.Starts);
            foreach (var r in racers)
            {
                Console.WriteLine(r);
            }
        }

        // 12.2.15 聚合操作符
        public static void AggregateCount()
        {
            // Count()方法来筛选，只返回获得冠军次数超过3次的赛车手。
            var query = from r in Formula1.GetChampions()
                        let numberYears = r.Years.Count()
                        where numberYears >= 3
                        orderby numberYears descending, r.LastName
                        select new
                        {
                            Name = r.FirstName + " " + r.LastName,
                            TimesChampion = numberYears
                        };

            foreach (var r in query)
            {
                Console.WriteLine($"{r.Name} {r.TimesChampion}");
            }
        }
        public static void AggregateSum()
        {
            /*
             * Sum()方法汇总序列中的所有数字，返回这些数字的和。
             * 
             * 首先根据国家对赛车手分组，再在新创建的匿名类型中，
             * 把Wins属性赋予某个国家赢得比赛的总次数。
             */ 
            var countries = (from c in
                                 from r in Formula1.GetChampions()
                                 group r by r.Country into c
                                 select new
                                 {
                                     Country = c.Key,
                                     Wins = (from r1 in c
                                             select r1.Wins).Sum()
                                 }
                             orderby c.Wins descending, c.Country
                             select c).Take(5);

            foreach (var country in countries)
            {
                Console.WriteLine($"{country.Country} {country.Wins}");
            }
        }

        // 12.2.14 分区
        public static void Partitioning()
        {
            int pageSize = 5;

            int numberPages = (int)Math.Ceiling(Formula1.GetChampions().Count() /
                  (double)pageSize);

            for (int page = 0; page < numberPages; page++)
            {
                Console.WriteLine($"Page {page}");
                /*
                 * 把扩展方法Skip()和Take()添加到查询的最后。
                 * Skip()方法先忽略根据页面大小和实际页数计算出的项数，
                 * 在使用Take()方法根据页面大小提取一定数量的项。
                 * 
                 * 使用TakeWhile()和SkipWhile()扩展方法，还可以传递一个谓词，
                 * 根据结果提取或跳过某些项。
                 */ 
                var racers =
                   (from r in Formula1.GetChampions()
                    orderby r.LastName, r.FirstName
                    select r.FirstName + " " + r.LastName).
                   Skip(page * pageSize).Take(pageSize);

                foreach (var name in racers)
                {
                    Console.WriteLine(name);
                }
                Console.WriteLine();
            }
        }

        // 12.2.12 集合操作
        // Distinct()、Union()、Intersect()、Except()都是集合操作。
        public static void SetOperations()
        {
            IEnumerable<Racer> racersByCar(string car) =>
                from r in Formula1.GetChampions()
                from c in r.Cars
                where c == car
                orderby r.LastName
                select r;

            Console.WriteLine("World champion with Ferrari and McLaren");
            foreach (var racer in racersByCar("Ferrari").Intersect(racersByCar("McLaren")))
            {
                Console.WriteLine(racer);
            }
        }

        // 12.2.16 转换操作符
        public static void ToList()
        {
            /*
             * 查询可以推迟到访问数据项时再执行。在迭代中使用查询时，查询会执行。
             * 而使用转换操作符会立即执行查询，把查询结果放在数组、列表或字典中。
             * 
             * 下面调用ToList()扩展方法，立即执行查询，结果放到List<T> 类中。
             */ 
            List<Racer> racers = (from r in Formula1.GetChampions()
                                  where r.Starts > 200
                                  orderby r.Starts descending
                                  select r).ToList();

            foreach (var racer in racers)
            {
                Console.WriteLine($"{racer} {racer:S}");
            }
        }

        /*
         * 注意：Dictionary<TKey,TValue>类只支持一个键对应一个值。
         * 在Lookup<TKey,TValue>类中，一个键可以对应多个值。
         */
        public static void ToLookup()
        {
            /*
             * 摊平赛车手和赛车序列，创建带有Car和Racer属性的匿名类型。
             * 在返回Lookup对象中，键的类型应是表示汽车的string，值类型应是Racer。
             * 为了进行这个选择，可以给ToLookup()的一个重载版本传递一个键和一个元素选择器。
             * 键选择器引用Car属性，元素选择器引用Racer属性。
             */ 
            var racers = (from r in Formula1.GetChampions()
                          from c in r.Cars
                          select new
                          {
                              Car = c,
                              Racer = r
                          }).ToLookup(cr => cr.Car, cr => cr.Racer);

            if (racers.Contains("Williams"))
            {
                foreach (var williamsRacer in racers["Williams"])
                {
                    Console.WriteLine(williamsRacer);
                }
            }
        }

        public static void ConvertWithCast()
        {
            var list = new System.Collections.ArrayList(Formula1.GetChampions() as System.Collections.ICollection);
            /*
             * 需要在非类型化的集合上使用Linq查询，就可以使用Cast();
             * 
             * 下面基于Object类型的ArrayList集合用Racer对象填充。
             * 为定义强类型化的查询，可使用Cast()方法。
             */
            var query = from r in list.Cast<Racer>()
                        where r.Country == "USA"
                        orderby r.Wins descending
                        select r;
            foreach (var racer in query)
            {
                Console.WriteLine($"{racer:A}");
            }
        }

        // 12.2.17 生成操作符
        public static void GenerateRange()
        {
            /*
             * 生成操作符Range()、Empty()、Repeat()不是扩展方法，
             * 而是返回序列的正常静态方法。
             * 在Linq to Objects中，这些可用于Enumerable类。
             *
             * 有时需要填充一个范围数字，此时就应使用Range()。
             * 把第一个参数作为起始值，把第二个参数作为要填充的项数。
             */

            var values = Enumerable.Range(1, 20);
            foreach (var item in values)
            {
                Console.Write($"{item} ", item);
            }
            Console.WriteLine();

            /*
             * 可以把该结果与其他扩展方法合并起来，获得另一个结果，例
             * var values = Enumerable.Range(1,20).Select(n=>n*3);
             * 
             * Empty()方法返回一个不返回值得迭代器，它可以用于需要一个集合的参数，
             * 其中可以给参数传递空集合。
             * 
             * Repeat()方法返回一个迭代器，该迭代器把同一个值重复特定的次数。
             */
        }

    }
}
