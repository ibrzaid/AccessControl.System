using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Payment;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Parking.WebService.Models.Request.V1;

namespace ACS.Parking.WebService.Services.V1.Interfaces
{
    public interface ISessionService
    {
        Task<EntrySessionResponse> CreateEntrySessionAsync(string workspace, string project, string projectArea, string zone, string accessPoint, string userSession, string createdBy, CreateSessionRequest request, 
            string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default);
        Task<EntrySessionResponse> UpdateParkingSessionStatusAsync(long id, string status, int? cancellationReason, int? exitAccessPointId, string workspace, CancellationToken cancellationToken = default);
        Task<EntrySessionResponse> DeleteParkingSessionAsync(long id, string workspace, CancellationToken cancellationToken = default);
        Task<ValidateSessionResponse> ValidateSessionBySessionIdAsync(string workspace, string project, string projectArea, string zone, string accessPoint,
             string sessionCode, string sessionId, string plateCode, string plateNumber, string country, string state, string category, string userId, double latitude, double longitude, string? ipAddress,
             string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default);

        Task<ProcessPaymentResponse> PaymentSessionAsync(string workspace, string project, string projectArea, string zone, string accessPoint, string userSession,
            string createdBy, PaymentSessionRequest request, int sessionId, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default);
    }
}
