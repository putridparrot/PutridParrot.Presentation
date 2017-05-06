using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tests.Presentation.Helpers
{
    [ExcludeFromCodeCoverage]
    public class DivisibleBy100Comparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            // only change if value divisible by 100
            return y % 100 != 0;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }
    }
}