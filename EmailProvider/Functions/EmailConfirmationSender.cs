using System;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Azure.Communication.Email;
using EmailProvider.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EmailProvider.Functions
{
    public class EmailConfirmationSender
    {
        private readonly ILogger<EmailConfirmationSender> _logger;
        private readonly EmailClient _emailClient;

        public EmailConfirmationSender(ILogger<EmailConfirmationSender> logger, EmailClient emailClient)
        {
            _logger = logger;
            _emailClient = emailClient;
        }

        [Function(nameof(EmailConfirmationSender))]
        public async Task Run(
            [ServiceBusTrigger("email_request", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            try
            {
                var emailRequest = UnPackEmailRequest(message);

                if (emailRequest != null && !string.IsNullOrEmpty(emailRequest.To))
                {
                    if (SendEmail(emailRequest))
                    {
                        await messageActions.CompleteMessageAsync(message);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"ERROR : EmailConfirmationSender.Run :: {ex.Message}");
            }
        }

        public EmailRequest UnPackEmailRequest(ServiceBusReceivedMessage message)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<EmailRequest>(message.Body.ToString());
                if (request != null)
                    return request;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : EmailConfirmationSender.UnPackEmailRequest :: {ex.Message}");
            }
            return null!;
        }

        public bool SendEmail(EmailRequest emailRequest)
        {
            try
            {
                var result = _emailClient.Send(
                    WaitUntil.Completed,
                    senderAddress: Environment.GetEnvironmentVariable("SENDER_ADDRESS"),
                    recipientAddress: emailRequest.To,
                    subject: emailRequest.Subject,
                    htmlContent: emailRequest.HtmlBody,
                    plainTextContent: emailRequest.PlainText);

                if (result.HasCompleted) 
                    return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : EmailConfirmationSender.SendEmailAsync :: {ex.Message}");
            }
            return false;
        }
    }
}