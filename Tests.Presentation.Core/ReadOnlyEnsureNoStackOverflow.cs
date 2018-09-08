using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation.Core;
using PutridParrot.Presentation.Core.Exceptions;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ReadOnlyEnsureNoStackOverflow
    {
        class TestViewModel : ViewModel
        {
            [DefaultValue(2)]
            public int Value1
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }

            public int Result
            {
                get { return ReadOnlyProperty(() => Value1 + Result); }
            }
        }

        [Test]
        public void CallRecursiveProperty_PotentialStackOverflow_ShouldInsteadRaisePropertyCannotCallItselfException()
        {
            var vm = new TestViewModel();

            Action action = () =>
            {
                var r = vm.Result;
            };
            action
                .Should()
                .Throw<PropertyCannotCallItselfException>();
        }
    }
}