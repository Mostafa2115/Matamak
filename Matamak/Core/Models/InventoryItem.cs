using System;

namespace Core.Models
{
        public class InventoryItem
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
        public Item? Item { get; set; }
            public int QuantityInStock { get; set; }
            public int LowStockThreshold { get; set; } = 10;
            public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsLowStock => QuantityInStock <= LowStockThreshold;
    }
}
