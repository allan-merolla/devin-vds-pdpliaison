using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdpLiaison.Exceptions
{
    /// <summary>
    /// This type of exception is thrown when the AIK fails to connect to the PDP.
    /// </summary>
    class AikConnectionException : AikException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AikConnectionException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public AikConnectionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public AikConnectionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
