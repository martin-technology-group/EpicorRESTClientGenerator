using System.Net;

namespace EpicorRESTGenerator.Shared.Services
{
    public static class WebClientService
    {
        public static WebClient GetWebClient(string username = null, string password = null)
        {
            WebClient client = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback += (senderC, cert, chain, sslPolicyErrors) => true;

            if (!string.IsNullOrEmpty(username))
            {
                client.Credentials = new NetworkCredential() { UserName = username, Password = password };
            }
            else
            {
                client.UseDefaultCredentials = true;
            }

            return client;
        }
    }
}
