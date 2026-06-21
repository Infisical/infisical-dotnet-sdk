using System.Threading.Tasks;
using Infisical.Sdk.Api;
using Infisical.Sdk.Client;
using Infisical.Sdk.Model;

namespace Infisical.Sdk
{
  public class InfisicalClient
  {
    internal ApiClient _apiClient;
    private AuthClient _authClient;
    private SecretsClient _secretsClient;
    private PkiClient _pkiClient;
    private FoldersClient _foldersClient;
    public InfisicalClient(InfisicalSdkSettings settings)
    {
      _apiClient = new ApiClient(settings.HostUri);
      _secretsClient = new SecretsClient(_apiClient);
      _authClient = new AuthClient(_apiClient, (accessToken) => _apiClient.SetAccessToken(accessToken));
      _pkiClient = new PkiClient(_apiClient);
      _foldersClient = new FoldersClient(_apiClient);
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

    public FoldersClient Folders()
    {
      return _foldersClient;
    }
  }
}