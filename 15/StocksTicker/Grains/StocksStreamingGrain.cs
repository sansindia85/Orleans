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
        private RealtimeStockPrice _realTimeStockPrice;

        public override async Task OnActivateAsync()
        {
            var ticker = this.GetPrimaryKeyString();

            await UpdatePrice(ticker);

            RegisterTimer(UpdatePrice, ticker, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            await base.OnActivateAsync();
        }
        private async Task UpdatePrice(object stock)
        {
            var price = await GetPriceQuote(stock as string);
            Console.WriteLine(price);
        }
        public async Task<string> GetPriceQuote(string ticker)
        {
            Configuration.Default.AddApiKey("api_key", "OjkyMWQyNDg3YjdjNWFlOTA2YjNmM2ExYjFiY2M4Nzlm");
            Configuration.Default.AllowRetries = true;

            var securityApi = new SecurityApi();

            _realTimeStockPrice = await securityApi.GetSecurityRealtimePriceAsync(this.GetPrimaryKeyString(), null);
            Console.WriteLine(JsonConvert.SerializeObject(_realTimeStockPrice, Formatting.Indented));

            return _realTimeStockPrice.LastPrice.ToString();
        }

        public Task<string> GetPrice()
        {
            return Task.FromResult(_realTimeStockPrice.LastPrice.ToString());
        }
    }
}
