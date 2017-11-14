using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Presentation.Patterns.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class PropertyNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PropertyNotFoundException()
        {
        }

        public PropertyNotFoundException(string message) : base(message)
        {
        }

        public PropertyNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PropertyNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

}
