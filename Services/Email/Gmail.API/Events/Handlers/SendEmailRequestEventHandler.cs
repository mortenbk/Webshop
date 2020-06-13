using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebShop.RabbitMQ;

namespace Gmail.API.Events.Handlers
{
    public class SendEmailRequestEventHandler : IMessageQueueEventHandler<SendEmailRequestMessageEvent>
    {
        static string ApplicationName = "Gmail API .NET Quickstart";


        public IEventBus EventBus { get; }
        public IConfiguration Configuration { get; }

        public SendEmailRequestEventHandler(IEventBus eventBus, IConfiguration configuration)
        {
            EventBus = eventBus;
            Configuration = configuration;
        }


        public Task HandleMessage(SendEmailRequestMessageEvent sendEmailRequest)
        {
            return Task.Run(async () =>
            {
                IConfigurationSection googleAuthNSection =
                                Configuration.GetSection("Authentication:Google");
                var clientId = googleAuthNSection["ClientId"];
                var clientSecret = googleAuthNSection["ClientSecret"];

                var userCred = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }, scopes: new List<string>() { GmailService.Scope.GmailSend }, "me", CancellationToken.None);



                // Create Gmail API service.
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = userCred,
                    ApplicationName = ApplicationName
                });

                string message = $@"To: {sendEmailRequest.Email}
Subject: {sendEmailRequest.Subject}

{sendEmailRequest.Body}";
                string base64Encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
                var request = service.Users.Messages.Send(new Message { Raw = base64Encoded }, "me");
                var response = await request.ExecuteAsync();
            });

        }
    }
}
