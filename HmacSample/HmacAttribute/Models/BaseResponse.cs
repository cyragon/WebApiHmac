using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HmacAttribute.Models
{
    public class BaseResponse
    {
        public bool ErrorOccurred { get; set; }
        public string ErrorMessage { get; set; }
    }
}