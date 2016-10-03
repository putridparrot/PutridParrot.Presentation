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
    /// Extends MyViewModel to demonstrate the use of property chaining
    /// </summary>
    public class MyViewModel3 : ViewModel
    {
        private string firstName;
        private string lastName;

        public MyViewModel3()
        {
            Rules = new Rules();
            Rules.Add(new PropertyChainRule(new[] { this.NameOf(x => x.FullName) }), this.NameOf(x => x.FirstName));
            Rules.Add(new PropertyChainRule(new[] { this.NameOf(x => x.FullName) }), this.NameOf(x => x.LastName));

            ValidateCommand = new ActionCommand(() => { /* no validation in this implementation */});
        }

        public string FirstName
        {
            get { return firstName; }
#if !NET4
            set { SetProperty(ref firstName, value); }
#else
            set { SetProperty(ref firstName, value, this.NameOf(x => x.FirstName)); }
#endif
        }

        public string LastName
        {
            get { return lastName; }
#if !NET4
            set { SetProperty(ref lastName, value); }
#else
            set { SetProperty(ref lastName, value, this.NameOf(x => x.LastName)); }
#endif
        }

        public string FullName => $"{FirstName} {LastName}";

        public ICommand ValidateCommand { get; private set; }
    }

}
