using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using Thinktecture.IdentityModel.WebApi;

namespace WebApiResource.Controllers
{
    [ResourceAuthorize("site")]
    [EnableCors(origins: "https://localhost:4200", headers: "*", methods: "*")]
    public class WebApiResrouceController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            //var vendorid = (User.Identity as ClaimsIdentity).Claims.First(c => c.Type == "vendorid").Value;
            var user = User as ClaimsPrincipal;
            var claims = user.Claims
                .Select(c => new
                {
                    type = c.Type,
                    value = c.Value
                });
            return Json(claims);
        }
    }
}
