using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ChainReadPropertyTests
    {
        class TestViewModel : ViewModel
        {
            public int Value
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            public int Add1
            {
                get { return ReadOnlyProperty(() => Value + 1); }
            }

            public int Multiply100
            {
                get { return ReadOnlyProperty(() => Add1 * 100); }
            }
        }

        [Test]
        public void Value_ChangeShouldGiveResultsValuePlusMultipliedBy100()
        {
            var vm = new TestViewModel();

            vm.Value = 3;

            vm.Add1
                .Should()
                .Be(4);
            vm.Multiply100
                .Should()
                .Be(400);
        }

        [Test]
        public void Value_ChangeShouldTriggerBothAdd1WhichShouldThenTriggerMultiply100()
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
                .Be("Add1");
            viewBinding.Changed[3]
                .Should()
                .Be("Multiply100");
        }
    }
}