using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace XForms.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    // Properties (Not all extended from Identity User
    [Required]
    [MaxLength(100, ErrorMessage = "Your given name cannot be longer than 100 characters.")]
    public required string DisplayName { get; set; }
    [DataType(DataType.ImageUrl)]
    public string? AvatarUrl { get; set; }
    public bool IsDarkMode { get; set; }
    
    // Navigation
    public virtual ICollection<Template>? Templates { get; set; }
    public virtual ICollection<Form>? Forms { get; set; }
    public virtual ICollection<Comment>? Comments { get; set; }
    public virtual ICollection<Like>? Likes { get; set; }
}