using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PdpLiaison
{
    class XacmlValue
    {
        public string strValue;
        public string dataType;

        public XacmlValue(string dataType, string strValue)
        {
            this.dataType = dataType;
            this.strValue = strValue;
        }

        internal void populateXmlNode(XmlNode parentNode)
        {
            XmlNode innerNode = parentNode.OwnerDocument.CreateNode(XmlNodeType.Text, "text", "");
            innerNode.Value = strValue;

            XmlNode node = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AttributeValue", namespaceURI.xacml_schema);
            node.Prefix = parentNode.Prefix;
            XmlAttribute xmlAtt = node.OwnerDocument.CreateAttribute("DataType");
            xmlAtt.Value = dataType;
            node.Attributes.Append(xmlAtt);

            node.AppendChild(innerNode);
            parentNode.AppendChild(node);
        }

        internal bool equals(object obj)
        {
            XacmlValue compObj = (XacmlValue)obj;

            if (this.dataType == compObj.dataType) {
                if (this.strValue == compObj.strValue) {
                    return true;
                }
            }

            return false;
        }

        private static readonly string[] escape =
        {
            /* 0x00 */ "\\u0000",
            /* 0x01 */ "\\u0001",
            /* 0x02 */ "\\u0002",
            /* 0x03 */ "\\u0003",
            /* 0x04 */ "\\u0004",
            /* 0x05 */ "\\u0005",
            /* 0x06 */ "\\u0006",
            /* 0x07 */ "\\u0007",
            /* 0x08 */ "\\b",
            /* 0x09 */ "\\t",
            /* 0x0A */ "\\n",
            /* 0x0B */ "\\u000B",
            /* 0x0C */ "\\f",
            /* 0x0D */ "\\r",
            /* 0x0E */ "\\u000E",
            /* 0x0F */ "\\u000F",
            /* 0x10 */ "\\u0010",
            /* 0x11 */ "\\u0011",
            /* 0x12 */ "\\u0012",
            /* 0x13 */ "\\u0013",
            /* 0x14 */ "\\u0014",
            /* 0x15 */ "\\u0015",
            /* 0x16 */ "\\u0016",
            /* 0x17 */ "\\u0017",
            /* 0x18 */ "\\u0018",
            /* 0x19 */ "\\u0019",
            /* 0x1A */ "\\u001A",
            /* 0x1B */ "\\u001B",
            /* 0x1C */ "\\u001C",
            /* 0x1D */ "\\u001D",
            /* 0x1E */ "\\u001E",
            /* 0x1F */ "\\u001F"
        };

        public string JsonRepresentation()
        {
            switch (dataType)
            {
                case DataTypes._double:
                case DataTypes.boolean:
                case DataTypes.integer:
                    return strValue;
                default:
                    StringBuilder builder = new StringBuilder();
                    builder.Append('"');
                    int i = 0;
                    int start = 0;
                    while (i < strValue.Length)
                    {
                        char ch = strValue[i];
                        if (ch < 0x20)
                        {
                            if (i > start)
                                builder.Append(strValue, start, i - start);
                            builder.Append(escape[ch]);
                            i++;
                            start = i;
                        }
                        else if (ch == '"' || ch == '\\')
                        {
                            if (i > start)
                                builder.Append(strValue, start, i - start);
                            builder.Append('\\');
                            start = i;
                            i++;
                        }
                        else
                            i++;
                    }
                    if (i > start)
                        builder.Append(strValue, start, i - start);
                    builder.Append('"');
                    return builder.ToString();
            }
        }

        public override string ToString()
        {
            return strValue;
        }
    }
}
