//! C FFI (Foreign Function Interface) layer for cross-language consumption.
//!
//! This module exposes the PDP Liaison library through a C-compatible ABI using opaque handles.
//! Consumer languages use these functions via:
//! - **PHP**: FFI extension (`FFI::cdef(...)`)
//! - **.NET Framework / .NET 8**: P/Invoke (`[DllImport("pdp_liaison")]`)
//! - **Java**: JNA (`Native.load("pdp_liaison", ...)`)
//!
//! # Memory management
//! All `*_create` functions return opaque pointers that MUST be freed with the corresponding
//! `*_free` function. Strings returned by `*_to_string` / `*_get_*` functions must be freed
//! with `pdp_string_free`.
//!
//! # Safety
//! All FFI functions that accept raw pointers perform null checks before dereferencing.
//! The `not_unsafe_ptr_arg_deref` lint is suppressed at the module level because these
//! are extern "C" functions designed for use across FFI boundaries where the caller
//! is responsible for providing valid pointers.
#![allow(clippy::not_unsafe_ptr_arg_deref)]

use std::ffi::{CStr, CString};
use std::os::raw::c_char;
use std::ptr;

use crate::connector::{PdpConnector, PdpConnectorConfig};
use crate::request::{AuthorizationRequest, MultiRequest};
use crate::response::{AuthorizationResponse, MultiResponse};
use crate::types::{AttributeDataType, CommunicationType};

// ── Helper functions ──

fn cstr_to_str<'a>(s: *const c_char) -> &'a str {
    if s.is_null() {
        return "";
    }
    unsafe { CStr::from_ptr(s) }.to_str().unwrap_or("")
}

fn string_to_cstring(s: &str) -> *mut c_char {
    CString::new(s).unwrap_or_default().into_raw()
}

// ── String management ──

/// Free a string returned by any `pdp_*` function.
/// # Safety
/// The pointer must have been returned by a `pdp_*` function.
#[no_mangle]
pub unsafe extern "C" fn pdp_string_free(s: *mut c_char) {
    if !s.is_null() {
        drop(CString::from_raw(s));
    }
}

// ══════════════════════════════════════════════════════
// ── PdpConnector ──
// ══════════════════════════════════════════════════════

/// Create an anonymous PDP connector (no client certificate).
/// Returns an opaque handle. Free with `pdp_connector_free`.
#[no_mangle]
pub extern "C" fn pdp_connector_create_anonymous(
    pdp_url: *const c_char,
    communication_type: i32,
) -> *mut PdpConnector {
    let url = cstr_to_str(pdp_url);
    let comm_type = match communication_type {
        0 => CommunicationType::XmlSoap,
        1 => CommunicationType::XmlRest,
        2 => CommunicationType::JsonRest,
        _ => CommunicationType::XmlSoap,
    };
    let config = PdpConnectorConfig::anonymous(url, comm_type);
    Box::into_raw(Box::new(PdpConnector::new(config)))
}

/// Create a client SSL PDP connector.
/// Returns an opaque handle. Free with `pdp_connector_free`.
/// Returns null on error (e.g., non-HTTPS URL).
#[no_mangle]
pub extern "C" fn pdp_connector_create_client_ssl(
    pdp_url: *const c_char,
    communication_type: i32,
    client_cert_pem: *const c_char,
    client_key_pem: *const c_char,
) -> *mut PdpConnector {
    let url = cstr_to_str(pdp_url);
    let cert = cstr_to_str(client_cert_pem);
    let key = cstr_to_str(client_key_pem);
    let comm_type = match communication_type {
        0 => CommunicationType::XmlSoap,
        1 => CommunicationType::XmlRest,
        2 => CommunicationType::JsonRest,
        _ => CommunicationType::XmlSoap,
    };

    match PdpConnectorConfig::client_ssl(url, comm_type, cert, key) {
        Ok(config) => Box::into_raw(Box::new(PdpConnector::new(config))),
        Err(_) => ptr::null_mut(),
    }
}

/// Free a PDP connector.
/// # Safety
/// The pointer must have been returned by a `pdp_connector_create_*` function.
#[no_mangle]
pub unsafe extern "C" fn pdp_connector_free(connector: *mut PdpConnector) {
    if !connector.is_null() {
        drop(Box::from_raw(connector));
    }
}

