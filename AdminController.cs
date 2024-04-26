using Microsoft.AspNetCore.Mvc;
using Trip.DateBase;
using Trip.Models;

namespace Trip.Controllers
{
    public class AdminController : Controller
    {
        public static int count = 0;
        public static string msg = "";

        public IActionResult AddProduct()
        {
            using Data Category = new Data();
            ViewBag.category = Category.category.ToList();
            if(count == 1) msg = (string)TempData["message"];
            TempData["message"] = msg;
            if (TempData.ContainsKey("message")) { ViewBag.message = TempData["message"]; }
            else{ ViewBag.message = ""; }
          
            return View();
        }
        
        [HttpPost]
        public IActionResult AddProduct(ProductsModel product)
        {
            if (ModelState.ErrorCount < 4)
            {
                using Data data = new Data();
                if (product.ImgF != null)
                {
                    string wwwpath = HomeController.WebRootPath;
                    string path = Path.Combine(wwwpath, "src");
                    string filename = product.ImgF.FileName;
                    string filedir = Path.Combine(path, filename);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = "src/" + filename;
                    if (!System.IO.File.Exists(filedir))
                    {
                        using (FileStream fs = new FileStream(filedir, FileMode.Create))
                        {
                            product.ImgF.CopyTo(fs);
                        }
                    }
                    product.Img = path;
                }
                product.categoryid = product.categoryid.Split('-')[1];
                product.CategoryId = data.category.Where(x => x.Name == product.categoryid).ToList()[0].Id;
                product.Category = null;
                data.products.Add(product);
                data.SaveChanges();
                TempData["message"] = "The Product is Added Successfully";
                count = 1;
                return RedirectToAction("AddProduct");
            }
            return View();
        }

        public IActionResult Accounts()
        {
            using Data accounts = new Data();
            ViewBag.Accounts = accounts.accounts.Where(x => x.type != "admin").ToList();
            return View();
        }
        [HttpPost]
        public IActionResult ChangeType(AccountsModel ac)
        {
            using Data accounts = new Data();
            var Type = accounts.accounts.Where(x => x.Id == ac.Id).ToList()[0];
            if (Type.type == "customer") Type.type = "employee";
            else Type.type = "customer";
            accounts.accounts.Update(Type);
            accounts.SaveChanges();
               
            return RedirectToAction("Accounts");

        }
        [HttpPost]
        public IActionResult Delete(AccountsModel ac)
        {
            using Data accounts = new Data();
            accounts.basket.RemoveRange(accounts.basket.Where(x => x.AccountId == ac.Id).ToList());
            accounts.accounts.Remove(accounts.accounts.Where(x => x.Id == ac.Id).ToList()[0]);
            accounts.SaveChanges();
            return RedirectToAction("Accounts");
        }

        public IActionResult AddCategory(ProductsModel model)
        {

            using Data data = new Data();
            data.category.Add(model.Category);
            data.SaveChanges();
           
            return RedirectToAction("AddProduct");
        }
        
    }
}
