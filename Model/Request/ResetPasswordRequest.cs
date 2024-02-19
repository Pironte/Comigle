using System.ComponentModel.DataAnnotations;

namespace ComigleApi.Model.Request
{
    public class ResetPasswordRequest
    {
        [Required]
        public string? Token { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string? ConfirmPassword { get; set; }
    }
}
