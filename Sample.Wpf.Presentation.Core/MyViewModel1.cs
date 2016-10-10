using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;
using Presentation.Core;

namespace Sample.Wpf.Presentation.Core
{
    /// <summary>
    /// Demonstrates the minimalist view model with backing fields and using
    /// the bare bones NotifyPropertyChanged class
    /// 
    /// No validation included and FullName change event explicitly called to 
    /// refresh after changes to FirstName and LastName
    /// </summary>
    public class MyViewModel1 : NotifyPropertyChanged
    {
        private string firstName;
        private string lastName;

        public MyViewModel1()
        {
            ValidateCommand = new ActionCommand(() => { /* no validation in this implementation */});
        }

        public string FirstName
        {
            get { return firstName; }
#if !NET4
            set { this.RaiseAndSetIfChanged(ref firstName, value); }
#else
            set
            {
                if(this.RaiseAndSetIfChanged(x => x.FirstName, ref firstName, value))
                    this.RaisePropertyChanged(x => x.FullName);
            }
#endif
        }

        public string LastName
        {
            get { return lastName; }
#if !NET4
            set { this.RaiseAndSetIfChanged(ref lastName, value); }
#else
            set
            {
                if(this.RaiseAndSetIfChanged(x => x.LastName, ref lastName, value))
                    this.RaisePropertyChanged(x => x.FullName);
            }
#endif
        }

        public string FullName => $"{FirstName} {LastName}";

        public ICommand ValidateCommand { get; private set; }

    }

}
