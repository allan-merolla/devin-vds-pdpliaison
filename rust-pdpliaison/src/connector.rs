//! PDP connector implementations for communicating with a Policy Decision Point.

use crate::error::PdpError;
use crate::json_builder;
use crate::request::{AuthorizationRequest, MultiRequest};
use crate::response::{AuthorizationResponse, MultiResponse};
use crate::types::CommunicationType;
use crate::xml_builder;

/// Configuration for connecting to a PDP.
#[derive(Debug, Clone)]
pub struct PdpConnectorConfig {
    /// The URL of the PDP server.
    pub pdp_url: String,
    /// The communication type (XML_SOAP, XML_REST, JSON_REST).
    pub communication_type: CommunicationType,
    /// Whether to verify XML signatures on responses.
    pub verify_signature: bool,
    /// Client certificate in PEM format (for SSL client auth).
    pub client_cert_pem: Option<String>,
    /// Client private key in PEM format (for SSL client auth).
    pub client_key_pem: Option<String>,
    /// Signing certificate in PEM format (for XML signing).
    pub signing_cert_pem: Option<String>,
    /// Signing private key in PEM format (for XML signing).
    pub signing_key_pem: Option<String>,
    /// CA certificates in PEM format for server verification.
    pub ca_certs_pem: Option<String>,
    /// Whether to accept invalid/self-signed server certificates.
    pub accept_invalid_certs: bool,
}

impl PdpConnectorConfig {
    /// Create a config for anonymous connection (no client cert).
    pub fn anonymous(pdp_url: &str, communication_type: CommunicationType) -> Self {
        Self {
            pdp_url: pdp_url.to_string(),
            communication_type,
            verify_signature: false,
            client_cert_pem: None,
            client_key_pem: None,
            signing_cert_pem: None,
            signing_key_pem: None,
            ca_certs_pem: None,
            accept_invalid_certs: false,
        }
    }

    /// Create a config for client SSL authentication.
    pub fn client_ssl(
        pdp_url: &str,
        communication_type: CommunicationType,
        client_cert_pem: &str,
        client_key_pem: &str,
    ) -> std::result::Result<Self, PdpError> {
        if !pdp_url.to_lowercase().starts_with("https") {
            return Err(PdpError::security(
                "The URL must begin with 'https' when using client SSL authentication",
            ));
        }
        Ok(Self {
            pdp_url: pdp_url.to_string(),
            communication_type,
            verify_signature: false,
            client_cert_pem: Some(client_cert_pem.to_string()),
            client_key_pem: Some(client_key_pem.to_string()),
            signing_cert_pem: None,
            signing_key_pem: None,
            ca_certs_pem: None,
            accept_invalid_certs: false,
        })
    }
}

/// The PDP connector used to send requests and receive responses.
#[derive(Debug, Clone)]
pub struct PdpConnector {
    config: PdpConnectorConfig,
    recognised_obligations: Vec<String>,
}

impl PdpConnector {
    /// Create a new PDP connector with the given configuration.
    pub fn new(config: PdpConnectorConfig) -> Self {
        Self {
            config,
            recognised_obligations: Vec::new(),
        }
    }

    /// Get mutable access to the connector configuration.
    pub fn config_mut(&mut self) -> &mut PdpConnectorConfig {
        &mut self.config
    }

    /// Register an obligation that the application recognizes and can fulfill.
    pub fn register_obligation(&mut self, obligation_id: &str) {
        self.recognised_obligations
            .push(obligation_id.to_string());
    }

    /// Create a new empty authorization request.
    pub fn create_request(&self) -> AuthorizationRequest {
        AuthorizationRequest::new()
    }

    /// Create a new authorization request with trace support.
    pub fn create_request_with_trace(&self, trace: bool) -> AuthorizationRequest {
        AuthorizationRequest::with_trace(trace)
    }

    /// Create a new authorization request with all options.
    pub fn create_request_with_options(
        &self,
        trace: bool,
        return_policy_id_list: bool,
        combine_policies: bool,
    ) -> AuthorizationRequest {
        AuthorizationRequest::with_all_options(trace, return_policy_id_list, combine_policies)
    }

    /// Evaluate a single authorization request against the PDP.
    pub fn evaluate(
        &self,
        request: &AuthorizationRequest,
    ) -> std::result::Result<AuthorizationResponse, PdpError> {
        let mut response = match self.config.communication_type {
            CommunicationType::JsonRest => self.evaluate_json_rest(request)?,
            CommunicationType::XmlRest => self.evaluate_xml(request, false)?,
            CommunicationType::XmlSoap => self.evaluate_xml(request, true)?,
        };

        response.set_deny_biased_result(&self.recognised_obligations);
        Ok(response)
    }

    /// Evaluate a multi-request against the PDP.
    pub fn evaluate_multi(
        &self,
        multi_request: &MultiRequest,
    ) -> std::result::Result<MultiResponse, PdpError> {
        let mut multi_response = match self.config.communication_type {
            CommunicationType::JsonRest => self.evaluate_multi_json_rest(multi_request)?,
            CommunicationType::XmlRest => self.evaluate_multi_xml(multi_request, false)?,
            CommunicationType::XmlSoap => self.evaluate_multi_xml(multi_request, true)?,
        };

        for resp in &mut multi_response.responses {
            resp.set_deny_biased_result(&self.recognised_obligations);
        }

        Ok(multi_response)
    }

    // ── JSON REST ──

