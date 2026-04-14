//! Integration tests for the PDP Liaison library.

use pdp_liaison::constants::*;
use pdp_liaison::json_builder;
use pdp_liaison::request::*;
use pdp_liaison::response::*;
use pdp_liaison::types::*;
use pdp_liaison::xml_builder;

// ══════════════════════════════════════════════════════
// ── Types tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_result_display() {
    assert_eq!(format!("{}", Result::Deny), "deny");
    assert_eq!(format!("{}", Result::Permit), "permit");
    assert_eq!(
        format!("{}", Result::DenyWithObligations),
        "denyWithObligations"
    );
    assert_eq!(
        format!("{}", Result::DenyDueToUnrecognizedObligations),
        "denyDueToUnrecognizedObligations"
    );
    assert_eq!(
        format!("{}", Result::DenyUnlessAllObligationsSatisfied),
        "denyUnlessAllObligationsSatisfied"
    );
}

#[test]
fn test_xacml_result_from_str() {
    assert_eq!(XacmlResult::parse("Permit"), XacmlResult::Permit);
    assert_eq!(XacmlResult::parse("permit"), XacmlResult::Permit);
    assert_eq!(XacmlResult::parse("Deny"), XacmlResult::Deny);
    assert_eq!(
        XacmlResult::parse("Indeterminate"),
        XacmlResult::Indeterminate
    );
    assert_eq!(
        XacmlResult::parse("NotApplicable"),
        XacmlResult::NotApplicable
    );
    assert_eq!(
        XacmlResult::parse("garbage"),
        XacmlResult::NoResponse
    );
}

#[test]
fn test_xacml_status_from_code() {
    assert_eq!(
        XacmlStatus::from_code("urn:oasis:names:tc:xacml:1.0:status:ok"),
        XacmlStatus::Ok
    );
    assert_eq!(
        XacmlStatus::from_code("urn:oasis:names:tc:xacml:1.0:status:missing-attribute"),
        XacmlStatus::MissingAttribute
    );
    assert_eq!(
        XacmlStatus::from_code("urn:oasis:names:tc:xacml:1.0:status:processing-error"),
        XacmlStatus::ProcessingError
    );
    assert_eq!(
        XacmlStatus::from_code("urn:oasis:names:tc:xacml:1.0:status:syntax-error"),
        XacmlStatus::SyntaxError
    );
    assert_eq!(
        XacmlStatus::from_code("unknown"),
        XacmlStatus::NotDefined
    );
}

#[test]
fn test_data_type_id_mapping() {
    assert_eq!(
        data_type_id(AttributeDataType::String),
        data_types::STRING
    );
    assert_eq!(
        data_type_id(AttributeDataType::Boolean),
        data_types::BOOLEAN
    );
    assert_eq!(
        data_type_id(AttributeDataType::Integer),
        data_types::INTEGER
    );
    assert_eq!(
        data_type_id(AttributeDataType::AnyUri),
        data_types::ANY_URI
    );
    assert_eq!(
        data_type_id(AttributeDataType::X500Name),
        data_types::X500_NAME
    );
}

#[test]
fn test_data_type_shorthand() {
    assert_eq!(data_type_shorthand(data_types::STRING), "string");
    assert_eq!(data_type_shorthand(data_types::BOOLEAN), "boolean");
    assert_eq!(data_type_shorthand(data_types::INTEGER), "integer");
    assert_eq!(data_type_shorthand(data_types::ANY_URI), "anyURI");
    assert_eq!(
        data_type_shorthand("http://custom/type"),
        "http://custom/type"
    );
}

#[test]
fn test_category_shorthand() {
    assert_eq!(
        category_shorthand(attribute_category::ACCESS_SUBJECT),
        "AccessSubject"
    );
    assert_eq!(
        category_shorthand(attribute_category::RESOURCE),
        "Resource"
    );
    assert_eq!(
        category_shorthand(attribute_category::ACTION),
        "Action"
    );
    assert_eq!(
        category_shorthand(attribute_category::ENVIRONMENT),
        "Environment"
    );
    assert_eq!(
        category_shorthand("urn:custom:category"),
        "urn:custom:category"
    );
}

