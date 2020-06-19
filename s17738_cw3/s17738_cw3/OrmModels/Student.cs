using System;
using System.Collections.Generic;

namespace s17738_cw3.OrmModels
{
    public partial class Student
    {
        public Student()
        {
            Token = new HashSet<Token>();
        }

        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int IdEnrollment { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string PasswordSalt { get; set; }

        public virtual Enrollment IdEnrollmentNavigation { get; set; }
        public virtual ICollection<Token> Token { get; set; }
    }
}
