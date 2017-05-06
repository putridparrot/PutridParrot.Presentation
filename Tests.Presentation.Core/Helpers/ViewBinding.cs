using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Presentation.Helpers
{
    /// <summary>
    /// A simple implementation of a view-like binding to try to test
    /// property change updates
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ViewBinding
    {
        private readonly Dictionary<string, PropertyInfo> _bindings;
        private readonly object _dataContext;

        private readonly IList<string> _propertyChanged = new List<string>();
        private readonly IList<string> _propertyChanging = new List<string>();

        public ViewBinding(object dataContext)
        {
            _bindings = new Dictionary<string, PropertyInfo>();
            _dataContext = dataContext;
            var notifyChanged = dataContext as INotifyPropertyChanged;
            if (notifyChanged != null)
            {
                notifyChanged.PropertyChanged += Notification_PropertyChanged;
            }

            var notifyChanging = dataContext as INotifyPropertyChanging;
            if (notifyChanging != null)
            {
                notifyChanging.PropertyChanging += NotifyChangingOnPropertyChanging;
            }
            Bind();
        }

        private void Bind()
        {
            foreach (var pi in _dataContext.GetType().GetProperties())
            {
                AddBinding(pi);
            }

            foreach (var pi in _bindings.Values)
            {
                ReadPropertyValue(pi);
            }
        }

        public IList<string> Changed => _propertyChanged;
        public IList<string> Changing => _propertyChanging;

        private void NotifyChangingOnPropertyChanging(object sender, PropertyChangingEventArgs propertyChangingEventArgs)
        {
            _propertyChanging.Add(propertyChangingEventArgs.PropertyName);
        }

        private void Notification_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _propertyChanged.Add(e.PropertyName);

            PropertyInfo pi;
            if (_bindings.TryGetValue(e.PropertyName, out pi))
            {
                ReadPropertyValue(pi);
            }
        }

        private void ReadPropertyValue(PropertyInfo pi)
        {
            var result = pi.GetMethod.Invoke(_dataContext, null);
        }

        private void AddBinding(PropertyInfo pi)
        {
            if (pi != null)
            {
                _bindings[pi.Name] = pi;
            }
        }
    }
}
