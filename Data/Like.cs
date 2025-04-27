using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class Like
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public required string UserId { get; set; }
    [Required]
    public required int TemplateId { get; set; }
    
    // Navigation
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
    [ForeignKey("TemplateId")]
    public virtual Template? Template { get; set; }
}