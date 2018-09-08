using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelWithModelTests
    {
        class PersonModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }

        class PersonViewModel : ViewModelWithModel
        {
            private readonly PersonModel _model;

            public PersonViewModel(PersonModel model)
            {
                _model = model;
            }

            public string FirstName
            {
                get
                {
                    return GetProperty(
                        () => _model.FirstName, 
                        v => _model.FirstName = v);
                }
                set
                {
                    SetProperty(
                        () => _model.FirstName, 
                        v => _model.FirstName = v, 
                        value);
                }
            }

            public string LastName
            {
                get
                {
                    return GetProperty(
                        () => _model.LastName, 
                        v => _model.LastName = v);
                }
                set
                {
                    SetProperty(
                        () => _model.LastName, 
                        v => _model.LastName = v, 
                        value);
                }
            }

            public string FullName
            {
                get { return ReadOnlyProperty(() => $"{FirstName} {LastName}"); }
            }

            [DefaultValue(32)]
            public int Age
            {
                get
                {
                    return GetProperty(
                        () => _model.Age, 
                        v => _model.Age = v);
                }
                set
                {
                    SetProperty(
                        () => _model.Age, 
                        v => _model.Age = v, 
                        value);
                }
            }
        }

        [Test]
        public void DefaultValueCorrectlyAssignedToUnderlyingModel()
        {
            var model = new PersonModel();
            var vm = new PersonViewModel(model);

            // we need to force a property creation
            var age = vm.Age;

            model.Age
                .Should()
                .Be(32);
        }

        [Test]
        public void SetProperty_EnsureValueWrittenToUnderlyingModel()
        {
            var model = new PersonModel { FirstName = "Scooby", LastName = "Doo", Age = 23 };
            var vm = new PersonViewModel(model);

            vm.FirstName = "Scrappy";

            model.FirstName
                .Should()
                .Be("Scrappy");
        }

        [Test]
        public void GetProperty_EnsureValueTakenFromUnderlyingModel()
        {
            var model = new PersonModel {FirstName = "Scooby", LastName = "Doo", Age = 23};
            var vm = new PersonViewModel(model);

            vm.FirstName
                .Should()
                .Be("Scooby");
        }

        [Test]
        public void ReadOnlyProperty_EnsureValueAsExpected()
        {
            var model = new PersonModel { FirstName = "Scooby", LastName = "Doo", Age = 23 };
            var vm = new PersonViewModel(model);

            vm.FullName
                .Should()
                .Be("Scooby Doo");
        }
    }
}