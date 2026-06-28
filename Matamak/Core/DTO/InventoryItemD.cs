using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class InventoryItemD
    {
        [Required]
        public int ItemId { get; set; }
        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }
        [Range(0, int.MaxValue)]
        public int LowStockThreshold { get; set; } = 10;
    }
}
