using System.ComponentModel.DataAnnotations;

namespace XForms.Data;

public class Tag
{
   [Key] 
   public int Id { get; set; }
   [Required]
   [MaxLength(25, ErrorMessage = "Tag name cannot be longer than 50 characters.")]
   public required string Name { get; set; }
   
   // Navigation
   public virtual ICollection<Template>? Templates { get; set; }
}