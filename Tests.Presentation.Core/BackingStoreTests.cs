using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class BackingStoreTests
    {
        [Test]
        public void Set_WithNewValue_ExpectOnChangingFuncCall()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            backingStore.Set("Test", "Some Value", (cv, nv, pn) =>
            {
                called = true;
                return true;
            }, null);

            Assert.IsTrue(called);
        }

        [Test]
        public void Set_WithExistingDefaultValue_ExpectNoOnChangingFuncCall()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            backingStore.Set<string>("Test", null, (cv, nv, pn) =>
            {
                called = true;
                return true;
            }, null);

            Assert.IsFalse(called);
        }

        [Test]
        public void Set_WithNewValue_ExpectOnChangedFuncCall()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            backingStore.Set("Test", "Some Value", null,
                (cv, nv, pn) =>
            {
                called = true;
            });

            Assert.IsTrue(called);
        }

        [Test]
        public void Set_WithExistingDefaultValue_ExpectNoOnChangedFuncCall()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            backingStore.Set<string>("Test", null, null,
                (cv, nv, pn) =>
                {
                    called = true;
                });

            Assert.IsFalse(called);
        }

        [Test]
        public void Set_WhilstInBeginInitState_ExpectNoOnChangingCall()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            var init = backingStore as ISupportInitialize;

            init.BeginInit();
            backingStore.Set("Test", "Some Value", (cv, nv, pn) =>
            {
                called = true;
                return true;
            }, null);
            init.EndInit();

            Assert.IsFalse(called);
        }

        [Test]
        public void Set_WhilstInBeginInitState_ExpectNoOnChangedCall()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value", null, (cv, nv, pn) =>
            {
                called = true;
            });
            init.EndInit();

            Assert.IsFalse(called);
        }

        [Test]
        public void Set_WhilstInBeginInitState_ExpectPropertyToBeSet()
        {
            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value");
            init.EndInit();

            Assert.AreEqual("Some Value", backingStore.Get<string>("Test"));
        }

        [Test]
        public void Get_WhenNoValueSet_ExpectDefaultForType()
        {
            IBackingStore backingStore = new BackingStore();
            Assert.AreEqual(default(bool), backingStore.Get<bool>("Test"));
        }

        [Test]
        public void RejectChanges_MakeChangesThenReject_ExpectOriginalValues()
        {
            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;
            var revertible = (ISupportRevertibleChangeTracking) backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value");
            init.EndInit();

            backingStore.Set("Test", "New Value");

            revertible.RejectChanges(null, null);

            Assert.AreEqual("Some Value", backingStore.Get<string>("Test"));
        }

        [Test]
        public void RejectChanges_MakeChangesThenReject_ExpectChangingFuncToBeCalled()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;
            var revertible = (ISupportRevertibleChangeTracking)backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value");
            init.EndInit();

            backingStore.Set("Test", "New Value");

            revertible.RejectChanges(pn => called = true, null);

            Assert.True(called);
        }

        [Test]
        public void RejectChanges_MakeChangesThenReject_ExpectChangedFuncToBeCalled()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;
            var revertible = (ISupportRevertibleChangeTracking)backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value");
            init.EndInit();

            backingStore.Set("Test", "New Value");

            revertible.RejectChanges(null, pn => called = true);

            Assert.True(called);
        }

        [Test]
        public void AcceptChanges_MakeChangesThenAccept_ExpectNewValueEventWhenNextRejecting()
        {
            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;
            var revertible = (ISupportRevertibleChangeTracking)backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value");
            init.EndInit();

            backingStore.Set("Test", "New Value");

            revertible.AcceptChanges(null, null);
            revertible.RejectChanges(null, null);

            Assert.AreEqual("New Value", backingStore.Get<string>("Test"));
        }

        [Test]
        public void AcceptChanges_MakeChangesThenAccept_ExpectOnChangingTobeCalled()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;
            var revertible = (ISupportRevertibleChangeTracking)backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value");
            init.EndInit();

            backingStore.Set("Test", "New Value");

            revertible.AcceptChanges(pn => called = true, null);

            Assert.True(called);
        }

        [Test]
        public void AcceptChanges_MakeChangesThenAccept_ExpectOnChangedTobeCalled()
        {
            var called = false;

            IBackingStore backingStore = new BackingStore();
            var init = (ISupportInitialize)backingStore;
            var revertible = (ISupportRevertibleChangeTracking)backingStore;

            init.BeginInit();
            backingStore.Set("Test", "Some Value");
            init.EndInit();

            backingStore.Set("Test", "New Value");

            revertible.AcceptChanges(null, pn => called = true);

            Assert.True(called);
        }
    }
}
