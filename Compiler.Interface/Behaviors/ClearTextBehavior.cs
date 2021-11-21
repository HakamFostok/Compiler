using System.Windows;
using System.Windows.Controls;

namespace Compiler.Interface;

public class ClearTextBehavior : BaseBehavior<TextBox>
{
    public bool ClearTextTrigger
    {
        get => (bool)GetValue(ClearTextTriggerProperty);
        set
        {
            SetValue(ClearTextTriggerProperty, value);
            RaisePropertyChanged();
        }
    }

    public static readonly DependencyProperty ClearTextTriggerProperty = DependencyProperty.Register(nameof(ClearTextTrigger), typeof(bool), typeof(ClearTextBehavior), new PropertyMetadata(false, OnClearTextTriggerChanged));

    protected override void OnAttached()
    {
        AssociatedObject.TextChanged += AssociatedObject_TextChanged; ;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        base.OnDetaching();
    }

    private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e) => ClearTextTrigger = false;

    private static void OnClearTextTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ClearTextBehavior behavior = d as ClearTextBehavior;

        if (behavior != null)
            behavior.OnClearTextTriggerChanged();
    }

    private void OnClearTextTriggerChanged()
    {
        if (ClearTextTrigger)
            AssociatedObject.Clear();
    }
}
