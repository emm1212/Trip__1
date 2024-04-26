using Microsoft.AspNetCore.Mvc;
using Trip.Models;
using Trip.DateBase;
using System.Text.RegularExpressions;

namespace Trip.Controllers
{
    public class AuthController : Controller
    {
        public static AccountsModel user;
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(AccountsModel account)
        {
            if (account. Name != "" && account.Password != "" && account.Password.Length > 5)
            {
                using Data Accounts = new Data();
                List<AccountsModel> ac = Accounts.accounts.Where(x => x.Name == account.Name).ToList();
                if (ac.Count != 0 && ac[0].Password == account.Password)
                {
                    user = ac[0];
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.msg = "Username or Password is Incorrect!";
            }
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(AccountsModel account)
        {
            ViewBag.msg = "";
            if (ModelState.IsValid)
            {
                using Data Accounts = new Data();
                if (Accounts.accounts.Where(x => x.Name == account.Name).ToList().Count == 0)
                {
                    AccountsModel ac = new AccountsModel()
                    {
                        Name = account.Name,
                        type = (Accounts.accounts.ToList().Count == 0) ? "admin" : "customer",
                        Email = account.Email,
                        Password = account.Password,
                        Phone = account.Phone,
                        Credit_Card_No = account.Credit_Card_No,
                        Credit_Card_Date = account.Credit_Card_Date,
                        Credit_Card_CVV = account.Credit_Card_CVV,
                    };
                    Accounts.Add(ac);
                    Accounts.SaveChanges();
                    return RedirectToAction("Login");
                }
                else ViewBag.msg = "this username is token";
            }
            return View();
        }

        public IActionResult Logout()
        {
            user = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
