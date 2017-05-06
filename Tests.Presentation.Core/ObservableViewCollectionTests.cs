using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Patterns;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ObservableViewCollectionTests
    {
        [Test]
        public void WithoutFilterSupplied_ExpectFilteredToBeAllItems()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filtered.Count
                .Should()
                .Be(4);

            collection.Filtered[0]
                .Should()
                .Be("One");
            collection.Filtered[1]
                .Should()
                .Be("Two");
            collection.Filtered[2]
                .Should()
                .Be("Three");
            collection.Filtered[3]
                .Should()
                .Be("Four");
        }

        [Test]
        public void AfterFilterRemoved_ExpectFilteredToBeAllItems()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            collection.Filtered.Count
                .Should()
                .Be(2);

            // clear the filter
            collection.Filter = null;

            collection.Filtered.Count
                .Should()
                .Be(4);

            collection.Filtered[0]
                .Should()
                .Be("One");
            collection.Filtered[1]
                .Should()
                .Be("Two");
            collection.Filtered[2]
                .Should()
                .Be("Three");
            collection.Filtered[3]
                .Should()
                .Be("Four");
        }

        [Test]
        public void WithFilterSupplied_ExpectFilteredToShowOnlyThoseItemsThatMatchFilter()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            collection.Filtered.Count
                .Should()
                .Be(2);

            collection.Filtered[0]
                .Should()
                .Be("Two");
            collection.Filtered[1]
                .Should()
                .Be("Three");
        }

        [Test]
        public void WithFilterSupplied_AddFilteredOutItem_ExpectFilteredUnChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection.Add("Five");

            collection.Filtered.Count
                .Should()
                .Be(2);

            collection.Filtered[0]
                .Should()
                .Be("Two");
            collection.Filtered[1]
                .Should()
                .Be("Three");
        }

        [Test]
        public void WithFilterSupplied_AddFilteredInItem_ExpectFilteredChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection.Add("Ten");

            collection.Filtered.Count
                .Should()
                .Be(3);

            collection.Filtered[0]
                .Should()
                .Be("Two");
            collection.Filtered[1]
                .Should()
                .Be("Three");
            collection.Filtered[2]
                .Should()
                .Be("Ten");
        }

        [Test]
        public void WithFilterSupplied_RemoveAlteredOutItem_ExpectFilteredUnChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection.Remove("One");

            collection.Filtered.Count
                .Should()
                .Be(2);

            collection.Filtered[0]
                .Should()
                .Be("Two");
            collection.Filtered[1]
                .Should()
                .Be("Three");
        }

        [Test]
        public void WithFilterSupplied_RemoveFilteredInItem_ExpectFilteredChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection.Remove("Two");

            collection.Filtered.Count
                .Should()
                .Be(1);

            collection.Filtered[0]
                .Should()
                .Be("Three");
        }

        [Test]
        public void WithFilterSupplied_ChangedFilteredOutItemToFilteredOut_ExpectFilteredUnChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection[0] = "Eleven";

            collection.Filtered.Count
                .Should()
                .Be(2);

            collection.Filtered[0]
                .Should()
                .Be("Two");
            collection.Filtered[1]
                .Should()
                .Be("Three");
        }

        [Test]
        public void WithFilterSupplied_ChangedFilteredOutItemToFilteredIn_ExpectFilteredChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection[0] = "Ten";

            collection.Filtered.Count
                .Should()
                .Be(3);

            collection.Filtered[0]
                .Should()
                .Be("Ten");
            collection.Filtered[1]
                .Should()
                .Be("Two");
            collection.Filtered[2]
                .Should()
                .Be("Three");
        }

        [Test]
        public void WithFilterSupplied_ChangedFilteredInItemToFilteredIn_ExpectFilteredChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection[2] = "Ten";

            collection.Filtered.Count
                .Should()
                .Be(2);

            collection.Filtered[0]
                .Should()
                .Be("Two");
            collection.Filtered[1]
                .Should()
                .Be("Ten");
        }

        [Test]
        public void WithFilterSupplied_ChangedFilteredInItemToFilteredOut_ExpectFilteredChanged()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Filter = s => s.StartsWith("T");

            // force an lazy load
            var filtered = collection.Filtered;

            // now change collection
            collection[2] = "Eleven";

            collection.Filtered.Count
                .Should()
                .Be(1);

            collection.Filtered[0]
                .Should()
                .Be("Two");
        }

        [Test]
        public void Selected_SelectUnknownValue_ExpectDefault()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Selected = "Five";

            collection.Selected
                .Should()
                .Be(default(string));
        }

        [Test]
        public void Selected_SelectKnownValue_ExpectToBeSelected()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.Selected = "Three";

            collection.Selected
                .Should()
                .Be("Three");
        }

        //[Test]
        //public void Selected_SelectSpecificInstance_ExpectToBeSelected()
        //{
        //    var collection = new ObservableViewCollection<string>(
        //            new[] { "One", "Two", "Three", "Four", "Two" }
        //        );

        //    collection.Selected = collection[4];

        //    collection.Selected
        //        .Should()
        //        .Be(collection[4]);
        //}
    }
}
