using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Trip.DateBase;
using Trip.Models;

namespace Trip.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IWebHostEnvironment hostEnvironment, ILogger<HomeController> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        public static string WebRootPath;

        public IActionResult Index()
        {
            WebRootPath = this._hostEnvironment.WebRootPath;
            using Data data = new Data();
            ViewBag.products = data.products.ToList();
            ViewBag.categories = data.category.ToList();
            if (AdminController.count == 1) AdminController.msg = (string)TempData["message"];
            TempData["message"] = AdminController.msg;
            if (TempData.ContainsKey("message")) { ViewBag.Message = TempData["message"]; }
            else { ViewBag.Message = ""; }
            return View();
        }
        

        public IActionResult Contact()
        {
            if (AdminController.count == 1) AdminController.msg = (string)TempData["message"];
            TempData["message"] = AdminController.msg;
            if (TempData.ContainsKey("message")) { ViewBag.message = TempData["message"]; }
            else { ViewBag.message = ""; }
            return View();
        }

        [HttpPost]
        public IActionResult Send()
        {
            TempData["message"] = "TYour massege send successfully";
            AdminController.count = 1;
            return RedirectToAction("Contact");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}