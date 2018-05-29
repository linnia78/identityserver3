﻿using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer.Library
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
            {
                new Scope
                {
                    Enabled = true,
                    Name = "roles",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                },
                new Scope
                {
                    Enabled = true,
                    DisplayName = "Web Api",
                    Name = "siteApi",
                    Description = "Access to web api",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role"),
                        new ScopeClaim("siteApi")
                    }
                },
                new Scope
                {
                    Enabled = true,
                    DisplayName = "Vendor Scope",
                    Name = "vendor",
                    Description = "Access to vendor info",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("vendorid")
                    }
                },
                new Scope
                {
                    Enabled = true,
                    DisplayName = "Site Abc WebForms Scope",
                    Name = "siteWebforms",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("userid"),
                        new ScopeClaim("siteWebforms")
                    }
                }
            };

            //scopes.AddRange(StandardScopes.All);
            scopes.Add(StandardScopes.OpenId);
            scopes.Add(StandardScopes.OfflineAccess);
            scopes.Add(StandardScopes.ProfileAlwaysInclude);
            scopes.Add(StandardScopes.Email);
            return scopes;
        }
    }
}