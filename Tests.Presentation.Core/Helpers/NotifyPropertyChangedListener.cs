using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Tests.Presentation.Helpers
{
    [ExcludeFromCodeCoverage]
    public class NotifyPropertyChangedListener
    {
        private readonly IList<string> _propertyChanged = new List<string>();
        private readonly IList<string> _propertyChanging = new List<string>();

        public NotifyPropertyChangedListener(object model)
        {
            var notifyChanged = model as INotifyPropertyChanged;
            if (notifyChanged != null)
            {
                notifyChanged.PropertyChanged += (sender, args) => _propertyChanged.Add(args.PropertyName);
            }

            var notifyChanging = model as INotifyPropertyChanging;
            if (notifyChanging != null)
            {
                notifyChanging.PropertyChanging += (sender, args) => _propertyChanging.Add(args.PropertyName);
            }
        }

        public IList<string> Changed => _propertyChanged;
        public IList<string> Changing => _propertyChanging;
    }

}
