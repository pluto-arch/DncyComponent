namespace Dotnetydd.QuartzHost.Components.Layout;


public partial class MainLayout
{
    

    private bool _expanded = true;

    private void ToggleNavMenu()
    {
        _expanded = !_expanded;
    }


}