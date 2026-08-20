using System.Net;
using System.Text.Json;
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

  public async Task<InfisicalFolder[]> ListAsync(ListFoldersOptions options)
  {
    try
    {
      options.Validate();

      var queryParams = new Dictionary<string, string>
      {
        ["projectId"] = options.ProjectId!,
        ["environment"] = options.EnvironmentSlug!,
        ["path"] = options.Path,
        ["recursive"] = options.Recursive.ToString().ToLowerInvariant()
      };

      var response = await _apiClient.GetAsync<ListFoldersResponse>("/api/v2/folders", queryParams).ConfigureAwait(false);
      return response.Folders;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to list folders", e);
    }
  }

  public async Task<IReadOnlyList<InfisicalFolder>> EnsurePathAsync(EnsureFolderPathOptions options)
  {
    try
    {
      options.Validate();

      var ensuredFolders = new List<InfisicalFolder>();
      var parentPath = "/";
      var segments = options.Path
        .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(segment => segment.Trim())
        .Where(segment => !string.IsNullOrEmpty(segment))
        .ToArray();

      foreach (var segment in segments)
      {
        var existingFolder = await FindFolderAsync(options.ProjectId!, options.EnvironmentSlug!, parentPath, segment).ConfigureAwait(false);
        if (existingFolder != null)
        {
          ensuredFolders.Add(existingFolder);
          parentPath = AppendPathSegment(parentPath, segment);
          continue;
        }

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
          ensuredFolders.Add(response.Value!.Folder);
          parentPath = AppendPathSegment(parentPath, segment);
          continue;
        }

        if (!IsAlreadyExistsResponse(response.StatusCode, response.Content))
        {
          throw new InfisicalException(response.FormatErrorMessage());
        }

        existingFolder = await FindFolderAsync(options.ProjectId!, options.EnvironmentSlug!, parentPath, segment).ConfigureAwait(false);
        if (existingFolder == null)
        {
          throw new InfisicalException($"Infisical reported folder '{segment}' already exists under '{parentPath}', but listing the parent path did not return it.");
        }

        ensuredFolders.Add(existingFolder);
        parentPath = AppendPathSegment(parentPath, segment);
      }

      return ensuredFolders;
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

    var apiError = TryParseApiError(responseBody);
    if (apiError == null)
    {
      return false;
    }

    if (apiError.StatusCode.HasValue && apiError.StatusCode.Value != (int)statusCode)
    {
      return false;
    }

    var message = apiError.Message ?? string.Empty;
    return message.IndexOf("folder", StringComparison.OrdinalIgnoreCase) >= 0 &&
           message.IndexOf("already exist", StringComparison.OrdinalIgnoreCase) >= 0;
  }

  private async Task<InfisicalFolder?> FindFolderAsync(string projectId, string environmentSlug, string parentPath, string folderName)
  {
    var folders = await ListAsync(new ListFoldersOptions
    {
      ProjectId = projectId,
      EnvironmentSlug = environmentSlug,
      Path = parentPath,
      Recursive = false
    }).ConfigureAwait(false);

    return folders.FirstOrDefault(folder =>
      string.Equals(folder.Name, folderName, StringComparison.Ordinal) ||
      string.Equals(folder.RelativePath, AppendPathSegment(parentPath, folderName), StringComparison.Ordinal));
  }

  private static string AppendPathSegment(string parentPath, string segment)
  {
    return parentPath == "/" ? "/" + segment : parentPath + "/" + segment;
  }

  private static FolderApiError? TryParseApiError(string responseBody)
  {
    try
    {
      return JsonSerializer.Deserialize<FolderApiError>(responseBody, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });
    }
    catch (JsonException)
    {
      return null;
    }
  }
}
