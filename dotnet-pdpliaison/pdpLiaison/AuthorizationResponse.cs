using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Net;

namespace PdpLiaison
{
    /// <summary>
    /// AuthorizationResponse
    /// </summary>
    public class AuthorizationResponse
    {
        //arDecision holds the string value of the Decision node of the result coming from the PDP.
        private string arDecision;
        private string arXacmlStatusMsg;
        private string arXacmlStatusCode;
        private string arXacmlStatusDetail;
        private List<Obligation> arObligations;
        private List<Advice> arAssociatedAdvice;
        private string arTraceInfo;
        private List<PolicyIdReference> arPolicyIdRefs;
        private string arRequestId;
        private Result arDenyBiasedResult;

        #region Properties

        /// <summary>
        /// Returns a deny-biased interpretation of the results.
        /// Returns:
        ///     Result.deny if (pdpDecision != XacmlResult.permit AND there are no obligations to be fulfilled)
        ///     Result.denyWithObligations if (pdpDecision != XacmlResult.permit AND there is at least one obligation to be fulfilled)
        ///     Result.denyDueToUnrecognizedObligations if (pdpDecision == XacmlResult.permit AND there is at least one unrecognized obligation)
        ///     Result.denyUnlessAllObligationsSatisfied if (pdpDecision == XacmlResult.permit AND there is at least one obligation to be fulfilled)
        ///     Result.permit if (pdpDecision == XacmlResult.permit AND there are no obligations to be fulfilled)
        /// </summary>
        public Result result
        {
            get { return arDenyBiasedResult; }
        }

        /// <summary>
        /// List of Obligations
        /// </summary>
        public List<Obligation> obligations
        {
            get { return arObligations; }
        }

        /// <summary>
        /// List of Advice
        /// </summary>
        public List<Advice> associatedAdvice
        {
            get { return arAssociatedAdvice; }
        }

        /// <summary>
        /// The XACML Result returned by the PDP
        /// </summary>
        internal XacmlResult pdpDecision
        {
            get
            {
                switch (arDecision.ToLower()) {
                    case "deny":
                        return XacmlResult.deny;
                    case "indeterminate":
                        return XacmlResult.indeterminate;
                    case "notapplicable":
                        return XacmlResult.notApplicable;
                    case "permit":
                        return XacmlResult.permit;
                    default:
                        return XacmlResult.noResponse;
                }
            }
        }

        /// <summary>
        /// XacmlStatus
        /// </summary>
        public XacmlStatus xacmlStatus
        {
            get
            {
                switch (arXacmlStatusCode) {
                    case "urn:oasis:names:tc:xacml:1.0:status:missing-attribute":
                        return XacmlStatus.missingAttribute;
                    case "urn:oasis:names:tc:xacml:1.0:status:ok":
                        return XacmlStatus.ok;
                    case "urn:oasis:names:tc:xacml:1.0:status:processing-error":
                        return XacmlStatus.processingError;
                    case "urn:oasis:names:tc:xacml:1.0:status:syntax-error":
                        return XacmlStatus.syntaxError;
                    default:
                        return XacmlStatus.notDefined;
                }
            }
        }

        /// <summary>
        /// Returns the XACML Status Message of the response.
        /// </summary>
        public string XacmlStatusMessage
        {
            get { return arXacmlStatusMsg; }
        }

        /// <summary>
        /// Returns the XACML Status Detail of the response.
        /// </summary>
        public string XacmlStatusDetail
        {
            get { return arXacmlStatusDetail; }
        }

        /// <summary>
        /// Returns the trace information received from the PDP if the trace switch of the request has been set to true.
        /// </summary>
        public string traceInfo
        {
            get { return arTraceInfo; }
        }

        /// <summary>
        /// Returns the ID list of the policies evaluated if the returnPolicyIdList switch of the request has been set to true.
        /// </summary>
        public List<PolicyIdReference> policyIdReferences
        {
            get { return arPolicyIdRefs; }
        }

