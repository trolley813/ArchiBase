using System.Globalization;
using ArchiBase.Users;
using ArchiBase.Utils;
using Blazor.WebAssembly.DynamicCulture.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddRadzenComponents();

builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
builder.Services.AddLocalizationDynamic(options =>
    {
        options.SetDefaultCulture("en-US");
        options.AddSupportedCultures("ru", "et", "pl", "be", "de");
        options.AddSupportedUICultures("ru", "et", "pl", "be", "de");
    });

builder.Services.AddScoped<BrowserTimeProvider>();

builder.Services.AddSingleton<IAuthorizationHandler, LocalEditorRequirementHandlerClient>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, LocalEditorPolicyProvider>();

builder.Services.AddHttpClient("", options =>
{
    options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

var host = builder.Build();

await host.SetMiddlewareCulturesAsync();

await host.RunAsync();
