//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Microsoft.Expression.Interactivity.Core;
//using Presentation.Core;

//namespace Sample.Wpf.Presentation.Core
//{
//    public class Property<T> : ViewModel
//    {
//        public T Value
//        {
//            get { return GetProperty<T>(this.NameOf(x => x.Value)); }
//            set { SetProperty(value, this.NameOf(x => x.Value)); }
//        }
//    }

//    public class MyViewModel5 : ViewModel
//    {
//        public MyViewModel5()
//        {
//            ValidateCommand = new ActionCommand(() => { /* no validation in this implementation */});
//        }

//        public Property<string> FirstName
//        {
//            get { return GetProperty<Property<string>>(this.NameOf(x => x.FirstName)); }
//            set { SetProperty(value, this.NameOf(x => x.FirstName)); }
//        }

//        public Property<string> LastName
//        {
//            get { return GetProperty<Property<string>>(this.NameOf(x => x.LastName)); }
//            set { SetProperty(value, this.NameOf(x => x.LastName)); }
//        }


//        public string FullName => $"{FirstName.Value} {LastName.Value}";

//        public ICommand ValidateCommand { get; private set; }
//    }

//}
