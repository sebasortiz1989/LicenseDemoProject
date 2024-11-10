using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace LicenseDemoProjectViews;

public partial class CustomComponents : Styles
{
    public CustomComponents()
    {
        AvaloniaXamlLoader.Load(this);
    }
}