    fn evaluate_json_rest(
        &self,
        request: &AuthorizationRequest,
    ) -> std::result::Result<AuthorizationResponse, PdpError> {
        let json_request = json_builder::build_json_request(request, true);
        let response_body = self.http_post(&json_request, "application/xacml+json; version=3.0")?;

        let json: serde_json::Value = serde_json::from_str(&response_body)?;
        let response_node = json
            .get("Response")
            .ok_or_else(|| PdpError::general("No 'Response' element in JSON response"))?;

        // If it's an array, take the first element
        let single = if response_node.is_array() {
            response_node
                .as_array()
                .and_then(|arr| arr.first())
                .ok_or_else(|| PdpError::general("Empty Response array"))?
        } else {
            response_node
        };

        AuthorizationResponse::from_json(single)
    }

    fn evaluate_multi_json_rest(
        &self,
        multi_request: &MultiRequest,
    ) -> std::result::Result<MultiResponse, PdpError> {
        let json_request = json_builder::build_json_multi_request(multi_request, true);
        let response_body = self.http_post(&json_request, "application/xacml+json; version=3.0")?;

        let json: serde_json::Value = serde_json::from_str(&response_body)?;
        let response_node = json
            .get("Response")
            .ok_or_else(|| PdpError::general("No 'Response' element in JSON response"))?;

        MultiResponse::from_json(response_node)
    }

    // ── XML (SOAP and REST) ──

    fn evaluate_xml(
        &self,
        request: &AuthorizationRequest,
        use_soap: bool,
    ) -> std::result::Result<AuthorizationResponse, PdpError> {
        let xacml_xml = xml_builder::build_xacml_request_xml(request);
        let saml_xml = xml_builder::wrap_saml(
            &xacml_xml,
            request.combine_policies,
            &request.xacml_extensions,
        );

        let message = if use_soap {
            xml_builder::wrap_soap(&saml_xml)
        } else {
            saml_xml.clone()
        };

        let content_type = if use_soap {
            "application/soap+xml"
        } else {
            "application/xacml+xml; version=3.0"
        };

        let response_body = self.http_post(&message, content_type)?;
        self.validate_saml_response(&saml_xml, &response_body)?;

        let result_xml = xml_builder::extract_result_xml(&response_body)
            .ok_or_else(|| PdpError::general("No Result element found in XML response"))?;

        AuthorizationResponse::from_xml(&result_xml)
    }

    fn evaluate_multi_xml(
        &self,
        multi_request: &MultiRequest,
        use_soap: bool,
    ) -> std::result::Result<MultiResponse, PdpError> {
        let xacml_xml = xml_builder::build_multi_request_xml(multi_request);
        let saml_xml = xml_builder::wrap_saml(
            &xacml_xml,
            multi_request.combine_policies,
            &multi_request.xacml_extensions,
        );

        let message = if use_soap {
            xml_builder::wrap_soap(&saml_xml)
        } else {
            saml_xml.clone()
        };

        let content_type = if use_soap {
            "application/soap+xml"
        } else {
            "application/xacml+xml; version=3.0"
        };

        let response_body = self.http_post(&message, content_type)?;
        self.validate_saml_response(&saml_xml, &response_body)?;

        let response_xml = xml_builder::extract_response_xml(&response_body)
            .ok_or_else(|| PdpError::general("No Response element found in XML response"))?;

        MultiResponse::from_xml(&response_xml)
    }

    fn validate_saml_response(
        &self,
        saml_request: &str,
        response: &str,
    ) -> std::result::Result<(), PdpError> {
        let query_id = xml_builder::extract_saml_query_id(saml_request);
        let saml_info = xml_builder::parse_saml_response(response);

        if let Some(qid) = query_id {
            if !saml_info.in_response_to.is_empty() && qid != saml_info.in_response_to {
                return Err(PdpError::general(
                    "Request ID does not match the response ID.",
                ));
            }
        }

        if saml_info.saml_status == "urn:oasis:names:tc:SAML:2.0:status:AuthnFailed" {
            return Err(PdpError::client_auth("Authentication failed."));
        }

        Ok(())
    }

    // ── HTTP ──

    fn http_post(
        &self,
        body: &str,
        content_type: &str,
    ) -> std::result::Result<String, PdpError> {
        let client_builder = reqwest::blocking::Client::builder()
            .danger_accept_invalid_certs(self.config.accept_invalid_certs);

        // Add client certificate if configured
        let client_builder = if let (Some(cert_pem), Some(key_pem)) =
            (&self.config.client_cert_pem, &self.config.client_key_pem)
        {
            let combined = format!("{}\n{}", cert_pem, key_pem);
            let identity = reqwest::Identity::from_pem(combined.as_bytes())
                .map_err(|e| PdpError::security(format!("Invalid client certificate: {}", e)))?;
            client_builder.identity(identity)
        } else {
            client_builder
        };

        // Add CA certificates if configured
        let client_builder = if let Some(ca_pem) = &self.config.ca_certs_pem {
            let ca_cert = reqwest::Certificate::from_pem(ca_pem.as_bytes())
                .map_err(|e| PdpError::security(format!("Invalid CA certificate: {}", e)))?;
            client_builder.add_root_certificate(ca_cert)
        } else {
            client_builder
        };

        let client = client_builder
            .build()
            .map_err(|e| PdpError::connection(format!("Failed to build HTTP client: {}", e)))?;

        let response = client
            .post(&self.config.pdp_url)
            .header("Content-Type", content_type)
            .body(body.to_string())
            .send()
            .map_err(|e| {
                PdpError::connection(format!("Error sending request to PDP: {}", e))
            })?;

        let status = response.status();
        let response_text = response.text().map_err(|e| {
            PdpError::connection(format!("Error reading response from PDP: {}", e))
        })?;

        if !status.is_success() {
            return Err(PdpError::connection(format!(
                "PDP returned HTTP {}: {}",
                status, response_text
            )));
        }

        Ok(response_text)
    }
}
