using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;
using System.Globalization;
using Shared;

namespace MvcClient.Library.Utility
{
    public static class HttpUtility
    {
        public static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.SetBearerToken(GetAccessToken());
            //_httpClient.BaseAddress = new Uri(IdentityServerSetting.WEB_API_RESOURCE_URL);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public static string GetAccessToken()
        {
            var currentClaimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            var expiresAtFromClaims =
                  DateTime.Parse(currentClaimsIdentity.FindFirst("expires_at").Value,
                  null, DateTimeStyles.RoundtripKind);

            // check if the access token hasn't expired.
            if (DateTime.Now.ToUniversalTime() <
                 expiresAtFromClaims)
            {
                return currentClaimsIdentity.FindFirst("access_token").Value;
            }

            // expired.  Get a new one.
            var tokenEndpointClient = new TokenClient(
                IdentityServerSetting.IDENTITY_SERVER_TOKEN_URL,
                IdentityServerSetting.MVC_CLIENT_CLIENT_ID,
                IdentityServerSetting.MVC_CLIENT_CLIENT_SECRET);

            var requestRefreshTokenResponse =
                 tokenEndpointClient
                .RequestRefreshTokenAsync(currentClaimsIdentity.FindFirst("refresh_token").Value).Result;

            if (!requestRefreshTokenResponse.IsError)
            {
                // replace the claims with the new values - this means creating a 
                // new identity!                              
                var claims = currentClaimsIdentity.Claims
                    .Where(c => c.Type != "access_token" && c.Type != "refresh_token" &&
                        c.Type != "expires_at" && c.Type != "id_token")
                    .ToList();

                var expirationDateAsRoundtripString =
                    DateTime.SpecifyKind(DateTime.UtcNow.AddSeconds(requestRefreshTokenResponse.ExpiresIn),
                    DateTimeKind.Utc).ToString("o");

                claims.Add(new Claim("access_token", requestRefreshTokenResponse.AccessToken));
                claims.Add(new Claim("expires_at", expirationDateAsRoundtripString));
                claims.Add(new Claim("refresh_token", requestRefreshTokenResponse.RefreshToken));

                // we'll have a new claims identity after the request has been completed,
                // containing the new tokens
                var newIdentity = new ClaimsIdentity(claims,
                    "Cookies",
                    IdentityModel.JwtClaimTypes.Name,
                    IdentityModel.JwtClaimTypes.Role);

                HttpContext.Current.Request.GetOwinContext().Authentication.SignIn(newIdentity);

                // return the new access token
                return requestRefreshTokenResponse.AccessToken;
            }
            else
            {
                // if errors or expired refresh_token then logout user
                HttpContext.Current.Request.GetOwinContext().Authentication.SignOut();
                return "";
            }
        }

    }
}