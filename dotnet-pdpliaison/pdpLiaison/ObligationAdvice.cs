using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Xml.Linq;

namespace PdpLiaison
{
    /// <summary>
    /// Represents the base class for XACML Obligation and XACML Advice
    /// </summary>
    public class ObligationAdvice
    {
        /// <summary>
        /// The identifier of the obligation/advice
        /// </summary>
        protected string obId;
        /// <summary>
        /// The list of attribute assignments
        /// </summary>
        protected List<AttributeAssignment> obAttributes;

        /// <summary>
        /// Obligation ID
        /// </summary>
        public string id
        {
            get { return obId; }
        }

        /// <summary>
        /// Attribute assignments
        /// </summary>
        public List<AttributeAssignment> attributes
        {
            get { return obAttributes; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ObligationAdvice()
        {
            this.obId = "";
            this.obAttributes = new List<AttributeAssignment>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        public ObligationAdvice(XmlNode node)
        {
            //attribute assignments
            obAttributes = new List<AttributeAssignment>();
            foreach (XmlNode childNode in node.ChildNodes) {
                if (childNode.LocalName == "AttributeAssignment") {
                    this.obAttributes.Add(new AttributeAssignment(childNode));
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        public ObligationAdvice(XElement element)
        {
            //attribute assignments
            obAttributes = new List<AttributeAssignment>();
            XElement attributeAssignmentElement = element.Element("AttributeAssignment");
            if (attributeAssignmentElement != null) {
                if (attributeAssignmentElement.Attribute("type").Value == "array") {
                    foreach (var attAssign in attributeAssignmentElement.Elements()) {
                        this.obAttributes.Add(new AttributeAssignment(attAssign));
                    }
                }
                else {
                    this.obAttributes.Add(new AttributeAssignment(attributeAssignmentElement));
                }
            }
        }

        /// <summary>
        /// Returns a string representation of the obligation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string returnVal = "\r\n" + this.obId + ": ";

            foreach (AttributeAssignment aa in obAttributes) {
                returnVal += "\r\n" + aa.ToString();
            }
            return returnVal + "\r\n";
        }
    }
}