/// Register an obligation that the application recognizes.
#[no_mangle]
pub extern "C" fn pdp_connector_register_obligation(
    connector: *mut PdpConnector,
    obligation_id: *const c_char,
) {
    if connector.is_null() {
        return;
    }
    let conn = unsafe { &mut *connector };
    conn.register_obligation(cstr_to_str(obligation_id));
}

/// Set whether to accept invalid server certificates.
#[no_mangle]
pub extern "C" fn pdp_connector_set_accept_invalid_certs(
    connector: *mut PdpConnector,
    accept: i32,
) {
    if connector.is_null() {
        return;
    }
    let conn = unsafe { &mut *connector };
    conn.config_mut().accept_invalid_certs = accept != 0;
}

// ══════════════════════════════════════════════════════
// ── AuthorizationRequest ──
// ══════════════════════════════════════════════════════

/// Create a new authorization request.
/// Free with `pdp_request_free`.
#[no_mangle]
pub extern "C" fn pdp_request_create() -> *mut AuthorizationRequest {
    Box::into_raw(Box::new(AuthorizationRequest::new()))
}

/// Create a new authorization request with trace option.
#[no_mangle]
pub extern "C" fn pdp_request_create_with_trace(trace: i32) -> *mut AuthorizationRequest {
    Box::into_raw(Box::new(AuthorizationRequest::with_trace(trace != 0)))
}

/// Create a new authorization request with all options.
#[no_mangle]
pub extern "C" fn pdp_request_create_with_options(
    trace: i32,
    return_policy_id_list: i32,
    combine_policies: i32,
) -> *mut AuthorizationRequest {
    Box::into_raw(Box::new(AuthorizationRequest::with_all_options(
        trace != 0,
        return_policy_id_list != 0,
        combine_policies != 0,
    )))
}

/// Free an authorization request.
/// # Safety
/// The pointer must have been returned by a `pdp_request_create*` function.
#[no_mangle]
pub unsafe extern "C" fn pdp_request_free(request: *mut AuthorizationRequest) {
    if !request.is_null() {
        drop(Box::from_raw(request));
    }
}

/// Add an attribute element to the request.
#[no_mangle]
pub extern "C" fn pdp_request_add_element(
    request: *mut AuthorizationRequest,
    category: *const c_char,
    attribute: *const c_char,
    data_type: *const c_char,
    value: *const c_char,
) {
    if request.is_null() {
        return;
    }
    let req = unsafe { &mut *request };
    req.add_element(
        cstr_to_str(category),
        cstr_to_str(attribute),
        cstr_to_str(data_type),
        cstr_to_str(value),
    );
}

/// Add an attribute element using the typed data type enum.
#[no_mangle]
pub extern "C" fn pdp_request_add_element_typed(
    request: *mut AuthorizationRequest,
    category: *const c_char,
    attribute: *const c_char,
    data_type: i32,
    value: *const c_char,
) {
    if request.is_null() {
        return;
    }
    let req = unsafe { &mut *request };
    let dt = match data_type {
        0 => AttributeDataType::X500Name,
        1 => AttributeDataType::Rfc822Name,
        2 => AttributeDataType::IpAddress,
        3 => AttributeDataType::DnsName,
        4 => AttributeDataType::String,
        5 => AttributeDataType::Boolean,
        6 => AttributeDataType::Integer,
        7 => AttributeDataType::Double,
        8 => AttributeDataType::Time,
        9 => AttributeDataType::Date,
        10 => AttributeDataType::DateTime,
        11 => AttributeDataType::AnyUri,
        12 => AttributeDataType::HexBinary,
        13 => AttributeDataType::Base64Binary,
        14 => AttributeDataType::DayTimeDuration,
        15 => AttributeDataType::YearMonthDuration,
        16 => AttributeDataType::AnyType,
        _ => AttributeDataType::String,
    };
    req.add_element_typed(cstr_to_str(category), cstr_to_str(attribute), dt, cstr_to_str(value));
}

/// Set the Content element for a category.
#[no_mangle]
pub extern "C" fn pdp_request_set_content(
    request: *mut AuthorizationRequest,
    category: *const c_char,
    content: *const c_char,
) {
    if request.is_null() {
        return;
    }
    let req = unsafe { &mut *request };
    req.set_content(cstr_to_str(category), cstr_to_str(content));
}

