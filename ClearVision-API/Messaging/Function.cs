using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Communication.Identity;

namespace ClearVision_API.Messaging
{
    public static class Function
    {
        [FunctionName("GenerateToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Initialize communication client
            string connectionString = Environment.GetEnvironmentVariable("ACS_CONNECTION_STRING");
            var client = new CommunicationIdentityClient(connectionString);

            // Create a user and token
            var tokenResponse = await client.CreateUserAndTokenAsync(scopes: new[] { CommunicationTokenScope.Chat });
            var user = tokenResponse.Value.User;
            var token = tokenResponse.Value.AccessToken;

            var result = new
            {
                userId = user.Id,
                token = token.Token,
                expiresOn = token.ExpiresOn
            };

            return new OkObjectResult(result);
        }
    }
}
