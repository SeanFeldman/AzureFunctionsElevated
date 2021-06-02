using System.Threading.Tasks;
using FunctionApp.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace FunctionApp
{
    public class HttpRegister
    {
        private readonly IFunctionEndpoint functionEndpoint;

        public HttpRegister(IFunctionEndpoint functionEndpoint)
        {
            this.functionEndpoint = functionEndpoint;
        }
        
        [FunctionName("register")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] RegistrationModel registrationModel, 
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"Registration intent for {registrationModel.Email} has been received.");

            var sendOptions = new SendOptions();
            sendOptions.RouteToThisEndpoint();

            var message = new StartRegistration
            {
                Email = registrationModel.Email,
                Password = registrationModel.Password
            };
            
            await functionEndpoint.Send(message, sendOptions, executionContext);

            return new OkObjectResult($"Registration for {registrationModel.Email} is kicked off.");
        }

        public class RegistrationModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}