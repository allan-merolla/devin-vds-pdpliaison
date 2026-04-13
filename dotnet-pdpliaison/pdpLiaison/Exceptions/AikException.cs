using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdpLiaison.Exceptions
{
    /// <summary>
    /// The general exception type in the AIK.
    /// </summary>
    public class AikException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AikException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public AikException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public AikException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
