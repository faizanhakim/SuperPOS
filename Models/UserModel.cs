using Microsoft.Build.Framework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class UserModel
    {
        private int id;
        private string userName;
        private string password;
        private int employee_id;

        
        public int Id { get { return id; } set { id = value; } }
        
        [RegularExpression(@"^[a-zA-Z\s]*$")]
        //[Required]
        public string UserName { get { return userName; } set { userName = value; } }

        //[Required]
        public string Password { get { return password; } set { password = value; } }

        //[Required]
        public int Employee_Id { get { return employee_id; } set { employee_id = value; } }

    }
}
