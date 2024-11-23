using Game.Data;
using Game.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Game.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GameDashboard(int heroId)
        {
            var hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.Id == heroId);

            if (hero == null)
            {
                return NotFound();
            }

            ViewData["HeroId"] = hero.Id;

            return View(hero);
        }

        [HttpGet]
        public async Task<IActionResult> Missions(int heroId)
        {
            var hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.Id == heroId);

            if (hero == null)
            {
                return NotFound();
            }

            var quests = await _context.Quests.ToListAsync();
            var heroQuests = await _context.HeroQuests
                .Where(hq => hq.HeroId == heroId)
                .ToListAsync();

            ViewBag.Quests = quests;
            ViewBag.HeroQuests = heroQuests;
            ViewData["HeroId"] = hero.Id;

            return View(hero);
        }

        public async Task<IActionResult> AssignQuest(int heroId, int questId)
        {
            var hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.Id == heroId);

            var quest = await _context.Quests
                .FirstOrDefaultAsync(q => q.Id == questId);

            if (hero == null || quest == null)
            {
                return NotFound();
            }

            var heroQuest = new HeroQuest
            {
                HeroId = heroId,
                QuestId = questId,
                IsCompleted = false,
                StartTime = DateTime.Now
            };

            _context.HeroQuests.Add(heroQuest);
            await _context.SaveChangesAsync();

            return RedirectToAction("QuestProgress", new { heroId, questId });
        }
        
        [HttpGet]
        public async Task<IActionResult> QuestProgress(int heroId, int questId)
        {
            var heroQuest = await _context.HeroQuests
                .Include(hq => hq.Hero)
                .Include(hq => hq.Quest)
                .FirstOrDefaultAsync(hq => hq.HeroId == heroId && hq.QuestId == questId);

            if (heroQuest == null)
            {
                return NotFound();
            }

            var missionEndTime = heroQuest.StartTime.AddMinutes(10);
            var timeRemaining = missionEndTime - DateTime.Now;

            if (timeRemaining.TotalSeconds <= 0)
            {
                heroQuest.IsCompleted = true;
                await _context.SaveChangesAsync();
            }

            ViewBag.TimeRemaining = timeRemaining;
            return View(heroQuest);
        }


        [HttpPost]
        public async Task<IActionResult> CompleteQuest(int heroId, int questId)
        {
            var hero = await _context.Heroes
                .FirstOrDefaultAsync(h => h.Id == heroId);

            var heroQuest = await _context.HeroQuests
                .FirstOrDefaultAsync(hq => hq.HeroId == heroId && hq.QuestId == questId);

            var quest = await _context.Quests
                .FirstOrDefaultAsync(q => q.Id == questId);

            if (hero == null || heroQuest == null || quest == null)
            {
                return NotFound();
            }

            if (heroQuest.IsCompleted)
            {
                TempData["ErrorMessage"] = "Ta misja już została ukończona.";
                return RedirectToAction("Missions", new { heroId });
            }

            heroQuest.IsCompleted = true;
            heroQuest.EndTime = DateTime.Now; 

            hero.Experience += quest.RewardExperience;


            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Misja ukończona! Zdobyłeś " + quest.RewardExperience + " doświadczenia.";
            return RedirectToAction("Missions", new { heroId });
        }

        [HttpGet]
        public async Task<IActionResult> Hero(int heroId)
        {
            var hero = await _context.Heroes
                .Include(h => h.Items)
                .FirstOrDefaultAsync(h => h.Id == heroId);

            if (hero == null)
            {
                return NotFound();
            }

            ViewData["HeroId"] = hero.Id;

            return View(hero);
        }

        [HttpGet]
        public async Task<IActionResult> Shop(int heroId)
        {
            var hero = await _context.Heroes
                .Include(h => h.Items)
                .FirstOrDefaultAsync(h => h.Id == heroId);

            if (hero == null)
            {
                return NotFound();
            }

            var weapons = await _context.Items.Where(i => i.Type == "Broń").ToListAsync();
            var armors = await _context.Items.Where(i => i.Type == "Zbroja").ToListAsync();
            var amulets = await _context.Items.Where(i => i.Type == "Amulet").ToListAsync();

            ViewBag.Weapons = weapons ?? new List<Game.Models.Item>();
            ViewBag.Armors = armors ?? new List<Game.Models.Item>();
            ViewBag.Amulets = amulets ?? new List<Game.Models.Item>();
            ViewBag.Inventory = hero.Items ?? new List<Game.Models.Item>();

            ViewData["HeroId"] = hero.Id;

            return View(hero);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseItem(int heroId, int itemId)
        {
            var hero = await _context.Heroes.Include(h => h.Items)
                                            .FirstOrDefaultAsync(h => h.Id == heroId);
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);

            if (hero == null || item == null || hero.Money < item.Price)
            {
                TempData["ErrorMessage"] = "Nie masz wystarczająco złota.";
                return RedirectToAction("Shop", new { heroId });
            }

            if (hero.Items.Contains(item))
            {
                TempData["ErrorMessage"] = "Już posiadasz ten przedmiot.";
                return RedirectToAction("Shop", new { heroId });
            }

            hero.Money -= item.Price;
            hero.Items.Add(item);

            await _context.SaveChangesAsync();
            return RedirectToAction("Shop", new { heroId });
        }

        [HttpPost]
        public async Task<IActionResult> SellItem(int heroId, int itemId)
        {
            var hero = await _context.Heroes
                                      .Include(h => h.Items) 
                                      .FirstOrDefaultAsync(h => h.Id == heroId);

            var item = await _context.Items
                                      .FirstOrDefaultAsync(i => i.Id == itemId);

            if (hero == null || item == null)
            {
                return NotFound();
            }

            if (hero.Items.Contains(item))
            {
                hero.Money += item.Price;

                hero.Items.Remove(item);

                _context.Items.Remove(item);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Shop", new { heroId });
        }

        [HttpPost]
        public async Task<IActionResult> EquipItem(int heroId, int itemId)
        {
            var hero = await _context.Heroes
                                     .Include(h => h.Items)
                                     .FirstOrDefaultAsync(h => h.Id == heroId);

            var item = await _context.Items
                                      .FirstOrDefaultAsync(i => i.Id == itemId);

            if (hero == null || item == null)
            {
                return NotFound();
            }

            if (!item.IsEquipped)
            {
                var existingItem = hero.Items
                                       .FirstOrDefault(i => i.Type == item.Type && i.IsEquipped);

                if (existingItem != null)
                {
                    existingItem.IsEquipped = false;

                    hero.Strength -= existingItem.BonusStrength;
                    hero.Dexterity -= existingItem.BonusDexterity;
                    hero.Intelligence -= existingItem.BonusIntelligence;
                    hero.Defense -= existingItem.BonusDefense;
                    hero.Health -= existingItem.BonusHealth;
                }

                hero.Strength += item.BonusStrength;
                hero.Dexterity += item.BonusDexterity;
                hero.Intelligence += item.BonusIntelligence;
                hero.Defense += item.BonusDefense;
                hero.Health += item.BonusHealth;

                item.IsEquipped = true;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Hero", new { heroId });
        }

        [HttpPost]
        public async Task<IActionResult> UnequipItem(int heroId, int itemId)
        {
            var hero = await _context.Heroes
                                     .Include(h => h.Items)
                                     .FirstOrDefaultAsync(h => h.Id == heroId);
            var item = await _context.Items
                                      .FirstOrDefaultAsync(i => i.Id == itemId);

            if (hero == null || item == null)
            {
                return NotFound();
            }

            if (item.IsEquipped)
            {
                hero.Strength -= item.BonusStrength;
                hero.Dexterity -= item.BonusDexterity;
                hero.Intelligence -= item.BonusIntelligence;
                hero.Defense -= item.BonusDefense;
                hero.Health -= item.BonusHealth;

                item.IsEquipped = false;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Hero", new { heroId });
        }
    }
}