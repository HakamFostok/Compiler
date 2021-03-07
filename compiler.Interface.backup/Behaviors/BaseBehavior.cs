using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interactivity;

namespace Compiler.Interface
{
    public abstract class BaseBehavior<T> : Behavior<T>, INotifyPropertyChanged where T : DependencyObject
    {
        protected BaseBehavior()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
       
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}