// ══════════════════════════════════════════════════════
// ── Request tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_authorization_request_creation() {
    let req = AuthorizationRequest::new();
    assert!(!req.trace);
    assert!(!req.return_policy_id_list);
    assert!(req.combine_policies);
    assert!(req.categories.is_empty());
}

#[test]
fn test_authorization_request_with_trace() {
    let req = AuthorizationRequest::with_trace(true);
    assert!(req.trace);
    // Trace adds an environment category
    assert!(!req.categories.is_empty());
    assert_eq!(
        req.categories[0].category_id,
        attribute_category::ENVIRONMENT
    );
}

#[test]
fn test_authorization_request_with_options() {
    let req = AuthorizationRequest::with_all_options(true, true, false);
    assert!(req.trace);
    assert!(req.return_policy_id_list);
    assert!(!req.combine_policies);
}

#[test]
fn test_add_element() {
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "user123",
    );

    assert_eq!(req.categories.len(), 1);
    assert_eq!(
        req.categories[0].category_id,
        attribute_category::ACCESS_SUBJECT
    );
    assert_eq!(req.categories[0].attributes.len(), 1);
    assert_eq!(
        req.categories[0].attributes[0].attribute_id,
        subject_attributes::SUBJECT_ID
    );
    assert_eq!(req.categories[0].attributes[0].values.len(), 1);
    assert_eq!(req.categories[0].attributes[0].values[0].value, "user123");
}

#[test]
fn test_add_element_typed() {
    let mut req = AuthorizationRequest::new();
    req.add_element_typed(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        AttributeDataType::String,
        "user123",
    );

    assert_eq!(req.categories[0].attributes[0].values[0].data_type, data_types::STRING);
}

#[test]
fn test_add_multiple_elements_same_category() {
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "user123",
    );
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::ROLE,
        data_types::STRING,
        "admin",
    );

    // Should be in same category
    assert_eq!(req.categories.len(), 1);
    assert_eq!(req.categories[0].attributes.len(), 2);
}

#[test]
fn test_add_multiple_values_same_attribute() {
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::ROLE,
        data_types::STRING,
        "admin",
    );
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::ROLE,
        data_types::STRING,
        "manager",
    );

    assert_eq!(req.categories.len(), 1);
    assert_eq!(req.categories[0].attributes.len(), 1);
    assert_eq!(req.categories[0].attributes[0].values.len(), 2);
}

#[test]
fn test_duplicate_values_not_added() {
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::ROLE,
        data_types::STRING,
        "admin",
    );
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::ROLE,
        data_types::STRING,
        "admin",
    );

    assert_eq!(req.categories[0].attributes[0].values.len(), 1);
}

#[test]
fn test_set_content() {
    let mut req = AuthorizationRequest::new();
    req.set_content(
        attribute_category::RESOURCE,
        "<xml>content</xml>",
    );

    assert_eq!(req.categories.len(), 1);
    assert_eq!(
        req.categories[0].xml_content,
        Some("<xml>content</xml>".to_string())
    );
}

#[test]
fn test_xacml_category_structural_equality() {
    let mut cat1 = XacmlCategory::new(attribute_category::RESOURCE);
    cat1.add_attribute("attr1", data_types::STRING, "val1", false);

    let mut cat2 = XacmlCategory::new(attribute_category::RESOURCE);
    cat2.add_attribute("attr1", data_types::STRING, "val1", false);

    assert!(cat1.structurally_equal(&cat2));
}

#[test]
fn test_xacml_category_structural_inequality() {
    let mut cat1 = XacmlCategory::new(attribute_category::RESOURCE);
    cat1.add_attribute("attr1", data_types::STRING, "val1", false);

    let mut cat2 = XacmlCategory::new(attribute_category::RESOURCE);
    cat2.add_attribute("attr1", data_types::STRING, "val2", false);

    assert!(!cat1.structurally_equal(&cat2));
}

// ══════════════════════════════════════════════════════
// ── MultiRequest tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_multi_request_creation() {
    let mr = MultiRequest::new(true);
    assert!(mr.return_policy_id_list);
    assert!(mr.categories.is_empty());
    assert!(mr.requests.is_empty());
}

