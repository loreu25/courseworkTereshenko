using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystem2.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Department { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
