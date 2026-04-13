using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Xml.Linq;

namespace PdpLiaison
{
    /// <summary>
    /// AttributeAssignment
    /// </summary>
    public class AttributeAssignment
    {
        private string aaAttributeId;
        private string aaCategoryId;
        private string aaDataType;
        private string aaAttributeValue;

        /// <summary>
        /// The attribute identifier
        /// </summary>
        public string attributeId
        {
            get { return aaAttributeId; }
        }

        /// <summary>
        /// The attribute category identifier
        /// </summary>
        public string categoryId
        {
            get { return aaCategoryId; }
        }

        /// <summary>
        /// The attribute data type
        /// </summary>
        public string dataType
        {
            get { return aaDataType; }
        }

        /// <summary>
        /// The attribute value 
        /// </summary>
        public string attributeValue
        {
            get { return aaAttributeValue; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AttributeAssignment()
        {
            this.aaAttributeId = "";
            this.aaCategoryId = "";
            this.aaDataType = "";
            this.aaAttributeValue = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">The XML node which contains the attribute assignment's data</param>
        public AttributeAssignment(XmlNode node)
        {
            foreach (XmlAttribute xmlAtt in node.Attributes) {
                if (xmlAtt.LocalName == "DataType") {
                    this.aaDataType = xmlAtt.Value;
                }
                else if (xmlAtt.LocalName == "AttributeId") {
                    this.aaAttributeId = xmlAtt.Value;
                }
                else if (xmlAtt.LocalName == "Category") {
                    this.aaCategoryId = xmlAtt.Value;
                }
            }
            try {
                this.aaAttributeValue = node.InnerText;
            }
            catch {
                this.aaAttributeValue = "";
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The XElement which contains the attribute assignment's data</param>
        public AttributeAssignment(XElement element)
        {
            //Category
            try {
                if (element.Element("Category") != null) {
                    this.aaCategoryId = element.Element("Category").Value;
                }
                else {
                    this.aaCategoryId = "";
                }
            }
            catch {
                this.aaCategoryId = "";
            }

            //AttributeId
            try {
                if (element.Element("AttributeId") != null) {
                    this.aaAttributeId = element.Element("AttributeId").Value;
                }
                else {
                    this.aaAttributeId = "";
                }
            }
            catch {
                this.aaAttributeId = "";
            }

            //AttributeId
            try {
                if (element.Element("DataType") != null) {
                    this.aaDataType = element.Element("DataType").Value;
                }
                else {
                    this.aaDataType = "string";
                }
            }
            catch {
                this.aaDataType = "";
            }

            //AttributeId
            try {
                if (element.Element("Value") != null) {
                    this.aaAttributeValue = element.Element("Value").Value;
                }
                else {
                    this.aaAttributeValue = "";
                }
            }
            catch {
                this.aaAttributeValue = "";
            }
        }

        /// <summary>
        /// Returns the AttributeAssignment object in the "[dataType][category][attributeID] = value" format.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return string.Format("[dataType]{0} [category]{1} [attributeID]{2} = \r\n{3}",
                aaDataType,
                aaCategoryId,
                aaAttributeId,
                aaAttributeValue);
        }
    }
}
