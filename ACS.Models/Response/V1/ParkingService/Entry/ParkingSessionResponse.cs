using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.ParkingService.Entry
{
    public class ParkingSessionResponse
    {
        [JsonPropertyName("parking_session_id")]
        public long ParkingSessionId { get; set; }

        [JsonPropertyName("entry_plate_code")]
        public string? EntryPlateCode { get; set; }

        [JsonPropertyName("entry_plate_number")]
        public string? EntryPlateNumber { get; set; }

        [JsonPropertyName("entry_full_plate")]
        public string? EntryFullPlate { get; set; }

        [JsonPropertyName("entry_time")]
        public DateTime EntryTime { get; set; }

        [JsonPropertyName("entry_latitude")]
        public decimal? EntryLatitude { get; set; }

        [JsonPropertyName("entry_longitude")]
        public decimal? EntryLongitude { get; set; }

        [JsonPropertyName("entry_anpr_trans_id")]
        public int? EntryAnprTransId { get; set; }

        [JsonPropertyName("entry_camera_capture_url")]
        public string? EntryCameraCaptureUrl { get; set; }

        [JsonPropertyName("qr_code")]
        public string? QrCode { get; set; }

        [JsonPropertyName("qr_code_expiry")]
        public DateTime QrCodeExpiry { get; set; }

        //[JsonPropertyName("is_qr_used")]
        //public bool? IsQrUsed { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("current_full_plate")]
        public string? CurrentFullPlate { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("session_code")]
        public string? SessionCode { get; set; }
    }
}