#[test]
fn test_multi_request_add_request() {
    let mut mr = MultiRequest::new(false);
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "user1",
    );

    mr.add_request(&mut req).unwrap();

    assert_eq!(mr.requests.len(), 1);
    assert!(!mr.categories.is_empty());
    assert_eq!(mr.references().len(), 1);
}

// ══════════════════════════════════════════════════════
// ── XML builder tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_build_xacml_request_xml() {
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "user123",
    );
    req.add_element(
        attribute_category::RESOURCE,
        resource_attributes::RESOURCE_ID,
        data_types::STRING,
        "document1",
    );
    req.add_element(
        attribute_category::ACTION,
        action_attributes::ACTION_ID,
        data_types::STRING,
        "read",
    );

    let xml = xml_builder::build_xacml_request_xml(&req);

    assert!(xml.contains("xmlns:ns=\"urn:oasis:names:tc:xacml:3.0:core:schema:wd-17\""));
    assert!(xml.contains("ReturnPolicyIdList=\"false\""));
    assert!(xml.contains("user123"));
    assert!(xml.contains("document1"));
    assert!(xml.contains("read"));
    assert!(xml.contains(subject_attributes::SUBJECT_ID));
    assert!(xml.contains(resource_attributes::RESOURCE_ID));
    assert!(xml.contains(action_attributes::ACTION_ID));
}

#[test]
fn test_wrap_saml() {
    let request_xml = "<ns:Request xmlns:ns=\"urn:oasis:names:tc:xacml:3.0:core:schema:wd-17\"/>";
    let saml = xml_builder::wrap_saml(request_xml, true, &[]);

    assert!(saml.contains("XACMLAuthzDecisionQuery"));
    assert!(saml.contains("CombinePolicies=\"true\""));
    assert!(saml.contains("IssueInstant="));
    assert!(saml.contains(request_xml));
}

#[test]
fn test_wrap_soap() {
    let message = "<inner>content</inner>";
    let soap = xml_builder::wrap_soap(message);

    assert!(soap.contains("SOAP-ENV:Envelope"));
    assert!(soap.contains("SOAP-ENV:Header"));
    assert!(soap.contains("SOAP-ENV:Body"));
    assert!(soap.contains(message));
}

#[test]
fn test_extract_saml_query_id() {
    let saml = "<xsp:XACMLAuthzDecisionQuery ID=\"_test-id-123\" Version=\"2.0\"/>";
    let qid = xml_builder::extract_saml_query_id(saml);
    assert_eq!(qid, Some("_test-id-123".to_string()));
}

// ══════════════════════════════════════════════════════
// ── JSON builder tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_build_json_request() {
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "user123",
    );
    req.add_element(
        attribute_category::RESOURCE,
        resource_attributes::RESOURCE_ID,
        data_types::STRING,
        "document1",
    );
    req.add_element(
        attribute_category::ACTION,
        action_attributes::ACTION_ID,
        data_types::STRING,
        "read",
    );

    let json = json_builder::build_json_request(&req, false);

    assert!(json.contains("\"Request\""));
    assert!(json.contains("\"Category\""));
    assert!(json.contains("\"user123\""));
    assert!(json.contains("\"document1\""));
    assert!(json.contains("\"read\""));
}

#[test]
fn test_build_json_request_indented() {
    let mut req = AuthorizationRequest::new();
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "testuser",
    );

    let json = json_builder::build_json_request(&req, true);
    assert!(json.contains("\"Request\""));
    assert!(json.contains("\"testuser\""));
}

#[test]
fn test_json_content_escaping_special_chars() {
    let mut req = AuthorizationRequest::new();
    req.set_content(
        attribute_category::RESOURCE,
        "<xml>\n\t\"special\" content with \\backslash\r\nand newlines</xml>",
    );

    let json = json_builder::build_json_request(&req, false);
    // Should NOT contain raw newlines/tabs inside the JSON string
    assert!(!json.contains("\n\t\"special\""));
    // Should contain properly escaped versions
    assert!(json.contains("\\n"));
    assert!(json.contains("\\t"));
    assert!(json.contains("\\\\backslash"));
    assert!(json.contains("\\\"special\\\""));
}

