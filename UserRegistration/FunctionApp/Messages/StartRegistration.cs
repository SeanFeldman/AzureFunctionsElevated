using NServiceBus;

namespace FunctionApp.Messages
{
    public class StartRegistration : ICommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}