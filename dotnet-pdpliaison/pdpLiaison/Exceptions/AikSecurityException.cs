using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdpLiaison.Exceptions
{
    /// <summary>
    /// This type of exception is thrown when a security problem occurs, e.g. when:
    /// i) the ID of the request does not match the InResponseTo field of the response
    /// ii) (verifySignature == true AND communicationType != XML_SOAP)
    /// </summary>
    public class AikSecurityException : AikException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AikSecurityException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">string</param>
        public AikSecurityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">string</param>
        /// <param name="inner">Exception</param>
        public AikSecurityException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
