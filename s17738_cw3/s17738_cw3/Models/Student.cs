﻿
using System.ComponentModel.DataAnnotations;

namespace s17738_cw3.Models
{
    public class Student
    {
        public string IndexNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        public string Role { get; set; }
    }
}
