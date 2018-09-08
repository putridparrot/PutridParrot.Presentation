using PutridParrot.Presentation;

namespace ProfilingApp
{
    public class PersonViewModel : ViewModel
    {
        public string FirstName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public string LastName
        {
            get => GetProperty<string>();
            set => SetProperty(value); 
        }
        public int Age
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }
    }
}