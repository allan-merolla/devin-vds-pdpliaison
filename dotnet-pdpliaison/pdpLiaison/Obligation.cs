using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Xml.Linq;

namespace PdpLiaison
{
    /// <summary>
    /// Represents XACML Obligation
    /// </summary>
    public class Obligation : ObligationAdvice
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        public Obligation(XmlNode node)
            : base(node)
        {
            this.obId = node.Attributes["ObligationId"].Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        public Obligation(XElement element)
            : base(element)
        {
            this.obId = element.Element("Id").Value;
        }
    }
}
