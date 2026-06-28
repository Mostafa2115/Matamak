using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class ItemD
    {
        [Required]
        public string Name { get; set; }=string.Empty;
        [Required]
        public string Description { get; set; }= string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public int CatogryId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public  bool IsAvailable { get; set; }
    }
}
