namespace WebApplication1.Models.DTO.BookDTO
{
    public class BookEditUIDTO
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Genre { get; set; }
        public int PageCount { get; set; }
        public string AuthorName { get; set; }
        public int Price { get; set; }
    }
}
