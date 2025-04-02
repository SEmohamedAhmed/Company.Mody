using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Company.Mody.PL.Helper;

namespace Company.Mody.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager
            ,SignInManager<AppUser> signInManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                    var result = await _userManager.CheckPasswordAsync(user, model.Password);

                    if (result)
                    {
                        var IsSignedIn = await _signInManager.PasswordSignInAsync(user, model.Password, model.RemeberMe, false);
                        if (IsSignedIn.Succeeded)
                            return RedirectToAction("Index", "Home");
                    }

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

                    EmailSettings.SendEmail(email);

                    // Redirect to check Your inbox view

                    return RedirectToAction(nameof(CheckInbox));

                }
                ModelState.AddModelError("", "Invalid Reset Password");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult CheckInbox()
        {
            return View();
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


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
