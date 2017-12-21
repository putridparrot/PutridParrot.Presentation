using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ReadWithBranchesPropertyTests
    {
        class TestViewModel : ViewModel
        {
            public bool UseValue1
            {
                get { return GetProperty<bool>(); }
                set { SetProperty(value); }
            }

            [DefaultValue(1)]
            public int Value1
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            [DefaultValue(2)]
            public int Value2
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            public int Result
            {
                get { return ReadOnlyProperty(() => UseValue1 ? Value1 : Value2); }
            }
        }

        [Test]
        public void UseValue1_IsTrue_ShouldCauseResultToUpdate()
        {
            var vm = new TestViewModel();
            var viewBinding = new ViewBinding(vm);

            vm.UseValue1 = true;

            viewBinding.Changed.Count
                .Should()
                .Be(3);

            viewBinding.Changed[0]
                .Should()
                .Be("IsChanged");
            viewBinding.Changed[1]
                .Should()
                .Be("UseValue1");
            viewBinding.Changed[2]
                .Should()
                .Be("Result");
        }

        [Test]
        public void UseValue1_IsTrue_Value1ChangedShouldCauseResultToUpdate()
        {
            var vm = new TestViewModel();
            var viewBinding = new ViewBinding(vm);

            vm.UseValue1 = true;
            vm.Value1 = 10;

            viewBinding.Changed.Count
                .Should()
                .Be(5);

            viewBinding.Changed[0]
                .Should()
                .Be("IsChanged");
            viewBinding.Changed[1]
                .Should()
                .Be("UseValue1");
            viewBinding.Changed[2]
                .Should()
                .Be("Result");
            viewBinding.Changed[3]
                .Should()
                .Be("Value1");
            viewBinding.Changed[4]
                .Should()
                .Be("Result");
        }

        [Test]
        public void UseValue1_IsFalse_ChangingValue2ShouldCauseResultToUpdate()
        {
            var vm = new TestViewModel();
            var viewBinding = new ViewBinding(vm);

            vm.Value2 = 3;

            viewBinding.Changed.Count
                .Should()
                .Be(3);

            viewBinding.Changed[0]
                .Should()
                .Be("IsChanged");
            viewBinding.Changed[1]
                .Should()
                .Be("Value2");
            viewBinding.Changed[2]
                .Should()
                .Be("Result");
        }

        [Test]
        public void UseValue1_IsTrue_Value1ChangedShouldCauseResultToUpdate2()
        {
            var vm = new TestViewModel();
            var viewBinding = new ViewBinding(vm);

            vm.UseValue1 = true;
            vm.Value1 = 10;

            viewBinding.Changed.Count
                .Should()
                .Be(5);

            viewBinding.Changed[0]
                .Should()
                .Be("IsChanged");
            viewBinding.Changed[1]
                .Should()
                .Be("UseValue1");
            viewBinding.Changed[2]
                .Should()
                .Be("Result");
            viewBinding.Changed[3]
                .Should()
                .Be("Value1");
            viewBinding.Changed[4]
                .Should()
                .Be("Result");
        }
    }
}