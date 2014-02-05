using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace HmacAttribute.Filters
{
    public class AccessTokenRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            bool validRequest = true;
            validRequest = actionContext.Request.Headers.Contains("Client-Access-Token");

            if (validRequest)
            {
                string accessToken = actionContext.Request.Headers.GetValues("Client-Access-Token").First();

                var dataAccess = new DataAccess();

                if (!dataAccess.VerifyAccessToken(accessToken))
                {
                    validRequest = false;
                }
            }

            if (!validRequest)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("{\"ErrorOccurred\":true,\"ErrorMessage\":\"A client access token was not provided or was not allowed to access this resource.\"}", Encoding.UTF8,
                                                     "text/json");
                actionContext.Response = response;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}