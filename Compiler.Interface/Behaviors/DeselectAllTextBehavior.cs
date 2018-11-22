using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public class DeselectAllTextBehavior : BaseBehavior<TextBox>
    {
        public bool DeselectTrigger
        {
            get { return (bool)GetValue(DeselectTriggerProperty); }
            set
            {
                SetValue(DeselectTriggerProperty, value);
                RaisePropertyChanged();
            }
        }

        public static readonly DependencyProperty DeselectTriggerProperty = DependencyProperty.Register(nameof(DeselectTrigger), typeof(bool), typeof(DeselectAllTextBehavior), new PropertyMetadata(false, OnDeselectTriggerChanged));

        protected override void OnAttached()
        {
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
            base.OnDetaching();
        }

        private void AssociatedObject_SelectionChanged(object sender, RoutedEventArgs e)
        {
            this.DeselectTrigger = false;
        }

        private static void OnDeselectTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DeselectAllTextBehavior;

            if (behavior != null)
                behavior.OnDeselectTriggerChanged();
        }

        private void OnDeselectTriggerChanged()
        {
            if (this.DeselectTrigger)
                this.AssociatedObject.Select(0, 0);
        }
    }
}
