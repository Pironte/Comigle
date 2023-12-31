using System.ComponentModel.DataAnnotations;

namespace Comigle.Model.Dtos
{
    public class CreateUserDto
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
