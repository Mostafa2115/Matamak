using Core.DTO;
using Core.IReprosatory;
using Core.ModelView;
using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Reprosatory
{
    public class InventoryRepo : IInventoryRepo
    {
        private readonly DataContext dataContext;

        public InventoryRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public InventoryItemMV AddOrUpdate(InventoryItemD inventoryItem)
        {
            var item = dataContext.Items.FirstOrDefault(i => i.Id == inventoryItem.ItemId);
            if (item == null)
            {
                throw new Exception("Item not found.");
            }

            var existing = dataContext.InventoryItems
                .Include(i => i.Item)
                .FirstOrDefault(i => i.ItemId == inventoryItem.ItemId);

            if (existing == null)
            {
                existing = new InventoryItem
                {
                    ItemId = inventoryItem.ItemId,
                    QuantityInStock = inventoryItem.QuantityInStock,
                    LowStockThreshold = inventoryItem.LowStockThreshold,
                    LastUpdatedAt = DateTime.UtcNow
                };
                dataContext.InventoryItems.Add(existing);
            }
            else
            {
                existing.QuantityInStock = inventoryItem.QuantityInStock;
                existing.LowStockThreshold = inventoryItem.LowStockThreshold;
                existing.LastUpdatedAt = DateTime.UtcNow;
                dataContext.InventoryItems.Update(existing);
            }

            dataContext.SaveChanges();

            existing = dataContext.InventoryItems.Include(i => i.Item).First(i => i.Id == existing.Id);
            return Map(existing);
        }

        public List<InventoryItemMV> GetAll()
        {
            return dataContext.InventoryItems.Include(i => i.Item).Select(Map).ToList();
        }

        public List<InventoryItemMV> GetLowStockItems()
        {
            return dataContext.InventoryItems.Include(i => i.Item)
                .Where(i => i.QuantityInStock <= i.LowStockThreshold)
                .Select(Map)
                .ToList();
        }

        private static InventoryItemMV Map(InventoryItem inventoryItem)
        {
            return new InventoryItemMV
            {
                Id = inventoryItem.Id,
                ItemId = inventoryItem.ItemId,
                ItemName = inventoryItem.Item?.Name ?? string.Empty,
                QuantityInStock = inventoryItem.QuantityInStock,
                LowStockThreshold = inventoryItem.LowStockThreshold,
                IsLowStock = inventoryItem.IsLowStock,
                LastUpdatedAt = inventoryItem.LastUpdatedAt
            };
        }
    }
}
