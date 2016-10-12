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
    public class ExtendedObservableCollectionTests
    {
        [Test]
        public void AddRange_CheckWeCanAddMultipleItems()
        {
            var items = new[] { 1, 2, 3, 4, 5 };

            var c = new ExtendedObservableCollection<int>();
            c.AddRange(items);

            Assert.AreEqual(items.Length, c.Count);
        }

        [Test]
        public void AddRange_ShouldNotFireMultipleCollectionChangedEvents()
        {
            var items = new[] { 1, 2, 3, 4, 5 };

            var c = new ExtendedObservableCollection<int>();

            int eventCount = 0;
            c.CollectionChanged += (sender, args) =>
            {
                eventCount++;
            };

            c.AddRange(items);

            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void AddRange_NullEnumerably_ShouldException()
        {
            var c = new ExtendedObservableCollection<int>();
            Assert.Throws<ArgumentNullException>(() => c.AddRange(null));
        }
    }

}
