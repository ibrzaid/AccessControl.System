using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ACS.Models.Response.V1.ParkingService.Session
{
    public record ValidateSessionResponse(
      bool Success,
      string Message,
      string ErrorCode,
      string RequestId,
      [property: JsonPropertyName("details")] ValidateSessionDetailsResponse Details) : BaseResponses(Success, Message, ErrorCode, RequestId);

    public record ValidateSessionDetailsResponse(
        [property: JsonPropertyName("warnings")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<string> Warnings,
        [property: JsonPropertyName("errors")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<string> Errors,
        [property: JsonPropertyName("tax_details")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] TaxDetailsResponse TaxDetails,
        [property: JsonPropertyName("audit_log_id")] int AuditLogId,
        [property: JsonPropertyName("tariff_details")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] TariffDetailsResponse TariffDetails,
        [property: JsonPropertyName("payment_details")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] PaymentDetailsResponse PaymentDetails,
        [property: JsonPropertyName("session_details")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] SessionDetailsResponse SessionDetails,
        [property: JsonPropertyName("input_parameters")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] InputParametersResponse InputParameters,
        // ADD THESE NEW PROPERTIES:
        [property: JsonPropertyName("invoices")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] InvoicesResponse Invoices,
        [property: JsonPropertyName("payment_history")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] PaymentHistoryResponse PaymentHistory);

    // NEW: Invoices Response with Summary and List
    public record InvoicesResponse(
        [property: JsonPropertyName("summary")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] InvoiceSummaryResponse Summary,
        [property: JsonPropertyName("list")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<InvoiceDetailResponse> List);

    // NEW: Invoice Summary
    public record InvoiceSummaryResponse(
        [property: JsonPropertyName("total_paid")] decimal TotalPaid,
        [property: JsonPropertyName("invoice_count")] int InvoiceCount,
        [property: JsonPropertyName("last_invoice_date")] DateTime? LastInvoiceDate,
        [property: JsonPropertyName("has_invoices")] bool HasInvoices);

    // NEW: Invoice Detail with Header, Lines, Tax Summary, and Parties
    public record InvoiceDetailResponse(
        [property: JsonPropertyName("invoice_header")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] InvoiceHeaderResponse InvoiceHeader,
        [property: JsonPropertyName("lines")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<InvoiceLineResponse> Lines,
        [property: JsonPropertyName("tax_summary")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<InvoiceTaxSummaryResponse> TaxSummary,
        [property: JsonPropertyName("parties")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<InvoicePartyResponse> Parties);

    // NEW: Invoice Header
    public record InvoiceHeaderResponse(
        [property: JsonPropertyName("invoice_header_id")] long InvoiceHeaderId,
        [property: JsonPropertyName("invoice_uuid")] Guid InvoiceUuid,
        [property: JsonPropertyName("invoice_number")] string InvoiceNumber,
        [property: JsonPropertyName("invoice_type_code")] string InvoiceTypeCode,
        [property: JsonPropertyName("document_status_code")] string DocumentStatusCode,
        [property: JsonPropertyName("workspace_id")] int WorkspaceId,
        [property: JsonPropertyName("project_id")] int? ProjectId,
        [property: JsonPropertyName("subscriber_id")] int? SubscriberId,
        [property: JsonPropertyName("corporate_employee_id")] int? CorporateEmployeeId,
        [property: JsonPropertyName("parking_session_id")] decimal? ParkingSessionId,
        [property: JsonPropertyName("subscription_id")] int? SubscriptionId,
        [property: JsonPropertyName("issue_date")] DateTime IssueDate,
        [property: JsonPropertyName("due_date")] DateTime DueDate,
        [property: JsonPropertyName("tax_point_date")] DateTime? TaxPointDate,
        [property: JsonPropertyName("accounting_period_start")] DateTime? AccountingPeriodStart,
        [property: JsonPropertyName("accounting_period_end")] DateTime? AccountingPeriodEnd,
        [property: JsonPropertyName("document_currency_code")] string DocumentCurrencyCode,
        [property: JsonPropertyName("tax_currency_code")] string TaxCurrencyCode,
        [property: JsonPropertyName("pricing_currency_code")] string PricingCurrencyCode,
        [property: JsonPropertyName("line_extension_amount")] decimal LineExtensionAmount,
        [property: JsonPropertyName("tax_exclusive_amount")] decimal TaxExclusiveAmount,
        [property: JsonPropertyName("tax_inclusive_amount")] decimal TaxInclusiveAmount,
        [property: JsonPropertyName("prepaid_amount")] decimal? PrepaidAmount,
        [property: JsonPropertyName("payable_amount")] decimal PayableAmount,
        [property: JsonPropertyName("payable_rounding_amount")] decimal? PayableRoundingAmount,
        [property: JsonPropertyName("line_extension_amount_rounded")] decimal LineExtensionAmountRounded,
        [property: JsonPropertyName("tax_exclusive_amount_rounded")] decimal TaxExclusiveAmountRounded,
        [property: JsonPropertyName("tax_inclusive_amount_rounded")] decimal TaxInclusiveAmountRounded,
        [property: JsonPropertyName("prepaid_amount_rounded")] decimal? PrepaidAmountRounded,
        [property: JsonPropertyName("payable_amount_rounded")] decimal PayableAmountRounded,
        [property: JsonPropertyName("total_tax_amount")] decimal TotalTaxAmount,
        [property: JsonPropertyName("total_tax_amount_rounded")] decimal TotalTaxAmountRounded,
        [property: JsonPropertyName("total_allowance_amount")] decimal? TotalAllowanceAmount,
        [property: JsonPropertyName("total_charge_amount")] decimal? TotalChargeAmount,
        [property: JsonPropertyName("payment_means_code")] string PaymentMeansCode,
        [property: JsonPropertyName("payment_due_date")] DateTime? PaymentDueDate,
        [property: JsonPropertyName("payment_terms")] string PaymentTerms,
        [property: JsonPropertyName("invoice_note")] string InvoiceNote,
        [property: JsonPropertyName("internal_note")] string InternalNote,
        [property: JsonPropertyName("created_by")] int? CreatedBy,
        [property: JsonPropertyName("updated_by")] int? UpdatedBy,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
        [property: JsonPropertyName("validated_at")] DateTime? ValidatedAt,
        [property: JsonPropertyName("submitted_at")] DateTime? SubmittedAt,
        [property: JsonPropertyName("paid_at")] DateTime? PaidAt,
        [property: JsonPropertyName("cancelled_at")] DateTime? CancelledAt,
        [property: JsonPropertyName("cancellation_reason")] string CancellationReason);

    // NEW: Invoice Line
    public record InvoiceLineResponse(
        [property: JsonPropertyName("line_id")] int LineId,
        [property: JsonPropertyName("item_id")] string ItemId,
        [property: JsonPropertyName("item_name")] Dictionary<string, string> ItemName,
        [property: JsonPropertyName("item_description")] Dictionary<string, string>? ItemDescription,
        [property: JsonPropertyName("item_type")] string ItemType,
        [property: JsonPropertyName("source_reference_id")] long? SourceReferenceId,
        [property: JsonPropertyName("source_reference_name")] string SourceReferenceName,
        [property: JsonPropertyName("invoiced_quantity")] decimal InvoicedQuantity,
        [property: JsonPropertyName("unit_code")] string UnitCode,
        [property: JsonPropertyName("unit_price_amount")] decimal UnitPriceAmount,
        [property: JsonPropertyName("price_base_quantity")] decimal? PriceBaseQuantity,
        [property: JsonPropertyName("line_extension_amount")] decimal LineExtensionAmount,
        [property: JsonPropertyName("allowance_total_amount")] decimal? AllowanceTotalAmount,
        [property: JsonPropertyName("charge_total_amount")] decimal? ChargeTotalAmount,
        [property: JsonPropertyName("net_line_amount")] decimal NetLineAmount,
        [property: JsonPropertyName("line_extension_amount_rounded")] decimal LineExtensionAmountRounded,
        [property: JsonPropertyName("net_line_amount_rounded")] decimal NetLineAmountRounded,
        [property: JsonPropertyName("period_start_date")] DateTime? PeriodStartDate,
        [property: JsonPropertyName("period_end_date")] DateTime? PeriodEndDate,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
        [property: JsonPropertyName("tax_details")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<LineTaxDetailResponse>? TaxDetails);

    // NEW: Line Tax Detail
    public record LineTaxDetailResponse(
        [property: JsonPropertyName("tax_scheme_id")] long TaxSchemeId,
        [property: JsonPropertyName("tax_category_id")] long TaxCategoryId,
        [property: JsonPropertyName("jurisdiction_id")] long? JurisdictionId,
        [property: JsonPropertyName("taxable_base_amount")] decimal TaxableBaseAmount,
        [property: JsonPropertyName("taxable_base_amount_rounded")] decimal TaxableBaseAmountRounded,
        [property: JsonPropertyName("tax_percent")] decimal TaxPercent,
        [property: JsonPropertyName("tax_amount")] decimal TaxAmount,
        [property: JsonPropertyName("tax_amount_rounded")] decimal TaxAmountRounded,
        [property: JsonPropertyName("is_reverse_charge")] bool IsReverseCharge,
        [property: JsonPropertyName("exemption_reason_code")] string? ExemptionReasonCode,
        [property: JsonPropertyName("exemption_reason_text")] string? ExemptionReasonText,
        [property: JsonPropertyName("rounding_amount")] decimal? RoundingAmount,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt);

    // NEW: Invoice Tax Summary
    public record InvoiceTaxSummaryResponse(
        [property: JsonPropertyName("tax_scheme_id")] long TaxSchemeId,
        [property: JsonPropertyName("tax_category_id")] long TaxCategoryId,
        [property: JsonPropertyName("jurisdiction_id")] long? JurisdictionId,
        [property: JsonPropertyName("taxable_base_amount")] decimal TaxableBaseAmount,
        [property: JsonPropertyName("taxable_base_amount_rounded")] decimal TaxableBaseAmountRounded,
        [property: JsonPropertyName("tax_amount")] decimal TaxAmount,
        [property: JsonPropertyName("tax_amount_rounded")] decimal TaxAmountRounded,
        [property: JsonPropertyName("tax_percent")] decimal TaxPercent,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("tax_scheme")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] TaxSchemeResponse? TaxScheme,
        [property: JsonPropertyName("tax_category")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] TaxCategoryResponse? TaxCategory,
        [property: JsonPropertyName("jurisdiction")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] JurisdictionResponse? Jurisdiction);

    // NEW: Tax Scheme
    public record TaxSchemeResponse(
        [property: JsonPropertyName("tax_scheme_id")] long TaxSchemeId,
        [property: JsonPropertyName("country_id")] int CountryId,
        [property: JsonPropertyName("tax_scheme_code")] string TaxSchemeCode,
        [property: JsonPropertyName("tax_scheme_name")] Dictionary<string, string> TaxSchemeName,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("valid_from")] DateTime ValidFrom,
        [property: JsonPropertyName("valid_to")] DateTime? ValidTo,
        [property: JsonPropertyName("rounding_rule")] string RoundingRule,
        [property: JsonPropertyName("rounding_scale")] int RoundingScale);

    // NEW: Tax Category
    public record TaxCategoryResponse(
        [property: JsonPropertyName("tax_category_id")] long TaxCategoryId,
        [property: JsonPropertyName("tax_category_code")] string TaxCategoryCode,
        [property: JsonPropertyName("tax_category_name")] Dictionary<string, string> TaxCategoryName,
        [property: JsonPropertyName("rate_percent")] decimal? RatePercent,
        [property: JsonPropertyName("is_compound")] bool IsCompound,
        [property: JsonPropertyName("is_reverse_charge")] bool IsReverseCharge,
        [property: JsonPropertyName("is_exempt")] bool IsExempt,
        [property: JsonPropertyName("is_zero_rated")] bool IsZeroRated,
        [property: JsonPropertyName("valid_from")] DateTime ValidFrom,
        [property: JsonPropertyName("valid_to")] DateTime? ValidTo);

    // NEW: Jurisdiction
    public record JurisdictionResponse(
        [property: JsonPropertyName("jurisdiction_id")] long JurisdictionId,
        [property: JsonPropertyName("country_id")] int CountryId,
        [property: JsonPropertyName("parent_jurisdiction_id")] long? ParentJurisdictionId,
        [property: JsonPropertyName("jurisdiction_level")] string JurisdictionLevel,
        [property: JsonPropertyName("jurisdiction_code")] string JurisdictionCode,
        [property: JsonPropertyName("jurisdiction_name")] Dictionary<string, string> JurisdictionName);

    // NEW: Invoice Party
    public record InvoicePartyResponse(
        [property: JsonPropertyName("party_role")] string PartyRole,
        [property: JsonPropertyName("party_supplier_id")] long? PartySupplierId,
        [property: JsonPropertyName("party_customer_id")] long? PartyCustomerId,
        [property: JsonPropertyName("address_id")] long? AddressId,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("supplier_details")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] SupplierDetailsResponse? SupplierDetails,
        [property: JsonPropertyName("customer_details")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] CustomerDetailsResponse? CustomerDetails);

    // NEW: Supplier Details
    public record SupplierDetailsResponse(
        [property: JsonPropertyName("party_supplier_id")] long PartySupplierId,
        [property: JsonPropertyName("workspace_id")] int WorkspaceId,
        [property: JsonPropertyName("project_id")] int ProjectId,
        [property: JsonPropertyName("party_type")] string PartyType,
        [property: JsonPropertyName("company_name")] Dictionary<string, string>? CompanyName,
        [property: JsonPropertyName("trading_name")] string? TradingName,
        [property: JsonPropertyName("registration_name")] string? RegistrationName,
        [property: JsonPropertyName("company_id")] string? CompanyId,
        [property: JsonPropertyName("company_id_scheme_id")] string? CompanyIdSchemeId,
        [property: JsonPropertyName("tax_scheme_id")] long? TaxSchemeId,
        [property: JsonPropertyName("tax_registration_number")] string? TaxRegistrationNumber,
        [property: JsonPropertyName("tax_registration_country")] int? TaxRegistrationCountry,
        [property: JsonPropertyName("contact_name")] string? ContactName,
        [property: JsonPropertyName("contact_email")] string? ContactEmail,
        [property: JsonPropertyName("contact_phone")] string? ContactPhone,
        [property: JsonPropertyName("website")] string? Website,
        [property: JsonPropertyName("default_currency")] string DefaultCurrency,
        [property: JsonPropertyName("default_payment_terms")] string? DefaultPaymentTerms,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt);

    // NEW: Customer Details
    public record CustomerDetailsResponse(
        [property: JsonPropertyName("party_customer_id")] long PartyCustomerId,
        [property: JsonPropertyName("subscriber_id")] long SubscriberId,
        [property: JsonPropertyName("corporate_employee_id")] long? CorporateEmployeeId,
        [property: JsonPropertyName("party_type")] string PartyType,
        [property: JsonPropertyName("party_name")] Dictionary<string, string> PartyName,
        [property: JsonPropertyName("party_legal_entity_name")] string? PartyLegalEntityName,
        [property: JsonPropertyName("party_identification")] string? PartyIdentification,
        [property: JsonPropertyName("party_identification_scheme")] string? PartyIdentificationScheme,
        [property: JsonPropertyName("billing_address_id")] long? BillingAddressId,
        [property: JsonPropertyName("shipping_address_id")] long? ShippingAddressId,
        [property: JsonPropertyName("registered_address_id")] long? RegisteredAddressId,
        [property: JsonPropertyName("contact_name")] string? ContactName,
        [property: JsonPropertyName("contact_email")] string? ContactEmail,
        [property: JsonPropertyName("contact_phone")] string? ContactPhone,
        [property: JsonPropertyName("tax_scheme_id")] long? TaxSchemeId,
        [property: JsonPropertyName("tax_registration_number")] string? TaxRegistrationNumber,
        [property: JsonPropertyName("tax_registration_country")] int? TaxRegistrationCountry,
        [property: JsonPropertyName("is_tax_exempt")] bool IsTaxExempt,
        [property: JsonPropertyName("exemption_certificate_no")] string? ExemptionCertificateNo,
        [property: JsonPropertyName("exemption_reason")] string? ExemptionReason,
        [property: JsonPropertyName("default_payment_method")] string? DefaultPaymentMethod,
        [property: JsonPropertyName("payment_terms")] string? PaymentTerms,
        [property: JsonPropertyName("credit_limit")] decimal? CreditLimit,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt);

    // NEW: Payment History Response
    public record PaymentHistoryResponse(
        [property: JsonPropertyName("payments")][property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<PaymentFeeResponse> Payments,
        [property: JsonPropertyName("total_payments")] decimal TotalPayments,
        [property: JsonPropertyName("payment_count")] int PaymentCount);

    // NEW: Payment Fee Response
    public record PaymentFeeResponse(
        [property: JsonPropertyName("parking_fee_id")] long ParkingFeeId,
        [property: JsonPropertyName("pricing_plan_id")] long PricingPlanId,
        [property: JsonPropertyName("total_minutes")] int TotalMinutes,
        [property: JsonPropertyName("free_minutes")] int FreeMinutes,
        [property: JsonPropertyName("base_amount")] decimal BaseAmount,
        [property: JsonPropertyName("surge_multiplier")] decimal SurgeMultiplier,
        [property: JsonPropertyName("surged_amount")] decimal SurgedAmount,
        [property: JsonPropertyName("vat_percent")] decimal VatPercent,
        [property: JsonPropertyName("vat_amount")] decimal VatAmount,
        [property: JsonPropertyName("net_amount")] decimal NetAmount,
        [property: JsonPropertyName("gross_amount")] decimal GrossAmount,
        [property: JsonPropertyName("calculated_at")] DateTime CalculatedAt,
        [property: JsonPropertyName("details_json")] object DetailsJson,
        [property: JsonPropertyName("payment_status")] string PaymentStatus,
        [property: JsonPropertyName("payment_method")] string? PaymentMethod,
        [property: JsonPropertyName("payment_reference")] string? PaymentReference,
        [property: JsonPropertyName("payment_date")] DateTime? PaymentDate,
        [property: JsonPropertyName("invoice_id")] long? InvoiceId,
        [property: JsonPropertyName("created_by")] int? CreatedBy);

    public record TaxDetailsResponse(
        [property: JsonPropertyName("tax_mode")] string TaxMode,
        [property: JsonPropertyName("is_exempt")] bool IsExempt,
        [property: JsonPropertyName("net_amount")] decimal NetAmount,
        [property: JsonPropertyName("tax_amount")] decimal TaxAmount,
        [property: JsonPropertyName("is_compound")] bool IsCompound,
        [property: JsonPropertyName("tax_percent")] decimal TaxPercent,
        [property: JsonPropertyName("gross_amount")] decimal GrossAmount,
        [property: JsonPropertyName("tax_scheme_id")] long TaxSchemeId,
        [property: JsonPropertyName("tax_category_id")] long TaxCategoryId,
        [property: JsonPropertyName("tax_scheme_code")] string TaxSchemeCode,
        [property: JsonPropertyName("tax_category_code")] string TaxCategoryCode,
        [property: JsonPropertyName("taxable_base_amount")] decimal TaxableBaseAmount,
        [property: JsonPropertyName("exemption_reason_code")] string ExemptionReasonCode,
        [property: JsonPropertyName("exemption_reason_text")] string ExemptionReasonText);

    public record TariffDetailsResponse(
        [property: JsonPropertyName("amount")] decimal Amount,
        [property: JsonPropertyName("pricing_plan")] PricingPlanResponse PricingPlan,
        [property: JsonPropertyName("calculation_details")] CalculationDetailsResponse CalculationDetails);

    public record PricingPlanResponse(
        [property: JsonPropertyName("priority")] int Priority,
        [property: JsonPropertyName("valid_to")] DateTime? ValidTo,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("plan_name")] Dictionary<string, string> PlanName,
        [property: JsonPropertyName("valid_from")] DateTime ValidFrom,
        [property: JsonPropertyName("pricing_plan_id")] long PricingPlanId);

    public record CalculationDetailsResponse(
        [property: JsonPropertyName("cap_amount")] decimal? CapAmount,
        [property: JsonPropertyName("base_amount")] decimal BaseAmount,
        [property: JsonPropertyName("cap_applied")] bool CapApplied,
        [property: JsonPropertyName("final_amount")] decimal FinalAmount,
        [property: JsonPropertyName("free_minutes")] int FreeMinutes,
        [property: JsonPropertyName("total_minutes")] int TotalMinutes,
        [property: JsonPropertyName("rule_breakdown")] RuleBreakdownResponse RuleBreakdown,
        [property: JsonPropertyName("billable_minutes")] int BillableMinutes,
        [property: JsonPropertyName("surge_multiplier")] decimal SurgeMultiplier,
        [property: JsonPropertyName("min_charge_amount")] decimal MinChargeAmount,
        [property: JsonPropertyName("min_charge_applied")] bool MinChargeApplied);

    public record RuleBreakdownResponse(
        [property: JsonPropertyName("cap")] decimal? Cap,
        [property: JsonPropertyName("hourly")] HourlyRateResponse? Hourly,
        [property: JsonPropertyName("exit_time")] DateTime ExitTime,
        [property: JsonPropertyName("entry_time")] DateTime EntryTime,
        [property: JsonPropertyName("min_charge")] decimal MinCharge,
        [property: JsonPropertyName("occupancy_pct")] int OccupancyPct);

    public record HourlyRateResponse(
        [property: JsonPropertyName("rate")] decimal Rate,
        [property: JsonPropertyName("interval")] int Interval,
        [property: JsonPropertyName("remaining")] int Remaining);

    public record PaymentDetailsResponse(
        [property: JsonPropertyName("net_amount")] decimal NetAmount,
        [property: JsonPropertyName("tax_amount")] decimal TaxAmount,
        [property: JsonPropertyName("gross_amount")] decimal GrossAmount,
        [property: JsonPropertyName("currency")] string Currency);

    public record SessionDetailsResponse(
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("zone_id")] long ZoneId,
        [property: JsonPropertyName("exit_time")] DateTime? ExitTime,
        [property: JsonPropertyName("entry_time")] DateTime EntryTime,
        [property: JsonPropertyName("start_calculate_time")] DateTime StartCalculateTime,
        [property: JsonPropertyName("entry_plate")] string EntryPlate,
        [property: JsonPropertyName("session_code")] string SessionCode,
        [property: JsonPropertyName("subscriber_id")] int? SubscriberId,
        [property: JsonPropertyName("workspace_name")] string WorkspaceName,
        [property: JsonPropertyName("exit_plate_code")] string ExitPlateCode,
        [property: JsonPropertyName("exit_point_name")] Dictionary<string, string>? ExitPointName,
        [property: JsonPropertyName("project_area_id")] int ProjectAreaId,
        [property: JsonPropertyName("entry_plate_code")] string EntryPlateCode,
        [property: JsonPropertyName("entry_point_name")] Dictionary<string, string> EntryPointName,
        [property: JsonPropertyName("exit_plate_number")] string ExitPlateNumber,
        [property: JsonPropertyName("entry_plate_number")] string EntryPlateNumber,
        [property: JsonPropertyName("parking_session_id")] long ParkingSessionId,
        [property: JsonPropertyName("entry_country_id")] long EntryCountryId,
        [property: JsonPropertyName("entry_country_code")] string EntryCountryCode,
        [property: JsonPropertyName("entry_country_names")] Dictionary<string, string> EntryCountryName,
        [property: JsonPropertyName("entry_plate_state_id")] long EntryPlateStateId,
        [property: JsonPropertyName("entry_plate_state_name")] Dictionary<string, string> EntryPlateStateName,
        [property: JsonPropertyName("entry_plate_category_id")] long EntryPlateCategoryId,
        [property: JsonPropertyName("entry_category_names")] Dictionary<string, string> EntryCategoryNames);

    public record InputParametersResponse(
        [property: JsonPropertyName("zone_id")] long ZoneId,
        [property: JsonPropertyName("project_id")] int ProjectId,
        [property: JsonPropertyName("workspace_id")] int WorkspaceId,
        [property: JsonPropertyName("access_point_id")] long AccessPointId,
        [property: JsonPropertyName("project_area_id")] int ProjectAreaId);
}
#pragma warning restore IDE0130 // Namespace does not match folder structure