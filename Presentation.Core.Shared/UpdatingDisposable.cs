using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns
{
    /// <summary>
    /// Disposable initialzation helper class to 
    /// ensure that EndUpdate is called for the 
    /// corresponding BeginUpdate (which is called 
    /// on creation)
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// using(new UpdatingDisposable(viewModel))
    /// {
    ///    // do something
    /// }
    /// ]]>
    /// </example>
    public class UpdatingDisposable : IDisposable
    {
        private readonly ISupportUpdate _supportsUpdating;

        /// <summary>
        /// Creates and UpdatingDisposable object wrapper around
        /// an ISupportUpdate object, calling BeginUpdate
        /// </summary>
        /// <param name="supportsUpdating"></param>
        public UpdatingDisposable(ISupportUpdate supportsUpdating)
        {
            _supportsUpdating = supportsUpdating;
#if !NET4
            _supportsUpdating?.BeginUpdate();
#else
            if (_supportsUpdating != null)
            {
                _supportsUpdating.BeginUpdate();
            }
#endif
        }

        /// <summary>
        /// Calls EndUpdate on a supplied ISupportUpdate implementation
        /// </summary>
        public void Dispose()
        {
#if !NET4
            _supportsUpdating?.EndUpdate();
#else
            if (_supportsUpdating != null)
            {
                _supportsUpdating.EndUpdate();
            }
#endif
        }
    }

}
