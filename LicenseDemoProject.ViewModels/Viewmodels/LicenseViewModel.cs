using LicenseDemoProject.Services;

namespace LicenseDemoProject.ViewModels.Viewmodels;

public class LicenseViewModel : ViewModelBase
{
    public LicenseViewModel(LicenseValidator licenseValidator)
    {
        LicenseValidator = licenseValidator;
    }

    public LicenseValidator LicenseValidator { get; }
}