using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Webforms
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /*
              Project -> Mvc template
             
              install-package Microsoft.Owin.Host.Systemweb
              install-package Microsoft.Owin.Security.Cookies
              install-package Microsoft.Owin.Security.OpenIdConnect
              install-package IdentityServer3
              

              To communicate with OAuth2 token endpoint
              install-package IdentityModel
            */

            //Remove default dot net claim types
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                ExpireTimeSpan = new TimeSpan(0, 30, 0),
                SlidingExpiration = true
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = IdentityServerSetting.BASE_IDENTITY_SERVER_URL,
                ClientId = IdentityServerSetting.WEBFORM_CLIENT_ID,
                RedirectUri = IdentityServerSetting.WEBFORM_REDIRECT_URI,
                ResponseType = "code id_token token",
                PostLogoutRedirectUri = IdentityServerSetting.WEBFORM_POST_REDIRECT_URI,
                Scope = "openid profile roles siteWebforms offline_access vendor", //siteWebforms scope requested to access this webforms
                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false, //use lifetime of authentication ticket instead of checking expiration on id_token

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    //Claim transformations in notification to remove un-needed claims
                    SecurityTokenValidated = async (Microsoft.Owin.Security.Notifications.SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n) =>
                    {
                        var newClaimIdentity = new ClaimsIdentity(
                            n.AuthenticationTicket.Identity.AuthenticationType,
                            IdentityServer3.Core.Constants.ClaimTypes.GivenName,
                            IdentityServer3.Core.Constants.ClaimTypes.Role);

                        // get userinfo data
                        var userInfo = await new UserInfoClient(
                            n.Options.Authority + "/connect/userinfo")
                            .GetAsync(n.ProtocolMessage.AccessToken);

                        userInfo
                            .Claims
                            .ToList()
                            .ForEach(ui => newClaimIdentity.AddClaim(new Claim(ui.Type, ui.Value)));

                        // request a refresh token
                        var tokenClientForRefreshToken = new TokenClient(
                            IdentityServerSetting.IDENTITY_SERVER_TOKEN_URL,
                            IdentityServerSetting.WEBFORM_CLIENT_ID,
                            IdentityServerSetting.WEBFORM_CLIENT_SECRET);

                        var refreshResponse = await tokenClientForRefreshToken.RequestAuthorizationCodeAsync(n.ProtocolMessage.Code, IdentityServerSetting.WEBFORM_REDIRECT_URI);

                        var expirationDateAsRoundtripString
                            = DateTime.SpecifyKind(DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn)
                            , DateTimeKind.Utc).ToString("o");

                        // keep the id_token for logout
                        newClaimIdentity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        // add access token for web API
                        newClaimIdentity.AddClaim(new Claim("access_token", refreshResponse.AccessToken));

                        // keep the refresh token
                        newClaimIdentity.AddClaim(new Claim("refresh_token", refreshResponse.RefreshToken));

                        // keep track of access token expiration
                        newClaimIdentity.AddClaim(new Claim("expires_at", expirationDateAsRoundtripString));

                        // add some other app specific claim
                        newClaimIdentity.AddClaim(new Claim("app_specific", "some data"));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            newClaimIdentity,
                            n.AuthenticationTicket.Properties);
                    },

                    RedirectToIdentityProvider = (Microsoft.Owin.Security.Notifications.RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n) =>
                    {
                        //Add id_token for logout to prove a valid logged in user
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            });

            app.UseStageMarker(PipelineStage.Authenticate);
        }
    }
}