using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class Template
{
    // Properties
    [Key]
    public int Id { get; init; }
    [Required]
    [MaxLength(100, ErrorMessage = "Title has to be under 100 characters")]
    public required string Title { get; init; }
    [DataType(DataType.MultilineText)]
    public string? Description { get; init; }
    [DataType(DataType.ImageUrl)]
    public string? ImageUrl { get; init; }
    public bool IsPublic { get; init; }
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; init; }
    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; init; }
    [Required]
    public string CreatorId { get; init; }
   
    // Locking
    [Timestamp]
    public byte[] Version { get; set;}

    // Navigation
    [Key, ForeignKey("CreatorId")]
    public virtual ApplicationUser Creator { get; init; }
    public virtual ICollection<Question> Questions { get; set; }
}