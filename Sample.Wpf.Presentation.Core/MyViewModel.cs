using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;
using Presentation.Core;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Sample.Wpf.Presentation.Core
{
    [MetadataType(typeof(MyViewModelMeta))]
    public class MyViewModel : ViewModel
    {
        public MyViewModel() :
            base(new SimpleBackingStore())
        {
            DataErrorInfo = new DataErrorInfo();
            Validation = new ViewModelValidation<MyViewModel>(this);

            Rules = new Rules();
            Rules.Add(new PropertyChainRule(new []{ this.NameOf(x => x.FullName) }), 
                this.NameOf(x => x.FirstName));
            Rules.Add(new PropertyChainRule(new[] { this.NameOf(x => x.FullName) }),
                this.NameOf(x => x.LastName));

            ValidateCommand = new ActionCommand(() => Validation.Validate());
        }

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

        public string FullName => $"{FirstName} {LastName}";

        public ICommand ValidateCommand { get; private set; }
    }
}
