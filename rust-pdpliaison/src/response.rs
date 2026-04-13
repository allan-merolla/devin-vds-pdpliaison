//! XACML authorization response parsing.

use crate::constants::*;
use crate::error::PdpError;
use crate::types::*;

/// An authorization response from the PDP.
#[derive(Debug, Clone)]
pub struct AuthorizationResponse {
    decision: String,
    xacml_status_code: String,
    xacml_status_message: String,
    xacml_status_detail: String,
    pub obligations: Vec<Obligation>,
    pub associated_advice: Vec<Advice>,
    pub trace_info: String,
    pub policy_id_references: Vec<PolicyIdReference>,
    pub request_id: String,
    deny_biased_result: Option<Result>,
}

impl AuthorizationResponse {
    /// Create an empty response (used internally).
    pub fn empty() -> Self {
        Self {
            decision: String::new(),
            xacml_status_code: String::new(),
            xacml_status_message: String::new(),
            xacml_status_detail: String::new(),
            obligations: Vec::new(),
            associated_advice: Vec::new(),
            trace_info: String::new(),
            policy_id_references: Vec::new(),
            request_id: String::new(),
            deny_biased_result: None,
        }
    }

    /// Parse a response from an XML string (XACML Result element content).
    pub fn from_xml(xml: &str) -> std::result::Result<Self, PdpError> {
        let mut resp = Self::empty();
        resp.parse_xml_result(xml)?;
        Ok(resp)
    }

    /// Parse a response from a JSON object (serde_json::Value).
    pub fn from_json(json: &serde_json::Value) -> std::result::Result<Self, PdpError> {
        let mut resp = Self::empty();
        resp.parse_json_result(json)?;
        Ok(resp)
    }

    /// Get the deny-biased result.
    pub fn result(&self) -> Result {
        self.deny_biased_result.unwrap_or(Result::Deny)
    }

    /// Get the raw XACML decision from the PDP.
    pub fn pdp_decision(&self) -> XacmlResult {
        XacmlResult::parse(&self.decision)
    }

    /// Get the XACML status.
    pub fn xacml_status(&self) -> XacmlStatus {
        XacmlStatus::from_code(&self.xacml_status_code)
    }

    /// Get the XACML status message.
    pub fn xacml_status_message(&self) -> &str {
        &self.xacml_status_message
    }

    /// Get the XACML status detail.
    pub fn xacml_status_detail(&self) -> &str {
        &self.xacml_status_detail
    }

    /// Set the deny-biased result based on recognised obligations.
    pub fn set_deny_biased_result(&mut self, recognised_obligations: &[String]) {
        if self.pdp_decision() != XacmlResult::Permit {
            if self.obligations.is_empty() {
                self.deny_biased_result = Some(Result::Deny);
            } else {
                let refined: Vec<Obligation> = self
                    .obligations
                    .iter()
                    .filter(|ob| recognised_obligations.contains(&ob.id))
                    .cloned()
                    .collect();
                self.deny_biased_result = Some(Result::DenyWithObligations);
                self.obligations = refined;
            }
        } else {
            // permit
            if self.obligations.is_empty() {
                self.deny_biased_result = Some(Result::Permit);
            } else {
                let has_unrecognised = self
                    .obligations
                    .iter()
                    .any(|ob| !recognised_obligations.contains(&ob.id));
                if has_unrecognised {
                    self.deny_biased_result = Some(Result::DenyDueToUnrecognizedObligations);
                    self.obligations.clear();
                } else {
                    self.deny_biased_result = Some(Result::DenyUnlessAllObligationsSatisfied);
                }
            }
        }
    }

    // ── XML parsing ──

