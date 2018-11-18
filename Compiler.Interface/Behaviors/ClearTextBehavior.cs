using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public class ClearTextBehavior : Behavior<TextBox>
    {
        public bool ClearTextTrigger
        {
            get { return (bool)GetValue(ClearTextTriggerProperty); }
            set { SetValue(ClearTextTriggerProperty, value); }
        }

        public static readonly DependencyProperty ClearTextTriggerProperty =
            DependencyProperty.Register(nameof(ClearTextTrigger), typeof(bool), typeof(ClearTextBehavior),
                new PropertyMetadata(false, OnClearTextTriggerChanged));

        private static void OnClearTextTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as ClearTextBehavior;

            if (behavior != null)
            {
                behavior.OnClearTextTriggerChanged();
            }
        }
        private void OnClearTextTriggerChanged()
        {
            // when closetrigger is true, close the window
            if (this.ClearTextTrigger)
                this.AssociatedObject.Clear();
        }
    }
}
