using LicenseDemoProject.Services;
using QlmLicenseLib;

namespace LicenseDemoProject.ViewModels.Viewmodels;

public class LicenseViewModel : ViewModelBase
{
    private readonly LicenseValidator licenseValidator;
    private string webServiceUrl = string.Empty;
    private string computerId = "01ImmoPro";
    private string qlmVersion = "5.0.00";
    private string qlmUserData = string.Empty;
    private bool activationResult = false;

    public LicenseViewModel(LicenseValidator licenseValidator)
    {
        this.licenseValidator = licenseValidator;
    }

    public Action<Tuple<bool, string>>? OnActivation { get; set; }

    public string LicenseKeyText { get; set; }

    public bool IsLoading { get; set; }

    public bool ValidateLicense()
    {
        bool needsActivation = false;
        string errorMsg = string.Empty;
        return !licenseValidator.ValidateLicenseAtStartup(Environment.MachineName, ref needsActivation, ref errorMsg);
    }

    public void TryActivateLicense()
    {
        licenseValidator.QlmLicenseObject.ActivateLicense(
            webServiceUrl,
            LicenseKeyText,
            Environment.MachineName,
            Environment.MachineName,
            qlmVersion,
            qlmUserData,
            out var response);

        ILicenseInfo licenseInfo = new LicenseInfo();
        string message = string.Empty;
        var tryActivateLicenseBoolean = licenseValidator.QlmLicenseObject.ParseResults(response, ref licenseInfo, ref message);
        OnActivation?.Invoke(new Tuple<bool, string>(tryActivateLicenseBoolean, message));
    }
}