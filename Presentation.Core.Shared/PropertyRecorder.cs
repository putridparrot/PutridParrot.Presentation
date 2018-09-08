using System;
using System.Collections.Generic;
using System.Linq;
using PutridParrot.Presentation.Core.Interfaces;

namespace PutridParrot.Presentation.Core
{
    /// <summary>
    /// Implements a simple property change recorder.
    /// This will remove duplicate property changes
    /// </summary>
    public class PropertyRecorder : IPropertyRecorder
    {
        private readonly HashSet<string> _propertyChanges;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PropertyRecorder()
        {
            _propertyChanges = new HashSet<string>();
        }

        /// <summary>
        /// Can be used to store the property that 
        /// causes that isn't part of the recording
        /// but might be useful to know
        /// </summary>
        public string InitialProperty { get; set; }

        /// <summary>
        /// Record the property change
        /// </summary>
        /// <param name="propertyName"></param>
        public void Record(string propertyName)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                _propertyChanges.Add(propertyName);
            }
        }

        /// <summary>
        /// Returns the property changes that are recorded
        /// for playback, this will also clear any such 
        /// properties
        /// </summary>
        /// <returns></returns>
        public string[] Playback()
        {
            var playback = _propertyChanges.ToArray();
            //using (var e = _propertyChanges.GetEnumerator())
            //{
            //    while (e.MoveNext())
            //    {
            //        yield return e.Current;
            //    }
            //}
            _propertyChanges.Clear();
            return playback;
        }
    }
}