using ACS.License.V1;
using System.Globalization;
using ACS.Service.V1.Interfaces;
using ACS.Models.Request.V1.ANPRService;
using ACS.Database.IDataAccess.ANPRService.V1;
using ACS.Models.Response.V1.ANPRService.Anpr;
using ACS.Models.Response.V1.ANPRService.Seach;
using ACS.ANPR.WebService.Services.V1.Interfaces;

namespace ACS.ANPR.WebService.Services.V1.Services
{
    public class AnprService(ILicenseManager licenseManager, IMinioService minioService,  INotificationService notificationService) : Service.Service(licenseManager), IAnprService
    {
        private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".webp"];
        const int maxFileSize = 10 * 1024 * 1024;
        private IAnprDataAcess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ANPRService.V1.AnprDataAcess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in AnprService.")
                };
            }
        }

        public async Task<AnprInsertResponse> InsertAync(AnprInsertRequest request, string workspaceId, int projectId, int projectAreaId, int zoneId, int accessPointId, int hardwareId, string userIp, string userAgent, string requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            string vehicleImagePath= string.Empty;
            string plateImagePath = string.Empty;


            if (!double.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude  = 0;
            if (!double.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;


            if (!double.TryParse(request.CaptureLatitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _captureLatitude)) _captureLatitude  = 0;
            if (!double.TryParse(request.CaptureLongitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _captureLongitude)) _captureLongitude = 0;

            if (!IsValidImageFile(request.VehicleImage))
            {
                return new(
                    Success: false,
                    Message: "Invalid vehicle image. File must be JPG, PNG, GIF, BMP, or WEBP and less than 10MB",
                    ErrorCode: "UNSUPPORTED_EXTENSION",
                    RequestId: requestId,
                    Errors: null,
                    Warnings: null,
                    Data: null,
                    Metadata: null);
            }
            if (!IsValidImageFile(request.PlateCrop))
            {
                return new(
                    Success: false,
                    Message: "Invalid plate image. File must be JPG, PNG, GIF, BMP, or WEBP and less than 10MB",
                    ErrorCode: "UNSUPPORTED_EXTENSION",
                    RequestId: requestId,
                    Errors: null,
                    Warnings: null,
                    Data: null,
                    Metadata: null);
            }
            var shortId = requestId.Length >= 8 ? requestId[..8] : requestId;
            string name = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{shortId}";


            if (request.VehicleImage != null)
            {
                var ext = Path.GetExtension(request.VehicleImage.FileName).ToLowerInvariant();
                var filename = $"VEHICLE_{name}{ext}";
                using var carStream = request.VehicleImage.OpenReadStream();
                vehicleImagePath = await minioService.UploadImageAsync(carStream,
                    workspaceId,
                    projectId.ToString(),
                    projectAreaId.ToString(),
                    zoneId.ToString(),
                    accessPointId.ToString(),
                    hardwareId.ToString(),
                    filename,
                     request.VehicleImage.ContentType);
            }


            if (request.PlateCrop != null)
            {
                var ext = Path.GetExtension(request.PlateCrop.FileName).ToLowerInvariant();
                var filename = $"PLATE_{name}{ext}";
                using var carStream = request.PlateCrop.OpenReadStream();
                plateImagePath = await minioService.UploadImageAsync(carStream,
                    workspaceId,
                    projectId.ToString(),
                    projectAreaId.ToString(),
                    zoneId.ToString(),
                    accessPointId.ToString(),
                    hardwareId.ToString(),
                    filename,
                     request.PlateCrop.ContentType);
            }
            var result = await this[license?.DB!].InsertDetectionAsync(workspaceId: workspaceId,
                projectId: projectId,
                projectAreaId: projectAreaId,
                zoneId: zoneId,
                accessPointId: accessPointId,
                hardwareId: hardwareId,
                countryId: request.CountryId,
                plateStateId: request.PlateStateId,
                plateCategoryId: request.PlateCategoryId,
                plateCode: request.PlateCode!,
                plateNumber: request.PlateNumber!,
                confidenceScore: request.ConfidenceScore,
                detectionTime: request.DetectionTime,
                vehicleColor: request.VehicleColor,
                vehicleMake: request.VehicleMake,
                vehicleModel: request.VehicleModel,
                vehicleType: request.VehicleType,
                imageUrl: vehicleImagePath,
                imageStoragePath: vehicleImagePath,
                plateCropUrl: plateImagePath,
                vehicleSpeedKmh: request.VehicleSpeedKmh,
                processingTimeMs: request.ProcessingTimeMs,
                rawData: request.RawData,
                latitude: _captureLatitude,
                longitude: _captureLongitude,
                anprEngine: request.AnprEngine,
                direction: request.Direction,
                createdBy: null,
                userIp: userIp,
                userAgent: userAgent,
                requestId: requestId,
                latitudeGps: _latitude,
                longitudeGps: _longitude,
                cancellationToken: cancellationToken);

            if(result.Success && result.Data != null && result.Data.SignalrPayload != null)
            {
                var ev = result.Data.SignalrPayload.Event ?? "detection_recorded";
                var workspace = result.Data.SignalrPayload.WorkspaceId?.ToString()!;
                var project = result.Data.SignalrPayload.ProjectId;
                var data = result.Data.SignalrPayload.Data;
                if (project.HasValue && data != null) 
                    notificationService.SendToProjectGroup(ev, workspace, project.Value.ToString(), data);
            }
                


            return new(
                   Success: result.Success,
                   Message: result.Message,
                   ErrorCode: result.ErrorCod,
                   RequestId: requestId,
                   Errors: result.Errors,
                   Warnings: result.Warnings,
                   Data: result.Data != null ? new(
                       DetectionId: result.Data.DetectionId,
                       WorkspaceId: result.Data.WorkspaceId,
                       Plate: result.Data.Plate,
                       DetectionTime: result.Data.DetectionTime,
                       Confidence: result.Data.Confidence,
                       Coordinates: result.Data.Coordinates != null ? new(
                           Lat: result.Data.Coordinates.Lat,
                           Lng: result.Data.Coordinates.Lng
                           ) : null,
                       Warnings: result.Data.Warnings,
                       Detection: result.Data.Detection == null ? null : new(
                            DetectionId: result.Data.Detection.DetectionId,
                            DetectionTime: result.Data.Detection.DetectionTime,
                            DetectionTimeUtc: result.Data.Detection.DetectionTimeUtc,
                            AgeSeconds: result.Data.Detection.AgeSeconds,
                            PlateCode: result.Data.Detection.PlateCode,
                            PlateNumber: result.Data.Detection.PlateNumber,
                            FullPlate: result.Data.Detection.FullPlate,
                            ConfidenceScore: result.Data.Detection.ConfidenceScore,
                            ConfidenceDetails: result.Data.Detection.ConfidenceDetails,
                            ImageUrl: result.Data.Detection.ImageUrl,
                            PlateCropUrl: result.Data.Detection.PlateCropUrl,
                            VehicleColor: result.Data.Detection.VehicleColor,
                            VehicleMake: result.Data.Detection.VehicleMake,
                            VehicleModel: result.Data.Detection.VehicleModel,
                            VehicleType: result.Data.Detection.VehicleType,
                            VehicleSpeedKmh: result.Data.Detection.VehicleSpeedKmh,
                            Direction: result.Data.Detection.Direction,
                            LaneNumber: result.Data.Detection.LaneNumber,
                            Latitude: result.Data.Detection.Latitude,
                            Longitude: result.Data.Detection.Longitude,
                            AnprEngine: result.Data.Detection.AnprEngine,
                            ProcessingTimeMs: result.Data.Detection.ProcessingTimeMs,
                            IsValidated: result.Data.Detection.IsValidated,
                            ValidatedAt: result.Data.Detection.ValidatedAt,
                            ValidatedAtUtc: result.Data.Detection.ValidatedAtUtc,
                            ValidatedBy: result.Data.Detection.ValidatedBy,
                            ValidationNotes: result.Data.Detection.ValidationNotes,
                            Tags: result.Data.Detection.Tags,
                            CreatedAt: result.Data.Detection.CreatedAt,
                            IsBlacklisted: result.Data.Detection.IsBlacklisted,
                            BlacklistDetails: result.Data.Detection.BlacklistDetails is null ? null : new AnprSearchBlacklistResponse(
                                ListType: result.Data.Detection.BlacklistDetails.ListType,
                                Reason: result.Data.Detection.BlacklistDetails.Reason,
                                Severity: result.Data.Detection.BlacklistDetails.Severity,
                                ActionOnDetect: result.Data.Detection.BlacklistDetails.ActionOnDetect,
                                ReferenceNumber: result.Data.Detection.BlacklistDetails.ReferenceNumber),
                            Hardware: result.Data.Detection.Hardware is null ? null : new AnprSearchHardwareResponse(
                                Id: result.Data.Detection.Hardware.Id,
                                SerialNumber: result.Data.Detection.Hardware.SerialNumber,
                                Manufacturer: result.Data.Detection.Hardware.Manufacturer,
                                Model: result.Data.Detection.Hardware.Model,
                                HardwareType: result.Data.Detection.Hardware.HardwareType,
                                Status: result.Data.Detection.Hardware.Status),
                            AccessPoint: result.Data.Detection.AccessPoint is null ? null : new AnprSearchAccessPointResponse(
                                Id: result.Data.Detection.AccessPoint.Id,
                                Name: result.Data.Detection.AccessPoint.Name,
                                Prefix: result.Data.Detection.AccessPoint.Prefix,
                                Latitude: result.Data.Detection.AccessPoint.Latitude,
                                Longitude: result.Data.Detection.AccessPoint.Longitude,
                                Type: result.Data.Detection.AccessPoint.Type,
                                Orientation: result.Data.Detection.AccessPoint.Orientation),
                            Project: result.Data.Detection.Project is null ? null : new AnprSearchNamedRefResponse(result.Data.Detection.Project.Id, result.Data.Detection.Project.Name),
                            Area: result.Data.Detection.Area is null ? null : new AnprSearchNamedRefResponse(result.Data.Detection.Area.Id, result.Data.Detection.Area.Name),
                            Zone: result.Data.Detection.Zone is null ? null : new AnprSearchZoneResponse(result.Data.Detection.Zone.Id, result.Data.Detection.Zone.Code, result.Data.Detection.Zone.Name),
                            Country: result.Data.Detection.Country is null ? null : new AnprSearchCountryResponse(result.Data.Detection.Country.Id, result.Data.Detection.Country.Code, result.Data.Detection.Country.Name),
                            State: result.Data.Detection.State is null ? null : new AnprSearchNamedRefResponse(result.Data.Detection.State.Id, result.Data.Detection.State.Name),
                            Category: result.Data.Detection.Category is null ? null : new AnprSearchNamedRefResponse(result.Data.Detection.Category.Id, result.Data.Detection.Category.Name),
                            ValidatedByUser: result.Data.Detection.ValidatedByUser is null ? null : new AnprSearchUserResponse(
                                Id: result.Data.Detection.ValidatedByUser.Id,
                                Username: result.Data.Detection.ValidatedByUser.Username,
                                FullName: result.Data.Detection.ValidatedByUser.FullName))
                           ,
                       SignalrPayload: result.Data.SignalrPayload == null ? null : new (
                           Channel: result.Data.SignalrPayload.Channel,
                           Event: result.Data.SignalrPayload.Event,
                           WorkspaceId: result.Data.SignalrPayload.WorkspaceId,
                           ProjectId: result.Data.SignalrPayload.ProjectId,
                           Timestamp: result.Data.SignalrPayload.Timestamp,
                           Data: result.Data.SignalrPayload.Data== null ? null : new(
                            DetectionId: result.Data.SignalrPayload.Data.DetectionId,
                            DetectionTime: result.Data.SignalrPayload.Data.DetectionTime,
                            DetectionTimeUtc: result.Data.SignalrPayload.Data.DetectionTimeUtc,
                            AgeSeconds: result.Data.SignalrPayload.Data.AgeSeconds,
                            PlateCode: result.Data.SignalrPayload.Data.PlateCode,
                            PlateNumber: result.Data.SignalrPayload.Data.PlateNumber,
                            FullPlate: result.Data.SignalrPayload.Data.FullPlate,
                            ConfidenceScore: result.Data.SignalrPayload.Data.ConfidenceScore,
                            ConfidenceDetails: result.Data.SignalrPayload.Data.ConfidenceDetails,
                            ImageUrl: result.Data.SignalrPayload.Data.ImageUrl,
                            PlateCropUrl: result.Data.SignalrPayload.Data.PlateCropUrl,
                            VehicleColor: result.Data.SignalrPayload.Data.VehicleColor,
                            VehicleMake: result.Data.SignalrPayload.Data.VehicleMake,
                            VehicleModel: result.Data.SignalrPayload.Data.VehicleModel,
                            VehicleType: result.Data.SignalrPayload.Data.VehicleType,
                            VehicleSpeedKmh: result.Data.SignalrPayload.Data.VehicleSpeedKmh,
                            Direction: result.Data.SignalrPayload.Data.Direction,
                            LaneNumber: result.Data.SignalrPayload.Data.LaneNumber,
                            Latitude: result.Data.SignalrPayload.Data.Latitude,
                            Longitude: result.Data.SignalrPayload.Data.Longitude,
                            AnprEngine: result.Data.SignalrPayload.Data.AnprEngine,
                            ProcessingTimeMs: result.Data.SignalrPayload.Data.ProcessingTimeMs,
                            IsValidated: result.Data.SignalrPayload.Data.IsValidated,
                            ValidatedAt: result.Data.SignalrPayload.Data.ValidatedAt,
                            ValidatedAtUtc: result.Data.SignalrPayload.Data.ValidatedAtUtc,
                            ValidatedBy: result.Data.SignalrPayload.Data.ValidatedBy,
                            ValidationNotes: result.Data.SignalrPayload.Data.ValidationNotes,
                            Tags: result.Data.SignalrPayload.Data.Tags,
                            CreatedAt: result.Data.SignalrPayload.Data.CreatedAt,
                            IsBlacklisted: result.Data.SignalrPayload.Data.IsBlacklisted,
                            BlacklistDetails: result.Data.SignalrPayload.Data.BlacklistDetails is null ? null : new AnprSearchBlacklistResponse(
                                ListType: result.Data.SignalrPayload.Data.BlacklistDetails.ListType,
                                Reason: result.Data.SignalrPayload.Data.BlacklistDetails.Reason,
                                Severity: result.Data.SignalrPayload.Data.BlacklistDetails.Severity,
                                ActionOnDetect: result.Data.SignalrPayload.Data.BlacklistDetails.ActionOnDetect,
                                ReferenceNumber: result.Data.SignalrPayload.Data.BlacklistDetails.ReferenceNumber),
                            Hardware: result.Data.SignalrPayload.Data.Hardware is null ? null : new AnprSearchHardwareResponse(
                                Id: result.Data.SignalrPayload.Data.Hardware.Id,
                                SerialNumber: result.Data.SignalrPayload.Data.Hardware.SerialNumber,
                                Manufacturer: result.Data.SignalrPayload.Data.Hardware.Manufacturer,
                                Model: result.Data.SignalrPayload.Data.Hardware.Model,
                                HardwareType: result.Data.SignalrPayload.Data.Hardware.HardwareType,
                                Status: result.Data.SignalrPayload.Data.Hardware.Status),
                            AccessPoint: result.Data.SignalrPayload.Data.AccessPoint is null ? null : new AnprSearchAccessPointResponse(
                                Id: result.Data.SignalrPayload.Data.AccessPoint.Id,
                                Name: result.Data.SignalrPayload.Data.AccessPoint.Name,
                                Prefix: result.Data.SignalrPayload.Data.AccessPoint.Prefix,
                                Latitude: result.Data.SignalrPayload.Data.AccessPoint.Latitude,
                                Longitude: result.Data.SignalrPayload.Data.AccessPoint.Longitude,
                                Type: result.Data.SignalrPayload.Data.AccessPoint.Type,
                                Orientation: result.Data.SignalrPayload.Data.AccessPoint.Orientation),
                            Project: result.Data.SignalrPayload.Data.Project is null ? null : new AnprSearchNamedRefResponse(result.Data.SignalrPayload.Data.Project.Id, result.Data.SignalrPayload.Data.Project.Name),
                            Area: result.Data.SignalrPayload.Data.Area is null ? null : new AnprSearchNamedRefResponse(result.Data.SignalrPayload.Data.Area.Id, result.Data.SignalrPayload.Data.Area.Name),
                            Zone: result.Data.SignalrPayload.Data.Zone is null ? null : new AnprSearchZoneResponse(result.Data.SignalrPayload.Data.Zone.Id, result.Data.SignalrPayload.Data.Zone.Code, result.Data.SignalrPayload.Data.Zone.Name),
                            Country: result.Data.SignalrPayload.Data.Country is null ? null : new AnprSearchCountryResponse(result.Data.SignalrPayload.Data.Country.Id, result.Data.SignalrPayload.Data.Country.Code, result.Data.SignalrPayload.Data.Country.Name),
                            State: result.Data.SignalrPayload.Data.State is null ? null : new AnprSearchNamedRefResponse(result.Data.SignalrPayload.Data.State.Id, result.Data.SignalrPayload.Data.State.Name),
                            Category: result.Data.SignalrPayload.Data.Category is null ? null : new AnprSearchNamedRefResponse(result.Data.SignalrPayload.Data.Category.Id, result.Data.SignalrPayload.Data.Category.Name),
                            ValidatedByUser: result.Data.SignalrPayload.Data.ValidatedByUser is null ? null : new AnprSearchUserResponse(
                                Id: result.Data.SignalrPayload.Data.ValidatedByUser.Id,
                                Username: result.Data.SignalrPayload.Data.ValidatedByUser.Username,
                                FullName: result.Data.SignalrPayload.Data.ValidatedByUser.FullName))
                           )
                       ) : null,
                   Metadata: null); 
        }


        private static bool IsValidImageFile(IFormFile? file)
        {
            if (file == null) return true;

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext.ToLower()))
                return false;

            if (file.Length > maxFileSize)
                return false;

            return true;
        }

    }
}
