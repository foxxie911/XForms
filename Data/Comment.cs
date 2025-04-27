using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class Comment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string CreatorId { get; set; }
    [Required]
    public required int TemplateId { get; set; }
    [Required]
    public required string Message { get; set; }
    [DataType(DataType.DateTime)] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    [ForeignKey("creatorId")]
    public virtual ApplicationUser? Creator { get; set; }
    [ForeignKey("templateId")]
    public virtual Template? Template { get; set; }
}