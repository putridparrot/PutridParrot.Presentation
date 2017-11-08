#if !NETSTANDARD2_0
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace Presentation.Patterns.Helpers
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
        /// <param name="dispatcher">The dispatcher to apply this extension to</param>
        /// <param name="ex">The exception to be thrown on the dispatcher thread</param>
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
#endif