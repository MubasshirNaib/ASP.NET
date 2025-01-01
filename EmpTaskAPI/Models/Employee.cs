using System.ComponentModel.DataAnnotations;

namespace EmpTaskAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        
        public string Email { get; set; }
        
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry {get; set;}

        [Required]
        public string Stack { get; set; }

        public string? Role { get; set; }
    }

}
