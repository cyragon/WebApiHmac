using System;

namespace HmacAttribute
{
    public interface IDataAccess
    {
        string GetSecurityToken(string accessToken);
        bool VerifyAccessToken(string accessToken);
    }
}
