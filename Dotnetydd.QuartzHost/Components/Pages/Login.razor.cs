using Dotnetydd.QuartzHost.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace Dotnetydd.QuartzHost.Components.Pages;

public partial class Login : IAsyncDisposable
{
    private IJSObjectReference _jsModule;
    private MudButton _tokenTextField;
    private LoginVm _formModel;

    [Inject]
    public required NavigationManager NavigationManager { get; init; }
    [Inject]
    public required IJSRuntime JS { get; set; }


    [Parameter]
    [SupplyParameterFromQuery]
    public string ReturnUrl { get; set; }


    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is { } authStateTask)
        {
            var state = await authStateTask;
            if (state.User.Identity?.IsAuthenticated ?? false)
            {
                NavigationManager.NavigateTo(GetRedirectUrl(), forceLoad: true);
                return;
            }
        }
        _formModel = new LoginVm();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Dotnetydd.QuartzHost/js/login.js");
            _tokenTextField?.FocusAsync();
        }
    }


    private async Task SubmitAsync()
    {
        if (_jsModule is null)
        {
            return;
        }

        var result = await _jsModule.InvokeAsync<string>("validateToken", _formModel.Token);

        if (bool.TryParse(result, out var success))
        {
            if (success)
            {
                NavigationManager.NavigateTo(GetRedirectUrl(), forceLoad: true);
                return;
            }
        }
        else
        {
        }
    }

    private string GetRedirectUrl()
    {
        return ReturnUrl ?? "/";
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            try
            {
                await _jsModule.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {

            }
        }
    }
}