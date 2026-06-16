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

      var response = await _apiClient.PostAsync<UniversalAuthLoginRequest, MachineIdentityCredential>("/api/v1/auth/universal-auth/login", loginRequest).ConfigureAwait(false);
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

public class LdapAuth
{

  public LdapAuth(ApiClient apiClient, Action<string> setAccessTokenFunc)
  {
    _apiClient = apiClient;
    _setAccessTokenFunc = setAccessTokenFunc;
  }

  public async Task<MachineIdentityCredential> LoginAsync(string identityId, string username, string password)
  {
    try
    {
      var loginRequest = new LdapAuthLoginRequest(identityId, username, password);

      var response = await _apiClient.PostAsync<LdapAuthLoginRequest, MachineIdentityCredential>("/api/v1/auth/ldap-auth/login", loginRequest).ConfigureAwait(false);
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

/// <summary>
/// Authenticates using a pre-existing access token (e.g. from INFISICAL_TOKEN env var
/// or from a local CLI session).
/// </summary>
public class AccessTokenAuth
{

  public AccessTokenAuth(Action<string> setAccessTokenFunc)
  {
    _setAccessTokenFunc = setAccessTokenFunc;
  }

  /// <summary>
  /// Authenticates the SDK using a raw access token.
  /// No network call is made — the token is used directly for subsequent API requests.
  /// </summary>
  public void Login(string accessToken)
  {
    if (string.IsNullOrEmpty(accessToken))
    {
      throw new InfisicalException("Access token cannot be null or empty");
    }

    _setAccessTokenFunc(accessToken);
  }

  private readonly Action<string> _setAccessTokenFunc;
}



public class AuthClient
{
  private readonly ApiClient _apiClient;
  UniversalAuth _universalAuth;
  LdapAuth _ldapAuth;
  AccessTokenAuth _accessTokenAuth;
  private readonly Action<string> _setAccessTokenFunc;

  public AuthClient(ApiClient apiClient, Action<string> setAccessTokenFunc)
  {
    _apiClient = apiClient;
    _setAccessTokenFunc = setAccessTokenFunc;
    _universalAuth = new UniversalAuth(_apiClient, _setAccessTokenFunc);
    _ldapAuth = new LdapAuth(_apiClient, _setAccessTokenFunc);
    _accessTokenAuth = new AccessTokenAuth(_setAccessTokenFunc);
  }

  public UniversalAuth UniversalAuth()
  {
    return _universalAuth;
  }

  public LdapAuth LdapAuth()
  {
    return _ldapAuth;
  }

  /// <summary>
  /// Returns the access token auth handler for direct token-based authentication.
  /// </summary>
  public AccessTokenAuth AccessTokenAuth()
  {
    return _accessTokenAuth;
  }
}
