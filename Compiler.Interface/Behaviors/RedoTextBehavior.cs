using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public class RedoBehavior : BaseBehavior<TextBox>
    {
        public bool RedoTrigger
        {
            get { return (bool)GetValue(RedoTriggerProperty); }
            set { SetValue(RedoTriggerProperty, value); }
        }

        public static readonly DependencyProperty RedoTriggerProperty =
            DependencyProperty.Register(nameof(RedoTrigger), typeof(bool), typeof(RedoBehavior),
                new PropertyMetadata(false, OnRedoTriggerChanged));

        //protected override void OnAttached()
        //{
        //    this.AssociatedObject.undo += AssociatedObject_TextChanged; ;
        //    base.OnAttached();
        //}

        //protected override void OnDetaching()
        //{
        //    this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        //    base.OnDetaching();
        //}

        //private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    this.ClearTextTrigger = false;
        //}

        private static void OnRedoTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RedoBehavior behavior = d as RedoBehavior;

            if (behavior != null)
                behavior.OnRedoTriggerChanged();
        }

        private void OnRedoTriggerChanged()
        {
            if (this.RedoTrigger)
                this.AssociatedObject.Redo();
        }
    }
}
