using Core.DTO;
using Core.IReprosatory;
using Core.Models;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Reprosatory
{
    public class CategoryRepo : ICatrgoryRepo
    {
        private readonly DataContext dataContext;

        public CategoryRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        void ICatrgoryRepo.AddCategory(CategoryD caegory)
        {
            Category category = new Category
            {
                Name = caegory.Name
            };
            dataContext.Categories.Add(category);
            dataContext.SaveChanges();
        }

        List<CatrgoryMV> ICatrgoryRepo.GetAllCategories()
        {
           var categories = dataContext.Categories;
            List<CatrgoryMV> categoryDTOs = new List<CatrgoryMV>();
            foreach (var category in categories)
            {
                CatrgoryMV categoryDTO = new CatrgoryMV
                {
                    Id = category.Id,           
                    Name = category.Name
                };
                categoryDTOs.Add(categoryDTO);
            }
            return categoryDTOs;
        }

        CatrgoryMV ICatrgoryRepo.GetCategory(int id)
        {
            var category = dataContext.Categories.Find(id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            var categoryDTO = new CatrgoryMV
            {
                Id = category.Id,
                Name = category.Name
            };
            return categoryDTO;
        }

        void ICatrgoryRepo.RemoveCategory(int id)
        {
            var items = dataContext.Items.Where(i => i.CatogryId == id).ToList();
            if (items.Count > 0)
            {
                throw new Exception("Cannot delete category because it has associated items , Delete the items first.");
            }
            var category = dataContext.Categories.Find(id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
                dataContext.Categories.Remove(category);
            dataContext.SaveChanges();

        }

        void ICatrgoryRepo.UpdateCategory(CategoryD caegory, int id)
        {
            var category = dataContext.Categories.Find(id);
            if (category != null)
            {
                category.Name = caegory.Name;
                dataContext.SaveChanges();
            }
        }
    }
}
