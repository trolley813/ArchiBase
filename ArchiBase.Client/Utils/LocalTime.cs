using ArchiBase.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace ArchiBase.Components.Utils;

public sealed class LocalTime : ComponentBase, IDisposable
{
    [Inject]
    public BrowserTimeProvider TimeProvider { get; set; } = default!;

    [Parameter]
    public DateTime? DateTime { get; set; }

    protected override void OnInitialized()
    {
        TimeProvider.LocalTimeZoneChanged += LocalTimeZoneChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (DateTime != null)
        {
            builder.AddContent(0, TimeProvider.ToLocalDateTime(DateTime.Value));
        }
    }

    public void Dispose()
    {
        TimeProvider.LocalTimeZoneChanged -= LocalTimeZoneChanged;
    }

    private void LocalTimeZoneChanged(object? sender, EventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }
}