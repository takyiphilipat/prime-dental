using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime PublishedDate { get; set; } = DateTime.Now;
    }
}
