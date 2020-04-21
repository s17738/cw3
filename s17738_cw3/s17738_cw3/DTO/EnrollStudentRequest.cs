using System;
using System.ComponentModel.DataAnnotations;

namespace s17738_cw3.DTO
{
    public class EnrollStudentRequest
    {
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }

    }
}
