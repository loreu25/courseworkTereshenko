using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystem2.Models
{
    public enum AttendanceStatus
    {
        Присутствует,
        Отсутствует
    }

    public class Attendance
    {
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        [Required]
        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule Schedule { get; set; }
        [Required]
        public AttendanceStatus Status { get; set; }
    }
}
