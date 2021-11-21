using System.Windows;
using System.Windows.Controls;

namespace Compiler.Interface;

public class UndoBehavior : BaseBehavior<TextBox>
{
    public bool UndoTrigger
    {
        get => (bool)GetValue(UndoTriggerProperty);
        set
        {
            SetValue(UndoTriggerProperty, value);
            RaisePropertyChanged();
        }
    }

    public static readonly DependencyProperty UndoTriggerProperty = DependencyProperty.Register(nameof(UndoTrigger), typeof(bool), typeof(UndoBehavior), new PropertyMetadata(false, OnUndoTriggerChanged));

    private static void OnUndoTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UndoBehavior behavior = d as UndoBehavior;

        if (behavior != null)
            behavior.OnUndoTriggerChanged();
    }

    private void OnUndoTriggerChanged()
    {
        if (UndoTrigger)
            AssociatedObject.Undo();
    }
}
