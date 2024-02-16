namespace Project.Models
{
    public class ValuedCustomersModel
    {
        private int id;
        private int customer_id;
        private int total_transaction;
        private int dicount_code;

        public int Id { get { return id; } set { id = value; } }
        public int Customer_id { get { return customer_id; } set { customer_id = value; } }
        public int Total_transaction { get { return total_transaction; } set { total_transaction = value; } }
        public int Dicount_code { get { return dicount_code; } set { dicount_code = value; } }

    }
}
