using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation.Helpers;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class SafeConvertTests
    {
        [Test]
        public void ChangeType_FailChangeWithoutDefault_ExpectDefaultOfType()
        {
            // cannot change an int to a MyViewModel
            var result = SafeConvert.ChangeType<MyViewModel>(123);

            result
                .Should()
                .Be(default(MyViewModel));
        }

        [Test]
        public void ChangeType_FailChangeWithDefault_ExpectAssignedDefault()
        {
            var vm = new MyViewModel();
            // cannot change an int to a MyViewModel
            var result = SafeConvert.ChangeType<MyViewModel>(123, vm);

            result
                .Should()
                .Be(vm);
        }

        [Test]
        public void ChangeType_NativeTypeToNullable_ExpectTypeChange()
        {
            var result = SafeConvert.ChangeType<bool?>(true);

            result.HasValue
                .Should()
                .BeTrue();

            result.Value
                .Should()
                .BeTrue();
        }

        [Test]
        public void ChangeType_NullableToNativeType_ExpectTypeChange()
        {
            bool? value = true;
            var result = SafeConvert.ChangeType<bool>(value);

            result
                .Should()
                .BeTrue();
        }

        [Test]
        public void ChangeType_NonStringToString_ExpectToStringRepresentation()
        {
            var now = DateTime.Now;

            var result = SafeConvert.ChangeType<string>(now);

            result
                .Should()
                .Be(now.ToString());
        }

        [Test]
        public void ChangeType_FromStringToType_ExpectToTypeRepresentation()
        {
            var result = SafeConvert.ChangeType<DateTime>("12/2/2016");

            result
                .Should()
                .Be(new DateTime(2016, 2, 12));
        }
    }
}
