using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [MetadataType(typeof(MyViewModel))]
    public class MyViewModel : ViewModel
    {
        private string name;
        private string address;

        public MyViewModel()
        {
            Validation = new ViewModelValidation<MyViewModel>(this);
        }

        [Required]
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, this.NameOf(x => x.Name)); }
        }

        [Required]
        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value, this.NameOf(x => x.Address)); }
        }
    }
}
