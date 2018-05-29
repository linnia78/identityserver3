using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace WebApiResource.Library
{
    public class AuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            switch (context.Action.First().Value)
            {
                case "site":
                    return Eval(context.Principal.HasClaim(x => x.Type == "siteApi" || x.Type == "client_siteApi"));
                default:
                    return Nok();
            }
        }

        private Task<bool> AuthorizeContactDetails(ResourceAuthorizationContext context)
        {
            switch (context.Action.First().Value)
            {
                default:
                    return Nok();
            }
        }
    }
}