using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using HmacAttribute.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HmacAttribute.Tests.Controllers
{
    [TestClass]
    public class ApiIntegrationTest
    {
        public string UserAccessToken { get; set; }
        public string UserSecurityToken { get; set; }
        private string baseAddress = "http://localhost:57878/";
                

        [TestMethod]
        public void GetValuesWithClientAccessToken()
        {
            GetClientAccessToken();

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseAddress + "api/Values/10");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Client-Access-Token", UserAccessToken);
            request.Method = HttpMethod.Get;

            request.Headers.Add("Client-Signature", HmacHelper.CalculateHmac(request, UserSecurityToken));
            HttpClient client = new HttpClient();

            var task = client.SendAsync(request);
            task.Wait();

            var response = task.Result;
            var responseString = response.Content.ReadAsStringAsync();
            responseString.Wait();
            var jsonResponse = new JavaScriptSerializer().Deserialize<dynamic>(responseString.Result);

            Assert.IsNotNull(response.Content);
            Assert.IsNotNull(response.Content.Headers.ContentType);
            Assert.IsFalse(jsonResponse["ErrorOccurred"]);
            Assert.AreEqual("value10", jsonResponse["Message"]);
        }
        
        [TestMethod]
        public void GetValuesShouldFailWithInvalidClientSignature()
        {
            GetClientAccessToken();

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseAddress + "api/Values/10");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Client-Access-Token", UserAccessToken);
            request.Method = HttpMethod.Get;

            request.Headers.Add("Client-Signature", Guid.NewGuid().ToString());
            HttpClient client = new HttpClient();

            var task = client.SendAsync(request);
            task.Wait();

            var response = task.Result;
            var responseString = response.Content.ReadAsStringAsync();
            responseString.Wait();
            var jsonResponse = new JavaScriptSerializer().Deserialize<dynamic>(responseString.Result);

            Assert.IsNotNull(response.Content);
            Assert.IsNotNull(response.Content.Headers.ContentType);
            Assert.IsTrue(jsonResponse["ErrorOccurred"]);
            Assert.AreEqual("The request does not specify a client signature or the client signature is invalid.", jsonResponse["ErrorMessage"]);
        }
        
        [TestMethod]
        public void GetValuesShouldFailWithoutClientAccessToken()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseAddress + "api/Values/10");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Method = HttpMethod.Get;

            HttpClient client = new HttpClient();

            var task = client.SendAsync(request);
            task.Wait();

            var response = task.Result;
            var responseString = response.Content.ReadAsStringAsync();
            responseString.Wait();
            var jsonResponse = new JavaScriptSerializer().Deserialize<dynamic>(responseString.Result);

            Assert.IsNotNull(response.Content);
            Assert.IsNotNull(response.Content.Headers.ContentType);
            Assert.IsTrue(jsonResponse["ErrorOccurred"]);
            Assert.AreEqual("A client access token was not provided or was not allowed to access this resource.", jsonResponse["ErrorMessage"]);
        }

        [TestMethod]
        public void GetValuesShouldFailWithInvalidClientAccessToken()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseAddress + "api/Values/10");
            request.Headers.Add("Client-Access-Token", Guid.NewGuid().ToString());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Method = HttpMethod.Get;

            HttpClient client = new HttpClient();

            var task = client.SendAsync(request);
            task.Wait();

            var response = task.Result;
            var responseString = response.Content.ReadAsStringAsync();
            responseString.Wait();
            var jsonResponse = new JavaScriptSerializer().Deserialize<dynamic>(responseString.Result);

            Assert.IsNotNull(response.Content);
            Assert.IsNotNull(response.Content.Headers.ContentType);
            Assert.IsTrue(jsonResponse["ErrorOccurred"]);
            Assert.AreEqual("A client access token was not provided or was not allowed to access this resource.", jsonResponse["ErrorMessage"]);
        }

        private void GetClientAccessToken()
        {
            // ATbec7b65c6885459aa2309e3e00c73b00/STbec7b65c6885459aa2309e3e00c73b00
            this.UserAccessToken = "ATbec7b65c6885459aa2309e3e00c73b00";
            this.UserSecurityToken = "STbec7b65c6885459aa2309e3e00c73b00";
        }
    }
}
