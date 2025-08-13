using System.ComponentModel.DataAnnotations;
using Infisical.Sdk.Api;
using Infisical.Sdk.Model;
using Infisical.Sdk.Util;

namespace Infisical.Sdk.Client;


public class Subscribers
{

  public Subscribers(ApiClient apiClient)
  {
    _apiClient = apiClient;
  }

  public async Task<CertificateBundle> RetrieveLatestCertificateBundleAsync(RetrieveLatestCertificateBundleOptions options)
  {
    try
    {
      options.Validate();
      var dict = ObjectToDictionaryConverter.ToDictionary(options, false);

      var response = await _apiClient.GetAsync<CertificateBundle>(
        $"/api/v1/pki/subscribers/{options.SubscriberName}/latest-certificate-bundle",
        dict
      );

      return response;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to retrieve latest certificate bundle", e);
    }
  }

  public async Task<SubscriberIssuedCertificate> IssueCertificateAsync(IssueCertificateOptions options)
  {
    try
    {
      options.Validate();

      var response = await _apiClient.PostAsync<IssueCertificateOptions, SubscriberIssuedCertificate>($"/api/v1/pki/subscribers/{options.SubscriberName}/issue-certificate", options, true);

      return response;
    }
    catch (Exception e)
    {
      throw new InfisicalException("Failed to issue certificate", e);
    }
  }

  private readonly ApiClient _apiClient;
}

public class PkiClient
{
  private readonly ApiClient _apiClient;
  Subscribers _subscribers;

  public PkiClient(ApiClient apiClient)
  {
    _apiClient = apiClient;
    _subscribers = new Subscribers(_apiClient);
  }

  public Subscribers Subscribers()
  {
    return _subscribers;
  }
}