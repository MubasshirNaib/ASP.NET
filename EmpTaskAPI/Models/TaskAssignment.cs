using System.ComponentModel.DataAnnotations;

namespace EmpTaskAPI.Models
{
    public class TaskAssignment
    {

        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int EmployeeId { get; set; }

    }
}
