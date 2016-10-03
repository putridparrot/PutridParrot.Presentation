using System;

namespace Presentation.Core
{
    /// <summary>
    /// A validation rule implementation.
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    public class ValidationRule<TV> : Rule
        where TV : ViewModel
    {
        private readonly Func<TV, bool> _validationFunc;
        private readonly string _validationPropertyName;
        private readonly string _errorMessage;

        /// <summary>
        /// Constructs a validation rule
        /// </summary>
        /// <param name="validationFunc">The validation function</param>
        /// <param name="errorMessage">The error message if validation fails</param>
        /// <param name="validationPropertyName">The property to be used for validation, null indicates the calling property</param>
        public ValidationRule(Func<TV, bool> validationFunc, string errorMessage, string validationPropertyName = null)
        {
            _validationFunc = validationFunc;
            _errorMessage = errorMessage;
            _validationPropertyName = validationPropertyName;
        }

        private string GetPropertyName(string propertyName)
        {
            return String.IsNullOrEmpty(_validationPropertyName) ? propertyName : _validationPropertyName;
        }

        public override bool PreInvoke<T>(T viewModel, string propertyName)
        {
            viewModel.DataErrorInfo.Remove(GetPropertyName(propertyName));
            return true;
        }

        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            var vm = viewModel as ViewModel;
            if (vm != null)
            {
                if (!_validationFunc((TV)vm))
                {
                    var pn = GetPropertyName(propertyName);
                    vm.DataErrorInfo.Add(pn, _errorMessage);
                    ((IViewModel)vm).RaisePropertyChanged(pn);
                    return false;
                }
            }
            return true;
        }
    }
}