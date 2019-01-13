using System.ComponentModel.DataAnnotations;

namespace rockx.Models
{
    public class LoginViewModel
    {
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public bool success { get; set; } = true;
    }
}
