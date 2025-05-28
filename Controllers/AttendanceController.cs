using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem2.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace UniversitySystem2.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly UniversityContext _context;
        public AttendanceController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? scheduleId)
        {
            IQueryable<Attendance> query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Group)
                .Include(a => a.Schedule.Subject)
                .Include(a => a.Schedule.Teacher);
            
            // Фильтрация в зависимости от роли пользователя
            if (User.IsInRole("Студент"))
            {
                // Студенты видят только свои записи посещаемости
                string userLogin = User.Identity.Name;
                var student = await _context.Students
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.User.Login == userLogin);
                
                if (student != null)
                {
                    query = query.Where(a => a.StudentId == student.Id);
                }
            }
            
            if (scheduleId.HasValue)
            {
                query = query.Where(a => a.ScheduleId == scheduleId.Value);
            }
            
            var attendances = await query.ToListAsync();
            return View(attendances);
        }

        public IActionResult Create(int scheduleId)
        {
            var schedule = _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .FirstOrDefault(s => s.Id == scheduleId);
            ViewBag.Schedule = schedule;
            var groupId = schedule?.GroupId;
            ViewBag.Students = _context.Students.Where(s => s.GroupId == groupId).ToList();
            return View(new Attendance { ScheduleId = scheduleId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { scheduleId = attendance.ScheduleId });
            }
            var schedule = _context.Schedules.Find(attendance.ScheduleId);
            ViewBag.Schedule = schedule;
            var groupId = schedule?.GroupId;
            ViewBag.Students = _context.Students.Where(s => s.GroupId == groupId).ToList();
            return View(attendance);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var attendance = await _context.Attendances.Include(a => a.Schedule).FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null) return NotFound();
            ViewBag.Students = _context.Students.Where(s => s.GroupId == attendance.Schedule.GroupId).ToList();
            return View(attendance);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Attendances.Update(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { scheduleId = attendance.ScheduleId });
            }
            ViewBag.Students = _context.Students.Where(s => s.GroupId == attendance.Schedule.GroupId).ToList();
            return View(attendance);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var attendance = await _context.Attendances.Include(a => a.Student).Include(a => a.Schedule).FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null) return NotFound();
            return View(attendance);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { scheduleId = attendance?.ScheduleId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Group)
                .Include(a => a.Schedule.Subject)
                .Include(a => a.Schedule.Teacher)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null) return NotFound();
            return View(attendance);
        }
        
        // Групповая отметка посещаемости
        public async Task<IActionResult> MarkAttendance(int scheduleId)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);
                
            if (schedule == null) return NotFound();
            
            // Получаем студентов из группы
            var students = await _context.Students
                .Where(s => s.GroupId == schedule.GroupId)
                .OrderBy(s => s.FullName)
                .ToListAsync();
                
            // Проверяем, есть ли уже записи о посещаемости для этого занятия
            var existingAttendances = await _context.Attendances
                .Where(a => a.ScheduleId == scheduleId)
                .ToListAsync();
                
            // Формируем модель представления
            var viewModel = new GroupAttendanceViewModel
            {
                ScheduleId = scheduleId,
                Schedule = schedule,
                StudentRecords = new List<StudentAttendanceRecord>()
            };
            
            foreach (var student in students)
            {
                var existingRecord = existingAttendances.FirstOrDefault(a => a.StudentId == student.Id);
                
                viewModel.StudentRecords.Add(new StudentAttendanceRecord
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    Status = existingRecord?.Status ?? AttendanceStatus.Присутствует // По умолчанию присутствует
                });
            }
            
            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveGroupAttendance(int scheduleId, List<StudentAttendanceRecord> records)
        {
            if (scheduleId <= 0 || records == null || !records.Any())
            {
                return BadRequest();
            }
            
            // Получаем существующие записи о посещаемости для этого занятия
            var existingAttendances = await _context.Attendances
                .Where(a => a.ScheduleId == scheduleId)
                .ToListAsync();
                
            foreach (var record in records)
            {
                var existingRecord = existingAttendances.FirstOrDefault(a => a.StudentId == record.StudentId);
                
                if (existingRecord != null)
                {
                    // Обновляем существующую запись
                    existingRecord.Status = record.Status;
                    _context.Attendances.Update(existingRecord);
                }
                else
                {
                    // Создаем новую запись
                    _context.Attendances.Add(new Attendance
                    {
                        ScheduleId = scheduleId,
                        StudentId = record.StudentId,
                        Status = record.Status
                    });
                }
            }
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", new { scheduleId });
        }
    }
}
