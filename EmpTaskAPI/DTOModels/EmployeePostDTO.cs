using System.ComponentModel.DataAnnotations;

namespace EmpTaskAPI.DTOModels
{
    public class EmployeePostDTO
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Stack { get; set; }

        public string? Role { get; set; }
    }
}
