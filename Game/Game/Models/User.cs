﻿namespace Game.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Role { get; set; }

        public List<Hero> Heroes { get; set; }
    }
}
