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
    public class AsyncCommandTests
    {
        [Test]
        public void Constructor_DefaultCtor_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand();
            Assert.IsFalse(cmd.IsBusy);
        }

        [Test]
        public void Constructor_Overload1_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand(o => Task.FromResult<object>(null));
            Assert.IsFalse(cmd.IsBusy);
        }

        [Test]
        public void Constructor_Overload2_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand(o => Task.FromResult<object>(null), o => Task.FromResult(true));
            Assert.IsFalse(cmd.IsBusy);
        }

        [Test]
        public void Constructor_Overload3_ExpectDefaultIsBusy_ToBeFalse()
        {
            var cmd = new AsyncCommand(() => Task.FromResult<object>(null), () => Task.FromResult(true));
            Assert.IsFalse(cmd.IsBusy);
        }

        [Test]
        public void IsBusy_ExpectTrueWhenExecutingFunc_ShouldPropertyChangeTwice()
        {
            var cmd = new AsyncCommand(() => Task.FromResult<object>(null), () => Task.FromResult(true));
            var nl = new NotifyPropertyChangedListener(cmd);
            cmd.Execute(null);

            // should change to true and then to false upon completion
            Assert.AreEqual(2, nl.Changed.Count);
            Assert.AreEqual("IsBusy", nl.Changed[0]);
            Assert.AreEqual("IsBusy", nl.Changed[1]);
        }
    }
}
