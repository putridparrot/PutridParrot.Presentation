using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation;
using PutridParrot.Presentation.Attributes;
using PutridParrot.Presentation.Exceptions;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelRegistryTests
    {
        private class StubViewModel : ViewModel
        {
            [ChangeTracking(false)]
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

            [DefaultValue(20)]
            [Comparer(typeof(DivisibleBy100Comparer))]
            public int Age
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            [Comparer(typeof(DivisibleBy100Comparer))]
            public int ShoeSize
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }
        }

        [Test]
        public void Register_EnsureViewModelWasRegistered()
        {
            // just ensure we're in a default state
            ViewModelRegistry.Instance.Clear(typeof(StubViewModel));

            var vm = new StubViewModel();

            var definitions = ViewModelRegistry.Instance.Get(vm.GetType());
            definitions
                .Should()
                .NotBeNull();

            vm.Dispose();
        }

        [Test]
        public void TrackingAttribute_WhenExists_EnsureAttributesCorrectlyMatchedToProperties()
        {
            // just ensure we're in a default state
            ViewModelRegistry.Instance.Clear(typeof(StubViewModel));

            var vm = new StubViewModel();

            var definitions = ViewModelRegistry.Instance.Get(vm.GetType());

            var firstName = definitions["FirstName"];
            firstName
                .Should()
                .NotBeNull();

            firstName.ChangeTrackingDisabled
                .Should()
                .BeTrue();

            vm.Dispose();
        }

        [Test]
        public void TrackingAttribute_WhenDoesNotExists_AssumeNullReturn()
        {
            // just ensure we're in a default state
            ViewModelRegistry.Instance.Clear(typeof(StubViewModel));

            var vm = new StubViewModel();

            var definitions = ViewModelRegistry.Instance.Get(vm.GetType());

            var lastName = definitions["LastName"];
            lastName
                .Should()
                .BeNull();

            vm.Dispose();
        }

        [Test]
        public void ComparerAttribute_WhenTheSameComparerExistsMultipleTimes_ExpectSingleInstanceToBeCreated()
        {
            // just ensure we're in a default state
            ViewModelRegistry.Instance.Clear(typeof(StubViewModel));

            var vm = new StubViewModel();

            var definitions = ViewModelRegistry.Instance.Get(vm.GetType());

            var age = definitions["Age"];
            age
                .Should()
                .NotBeNull();

            age.Comparer
                .Should()
                .NotBeNull();

            var shoeSize = definitions["ShoeSize"];
            shoeSize
                .Should()
                .NotBeNull();

            shoeSize.Comparer
                .Should()
                .NotBeNull();


            age.Comparer
                .Should()
                .BeSameAs(shoeSize.Comparer);

            vm.Dispose();
        }

        [Test]
        public void DefaultValue_CheckPropertyHasDefaultValue()
        {
            // just ensure we're in a default state
            ViewModelRegistry.Instance.Clear(typeof(StubViewModel));

            var vm = new StubViewModel();

            var definitions = ViewModelRegistry.Instance.Get(vm.GetType());

            var age = definitions["Age"];
            age
                .Should()
                .NotBeNull();

            age.Default
                .Should()
                .Be(20);

            vm.Dispose();
        }

        [Test]
        public void ResgiterUnRegister_EnsureCleanedUpCorrectly()
        {
            // just ensure we're in a default state
            ViewModelRegistry.Instance.Clear(typeof(StubViewModel));

            var vm1 = new StubViewModel();
            var vm2 = new StubViewModel();
            var vm3 = new StubViewModel();

            vm1.Dispose();
            vm2.Dispose();
            vm3.Dispose();

            Action action = () => ViewModelRegistry.Instance.Get(typeof(StubViewModel));
            action
                .Should()
                .Throw<TypeNotRegisteredException>();
        }
    }
}
