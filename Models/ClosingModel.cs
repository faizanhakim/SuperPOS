using System.Data.SqlTypes;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Project.Models
{
    public class ClosingModel
    {
        
        private DateTime closingdate;
        private DateTime closingTime;
        private int itemsSold;
        private decimal sales;
        private decimal profit;
        private List<TopProductModel> topProducts = new List<TopProductModel>();

        [Key]
        public DateTime Closeddate { get { return closingdate; } set { closingdate = value; } }
        public DateTime ClosingTime { get { return closingTime; } set { closingTime = value; } }
        public int ItemsSold { get { return itemsSold; } set { itemsSold = value; } }
        public decimal Sales { get { return sales; } set { sales = value; } }
        public decimal Profit { get { return profit; } set { profit = value; } }
        public List<TopProductModel> TopProducts { get { return topProducts; } }

    }
}
