using System;

namespace Core.Models
{
        public class Review
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
        public Item? Item { get; set; }
            public string CustomerUsername { get; set; } = string.Empty;
            public int Rating { get; set; }
            public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
