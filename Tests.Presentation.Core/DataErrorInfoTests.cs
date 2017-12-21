using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DataErrorInfoTests
    {
        [Test]
        public void Constructor_ExpectEmptyErrors()
        {
            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Errors
                .Should()
                .BeNull();
        }

        [Test]
        public void Constructor_ExpectEmptyProperties()
        {
            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Properties
                .Should()
                .BeNull();
        }

        [Test]
        public void Add_AddANewPropertyError_ExpectPropertyToExist()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo[PROP]
                .Should()
                .Be(ERR);
        }

        [Test]
        public void Add_AddANewPropertyError_ExpectPropertiesToIncreaseByOne()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Properties.Length
                .Should()
                .Be(1);
        }

        [Test]
        public void Add_AddANewPropertyError_ExpectErrorsToIncreaseByOne()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Errors.Length
                .Should()
                .Be(1);
        }

        [Test]
        public void Remove_NonExistentProperty_ShouldReturnFalse()
        {
            const string PROP = "Prop1";

            var dataErrorInfo = new DataErrorInfo();

            dataErrorInfo.Remove(PROP)
                .Should()
                .BeFalse();
        }

        [Test]
        public void Remove_ExistingProperty_ShouldReturnTrue()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Remove(PROP)
                .Should()
                .BeTrue();
        }

        [Test]
        public void Clear_ShouldReturnTrue()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Clear()
                .Should()
                .BeTrue();
        }

        [Test]
        public void Clear_ShouldHaveNoProperties()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Clear();

            dataErrorInfo.Properties.Length
                .Should()
                .Be(0);
        }

        [Test]
        public void Clear_ShouldHaveNoErrors()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Clear();

            dataErrorInfo.Errors.Length
                .Should()
                .Be(0);
        }

        [Test]
        public void Item_SetterWithNoExistingProperty_ExpectItToBeAdded()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo {[PROP] = ERR};

            dataErrorInfo.Errors.Length
                .Should()
                .Be(1);
        }

        [Test]
        public void Item_SetterWithExistingProperty_ExpectItToBeChanged()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo[PROP] = ERR + "!";

            dataErrorInfo.Errors.Length
                .Should()
                .Be(1);
        }

        [Test]
        public void Item_SetterWithExistingPropertyToNull_ExpectItToBeRemoved()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo[PROP] = null;

            dataErrorInfo.Errors.Length
                .Should()
                .Be(0);
        }

        [Test]
        public void Item_SetterWithExistingPropertyToEmpty_ExpectItToBeRemoved()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo[PROP] = String.Empty;

            dataErrorInfo.Errors.Length
                .Should()
                .Be(0);
        }

        [Test]
        public void Error_WhenErrorsExist_ExpectConcatenationOfErrors()
        {
            const string PROP1 = "Prop1";
            const string ERR1 = "Prop1Error";
            const string PROP2 = "Prop2";
            const string ERR2 = "Prop2Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP1, ERR1);
            dataErrorInfo.Add(PROP2, ERR2);

            dataErrorInfo.Error
                .Should()
                .Be("Prop1Error\r\nProp2Error\r\n");
        }

        [Test]
        public void Error_WhenErrorsExistButOverriddenBySuppliedError_ExpectAssignedError()
        {
            const string PROP1 = "Prop1";
            const string ERR1 = "Prop1Error";
            const string PROP2 = "Prop2";
            const string ERR2 = "Prop2Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP1, ERR1);
            dataErrorInfo.Add(PROP2, ERR2);

            dataErrorInfo.SetError("Errors!");

            dataErrorInfo.Error
                .Should()
                .Be("Errors!");
        }
    }
}
