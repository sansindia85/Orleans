using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class HelloGrain : Grain, IHello
    {       
        public Task<string> SayHelloAsync(string greeting)
        {
            string keyExtension;
            var primaryKey = this.GetPrimaryKeyLong(out keyExtension);

            Console.WriteLine($"This is primary key {keyExtension}:{primaryKey}");

            return Task.FromResult($"You said: {greeting}, I say: Hello"); 
        }
    }
}
