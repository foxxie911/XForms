using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class Form
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int TemplateId { get; set; }
    [Required]
    public string CreatorId { get; set; }
    [DataType(DataType.DateTime)] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }
    
    // Locking
    [ConcurrencyCheck]
    public Guid Version { get; set; }
    
    // Navigation
    [ForeignKey("CreatorId")]
    public virtual ApplicationUser Creator { get; set; }
    [ForeignKey("TemplateId")]
    public virtual Template Template { get; set; }
    public virtual ICollection<Answer> Answers { get; set; }
}