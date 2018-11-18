using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public class DeselectAllTextBehavior : Behavior<TextBox>
    {
        public bool DeselectTrigger
        {
            get { return (bool)GetValue(DeselectTriggerProperty); }
            set { SetValue(DeselectTriggerProperty, value); }
        }

        public static readonly DependencyProperty DeselectTriggerProperty =
            DependencyProperty.Register(nameof(DeselectTrigger), typeof(bool), typeof(DeselectAllTextBehavior),
                new PropertyMetadata(false, OnDeselectTriggerChanged));

        private static void OnDeselectTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DeselectAllTextBehavior;

            if (behavior != null)
            {
                behavior.OnDeselectTriggerChanged();
            }
        }

        private void OnDeselectTriggerChanged()
        {
            // when closetrigger is true, close the window
            if (this.DeselectTrigger)
                this.AssociatedObject.Select(0, 0);
        }
    }
}
