using Interfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Runtime.Messaging;
using Polly;
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
                using (var client = StartClient())
                {
                    var grain = client.GetGrain<IGreetingsGrain>(0);
                    await grain.SendGreetings("Hello");

                    var grain1 = client.GetGrain<IGreetingsGrain>(0);
                    await grain1.SendGreetings("Morning");

                    var grain2 = client.GetGrain<IGreetingsGrain>(0);
                    await grain1.SendGreetings("Afternoon");

                    //Console.WriteLine($"Client is initialized: {client.IsInitialized}");

                    //RequestContext.Set("traceId", Guid.NewGuid());   
                    //var grain = client.GetGrain<IHello>(0);
                    //var response = await grain.SayHelloAsync("Good morning");

                    //var grain1 = client.GetGrain<IHello>(1);                    
                    //var response1 = await grain1.SayHelloAsync("Good afternoon");
                    //var response2 = await grain1.SayHelloAsync("Good midday");

                    //Console.WriteLine(response);

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

        static IClusterClient StartClient()
        {
            return Policy<IClusterClient>
                .Handle<SiloUnavailableException>()                                 
                .Or<OrleansMessageRejectionException>() //Silo is unavailable and client is trying to connect to it.
                .Or<ConnectionFailedException>()
                .WaitAndRetry(
                   new[]
                   {
                       TimeSpan.FromSeconds(2),
                       TimeSpan.FromSeconds(4)
                   })
                .Execute(() =>
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

                   client.Connect().GetAwaiter().GetResult();
                   Console.WriteLine("Client connected.");

                   return client;

               });
            
        }
    }
}
