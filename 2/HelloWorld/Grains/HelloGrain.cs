﻿using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class HelloGrain : Grain, IHello
    {       
        public Task<string> SayHelloAsync(string greeting)
        {
            var primaryKey = this.GetPrimaryKey();

            Console.WriteLine($"This is primary key: {primaryKey}");

            return Task.FromResult($"You said: {greeting}, I say: Hello"); 
        }
    }
}
