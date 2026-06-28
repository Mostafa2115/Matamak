using Core.DTO;
using Core.ModelView;
using System.Collections.Generic;

namespace Core.IReprosatory
{
    public interface IInventoryRepo
    {
        InventoryItemMV AddOrUpdate(InventoryItemD inventoryItem);
        List<InventoryItemMV> GetAll();
        List<InventoryItemMV> GetLowStockItems();
    }
}
