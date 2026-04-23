using System.Text.Json.Serialization;

namespace ACS.Models.Response.V1.AuthenticationService.UserLogs
{
    public class UserLogsNetworkResponse
    {
        [JsonPropertyName("ip_list")]
        public List<string>? IpList { get; set; }

        [JsonPropertyName("unique_ips")]
        public int UniqueIps { get; set; }
    }
}
