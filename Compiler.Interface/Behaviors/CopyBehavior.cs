using System.Windows;
using System.Windows.Controls;

namespace Compiler.Interface
{
    public class CopyBehavior : BaseBehavior<TextBox>
    {
        public bool CopyTrigger
        {
            get { return (bool)GetValue(CopyTriggerProperty); }
            set
            {
                SetValue(CopyTriggerProperty, value);
                RaisePropertyChanged();
            }
        }

        public static readonly DependencyProperty CopyTriggerProperty = DependencyProperty.Register(nameof(CopyTrigger), typeof(bool), typeof(CopyBehavior), new PropertyMetadata(false, OnCopyTriggerChanged));

        private static void OnCopyTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CopyBehavior behavior = d as CopyBehavior;

            if (behavior != null)
                behavior.OnCopyTriggerChanged();
        }

        private void OnCopyTriggerChanged()
        {
            if (this.CopyTrigger)
                this.AssociatedObject.Copy();
        }
    }
}
