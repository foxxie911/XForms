namespace XForms.Services.Interface;

public interface IUserService
{
    Task<bool> ToggleDarkMode(bool isDarkMode, string userId);
}