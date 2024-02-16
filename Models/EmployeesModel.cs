using System.Data.SqlTypes;
using System.Runtime.CompilerServices;

namespace Project.Models
{
    public class EmployeesModel
    {
        private int id;
        private string firstname;
        private string lastname;
        private string email;
        private string address;
        private string phonenumber;
        private DateTime dob;
        private DateTime hiredate;
        private decimal salary;
        private int? job_id;
        private int? department_id;

        public int Id { get { return id; } set { id = value; } }
        public string Firstname { get { return firstname; } set { firstname = value; } }
        public string Lastname { get { return lastname; } set { lastname = value; } }
        public string Email { get { return email; } set { email = value; } }
        public string Address { get { return address; } set { address = value; } }
        public string Phonenumber { get { return phonenumber; } set { phonenumber = value; } }
        public DateTime Dob { get { return dob; } set { dob = value; } }
        public DateTime Hiredate { get { return hiredate; } set { hiredate = value; } }
        public decimal Salary { get { return salary; } set { salary = value; } }
        public int? Job_id { get { return job_id; } set { job_id = value; } }
        public int? Department_id { get { return department_id; } set { department_id = value; } }

    }
}
