using System;

namespace PutridParrot.Presentation.Core.Attributes
{
    /// <summary>
    /// Special type of property attribute which adheres
    /// to the rule class unlike some of the other attributes
    /// which occur once, such as CreateInstance, Comparer etc.
    /// These "rule" occur every on changing and every on changed
    /// event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RuleAttribute : PropertyAttribute
    {
        /// <summary>
        /// Should be overridden in a derived class
        /// to implement functionality which occurs 
        /// prior to a property changing.
        /// </summary>
        /// <param name="o">The view model this is invoked from</param>
        /// <returns>True upon success, else False</returns>
        public virtual bool PreInvoke(object o)
        {
            return true;
        }

        /// <summary>
        /// Should be overridden in a derived class
        /// to implement functionality which occurs 
        /// after a property changes.
        /// </summary>
        /// <param name="o">The view model this is invoked from</param>
        /// <returns>True upon success, else False</returns>
        public virtual bool PostInvoke(object o)
        {
            return true;
        }
    }
}