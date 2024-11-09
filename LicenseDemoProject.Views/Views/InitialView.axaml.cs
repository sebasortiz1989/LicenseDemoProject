using Avalonia.Controls;
using Avalonia.Input;

namespace LicenseDemoProjectViews.Views;

public partial class InitialView : UserControl
{
    public InitialView()
    {
        InitializeComponent();
    }

    public event Action<ViewsEnum>? ShowViewAction;

    private void LicenseViewButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ShowViewAction?.Invoke(ViewsEnum.LicenseView);
    }
}