using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Gmail.API.Requests;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Gmail.v1.Data;

namespace Gmail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "967300687329-oevfo0q64fq6agjvu6jhhl4ntg11ovjv.apps.googleusercontent.com",
                    ClientSecret = "2C3toAigLsQaKVllbX82U3ww"
                },
                Scopes = new[] { GmailService.Scope.GmailSend }
            });

        static string[] Scopes = {
                    GmailService.Scope.GmailSend};
        static string ApplicationName = "Gmail API .NET Quickstart";

        [HttpPost]
        public async void SendEmail([FromBody] EmailRequest emailRequest)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "XXXX",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            string message = $@"To: Morten Korsholm <morphy_@msn.com>
Subject: Hjesa

Hey, kan du så modtage den her mail du...";
            string base64Encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
            var request = service.Users.Messages.Send(new Message { Raw = base64Encoded }, "me");
            var response = await request.ExecuteAsync();
        }
    }
}
