using IdentityServer.AspIdentity;
using IdentityServer3.AspNetIdentity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityServer.Library
{
    public static class UserServiceExtensions
    {
        public static void ConfigureUserService(this IdentityServerServiceFactory factory, string connString)
        {
            factory.UserService = new Registration<IUserService, UserService>();
            factory.Register(new Registration<UserManager>());
            factory.Register(new Registration<UserStore>());
            factory.Register(new Registration<AspIdentityDbContext>(resolver => new AspIdentityDbContext(connString)));
        }
    }

    public class UserService : AspNetIdentityUserService<User, string>
    {
        public UserService(UserManager userMgr)
            : base(userMgr)
        {
        }

        protected override async Task<IEnumerable<System.Security.Claims.Claim>> GetClaimsFromAccount(User user)
        {
            var claims = (await base.GetClaimsFromAccount(user)).ToList();
            if (!String.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new System.Security.Claims.Claim("given_name", user.FirstName));
            }
            if (!String.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new System.Security.Claims.Claim("family_name", user.LastName));
            }

            return claims;
        }
    }
}