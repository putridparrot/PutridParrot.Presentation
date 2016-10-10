using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;
using Presentation.Core;

namespace Sample.Wpf.Presentation.Core
{
    /// <summary>
    /// Demonstrates using a model for the underlying data, so all setters/getter
    /// delegate to the underlying model - chained property change events
    /// are explicitly called in this example
    /// </summary>
    public class MyViewModel4 : ViewModel
    {
        private readonly MyDomainObject domainObject = new MyDomainObject();

        public MyViewModel4()
        {
            ValidateCommand = new ActionCommand(() => { /* no validation in this implementation */});
        }

        public string FirstName
        {
            get { return domainObject.FirstName; }
#if !NET4
            set { SetProperty(() => domainObject.FirstName, v => domainObject.FirstName = v, value); }
#else
            set
            {
                if(SetProperty(() => domainObject.FirstName, v => domainObject.FirstName = v, value, this.NameOf(x => x.FirstName)))
                    RaisePropertyChanged(this.NameOf(x => x.FullName));
            }
#endif
        }

        public string LastName
        {
            get { return domainObject.LastName; }
#if !NET4
            set { SetProperty(() => domainObject.LastName, v => domainObject.LastName = v, value); }
#else
            set
            {
                if(SetProperty(() => domainObject.LastName, v => domainObject.LastName = v, value, this.NameOf(x => x.LastName)))
                    RaisePropertyChanged(this.NameOf(x => x.FullName));
            }
#endif
        }

        public string FullName => $"{FirstName} {LastName}";

        public ICommand ValidateCommand { get; private set; }
    }
}
