using System.ComponentModel.DataAnnotations;

namespace UniversitySystem2.Models
{
    public enum UserRole
    {
        Администратор,
        Преподаватель,
        Студент
    }

    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public UserRole Role { get; set; }
    }
}
