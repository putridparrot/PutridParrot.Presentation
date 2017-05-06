using Presentation.Patterns;

namespace ProfilingApp
{
    public class PersonViewModel : ViewModel
    {
        public string FirstName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
        public string LastName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
        public int Age
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }
    }
}