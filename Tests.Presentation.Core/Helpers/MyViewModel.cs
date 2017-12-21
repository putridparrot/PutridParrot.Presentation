using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Presentation.Core;
using Presentation.Core.Attributes;
using Presentation.Core.Helpers;

namespace Tests.Presentation.Helpers
{
    [ExcludeFromCodeCoverage]
    public class MyViewModel : ViewModel
    {
        public bool IsEnabled
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        [Comparer(typeof(DivisibleBy100Comparer))]
        public int Count
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }

        //public int Calculated
        //{
        //    get { return GetProperty(() => Count * 10); }
        //}

        [ChangeTracking(false)]
        public bool NotTracked
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        [DefaultValue("Hello World")]
        public string Message
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [DefaultValue("Hello World")]
        public bool BadDefault
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        public int CannotBeNegative
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value, v => Validation.Check(v > 0, "Value cannot be negative")); }
        }
    }
}
