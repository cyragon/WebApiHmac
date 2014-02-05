using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HmacAttribute
{
    public class DataAccess : HmacAttribute.IDataAccess
    {
        private CacheProvider Cache { get; set; }

        private List<string> AccountAccessTokens { get; set; }
        private Dictionary<string, string> AccountSecurityTokens { get; set; }

        public DataAccess()
        {
            Cache = new CacheProvider();

            LoadAccountAccessTokens();
            LoadAccountSecurityTokens();
        }

        public bool VerifyAccessToken(string accessToken)
        {
            return AccountAccessTokens.Contains(accessToken);
        }

        public string GetSecurityToken(string accessToken)
        {
            if (AccountSecurityTokens.ContainsKey(accessToken))
                return AccountSecurityTokens[accessToken];
            else
                return null;
        }

        private void LoadAccountAccessTokens()
        {
            var accessTokenData = Cache.Get("accessTokens") as List<string>;

            if (accessTokenData == null)
            {
                AccountAccessTokens = new List<string>();
                AccountAccessTokens.Add("ATd77918c0f7074a268d23da3304ea838f");
                AccountAccessTokens.Add("ATbec7b65c6885459aa2309e3e00c73b00");
            }
            else
            {
                AccountAccessTokens = accessTokenData;
            }
        }

        private void LoadAccountSecurityTokens()
        {
            var securityTokenData = Cache.Get("securityTokens") as Dictionary<string, string>;

            if (securityTokenData == null)
            {
                AccountSecurityTokens = new Dictionary<string, string>();
                AccountSecurityTokens.Add("ATd77918c0f7074a268d23da3304ea838f", "STd77918c0f7074a268d23da3304ea838f");
                AccountSecurityTokens.Add("ATbec7b65c6885459aa2309e3e00c73b00", "STbec7b65c6885459aa2309e3e00c73b00");
            }
            else
            {
                AccountSecurityTokens = securityTokenData;
            }
        }

    }
}