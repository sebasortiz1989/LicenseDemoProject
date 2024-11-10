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
    private string activationKey = string.Empty;
    private string computerKey = string.Empty;
    private string webServiceUrl = string.Empty;
    private string computerId = "01";

    private string qlmVersion = "5.0.00";
    private string qlmUserData = string.Empty;

    bool activationResult = false;

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
 
        bool needsActivation = false;
        string errorMsg = string.Empty;
        if (_viewModel?.LicenseValidator.ValidateLicenseAtStartup(Environment.MachineName, ref needsActivation, ref errorMsg) == false)
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
        if (_viewModel == null)
            return;

        _viewModel.LicenseValidator.QlmLicenseObject.ActivateLicense(
            webServiceUrl,
            LicenseKeyTextBox.Text,
            computerId,
            Environment.MachineName,
            qlmVersion,
            qlmUserData,
            out var response);

        ILicenseInfo licenseInfo = new LicenseInfo();
        string message = string.Empty;
        if (_viewModel.LicenseValidator.QlmLicenseObject.ParseResults(response, ref licenseInfo, ref message))
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