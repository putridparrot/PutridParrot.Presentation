using System.Diagnostics.CodeAnalysis;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    public class MyNotifyPropertyChanged : NotifyPropertyChanged
    {
        private string name;
        private string address;

        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(x => x.Name, ref name, value); }
        }

        public string Address
        {
            get { return address; }
            set { this.RaiseAndSetIfChanged(x => x.Address, ref address, value); }
        }
    }
}