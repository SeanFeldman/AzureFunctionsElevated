namespace FunctionApp.Messages
{
    public class ProcessConfirmation
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }
}