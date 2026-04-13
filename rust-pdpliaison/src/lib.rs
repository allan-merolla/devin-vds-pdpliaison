//! # PDP Liaison
//!
//! A cross-language XACML 3.0 PDP (Policy Decision Point) client library.
//!
//! This library provides functionality to:
//! - Build XACML authorization requests (single and multi-request)
//! - Serialize requests to XML (SOAP/SAML) and JSON (REST) formats
//! - Send requests to a PDP server over HTTP/HTTPS
//! - Parse authorization responses with deny-biased result interpretation
//! - Handle obligations and advice from PDP responses
//!
//! The library exposes a C FFI layer for consumption from PHP, .NET, Java, and other languages.

pub mod types;
pub mod constants;
pub mod error;
pub mod request;
pub mod response;
pub mod connector;
pub mod xml_builder;
pub mod json_builder;
pub mod ffi;

pub use types::*;
pub use constants::*;
pub use error::*;
pub use request::*;
pub use response::*;
pub use connector::*;
