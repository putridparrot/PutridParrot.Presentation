using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace Presentation.Core
{
    /// <summary>
    /// Dispatcher extensions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Rethrows an exception onto the current dispatcher. Useful for
        /// putting exceptions onto the UI thread.
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="ex"></param>
        public static void Throw(this Dispatcher dispatcher, Exception ex)
        {
            dispatcher.BeginInvoke(new Action(() =>
            {
                if (!Debugger.IsAttached)
                    throw ex;
            }));
        }
    }
}
