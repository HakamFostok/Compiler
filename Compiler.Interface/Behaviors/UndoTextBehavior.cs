using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public class UndoBehavior : Behavior<TextBox>
    {
        public bool UndoTrigger
        {
            get { return (bool)GetValue(UndoTriggerProperty); }
            set { SetValue(UndoTriggerProperty, value); }
        }

        public static readonly DependencyProperty UndoTriggerProperty =
            DependencyProperty.Register(nameof(UndoTrigger), typeof(bool), typeof(UndoBehavior),
                new PropertyMetadata(false, OnUndoTriggerChanged));

        private static void OnUndoTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as UndoBehavior;

            if (behavior != null)
            {
                behavior.OnUndoTriggerChanged();
            }
        }
        private void OnUndoTriggerChanged()
        {
            // when closetrigger is true, close the window
            if (this.UndoTrigger)
                this.AssociatedObject.Undo();
        }
    }
}
