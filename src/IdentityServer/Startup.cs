using IdentityServer.AspIdentity;
using IdentityServer.Library;
using IdentityServer3.Core.Configuration;
using Owin;
using Serilog;
using Serilog.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNet.Identity.Owin;
using IdentityServer.Library.Service;
using Microsoft.Owin.Security.DataProtection;

namespace IdentityServer
{
    public class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(@"c:\logs\log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Debug()
                .CreateLogger();


            Log.Logger.Write(LogEventLevel.Debug, "test");

            /* Asp.Net Identity Configurations*/
            // https://stackoverflow.com/questions/42563647/how-to-register-applicationusermanager-with-identityserver-di-framework
            DataProtectionProvider = app.GetDataProtectionProvider();




            /*
              Project -> No Template / Mvc ref
              Add web.config runAllManagedModulesForAllRequests

              install-package Microsoft.Owin.Host.Systemweb
              install-package IdentityServer3
            */
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "IdentityServer",
                    EnableWelcomePage = false,
                    SigningCertificate = LoadCertificate(),
                    
                    Factory = ServiceFactory.Configure(),
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true,
                        LoginPageLinks = new List<LoginPageLink>()
                        {
                            new LoginPageLink()
                            {
                                Type = "createaccount",
                                Text = "Create a new account",
                                Href = "~/account"
                            },
                            new LoginPageLink()
                            {
                                Type = "forgotpassword",
                                Text = "Forgot password",
                                Href = "~/account/forgotpassword"
                            }
                        }
                    }
                });
            });
        }

        X509Certificate2 LoadCertificate()
        {
            //TODO : Change to load from windows certificate store
            //https://github.com/IdentityServer/IdentityServer3.Samples/tree/master/source/Certificates
            //return new X509Certificate2(
            //    string.Format(@"{0}\bin\library\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");

            return new X509Certificate2(
                string.Format(@"{0}\bin\library\toolots.pfx", AppDomain.CurrentDomain.BaseDirectory), "password123");

            //X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            //store.Open(OpenFlags.ReadOnly);
            //var certs = store.Certificates.Find(X509FindType.FindByThumbprint, "6B7ACC520305BFDB4F7252DAEB2177CC091FAAE1", true);
            //store.Close();
            //return certs[0];
        }
    }
}