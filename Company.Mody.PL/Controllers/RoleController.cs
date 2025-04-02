using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.Role;
using Company.Mody.PL.DTOs.User;
using Company.Mody.PL.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company.Mody.PL.Controllers
{
    [Authorize(Roles ="Admin")]

    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager,
                                UserManager<AppUser> userManager,
                                IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;

        }



        [HttpGet]
        public  IActionResult Index(string? keyword)
        {


            IEnumerable<RoleViewModel> roles = Enumerable.Empty<RoleViewModel>();
            if (string.IsNullOrEmpty(keyword))
            {
                roles = _roleManager.Roles.AsNoTracking().Select(r => new RoleViewModel()
                {
                    Id = r.Id,
                    Name = r.Name,
                }).ToList();

            }
            else
            {
                roles = _roleManager.Roles.AsNoTracking().Where(r => r.Name
                                            .ToLower()
                                            .Contains(keyword.ToLower()))
                                            .Select(r => new RoleViewModel()
                                            {
                                                Id = r.Id,
                                                Name = r.Name,
                                            }).ToList();

            }


            if (Request.Headers["X-Requested-RoleSearch"] == "XMLHttpRequest") // if RoleSearch Ajax Call
            {
                // Return the HTML fragment of the employee table for AJAX
                return PartialView("PartialViews/_roleTable", roles);
            }

            return View(roles);
        }




        [HttpGet]
        public  IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Add employee to the database
        public async Task<IActionResult> Add(RoleViewModel model)
        {
            if (ModelState.IsValid) 
            {
                var role = new IdentityRole()
                {
                    Name = model.Name,
                };
                var result = await _roleManager.CreateAsync(role);
                
                if(result.Succeeded) return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Invalid Role Creation");
            return View(model);
        }





        [HttpGet]
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
        {
            if (id is null) return BadRequest(); // 400 status code

            var roleDb = await _roleManager.FindByIdAsync(id);
            if (roleDb == null) return NotFound();

            var role = new RoleViewModel()
            {
                Id = roleDb.Id,
                Name = roleDb.Name,
            };

            return View(viewName, role);
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
        public async Task<IActionResult> Update([FromRoute] string? id, RoleViewModel model)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            if (id != model.Id) return BadRequest();


            if (ModelState.IsValid) // checks data annotations validation
            {

                var roleDb = await _roleManager.FindByIdAsync(id);
                if (roleDb == null) return NotFound();

                roleDb.Name = model.Name;


                await _roleManager.UpdateAsync(roleDb);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUser(string? roleId)
        {

            if (string.IsNullOrEmpty(roleId)) return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null) return NotFound();


            ViewData["RoleId"] = roleId;


            var users = await _userManager.Users.ToListAsync();
            var usersInRoleView = new List<AddRemoveRoleToUser>();


            foreach(var user in users)
            {
                var userInRoleView = new AddRemoveRoleToUser()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    HasRole = await _userManager.IsInRoleAsync(user, role.Name)
                };



                usersInRoleView.Add(userInRoleView);
            }

            return View(usersInRoleView); 
        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUser(string? roleId, List<AddRemoveRoleToUser> users)
        {
            if (string.IsNullOrEmpty(roleId)) return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            if (ModelState.IsValid)
            {
                foreach(var user in users)
                {
                    var userDb = await  _userManager.FindByIdAsync(user.Id);

                    if(userDb is not null)
                    {
                        if (user.HasRole && !await _userManager.IsInRoleAsync(userDb, role.Name))
                        {
                            await _userManager.AddToRoleAsync(userDb, role.Name);
                        }
                        else if (!user.HasRole && await _userManager.IsInRoleAsync(userDb, role.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(userDb, role.Name);
                        }
                    }
                    
                }
                return RedirectToAction(nameof(Update), new { id = roleId });
            }
            return View(users);
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
        public async Task<IActionResult> Delete([FromRoute] string? id, RoleViewModel model)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            if (id != model.Id) return BadRequest();




            if (ModelState.IsValid) // checks data annotations validation
            {

                var roleDb = await _roleManager.FindByIdAsync(id);
                if (roleDb == null) return NotFound();


                await _roleManager.DeleteAsync(roleDb);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
