using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Company.Mody.PL.Helper;
using Company.Mody.PL.Helper.TwilioSms;
using Company.Mody.PL.Helper.MailKitHelper;
using Company.Mody.PL.Helper.Bitly;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Company.Mody.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMailService _mailService;
        private readonly ITwilioService _twilioService;
        private readonly IBitlyService _bitlyService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager
            ,SignInManager<AppUser> signInManager,
            IMailService mailService,
            ITwilioService twilioService,
            IBitlyService bitlyService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _twilioService = twilioService;
            _bitlyService = bitlyService;
            _mapper = mapper;
        }



        #region Sign Up
        [HttpGet]
        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user is null)
                {
                    user = await _userManager.FindByEmailAsync(model.Email);
                    if (user is null)
                    {
                        user = _mapper.Map<AppUser>(model);
                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            return RedirectToAction(nameof(SignIn));
                        }

                        //foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                        ViewData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Description;
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Email Exists!";
                    }
                }
                else
                {
                    ViewData["ErrorMessage"] = "User Name Exists!";

                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid password or no match. Try Again!";
            }
            return View(model);
        }


        #endregion


        #region Sign In


        [HttpGet]
        public IActionResult SignIn() => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(SigninViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var IsSignedIn = await _signInManager.PasswordSignInAsync(user, model.Password, model.RemeberMe, false);
                    if (IsSignedIn.Succeeded)
                        return RedirectToAction("Index", "Home");

                }

                ModelState.AddModelError("", "Invalid Login");
                ViewData["ErrorMessage"] = "Invalid Login!";
            }
            return View(model);
        }


        #endregion


        #region Sign Out
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }
        #endregion


        #region Reset Password


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPasswordSms()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendUrlToResetPasswordSms(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    // send Email then redirect to sent email view

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var url = Url.Action("ResetPassword", "Account", new { Email = model.Email, Token = token }, Request.Scheme);

                    var shortUrl = await _bitlyService.ShortenUrl(url);

                    if(shortUrl is not null)
                    {
                        var sms = new Sms()
                        {
                            //Body = $"Here is the page url to reset your password (Don't Share With anyone!): {url}",
                            Body = $"To Reset password, use {shortUrl}",
                            To = user.PhoneNumber
                        };

                        _twilioService.SendSms(sms);

                        // Redirect to check Your Phone view

                        return RedirectToAction(nameof(CheckPhone));
                    }

                    return RedirectToAction(nameof(SignIn));

                }
                ModelState.AddModelError("", "Invalid Reset Password");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CheckPhone()
        {
            return View();
        }



        [HttpGet]
        public IActionResult CheckInbox()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendUrlToResetPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    // send Email then redirect to sent email view

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var url = Url.Action("ResetPassword", "Account", new { Email = model.Email, Token = token }, Request.Scheme);

                    Email email = new Email()
                    {
                        To = model.Email,
                        Subject = "Reset Password for Company",
                        Body = url
                    };

                    // Send the email

                    //EmailSettings.SendEmail(email);
                    _mailService.SendEmail(email);


                    // Redirect to check Your inbox view

                    return RedirectToAction(nameof(CheckInbox));

                }
                ModelState.AddModelError("", "Invalid Reset Password");
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;

                var user = await _userManager.FindByEmailAsync(email);

                if (user is not null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(SignIn));
                    }
                }

                ModelState.AddModelError("", "Invalid Reset Passsword");


            }

            return View(model);
        }


        #endregion



        #region External Login (Google & Facebook)



        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ViewData["ErrorMessage"] = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(SignIn));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(SignIn));
            }

            // Try to sign in with this external login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // If user doesn't exist, try to create a new one
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);

                // user is null => new user => make him sign up with the new view of external auth (no password)
                if (user == null)
                {
                    var externalUser = new ExternalAuthUser { 
                        Email = email,
                        FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                        LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                        Provider = info.LoginProvider
                    };
                    return  RedirectToAction("SignupExternalAuth", externalUser);
                }
                // external user auth found before
                else
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        return RedirectToAction(nameof(SignIn));
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                
            }

            return RedirectToAction(nameof(SignIn));
        }


        [HttpGet]
        public IActionResult SignupExternalAuth(ExternalAuthUser model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignupExternalAuthPost(ExternalAuthUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user is null)
                {
                    user = _mapper.Map<AppUser>(model);
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(SignIn));
                    }

                    //foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                    TempData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Description;
                }
                else
                {
                    model.UserName = "";
                    TempData["ErrorMessage"] = "User Name Exists!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid SignUp, Try Again!";
            }
            // if validation failed => back to SignupExternalAuth View with the errors
            return RedirectToAction("SignupExternalAuth", model);
        }


        #endregion



        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
