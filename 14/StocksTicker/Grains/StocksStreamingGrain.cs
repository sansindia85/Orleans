using Interfaces;
using Intrinio.SDK.Api;
using Intrinio.SDK.Client;
using Intrinio.SDK.Model;
using Newtonsoft.Json;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class StocksStreamingGrain : Grain, IStocksStreamingGrain
    {
        public async Task<string> GetPrice()
        {
            Configuration.Default.AddApiKey("api_key", "OjkyMWQyNDg3YjdjNWFlOTA2YjNmM2ExYjFiY2M4Nzlm");
            Configuration.Default.AllowRetries = true;

            var securityApi = new SecurityApi();
           
            RealtimeStockPrice result = await securityApi.GetSecurityRealtimePriceAsync(this.GetPrimaryKeyString(), null);
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            return result.LastPrice.ToString();
        }
    }
}
