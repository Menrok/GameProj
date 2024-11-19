using System.ComponentModel.DataAnnotations;

namespace Game.Models
{
    public class Hero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Strength { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int PointsToSpend { get; set; } = 0;
        public int Defense { get; set; } = 0;
        public int Health { get; set; } = 100;

        public int UserId { get; set; }
        public User User { get; set; }
        
        public ICollection<Quest> Quests { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
