namespace api.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int? StockId { get; set; }
        public Stock? Stock { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
