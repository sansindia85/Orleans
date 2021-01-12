using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SiloHost
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await RunSilo();
        }

        private static async Task<int> RunSilo()
        {
            try
            {
                await StartSilo();
                Console.WriteLine("Silo started.");
                
                Console.WriteLine("Press enter to terminate.");
                Console.ReadLine();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            //Configuration is provided via Silo Host Builder Class
            var builder = new SiloHostBuilder()

                //Clustering Information
                .Configure<ClusterOptions>(options =>
                {
                    //This is unique ID for Orleans Cluster
                    //All Clients and Silos that use this ID will be able to talk to each other.
                    options.ClusterId = "dev";
                    //This is unique ID for our application that will be used by Provider.
                    //For example : Persistent Provider
                    //This ID should be stable and should not be changed across deployments
                    options.ServiceId = "HelloApp";
                })

                //Clustering Provider
                //EndPoints : Silo to Silo and Client to Silo communication
                //The client will discover all the Gateways available in the cluster using this provider.
                //There are many other providers such as ADO.Net
                //Localhost : For Development and single silo
                .UseLocalhostClustering()

                //EndPoints : Silo to Silo endpoints used for communciation between Silos in the same cluster.
                //            Client to Silo endpoints used for communiation between Clients and Silos in the same cluster.
                //We set the communcation gateway and Orleans Silo
                .Configure<EndpointOptions>(options =>
                {
                    //This is for Silo to Silo
                    options.SiloPort = 11111; //Default value
                    //This is for Client to Silo
                    options.GatewayPort = 30000; //Default value
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                });

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
