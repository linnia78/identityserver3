using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webforms
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                var pricipalClaims = (HttpContext.Current.User as System.Security.Claims.ClaimsPrincipal).Claims;
                var token = new JwtSecurityTokenHandler()
                    .ReadToken(pricipalClaims.First(x => x.Type == "access_token").Value) as JwtSecurityToken;
                var userClaims = token.Claims;
                if (userClaims.Any(x => x.Type == "siteWebforms"))
                {
                    //valid
                }
                else
                {
                    //invalid
                }
            }
        }
    }
}