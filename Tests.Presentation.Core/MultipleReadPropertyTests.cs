using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Patterns;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class MultipleReadPropertyTests
    {
        class TestViewModel : ViewModel
        {
            public int Value
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            public int Multiple10
            {
                get { return ReadOnlyProperty(() => Value * 10); }
            }

            public int Multiple100
            {
                get { return ReadOnlyProperty(() => Value*100); }
            }
        }

        [Test]
        public void Value_ChangeShouldTriggerBothMultiple10AndMultiple100Properties()
        {
            var vm = new TestViewModel();
            var viewBinding = new ViewBinding(vm);

            vm.Value = 3;

            viewBinding.Changed.Count
                .Should()
                .Be(4);

            viewBinding.Changed[0]
                .Should()
                .Be("IsChanged");
            viewBinding.Changed[1]
                .Should()
                .Be("Value");
            viewBinding.Changed[2]
                .Should()
                .Be("Multiple10");
            viewBinding.Changed[3]
                .Should()
                .Be("Multiple100");
        }
    }
}
