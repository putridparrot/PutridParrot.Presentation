using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PutridParrot.Presentation;

namespace Tests.PutridParrot.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class FixedSizeObservableCollectionTests
    {
        [Test]
        public void Constructor_Default_ExpectNoItemsEvenWithSizeSet()
        {
            var o = new FixedSizeObservableCollection<string>(3);

            o.Count
                .Should()
                .Be(0);
        }

        [Test]
        public void Size_EnsureSetCorrectly()
        {
            var o = new FixedSizeObservableCollection<string>(3);

            o.Size
                .Should()
                .Be(3);
        }

        [Test]
        public void AddRange_WithLargerThanSizeDataExpectLastNItemsOnly()
        {
            var o = new FixedSizeObservableCollection<string>(3);

            o.AddRange(new []{ "One", "Two", "Three", "Four", "Five"});

            o.Count
                .Should()
                .Be(3);

            o[0]
                .Should()
                .Be("Three");
            o[1]
                .Should()
                .Be("Four");
            o[2]
                .Should()
                .Be("Five");
        }

        [Test]
        public void Clear_ExpectCountToChangeButSizeUnchanged()
        {
            var o = new FixedSizeObservableCollection<string>(3);

            o.AddRange(new[] { "One", "Two", "Three", "Four", "Five" });

            o.Clear();

            o.Count
                .Should()
                .Be(0);
            o.Size
                .Should()
                .Be(3);
        }
    }
}
