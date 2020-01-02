using System;
using System.Collections.Generic;
using System.Text;

namespace BubbleSorter
{
    class Employee
    {
        public Employee(string name,decimal salary)
        {
            Name = name;
            Salary = salary;
        }

        public string Name { get; }
        public decimal Salary { get; }
        public override string ToString() => $"{Name},{Salary}";
        public static bool CompareSalary(Employee e1, Employee e2) => e1.Salary < e2.Salary;
    }
}
