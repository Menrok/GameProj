using System.ComponentModel.DataAnnotations;

namespace Game.Models
{
    public class Item
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nazwa przedmiotu jest wymagana")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Typ przedmiotu jest wymagany")]
        public string Type { get; set; }
        public int BonusStrength { get; set; } = 0; 
        public int BonusDexterity { get; set; } = 0;
        public int BonusIntelligence { get; set; } = 0;
        public int BonusDefense { get; set; } = 0;
        public int BonusHealth { get; set; } = 0;

        [Range(1, int.MaxValue, ErrorMessage = "Cena musi być większa niż 0.")]
        public int Price { get; set; }

        public bool IsForSale { get; set; } = true;

        public bool IsEquipped { get; set; } = false;

        public ICollection<Hero> Heroes { get; set; } = new List<Hero>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