#[test]
fn test_json_request_return_policy_id_list() {
    let mut req = AuthorizationRequest::with_options(false, true);
    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "user",
    );

    let json = json_builder::build_json_request(&req, false);
    assert!(json.contains("\"ReturnPolicyIdList\": true"));
}

// ══════════════════════════════════════════════════════
// ── Response parsing tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_parse_xml_permit_response() {
    let xml = r#"<Result>
        <Decision>Permit</Decision>
        <Status>
            <StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/>
        </Status>
    </Result>"#;

    let resp = AuthorizationResponse::from_xml(xml).unwrap();
    assert_eq!(resp.pdp_decision(), XacmlResult::Permit);
    assert_eq!(resp.xacml_status(), XacmlStatus::Ok);
}

#[test]
fn test_parse_xml_deny_response() {
    let xml = r#"<Result>
        <Decision>Deny</Decision>
        <Status>
            <StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/>
        </Status>
    </Result>"#;

    let resp = AuthorizationResponse::from_xml(xml).unwrap();
    assert_eq!(resp.pdp_decision(), XacmlResult::Deny);
}

#[test]
fn test_parse_xml_response_with_obligations() {
    let xml = r#"<Result>
        <Decision>Permit</Decision>
        <Status>
            <StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/>
        </Status>
        <Obligations>
            <Obligation ObligationId="urn:example:obligation:log">
                <AttributeAssignment AttributeId="urn:example:attribute:text" DataType="http://www.w3.org/2001/XMLSchema#string" Category="urn:oasis:names:tc:xacml:3.0:attribute-category:resource">Log this access</AttributeAssignment>
            </Obligation>
        </Obligations>
    </Result>"#;

    let resp = AuthorizationResponse::from_xml(xml).unwrap();
    assert_eq!(resp.pdp_decision(), XacmlResult::Permit);
    assert_eq!(resp.obligations.len(), 1);
    assert_eq!(resp.obligations[0].id, "urn:example:obligation:log");
    assert_eq!(resp.obligations[0].attributes.len(), 1);
    assert_eq!(resp.obligations[0].attributes[0].value, "Log this access");
}

#[test]
fn test_parse_xml_response_with_advice() {
    let xml = r#"<Result>
        <Decision>Permit</Decision>
        <Status>
            <StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/>
        </Status>
        <AssociatedAdvice>
            <Advice AdviceId="urn:example:advice:info">
                <AttributeAssignment AttributeId="urn:example:attribute:message" DataType="http://www.w3.org/2001/XMLSchema#string">Informational message</AttributeAssignment>
            </Advice>
        </AssociatedAdvice>
    </Result>"#;

    let resp = AuthorizationResponse::from_xml(xml).unwrap();
    assert_eq!(resp.associated_advice.len(), 1);
    assert_eq!(resp.associated_advice[0].id, "urn:example:advice:info");
}

#[test]
fn test_parse_xml_response_with_status_message() {
    let xml = r#"<Result>
        <Decision>Indeterminate</Decision>
        <Status>
            <StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:processing-error"/>
            <StatusMessage>Internal PDP error</StatusMessage>
        </Status>
    </Result>"#;

    let resp = AuthorizationResponse::from_xml(xml).unwrap();
    assert_eq!(resp.pdp_decision(), XacmlResult::Indeterminate);
    assert_eq!(resp.xacml_status(), XacmlStatus::ProcessingError);
    assert_eq!(resp.xacml_status_message(), "Internal PDP error");
}

#[test]
fn test_deny_biased_result_permit_no_obligations() {
    let xml = r#"<Result>
        <Decision>Permit</Decision>
        <Status><StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/></Status>
    </Result>"#;

    let mut resp = AuthorizationResponse::from_xml(xml).unwrap();
    resp.set_deny_biased_result(&[]);
    assert_eq!(resp.result(), Result::Permit);
}

#[test]
fn test_deny_biased_result_deny_no_obligations() {
    let xml = r#"<Result>
        <Decision>Deny</Decision>
        <Status><StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/></Status>
    </Result>"#;

    let mut resp = AuthorizationResponse::from_xml(xml).unwrap();
    resp.set_deny_biased_result(&[]);
    assert_eq!(resp.result(), Result::Deny);
}

