
using System.Data.Common;
using System.Net.Mail;
using System.Text.Json.Serialization;
using Infisical.Sdk.Client;
using Infisical.Sdk.Util;

public enum SecretType
{
  [JsonPropertyName("shared")]
  Shared,
  [JsonPropertyName("personal")]
  Personal,
}

public class MachineIdentityCredential
{
  public MachineIdentityCredential(string accessToken, decimal expiresIn, decimal accessTokenMaxTTL, string tokenType)
  {
    AccessToken = accessToken;
    ExpiresIn = expiresIn;
    AccessTokenMaxTTL = accessTokenMaxTTL;
    TokenType = tokenType;
  }

  [JsonPropertyName("accessToken")]
  public string AccessToken { get; }

  [JsonPropertyName("expiresIn")]
  public decimal ExpiresIn { get; }

  [JsonPropertyName("accessTokenMaxTTL")]
  public decimal AccessTokenMaxTTL { get; }

  [JsonPropertyName("tokenType")]
  public string TokenType { get; }
}

public class UniversalAuthLoginRequest
{
  public UniversalAuthLoginRequest(string clientId, string clientSecret)
  {
    ClientId = clientId;
    ClientSecret = clientSecret;
  }

  [JsonPropertyName("clientId")]
  public string ClientId { get; }

  [JsonPropertyName("clientSecret")]
  public string ClientSecret { get; }
}

public class ListSecretsOptions
{

  public bool SetSecretsAsEnvironmentVariables { get; set; } = false;

  [JsonPropertyName("workspaceId")]
  public string? ProjectId { get; init; } = null;
  [JsonPropertyName("environment")]
  public string? EnvironmentSlug { get; init; } = null;
  [JsonPropertyName("secretPath")]
  public string SecretPath { get; init; } = "/";
  [JsonPropertyName("viewSecretValue")]
  public bool? ViewSecretValue { get; init; } = null;
  [JsonPropertyName("expandSecretReferences")]
  public bool? ExpandSecretReferences { get; init; } = true;
  [JsonPropertyName("recursive")]
  public bool? Recursive { get; init; } = false;
  [JsonPropertyName("tagSlugs")]
  public string[]? TagSlugs { get; init; } = null;
  [JsonPropertyName("include_imports")]
  public bool IncludeImports { get; } = true;

  internal void Validate()
  {

    if (string.IsNullOrEmpty(ProjectId))
    {
      throw new Exception("ProjectId is required");
    }

    if (string.IsNullOrEmpty(EnvironmentSlug))
    {
      throw new Exception("EnvironmentSlug is required");
    }

    if (string.IsNullOrEmpty(SecretPath))
    {
      throw new Exception("SecretPath is required");
    }
  }
}

public class GetSecretOptions
{
  [JsonPropertyName("workspaceId")]
  public string? ProjectId { get; init; } = null;

  [JsonPropertyName("environment")]
  public string? EnvironmentSlug { get; init; } = null;

  [JsonPropertyName("secretPath")]
  public string SecretPath { get; init; } = "/";

  [JsonPropertyName("secretName")]
  public string SecretName { get; init; } = string.Empty;

  [JsonPropertyName("version")]
  public int? Version { get; init; } = null;

  [JsonPropertyName("type")]
  public SecretType? Type { get; init; } = null;

  [JsonPropertyName("viewSecretValue")]
  public bool? ViewSecretValue { get; init; } = null;

  [JsonPropertyName("expandSecretReferences")]
  public bool? ExpandSecretReferences { get; init; } = true;

  [JsonPropertyName("include_imports")]
  public bool IncludeImports { get; } = true;

  internal void Validate()
  {
    if (string.IsNullOrEmpty(ProjectId))
    {
      throw new Exception("ProjectId is required");
    }

    if (string.IsNullOrEmpty(EnvironmentSlug))
    {
      throw new Exception("EnvironmentSlug is required");
    }

    if (string.IsNullOrEmpty(SecretPath))
    {
      throw new Exception("SecretPath is required");
    }

    if (string.IsNullOrEmpty(SecretName))
    {
      throw new Exception("SecretName is required");
    }
  }
}

