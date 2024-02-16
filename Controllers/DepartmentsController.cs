using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using System.Data.SqlClient;
using Project.Models;

namespace Project.Controllers
{
    public class DepartmentsController : Controller
    {
       
        private readonly ProjectContext _context;
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;

        public DepartmentsController(ProjectContext context)
        {
            connection = new SqlConnection(connectionString);
            _context = context;
        }

        public IActionResult GetVerification()
        {
            TempData["action"] = "Index";
            TempData["controller"] = "Departments";
            return RedirectToAction("Index", "Admin");
        }
        // GET: Departments
        public async Task<IActionResult> Index()
        {
            List<DepartmentsModel> departments = new List<DepartmentsModel>();

            string query = "Select * From Departments";
            SqlCommand command = new SqlCommand(query, connection);


            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();


                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        DepartmentsModel department = new DepartmentsModel();
                        department.Id = (int)dataReader["Id"];
                        department.Name = dataReader["Name"].ToString();
                        
                        departments.Add(department);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();



            return View(departments);
        }

        private DepartmentsModel GetDepartment(int? id) 
        {
            string query = "select * from departments where id = @ID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = id;

            DepartmentsModel department = new DepartmentsModel();

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    dataReader.Read();

                    department.Id = (int)dataReader["Id"];
                    department.Name = dataReader["Name"].ToString();
                    
                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return department;
        }



        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            DepartmentsModel department = GetDepartment(id);
            

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {   
            return View();
        }

        public IActionResult AddDepartments(DepartmentsModel department)
        {
            CreateDepartments(department);
            return Redirect("Index");
        
        }

        private void CreateDepartments(DepartmentsModel department)
        {
            string query = "INSERT INTO Departments VALUES(@name)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = department.Name;


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

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] DepartmentsModel departmentsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(departmentsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentsModel);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DepartmentsModel == null)
            {
                return NotFound();
            }
            DepartmentsModel department = new DepartmentsModel();
            department = GetDepartment(id);
            return View(department);
            //var departmentsModel = await _context.DepartmentsModel.FindAsync(id);
            //if (departmentsModel == null)
            //{
            //    return NotFound();
            //}
        }

        public IActionResult UpdateDepartment(DepartmentsModel department)
        {
            string query = "UPDATE Departments Set Name=@name where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = department.Name;
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = department.Id;

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

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DepartmentsModel departmentsModel)
        {
            if (id != departmentsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departmentsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentsModelExists(departmentsModel.Id))
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
            return View(departmentsModel);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DepartmentsModel == null)
            {
                return NotFound();
            }

            DepartmentsModel department = new DepartmentsModel();
            department = GetDepartment(id);

            return View(department);
        }

        public IActionResult DeleteDepartment(int id)
        {
            if (id == null || _context.DepartmentsModel == null)
            {
                return NotFound();
            }

            string query = "Delete from departments where Id = @id";

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

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DepartmentsModel == null)
            {
                return Problem("Entity set 'ProjectContext.DepartmentsModel'  is null.");
            }
            var departmentsModel = await _context.DepartmentsModel.FindAsync(id);
            if (departmentsModel != null)
            {
                _context.DepartmentsModel.Remove(departmentsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentsModelExists(int id)
        {
          return _context.DepartmentsModel.Any(e => e.Id == id);
        }
    }
}
