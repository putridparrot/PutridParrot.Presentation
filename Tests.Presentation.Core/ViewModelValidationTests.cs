using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation.Core;
using PutridParrot.Presentation.Core.Interfaces;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelValidationTests
    {
        class PersonViewModel : ViewModel
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            [Required]
            [System.ComponentModel.DataAnnotations.Range(16, Int32.MaxValue, ErrorMessage = "Person must be older than 16")]
            public int Age
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }
        }

        // using metadata type
        [MetadataType(typeof(Person2Validation))]
        class Person2ViewModel : ViewModel
        {
            public string Name
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            public int Age
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }
        }

        class Person2Validation
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
            [Required]
            [System.ComponentModel.DataAnnotations.Range(16, Int32.MaxValue, ErrorMessage = "Person must be older than 16")]
            public int Age { get; set; }
        }

        [Test]
        public void Validation_PropertyChangeValidationTakesPlaceWithFailedValue_ExpectValidationFailure()
        {
            var vm = new PersonViewModel {Age = 2};

            var dataErrorInfo = vm as IExtendedDataErrorInfo;

            dataErrorInfo
                .Should()
                .NotBeNull();

            dataErrorInfo.Errors.Length
                .Should()
                .Be(1);

            dataErrorInfo.Errors[0]
                .Should()
                .Be("Person must be older than 16");
        }

        [Test]
        public void Validation_PropertyChangeValidationTakesPlaceWithSuccessfulValue_ExpectValidationSuccess()
        {
            var vm = new PersonViewModel { Age = 20 };

            var dataErrorInfo = vm as IExtendedDataErrorInfo;

            dataErrorInfo
                .Should()
                .NotBeNull();

            dataErrorInfo.Errors
                .Should()
                .BeNull();
        }

        [Test]
        public void Validation_ValidateWholeObjectWithMissingData_ExpectValidationFailure()
        {
            var vm = new PersonViewModel();

            var failures = vm.Validate();

            failures.Length
                .Should()
                .Be(2);

            var dataErrorInfo = vm as IExtendedDataErrorInfo;

            dataErrorInfo.Errors.Length
                .Should()
                .Be(2);
        }

        [Test]
        public void ValidationUsingMetaDataType_PropertyChangeValidationTakesPlaceWithFailedValue_ExpectValidationFailure()
        {
            var vm = new Person2ViewModel { Age = 2 };

            var dataErrorInfo = vm as IExtendedDataErrorInfo;

            dataErrorInfo
                .Should()
                .NotBeNull();

            dataErrorInfo.Errors.Length
                .Should()
                .Be(1);

            dataErrorInfo.Errors[0]
                .Should()
                .Be("Person must be older than 16");
        }

        [Test]
        public void ValidationUsingMetaDataType_PropertyChangeValidationTakesPlaceWithSuccessfulValue_ExpectValidationSuccess()
        {
            var vm = new Person2ViewModel { Age = 20 };

            var dataErrorInfo = vm as IExtendedDataErrorInfo;

            dataErrorInfo
                .Should()
                .NotBeNull();

            dataErrorInfo.Errors
                .Should()
                .BeNull();
        }

        [Test]
        public void ValidationUsingMetaDataType_ValidateWholeObjectWithMissingData_ExpectValidationFailure()
        {
            var vm = new Person2ViewModel();

            var failures = vm.Validate();

            failures.Length
                .Should()
                .Be(2);

            var dataErrorInfo = vm as IExtendedDataErrorInfo;

            dataErrorInfo.Errors.Length
                .Should()
                .Be(2);
        }
    }
}
