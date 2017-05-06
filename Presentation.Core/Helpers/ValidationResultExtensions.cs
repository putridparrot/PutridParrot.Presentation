using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Patterns.Helpers
{
    public static class Validation
    {
        /// <summary>
        /// Turns a boolean result into a ValidationResult or null
        /// </summary>
        /// <param name="result">If false, creates validation result with supplied message</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns></returns>
        public static ValidationResult Check(bool result, string errorMessage)
        {
            return !result ? new ValidationResult(errorMessage) : null;
        }
        /// <summary>
        /// For legacy code or alternate error reporting, this can
        /// be wrapped around a validation method which itself might 
        /// generates/set error info. Basically the result is ignored.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>Simple returns null</returns>
        public static ValidationResult Ignore(bool result)
        {
            return null;
        }
    }
}
