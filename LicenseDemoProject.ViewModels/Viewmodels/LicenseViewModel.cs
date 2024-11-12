using QlmLicenseLib;

namespace LicenseDemoProject.ViewModels.Viewmodels;

public class LicenseViewModel : ViewModelBase
{
    private LicenseValidator licenseValidator;
    private readonly string settingsFile;
    private string webServiceUrl = string.Empty;
    private string computerId = "01ImmoPro";
    private string qlmVersion = "5.0.00";
    private string qlmUserData = string.Empty;
    private bool activationResult = false;

    public LicenseViewModel()
    {
        var assembly = typeof(LicenseViewModel).Assembly;
        string fileName = "Demo Enterprise 1.0.lw.xml";
        string assemblyLocation = assembly.Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        string solutionDirectory = Directory.GetParent(assemblyDirectory).Parent.Parent.Parent.FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "LicenseDemoProject.ViewModels");
        settingsFile = Path.Combine(solutionDirectory, fileName);
        bool exists = File.Exists(settingsFile);
        licenseValidator = new LicenseValidator(settingsFile);
        // licenseValidator = new LicenseValidator(string.Empty, ResourcesLicenseDemo.XmlSettings);
        computerId = Environment.MachineName;
    }

    public Action<Tuple<bool, string>>? OnActivation { get; set; }

    public string LicenseKeyText { get; set; }

    public bool IsLoading { get; set; }

    public bool ValidateLicense()
    {
        bool needsActivation = false;
        string returnMsg = string.Empty;
        var validateLicenseAtStartup = licenseValidator.ValidateLicenseAtStartup(ELicenseBinding.ComputerName, ref needsActivation, ref returnMsg);
        return validateLicenseAtStartup;
    }

    public void TryActivateLicense()
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
        string message = string.Empty;
        var tryActivateLicenseBoolean = licenseValidator.QlmLicenseObject.ParseResults(response, ref licenseInfo, ref message);
        OnActivation?.Invoke(new Tuple<bool, string>(tryActivateLicenseBoolean, message));
    }
}