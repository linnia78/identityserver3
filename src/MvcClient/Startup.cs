using IdentityModel.Client;
using IdentityServer3.Core;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
using System.Web.Helpers;

namespace MvcClient
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

              Add identityserver's custom authorize attribute
              install-package Thinktecture.IdentityModel.Owin.ResourceAuthorization.Mvc

              To communicate with OAuth2 token endpoint
              install-package IdentityModel
            */

            //Remove default dot net claim types
            AntiForgeryConfig.UniqueClaimTypeIdentifier = IdentityServer3.Core.Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseResourceAuthorization(new Library.AuthorizationManager());

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                ExpireTimeSpan = new TimeSpan(0, 30, 0),
                SlidingExpiration = true
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = IdentityServerSetting.BASE_IDENTITY_SERVER_URL,
                ClientId = IdentityServerSetting.MVC_CLIENT_CLIENT_ID,
                RedirectUri = IdentityServerSetting.MVC_CLIENT_REDIRECT_URI,
                ResponseType = "code id_token token",
                PostLogoutRedirectUri = IdentityServerSetting.MVC_CLIENT_POST_REDIRECT_URI,
                Scope = "openid profile roles siteApi offline_access vendor", //api scope requested to access webapi
                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false, //use lifetime of authentication ticket instead of checking expiration on id_token

                Notifications = new OpenIdConnectAuthenticationNotifications
                { 
                    //Claim transformations in notification to remove un-needed claims
                    SecurityTokenValidated = async (Microsoft.Owin.Security.Notifications.SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n) =>
                    {
                        //Commented out after adding "token" (access token) to ResponseType
                        ////var id = n.AuthenticationTicket.Identity;

                        ////// we want to keep first name, last name, subject and roles
                        ////var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                        ////var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                        ////var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        ////var roles = id.FindAll(Constants.ClaimTypes.Role);

                        ////// create new identity and set name and role claim type
                        ////var nid = new ClaimsIdentity(
                        ////    id.AuthenticationType,
                        ////    Constants.ClaimTypes.GivenName,
                        ////    Constants.ClaimTypes.Role);

                        ////nid.AddClaim(givenName);
                        ////nid.AddClaim(familyName);
                        ////nid.AddClaim(sub);
                        ////nid.AddClaims(roles);

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
                            IdentityServerSetting.MVC_CLIENT_CLIENT_ID,
                            IdentityServerSetting.MVC_CLIENT_CLIENT_SECRET);

                        var refreshResponse = await tokenClientForRefreshToken.RequestAuthorizationCodeAsync(n.ProtocolMessage.Code, IdentityServerSetting.MVC_CLIENT_REDIRECT_URI);

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
        }
    }
}