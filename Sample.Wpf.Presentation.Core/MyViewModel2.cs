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
    /// Demonstrates the minimalist view model with backing fields but now
    /// using the ViewModel (higher level) class
    /// 
    /// No validation included and FullName change event called to refresh 
    /// after changes to FirstName and LastName. 
    /// </summary>
    public class MyViewModel2 : ViewModel
    {
        private string firstName;
        private string lastName;

        public MyViewModel2()
        {
            ValidateCommand = new ActionCommand(() => { /* no validation in this implementation */});
        }

        public string FirstName
        {
            get { return firstName; }
#if !NET4
            set { SetProperty(ref firstName, value); }
#else
            set
            {
                if (SetProperty(ref firstName, value, this.NameOf(x => x.FirstName)))
                    OnPropertyChanged(this.NameOf(x => x.FullName));
            }
#endif
        }

        public string LastName
        {
            get { return lastName; }
#if NET45
            set { SetProperty(ref lastName, value); }
#else
            set
            {
                if (SetProperty(ref lastName, value, this.NameOf(x => x.LastName)))
                    OnPropertyChanged(this.NameOf(x => x.FullName));
            }
#endif
        }

        public string FullName => $"{FirstName} {LastName}";

        public ICommand ValidateCommand { get; private set; }
    }
}
