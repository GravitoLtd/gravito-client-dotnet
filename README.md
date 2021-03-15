# Gravito Client Credentials Flow .Net Client
Gravito's IdentityServer4 client for C#.Net

A sample project which demonstrates how to connect to Gravito's Identity and get the `access_token`.

Open and view the project using `.zip` file or download from our [GitHub Repository]

## Table of Contents
- [Getting started](#getting-started)
- [Tools required](#tools-required)
- [Usage guide](#usage-guide)
- [What after getting token?](#what-after-getting-token)
- [Visit us at](#visit-us-at)

## Getting Started

You can find the detailed documentation about the **Gravito Identity Management** at [Gravito Docs].
Here, we have explained how Gravito works as an Identity Provider.

## Tools required

* Visual Studio 2019
* Microsoft .Net Core SDK 3.1.* (Haven't tested this sample with latest versions, it might need some changes)

## Usage guide

Code required to get the `access_token` from server:

* Declare private variable of `IHttpClientFactory`, inject it in a constructure
```c#
private readonly IHttpClientFactory _httpClientFactory;
```

* Create client for getting the `access_token`
```c#
var serverClient = _httpClientFactory.CreateClient();
```

* Get the available configuration of the server
```c#
var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync(_configuration.GetValue<string>("Identity:ServerAddress"));
```

You can access `ServerAddress` from `appsettings.json` or from Azure Key-Vault.


* Call TokenEndpoint and get the token
```c#
var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync (
new ClientCredentialsTokenRequest
{
    Address = discoveryDocument.TokenEndpoint,

    GrantType = _configuration.GetValue<string>("Identity:GrantType"),
    ClientId = _configuration.GetValue<string>("Identity:ClientId"),
    ClientSecret = _configuration.GetValue<string>("Identity:ClientSecret"),
    Scope = _configuration.GetValue<string>("Identity:Scope")
});
```

## After getting the token
### Here is what you can do next:
* Create another client to use protected APIs
* Set the received bearer token to created client

```c#
var apiClient = _httpClientFactory.CreateClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);
```

* Prepare your data

```c#
var sampleModel = new
{
    Property1 = "data",
    Property2 = "data"
};

var json = JsonConvert.SerializeObject(sampleModel);
var data = new StringContent(json, Encoding.UTF8, "application/json");
```
* Access the protected APIs

```c#
var response = await apiClient.PostAsync("https://protected-endpoint", data);
var content = await response.Content.ReadAsStringAsync();
```

### appsettings.json file
```json
"Identity": {
    "ClientId": "client_id",
    "ClientSecret": "client_secret",
    "Scope": "api",
    "GrantType": "client_credentials",
    "ServerAddress": "https://your-identityserver-address/"
}
```

## Visit us at
[Website]

[GitHub Repository]: https://github.com/GravitoLtd/gravito-client-dotnet
[Website]: https://www.gravito.net
[Gravito Docs]: https://docs.gravito.net/gravito-identity-provider/getting-started
