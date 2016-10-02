using System.ComponentModel;

namespace Presentation.Core
{
    /// <summary>
    /// Extends the IDataErrorInfo with simple capabilities for 
    /// interaction with data error info.
    /// </summary>
    public interface IExtendedDataErrorInfo : IDataErrorInfo
    {
        /// <summary>
        /// Gets the properties that error info. has been created for
        /// </summary>
        string[] Properties { get; }
        /// <summary>
        /// Gets the error strings in an array
        /// </summary>
        string[] Errors { get; }

        /// <summary>
        /// Add an error string against a given property name
        /// </summary>
        /// <param name="propertyName">The property name as a string</param>
        /// <param name="message">The rror/message to be stored</param>
        /// <returns>True if successfull, otherwise false</returns>
        bool Add(string propertyName, string message);
        /// <summary>
        /// Removes the error string (and the property name) from the errors
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool Remove(string propertyName);
        /// <summary>
        /// Clear all errors from the error info.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}