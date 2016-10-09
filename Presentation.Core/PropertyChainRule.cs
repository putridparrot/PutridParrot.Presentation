namespace Presentation.Core
{
    /// <summary>
    /// A property chain rule detects a property change on a
    /// specified property and triggers other property change 
    /// events
    /// </summary>
    public class PropertyChainRule : Rule
    {
        private readonly string[] _chainedProperties;

        public PropertyChainRule(string[] dependents)
        {
            _chainedProperties = dependents;
        }

        public override bool PreInvoke<T>(T viewModel, string propertyName)
        {
            return true;
        }

        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            var vm = viewModel as IViewModel;
#if !NET4
            vm?.RaisePropertyChanged(_chainedProperties);
#else
            if (vm != null)
            {
                vm.RaiseMultiplePropertyChanged(_chainedProperties);
            }
#endif
            return true;
        }
    }
}