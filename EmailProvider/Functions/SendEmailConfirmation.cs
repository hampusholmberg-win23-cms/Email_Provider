using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmailProvider.Functions
{
    public class SendEmailConfirmation
    {
        private readonly ILogger<SendEmailConfirmation> _logger;

        public SendEmailConfirmation(ILogger<SendEmailConfirmation> logger)
        {
            _logger = logger;
        }

        [Function("SendEmailConfirmation")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {




            return null!;
        }
    }
}
