using System;

namespace Presentation.Core
{
    /// <summary>
    /// Rule which allows a function to be associated with a 
    /// property change. 
    /// </summary>
    public class PropertyChangeRule<TV> : Rule
            where TV : IViewModel
    {
        private readonly Func<TV, bool> _func;

        public PropertyChangeRule(Func<TV, bool> func)
        {
            _func = func;
        }

        public override bool PreInvoke<T>(T viewModel, string propertyName)
        {
            return true;
        }

        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            var vm = viewModel as IViewModel;
            return vm == null || _func((TV)vm);
        }
    }
}