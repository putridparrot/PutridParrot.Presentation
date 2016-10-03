using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sample.Wpf.Presentation.Core
{
    public class MyViewModelMeta
    {
        [DisplayName("First Name")]
        [StringLength(Int32.MaxValue, MinimumLength = 3)]
        [Required]
        [CapitalFirstLetter]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [StringLength(Int32.MaxValue, MinimumLength = 3)]
        [Required]
        [CapitalFirstLetter]
        public string LastName { get; set; }

        [DisplayName("Full Name")]
        [Required]
        public string FullName { get; set; }
    }
}