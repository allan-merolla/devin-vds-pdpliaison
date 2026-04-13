using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PdpLiaison
{
    internal class XacmlCategory
    {
        public string catId;
        public string xmlId;
        public List<XacmlAttribute> attributes;
        public XmlElement xmlContent;

        #region Constructors

        public XacmlCategory(string catId)
        {
            this.catId = catId;
            this.xmlId = "";
            attributes = new List<XacmlAttribute>();
        }

        public XacmlCategory(string catId, string XmlID)
        {
            this.catId = catId;
            this.xmlId = XmlID;
            attributes = new List<XacmlAttribute>();
        }

        #endregion

        internal void populateXmlNode(XmlNode parentNode)
        {
            XmlNode node = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Attributes", namespaceURI.xacml_schema);
            node.Prefix = parentNode.Prefix;

            XmlAttribute catIdNodeAtt = node.OwnerDocument.CreateAttribute("Category");
            catIdNodeAtt.Value = catId;
            node.Attributes.Append(catIdNodeAtt);

            if (!string.IsNullOrEmpty(this.xmlId)) {
                XmlAttribute catXmlId = node.OwnerDocument.CreateAttribute("xml:id");
                catXmlId.Value = xmlId;
                node.Attributes.Append(catXmlId);
            }

            parentNode.AppendChild(node);

            if (xmlContent != null) {
                XmlNode contentPlaceHolder = node.OwnerDocument.CreateNode(
                    XmlNodeType.Element, "Content", namespaceURI.xacml_schema);
                contentPlaceHolder.Prefix = node.Prefix;
                contentPlaceHolder.AppendChild(node.OwnerDocument.ImportNode(xmlContent, true));
                node.AppendChild(node.OwnerDocument.ImportNode(contentPlaceHolder, true));
            }

            foreach (XacmlAttribute att in attributes) {
                att.populateXmlNode(node);
            }
        }

        public void addAttribute(string attribute, string dataType, string value, bool includeInResults)
        {
            foreach (XacmlAttribute att in attributes) {
                if (att.attributeId == attribute) {
                    att.addValue(dataType, value);
                    return;
                }
            }

            XacmlAttribute newAttribute = new XacmlAttribute(attribute, includeInResults);
            newAttribute.addValue(dataType, value);
            attributes.Add(newAttribute);
        }

        internal bool equals(object obj)
        {
            XacmlCategory compObj = (XacmlCategory)obj;
            
            if (this.catId != compObj.catId) {
                return false;
            }
            
            if (this.attributes.Count != compObj.attributes.Count) {
                return false;
            }

            // XOR operation both xmlContents
            if ((this.xmlContent == null && compObj.xmlContent != null) ||
                (this.xmlContent != null && compObj.xmlContent == null))
            {
                return false;
            }
            else if (this.xmlContent != null && compObj.xmlContent != null)
            {
                XElement elem1, elem2;
                XmlElement tmp1 = this.xmlContent;
                XmlElement tmp2 = compObj.xmlContent;
                tmp1.Normalize();
                tmp2.Normalize();
                elem1 = XElement.Parse(tmp1.OuterXml);
                elem2 = XElement.Parse(tmp2.OuterXml);
                if (!XElement.DeepEquals(elem1, elem2))
                {
                    return false;
                }
            }
            
            foreach (XacmlAttribute att in this.attributes) {
                if (!compObj.hasAttribute(att)) {
                    return false;
                }
            }

            return true;
        }

        private bool hasAttribute(XacmlAttribute attribute)
        {
            foreach (XacmlAttribute att in this.attributes) {
                if (att.equals(attribute)) {
                    return true;
                }
            }
            
            return false;
        }

        public override string ToString()
        {
            string retVal = this.xmlId + " ";

            try {
                if (catId.Contains(":")) {
                    retVal += catId.Substring(catId.LastIndexOf(":") + 1);
                }
                if (catId.Contains("/")) {
                    retVal += catId.Substring(catId.LastIndexOf("/") + 1);
                }
            }
            catch {
                retVal += catId;
            }

            foreach (XacmlAttribute att in this.attributes) {
                retVal += " " + att.ToString();
            }

            return retVal;
        }

        internal string jsonRepresentation(int indentationBase)
        {
            string returnVal = "";
            string level1 = "";
            string level2 = "";

            if (indentationBase != -1) {
                level1 = indent(indentationBase + 1);
                level2 = indent(indentationBase + 2);
            }

            string shorthandId = "";
            switch (catId) {
                case AttributeCategory.access_subject:
                    shorthandId = "AccessSubject";
                    break;
                case AttributeCategory.resource:
                    shorthandId = "Resource";
                    break;
                case AttributeCategory.action:
                    shorthandId = "Action";
                    break;
                case AttributeCategory.environment:
                    shorthandId = "Environment";
                    break;
                case AttributeCategory.recipient_subject:
                    shorthandId = "RecipientSubject";
                    break;
                case AttributeCategory.intermediary_subject:
                    shorthandId = "IntermediarySubject";
                    break;
                case AttributeCategory.codebase:
                    shorthandId = "Codebase";
                    break;
                case AttributeCategory.requesting_machine:
                    shorthandId = "RequestingMachine";
                    break;
                default:
                    shorthandId = catId;
                    break;
            }
            returnVal += level1 + "{";

            returnVal += level2 + "\"CategoryId\": \"" + shorthandId + "\",";

            // XML Content
            if (xmlContent != null) {
                returnVal += level2 + "\"Content\": ";
                returnVal += "\"" + xmlContent.OuterXml.Replace("\"", "\\\"") + "\"";
                if (attributes.Count > 0) {
                    returnVal += ",";
                }
            }

            // Attributes 
            if (attributes.Count > 0) {
                returnVal += level2 + "\"Attribute\": [";
                for (int i = 0; i < attributes.Count; i++) {
                    if (i > 0) {
                        returnVal += ",";
                    }
                    if (indentationBase == -1) {
                        returnVal += attributes[i].jsonRepresentation(-1);
                    }
                    else {
                        returnVal += attributes[i]
                            .jsonRepresentation(indentationBase + 3);
                    }
                }
                returnVal += level2 + "]";
            }

            if (!string.IsNullOrEmpty(xmlId)) {
                if (attributes.Count > 0 || xmlContent != null) {
                    returnVal += ",";
                }
                returnVal += level2 + "\"Id\": \"" + xmlId + "\"";
            }

            returnVal += level1 + "}";

            return returnVal;
        }

        private string indent(int level)
        {
            string retVal = "\r\n";

            for (int i = 0; i < level; i++) {
                retVal += "\t";
            }

            return retVal;
        }
    }
}
