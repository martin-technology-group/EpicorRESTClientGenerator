using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace EpicorRESTClient.Base
{
    public class ClientBase
    {
        private readonly string certFilePath;

        public ClientBase(string certPath = null)
        {
            certFilePath = certPath;
        }

        protected HttpClient CreateHttpClient()
        {
            //default set to true for internal use, 
            //may want to make into a method call and validate the cert based off of hash
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                if (!string.IsNullOrEmpty(certFilePath))
                {
                    var hash = X509Certificate.CreateFromCertFile(certFilePath).GetCertHashString();
                    return errors == SslPolicyErrors.None && hash == certificate.GetCertHashString();
                }
                return errors == SslPolicyErrors.None;
            };

            //for windows authentication
            var handler = new HttpClientHandler { UseDefaultCredentials = true };
            var client = new HttpClient(handler);

            //for token authentication
            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getToken());

            //for basic authentication
            //var credentials = Encoding.ASCII.GetBytes("{Username}:{Password}");
            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

            return client;
        }
    }
}

