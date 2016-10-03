using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        public void NameOf_SingleProperty_ExpectCorrectNameReturned()
        {
            var vm = new MyNotifyPropertyChanged();

            Assert.AreEqual("Name", vm.NameOf(x => x.Name));
        }

        [Test]
        public void NameOf_OnMethod_ExpectException()
        {
            var vm = new MyNotifyPropertyChanged();

            Assert.Throws<Exception>(() => vm.NameOf(x => x.ToString()));
        }

        [Test]
        public void NameOf_MultiplePropertiesOfSameType_ExpectCorrectNamesReturned()
        {
            var vm = new MyNotifyPropertyChanged();

            var result = vm.NameOf(x => x.Name, x => x.Address);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Name", result[0]);
            Assert.AreEqual("Address", result[1]);
        }
    }
}
