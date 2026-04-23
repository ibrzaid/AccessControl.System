
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ACS.Models.Response
{
    public class BaseResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool Success { get; set; }

        /// <summary>
        /// Response Error Code
        /// </summary>       
        [DefaultValue("0")]
        [JsonPropertyName("error_code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Response Error
        /// </summary>
        [DefaultValue("")]
        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }


        [DefaultValue("")]
        [JsonPropertyName("request_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestId { get; set; }



    }

    public  record BaseResponses(
        [property: JsonPropertyName("success")][property: DefaultValue(true)] bool Success = true,
        [property: JsonPropertyName("message")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Message = null,
        [property: JsonPropertyName("error_code")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ErrorCode = null,
        [property: JsonPropertyName("request_id")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? RequestId = null
        );

}
