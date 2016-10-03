using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Presentation.Core
{
    /// <summary>
    /// Used for view model validation, should be implemented to allow
    /// validation of both single properties and all properties
    /// </summary>
    public interface IValidateViewModel
    {
        /// <summary>
        /// Validates a property against the supplied value
        /// </summary>
        /// <param name="propertyName">The property name as a string</param>
        /// <param name="newValue">The value to be validated</param>
        /// <returns>An array of validation results returned, or null if no results</returns>
        Task<ValidationResult[]> Validate(string propertyName, object newValue);
        /// <summary>
        /// Validaties all properties
        /// </summary>
        /// <returns>An array of validation results returned, or null if no results</returns>
        Task<ValidationResult[]> Validate();
    }
}