using PutridParrot.Presentation.Core;
using PutridParrot.Presentation.Core.Helpers;

namespace ProfilingApp
{
    public class PersonNotifyPropertyChanged : NotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private int _age;

        public string FirstName
        {
            get { return _firstName; }
            set { this.SetProperty(ref _firstName, value); }
        }
        public string LastName
        {
            get { return _lastName; }
            set { this.SetProperty(ref _lastName, value); }
        }
        public int Age
        {
            get { return _age; }
            set { this.SetProperty(ref _age, value); }
        }
    }
}