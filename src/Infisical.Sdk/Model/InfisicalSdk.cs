namespace Infisical.Sdk.Model
{

  public enum InfisicalAuthMethod
  {
    Universal,
    Token,
  }

  public class InfisicalUniversalAuth
  {

    public InfisicalUniversalAuth(string clientId, string clientSecret)
    {
      _clientId = clientId;
      _clientSecret = clientSecret;
    }

    internal string _clientId;
    internal string _clientSecret;
  }

  public class InfisicalTokenAuth
  {
    public InfisicalTokenAuth(string token)
    {
      _token = token;
    }

    internal string _token;
  }

  public class InfisicalAuth
  {

    private InfisicalUniversalAuth? _universalAuth;
    private InfisicalTokenAuth? _tokenAuth;
    private InfisicalAuthMethod _authMethod;

    public InfisicalAuth(InfisicalUniversalAuth universalAuth)
    {
      _universalAuth = universalAuth;
      _authMethod = InfisicalAuthMethod.Universal;
    }

    public InfisicalAuth(InfisicalTokenAuth tokenAuth)
    {
      _tokenAuth = tokenAuth;
      _authMethod = InfisicalAuthMethod.Token;
    }

    internal InfisicalAuthMethod GetAuthMethod()
    {
      return _authMethod;
    }

    internal InfisicalUniversalAuth GetUniversalAuth()
    {
      if (_authMethod != InfisicalAuthMethod.Universal)
      {
        throw new Exception($"Unable to get universal auth details. Auth method is set to {_authMethod}");
      }

      if (_universalAuth == null)
      {
        throw new Exception("Universal auth details are not set");
      }

      return _universalAuth;
    }

    internal InfisicalTokenAuth GetTokenAuth()
    {
      if (_authMethod != InfisicalAuthMethod.Token)
      {
        throw new Exception($"Unable to get token auth details. Auth method is set to {_authMethod}");
      }

      if (_tokenAuth == null)
      {
        throw new Exception("Token auth details are not set");
      }

      return _tokenAuth;
    }

  }

  public class InfisicalSdkSettings
  {
    public string HostUri { get; internal set; } = "https://app.infisical.com";

    /// <summary>
    /// When true, the SDK automatically checks for the INFISICAL_TOKEN environment
    /// variable at initialization and uses it if present. Defaults to false.
    /// </summary>
    public bool AutoDetectToken { get; internal set; } = false;

    internal InfisicalSdkSettings() { }
  }

  public class InfisicalSdkSettingsBuilder
  {
    private InfisicalSdkSettings _settings = new InfisicalSdkSettings();

    public InfisicalSdkSettingsBuilder WithHostUri(string hostUri)
    {
      _settings.HostUri = hostUri;
      return this;
    }

    /// <summary>
    /// Controls whether the SDK automatically detects and uses the INFISICAL_TOKEN
    /// environment variable for authentication. Defaults to false.
    /// Set to true to enable zero-config local development auth.
    /// </summary>
    public InfisicalSdkSettingsBuilder WithAutoDetectToken(bool autoDetect)
    {
      _settings.AutoDetectToken = autoDetect;
      return this;
    }

    public InfisicalSdkSettings Build()
    {
      // we return a new class to make it immutable
      return new InfisicalSdkSettings
      {
        HostUri = _settings.HostUri,
        AutoDetectToken = _settings.AutoDetectToken
      };
    }
  }
}