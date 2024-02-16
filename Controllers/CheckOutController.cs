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
using System.Dynamic;
using Microsoft.AspNetCore.Http.Features;
using System.Web;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System.Web.SessionState;

namespace Project.Controllers
{
    public class CheckOutController : Controller
    {
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;
        List<PurchasesModel> cart;
        List<ProductsModel> items;
        static int idCounter = 1;

        public CheckOutController()
        {
            connection = new SqlConnection(connectionString);
            cart = new List<PurchasesModel>();
            items = new List<ProductsModel>();
        }

        public IActionResult Index()
        {
            
            if (TempData.ContainsKey("cart") && TempData.ContainsKey("items"))
            {
                cart = JsonConvert.DeserializeObject<List<PurchasesModel>>(TempData["cart"].ToString());
                items = JsonConvert.DeserializeObject<List<ProductsModel>>(TempData["items"].ToString());
            }
            var Cart = cart.Zip(items, (c, i) => new { cartItem = c, item = i });
            ViewBag.Cart = Cart;
            TempData["cart"] = JsonConvert.SerializeObject(cart);
            TempData["items"] = JsonConvert.SerializeObject(items);

            decimal totalCost = 0;
            foreach(var cartItem in cart)
            {
                totalCost += cartItem.Cost;
            }

            ViewBag.TotalCost = totalCost;

            return View(cart);
        }
        public IActionResult Create(PurchasesModel? purchase)
        {   
            return View(purchase);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(PurchasesModel cartItem)
        {
           
            ProductsModel product = GetProduct(cartItem.Product_id);
            if (cartItem.Quantity > product.Quantity)
            {
                ModelState.AddModelError("QuantityError", "Required Product Quantity Not Available");
                cartItem.Quantity = 0;
                return View("Create", cartItem);
            }
            if (TempData.ContainsKey("cart") && TempData.ContainsKey("items"))
            {
                cart = JsonConvert.DeserializeObject<List<PurchasesModel>>(TempData["cart"].ToString());
                items = JsonConvert.DeserializeObject<List<ProductsModel>>(TempData["items"].ToString());
            }

            
            int index = items.FindIndex(x => x.Name.Equals(product.Name));

            if (index>=0)
            {
                cart[index].Quantity += cartItem.Quantity;
                cart[index].Cost = cart[index].Quantity * product.Selling_Price;
            }
            else
            {
                cartItem.Id = idCounter++;
                cartItem.Cost = cartItem.Quantity * product.Selling_Price;
                items.Add(product);
                cart.Add(cartItem);
            }

            TempData["cart"] = JsonConvert.SerializeObject(cart);
            TempData["items"] = JsonConvert.SerializeObject(items);

            return Redirect("Index");
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
        public IActionResult DeleteItem(int? id)
        {
            if (TempData.ContainsKey("cart") && TempData.ContainsKey("items"))
            {
                cart = JsonConvert.DeserializeObject<List<PurchasesModel>>(TempData["cart"].ToString());
                items = JsonConvert.DeserializeObject<List<ProductsModel>>(TempData["items"].ToString());

                PurchasesModel purchasemodel = cart.Find(x => x.Id.Equals(id));
                if(purchasemodel != null)
                {
                    ProductsModel product = items.Find(y => y.Id.Equals(purchasemodel.Product_id));
                    if (product != null)
                    {
                        cart.Remove(purchasemodel);
                        items.Remove(product);                    
                    }
                }

                TempData["cart"] = JsonConvert.SerializeObject(cart);
                TempData["items"] = JsonConvert.SerializeObject(items);
            }

            return RedirectToAction("Index","Checkout");
        }
        public IActionResult ClearAllItems()
        {
            if (TempData.ContainsKey("cart") && TempData.ContainsKey("items"))
            {
                TempData.Remove("cart");
                TempData.Remove("items");
            }

            return Redirect("Index");
        }
        public IActionResult Edit(int? id)
        {   
            if(TempData.ContainsKey("cart"))
            {
                cart = JsonConvert.DeserializeObject<List<PurchasesModel>>(TempData["cart"].ToString());
            }
            PurchasesModel purchaseModel = cart.Find(x => x.Id.Equals(id));
            ProductsModel product = GetProduct(purchaseModel.Product_id);
            ViewBag.Product = product.Name.ToString();
            TempData["cart"] = JsonConvert.SerializeObject(cart);

            return View(purchaseModel);
        }
        public IActionResult EditPurchaseItem(PurchasesModel purchasesModel)
        {
            if (TempData.ContainsKey("cart") && TempData.ContainsKey("items"))
            {
                cart = JsonConvert.DeserializeObject<List<PurchasesModel>>(TempData["cart"].ToString());
                items = JsonConvert.DeserializeObject<List<ProductsModel>>(TempData["items"].ToString());
            }

            if (cart != null && items != null)
            {
                int index = cart.FindIndex(x => x.Id.Equals(purchasesModel.Id));
                cart[index].Quantity = purchasesModel.Quantity;
                cart[index].Cost = cart[index].Quantity * items[index].Selling_Price;
            }

            TempData["cart"] = JsonConvert.SerializeObject(cart);
            return RedirectToAction("Index", "Checkout");
        }
        public IActionResult Payment()
        {
            if (TempData.ContainsKey("cart") && TempData.ContainsKey("items"))
            {
                cart = JsonConvert.DeserializeObject<List<PurchasesModel>>(TempData["cart"].ToString());
                items = JsonConvert.DeserializeObject<List<ProductsModel>>(TempData["items"].ToString());
            }

            if (cart.Count == 0 || items.Count == 0)
                return RedirectToAction("Index", "Checkout");
            else
            {
                var Cart = cart.Zip(items, (c, i) => new { cartItem = c, item = i });
                ViewBag.Cart = Cart;
                
                decimal totalCost = 0;
                foreach (var cartItem in cart)
                {
                    totalCost += cartItem.Cost;
                }

                ViewBag.TotalCost = totalCost;

                TempData["cart"] = JsonConvert.SerializeObject(cart);
                TempData["items"] = JsonConvert.SerializeObject(items);

                return View("Payment");
            }
        }
        public IActionResult CheckOutItems(TransactionsModel transaction)
        {
            if (TempData.ContainsKey("cart") && TempData.ContainsKey("items"))
            {
                cart = JsonConvert.DeserializeObject<List<PurchasesModel>>(TempData["cart"].ToString());
                items = JsonConvert.DeserializeObject<List<ProductsModel>>(TempData["items"].ToString());
                TempData["cart"] = JsonConvert.SerializeObject(cart);
                TempData["items"] = JsonConvert.SerializeObject(items);
            }

            decimal totalCost = 0;
            foreach (var cartItem in cart)
            {
                totalCost += cartItem.Cost;
            }

            transaction.Cost = totalCost;
            transaction.Transaction_date = DateTime.Now;

            if (transaction.Payment < transaction.Cost)
            {
                ModelState.AddModelError("PaymentError", "Please pay requested ammount");
                return Redirect("Payment");
            }

            int userId;
            
            if(TempData.ContainsKey("SessionUser"))
            {
                userId = (int)TempData["SessionUser"];
                TempData["SessionUser"] = userId;
            }
            else
            {
                userId = 0;
            }

            string query = "INSERT INTO Transactions(Cost, Payment, User_id, TransactionDate) Values(@c,@p,@u,@d); SELECT SCOPE_IDENTITY()";
            

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@c", System.Data.SqlDbType.Decimal).Value = transaction.Cost;
            command.Parameters.Add("@p", System.Data.SqlDbType.Decimal).Value = transaction.Payment;
            command.Parameters.Add("@u", System.Data.SqlDbType.Int).Value = userId;
            command.Parameters.Add("@d", System.Data.SqlDbType.DateTime).Value = transaction.Transaction_date;

            //SqlCommand command2 = new SqlCommand(query2, connection);

            connection.Open();
            try
            {
                transaction.Id = Decimal.ToInt32((decimal)command.ExecuteScalar());
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            connection.Close();

            foreach(var cartItem in cart)
            {
                string query2 = "INSERT INTO Purchases Values(@pid,@q,@cost,@tid);";

                SqlCommand command2 = new SqlCommand(query2, connection);
                command2.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = cartItem.Product_id;
                command2.Parameters.Add("@q", System.Data.SqlDbType.Int).Value = cartItem.Quantity;
                command2.Parameters.Add("@cost", System.Data.SqlDbType.Decimal).Value = cartItem.Cost;
                command2.Parameters.Add("@tid", System.Data.SqlDbType.Int).Value = transaction.Id;

                connection.Open();
                try
                {
                    command2.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                connection.Close();
            }
            foreach (var cartItem in cart)
            {
                string query3 = "UPDATE PRODUCTS SET QUANTITY = QUANTITY - @q WHERE ID = @pid";

                SqlCommand command3 = new SqlCommand(query3, connection);
                command3.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = cartItem.Product_id;
                command3.Parameters.Add("@q", System.Data.SqlDbType.Int).Value = cartItem.Quantity;

                connection.Open();
                try
                {
                    command3.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                connection.Close();
            }

            return RedirectToAction("ClearAllItems", "Checkout");
        }
        public IActionResult Search(string? product_name)
        {   
            List<ProductsModel> products = new List<ProductsModel>();

            if(product_name is null)
                return View(products);

            string query = "Select * from Products where Name like'%'+@name+'%'";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = "%" + product_name + "%";


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
                }

                dataReader.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                return NotFound();
            }

            connection.Close();

            return View(products);
        }

        public IActionResult SearchRoute(int id)
        {
            return RedirectToAction("Create","Checkout",new PurchasesModel { Product_id = id });
        }
        
    }
}
