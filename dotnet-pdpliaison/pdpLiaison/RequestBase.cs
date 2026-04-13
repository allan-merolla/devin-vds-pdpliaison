using PdpLiaison.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PdpLiaison
{
    /// <summary>
    /// This abstract class defines a common base for AuthorizationRequest and MultiRequest
    /// </summary>
    public abstract class RequestBase
    {
        internal List<XacmlCategory> categories;
        private bool arTraceSwitch;
        private bool arReturnPolicyIdList;
        private bool arCombinePolicies;

        /// <summary>
        /// The XML representation of the request.
        /// </summary>
        public abstract XmlDocument xmlDoc
        {
            get;
        }

        /// <summary>
        /// This variable is used to store Policies, PolicySets, and ReferencedPolicies.
        /// </summary>
        protected List<XmlElement> arXacmlExtensions;

        /// <summary>
        /// The root node of the request 
        /// </summary>
        protected XmlNode arRequestNode;

        /// <summary>
        /// The internal structure to store the XML representation of the request
        /// </summary>
        protected XmlDocument arXmlDoc;

        /// <summary>
        /// Shows whether the trace capability is on or off.
        /// Read Only
        /// </summary>
        public bool trace
        {
            get { return arTraceSwitch; }
        }

        /// <summary>
        /// Shows whether the ReturnPolicyIdList attribute is true or false.
        /// Read Only
        /// </summary>
        public bool returnPolicyIdList
        {
            get { return arReturnPolicyIdList; }
        }

        /// <summary>
        /// Returns XACML extension elements which include Policy and PoicySet elements.
        /// </summary>
        public List<XmlElement> xacmlExtensions
        {
            get { return arXacmlExtensions; }
        }

        /// <summary>
        /// Returns the value of CombinePolicies attribute.
        /// </summary>
        public bool combinePolicies
        {
            get { return arCombinePolicies; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="traceSwitch">To turn the trace option on or off</param>
        /// <param name="returnPolicyIdSwitch">To set the returnPolicyIdSwitch on or off</param>
        /// <param name="combinePolicies">To turn the combinePolicies on or off</param>
        public RequestBase(bool traceSwitch, bool returnPolicyIdSwitch, bool combinePolicies)
        {
            this.arTraceSwitch = traceSwitch;
            this.arReturnPolicyIdList = returnPolicyIdSwitch;
            this.arCombinePolicies = combinePolicies;
            this.arXacmlExtensions = new List<XmlElement>();
        }

        /// <summary>
        /// This method is to be used for adding an XacmlExtension [Policy, PolicySet, ReferencedPolicies] to the request.
        /// </summary>
        /// <param name="xacmlExtension">The XACML extension element to be added</param>
        public void addElement(XmlElement xacmlExtension)
        {
            if (xacmlExtension == null)
            {
                throw new AikException("Unable to read element. XACML extension is null.");
            }
            arXacmlExtensions.Add(xacmlExtension);
        }

        //SOAP envelopes
        internal void initialiseXmlStructure()
        {
            arXmlDoc = new XmlDocument();
            arXmlDoc.PreserveWhitespace = true;

            arRequestNode = arXmlDoc.CreateNode(XmlNodeType.Element, "Request", namespaceURI.xacml_schema);
            arRequestNode.Prefix = "ns";

            XmlAttribute att6 = arXmlDoc.CreateAttribute("xmlns", "ns", "http://www.w3.org/2000/xmlns/");
            att6.Value = namespaceURI.xacml_schema;
            arRequestNode.Attributes.Append(att6);

            XmlAttribute att7 = arXmlDoc.CreateAttribute("ReturnPolicyIdList");
            att7.Value = arReturnPolicyIdList.ToString().ToLower();
            arRequestNode.Attributes.Append(att7);

            XmlAttribute att8 = arXmlDoc.CreateAttribute("CombinedDecision");
            att8.Value = "false";
            arRequestNode.Attributes.Append(att8);

            arXmlDoc.AppendChild(arRequestNode);
        }

        /// <summary>
        /// Gets the enumerated AttributeDataType and returns a string identifying the data type
        /// </summary>
        /// <param name="attributeDataType"></param>
        /// <returns></returns>
        protected string getDataTypeId(AttributeDataType attributeDataType)
        {
            switch (attributeDataType) {
                case AttributeDataType._double:
                    return DataTypes._double;
                case AttributeDataType._string:
                    return DataTypes._string;
                case AttributeDataType.anyURI:
                    return DataTypes.anyURI;
                case AttributeDataType.base64Binary:
                    return DataTypes.base64Binary;
                case AttributeDataType.boolean:
                    return DataTypes.boolean;
                case AttributeDataType.date:
                    return DataTypes.date;
                case AttributeDataType.dateTime:
                    return DataTypes.dateTime;
                case AttributeDataType.dayTimeDuration:
                    return DataTypes.dayTimeDuration;
                case AttributeDataType.dnsName:
                    return DataTypes.dnsName;
                case AttributeDataType.hexBinary:
                    return DataTypes.hexBinary;
                case AttributeDataType.integer:
                    return DataTypes.integer;
                case AttributeDataType.ipAddress:
                    return DataTypes.ipAddress;
                case AttributeDataType.rfc822Name:
                    return DataTypes.rfc822Name;
                case AttributeDataType.time:
                    return DataTypes.time;
                case AttributeDataType.x500Name:
                    return DataTypes.x500Name;
                //case AttributeDataType.xpathExpression:
                //    return DataTypes.xpathExpression;
                case AttributeDataType.yearMonthDuration:
                    return DataTypes.yearMonthDuration;
                case AttributeDataType.anyType:
                    return DataTypes.anyType;
            }
            return "";
        }

        /// <summary>
        /// Creates an XACML JSON Request object and adds the requestElements to the object.
        /// </summary>
        /// <param name="requestElements">The elements of the Request</param>
        /// <param name="indented">Indented format if true.</param>
        /// <returns>The JSON representation of the request.</returns>
        protected string jsonRequestObject(string requestElements, bool indented)
        {
            string jsonObj = "";
            string returnPolicyIdElement = "";

            if (arReturnPolicyIdList == true) {
                returnPolicyIdElement = (indented) ? 
                    indent(2) + "\"ReturnPolicyIdList\": true, " : 
                    "\"ReturnPolicyIdList\": true, ";
            }

            jsonObj = (indented) ?
                "{" + indent(1) + "\"Request\": {" + returnPolicyIdElement + requestElements + indent(1) + "}" + indent(0) + "}" :
                "{" + "\"Request\": {" + returnPolicyIdElement + requestElements + "}}";

            return jsonObj;
        }

        /// <summary>
        /// Converts the categories within the request to JSON format.
        /// </summary>
        /// <param name="indented">Indented if set to true.</param>
        /// <returns>The JSON representation of the categories within the request.</returns>
        protected string jsonCategories(bool indented)
        {
            string returnValue = "";

            returnValue = (indented) ? 
                indent(2) + "\"Category\": [" : 
                "\"Category\": [";

            for (int i = 0; i < categories.Count; i++) {
                if (i > 0) {
                    returnValue += ",";
                }
                returnValue += (indented) ?
                    categories[i].jsonRepresentation(2) :
                    categories[i].jsonRepresentation(-1);
            }
            returnValue += (indented) ? 
                indent(2) + "]" : 
                "]";

            return returnValue;
        }

        /// <summary>
        /// Indenting the JSON elements.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected string indent(int level)
        {
            string retVal = "\r\n";

            for (int i = 0; i < level; i++) {
                retVal += "\t";
            }

            return retVal;
        }
    }
}