/// Get the request UID as a string. Free the returned string with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_request_get_uid(request: *const AuthorizationRequest) -> *mut c_char {
    if request.is_null() {
        return string_to_cstring("");
    }
    let req = unsafe { &*request };
    string_to_cstring(&req.uid.to_string())
}

/// Get the JSON representation. Free the returned string with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_request_to_json(
    request: *const AuthorizationRequest,
    indented: i32,
) -> *mut c_char {
    if request.is_null() {
        return string_to_cstring("");
    }
    let req = unsafe { &*request };
    let json = crate::json_builder::build_json_request(req, indented != 0);
    string_to_cstring(&json)
}

/// Get the XML representation. Free the returned string with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_request_to_xml(request: *const AuthorizationRequest) -> *mut c_char {
    if request.is_null() {
        return string_to_cstring("");
    }
    let req = unsafe { &*request };
    let xml = crate::xml_builder::build_xacml_request_xml(req);
    string_to_cstring(&xml)
}

// ══════════════════════════════════════════════════════
// ── MultiRequest ──
// ══════════════════════════════════════════════════════

/// Create a new multi-request. Free with `pdp_multi_request_free`.
#[no_mangle]
pub extern "C" fn pdp_multi_request_create(return_policy_id_list: i32) -> *mut MultiRequest {
    Box::into_raw(Box::new(MultiRequest::new(return_policy_id_list != 0)))
}

/// Free a multi-request.
/// # Safety
/// The pointer must have been returned by `pdp_multi_request_create`.
#[no_mangle]
pub unsafe extern "C" fn pdp_multi_request_free(multi_request: *mut MultiRequest) {
    if !multi_request.is_null() {
        drop(Box::from_raw(multi_request));
    }
}

/// Add a request to the multi-request. Returns 0 on success, -1 on error.
#[no_mangle]
pub extern "C" fn pdp_multi_request_add(
    multi_request: *mut MultiRequest,
    request: *mut AuthorizationRequest,
) -> i32 {
    if multi_request.is_null() || request.is_null() {
        return -1;
    }
    let mr = unsafe { &mut *multi_request };
    let req = unsafe { &mut *request };
    match mr.add_request(req) {
        Ok(()) => 0,
        Err(_) => -1,
    }
}

// ══════════════════════════════════════════════════════
// ── Evaluate ──
// ══════════════════════════════════════════════════════

/// Evaluate a single request. Returns a response handle or null on error.
/// Free the response with `pdp_response_free`.
/// If error_out is not null, it receives a string describing the error (free with `pdp_string_free`).
#[no_mangle]
pub extern "C" fn pdp_connector_evaluate(
    connector: *const PdpConnector,
    request: *const AuthorizationRequest,
    error_out: *mut *mut c_char,
) -> *mut AuthorizationResponse {
    if connector.is_null() || request.is_null() {
        return ptr::null_mut();
    }
    let conn = unsafe { &*connector };
    let req = unsafe { &*request };
    match conn.evaluate(req) {
        Ok(resp) => Box::into_raw(Box::new(resp)),
        Err(e) => {
            if !error_out.is_null() {
                unsafe {
                    *error_out = string_to_cstring(&e.message);
                }
            }
            ptr::null_mut()
        }
    }
}

/// Evaluate a multi-request. Returns a multi-response handle or null on error.
/// Free the response with `pdp_multi_response_free`.
#[no_mangle]
pub extern "C" fn pdp_connector_evaluate_multi(
    connector: *const PdpConnector,
    multi_request: *const MultiRequest,
    error_out: *mut *mut c_char,
) -> *mut MultiResponse {
    if connector.is_null() || multi_request.is_null() {
        return ptr::null_mut();
    }
    let conn = unsafe { &*connector };
    let mr = unsafe { &*multi_request };
    match conn.evaluate_multi(mr) {
        Ok(resp) => Box::into_raw(Box::new(resp)),
        Err(e) => {
            if !error_out.is_null() {
                unsafe {
                    *error_out = string_to_cstring(&e.message);
                }
            }
            ptr::null_mut()
        }
    }
}