#[test]
fn test_deny_biased_result_permit_with_recognised_obligations() {
    let xml = r#"<Result>
        <Decision>Permit</Decision>
        <Status><StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/></Status>
        <Obligations>
            <Obligation ObligationId="urn:example:ob1">
                <AttributeAssignment AttributeId="a" DataType="http://www.w3.org/2001/XMLSchema#string">v</AttributeAssignment>
            </Obligation>
        </Obligations>
    </Result>"#;

    let mut resp = AuthorizationResponse::from_xml(xml).unwrap();
    resp.set_deny_biased_result(&["urn:example:ob1".to_string()]);
    assert_eq!(resp.result(), Result::DenyUnlessAllObligationsSatisfied);
}

#[test]
fn test_deny_biased_result_permit_with_unrecognised_obligations() {
    let xml = r#"<Result>
        <Decision>Permit</Decision>
        <Status><StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/></Status>
        <Obligations>
            <Obligation ObligationId="urn:example:ob-unknown">
                <AttributeAssignment AttributeId="a" DataType="http://www.w3.org/2001/XMLSchema#string">v</AttributeAssignment>
            </Obligation>
        </Obligations>
    </Result>"#;

    let mut resp = AuthorizationResponse::from_xml(xml).unwrap();
    resp.set_deny_biased_result(&[]);
    assert_eq!(resp.result(), Result::DenyDueToUnrecognizedObligations);
    assert!(resp.obligations.is_empty());
}

#[test]
fn test_deny_biased_result_deny_with_obligations() {
    let xml = r#"<Result>
        <Decision>Deny</Decision>
        <Status><StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/></Status>
        <Obligations>
            <Obligation ObligationId="urn:example:ob1">
                <AttributeAssignment AttributeId="a" DataType="http://www.w3.org/2001/XMLSchema#string">v</AttributeAssignment>
            </Obligation>
        </Obligations>
    </Result>"#;

    let mut resp = AuthorizationResponse::from_xml(xml).unwrap();
    resp.set_deny_biased_result(&["urn:example:ob1".to_string()]);
    assert_eq!(resp.result(), Result::DenyWithObligations);
}

// ── JSON response parsing ──

#[test]
fn test_parse_json_permit_response() {
    let json: serde_json::Value = serde_json::from_str(
        r#"{
            "Decision": "Permit",
            "Status": {
                "StatusCode": {
                    "Value": "urn:oasis:names:tc:xacml:1.0:status:ok"
                }
            }
        }"#,
    )
    .unwrap();

    let resp = AuthorizationResponse::from_json(&json).unwrap();
    assert_eq!(resp.pdp_decision(), XacmlResult::Permit);
    assert_eq!(resp.xacml_status(), XacmlStatus::Ok);
}

#[test]
fn test_parse_json_response_with_obligations() {
    let json: serde_json::Value = serde_json::from_str(
        r#"{
            "Decision": "Permit",
            "Status": {
                "StatusCode": {
                    "Value": "urn:oasis:names:tc:xacml:1.0:status:ok"
                }
            },
            "Obligations": [
                {
                    "Id": "urn:example:obligation:log",
                    "AttributeAssignment": [
                        {
                            "AttributeId": "urn:example:attr",
                            "Value": "log message",
                            "DataType": "string",
                            "Category": "urn:oasis:names:tc:xacml:3.0:attribute-category:resource"
                        }
                    ]
                }
            ]
        }"#,
    )
    .unwrap();

    let resp = AuthorizationResponse::from_json(&json).unwrap();
    assert_eq!(resp.obligations.len(), 1);
    assert_eq!(resp.obligations[0].id, "urn:example:obligation:log");
    assert_eq!(resp.obligations[0].attributes[0].value, "log message");
}

// ── MultiResponse tests ──

#[test]
fn test_multi_response_from_xml() {
    let xml = r#"<Response xmlns="urn:oasis:names:tc:xacml:3.0:core:schema:wd-17">
        <Result>
            <Decision>Permit</Decision>
            <Status><StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/></Status>
        </Result>
        <Result>
            <Decision>Deny</Decision>
            <Status><StatusCode Value="urn:oasis:names:tc:xacml:1.0:status:ok"/></Status>
        </Result>
    </Response>"#;

    let multi = MultiResponse::from_xml(xml).unwrap();
    assert_eq!(multi.responses.len(), 2);
    assert_eq!(multi.responses[0].pdp_decision(), XacmlResult::Permit);
    assert_eq!(multi.responses[1].pdp_decision(), XacmlResult::Deny);
}

