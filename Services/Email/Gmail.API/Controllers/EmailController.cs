using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Oauth2.v2.Data;
using System.Threading;
using Google.Apis.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Gmail.API.Requests;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Gmail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        public EmailController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        static string ApplicationName = "Gmail API .NET Quickstart";

        public IConfiguration Configuration { get; }

        [HttpPost]
        public async Task<ActionResult<bool>> SendEmail([FromBody] EmailRequest emailRequest)
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

            string message = $@"To: {emailRequest.Receiver}
Subject: {emailRequest.Subject}

{emailRequest.Body}";
            string base64Encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
            var request = service.Users.Messages.Send(new Message { Raw = base64Encoded }, "me");
            var response = await request.ExecuteAsync();
            return NoContent();
        }
    }
}
