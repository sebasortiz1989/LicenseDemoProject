using LicenseDemoProject.Services;
using LicenseDemoProject.ViewModels.Viewmodels;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseDemoProject.ViewModels.Container;

public class ViewModelProvider
{
    private static ViewModelProvider? instance;
    private ServiceCollection services = new();
    private ServiceProvider? serviceProvider;

    public ViewModelProvider()
    {
        RegisterServices();
        RegisterViewModels();
        serviceProvider = services.BuildServiceProvider();
    }

    public static ViewModelProvider Instance => instance ??= new ViewModelProvider();

    public ViewModelBase? GetViewModel(Type viewModelType)
    {
        var viewModel = serviceProvider?.GetService(viewModelType);
        if (viewModel is ViewModelBase viewModelSingleton)
        {
            return viewModelSingleton;       
        }

        return null;
    }
    
    private void RegisterViewModels()
    {
        services.AddSingleton<MainViewModel, MainViewModel>();
        services.AddSingleton<LicenseViewModel, LicenseViewModel>();
    }

    private void RegisterServices()
    {
        services.AddSingleton<LicenseValidator, LicenseValidator>();
    }
}