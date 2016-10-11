using System;

namespace Presentation.Core
{
    /// <summary>
    /// Rule which allows a function or Action to be associated with a 
    /// property change. 
    /// </summary>
    public class PropertyChangeRule<TV> : Rule
            where TV : IViewModel
    {
        private readonly Func<TV, bool> _func;
        private readonly Action<TV> _action;

        public PropertyChangeRule(Func<TV, bool> func)
        {
            _func = func;
        }

        public PropertyChangeRule(Action<TV> action)
        {
            _action = action;
        }

        public override bool PreInvoke<T>(T viewModel, string propertyName)
        {
            return true;
        }

        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            var vm = viewModel as IViewModel;
            if (vm != null)
            {
                if (_action != null)
                {
                    _action((TV) vm);
                    return true;
                }

                if (_func != null)
                {
                    return _func((TV) vm);
                }
            }

            return true;
        }
    }
}