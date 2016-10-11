using System;
using System.ComponentModel;

namespace Presentation.Core
{
    public interface ISupportRevertibleChangeTracking
    {
        void AcceptChanges(Action<string> changing, Action<string> changed);
        void RejectChanges(Action<string> changing, Action<string> changed);
    }
}