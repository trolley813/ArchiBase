using ArchiBase.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ArchiBase.Components.Utils;

public sealed class InitializeTimeZone : ComponentBase
{
    [Inject] public BrowserTimeProvider TimeProvider { get; set; } = default!;
    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !TimeProvider.IsLocalTimeZoneSet)
        {
            try
            {
                await using var module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./timezone.js");
                var timeZone = await module.InvokeAsync<string>("getBrowserTimeZone");
                TimeProvider.SetBrowserTimeZone(timeZone);
            }
            catch (JSDisconnectedException)
            {
            }
        }
    }
}