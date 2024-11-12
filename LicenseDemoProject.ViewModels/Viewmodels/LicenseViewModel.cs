using QlmLicenseLib;

namespace LicenseDemoProject.ViewModels.Viewmodels;

public class LicenseViewModel : ViewModelBase
{
    private LicenseValidator licenseValidator;
    private readonly string settingsFile;
    private readonly string computerKeyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "computerKey.txt");
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
        computerId = Environment.MachineName;
    }

    public Action<Tuple<bool, string>>? OnActivation { get; set; }

    public string LicenseKeyText { get; set; }

    public bool IsLoading { get; set; }

    public bool ValidateLicense()
    {
        bool needsActivation = false;
        string returnMsg = string.Empty;
        
        if (!File.Exists(computerKeyFilePath))
        {
            return false;
        }
        
        string storedKey = File.ReadAllText(computerKeyFilePath);
        var boolean = licenseValidator.ValidateLicense(string.Empty, storedKey, computerId, ref needsActivation, ref returnMsg);
        return boolean;
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

        if (tryActivateLicenseBoolean)
        {
            File.WriteAllText(computerKeyFilePath, licenseInfo.ComputerKey);
        }
        
        OnActivation?.Invoke(new Tuple<bool, string>(tryActivateLicenseBoolean, message));
    }
}