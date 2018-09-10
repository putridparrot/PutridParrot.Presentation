using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using PutridParrot.Presentation.Shared;
using Tests.Presentation.Helpers;

namespace Tests.PutridParrot.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ChangeTrackingObservableCollectionTests
    {
        [Test]
        public void EnsureInitialChangedStateIsFalse()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();
            Assert.IsFalse(c.IsChanged);
        }

        [Test]
        public void EnsureInitialTrackedChangesIsNull()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();
            Assert.IsNull(c.GetTrackedChanges());
        }

        [Test]
        public void Initialization_ShouldBeAddChanges()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.BeginInit();
            c.Add(new MyViewModel());
            c.EndInit();

            //Assert.IsFalse(c.IsChanged);
            Assert.IsNull(c.GetTrackedChanges());
        }

        [Test]
        public void AddItem_ExpectChangesToUpdate()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());

            //Assert.IsFalse(c.IsChanged);
            Assert.AreEqual(3, c.GetTrackedChanges().Count());
        }


        [Test]
        public void AddItem_ExpectChangesToBeMarkedAsAdded()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());

            var tracked = c.GetTrackedChanges().ToDictionary(i => i.Key);
            foreach (var item in c)
            {
                Assert.IsTrue(tracked.ContainsKey(item));
                Assert.AreEqual(TrackedState.Added, tracked[item].Value);
            }
        }


        [Test]
        public void EditItems_ExpectChangesToBeMarkedAsEdited()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.BeginInit();
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.EndInit();

            c[1].Message = "A new message should cause an edit";

            var tracked = c.GetTrackedChanges();
            Assert.AreEqual(c[1], tracked[0].Key);
            Assert.AreEqual(TrackedState.Edited, tracked[0].Value);
        }

        [Test]
        public void AddThenDeleteItem_ExpectNoLongerInChanges()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.BeginInit();
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.EndInit();

            c.Add(new MyViewModel());
            c.RemoveAt(2);

            var tracked = c.GetTrackedChanges();
            Assert.IsNull(tracked);
        }

        [Test]
        public void AddThenEditItem_ExpectTrackingToBeEdit()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.BeginInit();
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.EndInit();

            c.Add(new MyViewModel());
            c[2].Message = "This is a new message";

            var tracked = c.GetTrackedChanges();
            Assert.AreEqual(1, tracked.Length);
            Assert.AreEqual(TrackedState.Added, tracked[0].Value);
        }

        [Test]
        public void EditThenEditItem_ExpectTrackingToBeEdit()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.BeginInit();
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.EndInit();

            c[2].Message = "This is a new message";
            c[2].Message = "Changed the message";

            var tracked = c.GetTrackedChanges();
            Assert.AreEqual(1, tracked.Length);
            Assert.AreEqual(TrackedState.Edited, tracked[0].Value);
        }


        [Test]
        public void EditThenDeleteItem_ExpectTrackingToBeDeleted()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.BeginInit();
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.EndInit();

            c[2].Message = "This is a new message";
            c.RemoveAt(2);

            var tracked = c.GetTrackedChanges();
            Assert.AreEqual(1, tracked.Length);
            Assert.AreEqual(TrackedState.Deleted, tracked[0].Value);
        }

        [Test]
        public void ResetChanges_ExpectChangesToBeRemoved()
        {
            var c = new ChangeTrackingObservableCollection<MyViewModel>();

            c.Add(new MyViewModel());
            c.Add(new MyViewModel());
            c.Add(new MyViewModel());

            c.ResetChanges();

            Assert.IsNull(c.GetTrackedChanges());
            Assert.IsFalse(c.IsChanged);
        }
    }

}
