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
    /// Sample uses the simple backing store to store 
    /// fields/state. Explicitly raises property changes
    /// for readonly FullName property
    /// </summary>
    public class MyViewModel5 : ViewModel
    {
        public MyViewModel5() :
            base(new SimpleBackingStore())
        {
            ValidateCommand = new ActionCommand(() =>
            {
                /* no validation in this implementation */
            });
        }

        public string FirstName
        {
            get { return GetProperty<string>(this.NameOf(x => x.FirstName)); }
            set
            {
                if (SetProperty(value, this.NameOf(x => x.FirstName)))
                    RaisePropertyChanged(this.NameOf(x => x.FullName));
            }
        }

        public string LastName
        {
            get { return GetProperty<string>(this.NameOf(x => x.LastName)); }
            set
            {
                if (SetProperty(value, this.NameOf(x => x.LastName)))
                    RaisePropertyChanged(this.NameOf(x => x.FullName));
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public ICommand ValidateCommand { get; private set; }
    }
}