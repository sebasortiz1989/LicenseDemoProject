using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using LicenseDemoProject.ViewModels.Container;
using LicenseDemoProject.ViewModels.Viewmodels;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using LicenseDemoProjectViews.Assets;

namespace LicenseDemoProjectViews.Views;

public partial class LicenseView : UserControl
{
    private INotificationManager? _notificationManager;

    private INotificationManager NotificationManager => _notificationManager
        ??= new WindowNotificationManager(TopLevel.GetTopLevel(this)!) {Position = NotificationPosition.BottomRight};

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
            NotificationManager.Show(new Notification("Activation Failed", message));
        }
    }
}