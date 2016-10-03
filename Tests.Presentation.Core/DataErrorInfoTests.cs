using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DataErrorInfoTests
    {
        [Test]
        public void Constructor_ExpectEmptyErrors()
        {
            var dataErrorInfo = new DataErrorInfo();
            Assert.AreEqual(0, dataErrorInfo.Errors.Length);
        }

        [Test]
        public void Constructor_ExpectEmptyProperties()
        {
            var dataErrorInfo = new DataErrorInfo();
            Assert.AreEqual(0, dataErrorInfo.Errors.Length);
        }

        [Test]
        public void Add_AddANewPropertyError_ExpectPropertyToExist()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            Assert.AreEqual(ERR, dataErrorInfo[PROP]);
        }

        [Test]
        public void Add_AddANewPropertyError_ExpectPropertiesToIncreaseByOne()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            Assert.AreEqual(1, dataErrorInfo.Properties.Length);
        }

        [Test]
        public void Add_AddANewPropertyError_ExpectErrorsToIncreaseByOne()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            Assert.AreEqual(1, dataErrorInfo.Errors.Length);
        }

        [Test]
        public void Remove_NonExistentProperty_ShouldReturnFalse()
        {
            const string PROP = "Prop1";

            var dataErrorInfo = new DataErrorInfo();

            Assert.IsFalse(dataErrorInfo.Remove(PROP));
        }

        [Test]
        public void Remove_ExistingProperty_ShouldReturnTrue()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            Assert.IsTrue(dataErrorInfo.Remove(PROP));
        }

        [Test]
        public void Clear_ShouldReturnTrue()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            Assert.IsTrue(dataErrorInfo.Clear());
        }

        [Test]
        public void Clear_ShouldHaveNoProperties()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Clear();

            Assert.AreEqual(0, dataErrorInfo.Properties.Length);
        }

        [Test]
        public void Clear_ShouldHaveNoErrors()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo.Clear();

            Assert.AreEqual(0, dataErrorInfo.Errors.Length);
        }

        [Test]
        public void Item_SetterWithNoExistingProperty_ExpectItToBeAdded()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo[PROP] =  ERR;

            Assert.AreEqual(1, dataErrorInfo.Errors.Length);
        }

        [Test]
        public void Item_SetterWithExistingProperty_ExpectItToBeChanged()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo[PROP] = ERR + "!";

            Assert.AreEqual(1, dataErrorInfo.Errors.Length);
        }

        [Test]
        public void Item_SetterWithExistingPropertyToNull_ExpectItToBeRemoved()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo[PROP] = null;

            Assert.AreEqual(0, dataErrorInfo.Errors.Length);
        }

        [Test]
        public void Item_SetterWithExistingPropertyToEmpty_ExpectItToBeRemoved()
        {
            const string PROP = "Prop1";
            const string ERR = "Prop1Error";

            var dataErrorInfo = new DataErrorInfo();
            dataErrorInfo.Add(PROP, ERR);

            dataErrorInfo[PROP] = String.Empty;

            Assert.AreEqual(0, dataErrorInfo.Errors.Length);
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

            Assert.AreEqual("Prop1Error\r\nProp2Error\r\n", dataErrorInfo.Error);
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

            dataErrorInfo.Error = "Errors!";

            Assert.AreEqual("Errors!", dataErrorInfo.Error);
        }
    }
}
