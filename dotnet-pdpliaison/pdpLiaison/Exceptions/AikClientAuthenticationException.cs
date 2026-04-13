using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdpLiaison.Exceptions
{
    /// <summary>
    /// This type of exception is thrown when the client fails to authenticate to the PDP, i.e. when:
    /// i) WsseConnector is used and its username-password is not accepted by the PDP, or
    /// ii) XmlSigningConnector is used and the signature verification in the PDP fails, or
    /// iii) ClientSslConnector is used and the certificate used by the AIK to establish https connection to the server is not trusted.
    /// </summary>
    public class AikClientAuthenticationException : AikSecurityException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AikClientAuthenticationException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">string</param>
        public AikClientAuthenticationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">string</param>
        /// <param name="inner">Exception</param>
        public AikClientAuthenticationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
