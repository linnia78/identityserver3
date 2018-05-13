using IdentityServer.AspIdentity;
using IdentityServer3.Core;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentityServer.Controllers.Account
{
    public class AccountCreateController : Controller
    {
        private readonly UserManager _userManager;
        public AccountCreateController(UserManager userManager)
        {
            this._userManager = userManager;
        }
        // GET: AccountCreate
        [HttpGet]
        public ActionResult Index(string signin)
        {
            return View(new AccountCreateModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string signin, AccountCreateModel request)
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

                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "AccountCreate", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await _userManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                         + "before you can log in.";

                    return View("Info");
                    //return Redirect("~/identity/" + Constants.RoutePaths.Login + "?signin=" + signin);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            // If we got this far, something failed, redisplay form
            return View();
        }
    }
}