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
    public class CategoryController : Controller
    {
        private readonly ProjectContext _context;
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;


        public CategoryController(ProjectContext context)
        {
            _context = context;
            connection = new SqlConnection(connectionString);
        }

        public IActionResult GetVerification()
        {
            TempData["action"] = "Index";
            TempData["controller"] = "Category";
            return RedirectToAction("Index", "Admin");
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            List<CategoryModel> categories = new List<CategoryModel>();

            string query = "Select * From Category";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();


                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        CategoryModel category = new CategoryModel();
                        category.Id = (int)dataReader["Id"];
                        category.Name = dataReader["Name"].ToString();

                        categories.Add(category);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();
            //await _context.CategoryModel.ToListAsync()
            return View(categories);
        }

        private CategoryModel GetCategory(int? id)
        {
            CategoryModel category = new CategoryModel();

            string query = "Select * From Category where Id = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();



                if (dataReader.HasRows)
                {
                    dataReader.Read();

                    category.Id = (int)dataReader["Id"];
                    category.Name = dataReader["Name"].ToString();
                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return category;
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            CategoryModel category = GetCategory(id);

            //if (id == null || _context.CategoryModel == null)
            //{
            //    return NotFound();
            //}

            //var categoryModel = await _context.CategoryModel
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (categoryModel == null)
            //{
            //    return NotFound();
            //}

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AddCategory(CategoryModel category)
        {
            CreateCategory(category);
            return Redirect("Index");
        
        
        }


        private void CreateCategory(CategoryModel category)
        {
            string query = "INSERT INTO Category VALUES(@Name)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@Name", System.Data.SqlDbType.VarChar, 50).Value = category.Name;


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


        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryModel);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoryModel == null)
            {
                return NotFound();
            }

            CategoryModel category = new CategoryModel();
            category = GetCategory(id);

            return View(category);
        }

        public IActionResult UpdateCategory(CategoryModel category)
        {
            string query = "UPDATE Category Set Name=@name where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = category.Name;
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = category.Id;

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

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CategoryModel categoryModel)
        {
            if (id != categoryModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryModelExists(categoryModel.Id))
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
            return View(categoryModel);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoryModel == null)
            {
                return NotFound();
            }

            CategoryModel category = new CategoryModel();
            category = GetCategory(id);

            return View(category);
        }

        public IActionResult DeleteCategory(int id)
        {
       
            string query = "Delete from Category where Id = @id";

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

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoryModel == null)
            {
                return Problem("Entity set 'ProjectContext.CategoryModel'  is null.");
            }
            var categoryModel = await _context.CategoryModel.FindAsync(id);
            if (categoryModel != null)
            {
                _context.CategoryModel.Remove(categoryModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryModelExists(int id)
        {
          return _context.CategoryModel.Any(e => e.Id == id);
        }
    }
}
