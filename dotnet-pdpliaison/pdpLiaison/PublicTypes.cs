using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdpLiaison
{

    /// <summary>
    /// Xacml Status
    /// </summary>
    public enum XacmlStatus
    {
        /// <summary>
        /// An attribute is missing
        /// </summary>
        missingAttribute,
        /// <summary>
        /// The decision has been made 
        /// </summary>
        ok,
        /// <summary>
        /// An error occurs during processing the request
        /// </summary>
        processingError,
        /// <summary>
        /// An error was found in the request syntax
        /// </summary>
        syntaxError,
        /// <summary>
        /// not defined status
        /// </summary>
        notDefined
    }

    /// <summary>
    /// Xacml Result
    /// </summary>
    internal enum XacmlResult
    {
        /// <summary>
        /// deny
        /// </summary>
        deny,
        /// <summary>
        /// indeterminate 
        /// </summary>
        indeterminate,
        /// <summary>
        /// notApplicable
        /// </summary>
        notApplicable,
        /// <summary>
        /// permit
        /// </summary>
        permit,
        /// <summary>
        /// noResponse
        /// </summary>
        noResponse
    }

    /// <summary>
    /// The result of the authorization request
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// The result field in the AuthorizationResponse would be 'deny' if 
        /// the XACML result returned from the PDP response is not 'permit' and there is  
        /// no obligation to be fulfilled.
        /// </summary>
        deny,
        /// <summary>
        /// The result field in the AuthorizationResponse would be 'DenyWithObligations' if 
        /// the XACML result returned from the PDP response is not 'permit' and there is at least 
        /// one obligation to be fulfilled.
        /// </summary>
        denyWithObligations,
        /// <summary>
        /// The result field in the AuthorizationResponse would be 'denyDueToUnrecognizedObligations' if 
        /// the XACML result returned from the PDP response is 'permit' and there is at least 
        /// one unrecognised obligation.
        /// </summary>
        denyDueToUnrecognizedObligations,
        /// <summary>
        /// The result field in the AuthorizationResponse would be 'DenyUnlessAllObligationsSatisfied' if 
        /// the XACML result returned from the PDP response is 'permit' and there is  a non-empty list of 
        /// obligations all of which are recognised.
        /// </summary>
        denyUnlessAllObligationsSatisfied,
        /// <summary>
        /// The result field in the AuthorizationResponse would be 'permit' if 
        /// the XACML result returned from the PDP response is 'permit' and there
        /// is no obligations to be fulfilled.
        /// </summary>
        permit
    }

    /// <summary>
    /// Defines different ways of providing the certificates in the signature.
    /// </summary>
    public enum CertificateInclusion
    {
        /// <summary>
        /// Add only the signing certificate to the signature.
        /// </summary>
        signingCertOnly,
        /// <summary>
        /// Add all certificates in the certificate chain to the signature.
        /// </summary>
        certificateChain
    }

    /// <summary>
    /// Attribute Category
    /// </summary>
    public static class AttributeCategory
    {
        /// <summary>
        /// urn:oasis:names:tc:xacml:3.0:attribute-category:resource
        /// </summary>
        public const string resource = "urn:oasis:names:tc:xacml:3.0:attribute-category:resource";
        /// <summary>
        /// urn:oasis:names:tc:xacml:3.0:attribute-category:action
        /// </summary>
        public const string action = "urn:oasis:names:tc:xacml:3.0:attribute-category:action";
        /// <summary>
        /// urn:oasis:names:tc:xacml:3.0:attribute-category:environment
        /// </summary>
        public const string environment = "urn:oasis:names:tc:xacml:3.0:attribute-category:environment";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject-category:access-subject
        /// </summary>
        public const string access_subject = "urn:oasis:names:tc:xacml:1.0:subject-category:access-subject";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject-category:recipient-subject
        /// </summary>
        public const string recipient_subject = "urn:oasis:names:tc:xacml:1.0:subject-category:recipient-subject";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject-category:intermediary-subject
        /// </summary>
        public const string intermediary_subject = "urn:oasis:names:tc:xacml:1.0:subject-category:intermediary-subject";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject-category:codebase
        /// </summary>
        public const string codebase = "urn:oasis:names:tc:xacml:1.0:subject-category:codebase";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject-category:requesting-machine
        /// </summary>
        public const string requesting_machine = "urn:oasis:names:tc:xacml:1.0:subject-category:requesting-machine";
        /// <summary>
        /// urn:oasis:names:tc:xacml:2.0:subject-category:role-enablement-authority
        /// </summary>
        public const string role_enablement_authority = "urn:oasis:names:tc:xacml:2.0:subject-category:role-enablement-authority";
        /// <summary>
        /// urn:oasis:names:tc:xacml:3.0:attribute-category:delegate
        /// </summary>
        public const string delegate_category = "urn:oasis:names:tc:xacml:3.0:attribute-category:delegate";
        /// <summary>
        /// urn:oasis:names:tc:xacml:3.0:attribute-category:delegation-info
        /// </summary>
        public const string delegation_info = "urn:oasis:names:tc:xacml:3.0:attribute-category:delegation-info";
    }

    /// <summary>
    /// Attribute DataType
    /// </summary>
    public enum AttributeDataType
    {
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:data-type:x500Name
        /// </summary>
        x500Name,
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:data-type:rfc822Name
        /// </summary>
        rfc822Name,
        /// <summary>
        /// urn:oasis:names:tc:xacml:2.0:data-type:ipAddress
        /// </summary>
        ipAddress,
        /// <summary>
        /// urn:oasis:names:tc:xacml:2.0:data-type:dnsName
        /// </summary>
        dnsName,
        ///// <summary>
        ///// urn:oasis:names:tc:xacml:3.0:data-type:xpathExpression
        ///// </summary>
        //xpathExpression,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#string
        /// </summary>
        _string,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#boolean
        /// </summary>
        boolean,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#integer
        /// </summary>
        integer,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#double
        /// </summary>
        _double,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#time
        /// </summary>
        time,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#date
        /// </summary>
        date,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#dateTime
        /// </summary>
        dateTime,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#anyURI
        /// </summary>
        anyURI,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#hexBinary
        /// </summary>
        hexBinary,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#base64Binary
        /// </summary>
        base64Binary,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#dayTimeDuration
        /// </summary>
        dayTimeDuration,
        /// <summary>
        /// http://www.w3.org/2001/XMLSchema#yearMonthDuration
        /// </summary>
        yearMonthDuration,
        /// <summary>
        /// "http://www.w3.org/2001/XMLSchema#anyType"
        /// </summary>
        anyType
    }

    internal static class DataTypes
    {
        internal const string x500Name = "urn:oasis:names:tc:xacml:1.0:data-type:x500Name";
        internal const string rfc822Name = "urn:oasis:names:tc:xacml:1.0:data-type:rfc822Name";
        internal const string ipAddress = "urn:oasis:names:tc:xacml:2.0:data-type:ipAddress";
        internal const string dnsName = "urn:oasis:names:tc:xacml:2.0:data-type:dnsName";
        //internal const string xpathExpression = "urn:oasis:names:tc:xacml:3.0:data-type:xpathExpression";
        internal const string _string = "http://www.w3.org/2001/XMLSchema#string";
        internal const string boolean = "http://www.w3.org/2001/XMLSchema#boolean";
        internal const string integer = "http://www.w3.org/2001/XMLSchema#integer";
        internal const string _double = "http://www.w3.org/2001/XMLSchema#double";
        internal const string time = "http://www.w3.org/2001/XMLSchema#time";
        internal const string date = "http://www.w3.org/2001/XMLSchema#date";
        internal const string dateTime = "http://www.w3.org/2001/XMLSchema#dateTime";
        internal const string anyURI = "http://www.w3.org/2001/XMLSchema#anyURI";
        internal const string hexBinary = "http://www.w3.org/2001/XMLSchema#hexBinary";
        internal const string base64Binary = "http://www.w3.org/2001/XMLSchema#base64Binary";
        internal const string dayTimeDuration = "http://www.w3.org/2001/XMLSchema#dayTimeDuration";
        internal const string yearMonthDuration = "http://www.w3.org/2001/XMLSchema#yearMonthDuration";
        internal const string anyType = "http://www.w3.org/2001/XMLSchema#anyType";
    }

    /// <summary>
    /// Subject Attributes
    /// </summary>
    public static class SubjectAttributes
    {
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:subject-id
        /// </summary>
        public const string subject_id = "urn:oasis:names:tc:xacml:1.0:subject:subject-id";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:subject-id-qualifier
        /// </summary>
        public const string subject_id_qualifier = "urn:oasis:names:tc:xacml:1.0:subject:subject-id-qualifier";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:key-info
        /// </summary>
        public const string key_info = "urn:oasis:names:tc:xacml:1.0:subject:key-info";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:authentication-time
        /// </summary>
        public const string authentication_time = "urn:oasis:names:tc:xacml:1.0:subject:authentication-time";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:authentication-method
        /// </summary>
        public const string authentication_method = "urn:oasis:names:tc:xacml:1.0:subject:authentication-method";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:request-time
        /// </summary>
        public const string request_time = "urn:oasis:names:tc:xacml:1.0:subject:request-time";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:session-start-time
        /// </summary>
        public const string session_start_time = "urn:oasis:names:tc:xacml:1.0:subject:session-start-time";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:authn-locality:ip-address
        /// </summary>
        public const string ip_address = "urn:oasis:names:tc:xacml:1.0:subject:authn-locality:ip-address";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:subject:authn-locality:dns-name
        /// </summary>
        public const string dns_name = "urn:oasis:names:tc:xacml:1.0:subject:authn-locality:dns-name";
        /// <summary>
        /// urn:oasis:names:tc:xacml:2.0:subject:role
        /// </summary>
        public const string role = "urn:oasis:names:tc:xacml:2.0:subject:role";
    }

    /// <summary>
    /// Resource Attributes
    /// </summary>
    public static class ResourceAttributes
    {
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:resource:resource-id
        /// </summary>
        public const string resource_id = "urn:oasis:names:tc:xacml:1.0:resource:resource-id";
        /// <summary>
        /// urn:oasis:names:tc:xacml:2.0:resource:target-namespace
        /// </summary>
        public const string target_namespace = "urn:oasis:names:tc:xacml:2.0:resource:target-namespace";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:resource:xpath
        /// </summary>
        public const string xpath = "urn:oasis:names:tc:xacml:1.0:resource:xpath";
    }

    /// <summary>
    /// Action Attributes
    /// </summary>
    public static class ActionAttributes
    {
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:action:action-id
        /// </summary>
        public const string action_id = "urn:oasis:names:tc:xacml:1.0:action:action-id";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:action:implied-action
        /// </summary>
        public const string implied_action = "urn:oasis:names:tc:xacml:1.0:action:implied-action";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:action:action-namespace
        /// </summary>
        public const string action_namespace = "urn:oasis:names:tc:xacml:1.0:action:action-namespace";
    }

    /// <summary>
    /// Environment Attributes
    /// </summary>
    public static class EnvironmentAttributes
    {
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:environment:current-time
        /// </summary>
        public const string current_time = "urn:oasis:names:tc:xacml:1.0:environment:current-time";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:environment:current-date
        /// </summary>
        public const string current_date = "urn:oasis:names:tc:xacml:1.0:environment:current-date";
        /// <summary>
        /// urn:oasis:names:tc:xacml:1.0:environment:current-dateTime
        /// </summary>
        public const string current_dateTime = "urn:oasis:names:tc:xacml:1.0:environment:current-dateTime";
        /// <summary>
        /// "http://viewds.com/xacml/environment/trace"
        /// </summary>
        internal const string viewds_trace = "http://viewds.com/xacml/environment/trace";
        /// <summary>
        /// "http://viewds.com/xacml/environment/request-id"
        /// </summary>
        internal const string viewds_request_id = "http://viewds.com/xacml/environment/request-id";
    }

    /// <summary>
    /// XML Namespace URIs
    /// </summary>
    public static class namespaceURI
    {
        /// <summary>
        /// http://schemas.xmlsoap.org/soap/envelope/
        /// </summary>
        public const string soap_1_1 = "http://schemas.xmlsoap.org/soap/envelope/";
        /// <summary>
        /// http://www.w3.org/2003/05/soap-envelope
        /// </summary>
        public const string soap_1_2 = "http://www.w3.org/2003/05/soap-envelope";
        /// <summary>
        /// urn:oasis:names:tc:SAML:2.0:protocol
        /// </summary>
        public const string saml_protocol = "urn:oasis:names:tc:SAML:2.0:protocol";
        /// <summary>
        /// urn:oasis:names:tc:SAML:2.0:assertion
        /// </summary>
        public const string saml_assertion = "urn:oasis:names:tc:SAML:2.0:assertion";
        /// <summary>
        /// urn:oasis:names:tc:xacml:3.0:core:schema:wd-17
        /// </summary>
        public const string xacml_schema = "urn:oasis:names:tc:xacml:3.0:core:schema:wd-17";
        /// <summary>
        /// "urn:oasis:names:tc:xacml:3.0:profile:saml2.0:v2:schema:protocol:wd-14"
        /// </summary>
        public const string xacml_saml_schema = "urn:oasis:names:tc:xacml:3.0:profile:saml2.0:v2:schema:protocol:wd-14";
        /// <summary>
        /// http://www.w3.org/2000/09/xmldsig#
        /// </summary>
        public const string digital_signature = "http://www.w3.org/2000/09/xmldsig#";
    }

    /// <summary>
    /// Delegation Info Attributes
    /// </summary>
    public static class DelegationInfoAttributes
    {
        /// <summary>
        /// urn:oasis:names:tc:xacml:3.0:delegation:decision
        /// </summary>
        public const string decision = "urn:oasis:names:tc:xacml:3.0:delegation:decision";
    }

    /// <summary>
    /// Policy ID Reference 
    /// </summary>
    public class PolicyIdReference
    {
        /// <summary>
        /// The version of the policy ID reference
        /// </summary>
        public string version;
        /// <summary>
        /// The UUID of the policy ID reference
        /// </summary>
        public string uuid;
    }

    /// <summary>
    /// Different types of communication between the AIK and the PDP
    /// </summary>
    public enum CommunicationType
    {
        /// <summary>
        /// XML over SOAP
        /// </summary>
        XML_SOAP,
        /// <summary>
        /// XML over REST
        /// </summary>
        XML_REST,
        /// <summary>
        /// JSON over REST
        /// </summary>
        JSON_REST
    }

    /*
    /// <summary>
    /// Each NamespaceDeclaration object describes a single XML namespace declaration.
    /// </summary>
    public class NamespaceDeclaration
    {
        /// <summary>
        /// Contains the namespace prefix.
        /// </summary>
        public string prefix;
        /// <summary>
        /// Contains the namespace name.
        /// </summary>
        public Uri _namespace;
    }

    /// <summary>
    /// Defines objects of the xpathExpression data-type in JSON 
    /// </summary>
    public class XpathExpression
    {
        /// <summary>
        /// Contains the XPath category.
        /// </summary>
        public Uri xpathCategory;
        /// <summary>
        /// Contains namespace declarations for interpreting qualified names in the XPath expression.
        /// </summary>
        public List<NamespaceDeclaration> namespaces;
        /// <summary>
        /// Contains the XPath expression.
        /// </summary>
        public string xpath;
    }
    */
}
