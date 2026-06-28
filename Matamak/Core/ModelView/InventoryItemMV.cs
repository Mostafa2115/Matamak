using System;

namespace Core.ModelView
{
    public class InventoryItemMV
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
