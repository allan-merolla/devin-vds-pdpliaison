using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PdpLiaison
{
    class XacmlAttribute
    {
        public string attributeId;
        public bool includeInResult;
        public List<XacmlValue> values;

        public XacmlAttribute(string attributeId, bool incInResult)
        {
            this.attributeId = attributeId;
            this.includeInResult = incInResult;
            values = new List<XacmlValue>();
        }

        internal void populateXmlNode(XmlNode parentNode)
        {
            XmlNode node = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Attribute", namespaceURI.xacml_schema);
            node.Prefix = parentNode.Prefix;

            XmlAttribute attId = node.OwnerDocument.CreateAttribute("AttributeId");
            attId.Value = attributeId;
            node.Attributes.Append(attId);

            XmlAttribute include = node.OwnerDocument.CreateAttribute("IncludeInResult");
            include.Value = includeInResult.ToString().ToLower();
            node.Attributes.Append(include);

            parentNode.AppendChild(node);

            foreach (XacmlValue val in values) {
                val.populateXmlNode(node);
            }
        }

        public void addValue(string dataType, string value)
        {
            foreach (XacmlValue val in values) {
                if (val.dataType == dataType && val.strValue == value) {
                    return;
                }
            }

            XacmlValue newValue = new XacmlValue(dataType, value);
            values.Add(newValue);
        }

        internal bool equals(object obj)
        {
            XacmlAttribute compObj = (XacmlAttribute)obj;
            
            if (this.attributeId != compObj.attributeId) {
                return false;
            }

            if (this.values.Count != compObj.values.Count) {
                return false;
            }

            foreach (XacmlValue val in this.values) {
                if (!compObj.hasValue(val)) {
                    return false;
                }
            }

            return true;
        }

        private bool hasValue(XacmlValue value)
        {
            foreach (XacmlValue val in this.values) {
                if (val.equals(value)) {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string retVal = "";

            try {
                if (attributeId.Contains(":")) {
                    retVal = attributeId.Substring(attributeId.LastIndexOf(":") + 1);
                }
                if (attributeId.Contains("/")) {
                    retVal = attributeId.Substring(attributeId.LastIndexOf("/") + 1);
                }
            }
            catch {
                retVal = attributeId;
            }

            foreach (XacmlValue val in this.values) {
                retVal += " " + val.ToString();
            }

            return retVal;
        }

        private string jsonAddAttribute(int indentationBase, XacmlValue value)
        {
            string jsonRep = "";
            string level0 = "";
            string level1 = "";

            if (indentationBase != -1) {
                level0 = indent(indentationBase);
                level1 = indent(indentationBase + 1);
            }

            jsonRep += level0 + "{";

            //Attribute Id
            jsonRep += level1 + "\"AttributeId\": \"" + attributeId + "\",";

            //Data type
            if (value.dataType != DataTypes._string) {
                jsonRep += level1 + "\"DataType\": \"" + shorthand(value.dataType) + "\",";
            }

            //Value
            jsonRep += level1 + "\"Value\": [" + value.JsonRepresentation() + "]";

            //IncludeInResult
            if (includeInResult == true) {
                jsonRep += "," + level1 + "\"IncludeInResult\": true ";
            }

            jsonRep += level0 + "}";

            return jsonRep;
        }

        internal string jsonRepresentation(int indentationBase)
        {
            string jsonRep = "";

            if (attributeId == EnvironmentAttributes.viewds_trace) {
                if (includeInResult == true) {
                    throw new Exceptions.AikException("Tracing is not supported for the communication type JSON_REST.");
                }
            }

            if (values.Count == 0) {
                return "";
            }

            for (int i = 0; i < values.Count; i++) {
                jsonRep += jsonAddAttribute(indentationBase, values[i]);
               
                if (i < values.Count - 1) {
                    jsonRep += ", ";
                }
            }

            return jsonRep;
        }

        private string shorthand(string datatype)
        {
            switch (datatype) {
                case DataTypes._string:
                    return "string";
                case DataTypes.boolean:
                    return "boolean";
                case DataTypes.integer:
                    return "integer";
                case DataTypes._double:
                    return "double";
                case DataTypes.time:
                    return "time";
                case DataTypes.date:
                    return "date";
                case DataTypes.dateTime:
                    return "dateTime";
                case DataTypes.dayTimeDuration:
                    return "dayTimeDuration";
                case DataTypes.yearMonthDuration:
                    return "yearMonthDuration";
                case DataTypes.anyURI:
                    return "anyURI";
                case DataTypes.hexBinary:
                    return "hexBinary";
                case DataTypes.base64Binary:
                    return "base64Binary";
                case DataTypes.rfc822Name:
                    return "rfc822Name";
                case DataTypes.x500Name:
                    return "x500Name";
                case DataTypes.ipAddress:
                    return "ipAddress";
                case DataTypes.dnsName:
                    return "dnsName";
                //case DataTypes.xpathExpression:
                //    throw new NotImplementedException();
                case DataTypes.anyType:
                    throw new Exceptions.AikException("The data type anyType is not supported in JSON format.");
            }

            return datatype;
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
