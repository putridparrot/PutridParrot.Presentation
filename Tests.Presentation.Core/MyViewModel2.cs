using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [MetadataType(typeof(MyViewModel2))]
    public class MyViewModel2 : ViewModel
    {
        private class MyDomainObject
        {
            public string Name { get; set; }
            public string Address { get; set; }
        }

        private readonly MyDomainObject domainObject = new MyDomainObject();

        public MyViewModel2()
        {
            Validation = new ViewModelValidation<MyViewModel2>(this);
        }
        // called domain object
        [Required]
        public string Name
        {
            get { return domainObject.Name; }
            set { SetProperty(() => domainObject.Name, v => domainObject.Name = v, value, this.NameOf(x => x.Name)); }
        }

        // called domain object
        [Required]
        public string Address
        {
            get { return domainObject.Address; }
            set { SetProperty(() => domainObject.Address, v => domainObject.Address = v, value, this.NameOf(x => x.Address)); }
        }
    }
}