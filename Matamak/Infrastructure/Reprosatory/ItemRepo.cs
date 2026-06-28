using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Reprosatory
{
    public class ItemRepo : IItemRepo
    {
        private readonly DataContext dataContext;
        private readonly IItemService _item;

        public ItemRepo(DataContext dataContext, IItemService item)
        {
            this.dataContext = dataContext;
            this._item = item;
        }
        void IItemRepo.AddItem(ItemD item)
        {
          var item1=  _item.AddItem(item);
            if (item1 == null)
            {
                throw new Exception("Failed to add item.");
            }

            dataContext.Items.Add(item1);
            dataContext.SaveChanges();
        }

        List<ItemsModelView> IItemRepo.GetAllItems()
        {
            var items1 = _item.GetAllItems();
            if (items1 == null)
            {
                throw new Exception("Item From Service is null.");
            }
            return items1;
        }

        ItemsModelView IItemRepo.GetItemById(int id)
        {
              var item1 = _item.GetItem(id);
            if (item1 == null)
            {
                throw new Exception($"Item From Service is null for ID {id}.");
            }
            return item1;
        }

        List<ItemsModelView> IItemRepo.GetItemsByCategory(int categoryId)
        {
            var items1 = _item.GetItemsByCategory(categoryId);
            return items1 ?? new List<ItemsModelView>();
        }   
        List<ItemsModelView> IItemRepo.GetItemsByCountry(int countryId)
        {
            var items1 = _item.GetItemsByCountry(countryId);
            return items1 ?? new List<ItemsModelView>();
        }
           
        List<ItemsModelView> IItemRepo.GetItensByCountryAndCategory(int? countryId, int? categoryId)
        {
            if (!countryId.HasValue)
            {
                if (!categoryId.HasValue)
                {
                    return new List<ItemsModelView>();
                }
                var items1 = _item.GetItemsByCategory(categoryId.Value);
                return items1 ?? new List<ItemsModelView>();
            }
            else if (!categoryId.HasValue)
            {
                var items1 = _item.GetItemsByCountry(countryId.Value);
                return items1 ?? new List<ItemsModelView>();
            }
            else
            {
                var items1 = _item.GetItensByCountryAndCategory(countryId, categoryId);
                return items1 ?? new List<ItemsModelView>();
            }
        }

        void IItemRepo.RemoveItem(int id)
        {
            var item = dataContext.Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                dataContext.Items.Remove(item);
                dataContext.SaveChanges();
            }
                else
                {
                    throw new Exception($"Item with ID {id} not found.");
            }

        }

        void IItemRepo.UpdateItem(ItemD item, int id)
        {
            var item1 =_item.UpdateItem(item, id);
            if (item1 == null)
            { 
                throw new Exception($"Item From Service is null for ID {id}.");
            }

            dataContext.Items.Update(item1);
            dataContext.SaveChanges();

        }
    }
}
