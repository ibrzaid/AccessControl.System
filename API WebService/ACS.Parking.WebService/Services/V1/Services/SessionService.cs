using ACS.Background;
using ACS.Database.IDataAccess.ParkingService.V1;
using ACS.License.V1;
using ACS.Models.Response.V1.ParkingService.Entry;
using ACS.Models.Response.V1.ParkingService.Payment;
using ACS.Models.Response.V1.ParkingService.Session;
using ACS.Parking.WebService.Models.Request.V1;
using ACS.Parking.WebService.Services.V1.Interfaces;
using ACS.Service.V1.Interfaces;
using System.Globalization;


namespace ACS.Parking.WebService.Services.V1.Services
{
    public class SessionService(ILicenseManager licenseManager,  IBackgroundTaskQueue backgroundTaskQueue, IProjectSessionService projectSessionService, IBaseService baseService, IAccessPointSessionService accessPointSessionService, INotificationService notificationService, ILogger<SessionService> logger) : Service.Service(licenseManager), ISessionService
    {
       

        private ISessionDataAccess this[Connection conn]
        {
            get
            {
                return conn.Type switch
                {
                    Database.IConnection.DatabaseType.PostgresDatabase => Database.DataAccess.PostgresDataAccess.ParkingService.V1.SessionDataAccess.GetInstance($"{conn.Server}.{conn.Name}", conn.Conn),
                    _ => throw new NotSupportedException($"The database type '{conn.Type}' is not supported in EntrySessionService.")
                };
            }
        }

