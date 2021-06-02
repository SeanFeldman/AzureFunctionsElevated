using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using Shared;

class Program
{
    static async Task Main()
    {
        #region Get ASB connection string from configuration file

        var builder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.local.json", optional:false);
        var config = builder.Build();
        var asbConnectionString = config.GetConnectionString("AzureServiceBus");

        #endregion
        
        var endpointConfiguration = new EndpointConfiguration("pong");

        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(asbConnectionString);
        
        transport.Routing().RouteToEndpoint(typeof(Ping), "pong");
        
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        Console.WriteLine("Press 'esc' key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }

        await endpointInstance.Stop();
    }
}