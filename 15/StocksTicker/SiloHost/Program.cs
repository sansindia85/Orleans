using Grains;
using Interfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;

namespace SiloHost
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunSilo().Result;
        }

        private static async Task<int> RunSilo()
        {
            try
            {
                var host = await StartSilo();

                Console.WriteLine("Press Enter to terminate silo...");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }           
        }

        public static async Task<ISiloHost> StartSilo()
        {
            var silo = new SiloHostBuilder()
                      .UseLocalhostClustering()
                      .Configure<ClusterOptions>(options =>
                      {
                          options.ClusterId = "dev";
                          options.ServiceId = "StocksTickerApp";
                      })
                      //Endpoints
                      .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = System.Net.IPAddress.Loopback)
                      //Application parts: just reference one of the grain implementations that we use
                      .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(StocksStreamingGrain).Assembly).WithReferences())
                      //Now create the Silo!
                      .Build();

            await silo.StartAsync();

            return silo;
        }
    }
}
