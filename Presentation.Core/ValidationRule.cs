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
        private readonly string _erroMessage;

        public ValidationRule(Func<TV, bool> validationFunc, string errorMessage)
        {
            this._validationFunc = validationFunc;
            this._erroMessage = errorMessage;
        }

        public override bool PreInvoke<T>(T viewModel, string propertyName)
        {
            viewModel.DataErrorInfo.Remove(propertyName);
            return true;
        }

        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            var vm = viewModel as ViewModel;
            if (vm != null)
            {
                if (!_validationFunc((TV)vm))
                {
                    vm.DataErrorInfo.Add(propertyName, _erroMessage);
                    return false;
                }
            }
            return true;
        }
    }
}