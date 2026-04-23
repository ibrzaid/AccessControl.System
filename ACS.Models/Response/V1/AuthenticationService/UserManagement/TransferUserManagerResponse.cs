

using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserManagement
{
    public class TransferUserManagerResponse : BaseResponse
    {

        [JsonPropertyName("managed_user_id")]
        public long? ManagedUserId { get; set; }


        [JsonPropertyName("new_manager_id")]
        public long? NewManagedId { get; set; }


        [JsonPropertyName("new_manager_name")]
        public string? NewManagedName { get; set; }
    }
}
