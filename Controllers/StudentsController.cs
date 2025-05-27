using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using UniversitySystem2.Models;

namespace UniversitySystem2.Controllers
{
    [Authorize(Roles = "Администратор,Преподаватель")]
    public class StudentsController : Controller
    {
        private readonly UniversityContext _context;
        public StudentsController(UniversityContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.Include(s => s.Group).ToListAsync();
            return View(students);
        }
        public IActionResult Create()
        {
            ViewBag.Groups = _context.Groups.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            // Получаем данные пользователя из формы
            string login = Request.Form["Login"];
            string password = Request.Form["Password"];
            // Разбиваем ФИО на имя и фамилию
            string[] fio = (student.FullName ?? "").Trim().Split(' ');
            string firstName = fio.Length > 0 ? fio[0] : "";
            string lastName = fio.Length > 1 ? string.Join(" ", fio.Skip(1)) : "";

            // Проверка на уникальность логина
            if (await _context.Users.AnyAsync(u => u.Login == login))
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                ViewBag.Groups = _context.Groups.ToList();
                return View(student);
            }

            // Создаём пользователя
            var user = new User
            {
                Login = login,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Role = UserRole.Студент
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Связываем студента с пользователем
            student.UserId = user.Id;
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            ViewBag.Groups = _context.Groups.ToList();
            return View(student);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Groups = _context.Groups.ToList();
            return View(student);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }
    }
}
