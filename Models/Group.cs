using System.ComponentModel.DataAnnotations;

namespace UniversitySystem2.Models
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
