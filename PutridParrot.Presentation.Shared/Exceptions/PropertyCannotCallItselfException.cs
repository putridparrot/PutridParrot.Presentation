using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PutridParrot.Presentation.Exceptions
{
    /// <summary>
    /// Exception to show a property may not call itself which would
    /// result in a infinite loop
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class PropertyCannotCallItselfException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Default constructor
        /// </summary>
        public PropertyCannotCallItselfException()
        {
        }

        /// <summary>
        /// Constructor takes a message
        /// </summary>
        /// <param name="message">A message to be added to the exception</param>
        public PropertyCannotCallItselfException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor takes a message and an inner exception
        /// </summary>
        /// <param name="message">A message to be added to the exception</param>
        /// <param name="inner">An inner exception to be added to this exception</param>
        public PropertyCannotCallItselfException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PropertyCannotCallItselfException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}