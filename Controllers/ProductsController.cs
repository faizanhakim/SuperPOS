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
    public class ProductsController : Controller
    {
        private readonly ProjectContext _context;
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;
        public ProductsController(ProjectContext context)
        {
            connection = new SqlConnection(connectionString);
            _context = context;
        }

        // GET: ProductsModels
        public async Task<IActionResult> Index()
        {
            List<ProductsModel> products = new List<ProductsModel>();

            string query = "Select * From Products";
            SqlCommand command = new SqlCommand(query, connection);


            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();


                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        ProductsModel product = new ProductsModel();
                        product.Id = (int)dataReader["Id"];
                        product.Name = dataReader["Name"].ToString();
                        product.Category_id = (int)dataReader["Category_id"];
                        product.Buying_Price = (decimal)dataReader["Buying_Price"];
                        product.Selling_Price = (decimal)dataReader["Selling_Price"];
                        product.Profit_Margin = (decimal)dataReader["Profit_Margin"];
                        product.Quantity = (int)dataReader["Quantity"];
                        product.Entrydate = (DateTime)dataReader["EntryDate"];
                        product.Department_id = (int)dataReader["Department_id"];


                        products.Add(product);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();



            return View(products);
        }
        private ProductsModel GetProduct(int? id)
        {
            string query = "select * from products where id = @ID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = id;

            ProductsModel product = new ProductsModel();

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    dataReader.Read();

                    product.Id = (int)dataReader["Id"];
                    product.Name = dataReader["Name"].ToString();
                    product.Category_id = (int)dataReader["Category_id"];
                    product.Buying_Price = (decimal)dataReader["Buying_Price"];
                    product.Selling_Price = (decimal)dataReader["Selling_Price"];
                    product.Profit_Margin = (decimal)dataReader["Profit_Margin"];
                    product.Quantity = (int)dataReader["Quantity"];
                    product.Entrydate = (DateTime)dataReader["EntryDate"];
                    product.Department_id = (int)dataReader["Department_id"];

                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return product;
        }

        // GET: ProductsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ProductsModel product = GetProduct(id);

            return View(product);
        }

        // GET: ProductsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AddProducts(ProductsModel product)
        {
            CreateProducts(product);
            return Redirect("Index");

        }

        private void CreateProducts(ProductsModel product)
        {
            string query = "INSERT INTO Products VALUES(@name,@Category_id,@Buying_Price,@Selling_Price,@Profit_Margin,@Quantity,@EntryDate,@Department_id)";

            SqlCommand command = new SqlCommand(query, connection);

            //command.Parameters.Add("@id", System.Data.SqlDbType.Int, 50).Value = product.Id;
            command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = product.Name;
            command.Parameters.Add("@Category_id", System.Data.SqlDbType.Int, 50).Value = product.Category_id;
            command.Parameters.Add("@Buying_Price", System.Data.SqlDbType.Decimal, 50).Value = product.Buying_Price;
            command.Parameters.Add("@Selling_Price", System.Data.SqlDbType.Decimal, 50).Value = product.Selling_Price;
            command.Parameters.Add("@Profit_Margin ", System.Data.SqlDbType.Decimal, 50).Value = product.Selling_Price - product.Buying_Price;
            command.Parameters.Add("@Quantity", System.Data.SqlDbType.Int, 50).Value = product.Quantity;
            command.Parameters.Add("@EntryDate", System.Data.SqlDbType.DateTime, 50).Value = DateTime.Now;
            command.Parameters.Add("@Department_id", System.Data.SqlDbType.Int, 50).Value = product.Department_id;
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

        // POST: ProductsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category_id,Price,Quantity,Entrydate,Department_id")] ProductsModel productsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productsModel);
        }

        // GET: ProductsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmployeesModel == null)
            {
                return NotFound();
            }
            ProductsModel product = new ProductsModel();
            product = GetProduct(id);
            TempData["ProductEntryDate"] = product.Entrydate;
            return View(product);
        }

        public IActionResult UpdateProduct(ProductsModel product)
        {

            product.Entrydate = (DateTime)TempData["ProductEntryDate"];

            string query = "UPDATE Products Set Name=@name,Category_id=@cid,Buying_Price=@bp,Selling_Price=@sp,Profit_Margin=@pm,Quantity=@q,EntryDate=@ed,Department_id=@did where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = product.Name;
            command.Parameters.Add("@cid", System.Data.SqlDbType.Int).Value = product.Category_id;
            command.Parameters.Add("@bp", System.Data.SqlDbType.Decimal).Value = product.Buying_Price;
            command.Parameters.Add("@sp", System.Data.SqlDbType.Decimal).Value = product.Selling_Price;
            command.Parameters.Add("@pm", System.Data.SqlDbType.Decimal).Value = product.Selling_Price - product.Buying_Price;
            command.Parameters.Add("@q", System.Data.SqlDbType.Int).Value = product.Quantity;
            command.Parameters.Add("@ed", System.Data.SqlDbType.DateTime).Value = product.Entrydate;
            command.Parameters.Add("@did", System.Data.SqlDbType.Int).Value = product.Department_id;
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = product.Id;
            
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

        // GET: ProductsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductsModel == null)
            {
                return NotFound();
            }

            var productsModel = GetProduct(id);
            if (productsModel == null)
            {
                return NotFound();
            }

            return View(productsModel);
        }

        public IActionResult DeleteProduct(int id)
        {
            if (id == null || _context.EmployeesModel == null)
            {
                return NotFound();
            }

            string query = "Delete from Products where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

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


        // POST: ProductsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductsModel == null)
            {
                return Problem("Entity set 'ProjectContext.ProductsModel'  is null.");
            }
            var productsModel = await _context.ProductsModel.FindAsync(id);
            if (productsModel != null)
            {
                _context.ProductsModel.Remove(productsModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsModelExists(int id)
        {
            return _context.ProductsModel.Any(e => e.Id == id);
        }

        public IActionResult Search(string? product_name)
        {
            List<ProductsModel> products = new List<ProductsModel>();

            if (product_name is null)
                return View(products);
            
            string query = "Select * from Products where Name like'%'+@name+'%'";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = "%" + product_name + "%";


            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();
                if(dataReader.HasRows)
                {
                    while(dataReader.Read())
                    {
                        ProductsModel product  = new ProductsModel();
                        product.Id = (int)dataReader["Id"];
                        product.Name = dataReader["Name"].ToString();
                        product.Category_id = (int)dataReader["Category_id"];
                        product.Buying_Price = (decimal)dataReader["Buying_Price"];
                        product.Selling_Price = (decimal)dataReader["Selling_Price"];
                        product.Profit_Margin = (decimal)dataReader["Profit_Margin"];
                        product.Quantity = (int)dataReader["Quantity"];
                        product.Entrydate = (DateTime)dataReader["EntryDate"];
                        product.Department_id = (int)dataReader["Department_id"];

                        products.Add(product);
                    }
                }

                dataReader.Close();
            }
            catch(Exception e)
            {
                connection.Close();
                return NotFound();
            }

            connection.Close();
            
            return View(products);
        }
    }
}
