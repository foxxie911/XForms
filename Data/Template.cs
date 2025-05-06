using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class Template
{
    // Properties
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(250)]
    public required string Title { get; set; }
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
    [DataType(DataType.ImageUrl)]
    [MaxLength(200)]
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime? UpdatedAt { get; set; }
    [Required]
    [MaxLength(50)]
    public string? CreatorId { get; set; }
   
    // Locking
    [ConcurrencyCheck]
    public Guid Version { get; set; }

    // Navigation
    [Key, ForeignKey("CreatorId")]
    public virtual ApplicationUser? Creator { get; set; }
    public virtual ICollection<Question>? Questions { get; set; }
    public virtual ICollection<Form>? Forms { get; set; }
    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<Like>? Likes { get; set; }
    public virtual ICollection<Tag>? Tags { get; set; }
}