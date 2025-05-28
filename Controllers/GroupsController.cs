using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystem2.Models;
using System.Threading.Tasks;
using System.Linq;

namespace UniversitySystem2.Controllers
{
    public class GroupsController : Controller
    {
        private readonly UniversityContext _context;
        public GroupsController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.CurrentFilter = searchString;
            
            var groups = _context.Groups.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                groups = groups.Where(g => g.Name.Contains(searchString));
            }
            
            return View(await groups.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Groups.Update(group);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }
    }
}
