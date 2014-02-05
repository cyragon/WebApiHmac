using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HmacAttribute.Utilities;

namespace HmacAttribute.Handlers
{
    public class HmacGenerationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            // TODO: Only register this handle for API routes
            if (request != null && request.RequestUri.AbsoluteUri.Contains("/api/"))
            {
                bool validRequest = true;

                validRequest = request.Headers.Contains("Client-Signature");

                var dataAccess = new DataAccess();
                string accessToken = "";


                if (validRequest)
                {
                    accessToken = request.Headers.GetValues("Client-Access-Token").First();
                    if (!dataAccess.VerifyAccessToken(accessToken))
                    {   
                        validRequest = false;
                    }
                }
                string securityToken = null;
                if (validRequest)
                {
                    securityToken = dataAccess.GetSecurityToken(accessToken);

                    if (securityToken == null)
                    {
                        validRequest = false;
                    }
                }

                if (validRequest)
                {
                    var calculatedSignature = HmacHelper.CalculateHmac(request, securityToken);
                    request.Properties["CalculatedHmacSignature"] = calculatedSignature;
                }
            }

            return base.SendAsync(request, cancellationToken)
             .ContinueWith(task =>
             {
                 var response = task.Result;
                 return response;
             });
        }
    }
}