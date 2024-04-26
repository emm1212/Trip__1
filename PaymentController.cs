using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Trip.DateBase;
using Trip.Models;

namespace Trip.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Payment()
        {
            using Data data = new Data();
            List<BasketModel> basket = data.basket.Where(x => x.AccountId == AuthController.user.Id).ToList();
            List<ProductsModel> products = new List<ProductsModel>();
            if(basket.Count > 0)
            {
                foreach (BasketModel basketItem in basket)
                {
                    ProductsModel product = data.products.FirstOrDefault(x => x.Id == basketItem.ProductId);

                    if (product != null)
                    {

                        product = new ProductsModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Img = product.Img,
                            Amount = basketItem.Amount,
                            Price = product.Price,
                            Discription = product.Discription,
                            CategoryId = product.CategoryId
                        };

                        products.Add(product);
                    }
                }
                int sum = 0;
                foreach(ProductsModel pro in products)
                {
                    sum += Convert.ToInt32(Regex.Replace(pro.Price, @"\D", "")) * pro.Amount;
                }
                ViewBag.Sum = $"{sum} R.Y.";
                return View();
            }
            else
            {
                TempData["message"] = "add some products to basket first!";
                AdminController.count = 1;
                return RedirectToAction("Basket");
            }
        }
        public IActionResult Basket()
        {
            using Data data = new Data();
            List<BasketModel> basket = data.basket.Where(x => x.AccountId == AuthController.user.Id).ToList();
            List<ProductsModel> products = new List<ProductsModel>();
            if(basket.Count > 0)
            {


                foreach (BasketModel basketItem in basket)
                {
                    ProductsModel product = data.products.FirstOrDefault(x => x.Id == basketItem.ProductId);

                    if (product != null)
                    {
                        product = new ProductsModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Img = product.Img,
                            Amount = basketItem.Amount,
                            Price = product.Price,
                            Discription = product.Discription,
                            CategoryId = product.CategoryId
                        };

                        products.Add(product);
                    }
                }

            }
            ViewBag.products = products;
            if (AdminController.count == 1) AdminController.msg = (string)TempData["message"];
            TempData["message"] = AdminController.msg;
            if (TempData.ContainsKey("message")) { ViewBag.message = TempData["message"]; }
            else { ViewBag.message = ""; }
            return View();
        }

        [HttpPost]
        public IActionResult ChangeAmount(string sign, ProductsModel products)
        {
            using Data data = new Data();
            if(sign[0] == '+')
            {
                data.basket.Where(x => x.ProductId == Convert.ToInt32(sign.Substring(1)) && x.AccountId == AuthController.user.Id).First().Amount += 1;
                data.SaveChanges();
            }
            else
            {
                data.basket.Where(x => x.ProductId == Convert.ToInt32(sign.Substring(1)) && x.AccountId == AuthController.user.Id).First().Amount  -= 1;
                if (data.basket.Where(x => x.ProductId == Convert.ToInt32(sign.Substring(1)) && x.AccountId == AuthController.user.Id).First().Amount != 0)
                    data.SaveChanges();
            }
            return RedirectToAction("Basket");
        }

        [HttpPost]
        public IActionResult DeleteBasket(string del, ProductsModel products)
        {
            using Data data = new Data();
            data.basket.Remove(data.basket.Where(x => x.ProductId == Convert.ToInt32(del) && x.AccountId == AuthController.user.Id).First());
            data.SaveChanges();
            return RedirectToAction("Basket");
        }

        [HttpPost]
        public IActionResult Pay()
        {
            using Data data = new Data();
            List<BasketModel> baskets = data.basket.Where(x => x.AccountId == AuthController.user.Id).ToList();
            int num = 0;
            foreach(BasketModel bas in baskets)
            {
                num = data.products.Where(x => x.Id == bas.ProductId).First().Amount;
                num -= bas.Amount;
                if (num == 0) data.products.Remove(data.products.Where(x => x.Id == bas.ProductId).First());
                else data.products.Where(x => x.Id == bas.ProductId).First().Amount -= bas.Amount;
            }
            data.basket.RemoveRange(data.basket.Where(x => x.AccountId == AuthController.user.Id).ToList());
            data.SaveChanges();
            TempData["message"] = $"Thank you for Buying Your Recipe number is: {new Random().Next(100000000, 999999999)}";
            AdminController.count = 1;
            return RedirectToAction("Basket");
        }
    }
}
