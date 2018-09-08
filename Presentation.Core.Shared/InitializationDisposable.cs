using System;
using System.ComponentModel;

namespace PutridParrot.Presentation
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
            _supportsInitialize?.BeginInit();
        }
        /// <summary>
        /// Calls EndInit on a supplied ISupportInitialize implementation
        /// </summary>
        public void Dispose()
        {
            _supportsInitialize?.EndInit();
        }
    }
}
