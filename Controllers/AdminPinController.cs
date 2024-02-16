using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Project.Controllers
{
    public class AdminPinController : Controller
    {
        private readonly ProjectContext _context;
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;

        public AdminPinController(ProjectContext context)
        {
            connection = new SqlConnection(connectionString);
            _context = context;
        }

        // GET: AdminPin
        public async Task<IActionResult> Index()
        {
            List<AdminPinModel> adminPins = new List<AdminPinModel>();

            string query = "Select * From AdminPins";
            SqlCommand command = new SqlCommand(query, connection);


            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();


                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        AdminPinModel adminPin = new AdminPinModel();
                        adminPin.Pin = dataReader["Pin"].ToString();

                        adminPins.Add(adminPin);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            //await _context.EmployeesModel.ToListAsync()

            return View(adminPins);
        }

        private AdminPinModel GetAdminPin(string pin)
        {
            AdminPinModel adminPin = new AdminPinModel();

            string query = "Select * From AdminPins where Pin = @pin";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@pin", System.Data.SqlDbType.VarChar, 8).Value = pin;

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    dataReader.Read();

                    adminPin.Pin = dataReader["Pin"].ToString();
                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return adminPin;
        }


        // GET: AdminPin/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AddPin(AdminPinModel adminPin)
        {
            CreatePin(adminPin);

            return Redirect("Index");
        }

        private void CreatePin(AdminPinModel adminPin)
        {
            string query = "INSERT INTO AdminPins VALUES(@pin)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@pin", System.Data.SqlDbType.VarChar, 8).Value = adminPin.Pin;

            connection.Open();
            try
            {
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

        }

        // POST: AdminPin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Pin")] AdminPinModel adminPinModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminPinModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminPinModel);
        }

        // GET: AdminPin/Edit/5

        // GET: AdminPin/Delete/5
        public async Task<IActionResult> Delete(string pin)
        {
            AdminPinModel adminPin = GetAdminPin(pin);

            return View(adminPin);
        }

        public IActionResult DeletePin(string Pin)
        {

            string query = "Delete from AdminPins where pin = @pin";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@pin", System.Data.SqlDbType.VarChar, 8).Value = Pin;

            connection.Open();
            try
            {
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();


            return Redirect("Index");
        }

        // POST: AdminPin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.AdminPinModel == null)
            {
                return Problem("Entity set 'ProjectContext.AdminPinModel'  is null.");
            }
            var adminPinModel = await _context.AdminPinModel.FindAsync(id);
            if (adminPinModel != null)
            {
                _context.AdminPinModel.Remove(adminPinModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminPinModelExists(string id)
        {
          return _context.AdminPinModel.Any(e => e.Pin == id);
        }
    }
}
