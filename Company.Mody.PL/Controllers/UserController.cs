using System.Security.Claims;
using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.Employee;
using Company.Mody.PL.DTOs.User;
using Company.Mody.PL.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Company.Mody.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;

        }



        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
        {

            IEnumerable<UserViewModel> users = Enumerable.Empty<UserViewModel>();
            if(string.IsNullOrEmpty(keyword))
            {
                users = _userManager.Users.AsNoTracking().Select(u => new UserViewModel()
                {
                     Id = u.Id,
                     FirstName = u.FirstName,
                     LastName = u.LastName,
                     Email = u.Email,
                     Roles = _userManager.GetRolesAsync(u).Result
                }).ToList();

            }
            else
            {
                users =  _userManager.Users.AsNoTracking().Where(u=>u.Email
                                            .ToLower()
                                            .Contains(keyword.ToLower()))
                                            .Select(u => new UserViewModel()
                                            {
                                                Id = u.Id,
                                                FirstName = u.FirstName,
                                                LastName = u.LastName,
                                                Email = u.Email,
                                                Roles = _userManager.GetRolesAsync(u).Result
                                            }).ToList();

            }
                                                

            if (Request.Headers["X-Requested-UserSearch"] == "XMLHttpRequest") // if UserSearch Ajax Call
            {
                // Return the HTML fragment of the employee table for AJAX
                return PartialView("PartialViews/_userTable", users);
            }

            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null) return BadRequest(); // 400 status code

            var userDb = await _userManager.FindByIdAsync(id);
            if (userDb == null) return NotFound();

            var user = new UserViewModel()
            {
                Id = userDb.Id,
                FirstName = userDb.FirstName,
                LastName = userDb.LastName,
                Email = userDb.Email,
                Roles = _userManager.GetRolesAsync(userDb).Result
            };

            return View(viewName, user);
        }


        [HttpGet]
        // Get the update details view
        public async Task<IActionResult> Update(string? id)
        {
            return await Details(id, "Update");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Updates employee
        public async Task<IActionResult> Update([FromRoute] string? id, UserViewModel model)
        {


            if (string.IsNullOrEmpty(id)) return BadRequest();

            if (id != model.Id) return BadRequest();




            if (ModelState.IsValid) // checks data annotations validation
            {

                var userDb = await _userManager.FindByIdAsync(id);
                if (userDb == null) return NotFound();

                userDb.FirstName = model.FirstName;
                userDb.LastName = model.LastName;
                userDb.Email = model.Email;

                await _userManager.UpdateAsync(userDb);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }




        [HttpGet]
        // get deatils to delete employee
        public async Task<IActionResult> Delete(string? id)
        {
            return await Details(id, "Delete");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        // Delete employee
        public async Task<IActionResult> Delete([FromRoute] string? id, UserViewModel model)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            if (id != model.Id) return BadRequest();




            if (ModelState.IsValid) // checks data annotations validation
            {

                var userDb = await _userManager.FindByIdAsync(id);
                if (userDb == null) return NotFound();

                //userDb.FirstName = model.FirstName;
                //userDb.LastName = model.LastName;
                //userDb.Email = model.Email;

                await _userManager.DeleteAsync(userDb);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


    }
}
