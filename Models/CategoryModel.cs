using System;
namespace Project.Models
{
    public class CategoryModel
    {
        private int id;
        private string categoryname;

        public int Id { get { return id; } set { id = value; } }

        public string Name { get { return categoryname; } set { categoryname = value; } }
    }
}
