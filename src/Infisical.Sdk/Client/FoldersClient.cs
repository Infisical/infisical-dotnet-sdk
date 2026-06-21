using System.Net;
using Infisical.Sdk.Api;
using Infisical.Sdk.Model;

namespace Infisical.Sdk.Client;

public class FoldersClient
{
  private readonly ApiClient _apiClient;

  public FoldersClient(ApiClient apiClient)
  {
    _apiClient = apiClient;
  }

  public async Task<InfisicalFolder> CreateAsync(CreateFolderOptions options)
  {
    try
    {
      options.Validate();

      var response = await _apiClient.PostAsync<CreateFolderOptions, CreateFolderResponse>("/api/v2/folders", options, true).ConfigureAwait(false);
      return response.Folder;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to create folder", e);
    }
  }

  public async Task<IReadOnlyList<InfisicalFolder>> EnsurePathAsync(EnsureFolderPathOptions options)
  {
    try
    {
      options.Validate();

      var createdFolders = new List<InfisicalFolder>();
      var parentPath = "/";
      var segments = options.Path
        .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(segment => segment.Trim())
        .Where(segment => !string.IsNullOrEmpty(segment))
        .ToArray();

      foreach (var segment in segments)
      {
        var createOptions = new CreateFolderOptions
        {
          ProjectId = options.ProjectId,
          EnvironmentSlug = options.EnvironmentSlug,
          Name = segment,
          Path = parentPath,
          Description = options.Description
        };

        var response = await _apiClient.PostForResponseAsync<CreateFolderOptions, CreateFolderResponse>("/api/v2/folders", createOptions, true).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
          createdFolders.Add(response.Value!.Folder);
        }
        else if (!IsAlreadyExistsResponse(response.StatusCode, response.Content))
        {
          throw new InfisicalException(response.FormatErrorMessage());
        }

        parentPath = parentPath == "/" ? "/" + segment : parentPath + "/" + segment;
      }

      return createdFolders;
    }
    catch (Exception e) when (!(e is InfisicalException))
    {
      throw new InfisicalException("Failed to ensure folder path", e);
    }
  }

  internal static bool IsAlreadyExistsResponse(HttpStatusCode statusCode, string responseBody)
  {
    if (statusCode != HttpStatusCode.BadRequest && statusCode != HttpStatusCode.Conflict && (int)statusCode != 422)
    {
      return false;
    }

    return responseBody.IndexOf("already exists", StringComparison.OrdinalIgnoreCase) >= 0 ||
           responseBody.IndexOf("folder already", StringComparison.OrdinalIgnoreCase) >= 0 ||
           responseBody.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0;
  }
}
