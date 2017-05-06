using System;
using System.Collections.Generic;
using System.Linq;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns
{
    /// <summary>
    /// Implements a simple property change recorder.
    /// This will remove duplicate property changes
    /// </summary>
    public class PropertyRecorder : IPropertyRecorder
    {
        private readonly HashSet<string> _propertyChanges;

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

        public void Record(string propertyName)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                _propertyChanges.Add(propertyName);
            }
        }

        /// <summary>
        /// 
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