using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            // System.IdentityModel.Tokens.Jwt > 5 - Breaking change! can't use
            //var token = new JwtSecurityTokenHandler()
            //    .ReadJwtToken((User.Identity as ClaimsIdentity).FindFirst("access_token").Value);
            // System.IdentityModel.Tokens.Jwt 4 > version < 5 --MVC
            //var accessToken = (User.Identity as ClaimsIdentity).FindFirst("access_token").Value;
            //var token = new JwtSecurityTokenHandler()
            //    .ReadToken(accessToken) as JwtSecurityToken;
            var token = new JwtSecurityTokenHandler()
                .ReadJwtToken((User.Identity as ClaimsIdentity).FindFirst("access_token").Value);
            var vendorid = token.Claims.First(c => c.Type == "vendorid").Value;

            return View((User as ClaimsPrincipal).Claims);
        }

        [ResourceAuthorize("Read", "ContactDetails")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        [Authorize]
        public ActionResult Login()
        {
            return Redirect("/");
        }
    }
}