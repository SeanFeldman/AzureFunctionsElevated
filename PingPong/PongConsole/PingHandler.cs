using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

public class PingHandler : IHandleMessages<Ping>
{
    static readonly ILog logger = LogManager.GetLogger<PingHandler>();
        
    public async Task Handle(Ping message, IMessageHandlerContext context)
    {
        logger.Info($"Received a ping originated on {message.IssuedOn}");

        var pongMessage = new Pong { For = $"Pong for ping issues on {message.IssuedOn}" };
            
        await context.Reply(pongMessage);
    }
}
