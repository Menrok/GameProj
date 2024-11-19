using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Game.Models;
using System.Linq;
using System.Threading.Tasks;
using Game.Data;

namespace Game.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Game(int id)
        {
            var hero = await _context.Heroes
                .Include(h => h.Items)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hero == null)
            {
                return NotFound(); 
            }

            var quests = await _context.Quests.ToListAsync();
            var viewModel = new GameViewModel
            {
                Hero = hero,
                Quests = quests
            };

            return View(viewModel);
        }

        public async Task<IActionResult> StartQuest(int questId)
        {
            // Pobierz misję
            var quest = await _context.Quests.Include(q => q.Hero).FirstOrDefaultAsync(q => q.Id == questId);
            if (quest == null)
            {
                return NotFound();
            }

            // Logika rozpoczęcia misji (np. oznaczenie jej jako aktywnej)
            // Możesz dodać stan questa lub inne szczegóły w modelu Quest
            quest.Description = $"Rozpoczęto misję: {quest.Name}";

            await _context.SaveChangesAsync();

            // Przekierowanie z powrotem do widoku Game
            return RedirectToAction("Game", new { id = quest.HeroId });
        }



        [HttpPost]
        public async Task<IActionResult> EquipItem(int heroId, int itemId)
        {
            var hero = await _context.Heroes
                .Include(h => h.Items)
                .FirstOrDefaultAsync(h => h.Id == heroId);

            if (hero == null)
            {
                return NotFound();
            }

            var item = hero.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                hero.Strength += item.BonusStrength;
                hero.Dexterity += item.BonusDexterity;
                hero.Intelligence += item.BonusIntelligence;
                hero.Defense += item.BonusDefense;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
        public async Task<IActionResult> ShowQuests(int heroId)
        {
            var hero = await _context.Heroes.Include(h => h.Quests).FirstOrDefaultAsync(h => h.Id == heroId);
            if (hero == null)
            {
                return NotFound();
            }

            var availableQuests = await _context.Quests
                .Where(q => q.HeroId == heroId)
                .ToListAsync();

            var viewModel = new QuestsViewModel
            {
                Hero = hero,
                Quests = availableQuests
            };

            return View(viewModel);
        }

    }
}