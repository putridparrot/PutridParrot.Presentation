using System;
using System.Diagnostics;

namespace Presentation.Core.Helpers
{
    public static class SafeConvert
    {
        public static T ChangeType<T>(object value, T defaultValue = default(T))
        {
            try
            {
                var ttype = typeof(T);
                if (!(value is IConvertible))
                {
                    if (value == null)
                    {
                        // if the type is not nullable, but the value is, then switch to default
                        // saves having an exception
                        var canBeNull = !ttype.IsValueType || (Nullable.GetUnderlyingType(ttype) != null);
                        if (!canBeNull)
                        {
                            return defaultValue;
                        }
                    }
                    // doesn't support convertible so let's
                    // try to simply cast
                    return (T)value;
                }

                var type = Nullable.GetUnderlyingType(ttype);
                return (T)Convert.ChangeType(value, type ?? ttype);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return defaultValue;
            }
        }
    }
}
