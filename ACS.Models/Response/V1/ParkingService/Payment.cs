using System.Text.Json.Serialization;
using ACS.Models.Response.V1.ParkingService.Session;


#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ACS.Models.Response.V1.ParkingService.Payment
{
    public record ProcessPaymentResponse(
        bool Success,
        string Message,
        string ErrorCode,
        string RequestId,
       [property: JsonPropertyName("data")] PaymentDataResponse? Data,
       [property: JsonPropertyName("metadata")] PaymentMetadataResponse? Metadata,
       [property: JsonPropertyName("session_id")] int? SessionId = null,
       [property: JsonPropertyName("error_detail")] string? ErrorDetail = null,
       [property: JsonPropertyName("timestamp")] DateTime? Timestamp = null
   ) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record PaymentDataResponse(
        [property: JsonPropertyName("transaction")] TransactionResponse Transaction,
        [property: JsonPropertyName("session")] PaymentSessionResponse Session,
        [property: JsonPropertyName("financial")] FinancialResponse Financial,
        [property: JsonPropertyName("documents")] DocumentsResponse Documents
    );

    public record TransactionResponse(
        [property: JsonPropertyName("id")] long? Id,
        [property: JsonPropertyName("reference")] string Reference,
        [property: JsonPropertyName("timestamp")] DateTime Timestamp,
        [property: JsonPropertyName("method")] string Method,
        [property: JsonPropertyName("amount")] decimal Amount
    );

    public record PaymentSessionResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("vehicle")] string Vehicle,
        [property: JsonPropertyName("duration_minutes")] int DurationMinutes,
        [property: JsonPropertyName("entry_time")] DateTime EntryTime,
        [property: JsonPropertyName("exit_time")] DateTime ExitTime,
        [property: JsonPropertyName("status")] string Status
    );

    public record FinancialResponse(
        [property: JsonPropertyName("net")] decimal Net,
        [property: JsonPropertyName("vat")] decimal Vat,
        [property: JsonPropertyName("gross")] decimal Gross,
        [property: JsonPropertyName("vat_percent")] decimal VatPercent
    );

    public record DocumentsResponse(
        [property: JsonPropertyName("invoice_id")] long? InvoiceId,
        [property: JsonPropertyName("invoice_number")] string? InvoiceNumber,
        [property: JsonPropertyName("fee_id")] long? FeeId,
        [property: JsonPropertyName("invoice_details")] InvoiceDetailResponse? InvoiceDetails,
        [property: JsonPropertyName("existing_record_used")] bool ExistingRecordUsed
    );

    public record PaymentMetadataResponse(
        [property: JsonPropertyName("processing_time_ms")] decimal ProcessingTimeMs,
        [property: JsonPropertyName("audit_id")] int AuditId,
        [property: JsonPropertyName("request_id")] string? RequestId
    );

    // Error response details (for when success = false)
    public record PaymentErrorDetails(
        [property: JsonPropertyName("provided_amounts")] ProvidedAmountsResponse? ProvidedAmounts,
        [property: JsonPropertyName("calculated_amounts")] CalculatedAmountsResponse? CalculatedAmounts,
        [property: JsonPropertyName("session_status")] string? SessionStatus,
        [property: JsonPropertyName("session_id")] int? SessionId
    );

    public record ProvidedAmountsResponse(
        [property: JsonPropertyName("net")] decimal Net,
        [property: JsonPropertyName("vat")] decimal Vat,
        [property: JsonPropertyName("gross")] decimal Gross
    );

    public record CalculatedAmountsResponse(
        [property: JsonPropertyName("net")] decimal Net,
        [property: JsonPropertyName("vat")] decimal Vat,
        [property: JsonPropertyName("gross")] decimal Gross
    );
}
#pragma warning restore IDE0130 // Namespace does not match folder structure