public class CreateSecretOptions
{
  [JsonPropertyName("secretName")]
  public string SecretName { get; init; } = string.Empty;

  [JsonPropertyName("workspaceId")]
  public string? ProjectId { get; init; } = null;

  [JsonPropertyName("environment")]
  public string? EnvironmentSlug { get; init; } = null;

  [JsonPropertyName("secretValue")]
  public string SecretValue { get; init; } = string.Empty;

  [JsonPropertyName("secretPath")]
  public string SecretPath { get; init; } = "/";

  [JsonPropertyName("secretComment")]
  public string? SecretComment { get; init; } = null;

  [JsonPropertyName("secretMetadata")]
  public SecretMetadata[]? Metadata { get; set; } = null;

  [JsonPropertyName("skipMultilineEncoding")]
  public bool? SkipMultilineEncoding { get; init; } = null;

  [JsonPropertyName("type")]
  public SecretType? Type { get; init; } = null;

  [JsonPropertyName("secretReminderRepeatDays")]
  public int? SecretReminderRepeatDays { get; init; } = null;

  [JsonPropertyName("secretReminderNote")]
  public string? SecretReminderNote { get; init; } = null;

  [JsonPropertyName("isRotatedSecret")]
  public bool? IsRotatedSecret { get; init; } = null;

  internal void Validate()
  {
    if (string.IsNullOrEmpty(ProjectId))
    {
      throw new Exception("ProjectId is required");
    }

    if (string.IsNullOrEmpty(EnvironmentSlug))
    {
      throw new Exception("EnvironmentSlug is required");
    }

    if (string.IsNullOrEmpty(SecretName))
    {
      throw new Exception("SecretName is required");
    }

    if (string.IsNullOrEmpty(SecretValue))
    {
      throw new Exception("SecretValue is required");
    }

    if (string.IsNullOrEmpty(SecretPath))
    {
      throw new Exception("SecretPath is required");
    }
  }
}

public class UpdateSecretOptions
{
  [JsonPropertyName("secretName")]
  public string SecretName { get; init; } = string.Empty;

  [JsonPropertyName("newSecretName")]
  public string? NewSecretName { get; init; } = null;

  [JsonPropertyName("workspaceId")]
  public string? ProjectId { get; init; } = null;

  [JsonPropertyName("environment")]
  public string? EnvironmentSlug { get; init; } = null;

  [JsonPropertyName("type")]
  public SecretType? Type { get; init; } = null;

  [JsonPropertyName("secretPath")]
  public string SecretPath { get; init; } = "/";

  [JsonPropertyName("skipMultilineEncoding")]
  public bool? NewSkipMultilineEncoding { get; init; } = null;

  [JsonPropertyName("secretValue")]
  public string? NewSecretValue { get; init; } = null;

  [JsonPropertyName("secretComment")]
  public string? NewSecretComment { get; init; } = null;

  [JsonPropertyName("secretMetadata")]
  public SecretMetadata[]? NewMetadata { get; init; } = null;

  [JsonPropertyName("secretReminderNote")]
  public string? NewSecretReminderNote { get; init; } = null;

  [JsonPropertyName("secretReminderRepeatDays")]
  public int? NewSecretReminderRepeatDays { get; init; } = null;

  internal void Validate()
  {
    if (string.IsNullOrEmpty(ProjectId))
    {
      throw new Exception("ProjectId is required");
    }

    if (string.IsNullOrEmpty(SecretName))
    {
      throw new Exception("SecretName is required");
    }

    if (string.IsNullOrEmpty(SecretPath))
    {
      throw new Exception("SecretPath is required");
    }

    if (string.IsNullOrEmpty(EnvironmentSlug))
    {
      throw new Exception("EnvironmentSlug is required");
    }
  }
}