// ══════════════════════════════════════════════════════
// ── AuthorizationResponse ──
// ══════════════════════════════════════════════════════

/// Free an authorization response.
/// # Safety
/// The pointer must have been returned by `pdp_connector_evaluate`.
#[no_mangle]
pub unsafe extern "C" fn pdp_response_free(response: *mut AuthorizationResponse) {
    if !response.is_null() {
        drop(Box::from_raw(response));
    }
}

/// Get the deny-biased result (0=Deny, 1=DenyWithObligations, 2=DenyDueToUnrecognized, 3=DenyUnlessAll, 4=Permit).
#[no_mangle]
pub extern "C" fn pdp_response_get_result(response: *const AuthorizationResponse) -> i32 {
    if response.is_null() {
        return 0;
    }
    let resp = unsafe { &*response };
    resp.result() as i32
}

/// Get the XACML status (0=MissingAttribute, 1=Ok, 2=ProcessingError, 3=SyntaxError, 4=NotDefined).
#[no_mangle]
pub extern "C" fn pdp_response_get_xacml_status(response: *const AuthorizationResponse) -> i32 {
    if response.is_null() {
        return 4;
    }
    let resp = unsafe { &*response };
    resp.xacml_status() as i32
}

/// Get the status message. Free with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_response_get_status_message(
    response: *const AuthorizationResponse,
) -> *mut c_char {
    if response.is_null() {
        return string_to_cstring("");
    }
    let resp = unsafe { &*response };
    string_to_cstring(resp.xacml_status_message())
}

/// Get the trace info. Free with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_response_get_trace_info(
    response: *const AuthorizationResponse,
) -> *mut c_char {
    if response.is_null() {
        return string_to_cstring("");
    }
    let resp = unsafe { &*response };
    string_to_cstring(&resp.trace_info)
}

/// Get the request ID from the response. Free with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_response_get_request_id(
    response: *const AuthorizationResponse,
) -> *mut c_char {
    if response.is_null() {
        return string_to_cstring("");
    }
    let resp = unsafe { &*response };
    string_to_cstring(&resp.request_id)
}

/// Get the number of obligations.
#[no_mangle]
pub extern "C" fn pdp_response_get_obligation_count(
    response: *const AuthorizationResponse,
) -> i32 {
    if response.is_null() {
        return 0;
    }
    let resp = unsafe { &*response };
    resp.obligations.len() as i32
}

/// Get the ID of an obligation by index. Free with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_response_get_obligation_id(
    response: *const AuthorizationResponse,
    index: i32,
) -> *mut c_char {
    if response.is_null() {
        return string_to_cstring("");
    }
    let resp = unsafe { &*response };
    if let Some(ob) = resp.obligations.get(index as usize) {
        string_to_cstring(&ob.id)
    } else {
        string_to_cstring("")
    }
}

/// Get the number of advice items.
#[no_mangle]
pub extern "C" fn pdp_response_get_advice_count(response: *const AuthorizationResponse) -> i32 {
    if response.is_null() {
        return 0;
    }
    let resp = unsafe { &*response };
    resp.associated_advice.len() as i32
}

/// Get the full response as a display string. Free with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_response_to_string(response: *const AuthorizationResponse) -> *mut c_char {
    if response.is_null() {
        return string_to_cstring("");
    }
    let resp = unsafe { &*response };
    string_to_cstring(&format!("{}", resp))
}

// ══════════════════════════════════════════════════════
// ── MultiResponse ──
// ══════════════════════════════════════════════════════

/// Free a multi-response.
/// # Safety
/// The pointer must have been returned by `pdp_connector_evaluate_multi`.
#[no_mangle]
pub unsafe extern "C" fn pdp_multi_response_free(response: *mut MultiResponse) {
    if !response.is_null() {
        drop(Box::from_raw(response));
    }
}

/// Get the number of responses in a multi-response.
#[no_mangle]
pub extern "C" fn pdp_multi_response_count(response: *const MultiResponse) -> i32 {
    if response.is_null() {
        return 0;
    }
    let resp = unsafe { &*response };
    resp.responses.len() as i32
}

