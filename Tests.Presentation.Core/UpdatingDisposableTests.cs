using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Patterns;
using Presentation.Patterns.Interfaces;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class UpdatingDisposableTests
    {
        class SomeObject : ISupportUpdate
        {
            public void BeginUpdate()
            {
                Counter++;
            }

            public void EndUpdate()
            {
                Counter--;
            }

            public bool IsUpdating => Counter > 0;

            public int Counter { get; private set; }
        }

        [Test]
        public void BeginUpdate_ExpectToBeCalledWhenConstructed()
        {
            var o = new SomeObject();

            o.Counter
                .Should()
                .Be(0);

            using (new UpdatingDisposable(o))
            {
                o.Counter
                    .Should()
                    .Be(1);
            }
        }

        [Test]
        public void EndUpdate_ExpectToBeCalledWhenDisposed()
        {
            var o = new SomeObject();

            o.Counter
                .Should()
                .Be(0);

            using (new UpdatingDisposable(o))
            {
            }

            o.Counter
                .Should()
                .Be(0);
        }

        [Test]
        public void NestedCalls()
        {
            var o = new SomeObject();

            o.Counter
                .Should()
                .Be(0);

            using (new UpdatingDisposable(o))
            {
                o.Counter
                    .Should()
                    .Be(1);

                using (new UpdatingDisposable(o))
                {
                    o.Counter
                        .Should()
                        .Be(2);

                    using (new UpdatingDisposable(o))
                    {
                        o.Counter
                            .Should()
                            .Be(3);
                    }

                    o.Counter
                        .Should()
                        .Be(2);
                }

                o.Counter
                    .Should()
                    .Be(1);
            }

            o.Counter
                .Should()
                .Be(0);
        }
    }
}