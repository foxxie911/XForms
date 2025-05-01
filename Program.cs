using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using XForms.Authorization;
using XForms.Components;
using XForms.Components.Account;
using XForms.Data;
using XForms.Hubs;
using XForms.Seeder;
using XForms.Services.Implementation;
using XForms.Services.Interface;
using XForms.Services.SignalR;

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
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
    /*
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
    */
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // Custom Service
    builder.Services.AddScoped<ITemplateService, TemplateService>();
    builder.Services.AddScoped<TemplateService>();
    builder.Services.AddScoped<IQuestionService, QuestionService>();
    builder.Services.AddScoped<QuestionService>();
    builder.Services.AddScoped<ISearchService, SearchService>();
    builder.Services.AddScoped<SearchService>();
    builder.Services.AddScoped<IFormService, FormService>();
    builder.Services.AddScoped<FormService>();
    builder.Services.AddScoped<IAnswerService, AnswerService>();
    builder.Services.AddScoped<AnswerService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<CommentService>();
    builder.Services.AddScoped<CommentSignalRService>();
    builder.Services.AddScoped<ILikeService, LikeService>();
    builder.Services.AddScoped<LikeService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<TagService>();

    builder.Services.AddIdentityCore<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = false; })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();
    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
    builder.Services.AddScoped<IAuthorizationHandler, TemplateEditorHandler>();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("TemplateEditor", policy =>
            policy.Requirements.Add(new TemplateEditorRequirement()));
    });

    var cloudinaryConfiguration = builder.Configuration.GetSection("Cloudinary");
    var account = new Account
    (
        cloudinaryConfiguration["CloudName"],
        cloudinaryConfiguration["ApiKey"],
        cloudinaryConfiguration["ApiSecret"]
    );
    var cloudinary = new Cloudinary(account)
    {
        Api =
        {
            Secure = true
        }
    };
    builder.Services.AddSingleton(cloudinary);

    builder.Services.AddSignalR();
    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            ["application/octet-stream"]);
    });

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddControllers();
}

var app = builder.Build();
{
    app.UseResponseCompression();

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

    app.MapHub<CommentHub>("/commentHub");

    app.Run();
}