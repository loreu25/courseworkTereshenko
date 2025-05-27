using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem2.Models;
using System.Threading.Tasks;
using System.Linq;

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
            var attendances = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Group)
                .Include(a => a.Schedule.Subject)
                .Include(a => a.Schedule.Teacher);
            if (scheduleId.HasValue)
                return View(await attendances.Where(a => a.ScheduleId == scheduleId.Value).ToListAsync());
            return View(await attendances.ToListAsync());
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
    }
}
