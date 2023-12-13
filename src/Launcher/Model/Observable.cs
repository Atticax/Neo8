using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Launcher.Model
{
    public class Observable<T> : INotifyPropertyChanged
    {
        T _value;
        Action _updateAction;

        public Observable(T initialValue, Action updateAction = null)
        {
            _value = initialValue;
            _updateAction = updateAction;
        }

        public T Value
        {
            get => _value;
            set
            {
                if ((value == null && _value != null) || !value.Equals(_value))
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                    _updateAction?.Invoke();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public void Bind(FrameworkElement element, DependencyProperty property)
        {
            var binding = new Binding
            {
                Path = new PropertyPath("Value"),
                Source = this,
                Mode = BindingMode.OneWay
            };
            element.SetBinding(property, binding);
        }
        public void BidirectionalBind(FrameworkElement element, DependencyProperty property)
        {
            var binding = new Binding
            {
                Path = new PropertyPath("Value"),
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            element.SetBinding(property, binding);
        }
        public void BidirectionalBind(Action<Action<T>> setChangeNotifyer, Action<T> setValue)
        {
            setChangeNotifyer(x => Value = x);
            PropertyChanged += (s, e) => setValue(Value);
        }

        /*public static implicit operator Observable<T>(T value)
        {
            return new Observable<T>((T) null) { Value = value };
        }*/
    }
}
