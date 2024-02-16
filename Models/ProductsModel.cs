using System.Data.SqlTypes;

namespace Project.Models
{
    public class ProductsModel
    {
        private int id;
        private string name;
        private int category_id;
        private int quantity;
        private DateTime entrydate;
        private int department_id;
        private decimal buying_price;
        private decimal selling_price;
        private decimal profit_margin;

        public int Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public int Category_id { get { return category_id; } set { category_id = value; } }
        public decimal Buying_Price { get { return buying_price; } set { buying_price = value; } }
        public decimal Selling_Price { get { return selling_price; } set { selling_price = value; } }
        public decimal Profit_Margin { get { return profit_margin; } set { profit_margin = value; } }
        public int Quantity { get { return quantity; } set { quantity = value; } }
        public DateTime Entrydate { get { return entrydate; } set { entrydate = value; } }
        public int Department_id { get { return department_id; } set { department_id = value; } }
    }
}
