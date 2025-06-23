using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Index = Microsoft.EntityFrameworkCore.Metadata.Internal.Index;

namespace Management.Core.Entities
{

    public class Employee
    {
        [Key]
        public Guid EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        public string? Stack { get; set; }
        public string? Mobile { get; set; }
        [Required]
        [EmailAddress]  // Ensures valid email format
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? Salt { get; set; }
        public DateTime? JoiningDate { get; set; }
        public byte[]? Image { get; set; }
    }
}
