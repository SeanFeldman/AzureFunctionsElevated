using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace FunctionApp.Infrastructure
{
    public class ForceAutoSubscriptionFunction
    {
        readonly IFunctionEndpoint functionEndpoint;

        public ForceAutoSubscriptionFunction(IFunctionEndpoint functionEndpoint)
        {
            this.functionEndpoint = functionEndpoint;
        }

        [FunctionName("TimerFunc")]
        public async Task Run([TimerTrigger("* * * 1 1 *", RunOnStartup = true)]TimerInfo myTimer, // fire yearly and upon startup
            ILogger logger, ExecutionContext executionContext)
        {
            var sendOptions = new SendOptions();
            sendOptions.SetHeader(Headers.ControlMessageHeader, bool.TrueString);
            sendOptions.SetHeader(Headers.MessageIntent, MessageIntentEnum.Send.ToString());
            sendOptions.RouteToThisEndpoint();
            
            await functionEndpoint.Send(new ForceAutoSubscription(), sendOptions, executionContext, logger);
        }
    }}