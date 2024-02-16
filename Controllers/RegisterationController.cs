using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    public class RegisterationController : Controller
    {
        public IActionResult Index()
        {
            return View("RegisterUser");
        }

        public IActionResult ProcessRegisteration(UserModel user)
        {
            UserAuthentication userAuthentication = new UserAuthentication();
            if (userAuthentication.MakeUser(user))
                return Redirect("https://localhost:44383/Login");
            else
                return View("RegisterUser");

        }

    }
}
