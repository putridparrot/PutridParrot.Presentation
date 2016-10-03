using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [MetadataType(typeof(MyViewModel1))]
    public class MyViewModel1 : ViewModel
    {
        public MyViewModel1() :
            base(new SimpleBackingStore())
        {
            Validation = new ViewModelValidation<MyViewModel1>(this);
        }

        [Required]
        public string Name
        {
            get { return GetProperty<string>(this.NameOf(x => x.Name)); }
            set { SetProperty(value, this.NameOf(x => x.Name)); }
        }

        [Required]
        public string Address
        {
            get { return GetProperty<string>(this.NameOf(x => x.Address)); }
            set { SetProperty(value, this.NameOf(x => x.Address)); }
        }
    }
}