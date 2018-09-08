using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ActionCommandTests
    {
        [Test]
        public void Execute_EnsureExecuteCallTheExecuteMethod()
        {
            var counter = 0;
            var action = new ActionCommand(() => counter++);

            action.Execute(null);

            counter
                .Should()
                .Be(1);
        }

        [Test]
        public void CanExecute_EnsureCanExecuteCallsSuppliedCanExecuteMethod()
        {
            var counter = 0;
            var action = new ActionCommand(() => { }, () => counter++ > 0);

            action.CanExecute(null);

            counter
                .Should()
                .Be(1);
        }

        [Test]
        public void Execute_WithGenericParam_EnsureExecuteCallTheExecuteMethod()
        {
            var counter = 0;
            var action = new ActionCommand<int>(i => counter++);

            action.Execute(null);

            counter
                .Should()
                .Be(1);
        }

        [Test]
        public void CanExecute_WithGenericParam_EnsureCanExecuteCallsSuppliedCanExecuteMethod()
        {
            var counter = 0;
            var action = new ActionCommand<int>(i => { }, i => counter++ > 0);

            action.CanExecute(null);

            counter
                .Should()
                .Be(1);
        }
    }
}