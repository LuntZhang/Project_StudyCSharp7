using DataLib;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnumerableSample
{
    class CompoundFromSamples
    {
        internal static void Register(CommandLineApplication app)
        {
            app.Command("compound", cmd =>
            {
                var methodOption = new CommandOption("-m", CommandOptionType.NoValue);
                cmd.Options.AddRange(new[] { methodOption });
                cmd.Description = "compound -[m]";
                cmd.OnExecute(() =>
                {
                    if (methodOption.HasValue())
                    {
                        CompoundFromWithMethods();
                    }
                    else
                    {
                        CompoundFrom();
                    }
                    return 0;
                });
            });
        }

        // 复合的from子句
        public static void CompoundFrom()
        {
            // 第一个from子句返回Racer对象；
            // 第二个from子句访问Racer类中的Cars属性，返回所有类型赛车；
            // 接着在where子句中使用这些赛车筛选驾驶法拉利的所有冠军；
            var ferrariDrivers = from r in Formula1.GetChampions()
                                 from c in r.Cars
                                 where c == "Ferrari"
                                 orderby r.LastName
                                 select $"{r.FirstName} {r.LastName}";

            foreach (var racer in ferrariDrivers)
            {
                Console.WriteLine(racer);
            }
        }

        public static void CompoundFromWithMethods()
        {
            /* 
             * 第一个参数是隐式参数，它从GetChampions()方法中接受Racer对象序列；
             * 
             * 第二个参数是collectionSelector委托，其中定义了内部序列。
             * 在lambda表达式r =>r.Cars中，应返回赛车集合；
             * 
             * 第三个参数是一个委托，现在为每个赛车调用该委托，接收Racer和Car对象。
             * lambda表达式创建了一个匿名类型，它有Racer和Car属性。
             * 
             * SelectMany()方法的结果是摊平了赛车手和赛车的层次结构，
             * 为每辆赛车返回匿名类型的一个新对象集合。
             */

            var ferrariDrivers = Formula1.GetChampions()
                .SelectMany(r => r.Cars, (r, c) => new { Racer = r, Car = c })
                .Where(r => r.Car == "Ferrari")
                .OrderBy(r => r.Racer.LastName)
                .Select(r => r.Racer.FirstName + " " + r.Racer.LastName);
                //.SelectMany(r => r.Cars, (r1, cars) => new { Racer1 = r1, Cars1 = cars })
                //.Where(item => item.Cars1.Contains("Ferrari"))
                //.OrderBy(item => item.Racer1.LastName)
                //.Select(item => $"{item.Racer1.FirstName} {item.Racer1.LastName}");

            foreach (var racer in ferrariDrivers)
            {
                Console.WriteLine(racer);
            }
        }
    }
}
