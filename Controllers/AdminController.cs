using Microsoft.AspNetCore.Mvc;
using Project.Models;
using System.Data.SqlClient;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;

        public AdminController()
        {
            connection = new SqlConnection(connectionString);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VerifyAdminPin(AdminPinModel adminPin)
        {

            string qurey = "Select Pin from AdminPins where Pin=@pin";

            SqlCommand command = new SqlCommand(qurey,connection);
            command.Parameters.Add("@pin", System.Data.SqlDbType.VarChar, 8).Value = adminPin.Pin;

            string action = TempData["action"].ToString();
            string controller = TempData["controller"].ToString();
            TempData["action"] = action;
            TempData["controller"] = controller;

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    return RedirectToAction(action, controller);
                }


            }
            catch (Exception e)
            {
                ModelState.AddModelError("PinVerification", "Admin Pin not Verified");
                return View("Index");


                //if (controller == "AdminPin")
                    //controller = "Home";

                //return RedirectToAction("Index",controller);
            }

            ModelState.AddModelError("PinVerification", "Admin Pin not Verified");
            return View("Index");

            //if (controller == "AdminPin")
            //    controller = "Home";

            //return RedirectToAction("Index", controller);


        }
        
        public IActionResult VerifyPinFromNav()
        {
            TempData["action"] = "Index";
            TempData["controller"] = "AdminPin";
            return Redirect("Index");
        }
    }
}