        public async Task<EntrySessionResponse> CreateEntrySessionAsync(string workspace, string project, string projectArea, string zone,  string accessPoint, string userSession, string createdBy, CreateSessionRequest request, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            if (!double.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude = 0;
            if (!double.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;


            var response = await this[license?.DB!].CreateEntrySessionAsync(accessPoint: accessPoint,
                plateCode: request.PlateCode ?? "",
                plateNumber: request.PlateNumber ?? "",
                country: request.Country.ToString(),
                state: request.State.ToString(),
                category: request.Category.ToString(),
                latitude: _latitude,
                longitude: _longitude,
                anpr: request.ANPR,
                cameraCapureUrl: request.CameraCapureUrl,               
                project: project,
                projectArea: projectArea,
                zone: zone,                
                subscriber: request.Subscriber.ToString(),
                qr: "",
                createdBy: createdBy,
                workspace: workspace,
                userSession: userSession,
                ipAddress: ipAddress,
                deviceInfo: deviceInfo,
                agent: agent,
                requestId: requestId,
                cancellationToken: cancellationToken
                );
            EntrySessionResponse result =  response.Success ? new()
            {
                Success = true,
                Error= response.Message,
                ErrorCode = response.ErrorCode,
                Data = baseService.MapSessionDataToResponse(response.Data),
                Metadata = baseService.MapMetadataToResponse(response.Metadata),
                SystemError = response.SystemError,
                Timestamp = response.Timestamp ?? DateTime.UtcNow,
                RequestId = response.RequestId

            } : new()
            {
                Success = false,
                Error = response.Message,
                ErrorCode = response.ErrorCode,
                SystemError = response.SystemError,
                Timestamp = DateTime.UtcNow,
                RequestId = requestId
            };


            if (result.Success && result.Data != null)
                if (int.TryParse(workspace, out int workspaceId) 
                    && int.TryParse(project, out int projectId) 
                    && int.TryParse(projectArea, out int _projectArea) 
                    && int.TryParse(zone, out int _zone) 
                    && int.TryParse(accessPoint, out int _accessPoint))
                {
                    backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                    {
                        try
                        {
                            await projectSessionService.AddSessionToCacheAsync(
                                workspace: workspaceId, 
                                project: projectId, 
                                session: result.Data, 
                                status: "IN" ,
                                cancellationToken: token);


                            await projectSessionService.CloseOldSessionsInCacheAsync(
                               workspace: workspaceId,
                               project: projectId,                              
                               plateCode: request.PlateCode ?? "",
                               plateNumber: request.PlateNumber ?? "",
                               country: request.Country,
                               state: request.State,
                               category: request.Category,
                               result.Data.ParkingSession?.ParkingSessionId.ToString()!,
                               cancellationToken);

                            notificationService.SendParkingSession(workspace, project, result.Data);
                        }
                        catch(Exception ex)
                        {
                            logger.LogError(ex, "Error in background task for CreateEntrySessionAsync");
                        }

                        try
                        {
                            await accessPointSessionService.AddSessionToCacheAsync(
                                workspace: workspaceId,
                                project: projectId,
                                projectArea: _projectArea,
                                zone: _zone,
                                accessPoint: _accessPoint,
                                session: result.Data,
                                status: "IN",
                                cancellationToken: token);

                            await accessPointSessionService.CloseOldSessionsInCacheAsync(
                                workspace: workspaceId,
                                project: projectId,
                                projectArea: _projectArea,
                                zone: _zone,
                                accessPoint: _accessPoint,
                                plateCode: request.PlateCode ?? "",
                                plateNumber: request.PlateNumber ?? "",
                                country: request.Country,
                                state: request.State,
                                category: request.Category,
                                result.Data.ParkingSession?.ParkingSessionId.ToString()!, 
                                cancellationToken);


                            notificationService.SendParkingSession(workspace, project, projectArea, zone, accessPoint, result.Data);



                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error in background task for CreateEntrySessionAsync");
                        }

                    });
                } 
            return result;

        }

       
        public async Task<EntrySessionResponse> UpdateParkingSessionStatusAsync(long id, string status, int? cancellationReason, int? exitAccessPointId, string workspace, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var response = await this[license?.DB!].UpdateParkingSessionStatusAsync(
                id, status, cancellationReason, exitAccessPointId, cancellationToken);

            EntrySessionResponse result = response.Success ? new()
            {
                Success = true,
                Error = response.Message,
                ErrorCode = response.ErrorCode,
                Data = baseService.MapSessionDataToResponse(response.Data),
                Metadata = baseService.MapMetadataToResponse(response.Metadata),
                SystemError = response.SystemError,
                Timestamp = response.Timestamp ?? DateTime.UtcNow,
                RequestId = response.RequestId

            } : new()
            {
                Success = false,
                Error = response.Message,
                ErrorCode = response.ErrorCode,
                SystemError = response.SystemError,
                Timestamp = DateTime.UtcNow,
            };
            
            return result;
        }

        public async Task<EntrySessionResponse> DeleteParkingSessionAsync(long id, string workspace, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var success = await this[license?.DB!].DeleteParkingSessionAsync(id, cancellationToken);

            if (success)
            {
                return new EntrySessionResponse
                {
                    Success = true,
                    Error = "Parking session deleted successfully",
                    Timestamp = DateTime.UtcNow
                };
            }

            return new EntrySessionResponse
            {
                Success = false,
                Error = "Failed to delete parking session",
                ErrorCode = "DELETE_FAILED",
                Timestamp = DateTime.UtcNow
            };
        }


        public async Task<ValidateSessionResponse> ValidateSessionBySessionIdAsync(string workspace, string project, string projectArea, string zone, string accessPoint,
            string sessionCode, string sessionId, string plateCode, string plateNumber, string country, string state, string category, string userId, double latitude, 
            double longitude, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default)
        {
            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].ValidateSessionAsync(workspace, project, projectArea, zone, accessPoint, sessionCode, sessionId, plateCode, plateNumber, 
                country, state, category, userId, latitude, longitude, ipAddress, deviceInfo, agent, requestId, cancellationToken);

            return result == null || !result.Success ?
                new ValidateSessionResponse(
                    Success: false,
                    Message: result?.Message ?? "Validation failed",
                    ErrorCode: result?.ErrorCode ?? "VALIDATION_FAILED",
                    RequestId: requestId!,
                    Details: new ValidateSessionDetailsResponse(
                        Warnings: [],
                        Errors: result?.Details?.Errors ?? [],
                        TaxDetails: null!,
                        AuditLogId: result?.Details?.AuditLogId ?? 0,
                        TariffDetails: null!,
                        PaymentDetails: null!,
                        SessionDetails: null!,
                        InputParameters: null!,
                        Invoices: null!,
                        PaymentHistory: null!
                    )) :
                new ValidateSessionResponse(
                    Success: true,
                    Message: result.Message ?? "Session validated successfully",
                    ErrorCode: null!,
                    RequestId: requestId!,
                    Details: result.Details != null ?
                        new ValidateSessionDetailsResponse(
                            Warnings: result.Details.Warnings ?? [],
                            Errors: result.Details.Errors ?? [],
                            TaxDetails: result.Details.TaxDetails != null ?
                                new TaxDetailsResponse(
                                    TaxMode: result.Details.TaxDetails.TaxMode,
                                    IsExempt: result.Details.TaxDetails.IsExempt,
                                    NetAmount: result.Details.TaxDetails.NetAmount,
                                    TaxAmount: result.Details.TaxDetails.TaxAmount,
                                    IsCompound: result.Details.TaxDetails.IsCompound,
                                    TaxPercent: result.Details.TaxDetails.TaxPercent,
                                    GrossAmount: result.Details.TaxDetails.GrossAmount,
                                    TaxSchemeId: result.Details.TaxDetails.TaxSchemeId,
                                    TaxCategoryId: result.Details.TaxDetails.TaxCategoryId,
                                    TaxSchemeCode: result.Details.TaxDetails.TaxSchemeCode,
                                    TaxCategoryCode: result.Details.TaxDetails.TaxCategoryCode,
                                    TaxableBaseAmount: result.Details.TaxDetails.TaxableBaseAmount,
                                    ExemptionReasonCode: result.Details.TaxDetails.ExemptionReasonCode ?? string.Empty,
                                    ExemptionReasonText: result.Details.TaxDetails.ExemptionReasonText ?? string.Empty
                                ) : null!,
                            AuditLogId: result.Details.AuditLogId,
                            TariffDetails: result.Details.TariffDetails != null ?
                                new TariffDetailsResponse(
                                    Amount: result.Details.TariffDetails.Amount,
                                    PricingPlan: result.Details.TariffDetails.PricingPlan != null ?
                                        new PricingPlanResponse(
                                            Priority: result.Details.TariffDetails.PricingPlan.Priority,
                                            ValidTo: result.Details.TariffDetails.PricingPlan.ValidTo,
                                            IsActive: result.Details.TariffDetails.PricingPlan.IsActive,
                                            PlanName: result.Details.TariffDetails.PricingPlan.PlanName ?? [],
                                            ValidFrom: result.Details.TariffDetails.PricingPlan.ValidFrom,
                                            PricingPlanId: result.Details.TariffDetails.PricingPlan.PricingPlanId
                                        ) : null!,
                                    CalculationDetails: result.Details.TariffDetails.CalculationDetails != null ?
                                        new CalculationDetailsResponse(
                                            CapAmount: result.Details.TariffDetails.CalculationDetails.CapAmount,
                                            BaseAmount: result.Details.TariffDetails.CalculationDetails.BaseAmount,
                                            CapApplied: result.Details.TariffDetails.CalculationDetails.CapApplied,
                                            FinalAmount: result.Details.TariffDetails.CalculationDetails.FinalAmount,
                                            FreeMinutes: result.Details.TariffDetails.CalculationDetails.FreeMinutes,
                                            TotalMinutes: result.Details.TariffDetails.CalculationDetails.TotalMinutes,
                                            RuleBreakdown: result.Details.TariffDetails.CalculationDetails.RuleBreakdown != null ?
                                                new RuleBreakdownResponse(
                                                    Cap: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.Cap,
                                                    Hourly: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.Hourly != null ?
                                                        new HourlyRateResponse(
                                                            Rate: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.Hourly?.Rate?? 0,
                                                            Interval: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.Hourly?.Interval??0,
                                                            Remaining: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.Hourly?.Remaining??0
                                                        ) : null!,
                                                    ExitTime: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.ExitTime,
                                                    EntryTime: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.EntryTime,
                                                    MinCharge: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.MinCharge,
                                                    OccupancyPct: result.Details.TariffDetails.CalculationDetails.RuleBreakdown.OccupancyPct
                                                ) : null!,
                                            BillableMinutes: result.Details.TariffDetails.CalculationDetails.BillableMinutes,
                                            SurgeMultiplier: result.Details.TariffDetails.CalculationDetails.SurgeMultiplier,
                                            MinChargeAmount: result.Details.TariffDetails.CalculationDetails.MinChargeAmount,
                                            MinChargeApplied: result.Details.TariffDetails.CalculationDetails.MinChargeApplied
                                        ) : null!
                                ) : null!,
                            PaymentDetails: result.Details.PaymentDetails != null ?
                                new PaymentDetailsResponse(
                                    NetAmount: result.Details.PaymentDetails.NetAmount,
                                    TaxAmount: result.Details.PaymentDetails.TaxAmount,
                                    GrossAmount: result.Details.PaymentDetails.GrossAmount,
                                    Currency: result.Details.PaymentDetails.Currency ?? "AED"
                                ) : null!,
                            SessionDetails: result.Details.SessionDetails != null ?
                                new SessionDetailsResponse(
                                    Status: result.Details.SessionDetails.Status,
                                    ZoneId: result.Details.SessionDetails.ZoneId,
                                    ExitTime: result.Details.SessionDetails.ExitTime,
                                    EntryTime: result.Details.SessionDetails.EntryTime,
                                    StartCalculateTime: result.Details.SessionDetails.StartCalculateTime,
                                    EntryPlate: result.Details.SessionDetails.EntryPlate,
                                    SessionCode: result.Details.SessionDetails.SessionCode,
                                    SubscriberId: result.Details.SessionDetails.SubscriberId,
                                    WorkspaceName: result.Details.SessionDetails.WorkspaceName,
                                    ExitPlateCode: result.Details.SessionDetails.ExitPlateCode ?? string.Empty,
                                    ExitPointName: result.Details.SessionDetails.ExitPointName,
                                    ProjectAreaId: result.Details.SessionDetails.ProjectAreaId,
                                    EntryPlateCode: result.Details.SessionDetails.EntryPlateCode,
                                    EntryPointName: result.Details.SessionDetails.EntryPointName ?? [],
                                    ExitPlateNumber: result.Details.SessionDetails.ExitPlateNumber ?? string.Empty,
                                    EntryPlateNumber: result.Details.SessionDetails.EntryPlateNumber,
                                    ParkingSessionId: result.Details.SessionDetails.ParkingSessionId,
                                    EntryCountryId: result.Details.SessionDetails.EntryCountryId,
                                    EntryCountryCode: result.Details.SessionDetails.EntryCountryCode,
                                    EntryCountryName: result.Details.SessionDetails.EntryCountryName ?? [],
                                    EntryPlateStateId: result.Details.SessionDetails.EntryPlateStateId,
                                    EntryPlateStateName: result.Details.SessionDetails.EntryPlateStateName ?? [],
                                    EntryPlateCategoryId: result.Details.SessionDetails.EntryPlateCategoryId,
                                    EntryCategoryNames: result.Details.SessionDetails.EntryCategoryNames ?? []
                                ) : null!,
                            InputParameters: result.Details.InputParameters != null ?
                                new InputParametersResponse(
                                    ZoneId: result.Details.InputParameters.ZoneId,
                                    ProjectId: result.Details.InputParameters.ProjectId,
                                    WorkspaceId: result.Details.InputParameters.WorkspaceId,
                                    AccessPointId: result.Details.InputParameters.AccessPointId,
                                    ProjectAreaId: result.Details.InputParameters.ProjectAreaId
                                ) : null!,
                            // ADD THESE NEW PROPERTIES:
                            Invoices: result.Details.Invoices != null ?
                                new InvoicesResponse(
                                    Summary: result.Details.Invoices.Summary != null ?
                                        new InvoiceSummaryResponse(
                                            TotalPaid: result.Details.Invoices.Summary.TotalPaid,
                                            InvoiceCount: result.Details.Invoices.Summary.InvoiceCount,
                                            LastInvoiceDate: result.Details.Invoices.Summary.LastInvoiceDate,
                                            HasInvoices: result.Details.Invoices.Summary.HasInvoices
                                        ) : null!,
                                    List: result.Details.Invoices.List?
                                        .Select(invoice => new InvoiceDetailResponse(
                                            InvoiceHeader: invoice.InvoiceHeader != null ?
                                                new InvoiceHeaderResponse(
                                                    InvoiceHeaderId: invoice.InvoiceHeader.InvoiceHeaderId,
                                                    InvoiceUuid: invoice.InvoiceHeader.InvoiceUuid,
                                                    InvoiceNumber: invoice.InvoiceHeader.InvoiceNumber,
                                                    InvoiceTypeCode: invoice.InvoiceHeader.InvoiceTypeCode,
                                                    DocumentStatusCode: invoice.InvoiceHeader.DocumentStatusCode,
                                                    WorkspaceId: invoice.InvoiceHeader.WorkspaceId,
                                                    ProjectId: invoice.InvoiceHeader.ProjectId,
                                                    SubscriberId: invoice.InvoiceHeader.SubscriberId,
                                                    CorporateEmployeeId: invoice.InvoiceHeader.CorporateEmployeeId,
                                                    ParkingSessionId: invoice.InvoiceHeader.ParkingSessionId,
                                                    SubscriptionId: invoice.InvoiceHeader.SubscriptionId,
                                                    IssueDate: invoice.InvoiceHeader.IssueDate,
                                                    DueDate: invoice.InvoiceHeader.DueDate,
                                                    TaxPointDate: invoice.InvoiceHeader.TaxPointDate,
                                                    AccountingPeriodStart: invoice.InvoiceHeader.AccountingPeriodStart,
                                                    AccountingPeriodEnd: invoice.InvoiceHeader.AccountingPeriodEnd,
                                                    DocumentCurrencyCode: invoice.InvoiceHeader.DocumentCurrencyCode,
                                                    TaxCurrencyCode: invoice.InvoiceHeader.TaxCurrencyCode,
                                                    PricingCurrencyCode: invoice.InvoiceHeader.PricingCurrencyCode,
                                                    LineExtensionAmount: invoice.InvoiceHeader.LineExtensionAmount,
                                                    TaxExclusiveAmount: invoice.InvoiceHeader.TaxExclusiveAmount,
                                                    TaxInclusiveAmount: invoice.InvoiceHeader.TaxInclusiveAmount,
                                                    PrepaidAmount: invoice.InvoiceHeader.PrepaidAmount,
                                                    PayableAmount: invoice.InvoiceHeader.PayableAmount,
                                                    PayableRoundingAmount: invoice.InvoiceHeader.PayableRoundingAmount,
                                                    LineExtensionAmountRounded: invoice.InvoiceHeader.LineExtensionAmountRounded,
                                                    TaxExclusiveAmountRounded: invoice.InvoiceHeader.TaxExclusiveAmountRounded,
                                                    TaxInclusiveAmountRounded: invoice.InvoiceHeader.TaxInclusiveAmountRounded,
                                                    PrepaidAmountRounded: invoice.InvoiceHeader.PrepaidAmountRounded,
                                                    PayableAmountRounded: invoice.InvoiceHeader.PayableAmountRounded,
                                                    TotalTaxAmount: invoice.InvoiceHeader.TotalTaxAmount,
                                                    TotalTaxAmountRounded: invoice.InvoiceHeader.TotalTaxAmountRounded,
                                                    TotalAllowanceAmount: invoice.InvoiceHeader.TotalAllowanceAmount,
                                                    TotalChargeAmount: invoice.InvoiceHeader.TotalChargeAmount,
                                                    PaymentMeansCode: invoice.InvoiceHeader.PaymentMeansCode,
                                                    PaymentDueDate: invoice.InvoiceHeader.PaymentDueDate,
                                                    PaymentTerms: invoice.InvoiceHeader.PaymentTerms,
                                                    InvoiceNote: invoice.InvoiceHeader.InvoiceNote,
                                                    InternalNote: invoice.InvoiceHeader.InternalNote,
                                                    CreatedBy: invoice.InvoiceHeader.CreatedBy,
                                                    UpdatedBy: invoice.InvoiceHeader.UpdatedBy,
                                                    CreatedAt: invoice.InvoiceHeader.CreatedAt,
                                                    UpdatedAt: invoice.InvoiceHeader.UpdatedAt,
                                                    ValidatedAt: invoice.InvoiceHeader.ValidatedAt,
                                                    SubmittedAt: invoice.InvoiceHeader.SubmittedAt,
                                                    PaidAt: invoice.InvoiceHeader.PaidAt,
                                                    CancelledAt: invoice.InvoiceHeader.CancelledAt,
                                                    CancellationReason: invoice.InvoiceHeader.CancellationReason
                                                ) : null!,
                                            Lines: invoice.Lines?
                                                .Select(line => new InvoiceLineResponse(
                                                    LineId: line.LineId,
                                                    ItemId: line.ItemId,
                                                    ItemName: line.ItemName ?? [],
                                                    ItemDescription: line.ItemDescription,
                                                    ItemType: line.ItemType,
                                                    SourceReferenceId: line.SourceReferenceId,
                                                    SourceReferenceName: line.SourceReferenceName,
                                                    InvoicedQuantity: line.InvoicedQuantity,
                                                    UnitCode: line.UnitCode,
                                                    UnitPriceAmount: line.UnitPriceAmount,
                                                    PriceBaseQuantity: line.PriceBaseQuantity,
                                                    LineExtensionAmount: line.LineExtensionAmount,
                                                    AllowanceTotalAmount: line.AllowanceTotalAmount,
                                                    ChargeTotalAmount: line.ChargeTotalAmount,
                                                    NetLineAmount: line.NetLineAmount,
                                                    LineExtensionAmountRounded: line.LineExtensionAmountRounded,
                                                    NetLineAmountRounded: line.NetLineAmountRounded,
                                                    PeriodStartDate: line.PeriodStartDate,
                                                    PeriodEndDate: line.PeriodEndDate,
                                                    CreatedAt: line.CreatedAt,
                                                    UpdatedAt: line.UpdatedAt,
                                                    TaxDetails: line.TaxDetails?
                                                        .Select(td => new LineTaxDetailResponse(
                                                            TaxSchemeId: td.TaxSchemeId,
                                                            TaxCategoryId: td.TaxCategoryId,
                                                            JurisdictionId: td.JurisdictionId,
                                                            TaxableBaseAmount: td.TaxableBaseAmount,
                                                            TaxableBaseAmountRounded: td.TaxableBaseAmountRounded,
                                                            TaxPercent: td.TaxPercent,
                                                            TaxAmount: td.TaxAmount,
                                                            TaxAmountRounded: td.TaxAmountRounded,
                                                            IsReverseCharge: td.IsReverseCharge,
                                                            ExemptionReasonCode: td.ExemptionReasonCode,
                                                            ExemptionReasonText: td.ExemptionReasonText,
                                                            RoundingAmount: td.RoundingAmount,
                                                            CreatedAt: td.CreatedAt
                                                        )).ToList()
                                                )).ToList() ?? [],
                                            TaxSummary: invoice.TaxSummary?
                                                .Select(ts => new InvoiceTaxSummaryResponse(
                                                    TaxSchemeId: ts.TaxSchemeId,
                                                    TaxCategoryId: ts.TaxCategoryId,
                                                    JurisdictionId: ts.JurisdictionId,
                                                    TaxableBaseAmount: ts.TaxableBaseAmount,
                                                    TaxableBaseAmountRounded: ts.TaxableBaseAmountRounded,
                                                    TaxAmount: ts.TaxAmount,
                                                    TaxAmountRounded: ts.TaxAmountRounded,
                                                    TaxPercent: ts.TaxPercent,
                                                    CreatedAt: ts.CreatedAt,
                                                    TaxScheme: ts.TaxScheme != null ?
                                                        new TaxSchemeResponse(
                                                            TaxSchemeId: ts.TaxScheme.TaxSchemeId,
                                                            CountryId: ts.TaxScheme.CountryId,
                                                            TaxSchemeCode: ts.TaxScheme.TaxSchemeCode,
                                                            TaxSchemeName: ts.TaxScheme.TaxSchemeName ?? [],
                                                            IsActive: ts.TaxScheme.IsActive,
                                                            ValidFrom: ts.TaxScheme.ValidFrom,
                                                            ValidTo: ts.TaxScheme.ValidTo,
                                                            RoundingRule: ts.TaxScheme.RoundingRule,
                                                            RoundingScale: ts.TaxScheme.RoundingScale
                                                        ) : null!,
                                                    TaxCategory: ts.TaxCategory != null ?
                                                        new TaxCategoryResponse(
                                                            TaxCategoryId: ts.TaxCategory.TaxCategoryId,
                                                            TaxCategoryCode: ts.TaxCategory.TaxCategoryCode,
                                                            TaxCategoryName: ts.TaxCategory.TaxCategoryName ?? [],
                                                            RatePercent: ts.TaxCategory.RatePercent,
                                                            IsCompound: ts.TaxCategory.IsCompound,
                                                            IsReverseCharge: ts.TaxCategory.IsReverseCharge,
                                                            IsExempt: ts.TaxCategory.IsExempt,
                                                            IsZeroRated: ts.TaxCategory.IsZeroRated,
                                                            ValidFrom: ts.TaxCategory.ValidFrom,
                                                            ValidTo: ts.TaxCategory.ValidTo
                                                        ) : null!,
                                                    Jurisdiction: ts.Jurisdiction != null ?
                                                        new JurisdictionResponse(
                                                            JurisdictionId: ts.Jurisdiction.JurisdictionId,
                                                            CountryId: ts.Jurisdiction.CountryId,
                                                            ParentJurisdictionId: ts.Jurisdiction.ParentJurisdictionId,
                                                            JurisdictionLevel: ts.Jurisdiction.JurisdictionLevel,
                                                            JurisdictionCode: ts.Jurisdiction.JurisdictionCode,
                                                            JurisdictionName: ts.Jurisdiction.JurisdictionName ?? []
                                                        ) : null!
                                                )).ToList() ?? [],
                                            Parties: invoice.Parties?
                                                .Select(party => new InvoicePartyResponse(
                                                    PartyRole: party.PartyRole,
                                                    PartySupplierId: party.PartySupplierId,
                                                    PartyCustomerId: party.PartyCustomerId,
                                                    AddressId: party.AddressId,
                                                    CreatedAt: party.CreatedAt,
                                                    SupplierDetails: party.SupplierDetails != null ?
                                                        new SupplierDetailsResponse(
                                                            PartySupplierId: party.SupplierDetails.PartySupplierId,
                                                            WorkspaceId: party.SupplierDetails.WorkspaceId,
                                                            ProjectId: party.SupplierDetails.ProjectId,
                                                            PartyType: party.SupplierDetails.PartyType,
                                                            CompanyName: party.SupplierDetails.CompanyName,
                                                            TradingName: party.SupplierDetails.TradingName,
                                                            RegistrationName: party.SupplierDetails.RegistrationName,
                                                            CompanyId: party.SupplierDetails.CompanyId,
                                                            CompanyIdSchemeId: party.SupplierDetails.CompanyIdSchemeId,
                                                            TaxSchemeId: party.SupplierDetails.TaxSchemeId,
                                                            TaxRegistrationNumber: party.SupplierDetails.TaxRegistrationNumber,
                                                            TaxRegistrationCountry: party.SupplierDetails.TaxRegistrationCountry,
                                                            ContactName: party.SupplierDetails.ContactName,
                                                            ContactEmail: party.SupplierDetails.ContactEmail,
                                                            ContactPhone: party.SupplierDetails.ContactPhone,
                                                            Website: party.SupplierDetails.Website,
                                                            DefaultCurrency: party.SupplierDetails.DefaultCurrency,
                                                            DefaultPaymentTerms: party.SupplierDetails.DefaultPaymentTerms,
                                                            IsActive: party.SupplierDetails.IsActive,
                                                            CreatedAt: party.SupplierDetails.CreatedAt,
                                                            UpdatedAt: party.SupplierDetails.UpdatedAt
                                                        ) : null!,
                                                    CustomerDetails: party.CustomerDetails != null ?
                                                        new CustomerDetailsResponse(
                                                            PartyCustomerId: party.CustomerDetails.PartyCustomerId,
                                                            SubscriberId: party.CustomerDetails.SubscriberId,
                                                            CorporateEmployeeId: party.CustomerDetails.CorporateEmployeeId,
                                                            PartyType: party.CustomerDetails.PartyType,
                                                            PartyName: party.CustomerDetails.PartyName ?? [],
                                                            PartyLegalEntityName: party.CustomerDetails.PartyLegalEntityName,
                                                            PartyIdentification: party.CustomerDetails.PartyIdentification,
                                                            PartyIdentificationScheme: party.CustomerDetails.PartyIdentificationScheme,
                                                            BillingAddressId: party.CustomerDetails.BillingAddressId,
                                                            ShippingAddressId: party.CustomerDetails.ShippingAddressId,
                                                            RegisteredAddressId: party.CustomerDetails.RegisteredAddressId,
                                                            ContactName: party.CustomerDetails.ContactName,
                                                            ContactEmail: party.CustomerDetails.ContactEmail,
                                                            ContactPhone: party.CustomerDetails.ContactPhone,
                                                            TaxSchemeId: party.CustomerDetails.TaxSchemeId,
                                                            TaxRegistrationNumber: party.CustomerDetails.TaxRegistrationNumber,
                                                            TaxRegistrationCountry: party.CustomerDetails.TaxRegistrationCountry,
                                                            IsTaxExempt: party.CustomerDetails.IsTaxExempt,
                                                            ExemptionCertificateNo: party.CustomerDetails.ExemptionCertificateNo,
                                                            ExemptionReason: party.CustomerDetails.ExemptionReason,
                                                            DefaultPaymentMethod: party.CustomerDetails.DefaultPaymentMethod,
                                                            PaymentTerms: party.CustomerDetails.PaymentTerms,
                                                            CreditLimit: party.CustomerDetails.CreditLimit,
                                                            IsActive: party.CustomerDetails.IsActive,
                                                            CreatedAt: party.CustomerDetails.CreatedAt,
                                                            UpdatedAt: party.CustomerDetails.UpdatedAt
                                                        ) : null!
                                                )).ToList() ?? []
                                        )).ToList() ?? []
                                ) : null!,
                            PaymentHistory: result.Details.PaymentHistory != null ?
                                new PaymentHistoryResponse(
                                    Payments: result.Details.PaymentHistory.Payments?
                                        .Select(payment => new PaymentFeeResponse(
                                            ParkingFeeId: payment.ParkingFeeId,
                                            PricingPlanId: payment.PricingPlanId,
                                            TotalMinutes: payment.TotalMinutes,
                                            FreeMinutes: payment.FreeMinutes,
                                            BaseAmount: payment.BaseAmount,
                                            SurgeMultiplier: payment.SurgeMultiplier,
                                            SurgedAmount: payment.SurgedAmount,
                                            VatPercent: payment.VatPercent,
                                            VatAmount: payment.VatAmount,
                                            NetAmount: payment.NetAmount,
                                            GrossAmount: payment.GrossAmount,
                                            CalculatedAt: payment.CalculatedAt,
                                            DetailsJson: payment.DetailsJson,
                                            PaymentStatus: payment.PaymentStatus,
                                            PaymentMethod: payment.PaymentMethod,
                                            PaymentReference: payment.PaymentReference,
                                            PaymentDate: payment.PaymentDate,
                                            InvoiceId: payment.InvoiceId,
                                            CreatedBy: payment.CreatedBy
                                        )).ToList() ?? [],
                                    TotalPayments: result.Details.PaymentHistory.TotalPayments,
                                    PaymentCount: result.Details.PaymentHistory.PaymentCount
                                ) : null!
                        ) : null!
                );
        }

