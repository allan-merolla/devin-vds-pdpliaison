//! Core XACML types and enumerations.

use std::fmt;

/// The deny-biased result of an authorization request.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[repr(C)]
pub enum Result {
    /// PDP decision is not permit and there are no obligations.
    Deny = 0,
    /// PDP decision is not permit and there is at least one obligation.
    DenyWithObligations = 1,
    /// PDP decision is permit but there is at least one unrecognized obligation.
    DenyDueToUnrecognizedObligations = 2,
    /// PDP decision is permit and all obligations are recognized.
    DenyUnlessAllObligationsSatisfied = 3,
    /// PDP decision is permit and there are no obligations.
    Permit = 4,
}

impl fmt::Display for Result {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Result::Deny => write!(f, "deny"),
            Result::DenyWithObligations => write!(f, "denyWithObligations"),
            Result::DenyDueToUnrecognizedObligations => {
                write!(f, "denyDueToUnrecognizedObligations")
            }
            Result::DenyUnlessAllObligationsSatisfied => {
                write!(f, "denyUnlessAllObligationsSatisfied")
            }
            Result::Permit => write!(f, "permit"),
        }
    }
}

/// The raw XACML result from the PDP.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[repr(C)]
pub enum XacmlResult {
    Deny = 0,
    Indeterminate = 1,
    NotApplicable = 2,
    Permit = 3,
    NoResponse = 4,
}

impl XacmlResult {
    pub fn parse(s: &str) -> Self {
        match s.to_lowercase().as_str() {
            "deny" => XacmlResult::Deny,
            "indeterminate" => XacmlResult::Indeterminate,
            "notapplicable" => XacmlResult::NotApplicable,
            "permit" => XacmlResult::Permit,
            _ => XacmlResult::NoResponse,
        }
    }
}

/// XACML status codes.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[repr(C)]
pub enum XacmlStatus {
    MissingAttribute = 0,
    Ok = 1,
    ProcessingError = 2,
    SyntaxError = 3,
    NotDefined = 4,
}

impl XacmlStatus {
    pub fn from_code(code: &str) -> Self {
        match code {
            "urn:oasis:names:tc:xacml:1.0:status:missing-attribute" => XacmlStatus::MissingAttribute,
            "urn:oasis:names:tc:xacml:1.0:status:ok" => XacmlStatus::Ok,
            "urn:oasis:names:tc:xacml:1.0:status:processing-error" => XacmlStatus::ProcessingError,
            "urn:oasis:names:tc:xacml:1.0:status:syntax-error" => XacmlStatus::SyntaxError,
            _ => XacmlStatus::NotDefined,
        }
    }
}

/// Communication type between the client and the PDP.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[repr(C)]
pub enum CommunicationType {
    /// XML over SOAP
    XmlSoap = 0,
    /// XML over REST
    XmlRest = 1,
    /// JSON over REST
    JsonRest = 2,
}

/// Certificate inclusion mode for XML signing.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[repr(C)]
pub enum CertificateInclusion {
    /// Add only the signing certificate to the signature.
    SigningCertOnly = 0,
    /// Add all certificates in the certificate chain to the signature.
    CertificateChain = 1,
}

/// XACML attribute data type enumeration.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[repr(C)]
pub enum AttributeDataType {
    X500Name = 0,
    Rfc822Name = 1,
    IpAddress = 2,
    DnsName = 3,
    String = 4,
    Boolean = 5,
    Integer = 6,
    Double = 7,
    Time = 8,
    Date = 9,
    DateTime = 10,
    AnyUri = 11,
    HexBinary = 12,
    Base64Binary = 13,
    DayTimeDuration = 14,
    YearMonthDuration = 15,
    AnyType = 16,
}

/// Policy ID reference from the PDP response.
#[derive(Debug, Clone)]
pub struct PolicyIdReference {
    pub version: String,
    pub uuid: String,
}

/// Attribute assignment in an obligation or advice.
#[derive(Debug, Clone)]
pub struct AttributeAssignment {
    pub attribute_id: String,
    pub category_id: String,
    pub data_type: String,
    pub value: String,
}

impl fmt::Display for AttributeAssignment {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "[dataType]{} [category]{} [attributeID]{} = \n{}",
            self.data_type, self.category_id, self.attribute_id, self.value
        )
    }
}

/// Base type for XACML Obligation and Advice.
#[derive(Debug, Clone)]
pub struct ObligationAdvice {
    pub id: String,
    pub attributes: Vec<AttributeAssignment>,
}

impl fmt::Display for ObligationAdvice {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "\n{}: ", self.id)?;
        for aa in &self.attributes {
            write!(f, "\n{}", aa)?;
        }
        writeln!(f)
    }
}

/// XACML Obligation.
pub type Obligation = ObligationAdvice;

/// XACML Advice.
pub type Advice = ObligationAdvice;
