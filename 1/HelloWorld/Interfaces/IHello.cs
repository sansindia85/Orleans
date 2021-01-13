using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{    
    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHelloAsync(string greeting);
    }
}
