using System.Windows;
using System.Windows.Controls;

namespace Compiler.Interface;

public class CutBehavior : BaseBehavior<TextBox>
{
    public bool CutTrigger
    {
        get => (bool)GetValue(CutTriggerProperty);
        set
        {
            SetValue(CutTriggerProperty, value);
            RaisePropertyChanged();
        }
    }

    public static readonly DependencyProperty CutTriggerProperty = DependencyProperty.Register(nameof(CutTrigger), typeof(bool), typeof(CutBehavior), new PropertyMetadata(false, OnCutTriggerChanged));

    private static void OnCutTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CutBehavior behavior = d as CutBehavior;

        if (behavior != null)
            behavior.OnCutTriggerChanged();
    }

    private void OnCutTriggerChanged()
    {
        if (CutTrigger)
            AssociatedObject.Cut();
    }
}
