using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public class RedoBehavior : Behavior<TextBox>
    {
        public bool RedoTrigger
        {
            get { return (bool)GetValue(RedoTriggerProperty); }
            set { SetValue(RedoTriggerProperty, value); }
        }

        public static readonly DependencyProperty RedoTriggerProperty =
            DependencyProperty.Register(nameof(RedoTrigger), typeof(bool), typeof(RedoBehavior),
                new PropertyMetadata(false, OnRedoTriggerChanged));

        private static void OnRedoTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as RedoBehavior;

            if (behavior != null)
            {
                behavior.OnRedoTriggerChanged();
            }
        }
        private void OnRedoTriggerChanged()
        {
            // when closetrigger is true, close the window
            if (this.RedoTrigger)
                this.AssociatedObject.Redo();
        }
    }
}
