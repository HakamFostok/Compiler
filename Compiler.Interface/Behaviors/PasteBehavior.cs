using System.Windows;
using System.Windows.Controls;

namespace Compiler.Interface;

public class PasteBehavior : BaseBehavior<TextBox>
{
    public bool PasteTrigger
    {
        get => (bool)GetValue(PasteTriggerProperty);
        set
        {
            SetValue(PasteTriggerProperty, value);
            RaisePropertyChanged();
        }
    }

    public static readonly DependencyProperty PasteTriggerProperty = DependencyProperty.Register(nameof(PasteTrigger), typeof(bool), typeof(PasteBehavior), new PropertyMetadata(false, OnPasteTriggerChanged));

    private static void OnPasteTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        PasteBehavior behavior = d as PasteBehavior;

        if (behavior != null)
            behavior.OnPasteTriggerChanged();
    }

    private void OnPasteTriggerChanged()
    {
        if (this.PasteTrigger)
            this.AssociatedObject.Paste();
    }
}