        public async Task<ProcessPaymentResponse> PaymentSessionAsync(string workspace, string project, string projectArea, string zone, string accessPoint, string userSession,
            string createdBy, PaymentSessionRequest request, int sessionId, string? ipAddress, string? deviceInfo, string? agent, string? requestId, CancellationToken cancellationToken = default)
        {
            if (!double.TryParse(request.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _latitude)) _latitude = 0;
            if (!double.TryParse(request.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double _longitude)) _longitude = 0;

            var license = this.LicenseManager.GetLicense();
            var result = await this[license?.DB!].PaymentSessionAsync(
                workspace,
                project,
                projectArea,
                zone,
                accessPoint,
                sessionId.ToString(),
                request.PaymentMethod ?? "10",
                request.NetAmt ?? 0,
                request.VATAmt ?? 0,
                request.GrossAmt ?? 0,
                request.PlateCode ?? "",
                request.PlateNumber ?? "",
                request.Country.ToString(),
                request.State.ToString(),
                request.Category.ToString(),
                request.ANPR?.ToString() ?? "",
                request.CameraCapureUrl ?? "",
                request.PaymentReference ?? "",
                createdBy,
                userSession,
                _latitude,
                _longitude,
                ipAddress,
                deviceInfo,
                agent,
                requestId,
                cancellationToken
            );

            return result == null || !result.Success
                ? new ProcessPaymentResponse(
                    Success: false,
                    Message: result?.Message ?? "Payment processing failed",
                    ErrorCode: result?.ErrorCode ?? "PAYMENT_PROCESSING_ERROR",
                    RequestId: requestId ?? string.Empty,
                    Data: null,
                    Metadata: null,
                    SessionId: result?.SessionId ?? sessionId,
                    ErrorDetail: result?.ErrorDetail,
                    Timestamp: result?.Timestamp ?? DateTime.UtcNow
                )
                : new ProcessPaymentResponse(
                    Success: true,
                    Message: result.Message ?? "Payment processed successfully",
                    ErrorCode: null!,
                    RequestId: requestId ?? string.Empty,
                    Data: result.Data != null ? new PaymentDataResponse(
                        Transaction: new TransactionResponse(
                            Id: result.Data.Transaction.Id,
                            Reference: result.Data.Transaction.Reference,
                            Timestamp: result.Data.Transaction.Timestamp,
                            Method: result.Data.Transaction.Method,
                            Amount: result.Data.Transaction.Amount
                        ),
                        Session: new PaymentSessionResponse(
                            Id: result.Data.Session.Id,
                            Code: result.Data.Session.Code,
                            Vehicle: result.Data.Session.Vehicle,
                            DurationMinutes: result.Data.Session.DurationMinutes,
                            EntryTime: result.Data.Session.EntryTime,
                            ExitTime: result.Data.Session.ExitTime,
                            Status: result.Data.Session.Status
                        ),
                        Financial: new FinancialResponse(
                            Net: result.Data.Financial.Net,
                            Vat: result.Data.Financial.Vat,
                            Gross: result.Data.Financial.Gross,
                            VatPercent: result.Data.Financial.VatPercent
                        ),
                        Documents: new DocumentsResponse(
                            InvoiceId: result.Data.Documents.InvoiceId,
                            InvoiceNumber: result.Data.Documents.InvoiceNumber,
                            FeeId: result.Data.Documents.FeeId,
                            InvoiceDetails: result.Data.Documents.InvoiceDetails != null
                                ? new InvoiceDetailResponse(
                                    InvoiceHeader: new InvoiceHeaderResponse(
                                        InvoiceHeaderId: result.Data.Documents.InvoiceDetails.InvoiceHeader.InvoiceHeaderId,
                                        InvoiceUuid: result.Data.Documents.InvoiceDetails.InvoiceHeader.InvoiceUuid,
                                        InvoiceNumber: result.Data.Documents.InvoiceDetails.InvoiceHeader.InvoiceNumber,
                                        InvoiceTypeCode: result.Data.Documents.InvoiceDetails.InvoiceHeader.InvoiceTypeCode,
                                        DocumentStatusCode: result.Data.Documents.InvoiceDetails.InvoiceHeader.DocumentStatusCode,
                                        WorkspaceId: result.Data.Documents.InvoiceDetails.InvoiceHeader.WorkspaceId,
                                        ProjectId: result.Data.Documents.InvoiceDetails.InvoiceHeader.ProjectId,
                                        SubscriberId: result.Data.Documents.InvoiceDetails.InvoiceHeader.SubscriberId,
                                        CorporateEmployeeId: result.Data.Documents.InvoiceDetails.InvoiceHeader.CorporateEmployeeId,
                                        ParkingSessionId: result.Data.Documents.InvoiceDetails.InvoiceHeader.ParkingSessionId,
                                        SubscriptionId: result.Data.Documents.InvoiceDetails.InvoiceHeader.SubscriptionId,
                                        IssueDate: result.Data.Documents.InvoiceDetails.InvoiceHeader.IssueDate,
                                        DueDate: result.Data.Documents.InvoiceDetails.InvoiceHeader.DueDate,
                                        TaxPointDate: result.Data.Documents.InvoiceDetails.InvoiceHeader.TaxPointDate,
                                        AccountingPeriodStart: result.Data.Documents.InvoiceDetails.InvoiceHeader.AccountingPeriodStart,
                                        AccountingPeriodEnd: result.Data.Documents.InvoiceDetails.InvoiceHeader.AccountingPeriodEnd,
                                        DocumentCurrencyCode: result.Data.Documents.InvoiceDetails.InvoiceHeader.DocumentCurrencyCode,
                                        TaxCurrencyCode: result.Data.Documents.InvoiceDetails.InvoiceHeader.TaxCurrencyCode,
                                        PricingCurrencyCode: result.Data.Documents.InvoiceDetails.InvoiceHeader.PricingCurrencyCode,
                                        LineExtensionAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.LineExtensionAmount,
                                        TaxExclusiveAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.TaxExclusiveAmount,
                                        TaxInclusiveAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.TaxInclusiveAmount,
                                        PrepaidAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.PrepaidAmount,
                                        PayableAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.PayableAmount,
                                        PayableRoundingAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.PayableRoundingAmount,
                                        LineExtensionAmountRounded: result.Data.Documents.InvoiceDetails.InvoiceHeader.LineExtensionAmountRounded,
                                        TaxExclusiveAmountRounded: result.Data.Documents.InvoiceDetails.InvoiceHeader.TaxExclusiveAmountRounded,
                                        TaxInclusiveAmountRounded: result.Data.Documents.InvoiceDetails.InvoiceHeader.TaxInclusiveAmountRounded,
                                        PrepaidAmountRounded: result.Data.Documents.InvoiceDetails.InvoiceHeader.PrepaidAmountRounded,
                                        PayableAmountRounded: result.Data.Documents.InvoiceDetails.InvoiceHeader.PayableAmountRounded,
                                        TotalTaxAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.TotalTaxAmount,
                                        TotalTaxAmountRounded: result.Data.Documents.InvoiceDetails.InvoiceHeader.TotalTaxAmountRounded,
                                        TotalAllowanceAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.TotalAllowanceAmount,
                                        TotalChargeAmount: result.Data.Documents.InvoiceDetails.InvoiceHeader.TotalChargeAmount,
                                        PaymentMeansCode: result.Data.Documents.InvoiceDetails.InvoiceHeader.PaymentMeansCode,
                                        PaymentDueDate: result.Data.Documents.InvoiceDetails.InvoiceHeader.PaymentDueDate,
                                        PaymentTerms: result.Data.Documents.InvoiceDetails.InvoiceHeader.PaymentTerms,
                                        InvoiceNote: result.Data.Documents.InvoiceDetails.InvoiceHeader.InvoiceNote,
                                        InternalNote: result.Data.Documents.InvoiceDetails.InvoiceHeader.InternalNote,
                                        CreatedBy: result.Data.Documents.InvoiceDetails.InvoiceHeader.CreatedBy,
                                        UpdatedBy: result.Data.Documents.InvoiceDetails.InvoiceHeader.UpdatedBy,
                                        CreatedAt: result.Data.Documents.InvoiceDetails.InvoiceHeader.CreatedAt,
                                        UpdatedAt: result.Data.Documents.InvoiceDetails.InvoiceHeader.UpdatedAt,
                                        ValidatedAt: result.Data.Documents.InvoiceDetails.InvoiceHeader.ValidatedAt,
                                        SubmittedAt: result.Data.Documents.InvoiceDetails.InvoiceHeader.SubmittedAt,
                                        PaidAt: result.Data.Documents.InvoiceDetails.InvoiceHeader.PaidAt,
                                        CancelledAt: result.Data.Documents.InvoiceDetails.InvoiceHeader.CancelledAt,
                                        CancellationReason: result.Data.Documents.InvoiceDetails.InvoiceHeader.CancellationReason
                                    ),
                                    Lines: result.Data.Documents.InvoiceDetails.Lines?.Select(line => new InvoiceLineResponse(
                                        LineId: line.LineId,
                                        ItemId: line.ItemId,
                                        ItemName: line.ItemName,
                                        ItemDescription: line.ItemDescription,
                                        ItemType: line.ItemType,
                                        SourceReferenceId: line.SourceReferenceId,
                                        SourceReferenceName: line.SourceReferenceName,
                                        InvoicedQuantity: line.InvoicedQuantity,
                                        UnitCode: line.UnitCode,
                                        UnitPriceAmount: line.UnitPriceAmount,
                                        PriceBaseQuantity: line.PriceBaseQuantity,
                                        LineExtensionAmount: line.LineExtensionAmount,
                                        AllowanceTotalAmount: line.AllowanceTotalAmount,
                                        ChargeTotalAmount: line.ChargeTotalAmount,
                                        NetLineAmount: line.NetLineAmount,
                                        LineExtensionAmountRounded: line.LineExtensionAmountRounded,
                                        NetLineAmountRounded: line.NetLineAmountRounded,
                                        PeriodStartDate: line.PeriodStartDate,
                                        PeriodEndDate: line.PeriodEndDate,
                                        CreatedAt: line.CreatedAt,
                                        UpdatedAt: line.UpdatedAt,
                                        TaxDetails: line.TaxDetails?.Select(td => new LineTaxDetailResponse(
                                            TaxSchemeId: td.TaxSchemeId,
                                            TaxCategoryId: td.TaxCategoryId,
                                            JurisdictionId: td.JurisdictionId,
                                            TaxableBaseAmount: td.TaxableBaseAmount,
                                            TaxableBaseAmountRounded: td.TaxableBaseAmountRounded,
                                            TaxPercent: td.TaxPercent,
                                            TaxAmount: td.TaxAmount,
                                            TaxAmountRounded: td.TaxAmountRounded,
                                            IsReverseCharge: td.IsReverseCharge,
                                            ExemptionReasonCode: td.ExemptionReasonCode,
                                            ExemptionReasonText: td.ExemptionReasonText,
                                            RoundingAmount: td.RoundingAmount,
                                            CreatedAt: td.CreatedAt
                                        )).ToList()
                                    )).ToList() ?? new List<InvoiceLineResponse>(),
                                    TaxSummary: result.Data.Documents.InvoiceDetails.TaxSummary?.Select(ts => new InvoiceTaxSummaryResponse(
                                        TaxSchemeId: ts.TaxSchemeId,
                                        TaxCategoryId: ts.TaxCategoryId,
                                        JurisdictionId: ts.JurisdictionId,
                                        TaxableBaseAmount: ts.TaxableBaseAmount,
                                        TaxableBaseAmountRounded: ts.TaxableBaseAmountRounded,
                                        TaxAmount: ts.TaxAmount,
                                        TaxAmountRounded: ts.TaxAmountRounded,
                                        TaxPercent: ts.TaxPercent,
                                        CreatedAt: ts.CreatedAt,
                                        TaxScheme: ts.TaxScheme != null ? new TaxSchemeResponse(
                                            TaxSchemeId: ts.TaxScheme.TaxSchemeId,
                                            CountryId: ts.TaxScheme.CountryId,
                                            TaxSchemeCode: ts.TaxScheme.TaxSchemeCode,
                                            TaxSchemeName: ts.TaxScheme.TaxSchemeName,
                                            IsActive: ts.TaxScheme.IsActive,
                                            ValidFrom: ts.TaxScheme.ValidFrom,
                                            ValidTo: ts.TaxScheme.ValidTo,
                                            RoundingRule: ts.TaxScheme.RoundingRule,
                                            RoundingScale: ts.TaxScheme.RoundingScale
                                        ) : null,
                                        TaxCategory: ts.TaxCategory != null ? new TaxCategoryResponse(
                                            TaxCategoryId: ts.TaxCategory.TaxCategoryId,
                                            TaxCategoryCode: ts.TaxCategory.TaxCategoryCode,
                                            TaxCategoryName: ts.TaxCategory.TaxCategoryName,
                                            RatePercent: ts.TaxCategory.RatePercent,
                                            IsCompound: ts.TaxCategory.IsCompound,
                                            IsReverseCharge: ts.TaxCategory.IsReverseCharge,
                                            IsExempt: ts.TaxCategory.IsExempt,
                                            IsZeroRated: ts.TaxCategory.IsZeroRated,
                                            ValidFrom: ts.TaxCategory.ValidFrom,
                                            ValidTo: ts.TaxCategory.ValidTo
                                        ) : null,
                                        Jurisdiction: ts.Jurisdiction != null ? new JurisdictionResponse(
                                            JurisdictionId: ts.Jurisdiction.JurisdictionId,
                                            CountryId: ts.Jurisdiction.CountryId,
                                            ParentJurisdictionId: ts.Jurisdiction.ParentJurisdictionId,
                                            JurisdictionLevel: ts.Jurisdiction.JurisdictionLevel,
                                            JurisdictionCode: ts.Jurisdiction.JurisdictionCode,
                                            JurisdictionName: ts.Jurisdiction.JurisdictionName
                                        ) : null
                                    )).ToList() ?? new List<InvoiceTaxSummaryResponse>(),
                                    Parties: result.Data.Documents.InvoiceDetails.Parties?.Select(party => new InvoicePartyResponse(
                                        PartyRole: party.PartyRole,
                                        PartySupplierId: party.PartySupplierId,
                                        PartyCustomerId: party.PartyCustomerId,
                                        AddressId: party.AddressId,
                                        CreatedAt: party.CreatedAt,
                                        SupplierDetails: party.SupplierDetails != null ? new SupplierDetailsResponse(
                                            PartySupplierId: party.SupplierDetails.PartySupplierId,
                                            WorkspaceId: party.SupplierDetails.WorkspaceId,
                                            ProjectId: party.SupplierDetails.ProjectId,
                                            PartyType: party.SupplierDetails.PartyType,
                                            CompanyName: party.SupplierDetails.CompanyName,
                                            TradingName: party.SupplierDetails.TradingName,
                                            RegistrationName: party.SupplierDetails.RegistrationName,
                                            CompanyId: party.SupplierDetails.CompanyId,
                                            CompanyIdSchemeId: party.SupplierDetails.CompanyIdSchemeId,
                                            TaxSchemeId: party.SupplierDetails.TaxSchemeId,
                                            TaxRegistrationNumber: party.SupplierDetails.TaxRegistrationNumber,
                                            TaxRegistrationCountry: party.SupplierDetails.TaxRegistrationCountry,
                                            ContactName: party.SupplierDetails.ContactName,
                                            ContactEmail: party.SupplierDetails.ContactEmail,
                                            ContactPhone: party.SupplierDetails.ContactPhone,
                                            Website: party.SupplierDetails.Website,
                                            DefaultCurrency: party.SupplierDetails.DefaultCurrency,
                                            DefaultPaymentTerms: party.SupplierDetails.DefaultPaymentTerms,
                                            IsActive: party.SupplierDetails.IsActive,
                                            CreatedAt: party.SupplierDetails.CreatedAt,
                                            UpdatedAt: party.SupplierDetails.UpdatedAt
                                        ) : null,
                                        CustomerDetails: party.CustomerDetails != null ? new CustomerDetailsResponse(
                                            PartyCustomerId: party.CustomerDetails.PartyCustomerId,
                                            SubscriberId: party.CustomerDetails.SubscriberId,
                                            CorporateEmployeeId: party.CustomerDetails.CorporateEmployeeId,
                                            PartyType: party.CustomerDetails.PartyType,
                                            PartyName: party.CustomerDetails.PartyName,
                                            PartyLegalEntityName: party.CustomerDetails.PartyLegalEntityName,
                                            PartyIdentification: party.CustomerDetails.PartyIdentification,
                                            PartyIdentificationScheme: party.CustomerDetails.PartyIdentificationScheme,
                                            BillingAddressId: party.CustomerDetails.BillingAddressId,
                                            ShippingAddressId: party.CustomerDetails.ShippingAddressId,
                                            RegisteredAddressId: party.CustomerDetails.RegisteredAddressId,
                                            ContactName: party.CustomerDetails.ContactName,
                                            ContactEmail: party.CustomerDetails.ContactEmail,
                                            ContactPhone: party.CustomerDetails.ContactPhone,
                                            TaxSchemeId: party.CustomerDetails.TaxSchemeId,
                                            TaxRegistrationNumber: party.CustomerDetails.TaxRegistrationNumber,
                                            TaxRegistrationCountry: party.CustomerDetails.TaxRegistrationCountry,
                                            IsTaxExempt: party.CustomerDetails.IsTaxExempt,
                                            ExemptionCertificateNo: party.CustomerDetails.ExemptionCertificateNo,
                                            ExemptionReason: party.CustomerDetails.ExemptionReason,
                                            DefaultPaymentMethod: party.CustomerDetails.DefaultPaymentMethod,
                                            PaymentTerms: party.CustomerDetails.PaymentTerms,
                                            CreditLimit: party.CustomerDetails.CreditLimit,
                                            IsActive: party.CustomerDetails.IsActive,
                                            CreatedAt: party.CustomerDetails.CreatedAt,
                                            UpdatedAt: party.CustomerDetails.UpdatedAt
                                        ) : null
                                    )).ToList() ?? new List<InvoicePartyResponse>()
                                )
                                : null,
                            ExistingRecordUsed: result.Data.Documents.ExistingRecordUsed
                        )
                    ) : null,
                    Metadata: result.Metadata != null ? new PaymentMetadataResponse(
                        ProcessingTimeMs: result.Metadata.ProcessingTimeMs,
                        AuditId: result.Metadata.AuditId,
                        RequestId: result.Metadata.RequestId
                    ) : null,
                    SessionId: result.SessionId ?? sessionId,
                    ErrorDetail: null,
                    Timestamp: result.Timestamp ?? DateTime.UtcNow
                );
        }
    }
}
