//! XACML standard constants: namespace URIs, attribute categories, data types, and attribute identifiers.

// ── Namespace URIs ──

pub const SOAP_1_1: &str = "http://schemas.xmlsoap.org/soap/envelope/";
pub const SOAP_1_2: &str = "http://www.w3.org/2003/05/soap-envelope";
pub const SAML_PROTOCOL: &str = "urn:oasis:names:tc:SAML:2.0:protocol";
pub const SAML_ASSERTION: &str = "urn:oasis:names:tc:SAML:2.0:assertion";
pub const XACML_SCHEMA: &str = "urn:oasis:names:tc:xacml:3.0:core:schema:wd-17";
pub const XACML_SAML_SCHEMA: &str =
    "urn:oasis:names:tc:xacml:3.0:profile:saml2.0:v2:schema:protocol:wd-14";
pub const DIGITAL_SIGNATURE: &str = "http://www.w3.org/2000/09/xmldsig#";

// ── Attribute Categories ──

pub mod attribute_category {
    pub const RESOURCE: &str = "urn:oasis:names:tc:xacml:3.0:attribute-category:resource";
    pub const ACTION: &str = "urn:oasis:names:tc:xacml:3.0:attribute-category:action";
    pub const ENVIRONMENT: &str = "urn:oasis:names:tc:xacml:3.0:attribute-category:environment";
    pub const ACCESS_SUBJECT: &str = "urn:oasis:names:tc:xacml:1.0:subject-category:access-subject";
    pub const RECIPIENT_SUBJECT: &str =
        "urn:oasis:names:tc:xacml:1.0:subject-category:recipient-subject";
    pub const INTERMEDIARY_SUBJECT: &str =
        "urn:oasis:names:tc:xacml:1.0:subject-category:intermediary-subject";
    pub const CODEBASE: &str = "urn:oasis:names:tc:xacml:1.0:subject-category:codebase";
    pub const REQUESTING_MACHINE: &str =
        "urn:oasis:names:tc:xacml:1.0:subject-category:requesting-machine";
    pub const ROLE_ENABLEMENT_AUTHORITY: &str =
        "urn:oasis:names:tc:xacml:2.0:subject-category:role-enablement-authority";
    pub const DELEGATE: &str = "urn:oasis:names:tc:xacml:3.0:attribute-category:delegate";
    pub const DELEGATION_INFO: &str =
        "urn:oasis:names:tc:xacml:3.0:attribute-category:delegation-info";
}

// ── Data Types ──

pub mod data_types {
    pub const X500_NAME: &str = "urn:oasis:names:tc:xacml:1.0:data-type:x500Name";
    pub const RFC822_NAME: &str = "urn:oasis:names:tc:xacml:1.0:data-type:rfc822Name";
    pub const IP_ADDRESS: &str = "urn:oasis:names:tc:xacml:2.0:data-type:ipAddress";
    pub const DNS_NAME: &str = "urn:oasis:names:tc:xacml:2.0:data-type:dnsName";
    pub const STRING: &str = "http://www.w3.org/2001/XMLSchema#string";
    pub const BOOLEAN: &str = "http://www.w3.org/2001/XMLSchema#boolean";
    pub const INTEGER: &str = "http://www.w3.org/2001/XMLSchema#integer";
    pub const DOUBLE: &str = "http://www.w3.org/2001/XMLSchema#double";
    pub const TIME: &str = "http://www.w3.org/2001/XMLSchema#time";
    pub const DATE: &str = "http://www.w3.org/2001/XMLSchema#date";
    pub const DATE_TIME: &str = "http://www.w3.org/2001/XMLSchema#dateTime";
    pub const ANY_URI: &str = "http://www.w3.org/2001/XMLSchema#anyURI";
    pub const HEX_BINARY: &str = "http://www.w3.org/2001/XMLSchema#hexBinary";
    pub const BASE64_BINARY: &str = "http://www.w3.org/2001/XMLSchema#base64Binary";
    pub const DAY_TIME_DURATION: &str = "http://www.w3.org/2001/XMLSchema#dayTimeDuration";
    pub const YEAR_MONTH_DURATION: &str = "http://www.w3.org/2001/XMLSchema#yearMonthDuration";
    pub const ANY_TYPE: &str = "http://www.w3.org/2001/XMLSchema#anyType";
}

// ── Subject Attributes ──

pub mod subject_attributes {
    pub const SUBJECT_ID: &str = "urn:oasis:names:tc:xacml:1.0:subject:subject-id";
    pub const SUBJECT_ID_QUALIFIER: &str =
        "urn:oasis:names:tc:xacml:1.0:subject:subject-id-qualifier";
    pub const KEY_INFO: &str = "urn:oasis:names:tc:xacml:1.0:subject:key-info";
    pub const AUTHENTICATION_TIME: &str =
        "urn:oasis:names:tc:xacml:1.0:subject:authentication-time";
    pub const AUTHENTICATION_METHOD: &str =
        "urn:oasis:names:tc:xacml:1.0:subject:authentication-method";
    pub const REQUEST_TIME: &str = "urn:oasis:names:tc:xacml:1.0:subject:request-time";
    pub const SESSION_START_TIME: &str =
        "urn:oasis:names:tc:xacml:1.0:subject:session-start-time";
    pub const IP_ADDRESS: &str =
        "urn:oasis:names:tc:xacml:1.0:subject:authn-locality:ip-address";
    pub const DNS_NAME: &str = "urn:oasis:names:tc:xacml:1.0:subject:authn-locality:dns-name";
    pub const ROLE: &str = "urn:oasis:names:tc:xacml:2.0:subject:role";
}

