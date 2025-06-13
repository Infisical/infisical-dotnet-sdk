

namespace Infisical.Sdk.Util
{

  public static class SecretsUtil
  {

    // currently the last secret in the list is kept
    public static void EnsureUniqueSecretsByKey(IList<Secret> secrets)
    {
      var secretMap = new Dictionary<string, Secret>();
      foreach (var secret in secrets)
      {
        secretMap[secret.SecretKey] = secret;
      }

      secrets.Clear();
      foreach (var secret in secretMap.Values)
      {
        secrets.Add(secret);
      }
    }
  }


}