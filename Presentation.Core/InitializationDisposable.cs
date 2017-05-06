using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Presentation.Patterns
{
    /// <summary>
    /// Disposable initialzation helper class to 
    /// ensure that EndInit is called for the 
    /// corresponding  BeginInit (which is called 
    /// on creation)
    /// </summary>
    public class InitializationDisposable : IDisposable
    {
        private readonly ISupportInitialize _supportsInitialize;

        /// <summary>
        /// Creates an InitilizationDisposable wrapper around an
        /// ISupportInitialize implementation, calling BeginInit
        /// </summary>
        /// <param name="supportsUpdating"></param>
        public InitializationDisposable(ISupportInitialize supportsUpdating)
        {
            _supportsInitialize = supportsUpdating;
#if !NET4
            _supportsInitialize?.BeginInit();
#else
            if (_supportsInitialize != null)
            {
                _supportsInitialize.BeginInit();
            }
#endif
        }
        /// <summary>
        /// Calls EndInit on a supplied ISupportInitialize implementation
        /// </summary>
        public void Dispose()
        {
#if !NET4
            _supportsInitialize?.EndInit();
#else
            if (_supportsInitialize != null)
            {
                _supportsInitialize.EndInit();
            }
#endif
        }
    }

}
