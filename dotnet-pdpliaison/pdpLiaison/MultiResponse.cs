using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Xml.Linq;

namespace PdpLiaison
{
    /// <summary>
    /// MultiResponse
    /// </summary>
    public class MultiResponse
    {
        private List<AuthorizationResponse> arResponses;

        /// <summary>
        /// Returns a list of AuthorizationResponse objects each corresponding with an AuthorizationRequest in the MultiRequest.
        /// </summary>
        public List<AuthorizationResponse> responses
        {
            get { return arResponses; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="responseNode">The response received from the PDP.</param>
        public MultiResponse(XmlNode responseNode)
        {
            this.arResponses = new List<AuthorizationResponse>();
            foreach (XmlNode node in responseNode.ChildNodes) {
                if (node.LocalName == "Result") {
                    arResponses.Add(new AuthorizationResponse(node));
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverJsonResponse">The response received from the PDP in the JSON format.</param>
        public MultiResponse(XElement serverJsonResponse)
        {
            this.arResponses = new List<AuthorizationResponse>();
            foreach (var responseElement in serverJsonResponse.Elements()) {
                arResponses.Add(new AuthorizationResponse(responseElement));
            }
        }

        /// <summary>
        /// Returns the AuthorizationResponse corresponding with the provided requestUID.
        /// </summary>
        /// <param name="requestUID">Provide the ID of the request the result of which is of interest.</param>
        /// <returns>The AuthorizationResponse corresponding with the provided requestUID.</returns>
        public AuthorizationResponse getResponseForRequest(string requestUID)
        {
            AuthorizationResponse result = new AuthorizationResponse();

            foreach (AuthorizationResponse res in arResponses) {
                if (res.requestId.ToString() == requestUID) {
                    return res;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the AuthorizationResponse corresponding with the provided request.
        /// </summary>
        /// <param name="request">Provide the request the result of which is of interest.</param>
        /// <returns>AuthorizationResponse corresponding with the provided requestUID.</returns>
        public AuthorizationResponse getResponseForRequest(AuthorizationRequest request)
        {
            return getResponseForRequest(request.uid.ToString());
        }

        /// <summary>
        /// Returns the AuthorizationResponse corresponding with the provided requestUID.
        /// </summary>
        /// <param name="requestUID">Provide the ID of the request the result of which is of interest.</param>
        /// <returns>The AuthorizationResponse corresponding with the provided requestUID.</returns>
        public AuthorizationResponse getResponseForRequest(Guid requestUID)
        {
            return getResponseForRequest(requestUID.ToString());
        }

        /// <summary>
        /// Returns a string representation of the authorization response
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string temp = "";
            int i = 0;

            //Adding the basic elements to the string
            foreach (AuthorizationResponse res in arResponses) {
                i++;
                temp += "-------------Result " + i.ToString() + "-------------";
                temp += "\r\nRequest ID: " + res.requestId;
                temp += res.ToString() + "\r\n";
            }

            return temp;
        }
    }
}
