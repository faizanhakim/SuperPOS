using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Data.SqlClient;

namespace Project.Controllers
{
    public class JobsController : Controller
    {
        private readonly ProjectContext _context;
        private string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;


        public JobsController(ProjectContext context)
        {
            _context = context;
            connection = new SqlConnection(connectionString);
        }

        public IActionResult GetVerification()
        {
            TempData["action"] = "Index";
            TempData["controller"] = "Jobs";
            return RedirectToAction("Index", "Admin");
        }

        // GET: JobsModels
        public async Task<IActionResult> Index()
        {
            List<JobsModel> jobs = new List<JobsModel>();

            string query = "Select * From jobs";
            SqlCommand command = new SqlCommand(query, connection);


            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();


                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        JobsModel job = new JobsModel();
                        job.Id = (int)dataReader["Id"];
                        job.JobTitle = dataReader["JobTitle"].ToString();
                        job.Minsalary = (decimal)dataReader["MinSalary"];
                        job.Maxsalary = (decimal)dataReader["MaxSalary"];
                        

                        jobs.Add(job);
                    }

                    dataReader.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            //await _context.JobsModel.ToListAsync()

            return View(jobs);
        }

        private JobsModel GetJob(int? id)
        {
            JobsModel job = new JobsModel();

            string query = "Select * From Jobs where Id = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();



                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    job.Id = (int)dataReader["Id"];
                    job.JobTitle = dataReader["JobTitle"].ToString();
                    job.Minsalary = (decimal)dataReader["MinSalary"];
                    job.Maxsalary = (decimal)dataReader["MaxSalary"];

                }

                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return job;
        }

        // GET: JobsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobsModel == null)
            {
                return NotFound();
            }

            var jobsModel = GetJob(id);
            if (jobsModel == null)
            {
                return NotFound();
            }

            return View(jobsModel);
        }

        // GET: JobsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AddJob(JobsModel job)
        {
            CreateJob(job);

            return Redirect("Index");
        }

        private void CreateJob(JobsModel job)
        {
            string query = "INSERT INTO Jobs VALUES(@jtitle,@minsal,@maxsal)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@jtitle", System.Data.SqlDbType.VarChar, 50).Value = job.JobTitle;
            command.Parameters.Add("@minsal", System.Data.SqlDbType.Decimal).Value = job.Minsalary;
            command.Parameters.Add("@maxsal", System.Data.SqlDbType.Decimal).Value = job.Maxsalary;
            
            connection.Open();
            try
            {
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

        }

        // POST: JobsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobTitle,Minsalary,Maxsalary")] JobsModel jobsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobsModel);
        }

        // GET: JobsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobsModel == null)
            {
                return NotFound();
            }

            JobsModel job = new JobsModel();
            job = GetJob(id);
            return View(job);
        }

        public IActionResult UpdateJob(JobsModel job)
        {
            string query = "UPDATE Jobs Set JobTitle=@jtitle, MinSalary=@minsal, MaxSalary=@maxsal where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@jtitle", System.Data.SqlDbType.VarChar, 50).Value = job.JobTitle;
            command.Parameters.Add("@minsal", System.Data.SqlDbType.Decimal).Value = job.Minsalary;
            command.Parameters.Add("@maxsal", System.Data.SqlDbType.Decimal).Value = job.Maxsalary;
            command.Parameters.Add("@id", System.Data.SqlDbType.Decimal).Value = job.Id;

            connection.Open();
            try
            {
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return Redirect("Index");
        }

        // POST: JobsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobTitle,Minsalary,Maxsalary")] JobsModel jobsModel)
        {
            if (id != jobsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobsModelExists(jobsModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jobsModel);
        }

        // GET: JobsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobsModel == null)
            {
                return NotFound();
            }

            JobsModel job = GetJob(id);
          
            return View(job);
        }
        public IActionResult DeleteJob(int id)
        {
            if (id == null || _context.JobsModel == null)
            {
                return NotFound();
            }

            string query = "Delete from Jobs where Id = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@id", System.Data.SqlDbType.VarChar, 50).Value = id;

            connection.Open();
            try
            {
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();


            return Redirect("Index");
        }

        // POST: JobsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobsModel == null)
            {
                return Problem("Entity set 'ProjectContext.JobsModel'  is null.");
            }
            var jobsModel = await _context.JobsModel.FindAsync(id);
            if (jobsModel != null)
            {
                _context.JobsModel.Remove(jobsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobsModelExists(int id)
        {
          return _context.JobsModel.Any(e => e.Id == id);
        }
    }
}
