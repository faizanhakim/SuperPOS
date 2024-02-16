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

namespace Project.Controllers
{
    public class UserController : Controller
    {
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;
        private readonly ProjectContext _context;

        public UserController(ProjectContext context)
        {
            connection = new SqlConnection(connectionString);
            _context = context;
        }

        public IActionResult GetVerification()
        {
            TempData["action"] = "Index";
            TempData["controller"] = "User";
            return RedirectToAction("Index", "Admin");
        }

        // GET: UserModels
        public async Task<IActionResult> Index()
        {

            List<UserModel> users = new List<UserModel>();

            string query = "Select * From Users";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();


                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        UserModel user = new UserModel();
                        user.Id = (int)dataReader["Id"];
                        user.UserName = dataReader["UserName"].ToString();
                        user.Password = dataReader["Password"].ToString();
                        if(dataReader["Employee_id"] is not System.DBNull)
                        user.Employee_Id = (int)dataReader["Employee_id"];
                        
                        users.Add(user);
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

            return View(users);
        }

        private UserModel GetUser(int? id)
        {
            UserModel user = new UserModel();

            string query = "Select * From Users where Id = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    user.Id = (int)dataReader["Id"];
                    user.UserName = dataReader["UserName"].ToString();
                    user.Password = dataReader["Password"].ToString();
                    if (dataReader["Employee_id"] is not System.DBNull)
                        user.Employee_Id = (int)dataReader["Employee_id"];
                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return user;
        }

        // GET: UserModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserModel == null)
            {
                return NotFound();
            }

            var user = GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: UserModels/Create

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AddUser(UserModel user)
        {
            CreateUser(user);

            return Redirect("Index");
        }

        private void CreateUser(UserModel user)
        {
            string query = "Insert into Users Values(@uname,@pass,@eid)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@uname", System.Data.SqlDbType.VarChar, 50).Value = user.UserName;
            command.Parameters.Add("@pass", System.Data.SqlDbType.VarChar, 50).Value = user.Password;
            command.Parameters.Add("@eid", System.Data.SqlDbType.Int).Value = user.Employee_Id;
            
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

        // POST: UserModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Password,Employee_Id")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userModel);
        }

        // GET: UserModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            UserModel user = new UserModel();
            user = GetUser(id);
            return View(user);
        }

        public IActionResult UpdateUser(UserModel user)
        {
            string query = "UPDATE Users Set UserName=@uname, Password=@pass, Employee_id=@eid where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@uname", System.Data.SqlDbType.VarChar, 50).Value = user.UserName;
            command.Parameters.Add("@pass", System.Data.SqlDbType.VarChar, 50).Value = user.Password;
            command.Parameters.Add("@eid", System.Data.SqlDbType.Int).Value = user.Employee_Id;
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = user.Id;
          
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

        // POST: UserModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Password,Employee_Id")] UserModel userModel)
        {
            if (id != userModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserModelExists(userModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userModel);
        }

        // GET: UserModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            UserModel user = new UserModel();
            user = GetUser(id);
            return View(user);
        }

        public IActionResult DeleteUser(int id)
        {
            if (id == null || _context.EmployeesModel == null)
            {
                return NotFound();
            }

            string query = "Delete from Users where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@id", System.Data.SqlDbType.VarChar, 50).Value = id;

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

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserModel == null)
            {
                return Problem("Entity set 'ProjectContext.UserModel'  is null.");
            }
            var userModel = await _context.UserModel.FindAsync(id);
            if (userModel != null)
            {
                _context.UserModel.Remove(userModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserModelExists(int id)
        {
          return _context.UserModel.Any(e => e.Id == id);
        }
    }
}
