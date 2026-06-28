using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Country
    {
        public int id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Item> Items { get; set; } = new List<Item>();
    }
}
