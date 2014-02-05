using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HmacAttribute.Filters;
using HmacAttribute.Models;

namespace HmacAttribute.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [ClientSignatureRequired]
        public DataMessageResponse Get(int id)
        {
            var response = new DataMessageResponse();
            response.Message = "value" + id;

            return response;
        }

    }
}