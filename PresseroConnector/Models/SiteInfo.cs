using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class SiteInfo
    {
        public string SubscriberId { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public bool IsActive { get; set; }
        public string Url { get; set; }
        public string SiteType { get; set; }
        public string PrimaryDomain { get; set; }
        public string StartUrl { get; set; }
        public string Culture { get; set; }
        public IList<SiteDomain> SiteDomains { get; set; }
        public object MisId { get; set; }
        public IList<SiteShippingMethod> SiteShippingMethods { get; set; }
        public IList<SitePaymentMethod> SitePaymentMethods { get; set; }
        public SiteStore SiteStore { get; set; }
        public bool ExplicitConsentForPrivacy { get; set; }
        public object PrivacyPolicyPageURL { get; set; }
        public object PrivacyPolicyCustomPrompt { get; set; }
        public object ConsentToContactCustomPrompt { get; set; }
        public bool ReCaptcha { get; set; }
        public object ReCaptchaTermsOfUseUrl { get; set; }
        public object ReCaptchaCookiePolicyUrl { get; set; }
        public object ReCaptchaPrivacyPolicyUrl { get; set; }
    }
    public class SiteDomain
    {
        public string Domain { get; set; }
        public bool IsHTTPS { get; set; }
        public bool IsCannonical { get; set; }
        public string SiteId { get; set; }
    }

    public class ShippingMethod
    {
        public string Carrier { get; set; }
        public string ServiceName { get; set; }
        public string TrackingMethod { get; set; }
        public string TrackingUrl { get; set; }
        public string CarrierServiceCode { get; set; }
        public int MaxWt { get; set; }
        public bool IsActive { get; set; }
    }

    public class SubscriberShippingMethod
    {
        public string SubscriberShippingMethodId { get; set; }
        public bool IsResidential { get; set; }
        public string MethodName { get; set; }
        public double MinCharge { get; set; }
        public bool IsActive { get; set; }
        public double Markup { get; set; }
        public double Handling { get; set; }
        public object ClientID { get; set; }
        public double? MaxPkgWt { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public string Carrier { get; set; }
        public string ServiceName { get; set; }
        public string SubscriberId { get; set; }
        public string MisId { get; set; }
    }

    public class Acl
    {
        public string GroupId { get; set; }
        public string AclType { get; set; }
        public string TargetType { get; set; }
        public string Target { get; set; }
        public string AllowOrDeny { get; set; }
    }

    public class AllowedGroup
    {
        public string GroupId { get; set; }
        public string SubscriberId { get; set; }
        public string SiteId { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public string Description { get; set; }
        public IList<Acl> Acls { get; set; }
    }

    public class SiteShippingMethod
    {
        public string SiteShippingMethodId { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public bool AvailableToAllGroups { get; set; }
        public SubscriberShippingMethod SubscriberShippingMethod { get; set; }
        public IList<AllowedGroup> AllowedGroups { get; set; }
        public string SubscriberShippingMethodId { get; set; }
        public string SiteId { get; set; }
    }

    public class SitePMConfigInfo
    {
        public object Code { get; set; }
        public object Info { get; set; }
    }

    public class SitePaymentMethod
    {
        public SitePMConfigInfo SitePMConfigInfo { get; set; }
        public IList<object> AllowedGroups { get; set; }
        public string SitePaymentMethodId { get; set; }
        public string Provider { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public bool IsActive { get; set; }
        public object CustomSiteMessage { get; set; }
        public object LongDescription { get; set; }
        public bool UseRange { get; set; }
        public double MinTotal { get; set; }
        public double MaxTotal { get; set; }
        public int SortOrder { get; set; }
        public bool DisplayProviderError { get; set; }
        public object CustomCheckoutErrorMessage { get; set; }
        public string Implementor { get; set; }
        public bool AvailableToAllGroups { get; set; }
        public string SiteStoreId { get; set; }
        public string SubscriberId { get; set; }
        public string SubscriberPaymentMethodId { get; set; }
    }

    public class ShipAddress
    {
        public object UserId { get; set; }
        public string AddressId { get; set; }
        public object DefaultShipMethod { get; set; }
        public string Business { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }

    public class Location
    {
        public string LocationId { get; set; }
        public string SubscriberId { get; set; }
        public string AddressId { get; set; }
        public object SiteId { get; set; }
        public bool IsActive { get; set; }
        public string GeneralInfo { get; set; }
        public string Code { get; set; }
        public ShipAddress Address { get; set; }
    }

    public class BillToLocation
    {
        public string LocationId { get; set; }
        public string SubscriberId { get; set; }
        public string AddressId { get; set; }
        public string SiteId { get; set; }
        public bool IsActive { get; set; }
        public string GeneralInfo { get; set; }
        public string Code { get; set; }
        public Address BillAddress { get; set; }
    }

    public class Rule
    {
        public string ApprovalRuleId { get; set; }
        public string ApprovalPlanId { get; set; }
        public string RuleType { get; set; }
        public object DollarAmount { get; set; }
        public object AttributeName { get; set; }
        public string MatchText { get; set; }
        public int WhoCanApproveFlags { get; set; }
        public int ActionsAllowed { get; set; }
        public int Seq { get; set; }
        public string RuleName { get; set; }
        public string SiteShippingMethodId { get; set; }
        public string MatchGroupId { get; set; }
        public string ExtraApprovalGroupId { get; set; }
        public string LocationId { get; set; }
    }

    public class ApprovalPlan
    {
        public string ApprovalPlanId { get; set; }
        public string PlanName { get; set; }
        public int Seq { get; set; }
        public bool IsActive { get; set; }
        public string SubscriberId { get; set; }
        public string SiteId { get; set; }
        public IList<Rule> Rules { get; set; }
    }



    public class SiteStoreCust
    {
        public string SiteId { get; set; }
        public string SelfSignupApprovalGroup { get; set; }
        public bool CaptureShippingAddress { get; set; }
        public object AuthConfig { get; set; }
        public bool AllowOneClickApproval { get; set; }
        public bool AllowApproverEdit { get; set; }
        public bool AllowAdHocForm { get; set; }
        public BillToLocation BillToLocation { get; set; }
        public object PrimaryLocation { get; set; }
        public object BossGroup { get; set; }
        public object SalesUser { get; set; }
        public bool IsReturnFilesInHistoryToApprovers { get; set; }
        public int AddressBookMode { get; set; }
        public IList<ApprovalPlan> ApprovalPlans { get; set; }
        public IList<Location> Locations { get; set; }
        public IList<object> Departments { get; set; }
        public IList<object> Budgets { get; set; }
        public object PrimaryLocationId { get; set; }
        public object SubscriberSalesUser { get; set; }
        public string ApprovalPlanId { get; set; }
        public object BossGroupId { get; set; }
        public string BillToLocationId { get; set; }
        public object BrokerId { get; set; }
    }

    public class SiteStore
    {
        public string SiteId { get; set; }
        public string TaxStructure { get; set; }
        public string NavType { get; set; }
        public object MisId { get; set; }
        public string ShippingConfig { get; set; }
        public string BillToDeptOption { get; set; }
        public object CheckoutConversionTracking { get; set; }
        public bool CaptureReqShipDate { get; set; }
        public int CaptureRequestedShipDate { get; set; }
        public object DefaultShipDays { get; set; }
        public string PurchaseOrderPrompt { get; set; }
        public int CapturePurchaseOrder { get; set; }
        public object LogoutUrl { get; set; }
        public string ContinueShoppingUrl { get; set; }
        public string BackToCatalogUrl { get; set; }
        public object AnonymousCartInstructions { get; set; }
        public object CXMLUrl { get; set; }
        public object TaxId { get; set; }
        public bool CaptureTaxId { get; set; }
        public object TaxIDName { get; set; }
        public bool AllowCustomPONumber { get; set; }
        public bool ValidatePONumber { get; set; }
        public bool CaptureBillingAddress { get; set; }
        public bool ShipToMultipleAddress { get; set; }
        public object ProductQuickViewText { get; set; }
        public bool AllowGuestCheckout { get; set; }
        public object TaxPrompt { get; set; }
        public object TaxIdMask { get; set; }
        public bool ListPOCodeBasedOnBilling { get; set; }
        public bool IsEnableBudget { get; set; }
        public object BudgetPrompt { get; set; }
        public int ShoppingCartRedirectOption { get; set; }
        public bool SelectShipAddress { get; set; }
        public string ProofType { get; set; }
        public Location Location { get; set; }
        public object ShippingLocation { get; set; }
        public SiteStoreCust SiteStoreCust { get; set; }
        public object SiteCxmlPunchOutInfo { get; set; }
        public IList<object> Promos { get; set; }
        public object ShipFromLocation { get; set; }
        public string SubscriberLocationId { get; set; }
    }




}
