using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.Account
{
    public class ClientResponse
    {
        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }

        [JsonPropertyName("client_name")]
        public string? ClientName { get; set; }

        [JsonPropertyName("redirect_uris")]
        public string[]? RedirectUris { get; set; }

        [JsonPropertyName("allowed_scopes")]
        public string[]? AllowedScopes { get; set; }
    }
}
