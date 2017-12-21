using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Presentation.Core;
using Presentation.Core.Attributes;
using Presentation.Core.Helpers;

namespace Tests.Presentation.Helpers
{
    [ExcludeFromCodeCoverage]
    public class MyViewModelWithOwnBacking : ViewModelWithoutBacking
    {
        private bool _isEnabled;
        private int _count;
        private bool _noTracked;
        private string _message;
        private bool _badDefault;
        private int _cannotBeNegative;

        public bool IsEnabled
        {
            get { return GetProperty(ref _isEnabled); }
            set { SetProperty(ref _isEnabled, value); }
        }

        [Comparer(typeof(DivisibleBy100Comparer))]
        public int Count
        {
            get { return GetProperty(ref _count); }
            set { SetProperty(ref _count, value); }
        }

        [ChangeTracking(false)]
        public bool NotTracked
        {
            get { return GetProperty(ref _noTracked); }
            set { SetProperty(ref _noTracked, value); }
        }

        [DefaultValue("Hello World")]
        public string Message
        {
            get { return GetProperty(ref _message); }
            set { SetProperty(ref _message, value); }
        }

        [DefaultValue("Hello World")]
        public bool BadDefault
        {
            get { return GetProperty(ref _badDefault); }
            set { SetProperty(ref _badDefault, value); }
        }

        public int CannotBeNegative
        {
            get { return GetProperty(ref _cannotBeNegative); }
            set { SetProperty(ref _cannotBeNegative, value, v => Validation.Check(v > 0, "Value cannot be negative")); }
        }
    }
}