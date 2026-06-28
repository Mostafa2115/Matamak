using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class ReviewD
    {
        [Required]
        public int ItemId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
