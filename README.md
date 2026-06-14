<h1 align="center">
  <img width="300" src="/img/logoname-white.svg#gh-dark-mode-only" alt="infisical">
</h1>
<p align="center">
  <p align="center"><b>Infisical .NET SDK</b></p>
<h4 align="center">
|
  <a href="https://infisical.com/docs/sdks/languages/dotnet">Documentation</a> |
  <a href="https://www.infisical.com">Website</a> |
  <a href="https://infisical.com/slack">Slack</a> |
</h4>

<h4 align="center">
  <a href="https://github.com/Infisical/infisical-dotnet-sdk/blob/main/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="Infisical SDK's are released under the MIT license." />
  </a>
  <a href="https://infisical.com/slack">
    <img src="https://img.shields.io/badge/chat-on%20Slack-blueviolet" alt="Slack community channel" />
  </a>
  <a href="https://twitter.com/infisical">
    <img src="https://img.shields.io/twitter/follow/infisical?label=Follow" alt="Infisical Twitter" />
  </a>
</h4>

## Introduction

**[Infisical](https://infisical.com)** is the open source secret management platform that teams use to centralize their secrets like API keys, database credentials, and configurations.

If you're working with .NET, the official Infisical .NET SDK package is the easiest way to fetch and work with secrets for your application. You can read the documentation [here](https://infisical.com/docs/sdks/languages/dotnet).

## Installation

```bash
dotnet add package Infisical.Sdk
```

## Getting Started

```csharp
using Infisical.Sdk;
using Infisical.Sdk.Model;

var settings = new InfisicalSdkSettingsBuilder()
    // Optional — defaults to https://app.infisical.com
    .WithHostUri("https://app.infisical.com")
    .Build();

var infisicalClient = new InfisicalClient(settings);

// Authenticate with Universal Auth
var _ = await infisicalClient.Auth().UniversalAuth().LoginAsync(
    "<machine-identity-client-id>",
    "<machine-identity-client-secret>"
);

// Fetch secrets
var options = new ListSecretsOptions
{
    EnvironmentSlug = "dev",
    SecretPath = "/",
    ProjectId = "<your-project-id>",
};

var secrets = await infisicalClient.Secrets().ListAsync(options);

foreach (var secret in secrets)
{
    Console.WriteLine($"{secret.SecretKey}: {secret.SecretValue}");
}
```

### EU Region

If you signed up for Infisical's **EU (European Union)** data region, configure the host URI to point to the EU instance:

```csharp
var settings = new InfisicalSdkSettingsBuilder()
    .WithHostUri("https://eu.infisical.com")
    .Build();
```

> **Note:** The default host URI is `https://app.infisical.com` (US region). If you selected "Europe" during sign-up, you **must** set the host URI to `https://eu.infisical.com` — otherwise, authentication will fail.

### Self-Hosted

For self-hosted Infisical instances, point to your own deployment:

```csharp
var settings = new InfisicalSdkSettingsBuilder()
    .WithHostUri("https://your-self-hosted-infisical.example.com")
    .Build();
```

## Documentation
You can find the full documentation for the .NET SDK on our [SDK documentation page](https://infisical.com/docs/sdks/languages/dotnet).

## Security

Please do not file GitHub issues or post on our public forum for security vulnerabilities, as they are public!

Infisical takes security issues very seriously. If you have any concerns about Infisical or believe you have uncovered a vulnerability, please get in touch via the e-mail address security@infisical.com. In the message, try to provide a description of the issue and ideally a way of reproducing it. The security team will get back to you as soon as possible.

Note that this security address should be used only for undisclosed vulnerabilities. Please report any security problems to us before disclosing it publicly.