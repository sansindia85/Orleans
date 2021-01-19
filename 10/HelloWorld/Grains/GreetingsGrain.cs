using Interfaces;
using Orleans.EventSourcing;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace Grains
{
    [LogConsistencyProvider(ProviderName = "StateStorage")]
    public class GreetingsGrain : JournaledGrain<GreetingState, GreetingEvent>, IGreetingsGrain
    {
        public async Task<string> SendGreetings(string greetings)
        {
            var state = State.Greeting;

            RaiseEvent(new GreetingEvent { Greeting = greetings });
            await ConfirmEvents();

            return greetings;
        }
    }

    public class GreetingEvent
    {
        public string Greeting { get; set; }
    }

    public class GreetingState
    {
        public string Greeting { get; set; }

        //The @ symbol allows you to use reserved word. 
        public GreetingState Apply(GreetingEvent @event)
        {
            Greeting = @event.Greeting;
            return this;
        }
    }
}
