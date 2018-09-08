using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PutridParrot.Presentation.Core.Helpers
{
    public static class ListExtensions
    {
        /// <summary>
        /// Very simplistic implementation of AddRange for those IList implementations which 
        /// don't support it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list != null)
            {
                foreach (T item in items)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// A simple AddRange which allows the user to include or exclude items, for example adding all items
        /// except those with an notional > 1000000
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        /// <param name="filter"></param>
        /// <param name="function"></param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items, ListFilter filter, Predicate<T> function)
        {
            if (list != null)
            {
                if (function == null)
                {
                    list.AddRange(items);
                }
                else
                {
                    foreach (T item in items)
                    {
                        bool result = function(item);
                        if (filter == ListFilter.Include && result || filter == ListFilter.Exclude && !result)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sort for all list implementations, using a quick sort along with a supplied comparison function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="comparison"></param>
        public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list != null && list.Count > 0)
            {
                Sort(list, 0, list.Count - 1, comparison);
            }
        }

        public static void Sort<T>(this IList<T> list, IComparer comparer)
        {
            if (list != null && list.Count > 0 && comparer != null)
            {
                Sort(list, 0, list.Count - 1, (a, b) => comparer.Compare(a, b));
            }
        }

        public delegate int CompareValues<TSearchItem, TListItem>(TSearchItem searchItem, TListItem listItem);

        public static int BinarySearch<TSearchItem, TListItem>(this IList<TListItem> list, TSearchItem searchItem, CompareValues<TSearchItem, TListItem> matcher)
        {
            return BinarySearch(list, 0, list.Count - 1, searchItem, matcher);
        }

        public static int BinarySearch<TSearchItem, TListItem>(this IList<TListItem> list, int lowerBound, int upperBound, TSearchItem searchItem, CompareValues<TSearchItem, TListItem> matcher)
        {
            if (lowerBound > upperBound)
                throw new ArgumentOutOfRangeException("lowerBound must be less than or equal to upperBound");
            if (upperBound >= list.Count)
                throw new ArgumentOutOfRangeException("upperBound must be less than the size of the collection");

            int start = lowerBound;
            int end = upperBound;
            while (start <= end)
            {
                int mid = start + (end - start) / 2;

                int match = matcher(searchItem, list[mid]);
                if (match == 0)
                    return mid;

                if (match < 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }
            return -1;
        }

        public static int BinarySearchInsertionPoint<TSearchItem, TListItem>(this IList<TListItem> list, TSearchItem searchItem, CompareValues<TSearchItem, TListItem> matcher)
        {
            return BinarySearchInsertionPoint(list, 0, list.Count - 1, searchItem, matcher);
        }

        public static int BinarySearchInsertionPoint<TSearchItem, TListItem>(this IList<TListItem> list, int lowerBound,
            int upperBound, TSearchItem searchItem, CompareValues<TSearchItem, TListItem> matcher)
        {
            if (lowerBound > upperBound)
                throw new ArgumentOutOfRangeException("lowerBound must be less than or equal to upperBound");
            if (upperBound >= list.Count)
                throw new ArgumentOutOfRangeException("upperBound must be less than the size of the collection");

            var highIndex = upperBound;
            var lowIndex = lowerBound;

            while (lowIndex <= highIndex)
            {
                var mid = lowIndex + (highIndex - lowIndex) / 2;
                var test = matcher(searchItem, list[mid]);
                if (test > 0)
                    lowIndex = mid + 1;
                else if (test < 0)
                    highIndex = mid - 1;
                else
                {
                    // found a match, now find first value greater than match
                    for (int i = mid; i < highIndex; i++)
                    {
                        if (matcher(searchItem, list[i]) < 0)
                            return i;
                    }
                    // if there's no value larger than return upperbound
                    return upperBound + 1;
                }
            }

            return highIndex < 0 ? 0 : lowIndex;
        }

        public static int BinarySearch<T>(this IList<T> list, T item, Comparison<T> matcher)
        {
            return BinarySearch(list, 0, list.Count - 1, item, matcher);
        }

        public static int BinarySearch<T>(this IList<T> list, int lowerBound, int upperBound, T item, Comparison<T> matcher)
        {
            if (lowerBound > upperBound)
                throw new ArgumentOutOfRangeException("lowerBound must be less than or equal to upperBound");
            if (upperBound >= list.Count)
                throw new ArgumentOutOfRangeException("upperBound must be less than the size of the collection");

            int start = lowerBound;
            int end = upperBound;
            while (start <= end)
            {
                int mid = start + (end - start) / 2;

                int match = matcher(item, list[mid]);
                if (match == 0)
                    return mid;

                if (match < 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Basically an IndexOf for lists but with the ability to supply a matcher method,
        /// thus enabling us to find an index using part of an object that's not a key/reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this IList<T> list, Predicate<T> matcher)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (matcher(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void RemoveDuplicates<T>(this IList<T> list, Func<T, T, int> comparer)
        {
            IList<T> newList = list.Distinct(new ComparerImpl<T>(comparer)).ToList();
            list.Clear();
            list.AddRange(newList);
        }

        /// <summary>
        /// Basically a find method that enables us to use a matcher to check for a valid item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public static T Find<T>(this IList<T> list, Predicate<T> matcher)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (matcher(list[i]))
                {
                    return list[i];
                }
            }
            return default(T);
        }

        /// <summary>
        /// Adds an iten to a sorted list. It is expected the list is either sorted 
        /// or as items are added they're put in order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="comparison"></param>
        public static void Add<T>(this IList<T> list, T item, Comparison<T> comparison)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (comparison(item, list[i]) < 0)
                {
                    list.Insert(i, item);
                    return;
                }
            }

            list.Add(item);
        }

        // standard quick sort implementation
        private static void Sort<T>(IList<T> list, int left, int right, Comparison<T> comparison)
        {
            int i = left;
            int j = right;
            T x = list[(left + right) / 2];
            while (i <= j)
            {
                while (comparison(list[i], x) < 0)
                {
                    i++;
                }
                while (comparison(x, list[j]) < 0)
                {
                    j--;
                }
                if (i <= j)
                {
                    T tmp = list[i];
                    list[i++] = list[j];
                    list[j--] = tmp;
                }
            }
            if (left < j)
            {
                Sort(list, left, j, comparison);
            }
            if (i < right)
            {
                Sort(list, i, right, comparison);
            }
        }
    }

}
