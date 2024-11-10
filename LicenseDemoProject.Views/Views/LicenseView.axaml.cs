using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using LicenseDemoProject.ViewModels.Container;
using LicenseDemoProject.ViewModels.Viewmodels;
using Avalonia.Media.Imaging;
using System.IO;
using LicenseDemoProjectViews.Assets;

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
        // If license unlocked
        ShowViewAction?.Invoke(ViewsEnum.LicenseActivatedView);
    }
}