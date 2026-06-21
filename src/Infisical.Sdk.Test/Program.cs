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

  private static async Task Main(string[] args)
  {

    var machineIdentityClientId = Environment.GetEnvironmentVariable("INFISICAL_MACHINE_IDENTITY_CLIENT_ID");
    var machineIdentityClientSecret = Environment.GetEnvironmentVariable("INFISICAL_MACHINE_IDENTITY_CLIENT_SECRET");
    var projectId = Environment.GetEnvironmentVariable("INFISICAL_PROJECT_ID");


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
    var _ = await client.Auth().UniversalAuth().LoginAsync(machineIdentityClientId, machineIdentityClientSecret);

    // sleep for 10 seconds
    Console.WriteLine("Sleeping for 5 seconds");
    Thread.Sleep(5000);

    Console.WriteLine("Done sleeping");

    var envVars = Environment.GetEnvironmentVariables();
    Console.WriteLine("\n\n\nEnvironment variables:");
    foreach (var envVar in envVars)
    {
      Console.WriteLine($"{envVar} {Environment.NewLine}");
    }

    var folderPath = $"/test/{RandomString(8)}";
    var ensureFolderPathOptions = new EnsureFolderPathOptions
    {
      EnvironmentSlug = "dev",
      Path = folderPath,
      ProjectId = projectId,
      Description = ".NET SDK sample folder"
    };

    Console.WriteLine("\n\n\nEnsure folder path response:");
    Console.WriteLine(JsonSerializer.Serialize(await client.Folders().EnsurePathAsync(ensureFolderPathOptions), new JsonSerializerOptions { WriteIndented = true }));

    var newSecretName = $".NET-SDK-TEST-{RandomString(32)}";

    var createSecretOptions = new CreateSecretOptions
    {
      SecretName = newSecretName,
      EnvironmentSlug = "dev",
      SecretPath = folderPath,
      SecretValue = RandomString(10),
      ProjectId = projectId,
    };

    Console.WriteLine("\n\n\nCreate secret response:");
    Console.WriteLine(JsonSerializer.Serialize(await client.Secrets().CreateAsync(createSecretOptions), new JsonSerializerOptions { WriteIndented = true }));

    var options = new ListSecretsOptions
    {
      SetSecretsAsEnvironmentVariables = true,
      EnvironmentSlug = "dev",
      SecretPath = folderPath,
      Recursive = true,
      // ExpandSecretReferences = true,
      ProjectId = projectId,
      // ViewSecretValue = true,
    };
    Console.WriteLine("\n\n\nList secrets response:");
    Console.WriteLine(JsonSerializer.Serialize(await client.Secrets().ListAsync(options), new JsonSerializerOptions { WriteIndented = true }));

    var getSecretOptions = new GetSecretOptions
    {
      SecretName = newSecretName,
      EnvironmentSlug = "dev",
      SecretPath = folderPath,
      ProjectId = projectId,
    };
    Console.WriteLine("\n\n\nGet secret response:");
    Console.WriteLine(JsonSerializer.Serialize(await client.Secrets().GetAsync(getSecretOptions), new JsonSerializerOptions { WriteIndented = true }));


    // Close the Universal Auth client and create a new one with LDAP Auth
    Console.WriteLine("\n\n\n=== Switching to LDAP Auth ===");
    
    var ldapIdentityId = Environment.GetEnvironmentVariable("LDAP_IDENTITY_ID");
    var ldapUsername = Environment.GetEnvironmentVariable("LDAP_USERNAME");
    var ldapPassword = Environment.GetEnvironmentVariable("LDAP_PASSWORD");

    if (string.IsNullOrEmpty(ldapIdentityId) || string.IsNullOrEmpty(ldapUsername) || string.IsNullOrEmpty(ldapPassword))
    {
      Console.WriteLine("LDAP_IDENTITY_ID, LDAP_USERNAME, and LDAP_PASSWORD must be set to test LDAP auth");
      Console.WriteLine("Skipping LDAP auth test and continuing with Universal Auth client...");
    }
    else
    {
      // Create a new client with LDAP auth
      var ldapClient = new InfisicalClient(settings);
      try
      {
        Console.WriteLine("Authenticating with LDAP...");
        var ldapCredential = await ldapClient.Auth().LdapAuth().LoginAsync(ldapIdentityId, ldapUsername, ldapPassword);
        Console.WriteLine($"✅ LDAP Auth successful! Token: {ldapCredential.AccessToken.Substring(0, Math.Min(20, ldapCredential.AccessToken.Length))}...");
        Console.WriteLine($"   Expires In: {ldapCredential.ExpiresIn} seconds");
        
        // Use the LDAP-authenticated client for remaining operations
        client = ldapClient;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"❌ LDAP Auth failed: {ex.Message}");
        if (ex.InnerException != null)
        {
          Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
        }
        Console.WriteLine("Continuing with Universal Auth client...");
      }
    }

    var updateSecretOptions = new UpdateSecretOptions
    {
      SecretName = newSecretName,
      EnvironmentSlug = "dev",
      SecretPath = folderPath,
      NewSecretName = $"{newSecretName}-updated-name",
      NewSecretValue = $"{RandomString(10)}-updated-value",
      ProjectId = projectId,
    };

    Console.WriteLine("\n\n\nUpdate secret response:");
    Console.WriteLine(JsonSerializer.Serialize(await client.Secrets().UpdateAsync(updateSecretOptions), new JsonSerializerOptions { WriteIndented = true }));


    var deleteSecretOptions = new DeleteSecretOptions
    {
      SecretName = $"{newSecretName}-updated-name",
      EnvironmentSlug = "dev",
      SecretPath = folderPath,
      ProjectId = projectId,
    };

    Console.WriteLine("\n\n\nDelete secret response:");
    Console.WriteLine(JsonSerializer.Serialize(await client.Secrets().DeleteAsync(deleteSecretOptions), new JsonSerializerOptions { WriteIndented = true }));
  }
}