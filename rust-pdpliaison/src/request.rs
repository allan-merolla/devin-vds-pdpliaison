//! XACML authorization request building.

use uuid::Uuid;

use crate::constants::*;
use crate::types::AttributeDataType;

/// A single XACML attribute value with its data type.
#[derive(Debug, Clone, PartialEq)]
pub struct XacmlValue {
    pub data_type: String,
    pub value: String,
}

/// An XACML attribute with an ID and one or more values.
#[derive(Debug, Clone)]
pub struct XacmlAttribute {
    pub attribute_id: String,
    pub include_in_result: bool,
    pub values: Vec<XacmlValue>,
}

impl XacmlAttribute {
    pub fn new(attribute_id: &str, include_in_result: bool) -> Self {
        Self {
            attribute_id: attribute_id.to_string(),
            include_in_result,
            values: Vec::new(),
        }
    }

    pub fn add_value(&mut self, data_type: &str, value: &str) {
        let new_val = XacmlValue {
            data_type: data_type.to_string(),
            value: value.to_string(),
        };
        if !self.values.contains(&new_val) {
            self.values.push(new_val);
        }
    }
}

/// An XACML category (e.g., subject, resource, action, environment).
#[derive(Debug, Clone)]
pub struct XacmlCategory {
    pub category_id: String,
    pub xml_id: String,
    pub attributes: Vec<XacmlAttribute>,
    pub xml_content: Option<String>,
}

impl XacmlCategory {
    pub fn new(category_id: &str) -> Self {
        Self {
            category_id: category_id.to_string(),
            xml_id: String::new(),
            attributes: Vec::new(),
            xml_content: None,
        }
    }

    pub fn with_xml_id(category_id: &str, xml_id: &str) -> Self {
        Self {
            category_id: category_id.to_string(),
            xml_id: xml_id.to_string(),
            attributes: Vec::new(),
            xml_content: None,
        }
    }

    pub fn add_attribute(
        &mut self,
        attribute_id: &str,
        data_type: &str,
        value: &str,
        include_in_result: bool,
    ) {
        for att in &mut self.attributes {
            if att.attribute_id == attribute_id {
                att.add_value(data_type, value);
                return;
            }
        }
        let mut new_attr = XacmlAttribute::new(attribute_id, include_in_result);
        new_attr.add_value(data_type, value);
        self.attributes.push(new_attr);
    }

    /// Check structural equality (same category_id, same attributes with same values).
    pub fn structurally_equal(&self, other: &XacmlCategory) -> bool {
        if self.category_id != other.category_id {
            return false;
        }
        if self.attributes.len() != other.attributes.len() {
            return false;
        }
        match (&self.xml_content, &other.xml_content) {
            (Some(a), Some(b)) => {
                if a != b {
                    return false;
                }
            }
            (None, None) => {}
            _ => return false,
        }
        for att in &self.attributes {
            let found = other.attributes.iter().any(|other_att| {
                att.attribute_id == other_att.attribute_id
                    && att.values.len() == other_att.values.len()
                    && att
                        .values
                        .iter()
                        .all(|v| other_att.values.contains(v))
            });
            if !found {
                return false;
            }
        }
        true
    }
}

/// An XACML authorization request.
#[derive(Debug, Clone)]
pub struct AuthorizationRequest {
    pub uid: Uuid,
    pub trace: bool,
    pub return_policy_id_list: bool,
    pub combine_policies: bool,
    pub categories: Vec<XacmlCategory>,
    pub xacml_extensions: Vec<String>,
}

impl AuthorizationRequest {
    /// Create a new authorization request with default settings.
    pub fn new() -> Self {
        Self {
            uid: Uuid::new_v4(),
            trace: false,
            return_policy_id_list: false,
            combine_policies: true,
            categories: Vec::new(),
            xacml_extensions: Vec::new(),
        }
    }

    /// Create a new authorization request with trace support.
    pub fn with_trace(trace: bool) -> Self {
        let mut req = Self {
            uid: Uuid::new_v4(),
            trace,
            return_policy_id_list: false,
            combine_policies: true,
            categories: Vec::new(),
            xacml_extensions: Vec::new(),
        };
        if trace {
            req.add_element_internal(
                attribute_category::ENVIRONMENT,
                environment_attributes::VIEWDS_TRACE,
                data_types::ANY_TYPE,
                "",
                true,
            );
        }
        req
    }

    /// Create a new authorization request with trace and policy ID list support.
    pub fn with_options(trace: bool, return_policy_id_list: bool) -> Self {
        let mut req = Self {
            uid: Uuid::new_v4(),
            trace,
            return_policy_id_list,
            combine_policies: true,
            categories: Vec::new(),
            xacml_extensions: Vec::new(),
        };
        if trace {
            req.add_element_internal(
                attribute_category::ENVIRONMENT,
                environment_attributes::VIEWDS_TRACE,
                data_types::ANY_TYPE,
                "",
                true,
            );
        }
        req
    }

    /// Create a new authorization request with all options.
    pub fn with_all_options(
        trace: bool,
        return_policy_id_list: bool,
        combine_policies: bool,
    ) -> Self {
        let mut req = Self {
            uid: Uuid::new_v4(),
            trace,
            return_policy_id_list,
            combine_policies,
            categories: Vec::new(),
            xacml_extensions: Vec::new(),
        };
        if trace {
            req.add_element_internal(
                attribute_category::ENVIRONMENT,
                environment_attributes::VIEWDS_TRACE,
                data_types::ANY_TYPE,
                "",
                true,
            );
        }
        req
    }