/// Get a response from a multi-response by index.
/// Returns a pointer into the multi-response (do NOT free individually).
#[no_mangle]
pub extern "C" fn pdp_multi_response_get(
    response: *const MultiResponse,
    index: i32,
) -> *const AuthorizationResponse {
    if response.is_null() {
        return ptr::null();
    }
    let resp = unsafe { &*response };
    if let Some(r) = resp.responses.get(index as usize) {
        r as *const AuthorizationResponse
    } else {
        ptr::null()
    }
}

// ══════════════════════════════════════════════════════
// ── Constants access ──
// ══════════════════════════════════════════════════════

/// Get an attribute category constant by name. Free with `pdp_string_free`.
/// Names: "resource", "action", "environment", "access_subject", "recipient_subject",
///        "intermediary_subject", "codebase", "requesting_machine"
#[no_mangle]
pub extern "C" fn pdp_constant_category(name: *const c_char) -> *mut c_char {
    let n = cstr_to_str(name);
    let val = match n {
        "resource" => crate::constants::attribute_category::RESOURCE,
        "action" => crate::constants::attribute_category::ACTION,
        "environment" => crate::constants::attribute_category::ENVIRONMENT,
        "access_subject" => crate::constants::attribute_category::ACCESS_SUBJECT,
        "recipient_subject" => crate::constants::attribute_category::RECIPIENT_SUBJECT,
        "intermediary_subject" => crate::constants::attribute_category::INTERMEDIARY_SUBJECT,
        "codebase" => crate::constants::attribute_category::CODEBASE,
        "requesting_machine" => crate::constants::attribute_category::REQUESTING_MACHINE,
        _ => "",
    };
    string_to_cstring(val)
}

/// Get an attribute identifier constant. Free with `pdp_string_free`.
/// Category: "subject", "resource", "action", "environment"
/// Name: e.g., "subject_id", "resource_id", "action_id", "current_time"
#[no_mangle]
pub extern "C" fn pdp_constant_attribute(
    category: *const c_char,
    name: *const c_char,
) -> *mut c_char {
    let cat = cstr_to_str(category);
    let n = cstr_to_str(name);
    let val = match (cat, n) {
        ("subject", "subject_id") => crate::constants::subject_attributes::SUBJECT_ID,
        ("subject", "role") => crate::constants::subject_attributes::ROLE,
        ("resource", "resource_id") => crate::constants::resource_attributes::RESOURCE_ID,
        ("action", "action_id") => crate::constants::action_attributes::ACTION_ID,
        ("environment", "current_time") => crate::constants::environment_attributes::CURRENT_TIME,
        ("environment", "current_date") => crate::constants::environment_attributes::CURRENT_DATE,
        _ => "",
    };
    string_to_cstring(val)
}

/// Get a data type constant string. Free with `pdp_string_free`.
/// Names: "string", "boolean", "integer", "double", "anyURI", "date", "dateTime", etc.
#[no_mangle]
pub extern "C" fn pdp_constant_datatype(name: *const c_char) -> *mut c_char {
    let n = cstr_to_str(name);
    let val = match n {
        "string" => crate::constants::data_types::STRING,
        "boolean" => crate::constants::data_types::BOOLEAN,
        "integer" => crate::constants::data_types::INTEGER,
        "double" => crate::constants::data_types::DOUBLE,
        "time" => crate::constants::data_types::TIME,
        "date" => crate::constants::data_types::DATE,
        "dateTime" => crate::constants::data_types::DATE_TIME,
        "anyURI" => crate::constants::data_types::ANY_URI,
        "hexBinary" => crate::constants::data_types::HEX_BINARY,
        "base64Binary" => crate::constants::data_types::BASE64_BINARY,
        "x500Name" => crate::constants::data_types::X500_NAME,
        "rfc822Name" => crate::constants::data_types::RFC822_NAME,
        "ipAddress" => crate::constants::data_types::IP_ADDRESS,
        "dnsName" => crate::constants::data_types::DNS_NAME,
        _ => "",
    };
    string_to_cstring(val)
}

// ══════════════════════════════════════════════════════
// ── Library info ──
// ══════════════════════════════════════════════════════

/// Get the library version. Free with `pdp_string_free`.
#[no_mangle]
pub extern "C" fn pdp_library_version() -> *mut c_char {
    string_to_cstring(env!("CARGO_PKG_VERSION"))
}
