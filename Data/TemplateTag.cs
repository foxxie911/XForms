using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XForms.Data;

public class TemplateTag
{
   [Key]
   public int Id { get; set; }
   [Required]
   public int TemplateId { get; set; }
   [Required]
   public int TagId { get; set; }
}