# gravito-is4-client-dotnet
Gravito's IdentityServer4 client for C#.Net

A sample project which demonstrate how to connect to Gravito and get the access_token.

Open and view the Project using the `.zip` file provided or at my [GitHub Repository]

## Table of Contents
- [Getting Started](#getting-started)
	- [Tools Required](#tools-required)
- [Running the App](#running-the-app)

## Getting Started

The project might have multiple branches: `master`, `development`, etc. which are:

* `master` contains aggregate code of all branches
* `development` contains code under development

### Tools Required

* Visual Studio Code
* Visual Studio 2019
* Microsoft .Net Core SDK 3.1.*

## Running the App

Steps for running the app:

* Declare private variable of `IHttpClientFactory`, inject it in constructure
```c#
private readonly IHttpClientFactory _httpClientFactory;
```

* Create client for getting the access_token
```c#
var serverClient = _httpClientFactory.CreateClient();
```

* Get the available configuration of the server
```c#
var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync(_configuration.GetValue<string>("Identity:ServerAddress"));
```

We can access the ServerAddress from `appsettings.json` or from Azure Key-Vault.


* Call TokenEndpoint and get the token
```c#
var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync (
new ClientCredentialsTokenRequest
{
    Address = discoveryDocument.TokenEndpoint,

    GrantType = _configuration.GetValue<string>("Identity:GrantType"),
    ClientId = _configuration.GetValue<string>("Identity:ClientId"),
    Scope = _configuration.GetValue<string>("Identity:Scope")
});
```

[GitHub Repository]: https://github.com/GravitoLtd/gravito-client-dotnet

[Website]:(https://www.gravito.net)
