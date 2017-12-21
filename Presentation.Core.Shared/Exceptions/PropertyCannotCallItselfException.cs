using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Presentation.Core.Exceptions
{
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

        public PropertyCannotCallItselfException()
        {
        }

        public PropertyCannotCallItselfException(string message) : base(message)
        {
        }

        public PropertyCannotCallItselfException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PropertyCannotCallItselfException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}