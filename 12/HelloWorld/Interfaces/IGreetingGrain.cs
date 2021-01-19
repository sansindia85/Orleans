using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IGreetingsGrain : IGrainWithIntegerKey
    {
        Task<string> SendGreetings(string greetings);
    }
}
