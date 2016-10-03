using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    public class NotifyPropertyChangedListener
    {
        private readonly IList<string> propertyChanged = new List<string>();
        private readonly IList<string> propertyChanging = new List<string>();

        public NotifyPropertyChangedListener(object model)
        {
            var notifyChanged = model as INotifyPropertyChanged;
            if (notifyChanged != null)
            {
                notifyChanged.PropertyChanged += (sender, args) => propertyChanged.Add(args.PropertyName);
            }

            var notifyChanging = model as INotifyPropertyChanging;
            if (notifyChanging != null)
            {
                notifyChanging.PropertyChanging += (sender, args) => propertyChanging.Add(args.PropertyName);
            }

        }

        public IList<string> Changed
        {
            get { return propertyChanged; }
        }

        public IList<string> Changing
        {
            get { return propertyChanging; }
        }
    }

}
