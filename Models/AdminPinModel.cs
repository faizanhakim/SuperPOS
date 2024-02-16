using Microsoft.Build.Framework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class AdminPinModel
    {
        
        private string pin;
        
        [Key]
        public string Pin { get { return pin; } set { pin = value; } }

    }
}
