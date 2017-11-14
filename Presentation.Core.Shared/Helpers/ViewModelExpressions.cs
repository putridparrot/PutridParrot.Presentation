using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns.Helpers
{
    /// <summary>
    /// Utility class for working with expressions on INotifyViewModel objects
    /// </summary>
    public static class ViewModelExpressions
    {
        /// <summary>
        /// Converts a property Expression into a property string
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="propertyExpression">A property expression of the property</param>
        /// <returns>The string representing the property name</returns>
        [DebuggerStepThrough]
        public static string NameOf<TObj, T>(this TObj o, Expression<Func<TObj, T>> propertyExpression) where
            TObj : INotifyViewModel
        {
#if !NET4
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));
#else
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");
#endif

            MemberExpression property = null;

            // it's possible to end up with conversions, as the expressions are trying 
            // to convert to the same underlying type
            if (propertyExpression.Body.NodeType == ExpressionType.Convert)
            {
                if (propertyExpression.Body is UnaryExpression convert)
                {
                    property = convert.Operand as MemberExpression;
                }
            }

            if (property == null)
            {
                property = propertyExpression.Body as MemberExpression;
            }
            if (property == null)
                throw new Exception(
                    "propertyExpression cannot be null and should be passed in the format x => x.PropertyName");

            return property.Member.Name;
        }

        /// <summary>
        /// Converts multiple property Expressions into a property string array
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="propertyExpressions">One or more property expressions</param>
        /// <returns>The string array representing the property names</returns>
        [DebuggerStepThrough]
        public static string[] NameOf<TObj, T>(this TObj o, params Expression<Func<TObj, T>>[] propertyExpressions) where
            TObj : INotifyViewModel
        {
            return propertyExpressions.Select(p => NameOf(o, p)).ToArray();
        }
    }
}
