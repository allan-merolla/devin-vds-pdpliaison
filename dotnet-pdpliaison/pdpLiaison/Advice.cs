using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Xml.Linq;

namespace PdpLiaison
{
    /// <summary>
    /// Represents XACML Advice
    /// </summary>
    public class Advice : ObligationAdvice
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        public Advice(XmlNode node)
            : base(node)
        {
            this.obId = node.Attributes["AdviceId"].Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        public Advice(XElement element)
            : base(element)
        {
            this.obId = element.Element("Id").Value;
        }
    }
}
