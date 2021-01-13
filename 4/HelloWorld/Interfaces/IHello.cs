using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{    
    public interface IHello : IGrainWithIntegerCompoundKey
    {
        Task<string> SayHelloAsync(string greeting);
    }
}
