namespace Presentation.Core.Shared.Interfaces
{
    /// <summary>
    /// Defines an interface for commands to allow code
    /// outside of the command to raise a CanExecute change
    /// event and allow binding to update
    /// </summary>
    public interface IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Causes a CanExecuteChanged event to be raised
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
