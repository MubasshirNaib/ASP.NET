using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpTaskAPI.Models
{
    public class Project
    {
        [Key]  // Explicitly defining the primary key
        [Column("ProjectId")]
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


    }

}
