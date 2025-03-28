using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.AppUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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



        #region SignIn
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
            }
            return View(model);
        }


        #endregion


        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }
    }
}
