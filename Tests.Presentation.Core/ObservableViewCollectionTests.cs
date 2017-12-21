using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core;

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
            var collection = new ObservableViewCollection<string>(new[] {"One", "Two", "Three", "Four"})
            { Filter = s => s.StartsWith("T")};

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
            var collection = new ObservableViewCollection<string>(new[] {"One", "Two", "Three", "Four"}
            ) {Filter = s => s.StartsWith("T")};


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

            collection.SelectedItem = "Five";

            collection.SelectedItem
                .Should()
                .Be(default(string));
        }

        [Test]
        public void Selected_SelectKnownValue_ExpectToBeSelected()
        {
            var collection = new ObservableViewCollection<string>(
                    new[] { "One", "Two", "Three", "Four" }
                );

            collection.SelectedItem = "Three";

            collection.SelectedItem
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
        [Test]
        public void Selected_EnsureSelectCorrectlySet()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                4,
                7,
                21,
                100
            };

            collection.SelectedItem = 21;
            Assert.AreEqual(21, collection.SelectedItem);
        }

        [Test]
        public void Selected_SetToNonExistentValue_ShouldNotBeSet()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                4,
                7,
                21,
                100
            };

            collection.SelectedItem = 999;
            Assert.AreEqual(0, collection.SelectedItem);
        }

        [Test]
        public void Selected_SetToValidValue_ThenRemoveItem_ExpectSelectedChangesToDefault()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                4,
                7,
                21,
                100
            };

            collection.SelectedItem = 7;
            collection.Remove(7);
            Assert.AreEqual(0, collection.SelectedItem);
        }

        [Test]
        public void Selected_SetToValidValue_Clear_ExpectSelectedChangesToDefault()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                4,
                7,
                21,
                100
            };

            collection.SelectedItem = 7;
            collection.Clear();
            Assert.AreEqual(0, collection.SelectedItem);
        }

        [Test]
        public void DefaultValue_SetToValue_ExpectSameValueBack()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                4,
                7,
                21,
                100
            };

            collection.DefaultValue = 999;
            Assert.AreEqual(999, collection.DefaultValue);
        }

        [Test]
        public void DefaultValue_Selected_SetToValidValue_Clear_ExpectSelectedChangesToDefault()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                4,
                7,
                21,
                100
            };

            collection.DefaultValue = 999;
            collection.SelectedItem = 7;
            collection.Clear();
            Assert.AreEqual(999, collection.SelectedItem);
        }

        [Test]
        public void SetItem_SelectedItemChangesSo_ExpectDefaultValue()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                4,
                7,
                21,
                100
            };

            collection.DefaultValue = 999;
            collection.SelectedItem = 7;
            collection[3] = 9;
            Assert.AreEqual(999, collection.SelectedItem);
        }

        [Test]
        public void AddCommand_WhenCommandExecutedWithParam_ExpectListToGrow()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };

            collection.AddCommand.Execute(123);
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(123, collection[3]);
        }

        [Test]
        public void AddCommand_WhenCommandExecutedWithoutParam_ExpectListToGrowButItemAddedIsDefaultT()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };

            collection.AddCommand.Execute(null);
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(0, collection[3]);
        }


        [Test]
        public void AddCommand_WhenOverridenCommandExecuted_ExpectListToGrowByTwo()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };
            collection.AddCommand = new ActionCommand(() => collection.AddRange(new [] { 4, 578 }));
            collection.AddCommand.Execute(null);

            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual(578, collection[4]);
        }

        [Test]
        public void DeleteCommand_WhenNoItemSelected_ExpectCanExecuteCommandToBeFalse()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };

            Assert.IsFalse(collection.DeleteCommand.CanExecute(null));
        }

        [Test]
        public void DeleteCommand_WithSelectedItem_ExpectCanExecuteCommandToBeTrue()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };
            collection.SelectedIndex = 0;

            Assert.IsTrue(collection.DeleteCommand.CanExecute(null));
        }
        [Test]
        public void DeleteCommand_WhenOverridenCommandExecuted_ExpectListToReduceByTwo()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };
            collection.DeleteCommand = new ActionCommand(() =>
            {
                collection.RemoveAt(0);
                collection.RemoveAt(0);
            });
            collection.DeleteCommand.Execute(null);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(3, collection[0]);
        }

        [Test]
        public void ClearCommand_IfNoItems_ExpectCanExecuteToBeFalse()
        {
            var collection = new ObservableViewCollection<int>();

            Assert.IsFalse(collection.ClearCommand.CanExecute(null));
        }

        [Test]
        public void ClearCommand_IfHasItems_ExpectCanExecuteToBeTrue()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };

            Assert.IsTrue(collection.ClearCommand.CanExecute(null));
        }

        [Test]
        public void ClearCommand_Execute_ExpectNoItemsLeftInCollection()
        {
            var collection = new ObservableViewCollection<int>
            {
                1,
                2,
                3
            };

            collection.ClearCommand.Execute(null);
            Assert.AreEqual(0, collection.Count);
        }
    }
}
