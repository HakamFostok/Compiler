using System;
using System.Windows;
using System.Windows.Controls;

namespace Compiler.Interface
{

    public class RedoBehavior : BaseBehavior<TextBox>
    {
        public bool RedoTrigger
        {
            get { return (bool)GetValue(RedoTriggerProperty); }
            set
            {
                SetValue(RedoTriggerProperty, value);
                RaisePropertyChanged();
            }
        }

        public static readonly DependencyProperty RedoTriggerProperty = DependencyProperty.Register(nameof(RedoTrigger), typeof(bool), typeof(RedoBehavior), new PropertyMetadata(false, OnRedoTriggerChanged));

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
