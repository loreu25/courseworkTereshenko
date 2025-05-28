using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using UniversitySystem2.Models;
using System.Linq;

namespace UniversitySystem2.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class TeachersController : Controller
    {
        private readonly UniversityContext _context;
        public TeachersController(UniversityContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.CurrentFilter = searchString;
            
            var teachers = _context.Teachers.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(t => t.FullName.Contains(searchString));
            }
            
            return View(await teachers.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            // Получаем данные пользователя из формы
            string login = Request.Form["Login"];
            string password = Request.Form["Password"];
            // Разбиваем ФИО на имя и фамилию
            string[] fio = (teacher.FullName ?? "").Trim().Split(' ');
            string firstName = fio.Length > 0 ? fio[0] : "";
            string lastName = fio.Length > 1 ? string.Join(" ", fio.Skip(1)) : "";

            // Проверка на уникальность логина
            if (await _context.Users.AnyAsync(u => u.Login == login))
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                return View(teacher);
            }

            // Создаём пользователя
            var user = new User
            {
                Login = login,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Role = UserRole.Преподаватель
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Связываем преподавателя с пользователем
            teacher.UserId = user.Id;
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Update(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }
    }
}
