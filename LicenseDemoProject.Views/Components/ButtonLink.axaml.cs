using Avalonia;
using Avalonia.Controls;

namespace LicenseDemoProjectViews.Components;

public class ButtonLink : Button
{
    private string? linkText = "Login";

    public static readonly DirectProperty<ButtonLink, string?> LinkTextProperty =
        AvaloniaProperty.RegisterDirect<ButtonLink, string?>(
            nameof(LinkText),
            o => o.LinkText,
            (o, v) => o.LinkText = v);

    public string? LinkText
    {
        get => linkText;
        set => SetAndRaise(LinkTextProperty, ref linkText, value);
    }
}