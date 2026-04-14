/**
 * PDP Liaison - C FFI Header
 *
 * Cross-language common library for communicating with a Policy Decision Point (PDP).
 * Supports XACML 3.0 over XML/SOAP, XML/REST, and JSON/REST protocols.
 *
 * Usage from different languages:
 *   PHP:           $ffi = FFI::cdef(file_get_contents("pdp_liaison.h"), "libpdp_liaison.so");
 *   .NET Framework: [DllImport("pdp_liaison")] static extern IntPtr pdp_connector_create_anonymous(...);
 *   .NET 8:        [LibraryImport("pdp_liaison")] static partial IntPtr pdp_connector_create_anonymous(...);
 *   Java (JNA):    Native.load("pdp_liaison", PdpLiaison.class);
 *
 * Memory management:
 *   - All *_create functions return opaque pointers that MUST be freed with the corresponding *_free function
 *   - Strings returned by *_to_string / *_get_* functions MUST be freed with pdp_string_free()
 */

#ifndef PDP_LIAISON_H
#define PDP_LIAISON_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

/* ── Opaque handle types ── */
typedef void PdpConnector;
typedef void AuthorizationRequest;
typedef void MultiRequest;
typedef void AuthorizationResponse;
typedef void MultiResponse;

/* ── Communication types ── */
/* 0 = XmlSoap, 1 = XmlRest, 2 = JsonRest */

/* ── String management ── */
void pdp_string_free(char *s);

/* ── Library info ── */
char *pdp_library_version(void);

/* ── PdpConnector ── */
PdpConnector *pdp_connector_create_anonymous(const char *pdp_url, int communication_type);
PdpConnector *pdp_connector_create_client_ssl(const char *pdp_url, int communication_type,
                                               const char *client_cert_pem, const char *client_key_pem);
void pdp_connector_free(PdpConnector *connector);
void pdp_connector_register_obligation(PdpConnector *connector, const char *obligation_id);
void pdp_connector_set_accept_invalid_certs(PdpConnector *connector, int accept);

/* ── AuthorizationRequest ── */
AuthorizationRequest *pdp_request_create(void);
AuthorizationRequest *pdp_request_create_with_trace(int trace);
AuthorizationRequest *pdp_request_create_with_options(int trace, int return_policy_id_list, int combine_policies);
void pdp_request_free(AuthorizationRequest *request);
void pdp_request_add_element(AuthorizationRequest *request,
                              const char *category, const char *attribute_id,
                              const char *data_type, const char *value);
void pdp_request_add_element_typed(AuthorizationRequest *request,
                                    const char *category, const char *attribute_id,
                                    int data_type_enum, const char *value);
void pdp_request_set_content(AuthorizationRequest *request,
                              const char *category, const char *content);
char *pdp_request_get_uid(const AuthorizationRequest *request);
char *pdp_request_to_json(const AuthorizationRequest *request, int indented);
char *pdp_request_to_xml(const AuthorizationRequest *request);

/* ── MultiRequest ── */
MultiRequest *pdp_multi_request_create(int return_policy_id_list);
void pdp_multi_request_free(MultiRequest *multi_request);
void pdp_multi_request_add(MultiRequest *multi_request, AuthorizationRequest *request);

/* ── Evaluation ── */
AuthorizationResponse *pdp_connector_evaluate(const PdpConnector *connector,
                                               const AuthorizationRequest *request,
                                               char **error_out);
MultiResponse *pdp_connector_evaluate_multi(const PdpConnector *connector,
                                             const MultiRequest *multi_request,
                                             char **error_out);

/* ── AuthorizationResponse ── */
void pdp_response_free(AuthorizationResponse *response);
int  pdp_response_get_result(const AuthorizationResponse *response);
int  pdp_response_get_xacml_status(const AuthorizationResponse *response);
char *pdp_response_get_status_message(const AuthorizationResponse *response);
char *pdp_response_get_trace_info(const AuthorizationResponse *response);
char *pdp_response_get_request_id(const AuthorizationResponse *response);
int  pdp_response_get_obligation_count(const AuthorizationResponse *response);
char *pdp_response_get_obligation_id(const AuthorizationResponse *response, int index);
int  pdp_response_get_advice_count(const AuthorizationResponse *response);
char *pdp_response_to_string(const AuthorizationResponse *response);

/* ── MultiResponse ── */
void pdp_multi_response_free(MultiResponse *multi_response);
int  pdp_multi_response_count(const MultiResponse *multi_response);
const AuthorizationResponse *pdp_multi_response_get(const MultiResponse *multi_response, int index);

/* ── Constants ── */
char *pdp_constant_category(const char *name);
char *pdp_constant_attribute(const char *category, const char *name);
char *pdp_constant_datatype(const char *name);

/* ── Result enum values ── */
/* pdp_response_get_result() returns: */
/*   0 = Deny                                */
/*   1 = DenyWithObligations                 */
/*   2 = DenyDueToUnrecognizedObligations    */
/*   3 = DenyUnlessAllObligationsSatisfied   */
/*   4 = Permit                              */

/* ── XacmlStatus enum values ── */
/* pdp_response_get_xacml_status() returns: */
/*   0 = MissingAttribute  */
/*   1 = Ok                */
/*   2 = ProcessingError   */
/*   3 = SyntaxError       */
/*   4 = NotDefined        */

#ifdef __cplusplus
}
#endif

#endif /* PDP_LIAISON_H */
