using Microsoft.Build.Framework;

namespace XForms.Data.Dtos;

public class CreateTemplateDto
{
   public string Title { get; set; }
   public string Description { get; set; }
   public string ImageUrl { get; set; }
   public bool IsPublic { get; set; }
   public List<string> AllowedUserIds { get; set; }
}