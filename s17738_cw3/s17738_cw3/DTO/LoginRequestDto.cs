using System.ComponentModel.DataAnnotations;

namespace s17738_cw3.DTO
{
    public class LoginRequestDto
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}