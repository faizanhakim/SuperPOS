using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
    public class ProjectContext : DbContext
    {
        public ProjectContext (DbContextOptions<ProjectContext> options)
            : base(options)
        {
        }

        public DbSet<Project.Models.EmployeesModel> EmployeesModel { get; set; } = default!;
        public DbSet<Project.Models.AdminPinModel> AdminPinModel { get; set; }
        public DbSet<Project.Models.CategoryModel> CategoryModel { get; set; }
        public DbSet<Project.Models.DepartmentsModel> DepartmentsModel { get; set; }
        public DbSet<Project.Models.UserModel> UserModel { get; set; }
        public DbSet<Project.Models.ProductsModel> ProductsModel { get; set; }
        public DbSet<Project.Models.PurchasesModel> PurchasesModel { get; set; }
        public DbSet<Project.Models.TransactionsModel> TransactionsModel { get; set; }
        public DbSet<Project.Models.JobsModel> JobsModel { get; set; }
        public DbSet<Project.Models.ClosingModel> ClosingModel { get; set; }
    }
}
