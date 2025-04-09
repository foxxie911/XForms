using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace XForms.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100, ErrorMessage = "User name cannot be longer than 100 characters.")]
    public required string DisplayName { get; set; }
    
    [DataType(DataType.ImageUrl)]
    public string? AvatarUrl { get; set; }
    
    
}

