using Core.DTO;
using Core.Models;
using Core.ModelView;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IServices
{
    public interface IItemService
    {
        public Item AddItem(ItemD item);
        public Item UpdateItem(ItemD item, int id);
        public ItemsModelView GetItem(int id);
        public List<ItemsModelView> GetAllItems();
         public List<ItemsModelView> GetItemsByCategory(int categoryId);
          public List<ItemsModelView> GetItemsByCountry(int countryId);
        public List<ItemsModelView> GetItensByCountryAndCategory(int? countryId, int? categoryId);
    }
}
