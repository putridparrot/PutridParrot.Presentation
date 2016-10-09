using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Presentation.Core;

namespace Presentation.Core
{
    public static class ViewModelExtensions
    {
        public static void RaisePropertyChanged<TObj>(this TObj o) where
            TObj : INotifyViewModel
        {
            o.RaisePropertyChanged((string)null);
        }

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

#if !NET4
    public static void RaisePropertyChanged<TObj, TRet>(this TObj o, [CallerMemberName] string propertyName = null) where
    TObj : INotifyViewModel
        {
            o.RaisePropertyChanged(propertyName);
        }
#endif

        public static void RaisePropertyChanged<TObj, TRet>(this TObj o, Expression<Func<TObj, TRet>> propertyExpression)
            where TObj : INotifyViewModel
        {
            if (propertyExpression == null)
            {
#if !NET4
                throw new ArgumentNullException(nameof(propertyExpression));
#else
                throw new ArgumentNullException("propertyExpression");
#endif
            }

            MemberExpression property = null;

            // it's possible (with the RaiseMultiple method) to end up with conversions
            // as the expressions are trying to convert to the same underlying type
            if (propertyExpression.Body.NodeType == ExpressionType.Convert)
            {
                var convert = propertyExpression.Body as UnaryExpression;
                if (convert != null)
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

#if !NET4
    public static bool RaiseAndSetIfChanged<TObj, TRet>(this TObj o, ref TRet backingField, TRet newValue, Action validation = null, [CallerMemberName] string propertyName = null) where
        TObj : INotifyViewModel
        {
            if(String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyExpression");

            return o.RaiseAndSetIfChanged(() => propertyName, ref backingField, newValue, validation);           
        }
#endif

        public static bool RaiseAndSetIfChanged<TObj, TRet>(this TObj o,
            Expression<Func<TObj, TRet>> propertyExpression,
            ref TRet backingField, TRet newValue, Action validation = null) where
                TObj : INotifyViewModel
        {
            if (propertyExpression == null)
            {
#if !NET4
                throw new ArgumentNullException(nameof(propertyExpression));
#else
                throw new ArgumentNullException("propertyExpression");
#endif
            }

            return o.RaiseAndSetIfChanged(() => o.NameOf(propertyExpression), ref backingField, newValue, validation);
        }

        private static bool RaiseAndSetIfChanged<TObj, TRet>(this TObj o, Func<string> getPropertyName,
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

            if (validation != null)
            {
                validation();
            }

            o.RaisePropertyChanged(propertyName);

            return true;
        }
    }
}