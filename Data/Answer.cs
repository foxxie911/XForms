using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class Answer
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100, ErrorMessage = "Answer should be less than 100 characters")]
    public string? SingleLine { get; set; }
    [DataType(DataType.MultilineText)]
    public string? Paragraph { get; set; }
    public uint? Number { get; set; }
    [Required]
    public int QuestionId { get; set; }
    [Required]
    public int FormId { get; set; }
    
    // Locking
    [ConcurrencyCheck]
    public Guid Version { get; set; }
    
    // Navigation
    [ForeignKey("FormId")]
    public virtual Form? Form { get; set; }
    [ForeignKey("QuestionId")]
    public virtual Question? Question { get; set; }
}