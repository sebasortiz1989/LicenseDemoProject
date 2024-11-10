using LicenseDemoProject.Services;
using QlmLicenseLib;

namespace LicenseDemoProject.ViewModels.Viewmodels;

public class LicenseViewModel : ViewModelBase
{
    private readonly LicenseValidator licenseValidator;
    private string webServiceUrl = string.Empty;
    private string computerId = "01";
    private string qlmVersion = "5.0.00";
    private string qlmUserData = string.Empty;
    private bool activationResult = false;

    public LicenseViewModel(LicenseValidator licenseValidator)
    {
        this.licenseValidator = licenseValidator;
    }
    
    public string LicenseKeyText { get; set; }

    public bool ValidateLicense()
    {
        bool needsActivation = false;
        string errorMsg = string.Empty;
        return licenseValidator.ValidateLicenseAtStartup(Environment.MachineName, ref needsActivation, ref errorMsg) == false;
    }

    public bool TryActivateLicense(out string message)
    {
        licenseValidator.QlmLicenseObject.ActivateLicense(
            webServiceUrl,
            LicenseKeyText,
            computerId,
            Environment.MachineName,
            qlmVersion,
            qlmUserData,
            out var response);

        ILicenseInfo licenseInfo = new LicenseInfo();
        message = string.Empty;
        return licenseValidator.QlmLicenseObject.ParseResults(response, ref licenseInfo, ref message);
    }
}