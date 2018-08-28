using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Presentation.Core.Interfaces;

namespace Presentation.Core.Helpers
{
    /// <summary>
    /// INotifyViewModel extension methods
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Raises a property changes with a null against the view model
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="o"></param>
        public static void RaisePropertyChanged<TObj>(this TObj o) where
            TObj : INotifyViewModel
        {
            o.RaisePropertyChanged((string)null);
        }

        /// <summary>
        /// Raises multiple properties change events
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="o"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="morePropertyExpression"></param>
        public static void RaiseMultiplePropertyChanged<TObj>(this TObj o,
            Expression<Func<TObj, object>> propertyExpression,
            params Expression<Func<TObj, object>>[] morePropertyExpression)
            where TObj : INotifyViewModel
        {
            RaisePropertyChanged(o, propertyExpression);
            foreach (var p in morePropertyExpression)
            {
                RaisePropertyChanged(o, p);
            }
        }

        /// <summary>
        /// Raises a property change for the given property name
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="o"></param>
        /// <param name="propertyName"></param>
        public static void RaisePropertyChanged<TObj, TRet>(this TObj o, [CallerMemberName] string propertyName = null) where
            TObj : INotifyViewModel
        {
            o.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises a property change for a given property where the property
        /// is supplied via an expression
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="o"></param>
        /// <param name="propertyExpression"></param>
        public static void RaisePropertyChanged<TObj, TRet>(this TObj o, Expression<Func<TObj, TRet>> propertyExpression)
            where TObj : INotifyViewModel
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            MemberExpression property = null;

            // it's possible (with the RaiseMultiple method) to end up with conversions
            // as the expressions are trying to convert to the same underlying type
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

            o.RaisePropertyChanged(property.Member.Name);
        }

        /// <summary>
        /// Sets a backing field if it's different from the supplied
        /// value, then raises a property change event
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="o"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="validation"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool SetProperty<TObj, TRet>(this TObj o, ref TRet backingField, TRet newValue, Action validation = null, [CallerMemberName] string propertyName = null) where
            TObj : INotifyViewModel
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyExpression");

            return o.SetProperty(() => propertyName, ref backingField, newValue, validation);
        }

        /// <summary>
        /// Sets a backing field if it's different from the supplied
        /// value, then raises a property change event
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="o"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="validation"></param>
        /// <returns></returns>
        public static bool SetProperty<TObj, TRet>(this TObj o,
            Expression<Func<TObj, TRet>> propertyExpression,
            ref TRet backingField, TRet newValue, Action validation = null) where
                TObj : INotifyViewModel
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            return o.SetProperty(() => o.NameOf(propertyExpression), ref backingField, newValue, validation);
        }

        /// <summary>
        /// Sets a backing field if it's different from the supplied
        /// value, then raises a property change event
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="o"></param>
        /// <param name="getPropertyName"></param>
        /// <param name="backingField"></param>
        /// <param name="newValue"></param>
        /// <param name="validation"></param>
        /// <returns></returns>
        private static bool SetProperty<TObj, TRet>(this TObj o, Func<string> getPropertyName,
            ref TRet backingField, TRet newValue, Action validation = null) where
                TObj : INotifyViewModel
        {
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return false;
            }

            var propertyName = getPropertyName();

            if (!o.RaisePropertyChanging(propertyName))
            {
                return false;
            }

            backingField = newValue;
            validation?.Invoke();
            o.RaisePropertyChanged(propertyName);

            return true;
        }
    }
}
