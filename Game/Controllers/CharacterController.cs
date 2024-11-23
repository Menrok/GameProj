using Game.Data;
using Game.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Game.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CharacterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CreateHero()
        {
            var hero = new Hero();
            return View(hero);
        }

        public async Task<IActionResult> CharacterSelection()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.Include(u => u.Heroes).FirstOrDefaultAsync(u => u.Email == email);
            if (user?.Heroes == null || !user.Heroes.Any())
            {
                return View();
            }

            var heroesList = user.Heroes.ToList();

            return View(heroesList);
        }
        public async Task<IActionResult> SelectHero(int id)
        {
            var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.Id == id);
            if (hero == null)
            {
                return NotFound();
            }

            return RedirectToAction("GameDashboard", "Game", new { heroId = hero.Id });
        }


        [HttpPost]
        public async Task<IActionResult> CreateHero(Hero hero)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.Include(u => u.Heroes).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (user.Heroes != null && user.Heroes.Count >= 4)
            {
                TempData["Error"] = "Możesz stworzyć maksymalnie 4 postacie.";
                return RedirectToAction("CharacterSelection");
            }

            hero.UserId = user.Id;

            switch (hero.Class)
            {
                case "Warrior":
                    hero.Strength = 20;
                    hero.Intelligence = 10;
                    hero.Dexterity = 10;
                    break;
                case "Mage":
                    hero.Strength = 10;
                    hero.Intelligence = 20;
                    hero.Dexterity = 10;
                    break;
                case "Archer":
                    hero.Strength = 10;
                    hero.Intelligence = 10;
                    hero.Dexterity = 20; 
                    break;
                default:
                    break;
            }

            _context.Heroes.Add(hero);
            await _context.SaveChangesAsync();

            return RedirectToAction("CharacterSelection");
        }


        [HttpGet]
        public async Task<IActionResult> EditHero(int id)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.Include(u => u.Heroes).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                Console.WriteLine("Użytkownik nie znaleziony.");
                return RedirectToAction("Login", "Account");
            }

            if (user.Heroes == null || !user.Heroes.Any())
            {
                Console.WriteLine("Postać użytkownika nie znaleziona.");
                return RedirectToAction("CreateHero");
            }

            var hero = user.Heroes.FirstOrDefault(h => h.Id == id);
            if (hero == null)
            {
                Console.WriteLine($"Postać o ID {id} nie została znaleziona.");
                return RedirectToAction("CharacterSelection");
            }

            return View(hero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHero(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Nazwa postaci nie może być pusta.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var existingHero = await _context.Heroes.FindAsync(id);
            if (existingHero == null)
            {
                return NotFound();
            }

            existingHero.Name = name;

            _context.Update(existingHero);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CharacterSelection));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHero(int id)
        {
            var hero = await _context.Heroes.FindAsync(id);
            if (hero == null)
            {
                return NotFound();
            }

            _context.Heroes.Remove(hero);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CharacterSelection));
        }
    }
}