//! XML serialization for XACML requests (SAML and SOAP wrapping).

use chrono::Utc;
use uuid::Uuid;

use crate::constants::*;
use crate::request::{AuthorizationRequest, MultiRequest, XacmlCategory};

/// Build the XACML Request XML document from an `AuthorizationRequest`.
pub fn build_xacml_request_xml(request: &AuthorizationRequest) -> String {
    let mut xml = String::new();
    xml.push_str(&format!(
        "<ns:Request xmlns:ns=\"{}\" ReturnPolicyIdList=\"{}\" CombinedDecision=\"false\">",
        XACML_SCHEMA,
        request.return_policy_id_list.to_string().to_lowercase()
    ));

    for cat in &request.categories {
        write_category_xml(&mut xml, cat, "ns");
    }

    xml.push_str("</ns:Request>");
    xml
}

/// Build the XACML Request XML document from a `MultiRequest`.
pub fn build_multi_request_xml(multi_req: &MultiRequest) -> String {
    let mut xml = String::new();
    xml.push_str(&format!(
        "<ns:Request xmlns:ns=\"{}\" ReturnPolicyIdList=\"{}\" CombinedDecision=\"false\">",
        XACML_SCHEMA,
        multi_req.return_policy_id_list.to_string().to_lowercase()
    ));

    for cat in &multi_req.categories {
        write_category_xml(&mut xml, cat, "ns");
    }

    // MultiRequests element
    xml.push_str("<ns:MultiRequests>");
    for (_uid, cat_refs) in multi_req.references() {
        xml.push_str("<ns:RequestReference>");
        for ref_id in cat_refs {
            xml.push_str(&format!(
                "<ns:AttributesReference ReferenceId=\"{}\"/>",
                ref_id
            ));
        }
        xml.push_str("</ns:RequestReference>");
    }
    xml.push_str("</ns:MultiRequests>");

    xml.push_str("</ns:Request>");
    xml
}

fn write_category_xml(xml: &mut String, cat: &XacmlCategory, prefix: &str) {
    xml.push_str(&format!("<{}:Attributes Category=\"{}\"", prefix, cat.category_id));
    if !cat.xml_id.is_empty() {
        xml.push_str(&format!(" xml:id=\"{}\"", cat.xml_id));
    }
    xml.push('>');

    if let Some(ref content) = cat.xml_content {
        xml.push_str(&format!("<{}:Content>{}</{0}:Content>", prefix, content));
    }

    for attr in &cat.attributes {
        xml.push_str(&format!(
            "<{}:Attribute AttributeId=\"{}\" IncludeInResult=\"{}\">",
            prefix,
            attr.attribute_id,
            attr.include_in_result.to_string().to_lowercase()
        ));
        for val in &attr.values {
            xml.push_str(&format!(
                "<{}:AttributeValue DataType=\"{}\">{}</{0}:AttributeValue>",
                prefix,
                val.data_type,
                xml_escape(&val.value)
            ));
        }
        xml.push_str(&format!("</{0}:Attribute>", prefix));
    }

    xml.push_str(&format!("</{0}:Attributes>", prefix));
}

/// Wrap an XACML request in a SAML XACMLAuthzDecisionQuery envelope.
pub fn wrap_saml(request_xml: &str, combine_policies: bool, extensions: &[String]) -> String {
    let query_id = format!("_{}", Uuid::new_v4());
    let issue_instant = format_global_time(&Utc::now());

    let mut xml = String::new();
    xml.push_str(&format!(
        "<xsp:XACMLAuthzDecisionQuery xmlns:xsp=\"{}\" ID=\"{}\" Version=\"2.0\" IssueInstant=\"{}\" CombinePolicies=\"{}\">",
        XACML_SAML_SCHEMA,
        query_id,
        issue_instant,
        combine_policies.to_string().to_lowercase()
    ));
    xml.push_str(request_xml);

    for ext in extensions {
        xml.push_str(ext);
    }

    xml.push_str("</xsp:XACMLAuthzDecisionQuery>");
    xml
}

/// Wrap a message in a SOAP 1.2 envelope.
pub fn wrap_soap(message_xml: &str) -> String {
    let mut xml = String::new();
    xml.push_str("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    xml.push_str(&format!(
        "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"{}\">",
        SOAP_1_2
    ));
    xml.push_str("<SOAP-ENV:Header/>");
    xml.push_str("<SOAP-ENV:Body>");
    xml.push_str(message_xml);
    xml.push_str("</SOAP-ENV:Body>");
    xml.push_str("</SOAP-ENV:Envelope>");
    xml
}

/// Format a UTC DateTime as an XML global time string.
pub fn format_global_time(dt: &chrono::DateTime<Utc>) -> String {
    dt.format("%Y-%m-%dT%H:%M:%S%.3fZ").to_string()
}

