using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;

namespace HmacAttribute.Filters
{
    public class ClientSignatureRequiredAttribute : AccessTokenRequiredAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // TODO: Setup filter chaining instead of inheritance?  Not sure which one is a better approach.

            base.OnActionExecuting(actionContext);
            if (actionContext.Response == null)
            {
                bool validRequest = true;

                validRequest = actionContext.Request.Headers.Contains("Client-Signature");
                string clientSignature = null;
                string calculatedSignature = string.Empty;

                if (validRequest)
                {
                    var dataAccess = new DataAccess();

                    string accessToken = actionContext.Request.Headers.GetValues("Client-Access-Token").First();
                    if (dataAccess.VerifyAccessToken(accessToken) && actionContext.Request.Properties.ContainsKey("CalculatedHmacSignature"))
                    {
                        clientSignature = actionContext.Request.Headers.GetValues("Client-Signature").First();
                        calculatedSignature = (string)actionContext.Request.Properties["CalculatedHmacSignature"];
                    }
                }

                if (validRequest)
                {
                    if (calculatedSignature.ToLower() != clientSignature.ToLower())
                    {
                        validRequest = false;
                    }
                }

                if (!validRequest)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent("{\"ErrorOccurred\":true,\"ErrorMessage\":\"The request does not specify a client signature or the client signature is invalid.\"}", Encoding.UTF8,
                                                         "text/json");
                    actionContext.Response = response;
                }
            }

        }
    }
}