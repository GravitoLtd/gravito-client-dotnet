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
                    ClientSecret = _configuration.GetValue<string>("Identity:ClientSecret"),
                    Scope = _configuration.GetValue<string>("Identity:Scope")
                }) ;

            // Create another client to connect to Gravito
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var magicLink = new
            {
                Email = "testemail@gravito.net",
                Token = ""
            };

            var json = JsonConvert.SerializeObject(magicLink);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Access the API secured / authorized by IdentityServer
            var response = await apiClient.PostAsync("https://dev-api.gravito.net/api/account/magiclink", data);
            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                message = content,
            });
        }
    }
}
