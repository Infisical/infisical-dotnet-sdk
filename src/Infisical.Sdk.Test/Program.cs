namespace Infisical.Sdk.Samples;

using System.Text.Json;
using Infisical.Sdk;
using Infisical.Sdk.Model;

internal class Program
{

  public static string RandomString(int length)
  {
    return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length)
        .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
  }

  private static void Main(string[] args)
  {

    var machineIdentityClientId = Environment.GetEnvironmentVariable("MACHINE_IDENTITY_UNIVERSAL_AUTH_CLIENT_ID");
    var machineIdentityClientSecret = Environment.GetEnvironmentVariable("MACHINE_IDENTITY_UNIVERSAL_AUTH_CLIENT_SECRET");
    var projectId = Environment.GetEnvironmentVariable("MACHINE_IDENTITY_PROJECT_ID");


    if (string.IsNullOrEmpty(machineIdentityClientId) || string.IsNullOrEmpty(machineIdentityClientSecret) || string.IsNullOrEmpty(projectId))
    {

      Console.WriteLine("INFISICAL_MACHINE_IDENTITY_CLIENT_ID: " + machineIdentityClientId);
      Console.WriteLine("INFISICAL_MACHINE_IDENTITY_CLIENT_SECRET: " + machineIdentityClientSecret);
      Console.WriteLine("INFISICAL_PROJECT_ID: " + projectId);

      Console.WriteLine("INFISICAL_MACHINE_IDENTITY_CLIENT_ID, INFISICAL_MACHINE_IDENTITY_CLIENT_SECRET, and INFISICAL_PROJECT_ID must be set");
      return;
    }


    var settings = new InfisicalSdkSettingsBuilder()
      .WithHostUri("http://localhost:8080")
      .Build();


    var client = new InfisicalClient(settings);
    var _ = client.Auth().UniversalAuth().LoginAsync(machineIdentityClientId, machineIdentityClientSecret).Result;

    var options = new ListSecretsOptions
    {
      SetSecretsAsEnvironmentVariables = true,
      EnvironmentSlug = "dev",
      SecretPath = "/test",
      Recursive = true,
      // ExpandSecretReferences = true,
      ProjectId = projectId,
      // ViewSecretValue = true,
    };
    Console.WriteLine("\n\n\nList secrets response:");
    Console.WriteLine(JsonSerializer.Serialize(client.Secrets().ListAsync(options).Result, new JsonSerializerOptions { WriteIndented = true }));


    var envVars = Environment.GetEnvironmentVariables();
    Console.WriteLine("\n\n\nEnvironment variables:");
    foreach (var envVar in envVars)
    {
      Console.WriteLine($"{envVar} {Environment.NewLine}");
    }


    var getSecretOptions = new GetSecretOptions
    {
      SecretName = "DEV_SEC",
      EnvironmentSlug = "dev",
      SecretPath = "/test",
      ProjectId = projectId,
    };
    Console.WriteLine("\n\n\nGet secret response:");
    Console.WriteLine(JsonSerializer.Serialize(client.Secrets().GetAsync(getSecretOptions).Result, new JsonSerializerOptions { WriteIndented = true }));


    var newSecretName = $".NET-SDK-TEST-{RandomString(32)}";

    var createSecretOptions = new CreateSecretOptions
    {
      SecretName = newSecretName,
      EnvironmentSlug = "dev",
      SecretPath = "/test",
      SecretValue = RandomString(10),
      ProjectId = projectId,
    };

    Console.WriteLine("\n\n\nCreate secret response:");
    Console.WriteLine(JsonSerializer.Serialize(client.Secrets().CreateAsync(createSecretOptions).Result, new JsonSerializerOptions { WriteIndented = true }));



    var updateSecretOptions = new UpdateSecretOptions
    {
      SecretName = newSecretName,
      EnvironmentSlug = "dev",
      SecretPath = "/test",
      NewSecretName = $"{newSecretName}-updated-name",
      NewSecretValue = $"{RandomString(10)}-updated-value",
      ProjectId = projectId,
    };

    Console.WriteLine("\n\n\nUpdate secret response:");
    Console.WriteLine(JsonSerializer.Serialize(client.Secrets().UpdateAsync(updateSecretOptions).Result, new JsonSerializerOptions { WriteIndented = true }));


    var deleteSecretOptions = new DeleteSecretOptions
    {
      SecretName = $"{newSecretName}-updated-name",
      EnvironmentSlug = "dev",
      SecretPath = "/test",
      ProjectId = projectId,
    };

    Console.WriteLine("\n\n\nDelete secret response:");
    Console.WriteLine(JsonSerializer.Serialize(client.Secrets().DeleteAsync(deleteSecretOptions).Result, new JsonSerializerOptions { WriteIndented = true }));
  }
}