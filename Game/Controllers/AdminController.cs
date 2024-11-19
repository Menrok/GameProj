using Game.Data;
using Game.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Game.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AdminDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        public IActionResult AddQuest()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return Unauthorized();
            }

            return View(new Quest());
        }

        [HttpPost]
        public async Task<IActionResult> AddQuest(Quest quest)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                _context.Quests.Add(quest);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Quest został dodany!";
                return RedirectToAction("AdminDashboard");
            }

            return View(quest);
        }
    }
}
