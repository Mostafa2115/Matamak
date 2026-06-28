

namespace Infrastructure.Services
{
    public class IteamServices : IItemService
    {
        private readonly DataContext dataContext;

        public IteamServices(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public Item AddItem(ItemD item)
        {
            Item item1 = new Item();
            item1.Name = item.Name;
            item1.Description = item.Description;
            item1.Price = item.Price;
            item1.ImageUrl = item.ImageUrl;
            item1.CatogryId = item.CatogryId;
            item1.CountryId = item.CountryId;
            item1.IsAvailable = item.IsAvailable;

            return item1;

        }


        public Item UpdateItem(ItemD item, int id)
        {
            var item1 = dataContext.Items.Find(id);
            
            if (item1 != null)
            {
                item1.Name = item.Name;
                item1.Description = item.Description;
                item1.Price = item.Price;
                item1.ImageUrl = item.ImageUrl;
                item1.CatogryId = item.CatogryId;
                item1.CountryId = item.CountryId;
                item1.IsAvailable = item.IsAvailable;
                return item1;
            }
            else
            {
                throw new Exception("Item not found for update.");
            }
        }

        List<ItemsModelView> IItemService.GetAllItems()
        {
            var items = new List<ItemsModelView >();
            var item1 = dataContext.Items.ToList();
            if (item1 == null)
            {
                return items;
            }
            foreach (var item in item1)
            {
                items.Add(new ItemsModelView
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                    CatogryId = item.CatogryId,
                    CountryId = item.CountryId,
                    IsAvailable = item.IsAvailable
                });
            }
            return items;
        }

        ItemsModelView IItemService.GetItem(int id)
        {
            var item = dataContext.Items.Find(id);
            if (item != null)
            {
                return new ItemsModelView
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                    CatogryId = item.CatogryId,
                    CountryId = item.CountryId,
                    IsAvailable = item.IsAvailable
                };
            }
            throw new Exception($"Item with ID {id} not found.");
        }

        List<ItemsModelView> IItemService.GetItemsByCategory(int categoryId)
        {
            var items = new List<ItemsModelView>();
            var item = dataContext.Items.Where(i => i.CatogryId == categoryId).ToList();
            if (item == null)
            {
                return items;
            }
            foreach (var i in item)
            {
                items.Add(new ItemsModelView
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    ImageUrl = i.ImageUrl,
                    CatogryId = i.CatogryId,
                    CountryId = i.CountryId,
                    IsAvailable = i.IsAvailable
                });
            }
            return items;
        }

        List<ItemsModelView> IItemService.GetItemsByCountry(int countryId)
        {
            var items = new List<ItemsModelView>();
            var item = dataContext.Items.Where(i => i.CountryId == countryId).ToList();
            if (item == null)
            {
                return items;
            }
            foreach (var i in item)
            {
                items.Add(new ItemsModelView
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    ImageUrl = i.ImageUrl,
                    CatogryId = i.CatogryId,
                    CountryId = i.CountryId,
                    IsAvailable = i.IsAvailable
                });
            }
            return items;
        }

        List<ItemsModelView> IItemService.GetItensByCountryAndCategory(int? countryId, int? categoryId)
        {
            var items = new List<ItemsModelView>();
            var item = dataContext.Items.Where(i => i.CountryId == countryId && i.CatogryId == categoryId).ToList();
            if (item == null)
            {
                return items;
            }
            foreach (var i in item)
            {
                items.Add(new ItemsModelView
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    ImageUrl = i.ImageUrl,
                    CatogryId = i.CatogryId,
                    CountryId = i.CountryId,
                    IsAvailable = i.IsAvailable
                });
            }
            return items;
        }
    }
}
