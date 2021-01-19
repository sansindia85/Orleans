using Interfaces;
using Orleans;
using Orleans.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{

    [StorageProvider]
    public class HelloGrain : Grain<GreetingArchive>, IHello
    {
        
        public async Task<string> SayHelloAsync(string greeting)
        {
            State.Greetings.Add(greeting);

            await WriteStateAsync();

            var primaryKey = this.GetPrimaryKeyLong();

            var output = $"You said: {greeting}, I say: Hello";
            
            return output;
        }        
    }

    public class GreetingArchive
    {
        public List<string> Greetings { get; private set; } = new List<string>();
    }
}
