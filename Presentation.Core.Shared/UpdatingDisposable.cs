using System;
using PutridParrot.Presentation.Core.Interfaces;

namespace PutridParrot.Presentation.Core
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
            _supportsUpdating?.BeginUpdate();
        }

        /// <summary>
        /// Calls EndUpdate on a supplied ISupportUpdate implementation
        /// </summary>
        public void Dispose()
        {
            _supportsUpdating?.EndUpdate();
        }
    }

}
