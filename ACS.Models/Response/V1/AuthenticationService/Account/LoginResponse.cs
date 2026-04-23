using ACS.Models.Response;
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.Account
{
    public class LoginResponse : BaseResponse
    {
        /// <summary>
        /// JWT Access Token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Token lifetime in seconds
        /// </summary>
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonPropertyName("requires_mfa")]
        public bool RequiresMfa { get; set; }

        [JsonPropertyName("session_id")]
        public Guid? SessionId { get; set; }

        [JsonPropertyName("token_expires_at")]
        public DateTime? TokenExpiresAt { get; set; }

        [JsonPropertyName("refresh_expires_at")]
        public DateTime? RefreshExpiresAt { get; set; }

        // ── Identity ──────────────────────────────────────────────────────────────

        [JsonPropertyName("user")]
        public UserProfileResponse? User { get; set; }

        // ── Role-based permissions (WHAT can the user do) ─────────────────────────
        // Source: tbl_user_role_assignments → tbl_user_role

        [JsonPropertyName("roles")]
        public List<UserRoleResponse> Roles { get; set; } = [];

        /// <summary>
        /// Flat deduplicated permission strings across all roles.
        /// e.g. ["anpr.view", "parking.manage", "blacklist.view"]
        /// </summary>
        [JsonPropertyName("permissions")]
        public List<string> Permissions { get; set; } = [];

        // ── Access scope (WHERE can the user go) ──────────────────────────────────
        // Source: tbl_user_access + tbl_user_access_level

        /// <summary>
        /// Highest scope level from tbl_user_access.
        /// 4 = WORKSPACE | 3 = PROJECT | 2 = AREA | 1 = ZONE | 5 = GATE
        /// </summary>
        [JsonPropertyName("scope_type_id")]
        public int ScopeTypeId { get; set; } = 1;

        /// <summary>
        /// Access level from tbl_user_access_level.
        /// VIEW_ONLY | OPERATOR | SUPERVISOR | ADMIN
        /// </summary>
        [JsonPropertyName("access_level")]
        public string AccessLevel { get; set; } = "VIEW_ONLY";

        /// <summary>
        /// True when ScopeTypeId == 4 (workspace admin).
        /// Controls admin UI visibility and SignalR admins_{wid} group membership.
        /// </summary>
        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Project IDs this user can access.
        /// For workspace admins this is ALL projects in the workspace.
        /// Use to scope every data query: WHERE project_id = ANY(@ProjectIds)
        /// </summary>
        [JsonPropertyName("project_ids")]
        public List<int> ProjectIds { get; set; } = [];

        // ── Error helpers ─────────────────────────────────────────────────────────

        [JsonPropertyName("failed_attempts")]
        public int? FailedAttempts { get; set; }

        [JsonPropertyName("remaining_attempts")]
        public int? RemainingAttempts { get; set; }

        // ── Client ────────────────────────────────────────────────────────────────

        [JsonPropertyName("client")]
        public ClientResponse? Client { get; set; }
    }
}
