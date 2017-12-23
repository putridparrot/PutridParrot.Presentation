using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Presentation.Core.Interfaces;

namespace Presentation.Core
{
    /// <summary>
    /// Implementation of an IExtendedDataErrorInfo for use with the IDataErrorInfo
    /// implementations.
    /// </summary>
    public class DataErrorInfo : IExtendedDataErrorInfo
    {
        private readonly object _syncObject = new object();
        private Dictionary<string, string> _errors;
        private string _error;

#if !NET4
        /// <summary>
        /// Event is raised when errors have been added/removed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
#endif

        /// <summary>
        /// Gets an enumerable with items associated with
        /// the property name or null of none exist
        /// </summary>
        /// <param name="propertyName">The property name to get associated errors for</param>
        /// <returns>An enumerable of errors or null if none exist</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            var error = this[propertyName];
            if (!String.IsNullOrEmpty(error))
                return new[] { error };

            return null;
        }

#if !NET4
        /// <summary>
        /// Gets whether any errors exist
        /// </summary>
        public bool HasErrors
        {
            get
            {
                if (_errors != null)
                {
                    lock (_syncObject)
                    {
                        return _errors?.Count > 0;
                    }
                }
                return false;
            }
        }
#else
        /// <summary>
        /// Gets whether any errors exist
        /// </summary>
        public bool HasErrors
        {
            get { return _errors != null && _errors.Count > 0; }
        }
#endif

        /// <summary>
        /// Gets or creates a dictionary of errors
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetOrCreateErrors()
        {
            return _errors ?? (_errors = new Dictionary<string, string>());
        }

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
                    if (_errors != null)
                    {
                        var sb = new StringBuilder();
                        lock (_syncObject)
                        {
                            if (_errors != null)
                            {
                                foreach (var e in _errors.Values)
                                {
                                    sb.AppendLine(e);
                                }
                            }
                        }
                        return sb.ToString();
                    }
                }
                return _error;
            }
        }

        /// <summary>
        /// Set the error message for the object as a whole
        /// </summary>
        /// <param name="errorMessage"></param>
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
                if (_errors == null)
                    return null;

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
                if (_errors == null)
                    return null;

                lock (_syncObject)
                {
                    return (from propertyErrorPair in _errors
                            where !String.IsNullOrEmpty(propertyErrorPair.Value)
                            select propertyErrorPair.Value).ToArray();
                }
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
#if !NET4
            var errorsChanged = ErrorsChanged;
            errorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
#endif
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
                    string value = null;
#if !NET4
                    _errors?.TryGetValue(propertyName, out value);
#else
                    if (_errors != null)
                    {
                        _errors.TryGetValue(propertyName, out value);
                    }
#endif
                    return value;
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
                var errors = GetOrCreateErrors();
                errors[propertyName] = message;
            }
            OnErrorsChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Remove a property name from the collection of errors
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool Remove(string propertyName)
        {
            if (_errors != null)
            {
                lock (_syncObject)
                {
                    if (_errors != null)
                    {
                        return _errors.Remove(propertyName);
                    }
                }
            }
            OnErrorsChanged(propertyName);
            return false;
        }

        /// <summary>
        /// Clear all properties/errors from the collection
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            if (_errors != null)
            {
                string[] properties;
                lock (_syncObject)
                {
                    properties = Properties;
#if !NET4
                    _errors?.Clear();
#else
                    if (_errors != null)
                    {
                        _errors.Clear();
                    }
#endif
                }
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        OnErrorsChanged(property);
                    }
                }
            }
            return true;
        }
    }
}
