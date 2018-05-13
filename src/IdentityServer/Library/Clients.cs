using IdentityServer3.Core.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer.Library
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "MVC Client",
                    ClientId = IdentityServerSetting.MVC_CLIENT_CLIENT_ID,
                    Flow = Flows.Hybrid,

                    RedirectUris = new List<string>
                    {
                        IdentityServerSetting.MVC_CLIENT_REDIRECT_URI
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        IdentityServerSetting.MVC_CLIENT_POST_REDIRECT_URI
                    },
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(IdentityServerSetting.MVC_CLIENT_CLIENT_SECRET.Sha256())
                    },
                    IdentityTokenLifetime = 10, //default 300
                    AccessTokenLifetime = 60, //default 3600
                    RequireConsent = false,
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "roles",
                        "api", //to allow access to the webapi
                        "offline_access", //refresh_tokens
                        "vendor"
                    }
                },
                new Client
                {
                    Enabled = true,
                    ClientName = "Web Api Services",
                    ClientId = IdentityServerSetting.WEB_API_CLIENT_ID,
                    Flow = Flows.ClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(IdentityServerSetting.WEB_API_CLIENT_SECRET.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "api"
                    }
                },
                new Client
                {
                    Enabled = true,
                    ClientName = "Angular Client",
                    ClientId = IdentityServerSetting.ANGULAR_CLIENT_ID,
                    Flow = Flows.Implicit,
                    RequireConsent = false,
                    IdentityTokenLifetime = 30, //default 300
                    AccessTokenLifetime = 60, //default 3600
                    RedirectUris = new List<string>
                    {
                        IdentityServerSetting.ANGULAR_CLIENT_REDIRECT_URI,
                        IdentityServerSetting.ANGULAR_CLIENT_SILENT_REFRESH_REDIRECT_URI
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        IdentityServerSetting.ANGULAR_CLIENT_POST_REDIRECT_URI
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:4200/"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "roles",
                        "api", //to allow access to the webapi
                        "vendor"
                    }
                }
            };
        }
    }
}