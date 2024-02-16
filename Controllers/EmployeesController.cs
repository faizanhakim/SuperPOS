using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using System.Data.SqlClient;
using Project.Models;

namespace Project.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ProjectContext _context;

        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;

        public EmployeesController(ProjectContext context)
        {
            _context = context;
            connection = new SqlConnection(connectionString);
        }

        public IActionResult GetVerification()
        {
            TempData["action"] = "Index";
            TempData["controller"] = "Employees";
            return RedirectToAction("Index","Admin");  
        }
        // GET: Employees
        public async Task<IActionResult> Index()
        {
            List<EmployeesModel> employees = new List<EmployeesModel>(); 

            string query = "Select * From Employees";
            SqlCommand command = new SqlCommand(query, connection);


            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        EmployeesModel employee = new EmployeesModel();
                        employee.Id = (int)dataReader["Id"];
                        employee.Firstname = dataReader["FirstName"].ToString();
                        employee.Lastname = dataReader["LastName"].ToString();
                        employee.Email = dataReader["Email"].ToString();
                        employee.Address = dataReader["Address"].ToString();
                        employee.Phonenumber = dataReader["PhoneNumber"].ToString();
                        employee.Dob = (DateTime)dataReader["DOB"];
                        employee.Hiredate = (DateTime)dataReader["HireDate"];
                        employee.Salary = (decimal)dataReader["Salary"];
                        employee.Department_id = (int)dataReader["Department_id"];
                        employee.Job_id = (int)dataReader["Job_id"];

                        employees.Add(employee);
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

            return View(employees);
        }

        private EmployeesModel GetEmployee(int? id)
        {
            EmployeesModel employee = new EmployeesModel();

            string query = "Select * From Employees where Id = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();



                if (dataReader.HasRows)
                {
                    dataReader.Read();

                    employee.Id = (int)dataReader["Id"];
                    employee.Firstname = dataReader["FirstName"].ToString();
                    employee.Lastname = dataReader["LastName"].ToString();
                    employee.Email = dataReader["Email"].ToString();
                    employee.Address = dataReader["Address"].ToString();
                    employee.Phonenumber = dataReader["PhoneNumber"].ToString();
                    employee.Dob = (DateTime)dataReader["DOB"];
                    employee.Hiredate = (DateTime)dataReader["HireDate"];
                    employee.Salary = (decimal)dataReader["Salary"];
                    employee.Department_id = (int)dataReader["Department_id"];
                    employee.Job_id = (int)dataReader["Job_id"];
                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return employee;
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            EmployeesModel employee = new EmployeesModel();

            employee = GetEmployee(id);
            
            if (id == null || _context.EmployeesModel == null)
            {
                return NotFound();
            }

            /*var employeesModel = await _context.EmployeesModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeesModel == null)
            {
                return NotFound();
            }*/

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult AuthenticateAdminCreate()
        {
            TempData["returnAction"] = "Create";
            TempData["returnContoller"] = "Employees";

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult AuthenticateAdminEdit(int? id)
        {
            TempData["action"] = "Edit";
            TempData["controller"] = "Employees";
            TempData["employee_id"] = id;

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult AuthenticateAdminDelete(int? id)
        {
            TempData["action"] = "Delete";
            TempData["controller"] = "Employees";
            TempData["employee_id"] = id;

            return RedirectToAction("Index", "Admin");
        }



        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AddEmployee(EmployeesModel employee)
        {
            CreateEmployee(employee);
            
            return Redirect("Index");
        }

        private void CreateEmployee(EmployeesModel employee)
        {
            string query = "INSERT INTO Employees VALUES(@fname,@lname,@email,@address,@pnum,@dob,@hd,@sal,@jid,@did)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@fname", System.Data.SqlDbType.VarChar, 50).Value = employee.Firstname;
            command.Parameters.Add("@lname", System.Data.SqlDbType.VarChar, 50).Value = employee.Lastname;
            command.Parameters.Add("@email", System.Data.SqlDbType.VarChar, 50).Value = employee.Email;
            command.Parameters.Add("@address", System.Data.SqlDbType.VarChar, 70).Value = employee.Address;
            command.Parameters.Add("@pnum", System.Data.SqlDbType.VarChar, 11).Value = employee.Phonenumber;
            command.Parameters.Add("@dob", System.Data.SqlDbType.Date).Value = employee.Dob;
            command.Parameters.Add("@hd", System.Data.SqlDbType.Date).Value = employee.Hiredate;
            command.Parameters.Add("@sal", System.Data.SqlDbType.Decimal).Value = employee.Salary;
            command.Parameters.Add("@jid", System.Data.SqlDbType.Int).Value = employee.Job_id;
            command.Parameters.Add("@did", System.Data.SqlDbType.Int).Value = employee.Department_id;
            
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


        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Firstname,Lastname,Email,Address,Phonenumber,Dob,Hiredate,Salary,Job_id,Department_id")] EmployeesModel employeesModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeesModel);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            /*int id = -1;

            if(TempData.ContainsKey("employee_id"))
                id = (int)TempData["employee_id"];*/

            EmployeesModel employee = new EmployeesModel();
            employee = GetEmployee(id);            
            return View(employee);
        }

        public IActionResult UpdateEmployee(EmployeesModel employee)
        {
            string query = "UPDATE Employees Set FirstName=@fname,LastName=@lname,Email=@email,Address=@address,PhoneNumber=@pnum,DOB=@dob,HireDate=@hd,Salary=@sal,Job_id=@jid,Department_id=@did where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@fname", System.Data.SqlDbType.VarChar, 50).Value = employee.Firstname;
            command.Parameters.Add("@lname", System.Data.SqlDbType.VarChar, 50).Value = employee.Lastname;
            command.Parameters.Add("@email", System.Data.SqlDbType.VarChar, 50).Value = employee.Email;
            command.Parameters.Add("@address", System.Data.SqlDbType.VarChar, 70).Value = employee.Address;
            command.Parameters.Add("@pnum", System.Data.SqlDbType.VarChar, 11).Value = employee.Phonenumber;
            command.Parameters.Add("@dob", System.Data.SqlDbType.Date).Value = employee.Dob;
            command.Parameters.Add("@hd", System.Data.SqlDbType.Date).Value = employee.Hiredate;
            command.Parameters.Add("@sal", System.Data.SqlDbType.Decimal).Value = employee.Salary;
            command.Parameters.Add("@jid", System.Data.SqlDbType.Int).Value = employee.Job_id;
            command.Parameters.Add("@did", System.Data.SqlDbType.Int).Value = employee.Department_id;
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = employee.Id;

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

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Firstname,Lastname,Email,Address,Phonenumber,Dob,Hiredate,Salary,Job_id,Department_id")] EmployeesModel employeesModel)
        {
            if (id != employeesModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesModelExists(employeesModel.Id))
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
            return View(employeesModel);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            /*int id = -1;

            if (TempData.ContainsKey("employee_id"))
                id = (int)TempData["employee_id"];*/


            EmployeesModel employee = new EmployeesModel();
            employee = GetEmployee(id);

            return View(employee);
        }

        public IActionResult DeleteEmployee(int id)
        {
            if (id == null || _context.EmployeesModel == null)
            {
                return NotFound();
            }

            string query = "Delete from employees where Id = @id";

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

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EmployeesModel == null)
            {
                return Problem("Entity set 'ProjectContext.EmployeesModel'  is null.");
            }
            var employeesModel = await _context.EmployeesModel.FindAsync(id);
            if (employeesModel != null)
            {
                _context.EmployeesModel.Remove(employeesModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeesModelExists(int id)
        {
          return _context.EmployeesModel.Any(e => e.Id == id);
        }
    }
}