    fn parse_xml_result(&mut self, xml: &str) -> std::result::Result<(), PdpError> {
        use quick_xml::events::Event;
        use quick_xml::Reader;

        let mut reader = Reader::from_str(xml);
        reader.trim_text(true);

        let mut current_path: Vec<String> = Vec::new();
        let mut buf = Vec::new();
        let mut current_text = String::new();

        // Track attribute values for current element
        let mut current_obligation_id = String::new();
        let mut current_advice_id = String::new();
        let mut current_attr_assignment = Option::<AttributeAssignment>::None;
        let mut current_obligation_attrs: Vec<AttributeAssignment> = Vec::new();
        let mut current_advice_attrs: Vec<AttributeAssignment> = Vec::new();
        let mut current_status_code_value = String::new();
        let mut current_attr_id_for_result = String::new();
        let mut in_obligations = false;
        let mut in_advice = false;
        let mut in_policy_id_list = false;
        let mut current_policy_ref_version = String::new();

        loop {
            match reader.read_event_into(&mut buf) {
                Ok(Event::Start(ref e)) => {
                    let local_name = String::from_utf8_lossy(e.local_name().as_ref()).to_string();
                    current_path.push(local_name.clone());
                    current_text.clear();

                    match local_name.as_str() {
                        "Decision" => {}
                        "StatusCode" => {
                            for attr in e.attributes().flatten() {
                                if attr.key.local_name().as_ref() == b"Value" {
                                    current_status_code_value =
                                        String::from_utf8_lossy(&attr.value).to_string();
                                }
                            }
                        }
                        "Obligations" => {
                            in_obligations = true;
                        }
                        "Obligation" => {
                            current_obligation_attrs.clear();
                            for attr in e.attributes().flatten() {
                                if attr.key.local_name().as_ref() == b"ObligationId" {
                                    current_obligation_id =
                                        String::from_utf8_lossy(&attr.value).to_string();
                                }
                            }
                        }
                        "AssociatedAdvice" => {
                            in_advice = true;
                        }
                        "Advice" => {
                            current_advice_attrs.clear();
                            for attr in e.attributes().flatten() {
                                if attr.key.local_name().as_ref() == b"AdviceId" {
                                    current_advice_id =
                                        String::from_utf8_lossy(&attr.value).to_string();
                                }
                            }
                        }
                        "AttributeAssignment" => {
                            let mut aa = AttributeAssignment {
                                attribute_id: String::new(),
                                category_id: String::new(),
                                data_type: String::new(),
                                value: String::new(),
                            };
                            for attr in e.attributes().flatten() {
                                let key =
                                    String::from_utf8_lossy(attr.key.local_name().as_ref())
                                        .to_string();
                                let val = String::from_utf8_lossy(&attr.value).to_string();
                                match key.as_str() {
                                    "AttributeId" => aa.attribute_id = val,
                                    "Category" => aa.category_id = val,
                                    "DataType" => aa.data_type = val,
                                    _ => {}
                                }
                            }
                            current_attr_assignment = Some(aa);
                        }
                        "Attribute" => {
                            current_attr_id_for_result.clear();
                            for attr in e.attributes().flatten() {
                                if attr.key.local_name().as_ref() == b"AttributeId" {
                                    current_attr_id_for_result =
                                        String::from_utf8_lossy(&attr.value).to_string();
                                }
                            }
                        }
                        "PolicyIdentifierList" => {
                            in_policy_id_list = true;
                        }
                        "PolicyIdReference" => {
                            for attr in e.attributes().flatten() {
                                if attr.key.local_name().as_ref() == b"Version" {
                                    current_policy_ref_version =
                                        String::from_utf8_lossy(&attr.value).to_string();
                                }
                            }
                        }
                        _ => {}
                    }
                }
                Ok(Event::Text(ref e)) => {
                    current_text = e.unescape().unwrap_or_default().to_string();
                }
                Ok(Event::End(ref e)) => {
                    let local_name = String::from_utf8_lossy(e.local_name().as_ref()).to_string();

                    match local_name.as_str() {
                        "Decision" => {
                            self.decision = current_text.clone();
                        }
                        "StatusCode" => {
                            self.xacml_status_code = current_status_code_value.clone();
                        }
                        "StatusMessage" => {
                            self.xacml_status_message = current_text.clone();
                        }
                        "StatusDetail" => {
                            self.xacml_status_detail = current_text.clone();
                        }
                        "AttributeAssignment" => {
                            if let Some(mut aa) = current_attr_assignment.take() {
                                aa.value = current_text.clone();
                                if in_obligations {
                                    current_obligation_attrs.push(aa);
                                } else if in_advice {
                                    current_advice_attrs.push(aa);
                                }
                            }
                        }
                        "Obligation" => {
                            self.obligations.push(Obligation {
                                id: current_obligation_id.clone(),
                                attributes: current_obligation_attrs.clone(),
                            });
                            current_obligation_attrs.clear();
                        }
                        "Obligations" => {
                            in_obligations = false;
                        }
                        "Advice" => {
                            self.associated_advice.push(Advice {
                                id: current_advice_id.clone(),
                                attributes: current_advice_attrs.clone(),
                            });
                            current_advice_attrs.clear();
                        }
                        "AssociatedAdvice" => {
                            in_advice = false;
                        }
                        "AttributeValue" => {
                            if current_attr_id_for_result
                                == environment_attributes::VIEWDS_REQUEST_ID
                            {
                                self.request_id = current_text.clone();
                            }
                        }
                        "PolicyIdReference" => {
                            if in_policy_id_list {
                                self.policy_id_references.push(PolicyIdReference {
                                    version: current_policy_ref_version.clone(),
                                    uuid: current_text.clone(),
                                });
                            }
                        }
                        "PolicyIdentifierList" => {
                            in_policy_id_list = false;
                        }
                        _ => {}
                    }

                    current_path.pop();
                }
                Ok(Event::Empty(ref e)) => {
                    let local_name = String::from_utf8_lossy(e.local_name().as_ref()).to_string();
                    if local_name == "StatusCode" {
                        for attr in e.attributes().flatten() {
                            if attr.key.local_name().as_ref() == b"Value" {
                                self.xacml_status_code =
                                    String::from_utf8_lossy(&attr.value).to_string();
                            }
                        }
                    }
                }
                Ok(Event::Eof) => break,
                Err(e) => {
                    return Err(PdpError::general(format!("XML parse error: {}", e)));
                }
                _ => {}
            }
            buf.clear();
        }

        Ok(())
    }

