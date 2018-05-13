using IdentityServer3.AccessTokenValidation;
using Owin;
using Shared;
using System.Web.Http;

namespace WebApiResource
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /*
                Project -> No Template / WebApi ref
                install-package Microsoft.Owin.Host.SystemWeb
                install-package Microsoft.AspNet.WebApi.Owin
                install-package IdentityServer3.AccessTokenValidation
            */
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = IdentityServerSetting.BASE_IDENTITY_SERVER_URL,
                RequiredScopes = new[] { "api" }
            });

            // web api configuration
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);
        }
    }
}