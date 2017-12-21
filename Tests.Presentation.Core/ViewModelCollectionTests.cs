using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;
using Presentation.Core.Attributes;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelCollectionTests
    {
        class PersonViewModel : ViewModel
        {
            public string Name
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            [CreateInstanceUsing(typeof(GenderFactory))]
            public ExtendedObservableCollection<string> Gender => GetProperty<ExtendedObservableCollection<string>>();

            [CreateInstance]
            public ExtendedObservableCollection<PersonViewModel> Children => GetProperty<ExtendedObservableCollection<PersonViewModel>>();

            [CreateInstance]
            public ExtendedObservableCollection<PersonViewModel> GrandChildren => GetProperty<ExtendedObservableCollection<PersonViewModel>>();

            public ExtendedObservableCollection<PersonViewModel> Friends
            {
                get { return GetProperty<ExtendedObservableCollection<PersonViewModel>>(); }
                set { SetProperty(value); }
            }

            // not created automatically
            public ExtendedObservableCollection<PersonViewModel> Team => GetProperty<ExtendedObservableCollection<PersonViewModel>>();

            [ChangeTracking(false)]
            [CreateInstance]
            public ExtendedObservableCollection<string> Pets => GetProperty<ExtendedObservableCollection<string>>();
        }

        [Test]
        public void EnsureCollectionIsNotCreatedViaGetPropertyWithoutCreateInstanceAttribute()
        {
            var vm = new PersonViewModel();

            vm.Team
                .Should()
                .BeNull();
        }

        [Test]
        public void EnsureCollectionIsAssignedCorrectlyViaSetter()
        {
            var vm = new PersonViewModel();

            var friends = new ExtendedObservableCollection<PersonViewModel>();

            vm.Friends = friends;

            vm.Friends
                .Should()
                .NotBeNull();
        }

        [Test]
        public void EnsureCollectionIsCreatedWithCreateInstanceAttribute()
        {
            var vm = new PersonViewModel();

            vm.Children
                .Should()
                .NotBeNull();
        }

        [Test]
        public void EnsureCollectionIsCreatedWithCreateInstanceAttributeAndAreUniqueBetweenProperties()
        {
            var vm = new PersonViewModel();

            vm.Children
                .Should()
                .NotBeNull();

            vm.GrandChildren
                .Should()
                .NotBeNull();

            vm.Children
                .Should()
                .NotBeSameAs(vm.GrandChildren);
        }

        [Test]
        public void EnsureCollectionIsCreatedWithCreateInstanceAttributeAndAreUnique()
        {
            var vm1 = new PersonViewModel();
            var vm2 = new PersonViewModel();

            vm1.Children.Add(new PersonViewModel());

            vm1.Children
                .Should()
                .NotBeNull();

            vm2.Children
                .Should()
                .NotBeSameAs(vm1.Children);
        }

        [Test]
        public void CollectionAddTo_ExpectIsChangeSetToTrue()
        {
            var vm = new PersonViewModel();

            vm.Children.Add(new PersonViewModel());

            vm.IsChanged
                .Should()
                .BeTrue();
        }

        [Test]
        public void CollectionRemovedFrom_ExpectIsChangeSetToTrue()
        {
            var vm = new PersonViewModel();

            var child = new PersonViewModel();
            var friends = new ExtendedObservableCollection<PersonViewModel>
            {
                child
            };

            vm.Friends = friends;
            vm.Friends.Remove(child);

            vm.IsChanged
                .Should()
                .BeTrue();
        }

        [Test]
        public void CollectionItemChanged_ExpectIsChangeSetToTrue()
        {
            var vm = new PersonViewModel();

            var child = new PersonViewModel();
            var friends = new ExtendedObservableCollection<PersonViewModel>
            {
                child
            };

            vm.Friends = friends;

            child.Friends = new ExtendedObservableCollection<PersonViewModel>();

            vm.IsChanged
                .Should()
                .BeTrue();
        }

        [Test]
        public void CollectionChange_WithChangeTrackingOff_ExpectIsChangeSetToNotChange()
        {
            var vm = new PersonViewModel();

            vm.Pets.Add("Scooby");

            vm.IsChanged
                .Should()
                .BeFalse();
        }

        // this is the same as normal SetProperty, but want to 
        // start to verify collection specific code.
        [Test]
        public void SetProperty_WithCollection_EnsureCorrectlyAssigned()
        {
            var vm = new PersonViewModel();

            var friends1 = new ExtendedObservableCollection<PersonViewModel>();
            var friends2 = new ExtendedObservableCollection<PersonViewModel>();

            vm.Friends = friends1;
            vm.Friends = friends2;

            vm.Friends
                .Should()
                .BeSameAs(friends2);
        }

        [Test]
        public void CreateUsing_WithCollection_ExpectValuesToBePopulated()
        {
            var vm = new PersonViewModel();

            vm.Gender
                .Should()
                .NotBeNull();

            vm.Gender.Count
                .Should()
                .Be(2);

            vm.Gender[0]
                .Should()
                .Be("Male");

            vm.Gender[1]
                .Should()
                .Be("Female");
        }
    }
}
