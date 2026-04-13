//! JSON serialization for XACML requests (REST mode).

use crate::constants::*;
use crate::request::{AuthorizationRequest, MultiRequest};

/// Build a JSON XACML request from an `AuthorizationRequest`.
pub fn build_json_request(request: &AuthorizationRequest, indented: bool) -> String {
    let categories_json = build_categories_json(&request.categories, indented, 2);
    let return_policy = if request.return_policy_id_list {
        if indented {
            "\n\t\t\"ReturnPolicyIdList\": true, ".to_string()
        } else {
            "\"ReturnPolicyIdList\": true, ".to_string()
        }
    } else {
        String::new()
    };

    if indented {
        format!(
            "{{\n\t\"Request\": {{{}{}\n\t}}\n}}",
            return_policy, categories_json
        )
    } else {
        format!("{{\"Request\": {{{}{}}}}}", return_policy, categories_json)
    }
}

/// Build a JSON XACML multi-request from a `MultiRequest`.
pub fn build_json_multi_request(multi_req: &MultiRequest, indented: bool) -> String {
    let categories_json = build_categories_json(&multi_req.categories, indented, 2);

    let mut references_json = String::new();
    if indented {
        references_json.push_str(",\n\t\t\"MultiRequests\": {");
        references_json.push_str("\n\t\t\t\"RequestReference\": [");
    } else {
        references_json.push_str(",\"MultiRequests\": {");
        references_json.push_str("\"RequestReference\": [");
    }

    for (i, (_uid, cat_refs)) in multi_req.references().iter().enumerate() {
        if i > 0 {
            references_json.push(',');
        }
        if indented {
            references_json.push_str("\n\t\t\t\t{");
            references_json.push_str("\n\t\t\t\t\t\"ReferenceId\": [");
        } else {
            references_json.push_str("{\"ReferenceId\": [");
        }

        for (j, ref_id) in cat_refs.iter().enumerate() {
            if j > 0 {
                references_json.push_str(", ");
            }
            references_json.push_str(&format!("\"{}\"", ref_id));
        }

        if indented {
            references_json.push_str("]\n\t\t\t\t}");
        } else {
            references_json.push_str("]}");
        }
    }

    if indented {
        references_json.push_str("\n\t\t\t]");
        references_json.push_str("\n\t\t}");
    } else {
        references_json.push_str("]}");
    }

    let return_policy = if multi_req.return_policy_id_list {
        if indented {
            "\n\t\t\"ReturnPolicyIdList\": true, ".to_string()
        } else {
            "\"ReturnPolicyIdList\": true, ".to_string()
        }
    } else {
        String::new()
    };

    let request_elements = format!("{}{}", categories_json, references_json);

    if indented {
        format!(
            "{{\n\t\"Request\": {{{}{}\n\t}}\n}}",
            return_policy, request_elements
        )
    } else {
        format!(
            "{{\"Request\": {{{}{}}}}}",
            return_policy, request_elements
        )
    }
}

fn build_categories_json(
    categories: &[crate::request::XacmlCategory],
    indented: bool,
    base_indent: usize,
) -> String {
    let mut json = String::new();

    if indented {
        json.push_str(&indent(base_indent));
        json.push_str("\"Category\": [");
    } else {
        json.push_str("\"Category\": [");
    }

    for (i, cat) in categories.iter().enumerate() {
        if i > 0 {
            json.push(',');
        }
        json.push_str(&build_category_json(cat, indented, base_indent));
    }

    if indented {
        json.push_str(&indent(base_indent));
        json.push(']');
    } else {
        json.push(']');
    }

    json
}

