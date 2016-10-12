using System.Collections.Generic;

namespace Presentation.Core
{
    /// <summary>
    /// The AndRules will only invoke after all property changes have been 
    /// received, as such it has to hold a copy of property changes until
    /// all properties have been raised before clearing the list - order
    /// or property changes doesn't matter
    /// </summary>
    public class AndRules : Rules
    {
        private readonly List<string> _propertyChanges = new List<string>();

        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            foreach (var kv in _rules)
            {
                if (kv.Key == propertyName)
                {
                    if (!_propertyChanges.Contains(propertyName))
                    {
                        _propertyChanges.Add(propertyName);
                        break;
                    }
                }
            }

            var result = _propertyChanges.Count == _rules.Count && base.PostInvoke(viewModel, propertyName);
            if (result)
            {
                // all property changes have fired so clear the list of changes
                // to start the process again
                _propertyChanges.Clear();
            }
            return result;
        }
    }
}