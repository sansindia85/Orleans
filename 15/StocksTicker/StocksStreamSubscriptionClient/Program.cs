using Grains;
using Interfaces;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace StocksStreamSubscriptionClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = InitClient().Result;

            var grain = client.GetGrain<IStocksStreamingGrain>("AAPL");
            var price = grain.GetPrice().GetAwaiter().GetResult();

            Console.WriteLine(price);

            Console.ReadLine();
        }

      

        private static async Task<IClusterClient> InitClient()
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "StocksTickerApp";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IStocksStreamingGrain).Assembly))
                .Build();

            await client.Connect();
            Console.WriteLine("Client succefully connected to silo host...");
            return client;
        }
    }
}
