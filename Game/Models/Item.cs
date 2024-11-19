namespace Game.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int BonusStrength { get; set; } = 0; 
        public int BonusDexterity { get; set; } = 0;
        public int BonusIntelligence { get; set; } = 0;
        public int BonusDefense { get; set; } = 0;
        public int BonusHealth { get; set; } = 0;

        public int HeroId { get; set; }
        public Hero Hero { get; set; }
    }
}
