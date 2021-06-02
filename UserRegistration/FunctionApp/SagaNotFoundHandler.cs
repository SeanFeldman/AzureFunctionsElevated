using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Sagas;

namespace FunctionApp
{
    public class SagaNotFoundHandler : IHandleSagaNotFound
    {
        static readonly ILog logger = LogManager.GetLogger<SagaNotFoundHandler>();
        
        public Task Handle(object message, IMessageProcessingContext context)
        {
            logger.Warn($"Message {message.GetType()} arrived but no active saga instance was found.");
            
            return Task.CompletedTask;
        }
    }
}