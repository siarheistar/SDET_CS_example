using RestSharp;

namespace SDET.Tests.Interfaces;

/// <summary>
/// Interface for API-based tests using RestSharp.
/// Extends ITest with API-specific capabilities.
/// SOLID: Interface Segregation - API tests get only API-related methods
/// </summary>
public interface IAPITest : ITest
{
    /// <summary>
    /// Gets the REST API client
    /// </summary>
    RestClient ApiClient { get; }

    /// <summary>
    /// Gets the base URL for API requests
    /// </summary>
    string BaseUrl { get; }

    /// <summary>
    /// Sends a GET request to the specified endpoint
    /// </summary>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="parameters">Optional query parameters</param>
    Task<RestResponse> GetAsync(string endpoint, Dictionary<string, string>? parameters = null);

    /// <summary>
    /// Sends a POST request to the specified endpoint
    /// </summary>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="body">Request body</param>
    Task<RestResponse> PostAsync(string endpoint, object body);

    /// <summary>
    /// Sends a PUT request to the specified endpoint
    /// </summary>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="body">Request body</param>
    Task<RestResponse> PutAsync(string endpoint, object body);

    /// <summary>
    /// Sends a DELETE request to the specified endpoint
    /// </summary>
    /// <param name="endpoint">API endpoint</param>
    Task<RestResponse> DeleteAsync(string endpoint);

    /// <summary>
    /// Validates response status code
    /// </summary>
    /// <param name="response">REST response</param>
    /// <param name="expectedStatusCode">Expected HTTP status code</param>
    void ValidateStatusCode(RestResponse response, System.Net.HttpStatusCode expectedStatusCode);

    /// <summary>
    /// Validates response JSON schema
    /// </summary>
    /// <param name="response">REST response</param>
    /// <param name="schemaPath">Path to JSON schema file</param>
    Task ValidateJsonSchema(RestResponse response, string schemaPath);
}
