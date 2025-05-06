using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class Question
{
    // Properties
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(250)]
    public required string Title { get; set; }
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
    [Required]
    public QuestionType Type { get; set; }
    [Required]
    public int Order { get; set; }
    [Required]
    public int TemplateId { get; set; }
   
    // Locking
    [ConcurrencyCheck]
    public Guid Version { get; set; }
    
    // Navigation
    [Key, ForeignKey("TemplateId")]
    public Template? Template { get; set; }
}