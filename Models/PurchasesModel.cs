using System.Data.SqlTypes;

namespace Project.Models
{
    public class PurchasesModel
    {
        private int id;
        private int product_id;
        private int quantity;
        private decimal cost;
        private int transaction_id;
        //private int customer_id;

        public int Id { get { return id; } set { id = value; } }
        public int Product_id { get { return product_id; } set { product_id = value; } }
        public int Quantity { get { return quantity; } set { quantity = value; } }
        public decimal Cost { get { return cost; } set { cost = value; } }
        //public int Customer_id { get { return customer_id; } set { customer_id = value; } }
        public int Transaction_id { get { return transaction_id; } set { transaction_id = value; } }
    }
}
