using Core.DTO;
using Core.Models;
using Core.ModelView;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IReprosatory
{
    public interface IItemRepo
    {
        public void AddItem(ItemD item);
        public void RemoveItem(int id);
        public void UpdateItem( ItemD item, int id);
        public ItemsModelView GetItemById(int id);
        public List<ItemsModelView> GetAllItems();
        public List<ItemsModelView> GetItemsByCategory(int categoryId);
         public List<ItemsModelView> GetItemsByCountry(int countryId);
        public List<ItemsModelView> GetItensByCountryAndCategory(int? countryId, int? categoryId);
    }
}
