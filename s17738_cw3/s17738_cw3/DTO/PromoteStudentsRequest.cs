using System.ComponentModel.DataAnnotations;

namespace s17738_cw3.DTO
{
    public class PromoteStudentsRequest
    {
        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }

        [Required]
        [Range(minimum: 1, maximum: 20)]
        public int Semester { get; set; }
    }
}
