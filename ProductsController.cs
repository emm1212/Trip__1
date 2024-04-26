using Microsoft.AspNetCore.Mvc;
using Trip.DateBase;
using Trip.Models;
using System;
namespace Trip.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Products()
        {
            using Data data = new Data();
            ViewBag.prod = data.products.ToList();
            if (AdminController.count == 1) AdminController.msg = (string)TempData["message"];
            TempData["message"] = AdminController.msg;
            if (TempData.ContainsKey("message")) { ViewBag.Message = TempData["message"]; }
            else { ViewBag.Message = ""; }
            return View();
        }
        public IActionResult Category()
        {
            using Data data = new Data();
            ViewBag.cat = data.category.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AddToBasket(string input, ProductsModel products)
        {
            if (AuthController.user != null)
            {
                using Data data = new Data();
                int id = data.products.Where(x => x.Name == products.Name && x.Price == products.Price && x.Discription == products.Discription).ToList()[0].Id;
                data.basket.Add(new BasketModel { Amount = products.Amount, AccountId = AuthController.user.Id, ProductId = id });
                data.SaveChanges();
                TempData["message"] = "The Product is Added Successfully";
                AdminController.count = 1;
                if(input == "Add to Basket")
                    return RedirectToAction("Products");
                else
                    return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public IActionResult Products(string btn)
        {
            using Data data = new Data();
            try {
                int.Parse(btn);
                ViewBag.prod = data.products.Where(x=> x.CategoryId == Convert.ToInt32(btn)).ToList();
            }
            catch
            {
                ViewBag.prod = data.products.Where(x=> x.Name.Contains(btn)).ToList();
            }
            return View();
        }
    }
}