#[test]
fn test_multi_response_from_json() {
    let json: serde_json::Value = serde_json::from_str(
        r#"[
            {
                "Decision": "Permit",
                "Status": {"StatusCode": {"Value": "urn:oasis:names:tc:xacml:1.0:status:ok"}}
            },
            {
                "Decision": "Deny",
                "Status": {"StatusCode": {"Value": "urn:oasis:names:tc:xacml:1.0:status:ok"}}
            }
        ]"#,
    )
    .unwrap();

    let multi = MultiResponse::from_json(&json).unwrap();
    assert_eq!(multi.responses.len(), 2);
}

// ══════════════════════════════════════════════════════
// ── Connector tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_connector_anonymous_creation() {
    use pdp_liaison::connector::*;

    let config = PdpConnectorConfig::anonymous(
        "http://localhost:8080/pdp",
        pdp_liaison::types::CommunicationType::XmlSoap,
    );
    let connector = PdpConnector::new(config);

    let req = connector.create_request();
    assert!(!req.trace);
}

#[test]
fn test_connector_client_ssl_requires_https() {
    use pdp_liaison::connector::*;

    let result = PdpConnectorConfig::client_ssl(
        "http://insecure.example.com",
        pdp_liaison::types::CommunicationType::XmlSoap,
        "cert",
        "key",
    );

    assert!(result.is_err());
}

#[test]
fn test_connector_client_ssl_accepts_https() {
    use pdp_liaison::connector::*;

    let result = PdpConnectorConfig::client_ssl(
        "https://secure.example.com",
        pdp_liaison::types::CommunicationType::XmlSoap,
        "cert",
        "key",
    );

    assert!(result.is_ok());
}

#[test]
fn test_register_obligation() {
    use pdp_liaison::connector::*;

    let config = PdpConnectorConfig::anonymous(
        "http://localhost:8080/pdp",
        pdp_liaison::types::CommunicationType::JsonRest,
    );
    let mut connector = PdpConnector::new(config);
    connector.register_obligation("urn:example:obligation:log");
    // No panic = pass (obligations are private, tested via evaluate behavior)
}

// ══════════════════════════════════════════════════════
// ── FFI tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_ffi_library_version() {
    use std::ffi::CStr;

    unsafe {
        let version = pdp_liaison::ffi::pdp_library_version();
        let version_str = CStr::from_ptr(version).to_str().unwrap();
        assert_eq!(version_str, "0.1.0");
        pdp_liaison::ffi::pdp_string_free(version);
    }
}

#[test]
fn test_ffi_request_lifecycle() {
    use std::ffi::{CStr, CString};

    unsafe {
        // Create request
        let req = pdp_liaison::ffi::pdp_request_create();
        assert!(!req.is_null());

        // Add element
        let category = CString::new(attribute_category::ACCESS_SUBJECT).unwrap();
        let attribute = CString::new(subject_attributes::SUBJECT_ID).unwrap();
        let data_type = CString::new(data_types::STRING).unwrap();
        let value = CString::new("testuser").unwrap();

        pdp_liaison::ffi::pdp_request_add_element(
            req,
            category.as_ptr(),
            attribute.as_ptr(),
            data_type.as_ptr(),
            value.as_ptr(),
        );

        // Get UID
        let uid = pdp_liaison::ffi::pdp_request_get_uid(req);
        assert!(!uid.is_null());
        let uid_str = CStr::from_ptr(uid).to_str().unwrap();
        assert!(!uid_str.is_empty());
        pdp_liaison::ffi::pdp_string_free(uid);

        // Get JSON
        let json = pdp_liaison::ffi::pdp_request_to_json(req, 0);
        assert!(!json.is_null());
        let json_str = CStr::from_ptr(json).to_str().unwrap();
        assert!(json_str.contains("testuser"));
        pdp_liaison::ffi::pdp_string_free(json);

        // Get XML
        let xml = pdp_liaison::ffi::pdp_request_to_xml(req);
        assert!(!xml.is_null());
        let xml_str = CStr::from_ptr(xml).to_str().unwrap();
        assert!(xml_str.contains("testuser"));
        pdp_liaison::ffi::pdp_string_free(xml);

        // Free
        pdp_liaison::ffi::pdp_request_free(req);
    }
}

