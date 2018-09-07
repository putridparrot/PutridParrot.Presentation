using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Presentation.Core.Helpers;

namespace Tests.Presentation
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ListExtensionTests
    {
        [Test]
        public void AddRange_AddItems_ExpectToSeeTheItems()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 0 };
            list.AddRange(tmp);

            list.Count
                .Should()
                .Be(6);
        }

        [Test]
        public void AddRange_AddEmptyList_ExpectToSeeTheItems()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new int[] { };
            list.AddRange(tmp);

            list.Count
                .Should()
                .Be(5);
        }

        [Test]
        public void AddRange_WithFilter_Include()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 1, 0, 1, 0, 1 };
            list.AddRange(tmp, ListFilter.Include, a => a > 0);

            list.Count
                .Should()
                .Be(8);
        }

        [Test]
        public void AddRange_WithFilter_Exclude()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 1, 0, 1, 0, 1 };
            list.AddRange(tmp, ListFilter.Exclude, a => a > 0);

            list.Count
                .Should()
                .Be(7);
        }

        [Test]
        public void AddRange_WithoutFilter_ShouldActLikeStandardAddRange()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 1, 0, 1, 0, 1 };
            list.AddRange(tmp, ListFilter.Exclude, null);

            list.Count
                .Should()
                .Be(10);
        }

        class Person
        {
            public Person(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public int Id { get; private set; }
            public string Name { get; private set; }
        }

        [Test]
        public void Sort_ShouldPlaceItemsInCorrectOrder()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oilver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.Sort((a, b) => a.Id - b.Id);

            list[0].Id
                .Should()
                .Be(1);
            list[1].Id
                .Should()
                .Be(2);
            list[2].Id
                .Should()
                .Be(3);
            list[3].Id
                .Should()
                .Be(4);
            list[4].Id
                .Should()
                .Be(5);
        }

        [Test]
        public void Sort_UsingIComparer_ShouldPlaceItemsInCorrectOrder()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oilver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.Sort(new ComparerImpl<Person>((a, b) => a.Id - b.Id));

            list[0].Id
                .Should()
                .Be(1);
            list[1].Id
                .Should()
                .Be(2);
            list[2].Id
                .Should()
                .Be(3);
            list[3].Id
                .Should()
                .Be(4);
            list[4].Id
                .Should()
                .Be(5);
        }

        [Test]
        public void Sort_WithEmptyList_ShouldNotException()
        {
            var list = new List<Person>();

            // convert to an IList to use the extensions
            list.Sort((a, b) => a.Id - b.Id);

            // should make it to here okay
            Assert.True(true);
        }

        [Test]
        public void FindIndex_ItemExists()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oilver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.FindIndex(a => a.Name == "John")
                .Should()
                .Be(2);
        }

        [Test]
        public void FindIndex_ItemDoesNotExist()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oilver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.FindIndex(a => a.Name == "Paul")
                .Should()
                .Be(-1);
        }

        [Test]
        public void Find_ItemExists()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oilver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.Find(a => a.Name == "John").Name
                .Should()
                .Be("John");
        }

        [Test]
        public void Find_ItemDoesNotExist()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oilver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.Find(a => a.Name == "Paul")
                .Should()
                .BeNull();
        }

        [Test]
        public void Add_WithComparison_ToStart()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oilver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.Sort((a, b) => a.Id - b.Id);

            tmp.Add(new Person(0, "Bill"), (a, b) => a.Id - b.Id);

            tmp[0].Name
                .Should()
                .Be("Bill");
        }

        [Test]
        public void Add_WithComparison_ToEnd()
        {
            var list = new List<Person>
            {
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "John"),
                new Person(5, "Oliver"),
                new Person(2, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.Sort((a, b) => a.Id - b.Id);

            tmp.Add(new Person(10, "Bill"), (a, b) => a.Id - b.Id);

            tmp[tmp.Count - 1].Name
                .Should()
                .Be("Bill");
        }

        [Test]
        public void Add_WithComparison_ToMiddle()
        {
            var list = new List<Person>
            {
                new Person(2, "Bob"),
                new Person(4, "Jeff"),
                new Person(0, "John"),
                new Person(5, "Oliver"),
                new Person(1, "Alan")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.Sort((a, b) => a.Id - b.Id);

            tmp.Add(new Person(3, "Bill"), (a, b) => a.Id - b.Id);

            tmp[3].Name
                .Should()
                .Be("Bill");
        }

        [Test]
        public void BinarySearch_ItemExists()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.BinarySearch("Jeff", (item, listItem) => item.CompareTo(listItem.Name))
                .Should()
                .Be(3);
        }

        [Test]
        public void BinarySearch_ItemDoesNotExist()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.BinarySearch("Blah", (item, listItem) => item.CompareTo(listItem.Name))
                .Should()
                .Be(-1);
        }

        [Test]
        public void BinarySearch_WithBounds_ItemExists()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.BinarySearch(0, list.Count - 1, "Jeff", (item, listItem) => item.CompareTo(listItem.Name))
                .Should()
                .Be(3);
        }

        [Test]
        public void BinarySearch_WithBounds_ItemDoesNotExist()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.BinarySearch(0, list.Count - 1, "Blah", (item, listItem) => item.CompareTo(listItem.Name))
                .Should()
                .Be(-1);
        }

        [Test]
        public void BinarySearch_WithComparison_ItemDoesNotExist()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.BinarySearch(new Person(666, "B"), new Comparison<Person>((p1, p2) => p1.Id - p2.Id))
                .Should()
                .Be(-1);
        }

        [Test]
        public void BinarySearch_WithComparison_ItemExists()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.BinarySearch(list[3], new Comparison<Person>((p1, p2) => p1.Id - p2.Id))
                .Should()
                .Be(3);
        }

        [Test]
        public void BinarySearch_WithBounds_LowerGreatThanUpper_ExpectException()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            Action action = () => tmp.BinarySearch(3, 1, "Jeff", (item, listItem) => item.CompareTo(listItem.Name));
            action
                .Should()
                .Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void BinarySearch_WithBounds_UpperGreaterThanListLength_ExpectException()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            Action action = () => tmp.BinarySearch(0, 10, "Jeff", (item, listItem) => item.CompareTo(listItem.Name));
            action
                .Should()
                .Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void RemoveDuplicates_NoDuplicatesSoNoChangeExpected()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(3, "Bob"),
                new Person(4, "Jeff"),
                new Person(5, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.RemoveDuplicates((p1, p2) => p1.Id - p2.Id);

            list.Count
                .Should()
                .Be(5);
        }

        [Test]
        public void RemoveDuplicates_DuplicatesExistSoExpectOnlyOneOfThemToRemain()
        {
            var list = new List<Person>
            {
                new Person(1, "John"),
                new Person(2, "Alan"),
                new Person(1, "Bob"),
                new Person(4, "Jeff"),
                new Person(1, "Oilver")
            };

            // convert to an IList to use the extensions
            IList<Person> tmp = list;

            tmp.RemoveDuplicates((p1, p2) => p1.Id - p2.Id);

            list.Count
                .Should()
                .Be(3);
        }

        [Test]
        public void BinarySearchInsertionPoint_ItemShouldBeFirst()
        {
            var numbers = new List<int>
            {
                1,
                4,
                5,
                78,
                100
            };

            numbers.BinarySearchInsertionPoint(0, (a, b) => a - b)
                .Should()
                .Be(0);
        }

        [Test]
        public void BinarySearchInsertionPoint_ItemShouldBeLast()
        {
            var numbers = new List<int>
            {
                1,
                4,
                5,
                78,
                100
            };

            numbers.BinarySearchInsertionPoint(999, (a, b) => a - b)
                .Should()
                .Be(numbers.Count);
        }

        [Test]
        public void BinarySearchInsertionPoint_ItemShouldBeMiddle()
        {
            var numbers = new List<int>
            {
                1,
                4,
                78,
                100
            };

            numbers.BinarySearchInsertionPoint(6, (a, b) => a - b)
                .Should()
                .Be(2);
        }

        [Test]
        public void BinarySearchInsertionPoint_MatchingFirstItems_EXpectEndOfMatches()
        {
            var numbers = new List<int>
            {
                1,
                1,
                1,
                78,
                100
            };

            numbers.BinarySearchInsertionPoint(1, (a, b) => a - b)
                .Should()
                .Be(3);
        }

        [Test]
        public void BinarySearchInsertionPoint_AllItemsMatch_ExpectEndOfMatches()
        {
            var numbers = new List<int>
            {
                1,
                1,
                1,
                1,
                1
            };

            numbers.BinarySearchInsertionPoint(1, (a, b) => a - b)
                .Should()
                .Be(numbers.Count);
        }
    }

}
