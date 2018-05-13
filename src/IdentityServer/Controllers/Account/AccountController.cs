using IdentityServer.AspIdentity;
using IdentityServer3.Core;
using Shared;
using System.IdentityModel.Claims;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Security.Claims;

namespace IdentityServer.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly UserManager _userManager;
        public AccountController(UserManager userManager)
        {
            this._userManager = userManager;
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(AccountCreateModel request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityServer.AspIdentity.User
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        UserName = request.FirstName
                    };

                    var result = await _userManager.CreateAsync(user, request.Password);
                    await _userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(Constants.ClaimTypes.Role, request.Role));
                    await _userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("vendorid", request.VendorId));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}