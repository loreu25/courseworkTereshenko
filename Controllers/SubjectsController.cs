using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem2.Models;
using System.Threading.Tasks;
using System.Linq;

namespace UniversitySystem2.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly UniversityContext _context;
        public SubjectsController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.CurrentFilter = searchString;
            
            var subjectsQuery = _context.Subjects.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                subjectsQuery = subjectsQuery.Where(s => s.Name.Contains(searchString));
            }
            
            var subjects = await subjectsQuery.ToListAsync();
            return View(subjects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(subject);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Update(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(subject);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }
    }
}
