using NServiceBus;

namespace FunctionApp.Messages
{
    public class RegistrationCompleted : IEvent
    {
        public string Email { get; set; }
    }
}