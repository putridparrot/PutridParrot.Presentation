namespace Presentation.Core
{
    /// <summary>
    /// Used by view models which wish to support more
    /// advanced features thant he basic notification 
    /// view model. 
    /// </summary>
    public interface IViewModel : INotifyViewModel
    {
        /// <summary>
        /// Gets/sets the view model validation 
        /// implementation
        /// </summary>
        IValidateViewModel Validation { get; set; }
        /// <summary>
        /// Gets/sets the extended data error info. 
        /// implementation
        /// </summary>
        IExtendedDataErrorInfo DataErrorInfo { get; set; }
    }

}