using IdentityServer.AspIdentity;
using IdentityServer3.Core;
using Microsoft.AspNet.Identity;
using Shared;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IdentityServer.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly UserManager _userManager;
        public AccountController()
        {
            this._userManager = new UserManager(new UserStore(new AspIdentityDbContext(IdentityServerSetting.IDENTITY_SERVER_CONNECTION_STRING)));
        }

        // GET: AccountCreate
        [HttpGet]
        public ActionResult Index(string signin)
        {
            //var signinMessage = HttpContext.GetOwinContext().Environment.GetSignInMessage(signin);
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
                        UserName = request.Email
                    };

                    var result = await _userManager.CreateAsync(user, request.Password);
                    await _userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(IdentityServer3.Core.Constants.ClaimTypes.Role, request.Role));
                    await _userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("vendorid", request.VendorId));

                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code, signin = signin }, protocol: Request.Url.Scheme);
                    await _userManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed before you can log in.";

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

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code, string signin)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            ViewBag.signin = signin;
            var result = await _userManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //[HttpPost]
        //public async Task<ActionResult> Create(AccountCreateModel request)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var user = new IdentityServer.AspIdentity.User
        //            {
        //                FirstName = request.FirstName,
        //                LastName = request.LastName,
        //                Email = request.Email,
        //                UserName = request.Email
        //            };

        //            var result = await _userManager.CreateAsync(user, request.Password);
        //            await _userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(Constants.ClaimTypes.Role, request.Role));
        //            await _userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("vendorid", request.VendorId));
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return new HttpStatusCodeResult(HttpStatusCode.OK);
        //}

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string signin)
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model, string signin)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, signin = signin }, protocol: Request.Url.Scheme);
                await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string code, string signin)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model, string signin)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account", new { signin = signin });
            }
            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account", new { signin = signin });
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}