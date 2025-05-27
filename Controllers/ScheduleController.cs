using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem2.Models;
using System.Threading.Tasks;
using System.Linq;

namespace UniversitySystem2.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly UniversityContext _context;
        public ScheduleController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            IQueryable<Schedule> query = _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject);

            if (User.IsInRole("Студент"))
            {
                // Найти студента, связанного с этим пользователем
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (student != null)
                {
                    query = query.Where(s => s.GroupId == student.GroupId);
                }
                else
                {
                    // Если студент не найден, ничего не показывать
                    query = query.Where(s => false);
                }
            }
            else if (User.IsInRole("Преподаватель"))
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacher != null)
                {
                    query = query.Where(s => s.TeacherId == teacher.Id);
                }
                else
                {
                    query = query.Where(s => false);
                }
            }
            // Администратор видит всё
            var schedule = await query.ToListAsync();
            return View(schedule);
        }

        public IActionResult Create()
        {
            ViewBag.Groups = _context.Groups.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            ViewBag.Subjects = _context.Subjects.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Groups = _context.Groups.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            ViewBag.Subjects = _context.Subjects.ToList();
            return View(schedule);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return NotFound();
            ViewBag.Groups = _context.Groups.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            ViewBag.Subjects = _context.Subjects.ToList();
            return View(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Schedules.Update(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Groups = _context.Groups.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            ViewBag.Subjects = _context.Subjects.ToList();
            return View(schedule);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }
    }
}
