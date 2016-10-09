namespace Presentation.Core
{
    /// <summary>
    /// A rule can be defined so that any pre and post property changes
    /// will call the rule and allow other processing to take place. 
    /// For example validation can take place as rules or property chaining.
    /// </summary>
    public abstract class Rule
    {
        /// <summary>
        /// Called when a property is changing
        /// </summary>
        /// <typeparam name="T">The view model type</typeparam>
        /// <param name="viewModel">The view model</param>
        /// <param name="propertyName">The property name changing</param>
        /// <returns>True for success, false for failure</returns>
        public abstract bool PreInvoke<T>(T viewModel, string propertyName)
            where T : IViewModel;

        /// <summary>
        /// Called when a property has changed
        /// </summary>
        /// <typeparam name="T">The view model type</typeparam>
        /// <param name="viewModel">The view model</param>
        /// <param name="propertyName">The property name changed</param>
        /// <returns>True for success, false for failure</returns>
        public abstract bool PostInvoke<T>(T viewModel, string propertyName)
            where T : IViewModel;
    }
}