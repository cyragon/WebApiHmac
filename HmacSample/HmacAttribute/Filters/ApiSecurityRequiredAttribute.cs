using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using HmacAttribute.Controllers;
using Newtonsoft.Json;

namespace HmacAttribute.Filters
{
    public class ApiSecurityRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            bool validRequest = true;
            // do we have the access token and the signature?
            validRequest = actionContext.Request.Headers.Contains("Client-Access-Token") && actionContext.Request.Headers.Contains("Client-Signature");

            if (validRequest)
            {
                string accessToken = actionContext.Request.Headers.GetValues("Client-Access-Token").First();

                var dataAccess = new DataAccess();

                if (!dataAccess.VerifyAccessToken(accessToken))
                {
                    validRequest = false;
                }
                else
                {
                    // we have a valid request, so let's make sure everything is signed
                    string content = "";

                    if (actionContext.Request.Content != null)
                    {
                        // so, let's make sure the steam is at the start
                        actionContext.Request.Content.ReadAsStreamAsync().ContinueWith(t => t.Result.Seek(0, SeekOrigin.Begin)).Wait();
                        // read the stream
                        content = actionContext.Request.Content.ReadAsStringAsync().Result;
                        // reset the stream for the target controller, or anyone else that want's it.
                        actionContext.Request.Content.ReadAsStreamAsync().ContinueWith(t => t.Result.Seek(0, SeekOrigin.Begin)).Wait();
                    }

                    content += "Client-Access-Token|" + accessToken;
                    // get the secret
                    var securityToken = dataAccess.GetSecurityToken(accessToken);

                    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(securityToken));
                    byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(content));
                    var calculatedSignature = new StringBuilder(hash.Length * 2);
                    foreach (byte b in hash)
                    {
                        calculatedSignature.AppendFormat("{0:x2}", b);
                    }

                    // is the client signature and the computed one the same?
                    if (calculatedSignature.ToString().ToLower() != actionContext.Request.Headers.GetValues("Client-Signature").First().ToLower())
                    {
                        validRequest = false;
                    }

                }
            }

            if (!validRequest)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("{\"ErrorOccurred\":true,\"ErrorMessage\":\"A client access token or signature was not provided or was not allowed to access this resource.\"}", Encoding.UTF8,
                                                     "text/json");
                actionContext.Response = response;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}