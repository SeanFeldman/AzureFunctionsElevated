using NServiceBus;

namespace Shared
{
    public class Pong : IMessage
    {
        public string For { get; set; }
    }
}