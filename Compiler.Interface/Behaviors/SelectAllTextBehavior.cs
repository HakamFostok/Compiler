using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public class SelectAllTextBehavior : Behavior<TextBox>
    {
        public bool SelectTrigger
        {
            get { return (bool)GetValue(SelectTriggerProperty); }
            set { SetValue(SelectTriggerProperty, value); }
        }

        public static readonly DependencyProperty SelectTriggerProperty =
            DependencyProperty.Register(nameof(SelectTrigger), typeof(bool), typeof(SelectAllTextBehavior),
                new PropertyMetadata(false, OnSelectTriggerChanged));

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
            this.SelectTrigger = false;
        }

        private static void OnSelectTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as SelectAllTextBehavior;

            if (behavior != null)
            {
                behavior.OnSelectTriggerChanged();
            }
        }
        private void OnSelectTriggerChanged()
        {
            // when closetrigger is true, close the window
            if (this.SelectTrigger)
            {
                this.AssociatedObject.SelectAll();
                this.SelectTrigger = false;
            }
        }
    }
}