// ── Resource Attributes ──

pub mod resource_attributes {
    pub const RESOURCE_ID: &str = "urn:oasis:names:tc:xacml:1.0:resource:resource-id";
    pub const TARGET_NAMESPACE: &str = "urn:oasis:names:tc:xacml:2.0:resource:target-namespace";
    pub const XPATH: &str = "urn:oasis:names:tc:xacml:1.0:resource:xpath";
}

// ── Action Attributes ──

pub mod action_attributes {
    pub const ACTION_ID: &str = "urn:oasis:names:tc:xacml:1.0:action:action-id";
    pub const IMPLIED_ACTION: &str = "urn:oasis:names:tc:xacml:1.0:action:implied-action";
    pub const ACTION_NAMESPACE: &str = "urn:oasis:names:tc:xacml:1.0:action:action-namespace";
}

// ── Environment Attributes ──

pub mod environment_attributes {
    pub const CURRENT_TIME: &str = "urn:oasis:names:tc:xacml:1.0:environment:current-time";
    pub const CURRENT_DATE: &str = "urn:oasis:names:tc:xacml:1.0:environment:current-date";
    pub const CURRENT_DATE_TIME: &str =
        "urn:oasis:names:tc:xacml:1.0:environment:current-dateTime";
    pub(crate) const VIEWDS_TRACE: &str = "http://viewds.com/xacml/environment/trace";
    pub(crate) const VIEWDS_REQUEST_ID: &str = "http://viewds.com/xacml/environment/request-id";
}

// ── Delegation Info Attributes ──

pub mod delegation_info_attributes {
    pub const DECISION: &str = "urn:oasis:names:tc:xacml:3.0:delegation:decision";
}

/// Convert an `AttributeDataType` enum to its full XACML data type URI string.
pub fn data_type_id(dt: crate::types::AttributeDataType) -> &'static str {
    use crate::types::AttributeDataType;
    match dt {
        AttributeDataType::X500Name => data_types::X500_NAME,
        AttributeDataType::Rfc822Name => data_types::RFC822_NAME,
        AttributeDataType::IpAddress => data_types::IP_ADDRESS,
        AttributeDataType::DnsName => data_types::DNS_NAME,
        AttributeDataType::String => data_types::STRING,
        AttributeDataType::Boolean => data_types::BOOLEAN,
        AttributeDataType::Integer => data_types::INTEGER,
        AttributeDataType::Double => data_types::DOUBLE,
        AttributeDataType::Time => data_types::TIME,
        AttributeDataType::Date => data_types::DATE,
        AttributeDataType::DateTime => data_types::DATE_TIME,
        AttributeDataType::AnyUri => data_types::ANY_URI,
        AttributeDataType::HexBinary => data_types::HEX_BINARY,
        AttributeDataType::Base64Binary => data_types::BASE64_BINARY,
        AttributeDataType::DayTimeDuration => data_types::DAY_TIME_DURATION,
        AttributeDataType::YearMonthDuration => data_types::YEAR_MONTH_DURATION,
        AttributeDataType::AnyType => data_types::ANY_TYPE,
    }
}

/// Convert a full data type URI to its JSON shorthand.
pub fn data_type_shorthand(data_type: &str) -> &str {
    match data_type {
        data_types::STRING => "string",
        data_types::BOOLEAN => "boolean",
        data_types::INTEGER => "integer",
        data_types::DOUBLE => "double",
        data_types::TIME => "time",
        data_types::DATE => "date",
        data_types::DATE_TIME => "dateTime",
        data_types::DAY_TIME_DURATION => "dayTimeDuration",
        data_types::YEAR_MONTH_DURATION => "yearMonthDuration",
        data_types::ANY_URI => "anyURI",
        data_types::HEX_BINARY => "hexBinary",
        data_types::BASE64_BINARY => "base64Binary",
        data_types::RFC822_NAME => "rfc822Name",
        data_types::X500_NAME => "x500Name",
        data_types::IP_ADDRESS => "ipAddress",
        data_types::DNS_NAME => "dnsName",
        other => other,
    }
}

/// Convert a category URI to its JSON shorthand.
pub fn category_shorthand(category: &str) -> &str {
    match category {
        attribute_category::ACCESS_SUBJECT => "AccessSubject",
        attribute_category::RESOURCE => "Resource",
        attribute_category::ACTION => "Action",
        attribute_category::ENVIRONMENT => "Environment",
        attribute_category::RECIPIENT_SUBJECT => "RecipientSubject",
        attribute_category::INTERMEDIARY_SUBJECT => "IntermediarySubject",
        attribute_category::CODEBASE => "Codebase",
        attribute_category::REQUESTING_MACHINE => "RequestingMachine",
        other => other,
    }
}
