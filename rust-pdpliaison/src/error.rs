//! Error types for the PDP Liaison library.

use std::fmt;

/// The category of error.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
#[repr(C)]
pub enum PdpErrorKind {
    /// General error.
    General = 0,
    /// Connection error (network, HTTP).
    Connection = 1,
    /// Security error (certificate, TLS).
    Security = 2,
    /// Server authentication error (signature verification, certificate trust).
    ServerAuthentication = 3,
    /// Client authentication error (signing, credentials).
    ClientAuthentication = 4,
}

/// Error type for PDP Liaison operations.
#[derive(Debug, Clone)]
pub struct PdpError {
    pub kind: PdpErrorKind,
    pub message: String,
}

impl PdpError {
    pub fn new(kind: PdpErrorKind, message: impl Into<String>) -> Self {
        Self {
            kind,
            message: message.into(),
        }
    }

    pub fn general(message: impl Into<String>) -> Self {
        Self::new(PdpErrorKind::General, message)
    }

    pub fn connection(message: impl Into<String>) -> Self {
        Self::new(PdpErrorKind::Connection, message)
    }

    pub fn security(message: impl Into<String>) -> Self {
        Self::new(PdpErrorKind::Security, message)
    }

    pub fn server_auth(message: impl Into<String>) -> Self {
        Self::new(PdpErrorKind::ServerAuthentication, message)
    }

    pub fn client_auth(message: impl Into<String>) -> Self {
        Self::new(PdpErrorKind::ClientAuthentication, message)
    }
}

impl fmt::Display for PdpError {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{:?}: {}", self.kind, self.message)
    }
}

impl std::error::Error for PdpError {}

impl From<std::io::Error> for PdpError {
    fn from(e: std::io::Error) -> Self {
        PdpError::connection(format!("IO error: {}", e))
    }
}

impl From<reqwest::Error> for PdpError {
    fn from(e: reqwest::Error) -> Self {
        PdpError::connection(format!("HTTP error: {}", e))
    }
}

impl From<quick_xml::Error> for PdpError {
    fn from(e: quick_xml::Error) -> Self {
        PdpError::general(format!("XML error: {}", e))
    }
}

impl From<serde_json::Error> for PdpError {
    fn from(e: serde_json::Error) -> Self {
        PdpError::general(format!("JSON error: {}", e))
    }
}
