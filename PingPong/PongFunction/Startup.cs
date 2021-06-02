using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NServiceBus;
using PongFunction;

[assembly: FunctionsStartup(typeof(Startup))]

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.UseNServiceBus(() =>
        {
            var configuration = new ServiceBusTriggeredEndpointConfiguration(TriggerFunction.EndpointName);

            // configuration.LogDiagnostics();

            return configuration;
        });
    }
}