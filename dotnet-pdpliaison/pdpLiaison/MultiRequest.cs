using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace PdpLiaison
{
    /// <summary>
    /// MultiRequest
    /// </summary>
    public class MultiRequest : RequestBase
    {
        private Hashtable requestReferences;
        private bool isXmlDocValid;

        /// <summary>
        /// The XML representation of the request.
        /// </summary>
        public override  XmlDocument xmlDoc
        {
            get
            {
                if (!isXmlDocValid) {
                    populateXmlNode();
                }
                return arXmlDoc;
            }
        }

        /// <summary>
        /// Returns the list of AuthorizationRequest objects added to the MultiRequest.
        /// </summary>
        public List<AuthorizationRequest> requests;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="returnPolicyIdList">PolicyIdList will be included in the corresponding 
        /// MultiResponse object if this parameter is set to true.</param>
        public MultiRequest(bool returnPolicyIdList)
            : base(false, returnPolicyIdList, true)
        {
            categories = new List<XacmlCategory>();
            requestReferences = new Hashtable();
            isXmlDocValid = false;
            requests = new List<AuthorizationRequest>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="returnPolicyIdList">PolicyIdList will be included in the corresponding 
        /// MultiResponse object if this parameter is set to true.</param>
        /// <param name="combinePolicies">PDP will combine its policies with the policies provided 
        /// in the request if this flag is set to true.</param>
        public MultiRequest(bool returnPolicyIdList, bool combinePolicies)
            : base(false, returnPolicyIdList, combinePolicies)
        {
            categories = new List<XacmlCategory>();
            requestReferences = new Hashtable();
            isXmlDocValid = false;
            requests = new List<AuthorizationRequest>();
        }

        private string addElement(XacmlCategory category)
        {
            //If the element to be added has already been added to the multirequest, then return the existing ID.
            foreach (XacmlCategory cat in categories) {
                if (cat.equals(category)) {
                    return cat.xmlId;
                }
            }

            string catXmlId = "element-id-" + (categories.Count + 1).ToString();
            XacmlCategory newCategory = new XacmlCategory(category.catId, catXmlId);
            foreach (XacmlAttribute att in category.attributes) {
                foreach (XacmlValue val in att.values) {
                    newCategory.addAttribute(att.attributeId, val.dataType, val.strValue, att.includeInResult);
                }
            }
            newCategory.xmlContent = category.xmlContent;
            categories.Add(newCategory);
            isXmlDocValid = false;

            return catXmlId;
        }

        /// <summary>
        /// Adds an AuthorizationRequest object to the MultiRequest.
        /// </summary>
        /// <param name="request">The AuthorizationRequest to be added.</param>
        public void addRequest(AuthorizationRequest request)
        {
            if (request.xacmlExtensions != null) {
                if (request.xacmlExtensions.Count > 0) {
                    throw new Exceptions.AikException("Requests with XACML extension elements cannot be added to a MultiRequest object. \r\n" +
                        "Try adding the extension elements to the MultiRequest object.");
                }
            }

            string refId;
            isXmlDocValid = false;
            
            List<string> catRefs = new List<string>();

            bool reqIdAdded = false;

            foreach (XacmlCategory cat in request.categories) {
                if (cat.catId == AttributeCategory.environment) {
                    cat.addAttribute(
                        "http://viewds.com/xacml/environment/request-id",
                        getDataTypeId(AttributeDataType._string),
                        request.uid.ToString(),
                        true);
                    reqIdAdded = true;
                }
                refId = addElement(cat);
                catRefs.Add(refId);
            }

            if (reqIdAdded == false) {
                string catXmlId = "element-id-" + (categories.Count + 1).ToString();
                XacmlCategory envCat = new XacmlCategory(AttributeCategory.environment, catXmlId);
                envCat.addAttribute(
                    "http://viewds.com/xacml/environment/request-id",
                    getDataTypeId(AttributeDataType._string),
                    request.uid.ToString(),
                    true);
                categories.Add(envCat);
                catRefs.Add(catXmlId);
            }

            requests.Add(request);
            requestReferences.Add(request.uid, catRefs);
        }

        internal void populateXmlNode()
        {
            initialiseXmlStructure();

            XmlNode multiRequestsNode =
                arRequestNode.OwnerDocument.CreateNode(
                XmlNodeType.Element, "MultiRequests", namespaceURI.xacml_schema);
            multiRequestsNode.Prefix = arRequestNode.Prefix;

            List<string> catRef;
            foreach (AuthorizationRequest req in requests) {
                catRef = (List<string>)requestReferences[req.uid];
                addRequestReferences(multiRequestsNode, catRef);
            }

            foreach (XacmlCategory cat in categories) {
                cat.populateXmlNode(arRequestNode);
            }

            arRequestNode.AppendChild(multiRequestsNode);

            isXmlDocValid = true;
        }

        private void addRequestReferences(XmlNode parentNode, List<string> catRefs)
        {
            XmlNode requestRefNode = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "RequestReference", namespaceURI.xacml_schema);
            requestRefNode.Prefix = parentNode.Prefix;

            foreach (string catRef in catRefs) {
                XmlNode attRefNode = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "AttributesReference", namespaceURI.xacml_schema);
                attRefNode.Prefix = requestRefNode.Prefix;

                XmlAttribute refId = attRefNode.OwnerDocument.CreateAttribute("ReferenceId");
                refId.Value = catRef;
                attRefNode.Attributes.Append(refId);

                requestRefNode.AppendChild(attRefNode);
            }

            parentNode.AppendChild(requestRefNode);
        }

        /// <summary>
        /// Returns the JSON representation of the MultiRequest.
        /// </summary>
        /// <param name="indented">Indents the return value if set to true.</param>
        /// <returns>The JSON representation of the MultiRequest.</returns>
        public string jsonRepresentation(bool indented)
        {
            string references = "";
            
            references = (indented) ?
                 "," + indent(2) + "\"MultiRequests\": {" :
                 ",\"MultiRequests\": {";
            
            references += (indented) ?
                indent(3) + "\"RequestReference\": [" :
                "\"RequestReference\": [";

            for (int i = 0; i < requests.Count; i++) {
                if (i > 0) {
                    references += ",";
                }
                references += (indented) ?
                    addJsonRequestReference(requests[i], 4) :
                    addJsonRequestReference(requests[i], -1);
            }

            references += (indented) ?
                indent(3) + "]" :
                "]";

            references += (indented) ?
                indent(2) + "}" :
                "}";

            string requestElements = jsonCategories(indented) + references;
            string jsonRep = jsonRequestObject(requestElements, indented);

            return jsonRep;
        }

        private string addJsonRequestReference(AuthorizationRequest req, int indentationBase)
        {
            string returnValue = "";

            string level0 = "";
            string level1 = "";

            if (indentationBase != -1) {
                level0 = indent(indentationBase);
                level1 = indent(indentationBase + 1);
            }

            List<string> catRef = (List<string>)requestReferences[req.uid];

            returnValue = level0 + "{" + level1 + "\"ReferenceId\": [";
            
            for (int i = 0; i < catRef.Count; i++) {
                if (i > 0) {
                    returnValue += ", ";
                }
                returnValue += "\"" + catRef[i] + "\"";
            }
            
            returnValue += "]" + level0 + "}";

            return returnValue;
        }
    }
}