/// Format a DateTime as an XML local time string.
pub fn format_local_time(dt: &chrono::DateTime<Utc>) -> String {
    dt.format("%Y-%m-%dT%H:%M:%S%.3f").to_string()
}

/// Escape special XML characters.
fn xml_escape(s: &str) -> String {
    s.replace('&', "&amp;")
        .replace('<', "&lt;")
        .replace('>', "&gt;")
        .replace('"', "&quot;")
        .replace('\'', "&apos;")
}

/// Extract the query ID from a SAML request for response validation.
pub fn extract_saml_query_id(saml_xml: &str) -> Option<String> {
    // Look for ID="..." in XACMLAuthzDecisionQuery
    let marker = "ID=\"";
    if let Some(start) = saml_xml.find(marker) {
        let after = &saml_xml[start + marker.len()..];
        if let Some(end) = after.find('"') {
            return Some(after[..end].to_string());
        }
    }
    None
}

/// Extract SAML response metadata from the server's XML response.
pub struct SamlResponseInfo {
    pub issue_instant: String,
    pub in_response_to: String,
    pub saml_status: String,
    pub authz_issuer: String,
}

/// Parse SAML metadata from the server's XML response.
pub fn parse_saml_response(xml: &str) -> SamlResponseInfo {
    let mut info = SamlResponseInfo {
        issue_instant: String::new(),
        in_response_to: String::new(),
        saml_status: String::new(),
        authz_issuer: String::new(),
    };

    // Extract from <*:Response ... IssueInstant="..." InResponseTo="...">
    if let Some(resp_start) = xml.find("Response") {
        let chunk = &xml[resp_start..];
        if let Some(end) = chunk.find('>') {
            let tag = &chunk[..end];
            info.issue_instant = extract_attr(tag, "IssueInstant").unwrap_or_default();
            info.in_response_to = extract_attr(tag, "InResponseTo").unwrap_or_default();
        }
    }

    // StatusCode Value
    if let Some(sc_start) = xml.find("StatusCode") {
        let chunk = &xml[sc_start..];
        if let Some(end) = chunk.find('>') {
            let tag = &chunk[..end];
            info.saml_status = extract_attr(tag, "Value").unwrap_or_default();
        }
    }

    // Issuer
    if let Some(issuer_start) = xml.find("Issuer") {
        let chunk = &xml[issuer_start..];
        if let Some(gt) = chunk.find('>') {
            let after = &chunk[gt + 1..];
            if let Some(lt) = after.find('<') {
                info.authz_issuer = after[..lt].to_string();
            }
        }
    }

    info
}

fn extract_attr(tag: &str, attr_name: &str) -> Option<String> {
    let pattern = format!("{}=\"", attr_name);
    if let Some(start) = tag.find(&pattern) {
        let after = &tag[start + pattern.len()..];
        if let Some(end) = after.find('"') {
            return Some(after[..end].to_string());
        }
    }
    None
}

/// Extract the content between XACML Result tags from a full server response.
pub fn extract_result_xml(full_xml: &str) -> Option<String> {
    // Try non-namespaced first
    if let Some(start) = full_xml.find("<Result") {
        let after = &full_xml[start..];
        if let Some(end) = after.find("</Result>") {
            return Some(after[..end + "</Result>".len()].to_string());
        }
        // Try namespaced
        let mut search = 0;
        while let Some(pos) = after[search..].find("</") {
            let abs = search + pos;
            let rest = &after[abs + 2..];
            if let Some(gt) = rest.find('>') {
                let tag = &rest[..gt];
                if tag.ends_with("Result") {
                    return Some(after[..abs + 2 + gt + 1].to_string());
                }
            }
            search = abs + 1;
        }
    }
    None
}

/// Extract the XACML Response element content from a full server response.
pub fn extract_response_xml(full_xml: &str) -> Option<String> {
    // Look for <*:Response xmlns:*="...xacml..." or <Response
    // Find the XACML Response (not SAML Response)
    let xacml_ns = XACML_SCHEMA;
    if let Some(pos) = full_xml.find(xacml_ns) {
        // Back-track to find the enclosing element
        let before = &full_xml[..pos];
        if let Some(lt) = before.rfind('<') {
            let candidate = &full_xml[lt..];
            // Find matching close tag
            let tag_end = candidate.find('>').unwrap_or(0);
            let tag_content = &candidate[1..tag_end];
            let tag_name = tag_content
                .split_whitespace()
                .next()
                .unwrap_or("")
                .to_string();
            if tag_name.ends_with("Response") {
                let close = format!("</{}>", tag_name);
                if let Some(close_pos) = candidate.find(&close) {
                    return Some(candidate[..close_pos + close.len()].to_string());
                }
            }
        }
    }
    None
}
