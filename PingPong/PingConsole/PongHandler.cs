using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

public class PongHandler : IHandleMessages<Pong>
{
    static readonly ILog logger = LogManager.GetLogger<PongHandler>();
        
    public Task Handle(Pong message, IMessageHandlerContext context)
    {
        logger.Info($"Received a pong originated on {context.ReplyToAddress}{Environment.NewLine}{message.For}");
        
        return Task.CompletedTask;
    }
}
