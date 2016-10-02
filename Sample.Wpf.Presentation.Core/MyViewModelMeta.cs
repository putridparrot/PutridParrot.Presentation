using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sample.Wpf.Presentation.Core
{
    public partial class MyViewModelMeta
    {
        [DisplayName("First Name")]
        [StringLength(Int32.MaxValue, MinimumLength = 3)]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [StringLength(Int32.MaxValue, MinimumLength = 3)]
        public string LastName { get; set; }
    }
}