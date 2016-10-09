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
    public class ComparerImplTests
    {
        [Test]
        public void Constructor_WithNullComparer_ExpectException()
        {
            Assert.Throws<NullReferenceException>(() => new ComparerImpl<string>(null));
        }

        [Test]
        public void Compare_EqualValues_ExpectReturnIsZero()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.AreEqual(0, comparer.Compare("Hello", "Hello"));
        }

        [Test]
        public void Compare_NotEqualValues_ExpectReturnIsNotZero()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.AreNotEqual(0, comparer.Compare("Hello1", "Hello2"));
        }

        [Test]
        public void CompareObjects_EqualValues_ExpectReturnIsZero()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.AreEqual(0, comparer.Compare((object)"Hello", (object)"Hello"));
        }

        [Test]
        public void ComparObjectse_NotEqualValues_ExpectReturnIsNotZero()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.AreNotEqual(0, comparer.Compare((object)"Hello1", (object)"Hello2"));
        }

        [Test]
        public void Equals_EqualValues_ExpectReturnIsZero()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.True(comparer.Equals("Hello", "Hello"));
        }

        [Test]
        public void Equals_NotEqualValues_ExpectReturnIsNotZero()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.False(comparer.Equals("Hello1", "Hello2"));
        }

        [Test]
        public void GetHasCode_ExpectZeroForNullObject()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.AreEqual(0, comparer.GetHashCode(null));
        }

        [Test]
        public void GetHasCode_ExpectZeroByDefault()
        {
            var comparer = new ComparerImpl<string>((a, b) => a.CompareTo(b));
            Assert.AreEqual(0, comparer.GetHashCode("Hello"));
        }
    }

}
