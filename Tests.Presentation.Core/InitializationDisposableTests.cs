using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation.Core;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class InitializationDisposableTests
    {
        class SomeObject : ISupportInitialize
        {
            public void BeginInit()
            {
                Counter++;
            }

            public void EndInit()
            {
                Counter--;
            }

            public int Counter { get; private set; }
        }

        [Test]
        public void BeginInit_ExpectToBeCalledWhenConstructed()
        {
            var o = new SomeObject();

            o.Counter
                .Should()
                .Be(0);
            
            using (new InitializationDisposable(o))
            {
                o.Counter
                    .Should()
                    .Be(1);
            }
        }

        [Test]
        public void EndInit_ExpectToBeCalledWhenDisposed()
        {
            var o = new SomeObject();

            o.Counter
                .Should()
                .Be(0);

            using (new InitializationDisposable(o))
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

            using (new InitializationDisposable(o))
            {
                o.Counter
                    .Should()
                    .Be(1);

                using (new InitializationDisposable(o))
                {
                    o.Counter
                        .Should()
                        .Be(2);

                    using (new InitializationDisposable(o))
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
