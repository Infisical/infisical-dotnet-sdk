using Infisical.Sdk.Api;
using Infisical.Sdk.Util;

namespace Infisical.Sdk.Client;

public class SecretsClient
{

  public SecretsClient(ApiClient apiClient)
  {
    _apiClient = apiClient;
  }

  /// <summary>
  /// Resolves a project slug to a project ID by calling the Infisical API.
  /// If ProjectId is already set, this is a no-op.
  /// </summary>
  private async Task<string> ResolveProjectIdAsync(string? projectId, string? projectSlug)
  {
    if (!string.IsNullOrEmpty(projectId))
    {
      return projectId!;
    }

    if (string.IsNullOrEmpty(projectSlug))
    {
      throw new InfisicalException("Either ProjectId or ProjectSlug is required");
    }

    try
    {
      var escapedSlug = Uri.EscapeDataString(projectSlug);
      var response = await _apiClient.GetAsync<ProjectBySlugResponse>($"/api/v1/projects/slug/{escapedSlug}").ConfigureAwait(false);

      if (string.IsNullOrEmpty(response.Id))
      {
        throw new InfisicalException($"Project slug '{projectSlug}' resolved to an empty project ID");
      }

      return response.Id;
    }
    catch (InfisicalException)
    {
      throw;
    }
    catch (Exception e)
    {
      throw new InfisicalException($"Failed to resolve project slug '{projectSlug}' to a project ID", e);
    }
  }

  public async Task<Secret[]> ListAsync(ListSecretsOptions options)
  {
    try
    {
      options.Validate();

      // Resolve ProjectSlug to ProjectId if needed
      options.ProjectId = await ResolveProjectIdAsync(options.ProjectId, options.ProjectSlug).ConfigureAwait(false);

      var dict = ObjectToDictionaryConverter.ToDictionary(options, false);
      dict.Remove("tagSlugs");

      if (options.TagSlugs != null && options.TagSlugs.Length > 0)
      {
        dict["tagSlugs"] = string.Join(",", options.TagSlugs);
      }

      var response = await _apiClient.GetAsync<ListSecretsResponse>("/api/v3/secrets/raw", dict).ConfigureAwait(false);


      List<Secret> secrets = response.Secrets.ToList();

      if (options.Recursive == true)
      {
        // only run this if recursive is true for better performance
        SecretsUtil.EnsureUniqueSecretsByKey(secrets);
      }

      if (options.IncludeImports == true && response.Imports != null && response.Imports.Length > 0)
      {
        foreach (var import in response.Imports)
        {
          if (import.Secrets != null && import.Secrets.Length > 0)
          {
            foreach (var importSecret in import.Secrets)
            {
              // CASE: We need to ensure that the imported values don't override the "base" secrets.
              // Priority order is:
              // Local/Preset variables -> Actual secrets -> Imported secrets (high->low)

              // Check if the secret already exists in the secrets list
              if (!secrets.Any(secret => secret.SecretKey == importSecret.SecretKey))
              {
                if (options.ProjectId != null)
                {
                  importSecret.ProjectId = options.ProjectId;
                }
                importSecret.SecretPath = import.SecretPath;
                secrets.Add(importSecret);
              }
            }
          }
        }
      }

      if (options.SetSecretsAsEnvironmentVariables == true)
      {
        foreach (var secret in secrets)
        {
          if (Environment.GetEnvironmentVariable(secret.SecretKey) == null)
          {
            Environment.SetEnvironmentVariable(secret.SecretKey, secret.SecretValue);
          }
        }
      }

      return secrets.ToArray();

    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to list secrets", e);
    }
  }

  public async Task<Secret> GetAsync(GetSecretOptions options)
  {
    try
    {

      options.Validate();

      // Resolve ProjectSlug to ProjectId if needed
      options.ProjectId = await ResolveProjectIdAsync(options.ProjectId, options.ProjectSlug).ConfigureAwait(false);

      var dict = ObjectToDictionaryConverter.ToDictionary(options, false);

      var response = await _apiClient.GetAsync<GetSecretResponse>($"/api/v3/secrets/raw/{options.SecretName}", dict).ConfigureAwait(false);

      if (string.IsNullOrEmpty(response.Secret.SecretPath))
      {
        response.Secret.SecretPath = options.SecretPath;
      }

      return response.Secret;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to get secret", e);
    }
  }

  public async Task<Secret> CreateAsync(CreateSecretOptions options)
  {
    try
    {

      options.Validate();

      // Resolve ProjectSlug to ProjectId if needed
      options.ProjectId = await ResolveProjectIdAsync(options.ProjectId, options.ProjectSlug).ConfigureAwait(false);

      var response = await _apiClient.PostAsync<CreateSecretOptions, CreateSecretResponse>($"/api/v3/secrets/raw/{options.SecretName}", options, true).ConfigureAwait(false);
      return response.Secret;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to create secret", e);
    }
  }

  public async Task<Secret> UpdateAsync(UpdateSecretOptions options)
  {
    try
    {
      options.Validate();

      // Resolve ProjectSlug to ProjectId if needed
      options.ProjectId = await ResolveProjectIdAsync(options.ProjectId, options.ProjectSlug).ConfigureAwait(false);

      var response = await _apiClient.PatchAsync<UpdateSecretOptions, UpdateSecretResponse>($"/api/v3/secrets/raw/{options.SecretName}", options, true).ConfigureAwait(false);

      return response.Secret;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to update secret", e);
    }
  }

  public async Task<Secret> DeleteAsync(DeleteSecretOptions options)
  {
    try
    {
      options.Validate();

      // Resolve ProjectSlug to ProjectId if needed
      options.ProjectId = await ResolveProjectIdAsync(options.ProjectId, options.ProjectSlug).ConfigureAwait(false);

      var response = await _apiClient.DeleteAsync<DeleteSecretOptions, DeleteSecretResponse>($"/api/v3/secrets/raw/{options.SecretName}", options, true).ConfigureAwait(false);
      return response.Secret;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to delete secret", e);
    }
  }

  private readonly ApiClient _apiClient;
}