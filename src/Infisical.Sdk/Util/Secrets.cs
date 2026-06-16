

namespace Infisical.Sdk.Util
{

  public static class SecretsUtil
  {

    // Deduplicates secrets using a composite key of SecretPath + SecretKey.
    // This ensures secrets with the same name in different folders
    // (e.g. /elasticsearch/username and /postgres/username) are preserved.
    // When duplicates exist at the same path, the last secret in the list is kept.
    public static void EnsureUniqueSecretsByKey(IList<Secret> secrets)
    {
      var secretMap = new Dictionary<string, Secret>();
      foreach (var secret in secrets)
      {
        var compositeKey = $"{secret.SecretPath}:{secret.SecretKey}";
        secretMap[compositeKey] = secret;
      }

      secrets.Clear();
      foreach (var secret in secretMap.Values)
      {
        secrets.Add(secret);
      }
    }
  }


}