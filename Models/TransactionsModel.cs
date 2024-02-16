using System.Data.SqlTypes;

namespace Project.Models
{
    public class TransactionsModel
    {
        private int id;
        private decimal cost;
        private decimal payment;
        private int user_id;
        private DateTime transaction_date;

        public int Id { get { return id; } set { id = value; } }
        public decimal Cost { get { return cost; } set { cost = value; } }
        public decimal Payment { get { return payment; } set { payment = value; } }
        public int User_id { get { return user_id; } set { user_id = value; } }
        public DateTime Transaction_date { get { return transaction_date; } set { transaction_date = value; } }
    }
}
