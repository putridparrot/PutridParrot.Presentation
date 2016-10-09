using System;
using System.Linq.Expressions;

namespace Presentation.Core
{
    /// <summary>
    /// Extension methods for working with data error info. and the view model.
    /// </summary>
    public static class DataErrorInfoExtensions
    {
        /// <summary>
        /// Adds an error to the data error info. and raises the relevent property changed
        /// event for binding to be informed of the error.
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="TRet">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="dataErrorInfo">An extended data error info provider</param>
        /// <param name="propertyExpression">A property expression of the property</param>
        /// <param name="errorMessage">An error message to be stored against a property</param>
        public static void AddErrorAndRaisePropertyChanged<TObj, TRet>(this TObj o, IExtendedDataErrorInfo dataErrorInfo, Expression<Func<TObj, TRet>> propertyExpression, string errorMessage) where
            TObj : INotifyViewModel
        {
#if !NET4
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));
#else
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");
#endif

            var property = propertyExpression.Body as MemberExpression;
            if (property == null)
                throw new Exception("propertyExpression cannot be null and should be passed in the format x => x.PropertyName");

            string propertyName = property.Member.Name;

            dataErrorInfo.Add(propertyName, errorMessage);
            o.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Removes an error from the data error info. and raises the relvent
        /// property changed event
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="TRet">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="dataErrorInfo">An extended data error info provider</param>
        /// <param name="propertyExpression">A property expression of the property</param>
        public static void RemoveErrorAndRaisePropertyChanged<TObj, TRet>(this TObj o, IExtendedDataErrorInfo dataErrorInfo, Expression<Func<TObj, TRet>> propertyExpression) where
            TObj : INotifyViewModel
        {
#if !NET4
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));
#else
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");
#endif

            var property = propertyExpression.Body as MemberExpression;
            if (property == null)
                throw new Exception("propertyExpression cannot be null and should be passed in the format x => x.PropertyName");

            string propertyName = property.Member.Name;

            if (dataErrorInfo.Remove(propertyName))
            {
                o.RaisePropertyChanged(property.Member.Name);
            }
        }

        /// <summary>
        /// Adds an error to the data error info. and raises the relevent property changed
        /// event for binding to be informed of the error.
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="TRet">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="propertyExpression">A property expression of the property</param>
        /// <param name="errorMessage">An error message to be stored against a property</param>
        public static void AddErrorAndRaisePropertyChanged<TObj, TRet>(this TObj o, Expression<Func<TObj, TRet>> propertyExpression, string errorMessage) where
            TObj : IViewModel
        {
            o.AddErrorAndRaisePropertyChanged(o.DataErrorInfo, propertyExpression, errorMessage);
        }

        /// <summary>
        /// Removes an error from the data error info. and raises the relvent
        /// property changed event
        /// </summary>
        /// <typeparam name="TObj">The view model the property is on</typeparam>
        /// <typeparam name="TRet">The type of the property</typeparam>
        /// <param name="o">The view model</param>
        /// <param name="propertyExpression">A property expression of the property</param>
        public static void RemoveErrorAndRaisePropertyChanged<TObj, TRet>(this TObj o, Expression<Func<TObj, TRet>> propertyExpression) where
            TObj : IViewModel
        {
            o.RemoveErrorAndRaisePropertyChanged(o.DataErrorInfo, propertyExpression);
        }
    }
}
