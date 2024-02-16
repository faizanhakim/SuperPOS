namespace Project.Models
{
    public class TopProductModel:ProductsModel
    {
        private int sellingQuantity;
        private decimal sales;

        public int SellingQuantity { get; set; }
        public decimal Sales { get; set; }
    }
}
