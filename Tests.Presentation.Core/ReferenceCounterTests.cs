using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Presentation.Core;

namespace Tests.Presentation.Core
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ReferenceCounterTests
    {
        [Test]
        public void AddRef_AddASingleReference_ExpectReturnOfOne()
        {
            var rc = new ReferenceCounter();
            Assert.AreEqual(1, rc.AddRef());
        }

        [Test]
        public void AddRef_AddFourTimes_ExpectReturnOfFour()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();

            Assert.AreEqual(4, rc.AddRef());
        }

        [Test]
        public void Count_InitialState_ExpectZero()
        {
            var rc = new ReferenceCounter();
            Assert.AreEqual(0, rc.Count);
        }

        [Test]
        public void Count_AddASingleReference_ExpectCountOfOne()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            Assert.AreEqual(1, rc.Count);
        }

        [Test]
        public void Count_AddFourTimes_ExpectReturnCountOfFour()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();

            Assert.AreEqual(4, rc.Count);
        }

        [Test]
        public void Count_ReleaseOnZeroCount_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            rc.Release();
            Assert.AreEqual(0, rc.Count);
        }

        [Test]
        public void Count_AddRefThenRelease_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.Release();
            Assert.AreEqual(0, rc.Count);
        }

        [Test]
        public void Count_AddRefFourTimesThenRelease_ExpectZeroThree()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            rc.Release();
            Assert.AreEqual(3, rc.Count);
        }

        [Test]
        public void Release_OnZeroCount_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            Assert.AreEqual(0, rc.Release());
        }

        [Test]
        public void Release_AddRefThenRelease_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            Assert.AreEqual(0, rc.Release());
        }

        [Test]
        public void Release_AddRefFourTimesThenRelease_ExpectZeroThree()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            Assert.AreEqual(3, rc.Release());
        }
    }

}