    fn add_element_internal(
        &mut self,
        category: &str,
        attribute: &str,
        data_type: &str,
        value: &str,
        include_in_results: bool,
    ) {
        for cat in &mut self.categories {
            if cat.category_id == category {
                cat.add_attribute(attribute, data_type, value, include_in_results);
                return;
            }
        }
        let mut new_cat = XacmlCategory::new(category);
        new_cat.add_attribute(attribute, data_type, value, include_in_results);
        self.categories.push(new_cat);
    }

    /// Add an attribute to the request using string data type.
    pub fn add_element(&mut self, category: &str, attribute: &str, data_type: &str, value: &str) {
        self.add_element_internal(category, attribute, data_type, value, false);
    }

    /// Add an attribute to the request using the `AttributeDataType` enum.
    pub fn add_element_typed(
        &mut self,
        category: &str,
        attribute: &str,
        data_type: AttributeDataType,
        value: &str,
    ) {
        let dt_str = data_type_id(data_type);
        self.add_element(category, attribute, dt_str, value);
    }

    /// Set the Content element for a category.
    pub fn set_content(&mut self, category: &str, content: &str) {
        for cat in &mut self.categories {
            if cat.category_id == category {
                cat.xml_content = Some(content.to_string());
                return;
            }
        }
        let mut new_cat = XacmlCategory::new(category);
        new_cat.xml_content = Some(content.to_string());
        self.categories.push(new_cat);
    }

    /// Add the request GUID to the environment category (used for multi-requests).
    #[allow(dead_code)]
    pub(crate) fn add_guid(&mut self) {
        self.add_element_internal(
            attribute_category::ENVIRONMENT,
            "http://viewds.com/xacml/environment/request-id",
            data_types::STRING,
            &self.uid.to_string(),
            true,
        );
    }

    /// Add an XACML extension element (Policy, PolicySet, ReferencedPolicies) as raw XML.
    pub fn add_extension(&mut self, extension_xml: &str) {
        self.xacml_extensions.push(extension_xml.to_string());
    }
}

impl Default for AuthorizationRequest {
    fn default() -> Self {
        Self::new()
    }
}

/// A multi-request containing multiple authorization requests.
#[derive(Debug, Clone)]
pub struct MultiRequest {
    pub return_policy_id_list: bool,
    pub combine_policies: bool,
    pub categories: Vec<XacmlCategory>,
    pub requests: Vec<AuthorizationRequest>,
    pub xacml_extensions: Vec<String>,
    /// Maps request UID -> list of category xml_ids
    request_references: Vec<(Uuid, Vec<String>)>,
}

impl MultiRequest {
    pub fn new(return_policy_id_list: bool) -> Self {
        Self {
            return_policy_id_list,
            combine_policies: true,
            categories: Vec::new(),
            requests: Vec::new(),
            xacml_extensions: Vec::new(),
            request_references: Vec::new(),
        }
    }

    pub fn with_combine_policies(return_policy_id_list: bool, combine_policies: bool) -> Self {
        Self {
            return_policy_id_list,
            combine_policies,
            categories: Vec::new(),
            requests: Vec::new(),
            xacml_extensions: Vec::new(),
            request_references: Vec::new(),
        }
    }

    fn add_category(&mut self, category: &XacmlCategory) -> String {
        // Check if structurally equal category already exists
        for existing in &self.categories {
            if existing.structurally_equal(category) {
                return existing.xml_id.clone();
            }
        }

        let cat_xml_id = format!("element-id-{}", self.categories.len() + 1);
        let mut new_cat = XacmlCategory::with_xml_id(&category.category_id, &cat_xml_id);
        for att in &category.attributes {
            for val in &att.values {
                new_cat.add_attribute(
                    &att.attribute_id,
                    &val.data_type,
                    &val.value,
                    att.include_in_result,
                );
            }
        }
        new_cat.xml_content = category.xml_content.clone();
        self.categories.push(new_cat);
        cat_xml_id
    }

    /// Add an authorization request to the multi-request.
    pub fn add_request(
        &mut self,
        request: &mut AuthorizationRequest,
    ) -> std::result::Result<(), crate::error::PdpError> {
        if !request.xacml_extensions.is_empty() {
            return Err(crate::error::PdpError::general(
                "Requests with XACML extension elements cannot be added to a MultiRequest. \
                 Try adding the extension elements to the MultiRequest object.",
            ));
        }

        let mut cat_refs: Vec<String> = Vec::new();
        let mut req_id_added = false;

        for cat in &mut request.categories {
            if cat.category_id == attribute_category::ENVIRONMENT {
                cat.add_attribute(
                    "http://viewds.com/xacml/environment/request-id",
                    data_types::STRING,
                    &request.uid.to_string(),
                    true,
                );
                req_id_added = true;
            }
            let ref_id = self.add_category(cat);
            cat_refs.push(ref_id);
        }

        if !req_id_added {
            let cat_xml_id = format!("element-id-{}", self.categories.len() + 1);
            let mut env_cat =
                XacmlCategory::with_xml_id(attribute_category::ENVIRONMENT, &cat_xml_id);
            env_cat.add_attribute(
                "http://viewds.com/xacml/environment/request-id",
                data_types::STRING,
                &request.uid.to_string(),
                true,
            );
            self.categories.push(env_cat);
            cat_refs.push(cat_xml_id);
        }

        self.request_references
            .push((request.uid, cat_refs));
        self.requests.push(request.clone());
        Ok(())
    }

    /// Get the request references (uid -> category xml_id list).
    pub fn references(&self) -> &[(Uuid, Vec<String>)] {
        &self.request_references
    }

    /// Add an XACML extension element as raw XML.
    pub fn add_extension(&mut self, extension_xml: &str) {
        self.xacml_extensions.push(extension_xml.to_string());
    }
}
