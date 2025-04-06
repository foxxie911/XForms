using CloudinaryDotNet;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using XForms.Components;
using XForms.Components.Account;
using XForms.Data;
using XForms.Seeder;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddMudServices();
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityUserAccessor>();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        /*
        .AddRoleManager<RoleManager<IdentityRole>>()
        .AddRoleStore<RoleStore<IdentityRole, ApplicationDbContext>>()
        */
        .AddSignInManager()
        .AddDefaultTokenProviders();
    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

    var cloudinaryConfiguration = builder.Configuration.GetSection("Cloudinary");
    var account = new Account
    (
        cloudinaryConfiguration["CloudName"],
        cloudinaryConfiguration["ApiKey"],
        cloudinaryConfiguration["ApiSecret"]
    );
    var cloudinary = new Cloudinary(account);
    cloudinary.Api.Secure = true;
    builder.Services.AddSingleton(cloudinary);

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
}

var app = builder.Build();
{
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await AdminSeeder.SeedAdminUserAsync(services);
    }

    app.UseHttpsRedirection();


    app.UseAntiforgery();

    app.MapStaticAssets();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
// Add additional endpoints required by the Identity /Account Razor components.

    app.MapAdditionalIdentityEndpoints();

    app.Run();
}