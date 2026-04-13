using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdpLiaison.Exceptions
{
    /// <summary>
    /// This type of exception is thrown when the client cannot authenticate the PDP, i.e. when:
    /// i) XmlSigningConnector is used and the signature verification in the AIK fails, or
    /// ii) ClientSslConnector is used and the certificate used by the PDP to establish https connection to the AIK is not trusted.
    /// </summary>
    public class AikServerAuthenticationException : AikSecurityException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AikServerAuthenticationException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">string</param>
        public AikServerAuthenticationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">string</param>
        /// <param name="inner">Exception</param>
        public AikServerAuthenticationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
