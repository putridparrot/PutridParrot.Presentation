using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation.Core;
using Tests.Presentation.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class AsyncCommandTests
    {
        [Test]
        public void Constructor_DefaultCtor_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand();
            cmd.IsBusy
                .Should()
                .BeFalse();
        }

        [Test]
        public void Constructor_Overload1_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand<string>(o => Task.FromResult<object>(null));
            cmd.IsBusy
                .Should()
                .BeFalse();
        }

        [Test]
        public void Constructor_Overload2_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand<string>(o => Task.FromResult<object>(null), o => Task.FromResult(true));
            cmd.IsBusy
                .Should()
                .BeFalse();
        }

        [Test]
        public void Constructor_Overload3_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand(() => Task.FromResult<object>(null), () => Task.FromResult(true));
            cmd.IsBusy
                .Should()
                .BeFalse();
        }

        [Test]
        public void IsBusy_ExpectTrueWhenExecutingFunc_ShouldPropertyChangeTwice()
        {
            var cmd = new AsyncCommand(() => Task.FromResult<object>(null), () => Task.FromResult(true));
            var nl = new NotifyPropertyChangedListener(cmd);
            cmd.Execute(null);
            cmd.ExecuteCommand = () => { return Task.CompletedTask; };

            // should change to true and then to false upon completion
            nl.Changed.Count
                .Should()
                .Be(2);
            nl.Changed[0]
                .Should()
                .Be("IsBusy");
            nl.Changed[1]
                .Should()
                .Be("IsBusy");
        }
    }
}
