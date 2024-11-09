using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using LicenseDemoProject.ViewModels.Container;
using LicenseDemoProject.ViewModels.Viewmodels;

namespace LicenseDemoProjectViews.Views;

public partial class LicenseView : UserControl
{
    private LicenseViewModel? _viewModel;

    public LicenseView()
    {
        DataContext = ViewModelProvider.Instance.GetViewModel(typeof(LicenseViewModel));
        if (DataContext is LicenseViewModel testViewModel)
            _viewModel = testViewModel;

        InitializeComponent();
    }

    public event Action<ViewsEnum>? ShowViewAction;

    private void GoBackButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ShowViewAction?.Invoke(ViewsEnum.InitialView);
    }
}