using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using X.PagedList.Extensions;
using XForms.Data;
using XForms.Data.Dtos;
using Exception = System.Exception;

namespace XForms.Components.Admin;

public partial class ManageUser : ComponentBase
{
    private HashSet<AdminUserManageDto> _selectedUsers = [];
    private bool _deleteDialogVisible;
    private bool _blockDialogVisible;
    private bool _adminRightsDialogVisible;
    private bool _isCurrentUser;
    private MudDataGrid<AdminUserManageDto>? _dataGrid;
    private AuthenticationState? _authenticationState;
    private readonly GridDataDto? _gridDataDto = new GridDataDto();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
    }

    private async Task<GridData<AdminUserManageDto>> LoadUsersAsync(GridState<AdminUserManageDto> state)
    {
        try
        {
            await Task.Delay(100);

            _gridDataDto!.Page = state.Page;
            _gridDataDto.PageSize = state.PageSize;

            var pageData = await GetUsers(_gridDataDto.Page, _gridDataDto.PageSize, _gridDataDto.SearchText!);

            return new GridData<AdminUserManageDto>()
            {
                Items = pageData.Item1,
                TotalItems = pageData.Item2
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Snackbar.Add($"{e.Message}", Severity.Error);
            return new GridData<AdminUserManageDto>()
            {
                Items = new List<AdminUserManageDto>(),
                TotalItems = 0
            };
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task<Tuple<List<AdminUserManageDto>, int>> GetUsers(int page, int pageSize, string searchText)
    {
        var searchData = SearchData(UserManager.Users.AsNoTracking(), page, pageSize, searchText);
        var applicationUsers = string.IsNullOrWhiteSpace(searchText)
            ? UserManager.Users.AsNoTracking().ToPagedList(page + 1, pageSize).OrderBy(u => u.DisplayName).ToList()
            : searchData.Item1;
        var applicationUsersCount = string.IsNullOrWhiteSpace(searchText)
            ? await UserManager.Users.CountAsync()
            : searchData.Item2;
        var users = await ModelToDto(applicationUsers);
        return Tuple.Create(users, applicationUsersCount);
    }

    private async Task<List<AdminUserManageDto>> ModelToDto(List<ApplicationUser> applicationUsers)
    {
        var result = new List<AdminUserManageDto>();
        foreach (var applicationUser in applicationUsers)
        {
            var applicationUserRoles = await UserManager.GetRolesAsync(applicationUser);
            result.Add(new AdminUserManageDto
            {
                Id = applicationUser.Id,
                DisplayName = applicationUser.DisplayName,
                Email = applicationUser.Email,
                Roles = string.Join(",", applicationUserRoles),
                IsBlocked = applicationUser.LockoutEnd != null && applicationUser.LockoutEnd > DateTimeOffset.UtcNow
            });
        }

        return result;
    }

    private void OnSelectedItemsChanged(HashSet<AdminUserManageDto> users)
    {
        _selectedUsers = users;
        StateHasChanged();
    }

    private Tuple<List<ApplicationUser>, int> SearchData(IQueryable<ApplicationUser> query, int page, int pageSize,
        string searchString)
    {
        var result = query
            .Where(u =>
                EF.Functions.Like(u.DisplayName, $"%{searchString}%") ||
                EF.Functions.Like(u.Email, $"%{searchString}%")).ToList();
        var resultCount = result.Count;
        return Tuple.Create(result.ToPagedList(page + 1, pageSize).ToList(), resultCount);
    }

    private void ToggleDeleteDialog()
    {
        _deleteDialogVisible = !_deleteDialogVisible;
    }

    private void ToggleBlockDialog()
    {
        _blockDialogVisible = !_blockDialogVisible;
    }

    private void ToggleAdminRightsDialog()
    {
        _adminRightsDialogVisible = !_adminRightsDialogVisible;
    }

    private async Task SignOutIfCurrentUser()
    {
        try
        {
            if (_isCurrentUser)
            {
                _isCurrentUser = false;
                await JsRuntime.InvokeVoidAsync("localStorage.clear");
                await JsRuntime.InvokeVoidAsync("sessionStorage.clear");
                NavigationManager.NavigateTo("/Admin/LogOut", true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error => {e.Message}");
            Snackbar.Add($"{e.Message}", Severity.Error);
        }
    }

    private async Task DeleteSelectedUsers()
    {
        try
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

            var successCount = 0;
            var failureCount = 0;
            var unknownUserCount = 0;

            foreach (var user in _selectedUsers)
            {
                var dbUser = await UserManager.FindByIdAsync(user.Id);
                if (dbUser == null)
                {
                    unknownUserCount++;
                    continue;
                }

                if (dbUser == await UserManager.GetUserAsync(_authenticationState!.User)) _isCurrentUser = true;
                var result = await UserManager.DeleteAsync(dbUser);
                _ = result.Succeeded ? successCount++ : failureCount++;
            }

            if (failureCount > 0)
                Snackbar.Add($"Delete failed for {failureCount} users", Severity.Error);
            if (successCount > 0)
                Snackbar.Add($"Successfully deleted {successCount} users", Severity.Success);
            if (unknownUserCount > 0)
                Snackbar.Add($"{unknownUserCount} does not exist", Severity.Warning);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting users: {e.Message}");
            Snackbar.Add($"{e.Message}", Severity.Error);
        }
        finally
        {
            ToggleDeleteDialog();
            _selectedUsers.Clear();
            await _dataGrid!.ReloadServerData();
            await SignOutIfCurrentUser();
        }
    }

    private bool IsLockedOut(ApplicationUser user)
    {
        return user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow;
    }

    private async Task BlockSelectedUsers()
    {
        try
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

            var successCount = 0;
            var failureCount = 0;

            foreach (var user in _selectedUsers)
            {
                var dbUser = await UserManager.FindByIdAsync(user.Id);
                if (dbUser == null) Snackbar.Add("User not found", Severity.Error);
                var result = IsLockedOut(dbUser!)
                    ? await UserManager.SetLockoutEndDateAsync(dbUser!, DateTimeOffset.MinValue)
                    : await UserManager.SetLockoutEndDateAsync(dbUser!, DateTimeOffset.MaxValue);
                if (dbUser == await UserManager.GetUserAsync(_authenticationState!.User)) _isCurrentUser = true;
                _ = result.Succeeded ? successCount++ : failureCount++;
            }

            if (successCount > 0)
                Snackbar.Add($"Block status changed for {successCount} users", Severity.Success);
            if (failureCount > 0)
                Snackbar.Add($"Changing block status failed for {failureCount} user(s)", Severity.Error);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error changing block status: {e.Message}");
            Snackbar.Add($"{e.Message}", Severity.Error);
        }
        finally
        {
            ToggleBlockDialog();
            _selectedUsers.Clear();
            await _dataGrid!.ReloadServerData();
            await SignOutIfCurrentUser();
        }
    }

    private async Task ChangeAdminRights()
    {
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        try
        {
            var successCount = 0;
            var failureCount = 0;

            foreach (var user in _selectedUsers)
            {
                var dbUser = await UserManager.FindByIdAsync(user.Id);
                if (dbUser == null) Snackbar.Add("User not fount", Severity.Error);
                var result = await UserManager.IsInRoleAsync(dbUser!, "Admin")
                    ? await UserManager.RemoveFromRoleAsync(dbUser!, "Admin")
                    : await UserManager.AddToRoleAsync(dbUser!, "Admin");
                _ = result.Succeeded ? successCount++ : failureCount++;
                if (successCount > 0)
                    Snackbar.Add($"Admin rights changed for {successCount} user(s)", Severity.Success);
                if (failureCount > 0)
                    Snackbar.Add($"Operation failed for {failureCount} user(s)", Severity.Error);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Snackbar.Add(e.Message, Severity.Error);
        }
        finally
        {
            ToggleAdminRightsDialog();
            _selectedUsers.Clear();
            await _dataGrid!.ReloadServerData();
        }
    }
}