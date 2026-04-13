using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace PdpLiaison
{
    /// <summary>
    /// AuthorizationRequest
    /// </summary>
    public class AuthorizationRequest : RequestBase
    {
        private Guid arUID;
        private bool isXmlDocValid;

        /// <summary>
        /// Returns the unique identifier of the request. 
        /// This attribute is generated automatically and used to distinguish between the requests within a multirequest.
        /// </summary>
        public Guid uid
        {
            get { return arUID; }
        }

        /// <summary>
        /// The XML representation of the request.
        /// </summary>
        public override XmlDocument xmlDoc
        {
            get
            {
                if (!isXmlDocValid) {
                    populateXmlNode();
                }
                return arXmlDoc;
            }
        }

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="traceSwitch">Trace information will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <param name="returnPolicyIdSwitch">PolicyIdList will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <param name="combinePolicies">PDP will combine its policies with the policies provided 
        /// in the request if this flag is set to true.</param>
        public AuthorizationRequest(bool traceSwitch, bool returnPolicyIdSwitch, bool combinePolicies)
            : base(traceSwitch, returnPolicyIdSwitch, combinePolicies)
        {
            categories = new List<XacmlCategory>();
            arUID = Guid.NewGuid();
            isXmlDocValid = false;
            //Adding the empty attribute for the trace information
            if (traceSwitch == true) {
                addElement(AttributeCategory.environment,
                    EnvironmentAttributes.viewds_trace,
                    DataTypes.anyType,
                    "",
                    true);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="traceSwitch">Trace information will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        /// <param name="returnPolicyIdSwitch">PolicyIdList will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        public AuthorizationRequest(bool traceSwitch, bool returnPolicyIdSwitch)
            : base(traceSwitch, returnPolicyIdSwitch, true)
        {
            categories = new List<XacmlCategory>();
            arUID = Guid.NewGuid();
            isXmlDocValid = false;
            //Adding the empty attribute for the trace information
            if (traceSwitch == true) {
                addElement(AttributeCategory.environment,
                    EnvironmentAttributes.viewds_trace,
                    DataTypes.anyType,
                    "",
                    true);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="traceSwitch">Trace information will be included in the corresponding 
        /// AuthorizationResponse object if this parameter is set to true.</param>
        public AuthorizationRequest(bool traceSwitch)
            : base(traceSwitch, false, true)
        {
            categories = new List<XacmlCategory>();
            arUID = Guid.NewGuid();
            //Adding the empty attribute for the trace information
            if (traceSwitch == true) {
                addElement(AttributeCategory.environment,
                    EnvironmentAttributes.viewds_trace,
                    DataTypes.anyType,
                    "",
                    true);
            }
            isXmlDocValid = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorizationRequest()
            : base(false, false, true)
        {
            categories = new List<XacmlCategory>();
            arUID = Guid.NewGuid();
            isXmlDocValid = false;
        }

        #endregion

        private void addElement(string category, string attribute, string dataType, string value, bool includeInResults)
        {
            isXmlDocValid = false;

            foreach (XacmlCategory cat in categories) {
                if (cat.catId == category) {
                    cat.addAttribute(attribute, dataType, value, includeInResults);
                    return;
                }
            }

            XacmlCategory newCategory = new XacmlCategory(category);
            newCategory.addAttribute(attribute, dataType, value, includeInResults);
            categories.Add(newCategory);
        }

        /// <summary>
        /// This method is to be used for adding an attribute to the request.
        /// </summary>
        /// <param name="category">You may use the static class "AttributeCategory" which contains the list of XACML standard categories.</param>
        /// <param name="attribute">You may use one of the static classes "SubjectAttributes", "ResourceAttributes", "ActionAttributes", or "EnvironmentAttributes" which contain lists of XACML standard attribute identifiers.</param>
        /// <param name="dataType">String.</param>
        /// <param name="value">The value of the attribute.</param>
        public void addElement(string category, string attribute, string dataType, string value)
        {
            addElement(category, attribute, dataType, value, false);
        }

        /// <summary>
        /// This method is to be used for adding an attribute to the request.
        /// </summary>
        /// <param name="category">You may use the static class "AttributeCategory" which contains the list of XACML standard categories.</param>
        /// <param name="attribute">You may use one of the static classes "SubjectAttributes", "ResourceAttributes", "ActionAttributes", or "EnvironmentAttributes" which contain lists of XACML standard attribute identifiers.</param>
        /// <param name="attributeDataType">AttributeDataType enumeration.</param>
        /// <param name="value">The value of the attribute.</param>
        public void addElement(string category, string attribute, AttributeDataType attributeDataType, string value)
        {
            string dataType = getDataTypeId(attributeDataType);

            addElement(category, attribute, dataType, value);
        }

        /// <summary>
        /// This method sets the Content element of the specified category.
        /// </summary>
        /// <param name="category">The category to which the content will be assigned.</param>
        /// <param name="content">The XmlElement object to be assigned as the Content.</param>
        public void setContent(string category, XmlElement content)
        {
            isXmlDocValid = false;

            foreach (XacmlCategory cat in categories) {
                if (cat.catId == category) {
                    cat.xmlContent = content;
                    return;
                }
            }

            XacmlCategory newCategory = new XacmlCategory(category);
            newCategory.xmlContent = content;
            categories.Add(newCategory);
        }

        internal void addGuid()
        {
            addElement(AttributeCategory.environment,
                "http://viewds.com/xacml/environment/request-id",
                getDataTypeId(AttributeDataType._string),
                arUID.ToString(),
                true);
        }

        private void populateXmlNode()
        {
            initialiseXmlStructure();
            foreach (XacmlCategory cat in categories) {
                cat.populateXmlNode(arRequestNode);
            }
            isXmlDocValid = true;
        }

        /// <summary>
        /// Returns the JSON representation of the request.
        /// </summary>
        /// <param name="indented">Indents the return value if set to true.</param>
        /// <returns>The JSON representation of the request.</returns>
        public string jsonRepresentation(bool indented)
        {
            return jsonRequestObject(jsonCategories(indented), indented);
        }
    }
}
