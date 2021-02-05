using Orleans;
using System;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IStocksStreamingGrain : IGrainWithStringKey
    {
        Task<string> GetPrice();
    }
}
