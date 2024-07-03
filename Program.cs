using Radzen;
using ArchiBase.Components;
using ArchiBase.Data;
using ArchiBase.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ArchiBase.Utils;
using ArchiBase.Models;
using EFMaterializedPath.Extensions;
using EFMaterializedPath;
using Archibase.Utils;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor().AddHubOptions(options =>
        {
            options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
        });

builder.Services.AddRadzenComponents();

builder.Services.AddScoped<BrowserTimeProvider>();

builder.Services.AddDbContext<ModelContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ArchitectureDatabase")));

builder.Services.AddSingleton<IIdentifierSerializer<Guid>, GuidIdentifierSerializer>();
builder.Services.AddTreeRepository<ModelContext, Location, Guid>();
builder.Services.AddTreeRepository<ModelContext, DesignCategory, Guid>();

builder.Services.AddDbContext<UsersContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ArchitectureDatabase")));

builder.Services.AddIdentity<ArchiBaseUser, ArchiBaseRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<UsersContext>()
    .AddDefaultTokenProviders()
    .AddUserConfirmation<UserConfirmation>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanEdit", policy => policy.RequireRole("Admin", "Editor", "Local Editor"));

builder.Services.AddSingleton<StateContainer>();

builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

builder.Services.AddScoped<IUserConfirmation<ArchiBaseUser>, UserConfirmation>();

builder.Services.AddTransient<UserResolverService>();
builder.Services.AddTransient<CommentService>();
builder.Services.AddScoped<CadastreRecordService>();

builder.Services.AddScoped<LocalEditorService>();
builder.Services.AddScoped<IAuthorizationHandler, LocalEditorRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, LocalEditorPolicyProvider>();


var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
if (emailConfig != null)
{
    builder.Services.AddSingleton(emailConfig);
    builder.Services.AddScoped<IEmailSender, EmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, NoOpEmailSender>();
}
builder.Services.AddScoped<IEmailSender<ArchiBaseUser>, UserEmailSender>();


builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromHours(4));

var app = builder.Build();

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

var supportedCultures = new[] { "en-US", "ru-RU", "et-EE", "pl-PL", "be-BY", "de-DE" };

app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
