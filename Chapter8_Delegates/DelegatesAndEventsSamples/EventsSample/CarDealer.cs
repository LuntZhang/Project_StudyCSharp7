﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EventsSample
{
    public class CarInfoEventArgs : EventArgs
    {
        public CarInfoEventArgs(string car) => Car = car;
        public string Car { get; }
    }
    public class CarDealer
    {
        public event EventHandler<CarInfoEventArgs> NewCarInfo;
        public void NewCar(string car)
        {
            Console.WriteLine($"CarDealer,new car {car}");
            NewCarInfo?.Invoke(this, new CarInfoEventArgs(car));
        }
    }
}
