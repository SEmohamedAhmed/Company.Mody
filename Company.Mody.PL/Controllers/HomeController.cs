using System.Diagnostics;
using System.Text;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.Profile;
using Company.Mody.PL.Models;
using Company.Mody.PL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IScopedService scopedService01;
        private readonly IScopedService scopedService02;
        private readonly ITransientService transientService01;
        private readonly ITransientService transientService02;
        private readonly ISingletonService singletonService01;
        private readonly ISingletonService singletonService02;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
                                IScopedService scopedService01,
                                IScopedService scopedService02,
                                ITransientService transientService01,
                                ITransientService transientService02,
                                ISingletonService singletonService01,
                                ISingletonService singletonService02,
                                UserManager<AppUser> userManager)
        {
            _logger = logger;
            this.scopedService01 = scopedService01;
            this.scopedService02 = scopedService02;
            this.transientService01 = transientService01;
            this.transientService02 = transientService02;
            this.singletonService01 = singletonService01;
            this.singletonService02 = singletonService02;
            _userManager = userManager;
        }

        public string TestService()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"scopedService01: {scopedService01.Guid}\n");
            sb.Append($"scopedService02: {scopedService02.Guid}\n\n");
            sb.Append($"transientService01: {transientService01.Guid}\n");
            sb.Append($"transientService02: {transientService02.Guid}\n\n");
            sb.Append($"singletonService01: {singletonService01.Guid}\n");
            sb.Append($"singletonService02: {singletonService02.Guid}");
            return sb.ToString();
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            ProfileUser profileUser = null;
            if (user != null)
            {
                profileUser = new ProfileUser (){
                    UserName= user.UserName,
                    Email=user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
            }
            return View(profileUser);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
