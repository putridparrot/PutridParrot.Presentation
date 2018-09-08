using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation.Core;
using PutridParrot.Presentation.Core.Attributes;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelWithoutBackingCollectionTests
    {
        class PersonViewModel : ViewModelWithoutBacking
        {
            private string _name;
            private ExtendedObservableCollection<PersonViewModel> _children;
            private ExtendedObservableCollection<PersonViewModel> _team;
            private ExtendedObservableCollection<PersonViewModel> _grandChildren;
            private ExtendedObservableCollection<PersonViewModel> _friends;
            private ExtendedObservableCollection<string> _gender;

            public string Name
            {
                get { return GetProperty(ref _name); }
                set { SetProperty(ref _name, value); }
            }

            [CreateInstanceUsing(typeof(GenderFactory))]
            public ExtendedObservableCollection<string> Gender => GetProperty(ref _gender);

            [CreateInstance]
            public ExtendedObservableCollection<PersonViewModel> Children => GetProperty(ref _children);

            [CreateInstance]
            public ExtendedObservableCollection<PersonViewModel> GrandChildren => GetProperty(ref _grandChildren);

            public ExtendedObservableCollection<PersonViewModel> Friends
            {
                get { return GetProperty(ref _friends); }
                set { SetProperty(ref _friends, value); }
            }

            // not created automatically
            public ExtendedObservableCollection<PersonViewModel> Team => GetProperty(ref _team);
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
        public void EnsureCollectionIsCreatedWithCreateInstanceAttributeAndAreUnique()
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

        // this is the same as normal SetProperty, but want to 
        // start to verify colleciton specific code.
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