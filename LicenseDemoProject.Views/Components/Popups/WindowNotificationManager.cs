using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace LicenseDemoProjectViews.Components;

[TemplatePart("PART_Items", typeof(Panel))]
[PseudoClasses(":topleft", ":topright", ":bottomleft", ":bottomright", ":topcenter", ":bottomcenter")]
public class WindowNotificationManager : TemplatedControl
{
    public static readonly StyledProperty<NotificationPosition> PositionProperty =
      AvaloniaProperty.Register<WindowNotificationManager, NotificationPosition>(nameof(Position), NotificationPosition.TopRight);

    public static readonly StyledProperty<int> MaxItemsProperty =
      AvaloniaProperty.Register<WindowNotificationManager, int>(nameof(MaxItems), 5);

    private readonly Dictionary<object, NotificationCard> notificationCards = new();
    private Controls items = new();

    static WindowNotificationManager()
    {
        HorizontalAlignmentProperty.OverrideDefaultValue<WindowNotificationManager>(HorizontalAlignment.Stretch);
        VerticalAlignmentProperty.OverrideDefaultValue<WindowNotificationManager>(VerticalAlignment.Stretch);
    }

    public WindowNotificationManager(TopLevel? host)
    {
        if (host is not null)
        {
            InstallFromTopLevel(host);
        }
    }

    public WindowNotificationManager()
    {
        UpdatePseudoClasses(Position);
    }

    public Controls Items => items;

    public string ToolTipMessage { get; set; } = string.Empty;

    public NotificationPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public int MaxItems
    {
        get => GetValue(MaxItemsProperty);
        set => SetValue(MaxItemsProperty, value);
    }

    public NotificationCard Show(INotification content)
    {
        if (content == null)
        {
            return new NotificationCard();
        }

        return Show(content, content.Type, content.Expiration, content.OnClose);
    }

    public NotificationCard Show(
        object content,
        NotificationType type,
        TimeSpan? expiration = null,
        Action? onClose = null)
    {
        Dispatcher.UIThread.VerifyAccess();

        var notificationControl = new NotificationCard
        {
            Content = content,
            NotificationType = type,
        };

        notificationControl.Loaded += (sender, args) =>
        {
            var closeButton = notificationControl.GetVisualDescendants().FirstOrDefault(x => x.Name != null && string.Equals(x.Name, "CloseButton", StringComparison.InvariantCulture));
            if (closeButton is Button closeButtonButton)
            {
                closeButtonButton.Tapped += (_, _) =>
                {
                    onClose?.Invoke();
                    ClosePopup(notificationControl);
                };
            }
        };

        items?.Add(notificationControl);
        notificationCards.Add(content, notificationControl);

        if (items?.OfType<NotificationCard>().Count(i => !i.IsClosing) > MaxItems)
        {
            items.OfType<NotificationCard>().First(i => !i.IsClosing).Close();
        }

        var miliseconds = (int?)expiration?.TotalMilliseconds;
        if (miliseconds > 0)
        {
            _ = Task.Run(
                async () =>
                {
                    await Task.Delay(miliseconds is null or 0 ? 3000 : miliseconds.Value, CancellationToken.None).ConfigureAwait(true);
                    onClose?.Invoke();
                    ClosePopup(notificationControl);
                });
        }

        return notificationControl;
    }

    public void ClosePopup(NotificationCard notificationControl)
    {
        Dispatcher.UIThread.Post(() =>
        {
            notificationControl.Close();
            if (notificationCards.ContainsValue(notificationControl))
            {
                notificationCards.Remove(notificationControl);
            }

            if (items.Contains(notificationControl))
            {
                items.Remove(notificationControl);
            }
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (e == null)
        {
            return;
        }

        base.OnApplyTemplate(e);

        var itemsControl = e.NameScope.Find<Panel>("PART_Items");
        if (itemsControl != null)
        {
            items = itemsControl.Children;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change == null)
        {
            return;
        }

        base.OnPropertyChanged(change);

        if (change.Property == PositionProperty)
        {
            UpdatePseudoClasses(change.GetNewValue<NotificationPosition>());
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        notificationCards.Clear();
    }

    private void InstallFromTopLevel(TopLevel topLevel)
    {
        topLevel.TemplateApplied += TopLevelOnTemplateApplied;
        var adorner = topLevel.FindDescendantOfType<VisualLayerManager>()?.AdornerLayer;
        if (adorner is not null)
        {
            adorner.Children.Add(this);
            AdornerLayer.SetAdornedElement(this, adorner);
        }
    }

    private void TopLevelOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        if (Parent is AdornerLayer adornerLayer)
        {
            adornerLayer.Children.Remove(this);
            AdornerLayer.SetAdornedElement(this, this);
        }

        var topLevel = (TopLevel)sender!;
        topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
        InstallFromTopLevel(topLevel);
    }

    private void UpdatePseudoClasses(NotificationPosition position)
    {
        PseudoClasses.Set(":topleft", position == NotificationPosition.TopLeft);
        PseudoClasses.Set(":topright", position == NotificationPosition.TopRight);
        PseudoClasses.Set(":bottomleft", position == NotificationPosition.BottomLeft);
        PseudoClasses.Set(":bottomright", position == NotificationPosition.BottomRight);
        PseudoClasses.Set(":topcenter", position == NotificationPosition.TopCenter);
        PseudoClasses.Set(":bottomcenter", position == NotificationPosition.BottomCenter);
    }
}
