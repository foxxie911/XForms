using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace XForms.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(50, ErrorMessage = "Your display name must be lower than 50 characters")]
    public required string DisplayName {get; set;}
}