    // ── JSON parsing ──

    fn parse_json_result(&mut self, json: &serde_json::Value) -> std::result::Result<(), PdpError> {
        // Decision
        if let Some(decision) = json.get("Decision").and_then(|v| v.as_str()) {
            self.decision = decision.to_string();
        }

        // Status
        if let Some(status) = json.get("Status") {
            if let Some(code) = status.get("StatusCode").and_then(|v| v.as_str()) {
                self.xacml_status_code = code.to_string();
            }
            // StatusCode could also be an object with Value
            if let Some(code_obj) = status.get("StatusCode") {
                if let Some(val) = code_obj.get("Value").and_then(|v| v.as_str()) {
                    self.xacml_status_code = val.to_string();
                }
            }
            if let Some(msg) = status.get("StatusMessage").and_then(|v| v.as_str()) {
                self.xacml_status_message = msg.to_string();
            }
            if let Some(detail) = status.get("StatusDetail").and_then(|v| v.as_str()) {
                self.xacml_status_detail = detail.to_string();
            }
        }

        // Obligations
        if let Some(obligs) = json.get("Obligations") {
            self.parse_json_obligations_or_advice(obligs, true);
        }

        // AssociatedAdvice
        if let Some(advice) = json.get("AssociatedAdvice") {
            self.parse_json_obligations_or_advice(advice, false);
        }

        // PolicyIdentifierList
        if let Some(policy_list) = json.get("PolicyIdentifierList") {
            if let Some(refs) = policy_list.get("PolicyIdReference") {
                let refs_array = if refs.is_array() {
                    refs.as_array().unwrap().clone()
                } else {
                    vec![refs.clone()]
                };
                for r in &refs_array {
                    let uuid = r
                        .get("Id")
                        .and_then(|v| v.as_str())
                        .unwrap_or("")
                        .to_string();
                    let version = r
                        .get("Version")
                        .and_then(|v| v.as_str())
                        .unwrap_or("")
                        .to_string();
                    self.policy_id_references
                        .push(PolicyIdReference { version, uuid });
                }
            }
        }

        // Category (for request-id extraction)
        if let Some(categories) = json.get("Category") {
            let cats = if categories.is_array() {
                categories.as_array().unwrap().clone()
            } else {
                vec![categories.clone()]
            };
            for cat in &cats {
                let cat_id = cat
                    .get("CategoryId")
                    .and_then(|v| v.as_str())
                    .unwrap_or("");
                if cat_id == attribute_category::ENVIRONMENT {
                    if let Some(attrs) = cat.get("Attribute") {
                        let attrs_array = if attrs.is_array() {
                            attrs.as_array().unwrap().clone()
                        } else {
                            vec![attrs.clone()]
                        };
                        for attr in &attrs_array {
                            let attr_id = attr
                                .get("AttributeId")
                                .and_then(|v| v.as_str())
                                .unwrap_or("");
                            if attr_id == environment_attributes::VIEWDS_REQUEST_ID {
                                if let Some(values) = attr.get("Value") {
                                    let vals = if values.is_array() {
                                        values.as_array().unwrap().clone()
                                    } else {
                                        vec![values.clone()]
                                    };
                                    if let Some(first) = vals.first() {
                                        self.request_id = first
                                            .as_str()
                                            .unwrap_or(&first.to_string())
                                            .to_string();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        Ok(())
    }

    fn parse_json_obligations_or_advice(
        &mut self,
        json: &serde_json::Value,
        is_obligation: bool,
    ) {
        let items = if json.is_array() {
            json.as_array().unwrap().clone()
        } else {
            vec![json.clone()]
        };

        for item in &items {
            let id = item
                .get("Id")
                .and_then(|v| v.as_str())
                .unwrap_or("")
                .to_string();

            let mut attributes = Vec::new();
            if let Some(assigns) = item.get("AttributeAssignment") {
                let assigns_arr = if assigns.is_array() {
                    assigns.as_array().unwrap().clone()
                } else {
                    vec![assigns.clone()]
                };
                for assign in &assigns_arr {
                    let aa = AttributeAssignment {
                        attribute_id: assign
                            .get("AttributeId")
                            .and_then(|v| v.as_str())
                            .unwrap_or("")
                            .to_string(),
                        category_id: assign
                            .get("Category")
                            .and_then(|v| v.as_str())
                            .unwrap_or("")
                            .to_string(),
                        data_type: assign
                            .get("DataType")
                            .and_then(|v| v.as_str())
                            .unwrap_or("string")
                            .to_string(),
                        value: assign
                            .get("Value")
                            .map(|v| {
                                if v.is_string() {
                                    v.as_str().unwrap().to_string()
                                } else {
                                    v.to_string()
                                }
                            })
                            .unwrap_or_default(),
                    };
                    attributes.push(aa);
                }
            }

            let oa = ObligationAdvice { id, attributes };
            if is_obligation {
                self.obligations.push(oa);
            } else {
                self.associated_advice.push(oa);
            }
        }
    }
}

impl std::fmt::Display for AuthorizationResponse {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "Deny-biased Decision: {}", self.result())?;
        write!(f, "\nPDP Decision: {:?}", self.pdp_decision())?;
        write!(f, "\nStatus Code: {:?}", self.xacml_status())?;
        if !self.xacml_status_message.is_empty() {
            write!(f, "\nStatus Message: {}", self.xacml_status_message)?;
        }
        if !self.xacml_status_detail.is_empty() {
            write!(f, "\nStatus Detail: {}", self.xacml_status_detail)?;
        }
        writeln!(f)?;
        for ob in &self.obligations {
            write!(f, "\nObligation: {}", ob)?;
        }
        for ad in &self.associated_advice {
            write!(f, "\nAdvice: {}", ad)?;
        }
        if !self.trace_info.is_empty() {
            write!(f, "\nTrace Information {{\n{}\n}}\n", self.trace_info)?;
        }
        if !self.policy_id_references.is_empty() {
            write!(f, "\nPolicyIdReference:\n")?;
            for pref in &self.policy_id_references {
                writeln!(f, "ID={}, Version={}", pref.uuid, pref.version)?;
            }
        }
        Ok(())
    }
}

/// A multi-response containing multiple authorization responses.
#[derive(Debug, Clone)]
pub struct MultiResponse {
    pub responses: Vec<AuthorizationResponse>,
}

impl MultiResponse {
    /// Parse a multi-response from XML (the XACML Response element).
    pub fn from_xml(xml: &str) -> std::result::Result<Self, PdpError> {
        // Split out individual Result elements and parse each
        let mut responses = Vec::new();
        let mut remaining = xml;

        while let Some(start) = remaining.find("<Result") {
            // Find the end tag — handle both namespaced and non-namespaced
            let after_start = &remaining[start..];
            if let Some(end_offset) = after_start.find("</Result>") {
                let end = start + end_offset + "</Result>".len();
                let result_xml = &remaining[start..end];
                responses.push(AuthorizationResponse::from_xml(result_xml)?);
                remaining = &remaining[end..];
            } else if let Some(end_offset) = find_namespaced_end_tag(after_start, "Result") {
                let end = start + end_offset;
                let result_xml = &remaining[start..end];
                responses.push(AuthorizationResponse::from_xml(result_xml)?);
                remaining = &remaining[end..];
            } else {
                break;
            }
        }

        Ok(MultiResponse { responses })
    }

    /// Parse a multi-response from JSON.
    pub fn from_json(json: &serde_json::Value) -> std::result::Result<Self, PdpError> {
        let mut responses = Vec::new();
        let items = if json.is_array() {
            json.as_array().unwrap().clone()
        } else {
            vec![json.clone()]
        };
        for item in &items {
            responses.push(AuthorizationResponse::from_json(item)?);
        }
        Ok(MultiResponse { responses })
    }

    /// Get the response matching a specific request UID.
    pub fn get_response_for_request(&self, request_uid: &str) -> Option<&AuthorizationResponse> {
        self.responses.iter().find(|r| r.request_id == request_uid)
    }
}

impl std::fmt::Display for MultiResponse {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        for (i, res) in self.responses.iter().enumerate() {
            write!(f, "-------------Result {}-------------", i + 1)?;
            write!(f, "\nRequest ID: {}", res.request_id)?;
            writeln!(f, "{}", res)?;
        }
        Ok(())
    }
}

/// Helper to find a namespaced end tag like `</ns:Result>`.
fn find_namespaced_end_tag(xml: &str, local_name: &str) -> Option<usize> {
    let pattern = "</";
    let mut search_from = 0;
    while let Some(pos) = xml[search_from..].find(pattern) {
        let abs_pos = search_from + pos;
        let after = &xml[abs_pos + 2..];
        if let Some(gt) = after.find('>') {
            let tag_name = &after[..gt];
            // Check if it ends with :LocalName
            if tag_name.ends_with(local_name)
                && (tag_name.len() == local_name.len()
                    || tag_name.as_bytes()[tag_name.len() - local_name.len() - 1] == b':')
            {
                return Some(abs_pos + 2 + gt + 1);
            }
        }
        search_from = abs_pos + 1;
    }
    None
}