        /// <summary>
        /// The ID of the corresponding AuthorizationReuest.
        /// </summary>
        public string requestId
        {
            get { return arRequestId; }
            //get
            //{
            //    if (string.IsNullOrEmpty(arRequestId)) {
            //        return Guid.NewGuid();
            //    }
            //    return new Guid(arRequestId);
            //}
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">XmlNode</param>
        internal AuthorizationResponse(XmlNode node)
        {
            arObligations = new List<Obligation>();
            arAssociatedAdvice = new List<Advice>();
            arPolicyIdRefs = new List<PolicyIdReference>();
            arRequestId = "";
            arTraceInfo = "";
            populateValues(node);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal AuthorizationResponse()
        {
            arObligations = new List<Obligation>();
            arAssociatedAdvice = new List<Advice>();
            arPolicyIdRefs = new List<PolicyIdReference>();
            arRequestId = "";
            arTraceInfo = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="responseNode">The response received from the PDP in the JSON format</param>
        public AuthorizationResponse(XElement responseNode)
        {
            arObligations = new List<Obligation>();
            arAssociatedAdvice = new List<Advice>();
            arPolicyIdRefs = new List<PolicyIdReference>();
            arRequestId = "";
            arTraceInfo = "";
            populateValues(responseNode);
        }

        private void populateValues(XElement responseNode)
        {
            //decision
            arDecision = responseNode.Element("Decision").Value;

            //statusXacml
            arXacmlStatusCode = "";
            arXacmlStatusMsg = "";
            arXacmlStatusDetail = "";
            XElement statusElement = responseNode.Element("Status");
            if (statusElement != null) {
                if (statusElement.Element("StatusCode") != null) {
                    arXacmlStatusCode = statusElement.Element("StatusCode").Value;
                }
                if (statusElement.Element("StatusMessage") != null) {
                    arXacmlStatusMsg = statusElement.Element("StatusMessage").Value;
                }
                if (statusElement.Element("StatusDetail") != null) {
                    arXacmlStatusDetail = statusElement.Element("StatusDetail").Value;
                }
            }

            //obligations
            XElement obligationsElement = responseNode.Element("Obligations");
            if (obligationsElement != null) {
                if (obligationsElement.Attribute("type").Value == "array") {
                    foreach (var oblElement in obligationsElement.Elements()) {
                        arObligations.Add(new Obligation(oblElement));
                    }
                }
                else {
                    arObligations.Add(new Obligation(obligationsElement));
                }
            }

            //advice
            XElement adviceElement = responseNode.Element("AssociatedAdvice");
            if (adviceElement != null) {
                if (adviceElement.Attribute("type").Value == "array") {
                    foreach (var advElement in adviceElement.Elements()) {
                        arAssociatedAdvice.Add(new Advice(advElement));
                    }
                }
                else {
                    arAssociatedAdvice.Add(new Advice(adviceElement));
                }
            }

            //policy identifier list
            XElement policyIdListElement = responseNode.Element("PolicyIdentifierList");
            if (policyIdListElement != null) {
                XElement policyIdRefElement = policyIdListElement.Element("PolicyIdReference");
                if (policyIdRefElement != null) {
                    arPolicyIdRefs = new List<PolicyIdReference>();
                    if (policyIdRefElement.Attribute("type").Value == "array") {
                        foreach (var prElement in policyIdRefElement.Elements()) {
                            PolicyIdReference pRef = new PolicyIdReference();
                            pRef.uuid = prElement.Element("Id").Value;
                            pRef.version = prElement.Element("Version").Value;
                            arPolicyIdRefs.Add(pRef);
                        }
                    }
                    else {
                        PolicyIdReference pRef = new PolicyIdReference();
                        pRef.uuid = policyIdRefElement.Element("Id").Value;
                        pRef.version = policyIdRefElement.Element("Version").Value;
                        arPolicyIdRefs.Add(pRef);
                    }
                }
            }

            // request id
            XElement categories = responseNode.Element("Category");
            if (categories != null && categories.Attribute("type").Value == "array") {
                foreach (var categoryElement in categories.Elements())
                {
                    if (categoryElement.Element("CategoryId").Value == AttributeCategory.environment) {
                        XElement attributes = categoryElement.Element("Attribute");
                        if (attributes != null &&
                                attributes.Attribute("type").Value == "array") {
                            foreach (var attributeElement in attributes.Elements())
                            {
                                if (attributeElement.Element("AttributeId").Value == EnvironmentAttributes.viewds_request_id)
                                {
                                    XElement values = attributeElement.Element("Value");
                                    if (values != null &&
                                            values.Attribute("type").Value == "array")
                                    {
                                        foreach (var valueElement in values.Elements())
                                        {
                                            arRequestId = valueElement.Value;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void populateValues(XmlNode node)
        {
            #region performance check
            /* This has been commented out temporarily.
             * In future, I should make a comparison between the performance of the 
             * GetElementByTagName method and the loops currently being used.
             * 

            XmlNodeList nodeList;

            //decision
            nodeList = node.OwnerDocument.GetElementsByTagName("Decision", namespaceURI.xacml_schema);
            if (nodeList.Count > 0) {
                arDecision = nodeList[0].InnerText;
            }

            //Xacml Status Code
            nodeList = node.OwnerDocument.GetElementsByTagName("StatusCode", namespaceURI.xacml_schema);
            if (nodeList.Count > 0) {
                arXacmlStatusCode = nodeList[0].Attributes["Value"].Value;
            }

            //Xacml Status Message
            nodeList = node.OwnerDocument.GetElementsByTagName("StatusMessage", namespaceURI.xacml_schema);
            if (nodeList.Count > 0) {
                arXacmlStatusMsg = nodeList[0].InnerText;
            }

            //Obligations
            nodeList = node.OwnerDocument.GetElementsByTagName("Obligation", namespaceURI.xacml_schema);
            foreach (XmlNode obNode in nodeList) {
                arObligations.Add(new Obligation(obNode));
            }

            //Advice
            nodeList = node.OwnerDocument.GetElementsByTagName("Advice", namespaceURI.xacml_schema);
            foreach (XmlNode adNode in nodeList) {
                arAssociatedAdvice.Add(new Advice(adNode));
            }

             */
            #endregion

            foreach (XmlNode child5 in node.ChildNodes) {
                if (child5.NamespaceURI != namespaceURI.xacml_schema) {
                    continue;
                }
                //decision
                if (child5.LocalName == "Decision") {
                    arDecision = child5.InnerText;
                }
                //statusXacml
                else if (child5.LocalName == "Status") {
                    foreach (XmlNode child6 in child5.ChildNodes) {
                        if (child6.LocalName == "StatusCode") {
                            arXacmlStatusCode = child6.Attributes["Value"].Value;
                        }
                        else if (child6.LocalName == "StatusMessage") {
                            arXacmlStatusMsg = child6.InnerText;
                        }
                        else if (child6.LocalName == "StatusDetail") {
                            arXacmlStatusDetail = child6.InnerText;
                        }
                    }
                }
                //obligations
                else if (child5.LocalName == "Obligations") {
                    foreach (XmlNode child6 in child5.ChildNodes) {
                        //obligation
                        if (child6.LocalName == "Obligation") {
                            arObligations.Add(new Obligation(child6));
                        }
                    }
                }
                else if (child5.LocalName == "AssociatedAdvice") {
                    foreach (XmlNode child6 in child5.ChildNodes) {
                        //advice
                        if (child6.LocalName == "Advice") {
                            arAssociatedAdvice.Add(new Advice(child6));
                        }
                    }
                }
                //trace info and request-id
                else if (child5.LocalName == "Attributes") {
                    foreach (XmlNode child6 in child5.ChildNodes) {
                        if (child6.LocalName == "Attribute") {
                            //trace info
                            if (child6.Attributes["AttributeId"].Value == EnvironmentAttributes.viewds_trace) {
                                foreach (XmlNode child7 in child6.ChildNodes) {
                                    if (child7.LocalName == "AttributeValue") {
                                        foreach (XmlNode child8 in child7.ChildNodes) {
                                            if (child8.LocalName == "trace") {
                                                arTraceInfo = "Algorithm=" + child8.Attributes["algorithm"].Value.ToString() + "\r\n\r\n";
                                                foreach (XmlNode child9 in child8.ChildNodes) {
                                                    arTraceInfo += formatIndented(child9);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //request-id
                            if (child6.Attributes["AttributeId"].Value == EnvironmentAttributes.viewds_request_id) {
                                foreach (XmlNode child7 in child6.ChildNodes) {
                                    if (child7.LocalName == "AttributeValue") {
                                        arRequestId = child7.InnerXml;
                                    }
                                }
                            }
                        }
                    }
                }
                //policy identifier list
                else if (child5.LocalName == "PolicyIdentifierList") {
                    foreach (XmlNode child6 in child5.ChildNodes) {
                        if (child6.LocalName == "PolicyIdReference") {
                            PolicyIdReference pRef = new PolicyIdReference();
                            pRef.version = child6.Attributes["Version"].Value.ToString();
                            pRef.uuid = child6.InnerXml.ToString();
                            arPolicyIdRefs.Add(pRef);
                        }
                    }
                }
            }
        }

        private string formatIndented(XmlNode node)
        {
            //Format the Xml of the request
            StringBuilder sb = new StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);
            XmlTextWriter xtw = null;
            try {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                node.WriteTo(xtw);
            }
            finally {
                if (xtw != null) {
                    xtw.Close();
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string representation of the authorization response
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            string temp = "";

            try {
                temp += "Deny-biased Decision: " + arDenyBiasedResult.ToString();
                temp += "\r\nPDP Decision: " + pdpDecision.ToString();
                temp += "\r\nStatus Code: " + xacmlStatus.ToString();
                if (!string.IsNullOrEmpty(XacmlStatusMessage)) {
                    temp += "\r\nStatus Message: " + XacmlStatusMessage;
                }
                if (!string.IsNullOrEmpty(XacmlStatusDetail)) {
                    temp += "\r\nStatus Detail: " + XacmlStatusDetail;
                }

                //Adding the obligation and advice info the the string if available
                temp += "\r\n";
                foreach (Obligation ob in obligations) {
                    temp += "\r\nObligation: " + ob.ToString();
                }
                foreach (Advice ad in associatedAdvice) {
                    temp += "\r\nAdvice: " + ad.ToString();
                }

                //Adding the trace info to the string if available
                if (!string.IsNullOrEmpty(traceInfo)) {
                    temp += "\r\nTrace Information {\r\n" +
                        traceInfo + "\r\n}\r\n";
                }

                //Adding the policy ID references to the string if available
                if (policyIdReferences.Count > 0) {
                    temp += "\r\nPolicyIdReference:\r\n";
                    foreach (PolicyIdReference pRef in policyIdReferences) {
                        temp += "ID=" + pRef.uuid;
                        temp += ", Version=" + pRef.version;
                        temp += "\r\n";
                    }
                }
            }
            catch {
            }

            return temp;
        }

        /// <summary>
        /// This method sets the arDenyBiasedResult which holds a deny-biased interpretation of the pdpDecision.
        /// The arDenyBiasedResult is set to:
        ///     Result.deny if (pdpDecision != XacmlResult.permit AND there are no obligations to be fulfilled)
        ///     Result.denyWithObligations if (pdpDecision != XacmlResult.permit AND there is at least one obligation to be fulfilled)
        ///     Result.denyDueToUnrecognizedObligations if (pdpDecision == XacmlResult.permit AND there is at least one unrecognized obligation)
        ///     Result.denyUnlessAllObligationsSatisfied if (pdpDecision == XacmlResult.permit AND there is at least one obligation to be fulfilled)
        ///     Result.permit if (pdpDecision == XacmlResult.permit AND there are no obligations to be fulfilled)
        /// </summary>
        /// <param name="recognisedObligations"></param>
        internal void setDenyBiasedResult(List<string> recognisedObligations)
        {
            if (pdpDecision != XacmlResult.permit) {
                if (obligations.Count == 0) {
                    arDenyBiasedResult = Result.deny;
                }
                else if (obligations.Count > 0) {
                    List<Obligation> refinedListOfObligations = new List<Obligation>();
                    foreach (Obligation ob in obligations) {
                        if (recognisedObligations.Contains(ob.id)) {
                            refinedListOfObligations.Add(ob);
                        }
                    }
                    arDenyBiasedResult = Result.denyWithObligations;
                    obligations.Clear();
                    obligations.AddRange(refinedListOfObligations);
                }
            }
            else if (pdpDecision == XacmlResult.permit) {
                if (obligations.Count == 0) {
                    arDenyBiasedResult = Result.permit;
                }
                else if (obligations.Count > 0) {
                    bool unrecognisedObligationExist = false;
                    foreach (Obligation ob in obligations) {
                        if (!recognisedObligations.Contains(ob.id)) {
                            unrecognisedObligationExist = true;
                        }
                    }
                    if (unrecognisedObligationExist) {
                        arDenyBiasedResult = Result.denyDueToUnrecognizedObligations;
                        obligations.Clear();
                    }
                    else {
                        arDenyBiasedResult = Result.denyUnlessAllObligationsSatisfied;
                    }
                }
            }
        }
    }
}
