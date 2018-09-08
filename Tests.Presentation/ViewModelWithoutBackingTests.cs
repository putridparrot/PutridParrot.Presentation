using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelWithoutBackingTests
    {
        [Test]
        public void SetProperty_ExpectPropertyToChangeToDefaultValue()
        {
            var vm = new MyViewModelWithOwnBacking { IsEnabled = true };

            vm.IsEnabled
                .Should()
                .BeTrue();
        }

        [Test]
        public void SetProperty_ExpectPropertyToChangeToNewValue()
        {
            var vm = new MyViewModelWithOwnBacking { IsEnabled = false };

            vm.IsEnabled = true;

            vm.IsEnabled
                .Should()
                .BeTrue();
        }

        [Test]
        public void SetProperty_ExpectPropertyToChangeToNewValue_UseCustomComparer()
        {
            var vm = new MyViewModelWithOwnBacking { Count = 0 };

            vm.Count = 100;

            vm.Count
                .Should()
                .Be(100);
        }

        [Test]
        public void SetProperty_ExpectPropertyToRemainUnchangedIfSameValueSet_UseCustomComparer()
        {
            var count = 0;
            var vm = new MyViewModelWithOwnBacking { Count = 0 };
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Count")
                    count++;
            };

            vm.Count = 10;

            count
                .Should()
                .Be(0);
        }

        [Test]
        public void SetProperty_ExpectPropertyToRemainUnchangedIfSameValueSet()
        {
            var count = 0;
            var vm = new MyViewModelWithOwnBacking { IsEnabled = false };
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsEnabled")
                    count++;
            };

            vm.IsEnabled = false;

            count
                .Should()
                .Be(0);
        }

        [Test]
        public void SetProperty_ExpectPropertyChangeEventToOccurForIsEnabled()
        {
            var count = 0;
            var vm = new MyViewModelWithOwnBacking { IsEnabled = false };
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsEnabled")
                    count++;
            };

            vm.IsEnabled = true;

            count
                .Should()
                .Be(1);
        }

        //[Test]
        //public void GetDependencyProperty_EXpectCalculatedToBeCorrect()
        //{
        //    var vm = new MyViewModelWithOwnBacking { Count = 10 };

        //    vm.Calculated
        //        .Should()
        //        .Be(100);
        //}

        //[Test]
        //public void SetProperty_OnPropertyWithDependecny_ExpectPropertyChangeEventToOccurForCalculated()
        //{
        //    var count = 0;
        //    var vm = new MyViewModelWithOwnBacking();
        //    vm.PropertyChanged += (sender, args) =>
        //    {
        //        if (args.PropertyName == "Calculated")
        //            count++;
        //    };

        //    vm.Count = 10;

        //    count
        //        .Should()
        //        .Be(1);
        //}

        [Test]
        public void IsChanged_DuringInit_IsChangedShouldNotChange()
        {
            var vm = new MyViewModelWithOwnBacking();
            vm.BeginInit();
            vm.IsEnabled = true;
            vm.EndInit();

            vm.IsChanged
                .Should()
                .BeFalse();
        }

        [Test]
        public void IsChanged_WhenValueChangedAndNotInInit_ShouldBeTrue()
        {
            var vm = new MyViewModelWithOwnBacking { IsEnabled = true };

            vm.IsChanged
                .Should()
                .BeTrue();
        }

        [Test]
        public void IsChanged_WhenInUpdateMode_ShouldBeTrue()
        {
            var vm = new MyViewModelWithOwnBacking();
            vm.BeginUpdate();
            vm.IsEnabled = true;
            vm.EndUpdate();

            vm.IsChanged
                .Should()
                .BeTrue();
        }

        [Test]
        public void EndUpdate_WhenCalledExpectDeferredPropertiesEventsToOccur()
        {
            var properties = new List<string>();

            var vm = new MyViewModelWithOwnBacking();
            vm.PropertyChanged += (sender, args) =>
            {
                properties.Add(args.PropertyName);
            };

            vm.BeginUpdate();
            vm.IsEnabled = true;
            vm.CannotBeNegative = 100;
            vm.Message = "Test";
            vm.EndUpdate();

            // three properties above change but also the IsChanged property, so + 1
            properties.Count
                .Should()
                .Be(4);

            properties[0]
                .Should()
                .Be("IsChanged");

            properties[1]
                .Should()
                .Be("IsEnabled");

            properties[2]
                .Should()
                .Be("CannotBeNegative");

            properties[3]
                .Should()
                .Be("Message");
        }

        [Test]
        public void EndUpdate_WhenCalledExpectDeferredPropertiesEventsToOccur_IfMultipleChangesToSamePropertyExpectOnlyOneChange()
        {
            var properties = new List<string>();

            var vm = new MyViewModelWithOwnBacking();
            vm.PropertyChanged += (sender, args) =>
            {
                properties.Add(args.PropertyName);
            };

            vm.BeginUpdate();
            vm.Message = "T";
            vm.IsEnabled = true;
            vm.CannotBeNegative = 100;
            vm.Message = "Test";
            vm.Message = "Test2";
            vm.EndUpdate();

            // we need to + 1 to the number of properyt changes to include IsChanged
            properties.Count
                .Should()
                .Be(4);

            properties[0]
                .Should()
                .Be("IsChanged");

            properties[1]
                .Should()
                .Be("Message");

            properties[2]
                .Should()
                .Be("IsEnabled");

            properties[3]
                .Should()
                .Be("CannotBeNegative");
        }


        [Test]
        public void IsBusy_InternalPropertiesShouldNotBeIncludedInTracking_ExpectFalse()
        {
            var vm = new MyViewModelWithOwnBacking { IsBusy = true };

            vm.IsChanged
                .Should()
                .BeFalse();
        }

        [Test]
        public void EnsureNonTrackedPropertiesAreNotTracked()
        {
            var vm = new MyViewModelWithOwnBacking { NotTracked = true };

            vm.IsChanged
                .Should()
                .BeFalse();
        }

        [Test]
        public void DefaultValue_ExpectPropertyWithDefaultToHaveThatValue()
        {
            var vm = new MyViewModelWithOwnBacking();

            vm.Message
                .Should()
                .Be("Hello World");
        }

        [Test]
        public void DefaultValue_ExpectPropertyWithDefaultToHaveStillBeChanged()
        {
            var vm = new MyViewModelWithOwnBacking();

            vm.Message = "Scooby Doo";

            vm.Message
                .Should()
                .Be("Scooby Doo");
        }

        [Test]
        public void DefaultValue_BadDefault()
        {
            var vm = new MyViewModelWithOwnBacking();

            vm.BadDefault
                .Should()
                .BeFalse();
        }

        [Test]
        public void Validation_EnterInvalidData_ExpectError()
        {
            var vm = new MyViewModelWithOwnBacking { CannotBeNegative = -100 };

            var dataErrorInfo = (IDataErrorInfo)vm;
            var error = dataErrorInfo["CannotBeNegative"];

            error
                .Should()
                .Be("Value cannot be negative");
        }

        [Test]
        public void Undo_WhenNoChangesAreMade()
        {
            var vm = new MyViewModelWithOwnBacking();

            vm.Undo()
                .Should()
                .BeFalse();
        }

        [Test]
        public void Undo_WhenChangesAreMade()
        {
            var vm = new MyViewModelWithOwnBacking { Message = "Scooby Doo" };

            vm.Message
                .Should()
                .Be("Scooby Doo");

            vm.Undo()
                .Should()
                .BeTrue();

            vm.Message
                .Should()
                .Be("Hello World");
        }

        [Test]
        public void Undo_MultiplePropertyUndo()
        {
            var vm = new MyViewModelWithOwnBacking
            {
                Message = "Scooby Doo",
                CannotBeNegative = 123,
            };

            vm.Message
                .Should()
                .Be("Scooby Doo");

            vm.CannotBeNegative
                .Should()
                .Be(123);

            vm.Undo()
                .Should()
                .BeTrue();

            vm.Undo()
                .Should()
                .BeTrue();

            vm.Message
                .Should()
                .Be("Hello World");

            vm.CannotBeNegative
                .Should()
                .Be(0);
        }
    }

}
