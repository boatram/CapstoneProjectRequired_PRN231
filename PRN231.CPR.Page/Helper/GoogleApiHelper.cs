using BusinessObjects.DTOs.Request;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Response;

namespace PRN231.CPR.Page.Helper
{
    public class GoogleApiHelper
    {
        public static string ApplicationName = "PRN231";

        public static string ClientId = "1097283158973-qj9kbk5b3t1ku52qv330e6rve2kedunm.apps.googleusercontent.com";

        public static string ClientSecret = "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";

        public static string RedirectUri = "https://localhost:7024/StudentPages/HomePage";

        public static string OauthUri = "https://accounts.google.com/o/oauth2/v2/auth?";
        public static string TokenUri = "https://accounts.google.com/o/oauth2/token";

        public static string Scopes = " https://www.googleapis.com/auth/userinfo.email";
        public static string GetUserInfor = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=";

        public static string GetOauthUri(string extraParam) 
        {
            StringBuilder sbUri = new StringBuilder(OauthUri);
            sbUri.Append("client_id=" + ClientId);
            sbUri.Append("&redirect_uri=" + RedirectUri);
            sbUri.Append("&response_type=" + "code");
            sbUri.Append("&scope=" + Scopes);
            sbUri.Append("&access_type=" + "offline");
            sbUri.Append("&approval_prompt=" + "force");
            return sbUri.ToString();
        }
       
    }
    

}