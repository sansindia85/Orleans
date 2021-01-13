using Interfaces;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().GetAwaiter().GetResult();
        }

        static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await StartClient())
                {
                   
                    Console.WriteLine($"Client is initialized: {client.IsInitialized}");
    
                    IHello grain = client.GetGrain<IHello>(0, "key");
                    var response = await grain.SayHelloAsync("Good morning");
                    Console.WriteLine(response);

                    Console.ReadLine();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }            
        }

        static async Task<IClusterClient> StartClient()
        {
            //Configuration for client is done via ClientBuilder Class.
            //Bare client configuration.

            //Clustering Information
            var client = new ClientBuilder()
            //clustering information
            .Configure<ClusterOptions>(options =>
            {

                //This is unique id for Orleans Cluster
                //All the clients and silos use this id should able to talk with each other.
                options.ClusterId = "dev";
                //This is unique id used by our application and used by persistence provider and others.
                //This id should not be changed across deployments.
                options.ServiceId = "HelloApp";
            })

            //clustering provider
            //Client will discover all the gateways available in the cluster using this provider.
            //LocalHostClustering : Used for development and single silo.
            .UseLocalhostClustering()
            .Build();  //Build our configuration

            await client.Connect();
            Console.WriteLine("Client connected.");

            return client;
        }
    }
}
