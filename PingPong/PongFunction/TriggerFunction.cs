using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace PongFunction
{
    public class TriggerFunction
    {
        internal const string EndpointName = "pong";
        private readonly IFunctionEndpoint endpoint;

        public TriggerFunction(IFunctionEndpoint endpoint)
        {
            this.endpoint = endpoint;
        }

        [FunctionName(EndpointName)]
        public async Task Run(
            [ServiceBusTrigger(queueName: EndpointName)]
            Message message,
            ILogger logger,
            ExecutionContext executionContext)
        {
            await endpoint.Process(message, executionContext, logger);
        }
    }
}