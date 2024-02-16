using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services;
using System.Net;
using Microsoft.AspNetCore.SignalR;


namespace FirstWeb.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProcessLogin(UserModel user)
        {   

            if(ModelState.IsValid)
            {
                UserAuthentication userAuthentication = new UserAuthentication();
                if (userAuthentication.VerifyUser(user))
                {
                    TempData["userName"] = user.UserName.ToString();
                    TempData["SessionUser"] = user.Id;
                    return RedirectToAction("Index","Home");
                }
                else
                {   
                    ModelState.AddModelError("LoginError","The UserName or Password is Incorrect");
                }
            }

             return View("Index", user);
        }

        public IActionResult Logout()
        {
            if (TempData.ContainsKey("userName"))
                TempData.Remove("userName");
            return RedirectToAction("Index","Login");
        }
    }
}
