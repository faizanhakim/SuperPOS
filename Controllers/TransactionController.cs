using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Project.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Project.Controllers
{
    public class TransactionController : Controller
    {
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;

        public TransactionController()
        {
            connection = new SqlConnection(connectionString);
        }
        
        public IActionResult Index()
        {

            List<TransactionsModel> transactions = new List<TransactionsModel>();

            string query = "Select * From Transactions";
            SqlCommand command = new SqlCommand(query, connection);

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
                        
                        if(dataReader["User_id"].Equals(DBNull.Value))
                            transaction.User_id = 0;
                        else    
                            transaction.User_id = (int)dataReader["User_id"];
                        transaction.Transaction_date = (DateTime)dataReader["TransactionDate"];

                        transactions.Add(transaction);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            connection.Close();

            return View(transactions);
        }

        private TransactionsModel? GetTransaction(int? id)
        {
            TransactionsModel transaction = null;
            

            string query = "select * from Transactions where id = @ID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = id;

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    transaction = new TransactionsModel();
                    dataReader.Read();
                    transaction.Id = (int)dataReader["Id"];
                    transaction.Cost = (decimal)dataReader["Cost"];
                    transaction.Payment = (decimal)dataReader["Payment"];

                    if (dataReader["User_id"] == null)
                        transaction.User_id = 0;
                    else
                        transaction.User_id = (int)dataReader["User_id"];

                    transaction.Transaction_date = (DateTime)dataReader["TransactionDate"];
                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return transaction;

        }

        private List<PurchasesModel> GetAllPurchases(int id)
        {
            List<PurchasesModel> purchases = new List<PurchasesModel>();

            string query = "Select * From Purchases where Transaction_id = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@id",System.Data.SqlDbType.Int).Value = id;
        
            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        PurchasesModel purchase = new PurchasesModel();

                        purchase.Id = (int)dataReader["Id"];
                        purchase.Product_id = (int)dataReader["Product_id"];
                        purchase.Quantity = (int)dataReader["Quantity"];
                        purchase.Cost = (decimal)dataReader["Cost"];
                        purchase.Transaction_id = (int)dataReader["Transaction_id"];

                        purchases.Add(purchase);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return purchases;
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

        public IActionResult Details(int? id)
        {
            TransactionsModel? transaction = GetTransaction(id);            
            if (transaction == null)
                return Redirect("Index");

            List<PurchasesModel> purchases = GetAllPurchases(transaction.Id);

            List<ProductsModel> products = new List<ProductsModel>();

            decimal totalCost = 0;
            foreach (var cartItem in purchases)
            {
                ProductsModel product = GetProduct(cartItem.Product_id);
                products.Add(product);
                //totalCost += cartItem.Cost;
            }

            var Cart = purchases.Zip(products, (c, i) => new { cartItem = c, item = i });
            
            ViewBag.Cart = Cart;
            //ViewBag.TotalCost = totalCost;
            ViewBag.Change = (decimal)(transaction.Payment - transaction.Cost);

            return View(transaction);    
        }

        public IActionResult Search(DateTime? date)
        {
            List<TransactionsModel> transactions = new List<TransactionsModel>();

            if (date is null)
                return View(transactions);

            string query = "select * from Transactions where CONVERT(VARCHAR(50), TransactionDate,120) like @date + '%';";
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
                return NotFound();
            }

            connection.Close();


            return View(transactions);
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

    }
}
