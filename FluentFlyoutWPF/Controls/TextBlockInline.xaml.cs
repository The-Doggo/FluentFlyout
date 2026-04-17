using System.Windows;
using System.Windows.Controls;

namespace FluentFlyout.Controls;

public partial class TextBlockInline : UserControl
{
    public TextBlockInline()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBlockInline), new PropertyMetadata(string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty Inline =
        DependencyProperty.Register(nameof(InlineContent), typeof(UIElement), typeof(TextBlockInline), new PropertyMetadata(null));

    public UIElement InlineContent
    {
        get => (UIElement)GetValue(Inline);
        set => SetValue(Inline, value);
    }

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(TextBlockInline), new PropertyMetadata(TextWrapping.WrapWithOverflow));

    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }
}