using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystem2.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
