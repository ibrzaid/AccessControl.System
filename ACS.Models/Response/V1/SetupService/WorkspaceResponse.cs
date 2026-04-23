
using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.SetupService
{
    public class WorkspaceResponse
    {
        [JsonPropertyName("workspace_id")]
        public int WorkspaceId { get; set; }

        [JsonPropertyName("workspace_code")]
        public string? WorkspaceCode { get; set; }

        [JsonPropertyName("workspace_name")]
        public string? WorkspaceName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("license_status")]
        public string? LicenseStatus { get; set; }

        [JsonPropertyName("max_hardware")]
        public int? MaxHardware { get; set; }

        [JsonPropertyName("max_users")]
        public int? MaxUsers { get; set; }

        [JsonPropertyName("max_parking_spots")]
        public int? MaxParkingSpots { get; set; }

        [JsonPropertyName("max_vehicle_records")]
        public int? MaxVehicleRecords { get; set; }

        [JsonPropertyName("current_hardware")]
        public int? CurrentHardware { get; set; }

        [JsonPropertyName("current_users")]
        public int? CurrentUsers { get; set; }

        [JsonPropertyName("current_parking_spots")]
        public int? CurrentParkingSpots { get; set; }

        [JsonPropertyName("current_vehicle_records")]
        public int? CurrentVehicleRecords { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("contract_start_date")]
        public DateTime? ContractStartDate { get; set; }

        [JsonPropertyName("contract_end_date")]
        public DateTime? ContractEndDate { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