public class DeleteSecretOptions
{
  [JsonPropertyName("secretName")]
  public string SecretName { get; init; } = string.Empty;

  [JsonPropertyName("workspaceId")]
  public string? ProjectId { get; init; } = null;

  [JsonPropertyName("environment")]
  public string? EnvironmentSlug { get; init; } = null;

  [JsonPropertyName("secretPath")]
  public string SecretPath { get; init; } = "/";

  internal void Validate()
  {
    if (string.IsNullOrEmpty(ProjectId))
    {
      throw new Exception("ProjectId is required");
    }

    if (string.IsNullOrEmpty(SecretName))
    {
      throw new Exception("SecretName is required");
    }

    if (string.IsNullOrEmpty(SecretPath))
    {
      throw new Exception("SecretPath is required");
    }

    if (string.IsNullOrEmpty(EnvironmentSlug))
    {
      throw new Exception("EnvironmentSlug is required");
    }

  }

}

public class SecretMetadata
{
  [JsonPropertyName("key")]
  public string Key { get; set; } = string.Empty;
  [JsonPropertyName("value")]
  public string Value { get; set; } = string.Empty;
}


public class Secret
{
  [JsonPropertyName("id")]
  public string Id { get; set; } = string.Empty;
  [JsonPropertyName("workspace")]
  public string ProjectId { get; set; } = string.Empty;

  [JsonPropertyName("environment")]
  public string Environment { get; set; } = string.Empty;

  [JsonPropertyName("version")]
  public int Version { get; set; }

  [JsonPropertyName("secretKey")]
  public string SecretKey { get; set; } = string.Empty;

  [JsonPropertyName("secretValue")]
  public string SecretValue { get; set; } = string.Empty;

  [JsonPropertyName("secretComment")]
  public string SecretComment { get; set; } = string.Empty;

  [JsonPropertyName("secretReminderNote")]
  public string? SecretReminderNote { get; set; } = null;

  [JsonPropertyName("secretReminderRepeatDays")]
  public int? SecretReminderRepeatDays { get; set; } = null;


  [JsonPropertyName("skipMultilineEncoding")]
  public bool? SkipMultilineEncoding { get; set; } = null;

  [JsonPropertyName("isRotatedSecret")]
  public bool IsRotatedSecret { get; set; } = false;
  [JsonPropertyName("rotationId")]
  public string? RotationId { get; set; } = null;

  [JsonPropertyName("secretPath")]
  public string SecretPath { get; set; } = string.Empty;

  [JsonPropertyName("secretMetadata")]
  public SecretMetadata[] Metadata { get; set; } = Array.Empty<SecretMetadata>();
}

class SecretImport
{
  [JsonPropertyName("secretPath")]
  public string SecretPath { get; set; } = string.Empty;

  [JsonPropertyName("environment")]
  public string Environment { get; set; } = string.Empty;

  [JsonPropertyName("secrets")]
  public Secret[] Secrets { get; set; } = Array.Empty<Secret>();
}

class ListSecretsResponse
{
  [JsonPropertyName("secrets")]
  public Secret[] Secrets { get; set; } = Array.Empty<Secret>();

  [JsonPropertyName("imports")]
  public SecretImport[] Imports { get; set; } = Array.Empty<SecretImport>();
}

class GetSecretResponse
{
  [JsonPropertyName("secret")]
  public Secret Secret { get; set; } = new Secret();
}

class CreateSecretResponse
{
  [JsonPropertyName("secret")]
  public Secret Secret { get; set; } = new Secret();
}

class UpdateSecretResponse
{
  [JsonPropertyName("secret")]
  public Secret Secret { get; set; } = new Secret();
}

class DeleteSecretResponse
{
  [JsonPropertyName("secret")]
  public Secret Secret { get; set; } = new Secret();
}