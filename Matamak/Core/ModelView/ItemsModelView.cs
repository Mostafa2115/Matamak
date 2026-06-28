using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ModelView
{
    public class ItemsModelView
    {
        public int Id { get; set; }
        public string?   Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int CatogryId { get; set; }
        public int CountryId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
