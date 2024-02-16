using System.Data.SqlTypes;

namespace Project.Models
{
    public class JobsModel
    {
        private int id;
        private string jobtitle;
        private decimal minsalary;
        private decimal maxsalary;

        public int Id { get { return id; } set { id = value; } }
        public string JobTitle { get { return jobtitle; } set { jobtitle = value; } }
        public decimal Minsalary { get { return minsalary; } set { minsalary = value; } }
        public decimal Maxsalary { get { return maxsalary; } set { maxsalary = value; } }
    }
}
