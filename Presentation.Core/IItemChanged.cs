using System.ComponentModel;

namespace Presentation.Core
{
    public interface IItemChanged
    {
        event PropertyChangedEventHandler ItemChanged;
    }
}