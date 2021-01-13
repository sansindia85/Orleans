using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class HelloGrain : Grain, IHello
    {
        public override Task OnActivateAsync()
        {
            Console.WriteLine("OnActivate is called.");
            return base.OnActivateAsync();
        }
        public Task<string> SayHelloAsync(string greeting)
        {
            string keyExtension;
            var primaryKey = this.GetPrimaryKeyLong(out keyExtension);

            //This will call Grain Deactivation. This should not be done.
            this.DeactivateOnIdle();

            Console.WriteLine($"This is primary key {keyExtension}:{primaryKey}");
            return Task.FromResult($"You said: {greeting}, I say: Hello"); 
        }

        public override Task OnDeactivateAsync()
        {
            Console.WriteLine("OnDeActivate is called.");
            return base.OnDeactivateAsync();
        }
    }
}
