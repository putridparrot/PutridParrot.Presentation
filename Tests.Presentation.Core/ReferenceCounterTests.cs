using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ReferenceCounterTests
    {
        [Test]
        public void AddRef_AddASingleReference_ExpectReturnOfOne()
        {
            var rc = new ReferenceCounter();
            rc.AddRef()
                .Should()
                .Be(1);
        }

        [Test]
        public void AddRef_AddFourTimes_ExpectReturnOfFour()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();

            rc.AddRef()
                .Should()
                .Be(4);
        }

        [Test]
        public void Count_InitialState_ExpectZero()
        {
            var rc = new ReferenceCounter();
            rc.Count
                .Should()
                .Be(0);
        }

        [Test]
        public void Count_AddASingleReference_ExpectCountOfOne()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.Count
                .Should()
                .Be(1);
        }

        [Test]
        public void Count_AddFourTimes_ExpectReturnCountOfFour()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();

            rc.Count
                .Should()
                .Be(4);
        }

        [Test]
        public void Count_ReleaseOnZeroCount_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            rc.Release();
            rc.Count
                .Should()
                .Be(0);
        }

        [Test]
        public void Count_AddRefThenRelease_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.Release();
            rc.Count
                .Should()
                .Be(0);
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
            rc.Count
                .Should()
                .Be(3);
        }

        [Test]
        public void Release_OnZeroCount_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            rc.Release()
                .Should()
                .Be(0);
        }

        [Test]
        public void Release_AddRefThenRelease_ExpectZeroReturn()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.Release()
                .Should()
                .Be(0);
        }

        [Test]
        public void Release_AddRefFourTimesThenRelease_ExpectZeroThree()
        {
            var rc = new ReferenceCounter();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            rc.AddRef();
            rc.Release()
                .Should()
                .Be(3);
        }
    }
}