fn build_category_json(
    cat: &crate::request::XacmlCategory,
    indented: bool,
    base_indent: usize,
) -> String {
    let mut json = String::new();
    let l1 = if indented {
        indent(base_indent + 1)
    } else {
        String::new()
    };
    let l2 = if indented {
        indent(base_indent + 2)
    } else {
        String::new()
    };

    json.push_str(&l1);
    json.push('{');

    let shorthand = category_shorthand(&cat.category_id);
    json.push_str(&l2);
    json.push_str(&format!("\"CategoryId\": \"{}\",", shorthand));

    // XML Content
    if let Some(ref content) = cat.xml_content {
        json.push_str(&l2);
        json.push_str(&format!(
            "\"Content\": {}",
            json_escape_string(content)
        ));
        if !cat.attributes.is_empty() {
            json.push(',');
        }
    }

    // Attributes
    if !cat.attributes.is_empty() {
        json.push_str(&l2);
        json.push_str("\"Attribute\": [");
        let mut first_attr = true;
        for attr in &cat.attributes {
            // Skip trace attribute in JSON mode
            if attr.attribute_id == environment_attributes::VIEWDS_TRACE
                && attr.include_in_result
            {
                continue;
            }
            if attr.values.is_empty() {
                continue;
            }
            for val in &attr.values {
                if !first_attr {
                    json.push(',');
                }
                first_attr = false;
                json.push_str(&build_attribute_value_json(
                    &attr.attribute_id,
                    attr.include_in_result,
                    val,
                    indented,
                    base_indent + 3,
                ));
            }
        }
        json.push_str(&l2);
        json.push(']');
    }

    if !cat.xml_id.is_empty() {
        if !cat.attributes.is_empty() || cat.xml_content.is_some() {
            json.push(',');
        }
        json.push_str(&l2);
        json.push_str(&format!("\"Id\": \"{}\"", cat.xml_id));
    }

    json.push_str(&l1);
    json.push('}');

    json
}

fn build_attribute_value_json(
    attribute_id: &str,
    include_in_result: bool,
    val: &crate::request::XacmlValue,
    indented: bool,
    base_indent: usize,
) -> String {
    let mut json = String::new();
    let l0 = if indented {
        indent(base_indent)
    } else {
        String::new()
    };
    let l1 = if indented {
        indent(base_indent + 1)
    } else {
        String::new()
    };

    json.push_str(&l0);
    json.push('{');

    // AttributeId
    json.push_str(&l1);
    json.push_str(&format!("\"AttributeId\": \"{}\",", attribute_id));

    // DataType (omit if string)
    if val.data_type != data_types::STRING {
        let shorthand = data_type_shorthand(&val.data_type);
        json.push_str(&l1);
        json.push_str(&format!("\"DataType\": \"{}\",", shorthand));
    }

    // Value
    json.push_str(&l1);
    json.push_str(&format!("\"Value\": [{}]", json_value_repr(&val.data_type, &val.value)));

    // IncludeInResult
    if include_in_result {
        json.push(',');
        json.push_str(&l1);
        json.push_str("\"IncludeInResult\": true ");
    }

    json.push_str(&l0);
    json.push('}');

    json
}

fn json_value_repr(data_type: &str, value: &str) -> String {
    match data_type {
        dt if dt == data_types::DOUBLE || dt == data_types::BOOLEAN || dt == data_types::INTEGER => {
            value.to_string()
        }
        _ => json_escape_string(value),
    }
}

fn json_escape_string(s: &str) -> String {
    let mut result = String::with_capacity(s.len() + 2);
    result.push('"');
    for ch in s.chars() {
        match ch {
            '"' => result.push_str("\\\""),
            '\\' => result.push_str("\\\\"),
            '\n' => result.push_str("\\n"),
            '\r' => result.push_str("\\r"),
            '\t' => result.push_str("\\t"),
            c if (c as u32) < 0x20 => {
                result.push_str(&format!("\\u{:04X}", c as u32));
            }
            c => result.push(c),
        }
    }
    result.push('"');
    result
}

fn indent(level: usize) -> String {
    let mut s = "\r\n".to_string();
    for _ in 0..level {
        s.push('\t');
    }
    s
}
