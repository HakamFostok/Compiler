using System.Windows;
using System.Windows.Controls;

namespace Compiler.Interface;

public class SelectAllTextBehavior : BaseBehavior<TextBox>
{
    public bool SelectTrigger
    {
        get => (bool)GetValue(SelectTriggerProperty);
        set
        {
            SetValue(SelectTriggerProperty, value);
            RaisePropertyChanged();
        }
    }

    public static readonly DependencyProperty SelectTriggerProperty = DependencyProperty.Register(nameof(SelectTrigger), typeof(bool), typeof(SelectAllTextBehavior), new PropertyMetadata(false, OnSelectTriggerChanged));

    protected override void OnAttached()
    {
        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        base.OnDetaching();
    }

    private void AssociatedObject_SelectionChanged(object sender, RoutedEventArgs e) => SelectTrigger = false;

    private static void OnSelectTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SelectAllTextBehavior behavior = d as SelectAllTextBehavior;

        if (behavior != null)
            behavior.OnSelectTriggerChanged();
    }

    private void OnSelectTriggerChanged()
    {
        if (SelectTrigger)
            AssociatedObject.SelectAll();
    }
}
