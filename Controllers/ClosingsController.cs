using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Project.Models;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Project.Controllers
{
    public class ClosingsController : Controller
    {
        string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;

        public ClosingsController()
        {
            connection = new SqlConnection(connectionString);
        }
        public IActionResult GetVerification()
        {
            TempData["action"] = "Index";
            TempData["controller"] = "Closings";
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Index()
        {
            List<ClosingModel> closings = new List<ClosingModel>();

            string query = "Select * From Closings";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        ClosingModel closing = new ClosingModel();
                        closing.Closeddate = (DateTime)dataReader["ClosingDate"];
                        closing.ClosingTime = closing.Closeddate.Add((TimeSpan)dataReader["ClosingTime"]);
                        closing.ItemsSold = (int)dataReader["ItemsSold"];
                        closing.Sales = (decimal)dataReader["Sales"];
                        closing.Profit = (decimal)dataReader["Profit"];

                        closings.Add(closing);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();


            return View(closings);
        }
        public IActionResult CloseDay()
        {
            ClosingModel closing = new ClosingModel();

            string query = "select sum(p.Quantity),sum(p.Cost),sum(pd.Profit_Margin * p.Quantity) " +
                "from Purchases p inner join products pd on p.Product_id = pd.Id inner join Transactions t on p.Transaction_id = t.Id " +
                "where CONVERT(VARCHAR(50), t.TransactionDate,120) like @date + '%';";
            
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@date", System.Data.SqlDbType.VarChar, 50).Value = "%" + GetDate(DateTime.Now) + "%";

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {   
                    dataReader.Read();
                    closing.Closeddate = DateTime.Now.Date;
                    closing.ClosingTime = DateTime.Now;
                    closing.ItemsSold = dataReader.GetInt32(0);
                    closing.Sales = dataReader.GetDecimal(1);
                    closing.Profit = dataReader.GetDecimal(2);
                }

                dataReader.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                return NotFound();
            }

            connection.Close();

            //Get Top Products
            string query2 = "select Top(3) p.Product_id, sum(p.Quantity) " +
                "from Purchases p inner join Transactions t on p.Transaction_id = t.Id " +
                "where CONVERT(VARCHAR(50), TransactionDate,120) like @date + '%' group by p.Product_id " +
                "order by sum(p.Quantity) desc;";

            SqlCommand command2 = new SqlCommand(query2, connection);
            command2.Parameters.Add("@date", System.Data.SqlDbType.VarChar, 50).Value = "%" + GetDate(DateTime.Now) + "%";
            
            try
            {
                connection.Open();
                SqlDataReader dataReader = command2.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while(dataReader.Read())
                    {
                        TopProductModel topProduct = new TopProductModel();
                        topProduct.Id = dataReader.GetInt32(0);
                        topProduct.SellingQuantity = dataReader.GetInt32(1);
                        closing.TopProducts.Add(topProduct);
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

            ProductsModel tempProduct = new ProductsModel();
            foreach(var product in closing.TopProducts)
            {
                tempProduct = GetProduct(product.Id);
                product.Name = tempProduct.Name;
                product.Selling_Price = tempProduct.Selling_Price;
                product.Sales = tempProduct.Selling_Price * product.SellingQuantity;
            }
            TempData["Closing"] = JsonConvert.SerializeObject(closing);

            return View(closing);
        }

        [HttpPost]
        public IActionResult SaveClosing()
        {
           
            if (!TempData.ContainsKey("Closing"))
                return Redirect("Index");

            ClosingModel closing = JsonConvert.DeserializeObject<ClosingModel>(TempData["Closing"].ToString());

            string query = "INSERT INTO Closings VALUES(@date,@time,@isold,@sales,@profit)";
            SqlCommand command = new SqlCommand(query,connection);
            command.Parameters.Add("@date",System.Data.SqlDbType.Date).Value = closing.Closeddate;
            command.Parameters.Add("@time",System.Data.SqlDbType.Time).Value = closing.ClosingTime.TimeOfDay;
            command.Parameters.Add("@isold",System.Data.SqlDbType.Int).Value = closing.ItemsSold;
            command.Parameters.Add("@sales",System.Data.SqlDbType.Decimal).Value = closing.Sales;
            command.Parameters.Add("@profit",System.Data.SqlDbType.Decimal).Value = closing.Profit;

            try
            {
                connection.Open();
                command.ExecuteScalar();
                connection.Close();
            }
            catch(Exception e)
            {
                connection.Close();
                return View("Error");
            }
            return RedirectToAction("Logout", "Login");
        }
        public IActionResult Details(DateTime date)
        {
            ClosingModel closing = GetClosing(date);

            //Get Top Products

            string query2 = "select Top(3) p.Product_id, sum(p.Quantity) " +
                "from Purchases p inner join Transactions t on p.Transaction_id = t.Id " +
                "where CONVERT(VARCHAR(50), t.TransactionDate,120) like @date + '%' group by p.Product_id " +
                "order by sum(p.Quantity) desc;";

            SqlCommand command2 = new SqlCommand(query2, connection);
            command2.Parameters.Add("@date", System.Data.SqlDbType.VarChar, 50).Value = "%" + GetDate(date) + "%";

            try
            {
                connection.Open();
                SqlDataReader dataReader = command2.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        TopProductModel topProduct = new TopProductModel();
                        topProduct.Id = dataReader.GetInt32(0);
                        topProduct.SellingQuantity = dataReader.GetInt32(1);
                        closing.TopProducts.Add(topProduct);
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

            ProductsModel tempProduct = new ProductsModel();
            foreach (var product in closing.TopProducts)
            {
                tempProduct = GetProduct(product.Id);
                product.Name = tempProduct.Name;
                product.Selling_Price = tempProduct.Selling_Price;
                product.Sales = tempProduct.Selling_Price * product.SellingQuantity;
            }
            TempData["Closing"] = JsonConvert.SerializeObject(closing);


            //Get All Transactions
            List<TransactionsModel> transactions = GetTransactions(date);
            ViewBag.Transactions = transactions;
          
            return View(closing);
        }
        private ClosingModel GetClosing(DateTime date)
        {
            string query = "Select * from Closings where CONVERT(VARCHAR(50), ClosingDate,120) like @date + '%'";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@date", System.Data.SqlDbType.VarChar, 50).Value = "%" + GetDate(date) + "%";

            ClosingModel closing = new ClosingModel();

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    closing.Closeddate = (DateTime)dataReader["ClosingDate"];
                    closing.ClosingTime = closing.Closeddate.Add((TimeSpan)dataReader["ClosingTime"]);
                    closing.ItemsSold = (int)dataReader["ItemsSold"];
                    closing.Sales = (decimal)dataReader["Sales"];
                    closing.Profit = (decimal)dataReader["Profit"];
                }

                dataReader.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                return null;
            }

            connection.Close();
            return closing;
        }
        private string GetDate(DateTime? date)
        {
            if (date is null)
                return "";
            string month = date.Value.Month.ToString();
            string day = date.Value.Day.ToString();

            if (month.Length == 1)
                month = month.PadLeft(2, '0');
            if (day.Length == 1)
                day = day.PadLeft(2, '0');

            return date.Value.Year.ToString() + '-' + month + '-' + day;
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
        private List<TransactionsModel> GetTransactions(DateTime date)
        {
            List<TransactionsModel> transactions = new List<TransactionsModel>();

            string query = "select * from Transactions " +
                "where CONVERT(VARCHAR(50), TransactionDate,120) like @date + '%';";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@date", System.Data.SqlDbType.VarChar, 50).Value = "%" + GetDate(date) + "%";

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        TransactionsModel transaction = new TransactionsModel();
                        transaction.Id = (int)dataReader["Id"];
                        transaction.Cost = (decimal)dataReader["Cost"];
                        transaction.Payment = (decimal)dataReader["Payment"];

                        if (dataReader["User_id"].Equals(DBNull.Value))
                            transaction.User_id = 0;
                        else
                            transaction.User_id = (int)dataReader["User_id"];
                        transaction.Transaction_date = (DateTime)dataReader["TransactionDate"];

                        transactions.Add(transaction);
                    }
                }

                dataReader.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                return null;
            }

            connection.Close();

            return transactions;
        }


    }
}
