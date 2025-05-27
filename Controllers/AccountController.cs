using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UniversitySystem2.Models;

namespace UniversitySystem2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UniversityContext _context;
        public AccountController(UniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("FullName", user.FirstName + " " + user.LastName)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string login, string password, string firstName, string lastName, UserRole role)
        {
            if (await _context.Users.AnyAsync(u => u.Login == login))
            {
                ViewBag.Error = "Пользователь с таким логином уже существует";
                return View();
            }
            var user = new User { Login = login, Password = password, FirstName = firstName, LastName = lastName, Role = role };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
