using System.Runtime.Serialization;

namespace UniversityProject.Controllers
{
    [Serializable]
    internal class SomeException : Exception
    {
        public SomeException()
        {
        }

        public SomeException(string? message) : base(message)
        {
        }

        public SomeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SomeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}