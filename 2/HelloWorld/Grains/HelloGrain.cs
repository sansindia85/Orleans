using Interfaces;
using Orleans;
using System.Threading.Tasks;

namespace Grains
{
    public class HelloGrain : Grain, IHello
    {       
        public Task<string> SayHelloAsync(string greeting)
        {
            return Task.FromResult($"You said: {greeting}, I say: Hello"); 
        }
    }
}
