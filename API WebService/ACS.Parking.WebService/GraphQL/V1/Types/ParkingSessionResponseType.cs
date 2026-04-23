using GraphQL.Types;
using ACS.Models.Response.V1.ParkingService.Entry;

namespace ACS.Parking.WebService.GraphQL.V1.Types
{
    public class ParkingSessionResponseType : ObjectGraphType<ParkingSessionResponse>
    {
        public ParkingSessionResponseType()
        {
            Name = "ParkingSessionResponse";
            Description = "Parking session details";

            Field(x => x.ParkingSessionId).Description("Session ID");
            Field(x => x.EntryPlateCode, nullable: true).Description("Entry plate code");
            Field(x => x.EntryPlateNumber, nullable: true).Description("Entry plate number");
            Field(x => x.EntryFullPlate, nullable: true).Description("Full plate number");
            Field(x => x.EntryTime).Description("Entry time");
            Field(x => x.EntryLatitude, nullable: true).Description("Entry latitude");
            Field(x => x.EntryLongitude, nullable: true).Description("Entry longitude");
            Field(x => x.EntryAnprTransId, nullable: true).Description("ANPR transaction ID");
            Field(x => x.EntryCameraCaptureUrl, nullable: true).Description("Camera capture URL");
            Field(x => x.QrCode, nullable: true).Description("QR code");
            Field(x => x.QrCodeExpiry).Description("QR code expiry");
            Field(x => x.Status, nullable: true).Description("Session status");
            Field(x => x.CurrentFullPlate, nullable: true).Description("Current full plate");
            Field(x => x.IsActive).Description("Is active");
            Field(x => x.CreatedAt).Description("Created at");
            Field(x => x.UpdatedAt).Description("Updated at");
        }
    }
}
