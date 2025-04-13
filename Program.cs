using CloudinaryDotNet;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using XForms.Components;
using XForms.Components.Account;
using XForms.Data;
using XForms.Seeder;
using XForms.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddMudServices();
    builder.Services.AddMudMarkdownServices();
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
    
    // Custom Service
    builder.Services.AddScoped<TemplateService>();

    builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
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
    builder.Services.AddControllers();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await AdminSeeder.SeedAdminUserAsync(services);
        await UserSeeder.SeedUserAsync(services);
    }

    app.UseHttpsRedirection();
    app.UseAntiforgery();
    app.MapStaticAssets();
    
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
    app.MapAdditionalIdentityEndpoints();
    app.Run();
}