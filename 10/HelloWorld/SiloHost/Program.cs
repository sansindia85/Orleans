using Grains;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using SiloHost.Filters;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SiloHost.Context;

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
                var host = await StartSilo();

                Console.WriteLine("Silo started.");                
                Console.WriteLine("Press enter to terminate.");
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

        private static async Task<ISiloHost> StartSilo()
        {
            var configuration = LoadConfiguration();
            var orleansConfiguration = GetOrleansConfiguration(configuration);

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

               .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
               
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
                })
                .UseDashboard()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IOrleansRequestContext, OrleansRequestContext>();
                    services.AddSingleton(s => CreateGrainMethodsList());
                    services.AddSingleton(s => new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.None,
                        TypeNameHandling = TypeNameHandling.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                })
                .AddStateStorageBasedLogConsistencyProvider("StateStorage")
                .AddIncomingGrainCallFilter<LoggingFilter>()
                .AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.Invariant = orleansConfiguration.Invariant;
                    options.ConnectionString = orleansConfiguration.ConnectionString;
                    options.UseJsonFormat = true;
                })
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        private static GrainInformation CreateGrainMethodsList()
        {
            var grainInterfaces = typeof(IHello).Assembly.GetTypes()
                                                         .Where(type => type.IsInterface)
                                                         .SelectMany(type => type.GetMethods())
                                                         .Select(methodInfo => methodInfo.Name)
                                                         .Distinct();

            return new GrainInformation
            {
                Methods = grainInterfaces.ToList()
            };
        }

        private static IConfigurationRoot LoadConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();
            return configuration;
        }

        private static OrleansConfiguration GetOrleansConfiguration(IConfigurationRoot configuration)
        {
            var applicationConfiguration = new OrleansConfiguration();
            var section = configuration.GetSection("OrleansConfiguration");
            section.Bind(applicationConfiguration);
            return applicationConfiguration;            
        }
    }
}
