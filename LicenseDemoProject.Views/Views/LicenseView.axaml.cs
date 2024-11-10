using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using LicenseDemoProject.ViewModels.Container;
using LicenseDemoProject.ViewModels.Viewmodels;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia;
using Avalonia.Interactivity;
using LicenseDemoProjectViews.Assets;
using QlmLicenseLib;

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

        MyImage.Source = ByteArrayToBitmap(LicenseResources.KeyIconImg);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (_viewModel != null && _viewModel.ValidateLicense())
        {
            ShowViewAction?.Invoke(ViewsEnum.LicenseActivatedView);
        }
    }

    public event Action<ViewsEnum>? ShowViewAction;

    private static Bitmap ByteArrayToBitmap(byte[] byteArray)
    {
        using var ms = new MemoryStream(byteArray);
        return new Bitmap(ms);
    }

    private void GoBackButton_OnTapped(object? sender, TappedEventArgs e)
    {
        ShowViewAction?.Invoke(ViewsEnum.InitialView);
    }

    private void UnlockButton_OnTapped(object? sender, TappedEventArgs e)
    {
        string message = string.Empty;
        if (_viewModel != null && _viewModel.TryActivateLicense(out message))
        {
            ShowViewAction?.Invoke(ViewsEnum.LicenseActivatedView);
        }
        else
        {
            string text = "Activation failed. " + message;
            // Show popup with text
        }
    }
}