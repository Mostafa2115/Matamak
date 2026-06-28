using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class _DailyDineinCounter
    {
        public int Id { get; set; }
        public int Dinein { get; set; }
        public DateTime lastDineinDate { get; set; }
    }
}
