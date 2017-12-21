using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelWithoutBackingReadOnlyPropertyTests
    {
        class SimpleViewModel : ViewModelWithoutBacking
        {
            private string _firstName;
            private string _lastName;

            public string FirstName
            {
                get { return GetProperty(ref _firstName); }
                set { SetProperty(ref _firstName, value); }
            }

            public string LastName
            {
                get { return GetProperty(ref _lastName); }
                set { SetProperty(ref _lastName, value); }
            }

            public string FullName
            {
                get { return ReadOnlyProperty(() => $"{FirstName} {LastName}"); }
            }

            // tests if on properties
            public string Name
            {
                get
                {
                    return ReadOnlyProperty(() =>
                    {
                        if (!String.IsNullOrEmpty(FirstName))
                            return FirstName;
                        if (!String.IsNullOrEmpty(LastName))
                            return LastName;

                        return "None";
                    });
                }
            }
        }

        [Test]
        public void FullName_AfterInitialization_ShouldBeFullNameCombination()
        {
            var vm = new SimpleViewModel
            {
                FirstName = "Scooby",
                LastName = "Doo"
            };

            vm.FullName
                .Should()
                .Be("Scooby Doo");
        }

        [Test]
        public void FullName_IfDependentUponPropertyChanges_ExpectFullNamePropertyChange()
        {
            var vm = new SimpleViewModel
            {
                FirstName = "Scooby",
                LastName = "Doo"
            };

            // we need to call full name property to register it's being used
            var fullName = vm.FullName;

            var fullNameChange = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "FullName")
                    fullNameChange = true;
            };

            vm.FirstName = "Scrappy";

            fullNameChange
                .Should()
                .BeTrue();

            vm.FullName
                .Should()
                .Be("Scrappy Doo");
        }

        [Test]
        public void Name_IfDependentUponPropertiesChangeAsPartOfIfStatements_ExpectNamePropertyChanges()
        {
            var vm = new SimpleViewModel
            {
                FirstName = String.Empty,
                LastName = String.Empty
            };

            // we need to call full name property to register it's being used
            var name = vm.Name;

            var nameChange = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name")
                    nameChange = true;
            };

            vm.FirstName = "Scooby";

            nameChange
                .Should()
                .BeTrue();

            vm.Name
                .Should()
                .Be("Scooby");

            nameChange = false;

            vm.FirstName = String.Empty;

            nameChange
                .Should()
                .BeTrue();

            vm.Name
                .Should()
                .Be("None");

            nameChange = false;

            vm.LastName = "Doo";

            nameChange
                .Should()
                .BeTrue();

            vm.Name
                .Should()
                .Be("Doo");
        }
    }
}