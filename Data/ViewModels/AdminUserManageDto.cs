namespace XForms.Data.ViewModels;

public class AdminUserManageDto
{
    public required string Id { get; set; }
    public required string DisplayName { get; set; }
    public required string? Email { get; set; }
    public required string Roles { get; set; }
    public required bool IsBlocked { get; set; }
}