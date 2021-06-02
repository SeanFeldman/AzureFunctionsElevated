using FunctionApp;
using FunctionApp.Infrastructure;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NServiceBus;

[assembly: FunctionsStartup(typeof(Startup))]

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Functions don't create infrastructure
        // https://weblogs.asp.net/sfeldman/automatic-nservicebus-topology-creation-for-function
        TopologyCreator.Create(builder.GetContext().Configuration).GetAwaiter().GetResult();
        
        builder.UseNServiceBus(() =>
        {
            var configuration = new ServiceBusTriggeredEndpointConfiguration(TriggerFunction.EndpointName);

            var persistence = configuration.AdvancedConfiguration.UsePersistence<AzureTablePersistence>();

            var account = CloudStorageAccount.Parse(builder.GetContext().Configuration["AzureWebJobsStorage"]);
            persistence.UseCloudTableClient(account.CreateCloudTableClient());

            return configuration;
        });
    }
}