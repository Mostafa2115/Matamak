using Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IReprosatory
{
    public interface ICatrgoryRepo
    {
        public void AddCategory(CategoryD caegory);
        public void RemoveCategory(int id);
        public CatrgoryMV GetCategory(int id);

        public void UpdateCategory(CategoryD caegory, int id);

        public List<CatrgoryMV> GetAllCategories();
    }
}
