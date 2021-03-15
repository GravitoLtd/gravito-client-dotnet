# Gravito Client Credentials Flow .Net Client

This sample demonstrates how to connect to Gravito IdentityServer using C#.Net to get the `access_token`.

You can download the zip or clone the repository from [GitHub Repository]

## Table of Contents
- [Getting Started](#getting-started)
- [Tools Required](#tools-required)
- [Usage Guide](#usage-guide)
- [After Getting access_token?](#after-getting-access_token)
- [References](#references)
- [Visit Us At](#visit-us-at)

## Getting Started

We have explained how Gravito works as an **Identity Provider** in our detailed documentation at [Gravito Docs].

## Tools Required

* Visual Studio 2019
* Microsoft .Net Core SDK 3.1.* (Haven't tested this sample with upper versions, it might need some changes)

## Usage Guide

Code required to get the `access_token` from server:

* Declare private variable of `IHttpClientFactory` & inject it in constructor
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

`ServerAddress` parameter value can be stored in `appsettings.json` or Azure Key-Vault.

* Call `TokenEndpoint` and get the token
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

## After Getting access_token
* Create another client of type `HttpClientFactory` to use protected APIs & set the received bearer token to it

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

## References
* appsettings.json file
```json
"Identity": {
    "ClientId": "client_id",
    "ClientSecret": "client_secret",
    "Scope": "api",
    "GrantType": "client_credentials",
    "ServerAddress": "https://your-identityserver-address/"
}
```

## Visit Us At
[Website]

[GitHub Repository]: https://github.com/GravitoLtd/gravito-client-dotnet
[Website]: https://www.gravito.net
[Gravito Docs]: https://docs.gravito.net/gravito-identity-provider/getting-started
