using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class CategoryD
    {
        [Required]
        public string Name { get; set; }=string.Empty;
    }
}
