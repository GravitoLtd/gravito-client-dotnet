using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gravito.IdentityClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        // You can use Azure Key-Vault to store the key information like client_id, client_secret

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [Route("token")]
        public async Task<IActionResult> GetToken()
        {
            // Create client to connect to IdentityServer
            var serverClient = _httpClientFactory.CreateClient();

            // Get the available configuration of the server
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync(_configuration.GetValue<string>("Identity:ServerAddress"));

            // Call TokenEndpoint and get the token
            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,

                    GrantType = _configuration.GetValue<string>("Identity:GrantType"),
                    ClientId = _configuration.GetValue<string>("Identity:ClientId"),
                    Scope = _configuration.GetValue<string>("Identity:Scope")
                }) ;

            #region -- Create another client to connect to protected endpoint

            //var apiClient = _httpClientFactory.CreateClient();
            //apiClient.SetBearerToken(tokenResponse.AccessToken);

            //var sampleModel = new
            //{
            //    Property1 = "data",
            //    Property2 = "data"
            //};

            //var json = JsonConvert.SerializeObject(sampleModel);
            //var data = new StringContent(json, Encoding.UTF8, "application/json");

            //// Access the API secured / authorized by IdentityServer
            //var response = await apiClient.PostAsync("https://protected-endpoint", data);
            //var content = await response.Content.ReadAsStringAsync();

            #endregion

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                //message = content,
            });
        }
    }
}
