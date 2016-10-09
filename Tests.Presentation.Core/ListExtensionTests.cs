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
    public class ListExtensionTests
    {
        [Test]
        public void AddRange_AddItems_ExpectToSeeTheItems()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 0 };
            list.AddRange(tmp);

            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void AddRange_AddEmptyList_ExpectToSeeTheItems()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new int[] { };
            list.AddRange(tmp);

            Assert.AreEqual(5, list.Count);
        }

        [Test]
        public void AddRange_WithFilter_Include()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 1, 0, 1, 0, 1 };
            list.AddRange(tmp, ListFilter.Include, a => a > 0);

            Assert.AreEqual(8, list.Count);
        }

        [Test]
        public void AddRange_WithFilter_Exclude()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 1, 0, 1, 0, 1 };
            list.AddRange(tmp, ListFilter.Exclude, a => a > 0);

            Assert.AreEqual(7, list.Count);
        }

        [Test]
        public void AddRange_WithoutFilter_ShouldActLikeStandardAddRange()
        {
            var list = new List<int> { 1, 4, 5, 2, 3 };

            var tmp = new[] { 1, 0, 1, 0, 1 };
            list.AddRange(tmp, ListFilter.Exclude, null);

            Assert.AreEqual(10, list.Count);
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

            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual(4, list[3].Id);
            Assert.AreEqual(5, list[4].Id);
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

            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual(4, list[3].Id);
            Assert.AreEqual(5, list[4].Id);
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

            Assert.AreEqual(2, tmp.FindIndex(a => a.Name == "John"));
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

            Assert.AreEqual(-1, tmp.FindIndex(a => a.Name == "Paul"));
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

            Assert.AreEqual("John", tmp.Find(a => a.Name == "John").Name);
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

            Assert.Null(tmp.Find(a => a.Name == "Paul"));
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

            Assert.AreEqual("Bill", tmp[0].Name);
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

            Assert.AreEqual("Bill", tmp[tmp.Count - 1].Name);
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

            Assert.AreEqual("Bill", tmp[3].Name);
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

            Assert.AreEqual(3, tmp.BinarySearch("Jeff", (item, listItem) => item.CompareTo(listItem.Name)));
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

            Assert.AreEqual(-1, tmp.BinarySearch("Blah", (item, listItem) => item.CompareTo(listItem.Name)));
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

            Assert.AreEqual(3, tmp.BinarySearch(0, list.Count - 1, "Jeff", (item, listItem) => item.CompareTo(listItem.Name)));
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

            Assert.AreEqual(-1, tmp.BinarySearch(0, list.Count - 1, "Blah", (item, listItem) => item.CompareTo(listItem.Name)));
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

            Assert.AreEqual(-1, tmp.BinarySearch(new Person(666, "B"), new Comparison<Person>((p1, p2) => p1.Id - p2.Id)));
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

            Assert.AreEqual(3, tmp.BinarySearch(list[3], new Comparison<Person>((p1, p2) => p1.Id - p2.Id)));
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

            Assert.Throws<ArgumentOutOfRangeException>(() => tmp.BinarySearch(3, 1, "Jeff", (item, listItem) => item.CompareTo(listItem.Name)));
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

            Assert.Throws<ArgumentOutOfRangeException>(() => tmp.BinarySearch(0, 10, "Jeff", (item, listItem) => item.CompareTo(listItem.Name)));
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

            Assert.AreEqual(5, list.Count);
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

            Assert.AreEqual(3, list.Count);
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

            Assert.AreEqual(0, numbers.BinarySearchInsertionPoint(0, (a, b) => a - b));
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

            Assert.AreEqual(numbers.Count, numbers.BinarySearchInsertionPoint(999, (a, b) => a - b));
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

            Assert.AreEqual(2, numbers.BinarySearchInsertionPoint(6, (a, b) => a - b));
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

            Assert.AreEqual(3, numbers.BinarySearchInsertionPoint(1, (a, b) => a - b));
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

            Assert.AreEqual(numbers.Count, numbers.BinarySearchInsertionPoint(1, (a, b) => a - b));
        }
    }

}
