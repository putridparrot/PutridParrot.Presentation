using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presentation.Core
{
    /// <summary>
    /// Implementation of an IExtendedDataErrorInfo for use with the IDataErrorInfo
    /// implementations.
    /// </summary>
    public class DataErrorInfo : IExtendedDataErrorInfo
    {
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();
        private readonly object _syncObject = new object();
        private string _error;

        /// <summary>
        /// Gets/Sets the error string - if none exists and errors exist
        /// then it's created based upon those errors concatenated.
        /// </summary>
        public string Error
        {
            get
            {
                if (String.IsNullOrEmpty(_error))
                {
                    var sb = new StringBuilder();
                    lock (_syncObject)
                    {
                        foreach (var e in _errors.Values)
                        {
                            sb.AppendLine(e);
                        }
                    }
                    return sb.ToString();
                }
                return _error;
            }
        }

        public void SetError(string errorMessage)
        {
            _error = errorMessage;
        }

        /// <summary>
        /// Gets all the properties which errors exist for
        /// </summary>
        public string[] Properties
        {
            get
            {
                lock (_syncObject)
                {
                    return (from propertyErrorPair in _errors
                            where !String.IsNullOrEmpty(propertyErrorPair.Value)
                            select propertyErrorPair.Key).ToArray();
                }
            }
        }

        /// <summary>
        /// Gets all the error strings
        /// </summary>
        public string[] Errors
        {
            get
            {
                lock (_syncObject)
                {
                    return (from propertyErrorPair in _errors
                            where !String.IsNullOrEmpty(propertyErrorPair.Value)
                            select propertyErrorPair.Value).ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the error string for a given property name/key or null
        /// if no values are stored
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get
            {
                lock (_syncObject)
                {
                    return !_errors.ContainsKey(propertyName) ? null : _errors[propertyName];
                }
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    Remove(propertyName);
                }
                else
                {
                    Add(propertyName, value);
                }
            }
        }

        /// <summary>
        /// Add an error message against a given property name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Add(string propertyName, string message)
        {
            lock (_syncObject)
            {
                _errors[propertyName] = message;
            }
            return true;
        }

        /// <summary>
        /// Remove a property name from the collection of errors
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool Remove(string propertyName)
        {
            lock (_syncObject)
            {
                if (_errors.ContainsKey(propertyName))
                {
                    _errors.Remove(propertyName);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clear all properties/errors from the collection
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            lock (_syncObject)
            {
                _errors.Clear();
            }
            return true;
        }
    }
}