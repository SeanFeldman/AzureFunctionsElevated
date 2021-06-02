using System;
using System.Threading.Tasks;
using FunctionApp.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace FunctionApp
{
    public class RegistrationWorkflow : Saga<RegistrationState>, 
        IAmStartedByMessages<StartRegistration>,
        IHandleMessages<ProcessConfirmation>,
        IHandleMessages<RegistrationCompleted>,
        IHandleTimeouts<ReminderAfter24Hours>,
        IHandleTimeouts<TerminateAfter48Hours>
    {
        static readonly ILog logger = LogManager.GetLogger<RegistrationWorkflow>();
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<RegistrationState> mapper)
        {
            mapper.MapSaga(saga => saga.Email)
                .ToMessage<StartRegistration>(m => m.Email)
                .ToMessage<ProcessConfirmation>(m => m.Email)
                .ToMessage<RegistrationCompleted>(m => m.Email);
        }

        public async Task Handle(StartRegistration message, IMessageHandlerContext context)
        {
            var (confirmationCode, url) = GenerateConfirmationCodeAndUrl(message.Email);
            
            this.Data.Email = message.Email;
            this.Data.Password = message.Password;
            this.Data.ConfirmationCode = confirmationCode;

            logger.Warn($"Confirmation email sent out with code: {confirmationCode}");

            await RequestTimeout<ReminderAfter24Hours>(context, TimeSpan.FromSeconds(30));
        }

        static (string confirmationCode, string url) GenerateConfirmationCodeAndUrl(string email)
        {
            var host = $"{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}";
            var schema = host.StartsWith("localhost:") ? "http" : "https";
            var confirmationCode = ConfirmationCodeGenerator.Generate();
            var url =  $"{schema}://{host}/api/confirm/{email}/{confirmationCode}";
            
            return (confirmationCode, url);
        }

        public async Task Handle(ProcessConfirmation message, IMessageHandlerContext context)
        {
            if (message.ConfirmationCode != this.Data.ConfirmationCode)
            {
                return;
            }
            
            // Store registration information in a DB
            
            await context.Publish(new RegistrationCompleted { Email = this.Data.Email });
        }
        
        public Task Handle(RegistrationCompleted message, IMessageHandlerContext context)
        {
            logger.Warn($"Registration for {this.Data.Email} has successfully completed!");
            
            MarkAsComplete();

            return Task.CompletedTask;
        }

        public async Task Timeout(ReminderAfter24Hours state, IMessageHandlerContext context)
        {
            logger.Warn($"Send a reminder to activate the account for {this.Data.Email}.");
            
            await RequestTimeout<TerminateAfter48Hours>(context, TimeSpan.FromSeconds(30));
        }

        public Task Timeout(TerminateAfter48Hours state, IMessageHandlerContext context)
        {
            logger.Warn($"Aborting registration for {this.Data.Email}.");
            
            MarkAsComplete();

            return Task.CompletedTask;
        }
    }

    public class RegistrationState : ContainSagaData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class ReminderAfter24Hours
    {
    }

    public class TerminateAfter48Hours
    {
    }
}