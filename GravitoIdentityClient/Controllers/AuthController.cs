using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace Gravito.IdentityClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _accessToken;

        // You can use Azure Key-Vault to store the key information like client_id, client_secret

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;

            // client should store this token
            string accessToken = "";

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = tokenHandler.ReadJwtToken(accessToken);

                if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
                {
                    // The previous token is expired

                    _accessToken = GetAccessToken();
                }
                else
                    _accessToken = accessToken;
            }
            else
            {
                // get the access token for the first time
                _accessToken = GetAccessToken();
            }
        }

        [Route("token")]
        public IActionResult GetToken()
        {
            #region -- Create another client to connect to protected endpoint

            //var apiClient = _httpClientFactory.CreateClient();
            //apiClient.SetBearerToken(_accessToken);

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
                access_token = _accessToken,
                //message = content,
            });
        }

        /// <summary>
        /// Get the new access token
        /// </summary>
        ///

        private string GetAccessToken()
        {
            // retrieve access token
            var serverClient = new HttpClient();

            
            var discoveryDocument = serverClient.GetDiscoveryDocumentAsync(_configuration.GetValue<string>("Identity:ServerAddress"))
                .GetAwaiter().GetResult();

            var tokenResponse = serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,

                    GrantType = _configuration.GetValue<string>("Identity:GrantType"),
                    ClientId = _configuration.GetValue<string>("Identity:ClientId"),
                    ClientSecret = _configuration.GetValue<string>("Identity:ClientSecret"),
                    Scope = _configuration.GetValue<string>("Identity:Scope")
                }).GetAwaiter().GetResult();

            return tokenResponse.AccessToken;
        }
    }
}
