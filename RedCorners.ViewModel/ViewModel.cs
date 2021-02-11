using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Linq;

namespace RedCorners
{
    public class Command : ICommand
    {
        private readonly Action _action;
        private readonly bool _canExecute;
        public Command(Action action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class NoUpdate : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class Updates : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ManualUpdate : Attribute
    {
        public ManualUpdate() { }
        public ManualUpdate(bool updateIfForced)
        {
            UpdateIfForced = updateIfForced;
        }

        public bool UpdateIfForced { get; set; } = true;
    }

    public partial class ViewModel : INotifyPropertyChanged
    {
        public static Action<Action> DefaultDispatchAction = a => a();

        public virtual void Dispatch(Action a)
        {
            DefaultDispatchAction?.Invoke(a);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string m = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(m));

        public ViewModel() { }

        protected virtual void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            storage = value;
            RaisePropertyChanged(propertyName);
        }

        public void UpdateProperties(bool forceAll = false)
        {
            Dispatch(() =>
            {
                foreach (var item in GetType().GetProperties())
                {
                    if (item.GetCustomAttributes(typeof(NoUpdate), true).Any())
                        continue;
                    if (item.GetCustomAttributes(typeof(ManualUpdate), true).Any() && !forceAll)
                        continue;
                    if (item.PropertyType.IsAssignableFrom(typeof(ICommand)) && !item.GetCustomAttributes(typeof(Updates), true).Any())
                        continue;

                    RaisePropertyChanged(item.Name);
                }
            });
        }

        public void UpdateProperties(IEnumerable<string> names)
        {
            Dispatch(() =>
            {
                foreach (var item in names)
                    RaisePropertyChanged(item);
            });
        }
    }
}
