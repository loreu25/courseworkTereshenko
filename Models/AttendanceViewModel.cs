using System.Collections.Generic;

namespace UniversitySystem2.Models
{
    public class StudentAttendanceRecord
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public AttendanceStatus Status { get; set; } // Enum: Присутствует или Отсутствует
    }
    
    public class GroupAttendanceViewModel
    {
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public List<StudentAttendanceRecord> StudentRecords { get; set; }
    }
}
