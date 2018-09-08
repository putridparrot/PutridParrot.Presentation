using System.Diagnostics.CodeAnalysis;
using PutridParrot.Presentation.Core;
using PutridParrot.Presentation.Core.Interfaces;

namespace Tests.Presentation.Helpers
{
    [ExcludeFromCodeCoverage]
    class GenderFactory : IFactory
    {
        public object Create(params object[] args)
        {
            var gender = new ExtendedObservableCollection<string>
            {
                "Male",
                "Female"
            };
            return gender;
        }
    }
}