using Avalonia;
using Avalonia.Controls;
using LicenseDemoProjectViews.Container;

namespace LicenseDemoProjectViews.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        ((InitialView)ViewProvider.Instance.GetView(typeof(InitialView))!).ShowViewAction += ShowScreenActionFunction;
        ((LicenseView)ViewProvider.Instance.GetView(typeof(LicenseView))!).ShowViewAction += ShowScreenActionFunction;
        ShowScreenActionFunction(ViewsEnum.InitialView);
    }

    private void ShowScreenActionFunction(ViewsEnum view)
    {
        switch (view)
        {
            case ViewsEnum.InitialView:
                ContentView.Content = ViewProvider.Instance.GetView(typeof(InitialView));
                break;
            case ViewsEnum.LicenseView:
                ContentView.Content = ViewProvider.Instance.GetView(typeof(LicenseView));
                break;
        }
    }
}