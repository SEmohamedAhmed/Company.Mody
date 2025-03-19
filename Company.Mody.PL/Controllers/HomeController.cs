using System.Diagnostics;
using System.Text;
using Company.Mody.PL.Models;
using Company.Mody.PL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IScopedService scopedService01;
        private readonly IScopedService scopedService02;
        private readonly ITransientService transientService01;
        private readonly ITransientService transientService02;
        private readonly ISingletonService singletonService01;
        private readonly ISingletonService singletonService02;

        public HomeController(ILogger<HomeController> logger,
                                IScopedService scopedService01,
                                IScopedService scopedService02,
                                ITransientService transientService01,
                                ITransientService transientService02,
                                ISingletonService singletonService01,
                                ISingletonService singletonService02)
        {
            _logger = logger;
            this.scopedService01 = scopedService01;
            this.scopedService02 = scopedService02;
            this.transientService01 = transientService01;
            this.transientService02 = transientService02;
            this.singletonService01 = singletonService01;
            this.singletonService02 = singletonService02;
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
