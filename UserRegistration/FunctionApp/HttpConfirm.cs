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
    public class HttpConfirm
    {
        private readonly IFunctionEndpoint functionEndpoint;

        public HttpConfirm(IFunctionEndpoint functionEndpoint)
        {
            this.functionEndpoint = functionEndpoint;
        }
        
        [FunctionName("confirm")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] ConfirmationModel confirmationModel,
            HttpRequest req, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"Confirmation for {confirmationModel.Email} has been received.");

            var sendOptions = new SendOptions();
            sendOptions.RouteToThisEndpoint();

            var message = new ProcessConfirmation
            {
                Email = confirmationModel.Email,
                ConfirmationCode = confirmationModel.ConfirmationCode
            };
            
            await functionEndpoint.Send(message, sendOptions, executionContext);

            return new OkObjectResult($"Confirmation for {confirmationModel.Email} has been received.");
        }
    }
    
    public class ConfirmationModel
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }
}