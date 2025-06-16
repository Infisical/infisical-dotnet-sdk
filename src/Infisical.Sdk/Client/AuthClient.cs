using Infisical.Sdk.Api;
using Infisical.Sdk.Model;

namespace Infisical.Sdk.Client;


public class UniversalAuth
{

  public UniversalAuth(ApiClient apiClient, Action<string> setAccessTokenFunc)
  {
    _apiClient = apiClient;
    _setAccessTokenFunc = setAccessTokenFunc;
  }

  public async Task<MachineIdentityCredential> LoginAsync(string clientId, string clientSecret)
  {
    try
    {
      var loginRequest = new UniversalAuthLoginRequest(clientId, clientSecret);

      var response = await _apiClient.PostAsync<UniversalAuthLoginRequest, MachineIdentityCredential>("/api/v1/auth/universal-auth/login", loginRequest);
      _setAccessTokenFunc(response.AccessToken);
      return response;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to login", e);
    }
  }

  private readonly ApiClient _apiClient;
  private readonly Action<string> _setAccessTokenFunc;
}

public class AuthClient
{
  private readonly ApiClient _apiClient;
  UniversalAuth _universalAuth;
  private readonly Action<string> _setAccessTokenFunc;

  public AuthClient(ApiClient apiClient, Action<string> setAccessTokenFunc)
  {
    _apiClient = apiClient;
    _setAccessTokenFunc = setAccessTokenFunc;
    _universalAuth = new UniversalAuth(_apiClient, _setAccessTokenFunc);
  }

  public UniversalAuth UniversalAuth()
  {
    return _universalAuth;
  }
}