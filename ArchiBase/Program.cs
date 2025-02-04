using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ArchiBase.Components;
using ArchiBase.Data;
using ArchiBase.Users;
using ArchiBase.Utils;
using Archibase.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using EFMaterializedPath;
using EFMaterializedPath.Extensions;
using ArchiBase.Models;
using System.Text.Json.Serialization;
using Radzen;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Blazor.WebAssembly.DynamicCulture.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddRadzenComponents();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddOpenApi();

builder.Services.AddAuthentication()
    .AddCookie();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddResponseCaching();
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default",
        new CacheProfile()
        {
            Duration = 60
        });
    options.CacheProfiles.Add("Short",
        new CacheProfile()
        {
            Duration = 10
        });
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
});

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ModelContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ArchitectureDatabase"), npgsqlOptions => npgsqlOptions.CommandTimeout(600))
           .UseTriggers(triggerOptions =>
            {
                triggerOptions.AddTrigger<SaveActualCardTrigger>();
                triggerOptions.AddTrigger<SaveBuildingTrigger>();
                triggerOptions.AddTrigger<CascadeOwnedChangesTrigger>();
            }));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<IIdentifierSerializer<Guid>, GuidIdentifierSerializer>();
builder.Services.AddTreeRepository<ModelContext, Location, Guid>();
builder.Services.AddTreeRepository<ModelContext, DesignCategory, Guid>();

var mapperConfig = new MapperConfiguration(mc =>
     {
         mc.AddProfile(new MappingProfile());
     });

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddDbContext<UsersContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ArchitectureDatabase"));
});

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
if (emailConfig != null)
{
    builder.Services.AddSingleton(emailConfig);
    builder.Services.AddSingleton<IEmailSender, EmailSender>();
}
else
{
    builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();
}

builder.Services.AddSingleton<IEmailSender<ArchiBaseUser>, UserEmailSender>();

builder.Services.AddIdentity<ArchiBaseUser, ArchiBaseRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddApiEndpoints()
    .AddEntityFrameworkStores<UsersContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddUserConfirmation<UserConfirmation>();

builder.Services.AddScheduler(ctx =>
{
    ctx.AddJob<PhotoApprovalJob>(configure: options =>
    {
        options.CronSchedule = "0 0 0 * * *";
        options.CronTimeZone = "UTC";
        options.RunImmediately = true;
    });
    ctx.AddJob<PhotoCleanupJob>(configure: options =>
    {
        options.CronSchedule = "0 0 0 1 * *";
        options.CronTimeZone = "UTC";
        options.RunImmediately = true;
    });
});

builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

builder.Services.AddScoped<IUserConfirmation<ArchiBaseUser>, UserConfirmation>();

builder.Services.AddTransient<UserResolverService>();
builder.Services.AddTransient<CommentService>();
builder.Services.AddScoped<UploadLimitService>();
builder.Services.AddScoped<CadastreRecordService>();
builder.Services.AddScoped<ThumbnailService>();
builder.Services.AddScoped<StreetNameService>();

builder.Services.AddScoped<LocalEditorService>();
builder.Services.AddScoped<IAuthorizationHandler, LocalEditorRequirementHandlerServer>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, LocalEditorPolicyProvider>();

builder.Services.AddHttpClient();

var app = builder.Build();

// check migrations and apply them
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var modelContext = services.GetRequiredService<ModelContext>();
    if (modelContext.Database.GetPendingMigrations().Any())
    {
        modelContext.Database.Migrate();
    }

    var usersContext = services.GetRequiredService<UsersContext>();
    if (usersContext.Database.GetPendingMigrations().Any())
    {
        usersContext.Database.Migrate();
    }

    await SeedData.InitializeAsync(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseResponseCaching();

app.UseStaticFiles();
//app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ArchiBase.Client._Imports).Assembly);

app.MapControllers().WithOpenApi();

// Map Identity API with /api/account route prefix
app.MapGroup("/api/account").MapCustomIdentityApi().WithOpenApi();

// Enables the user to log out
app.MapPost("/api/account/logout", async (SignInManager<ArchiBaseUser> signInManager,
    [FromBody] object empty) =>
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
})
.WithOpenApi()
.RequireAuthorization();

app.MapOpenApi("/openapi/v1/openapi.json");

app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
    });

// Add additional endpoints required by the Identity /Account Razor components.
// app.MapAdditionalIdentityEndpoints();

app.Run();
