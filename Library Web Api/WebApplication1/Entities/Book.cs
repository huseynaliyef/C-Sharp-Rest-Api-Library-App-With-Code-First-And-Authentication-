using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string BookName { get; set; }
        [Required]
        [StringLength(50)]
        public string Genre { get; set; }
        [Required]
        public int PageCount { get; set; }
        [Required]
        [StringLength(50)]
        public string AuthorName { get; set; }
        [Required]
        public int Price { get; set; }
    }
}
