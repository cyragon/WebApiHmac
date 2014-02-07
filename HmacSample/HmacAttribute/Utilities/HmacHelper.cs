using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HmacAttribute.Utilities
{
	public class HmacHelper
	{
		public static string CalculateHmac(HttpRequestMessage request, string key)
		{
			string inputString = "";
			if (request.Content != null)
			{
				var contentTask = request.Content.ReadAsStringAsync();
				contentTask.Wait();
				inputString = contentTask.Result;
			}

			string accessToken = request.Headers.GetValues("Client-Access-Token").First();
			inputString += "Client-Access-Token|" + accessToken;

			var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
			byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputString));
			var sb = new StringBuilder(hash.Length * 2);
			foreach (byte b in hash)
			{
				sb.AppendFormat("{0:x2}", b);
			}
			return sb.ToString();
		}
	}
}
