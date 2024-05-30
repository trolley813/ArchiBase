using Radzen;
using ArchiBase.Components;
using ArchiBase.Data;
using ArchiBase.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ArchiBase.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddDbContext<ModelContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ArchitectureDatabase")));

builder.Services.AddDbContext<UsersContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ArchitectureDatabase")));

builder.Services.AddIdentity<ArchiBaseUser, ArchiBaseRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<UsersContext>()
    .AddDefaultTokenProviders()
    .AddUserConfirmation<UserConfirmation>();

builder.Services.AddSingleton<StateContainer>();

builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

builder.Services.AddScoped<IUserConfirmation<ArchiBaseUser>, UserConfirmation>();

builder.Services.AddTransient<UserResolverService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
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

var supportedCultures = new[] { "en-US", "ru-RU", "et-EE", "pl-PL" };

app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
