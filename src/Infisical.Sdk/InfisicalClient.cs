using System.Threading.Tasks;
using Infisical.Sdk.Api;
using Infisical.Sdk.Client;
using Infisical.Sdk.Model;

namespace Infisical.Sdk
{
  public class InfisicalClient
  {
    /// <summary>
    /// The environment variable name that the SDK checks for automatic authentication.
    /// </summary>
    public const string TokenEnvVarName = "INFISICAL_TOKEN";

    internal ApiClient _apiClient;
    private AuthClient _authClient;
    private SecretsClient _secretsClient;
    private PkiClient _pkiClient;
    private bool _isAuthenticated;

    public InfisicalClient(InfisicalSdkSettings settings)
    {
      _apiClient = new ApiClient(settings.HostUri);
      _secretsClient = new SecretsClient(_apiClient);
      _authClient = new AuthClient(_apiClient, (accessToken) =>
      {
        _apiClient.SetAccessToken(accessToken);
        _isAuthenticated = true;
      });
      _pkiClient = new PkiClient(_apiClient);

      if (settings.AutoDetectToken)
      {
        var token = Environment.GetEnvironmentVariable(TokenEnvVarName);
        if (!string.IsNullOrEmpty(token))
        {
          _authClient.AccessTokenAuth().Login(token);
        }
      }
    }

    public AuthClient Auth()
    {
      return _authClient;
    }

    public SecretsClient Secrets()
    {
      return _secretsClient;
    }

    public PkiClient Pki()
    {
      return _pkiClient;
    }

    /// <summary>
    /// Returns true if the client has been authenticated (via any method).
    /// </summary>
    public bool IsAuthenticated => _isAuthenticated;
  }
}