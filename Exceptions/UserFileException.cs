using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChubbHubMVVM.Exceptions
{
    public class UserFileException : Exception
    {
        public UserFileException()
        {
        }

        public UserFileException(string? message) : base(message)
        {
        }

        public UserFileException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
