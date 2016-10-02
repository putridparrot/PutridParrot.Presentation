namespace Presentation.Core
{
    /// <summary>
    /// Used to support updating functionality, aimed at allowing code 
    /// using this to pause updates/property changes between BeginUpdate 
    /// and EndUpdate calls.
    /// </summary>
    public interface ISupportUpdate
    {
        /// <summary>
        /// Called to pause updates/changes 
        /// </summary>
        void BeginUpdate();
        /// <summary>
        /// Restarts any updates/changes
        /// </summary>
        void EndUpdate();
    }
}