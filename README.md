# Infisical .NET SDK

The Infisical .NET SDK provides a convenient way to interact with the Infisical API. 

## Installation

```bash
dotnet add package Infisical.Sdk
```

## Getting Started

```csharp
namespace Example;

using Infisical.Sdk;
using Infisical.Sdk.Model;

public class Program {
  public static void Main(string[] args) {

    var settings = new InfisicalSdkSettingsBuilder()
        .WithHostUri("http://localhost:8080") // Optional. Will default to https://app.infisical.com
        .Build();

    var infisicalClient = new InfisicalClient(settings);

    var _ = client.Auth().UniversalAuth().LoginAsync("<machine-identity-universal-auth-client-id>", "<machine-identity-universal-auth-client-secret>").Result;

    var options = new ListSecretsOptions
    {
      SetSecretsAsEnvironmentVariables = true,
      EnvironmentSlug = "<your-env-slug>",
      SecretPath = "/",
      ProjectId = "<your-project-id>",
    };

    var secrets = client.Secrets().ListAsync(options).Result;

    if (secrets == null)
    {
      throw new Exception("Failed to fetch secrets, returned null response");
    }

    foreach (var secret in secrets)
    {
      Console.WriteLine($"{secret.SecretKey}: {secret.SecretValue}");
    }
  }
}

```

## Core Methods

The SDK methods are organized into the following high-level categories:

1. `Auth()`: Handles authentication methods.
2. `Secrets()`: Manages CRUD operations for secrets.

### `Auth`

The `Auth` component provides methods for authentication:

### Universal Auth

#### Authenticating
```cs
var _ = await sdk.Auth().UniversalAuth().LoginAsync(
  "CLIENT_ID",
  "CLIENT_SECRET"
);
```


**Parameters:**
- `clientId` (string): The client ID of your Machine Identity.
- `clientSecret` (string): The client secret of your Machine Identity.

### `Secrets`

This sub-class handles operations related to secrets:

#### List Secrets

```cs
Task<Secret[]> ListAsync(ListSecretsOptions options);

throws InfisicalException
```

```csharp
var options = new ListSecretsOptions
  {
    SetSecretsAsEnvironmentVariables = true,
    EnvironmentSlug = "dev",
    SecretPath = "/test",
    Recursive = true,
    ExpandSecretReferences = true,
    ProjectId = projectId,
    ViewSecretValue = true,
  };

Secret[] secrets = await sdk.Secrets().ListAsync(options);
```

**ListSecretsOptions:**
- `ProjectId` (string): The ID of your project.
- `EnvironmentSlug` (string): The environment in which to list secrets (e.g., "dev").
- `SecretPath` (string): The path to the secrets.
- `ExpandSecretReferences` (boolean): Whether to expand secret references.
- `Recursive` (boolean): Whether to list secrets recursively.
- `SetSecretsAsEnvironmentVariables` (boolean): Set the retrieved secrets as environment variables.

**Returns:**
- `Task<Secret[]>`: The response containing the list of secrets.

#### Create Secret

```cs
public Task<Secret> CreateAsync(CreateSecretOptions options);

throws InfisicalException
```

```cs

var options = new CreateSecretOptions
{
  SecretName = "SECRET_NAME",
  SecretValue = "SECRET_VALUE",
  EnvironmentSlug = "<environment-slug>",
  SecretPath = "/",
  ProjectId = "<your-project-id>",
  Metadata = new SecretMetadata[] {
    new SecretMetadata {
      Key = "metadata-key",
      Value = "metadata-value"
    }
  }
};

Task<Secret> newSecret = await sdk.Secrets().CreateAsync(options);
```

**Parameters:**
- `SecretName` (string): The name of the secret to create
- `SecretValue` (string): The value of the secret.
- `ProjectId` (string): The ID of your project.
- `EnvironmentSlug` (string): The environment in which to create the secret.
- `SecretPath` (string, optional): The path to the secret.
- `Metadata` (object, optional): Attach metadata to the secret.
- `SecretComment` (string, optional): Attach a secret comment to the secret.
- `SecretReminderNote` (string, optional): Attach a secret reminder note to the secret.
- `SecretReminderRepeatDays` (int, optional): Set the reminder repeat days on the secret.
- `SkipMultilineEncoding` (bool, optional): Weather or not to skip multiline encoding for the secret's value. Defaults to `false`.

**Returns:**
- `Task<Secret>`: The created secret.

#### Update Secret

```cs
public Task<Secret> UpdateAsync(UpdateSecretOptions options); 

throws InfisicalException
```


```cs

var updateSecretOptions = new UpdateSecretOptions
{
  SecretName = "EXISTING_SECRET_NAME",
  EnvironmentSlug = "<environment-slug>",
  SecretPath = "/",
  NewSecretName = "NEW_SECRET_NAME",
  NewSecretValue = "new-secret-value",
  ProjectId = "<project-id>",
};


Task<Secret> updatedSecret = await sdk.Secrets().UpdateAsync(options);
```

**Parameters:**
- `SecretName` (string): The name of the secret to update.`
- `ProjectId` (string): The ID of your project.
- `EnvironmentSlug` (string): The environment in which to update the secret.
- `SecretPath` (string): The path to the secret.
- `NewSecretValue` (string, optional): The new value of the secret.
- `NewSecretName` (string, optional): A new name for the secret.
- `NewMetadata` (object, optional): New metadata to attach to the secret.

**Returns:**
- `Task<Secret>`: The updated secret.

#### Get Secret by Name

```cs
public Secret GetAsync(GetSecretOptions options);

throws InfisicalException
```

```cs

var getSecretOptions = new GetSecretOptions
{
  SecretName = "SECRET_NAME",
  EnvironmentSlug = "<environment-slug>",
  SecretPath = "/",
  ProjectId = "<project-id>",
};

Secret secret = await sdk.Secrets().GetAsync(options);
```

**Parameters:**
- `SecretName` (string): The name of the secret to get`
- `ProjectId` (string): The ID of your project.
- `EnvironmentSlug` (string): The environment in which to retrieve the secret.
- `SecretPath` (string): The path to the secret.
- `ExpandSecretReferences` (boolean, optional): Whether to expand secret references.
- `Type` (SecretType, optional): The type of secret to fetch. Defaults to `Shared`.


**Returns:**
- `Task<Secret>`: The fetched secret.

#### Delete Secret by Name

```cs
public Secret DeleteAsync(DeleteSecretOptions options);

throws InfisicalException
```

```cs

var options = new DeleteSecretOptions
{
  SecretName = "SECRET_TO_DELETE",
  EnvironmentSlug = "<environment-slug>",
  SecretPath = "/",
  ProjectId = "<project-id>",
};


Secret deletedSecret = await sdk.Secrets().DeleteAsync(options);
```

**Parameters:**
- `SecretName` (string): The name of the secret to delete.
- `ProjectId` (string): The ID of your project.
- `EnvironmentSlug` (string): The environment in which to delete the secret.
- `SecretPath` (string, optional): The path to the secret.

**Returns:**
- `Task<Secret>`: The deleted secret.

