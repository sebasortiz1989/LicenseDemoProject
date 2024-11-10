using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace LicenseDemoProjectViews.Views;

public partial class LicenseActivatedView : UserControl
{
    public LicenseActivatedView()
    {
        InitializeComponent();
    }

    public event Action<ViewsEnum>? ShowViewAction;

    private void GoBackButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ShowViewAction?.Invoke(ViewsEnum.LicenseView);
    }
}