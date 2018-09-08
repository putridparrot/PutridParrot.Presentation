using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PutridParrot.Presentation.Exceptions
{
    /// <summary>
    /// Exception used when a type as not been registered
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class TypeNotRegisteredException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TypeNotRegisteredException()
        {
        }

        public TypeNotRegisteredException(string message) : base(message)
        {
        }

        public TypeNotRegisteredException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TypeNotRegisteredException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