#[test]
fn test_ffi_connector_anonymous() {
    use std::ffi::CString;

    unsafe {
        let url = CString::new("http://localhost:8080").unwrap();
        let conn = pdp_liaison::ffi::pdp_connector_create_anonymous(url.as_ptr(), 2);
        assert!(!conn.is_null());
        pdp_liaison::ffi::pdp_connector_free(conn);
    }
}

#[test]
fn test_ffi_constant_category() {
    use std::ffi::{CStr, CString};

    unsafe {
        let name = CString::new("resource").unwrap();
        let val = pdp_liaison::ffi::pdp_constant_category(name.as_ptr());
        let val_str = CStr::from_ptr(val).to_str().unwrap();
        assert_eq!(val_str, attribute_category::RESOURCE);
        pdp_liaison::ffi::pdp_string_free(val);
    }
}

#[test]
fn test_ffi_constant_datatype() {
    use std::ffi::{CStr, CString};

    unsafe {
        let name = CString::new("string").unwrap();
        let val = pdp_liaison::ffi::pdp_constant_datatype(name.as_ptr());
        let val_str = CStr::from_ptr(val).to_str().unwrap();
        assert_eq!(val_str, data_types::STRING);
        pdp_liaison::ffi::pdp_string_free(val);
    }
}

// ══════════════════════════════════════════════════════
// ── Error tests ──
// ══════════════════════════════════════════════════════

#[test]
fn test_error_display() {
    use pdp_liaison::error::*;

    let err = PdpError::general("test error");
    assert_eq!(format!("{}", err), "General: test error");

    let err = PdpError::connection("network down");
    assert_eq!(format!("{}", err), "Connection: network down");

    let err = PdpError::security("bad cert");
    assert_eq!(format!("{}", err), "Security: bad cert");
}

// ══════════════════════════════════════════════════════
// ── End-to-end request building test ──
// ══════════════════════════════════════════════════════

#[test]
fn test_full_request_build_and_serialize() {
    // Build a realistic request like the .NET demo
    let mut req = AuthorizationRequest::with_trace(true);

    req.add_element(
        attribute_category::ACCESS_SUBJECT,
        subject_attributes::SUBJECT_ID,
        data_types::STRING,
        "CN=Alice,OU=Users,O=Example",
    );
    req.add_element(
        attribute_category::RESOURCE,
        resource_attributes::RESOURCE_ID,
        data_types::ANY_URI,
        "http://example.com/resource/doc1",
    );
    req.add_element(
        attribute_category::ACTION,
        action_attributes::ACTION_ID,
        data_types::STRING,
        "read",
    );

    // Verify XML output
    let xml = xml_builder::build_xacml_request_xml(&req);
    assert!(xml.contains("CN=Alice,OU=Users,O=Example"));
    assert!(xml.contains("http://example.com/resource/doc1"));
    assert!(xml.contains("read"));
    assert!(xml.contains(attribute_category::ACCESS_SUBJECT));
    assert!(xml.contains(attribute_category::RESOURCE));
    assert!(xml.contains(attribute_category::ACTION));
    assert!(xml.contains(attribute_category::ENVIRONMENT));

    // Verify JSON output
    let json = json_builder::build_json_request(&req, true);
    assert!(json.contains("CN=Alice,OU=Users,O=Example"));
    assert!(json.contains("http://example.com/resource/doc1"));
    assert!(json.contains("read"));

    // Verify SAML + SOAP wrapping
    let saml = xml_builder::wrap_saml(&xml, req.combine_policies, &req.xacml_extensions);
    assert!(saml.contains("XACMLAuthzDecisionQuery"));

    let soap = xml_builder::wrap_soap(&saml);
    assert!(soap.contains("SOAP-ENV:Envelope"));
    assert!(soap.contains("XACMLAuthzDecisionQuery"));
}
