using System;
using System.Collections.Generic;

namespace PipelineWS.LocalModel
{
    #region ==== References ====
    #region ApplicationControl
    public class ApplicationControl
    {
        public int PRODUCT_ID { get; set; }
        public int CONTROL_ID { get; set; }
        public string CONTROL_NAME { get; set; }
        public string CONTROL_GROUP_NAME { get; set; }
        public int CONTROL_TAB_INDEX { get; set; }
    }
    #endregion

    #region ApplicationPage
    public class ApplicationPage
    {
        public int PRODUCT_ID { get; set; }
        public int PAGE_ID { get; set; }
        public string PAGE_NAME { get; set; }
        public string PAGE_GROUP_NAME { get; set; }
        public int PAGE_TAB_INDEX { get; set; }
    }
    #endregion

    #region == ApplicationState ==

    #region ApplicationState
    public class ApplicationState
    {
        public int APPLICATION_STATE_ID { get; set; }
        public string APPLICATION_STATE_NAME { get; set; }
        public string APPLICATION_STATE_SYS_NAME { get; set; }
        public string APPLICATION_STATE_ACTION { get; set; }
        public bool NEED_DIRECTION { get; set; }
        public int NEED_ACTION { get; set; }
        public bool VISIBLE_IN { get; set; }
        public bool VISIBLE_OUT { get; set; }
    }

    #endregion

    #region AcceleratorFinalResult
    public class AcceleratorFinalResult
    {
        public int FINAL_RESULT { get; set; }
        public string FINAL_RESULT_MANE { get; set; }
    }
    #endregion AcceleratorFinalResult

    #region ApplicationStateMap
    public class ApplicationStateMap
    {
        public int MAP_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public int CURRENT_STATE_ID { get; set; }
        public string CURRENT_STATE_NAME { get; set; }
        public string CURRENT_STATE_SYS_NAME { get; set; }
        public bool NEED_DIRECTION { get; set; }
        public int APPLICATION_STATE_ID { get; set; }
        public string NEXT_STATE_ACTION { get; set; }
        public string NEXT_STATE_NAME { get; set; }
        public string NEXT_STATE_SYS_NAME { get; set; }
        public int NEED_ACTION { get; set; }
        public bool VISIBLE_IN { get; set; }
        public bool VISIBLE_OUT { get; set; }
    }

    public class ProductRestructMap
    {
        public int BASE_PRODUCT_ID { get; set; }
        public bool IS_RESTRUCT { get; set; }
        public decimal RESTRUCT_RATE { get; set; }
    }
    #endregion

    #region ApplicationStateWorkPlaceToMap
    public class ApplicationStateWorkPlaceToMap
    {
        public int MAP_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public string WORK_PLACE_NAME { get; set; }
        public string WORK_PLACE_SYS_NAME { get; set; }
        public int APPLICATION_STATE_ID { get; set; }
        public string APPLICATION_STATE_ACTION { get; set; }
        public string APPLICATION_STATE_NAME { get; set; }
        public string APPLICATION_STATE_SYS_NAME { get; set; }
        public int WORK_PLACE_ID_TO { get; set; }
        public string WORK_PLACE_TO_NAME { get; set; }
        public string WORK_PLACE_TO_SYS_NAME { get; set; }
    }
    #endregion

    #endregion

    #region === Auto (Car) Methods ===
    #region AutoDealer
    public class AutoDealer
    {
        public int DEALER_ID { get; set; }
        public string DEALER_NAME { get; set; }
    }
    #endregion

    #region InsuranceCompany
    public class InsuranceCompany
    {
        public int INSURANCE_COMPANY_ID { get; set; }
        public string INSURANCE_COMPANY_NAME { get; set; }
    }
    #endregion

    #region LMSInsuranceType
    public class LMSInsuranceType
    {
        public int INSURANCE_TYPE_ID { get; set; }
        public string INSURANCE_TYPE_NAME { get; set; }
        public string SYS_CODE { get; set; }
    }
    #endregion


    #region InsuranceTariff
    public class InsuranceTariff
    {
        public int INSURANCE_RATE_ID { get; set; }
        public string PRODUCT_ID { get; set; }
        public string INSURANCE_RATE_NAME { get; set; }
        public double INSURANCE_RATE_VALUE { get; set; }
        public string INSURANCE_ATTRIBUTE_NAME { get; set; }
        public int INSURANCE_COMPANY_ID { get; set; }
        public string PRODUCT_PROPERTY_GUID { get; set; }
        public int INSURANCE_PRODUCT_ID { get; set; }
    }
    #endregion

    #region InsuranceThirdParty
    public class InsuranceThirdParty
    {
        public int THIRD_PARTY_INSURANCE_ID { get; set; }
        public string THIRD_PARTY_INSURANCE_NAME { get; set; }
        public double THIRD_PARTY_INSURANCE_VALUE { get; set; }
        public double THIRD_PARTY_INSURANCE_LIMIT { get; set; }
        public string THIRD_PARTY_INSURANCE_ATTRIBUTE_NAME { get; set; }
        public int INSURANCE_COMPANY_ID { get; set; }
        public string PRODUCT_PROPERTY_GUID { get; set; }
        public int INSURANCE_PRODUCT_ID { get; set; }
        public string INSURANCE_PRODUCT_CCY { get; set; }
    }
    #endregion

    #region InsuranceDriverPassenger
    public class InsuranceDriverPassenger
    {
        public int DRIVER_PASSENGER_INSURANCE_ID { get; set; }
        public string DRIVER_PASSENGER_INSURANCE_NAME { get; set; }
        public double DRIVER_PASSENGER_INSURANCE_VALUE { get; set; }
        public double DRIVER_PASSENGER_INSURANCE_LIMIT { get; set; }
        public string DRIVER_PASSENGER_INSURANCE_ATTRIBUTE_NAME { get; set; }
        public int INSURANCE_COMPANY_ID { get; set; }
        public string PRODUCT_PROPERTY_GUID { get; set; }
        public int INSURANCE_PRODUCT_ID { get; set; }
        public string INSURANCE_PRODUCT_CCY { get; set; }
    }
    #endregion
    #endregion

    #region Bank
    public class Bank
    {
        public string BANK_CODE { get; set; }
        public string BANK_NAME { get; set; }
    }
    #endregion

    #region AccountProducts
    public class AccountProduct
    {
        public int ACC_PRODUCT_ID { get; set; }
        public string ACC_PRODUCT_NAME { get; set; }
    }
    #endregion AccountProducts

    #region Branch
    public class Branch
    {
        public int BRANCH_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #region Business
    public class Business
    {
        public int BUSINESS_ID { get; set; }
        public string BUSINESS_NAME { get; set; }
        public string BUSINESS_SYS_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #region City
    public class City
    {
        public int CITY_ID { get; set; }
        public string CITY_NAME { get; set; }
        public int PRIVILEGE_ID { get; set; }
    }
    #endregion

    #region == Client ==

    #region ClientActivesType
    public class ClientActivesType
    {
        public int CLIENT_ACTIVES_TYPE_ID { get; set; }
        public string CLIENT_ACTIVES_TYPE_NAME { get; set; }
    }
    #endregion

    #region ClientRank
    public class ClientRank
    {
        public int CLIENT_RANK_ID { get; set; }
        public string CLIENT_RANK_NAME { get; set; }
    }
    #endregion

    #region ClientSalaryCategory
    public class ClientSalaryCategory
    {
        public int CLIENT_SALARY_CATEGORY_ID { get; set; }
        public string CLIENT_SALARY_CATEGORY_NAME { get; set; }
        public string CLIENT_SALARY_CATEGORY_SYS_NAME { get; set; }
    }
    #endregion

    #region ClientType
    public class ClientType
    {
        public int CLIENT_TYPE_ID { get; set; }
        public string CLIENT_TYPE_NAME { get; set; }
    }
    #endregion

    #region CoborrowerRelationType
    public class CoborrowerRelationType
    {
        public int COBORROWER_RELATION_TYPE_ID { get; set; }
        public string COBORROWER_RELATION_TYPE_NAME { get; set; }
    }
    #endregion

    #region ClientSubType
    public class ClientSubType
    {
        public int CLIENT_SUB_TYPE_ID { get; set; }
        public string CLIENT_SUB_TYPE_NAME { get; set; }
        public string CLIENT_SUB_TYPE_DESCRIPTION { get; set; }
    }
    #endregion ClientSubType

    #region ClientConversationalLanguage
    public class ClientConversationalLanguage
    {
        public int CONVERS_LANG_ID { get; set; }
        public string CONVERS_LANG_CODE_2 { get; set; }
        public string CONVERS_LANG { get; set; }
        public string CONVERS_LANG_LAT { get; set; }
    }
    #endregion ClientConversationalLanguage
    #endregion

    #region == Collateraltype ==

    #region CollateralType
    public class CollateralType
    {
        public int COLLATERAL_TYPE_ID { get; set; }
        public string COLLATERAL_TYPE_NAME { get; set; }
    }
    #endregion

    #region LoanCollateralType
    public class LoanCollateralType
    {
        public string COLLATERAL_TYPE_ID { get; set; }
        public string COLLATERAL_TYPE_NAME { get; set; }
    }
    #endregion

    #endregion

    #region == Committee ==

    #region Committee
    public class Committee
    {
        public int COMMITTEE_ID { get; set; }
        public string COMMITTEE_NANE { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public string BRANCH_ID { get; set; }
        public int COMMITTEE_USER_COUNT { get; set; }
    }
    #endregion

    #region CommitteeDelegate
    public class CommitteeDelegate
    {
        public int MAP_ID { get; set; }
        public int COMMITTEE_MAP_ID { get; set; }
        public string COMMITTEE_NANE { get; set; }
        public int COMMITTEE_USER_ID { get; set; }
        public string COMMITTEE_USER_NAME { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #region CommitteeProperties
    public class CommitteeProperties
    {
        public int COMMITTEE_PROPERTY_ID { get; set; }
        public string COMMITTEE_PROPERTY_NANE { get; set; }
        public string PROPERTY_SYS_NANE { get; set; }
    }
    #endregion

    #region CommitteePropertiesMap
    public class CommitteePropertiesMap
    {
        public int MAP_ID { get; set; }
        public int COMMITTEE_ID { get; set; }
        public string COMMITTEE_NANE { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public int COMMITTEE_PROPERTIES_ID { get; set; }
        public string COMMITTEE_PROPERTY_NANE { get; set; }
        public string PROPERTY_SYS_NANE { get; set; }
        public decimal MIN_VALUE { get; set; }
        public decimal MAX_VALUE { get; set; }
    }
    #endregion

    #region CommitteeUserMap
    public class CommitteeUserMap
    {
        public int MAP_ID { get; set; }
        public int COMMITTEE_ID { get; set; }
        public string COMMITTEE_NANE { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public int COMMITTEE_USER_ID { get; set; }
        public string COMMITTEE_USER_NAME { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public bool IS_COMMITTEE_MEMBER { get; set; }
    }
    #endregion

    #region UserCommittee
    public class UserCommittee
    {
        public int MAP_ID { get; set; }
        public int COMMITTEE_ID { get; set; }
        public string COMMITTEE_NANE { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public bool IS_COMMITTEE_MEMBER { get; set; }
        public bool IS_ACTIVE { get; set; }

    }
    #endregion

    #endregion

    #region Country
    public class Country
    {
        public string COUNTRY_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
    }
    public class CountryCodeMap
    {
        public string CODE2 { get; set; }
        public string CODE3 { get; set; }
    }
    #endregion

    #region CrediInfoGrade
    public class CrediInfoGrade
    {
        public int CREDIT_INFO_ID { get; set; }
        public string CREDIT_INFO_NAME { get; set; }
    }
    #endregion

    #region CreditHistoryKind
    public class CreditHistoryKind
    {
        public int CREDIT_HISTORY_KIND_ID { get; set; }
        public string CREDIT_HISTORY_KIND_NAME { get; set; }
    }
    #endregion

    #region CreditType
    public class CreditType
    {
        public int CREDIT_TYPE_ID { get; set; }
        public string CREDIT_TYPE_NAME { get; set; }
    }
    #endregion

    #region Currency
    public class Currency
    {
        public string CCY { get; set; }
        public int DIGITS { get; set; }
        public string CCY_NAME { get; set; }
        public string UNIT_NAME { get; set; }
        public string CENT_NAME { get; set; }
        public int CODE { get; set; }
        public int WEIGHT { get; set; }
        public int RATE_ITEMS { get; set; }
    }
    #endregion

    #region Employee
    public class Employee
    {
        public int EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
    }
    #endregion

    #region EnshureType
    public class EnshureType
    {
        public int ENSURE_TYPE_ID { get; set; }
        public string ENSURE_TYPE_NAME { get; set; }
    }
    #endregion

    #region == Loan Exception ==

    #region ExceptionLoanPartipication
    public class ExceptionLoanPartipication
    {
        public int PRODUCT_ID { get; set; }
        public int SALARY_CATEGORY { get; set; }
        public decimal PARTIPICATION_MIN { get; set; }
    }
    #endregion

    #region ExceptionLoanPrepaiment
    public class ExceptionLoanPrepaiment
    {
        public int PRODUCT_ID { get; set; }
        public decimal INTEREST_MIN { get; set; }
        public decimal INTEREST_MAX { get; set; }
        public decimal PREPAIMENT { get; set; }

    }
    #endregion

    #region ExceptionProdOrg
    public class ExceptionProdOrg
    {
        public int PRODUCT_ID { get; set; }
        public int VIP_ORGANIZATION_ID { get; set; }
        public decimal COMMISSION_MIN { get; set; }
        public decimal COMMISSION_MAX { get; set; }
    }
    #endregion

    #region ExceptionProdRanges
    public class ExceptionProdRanges
    {
        public int PRODUCT_ID { get; set; }
        public decimal INTEREST_MIN { get; set; }
        public decimal INTEREST_MAX { get; set; }
        public decimal COMMISSION_MIN { get; set; }
        public decimal COMMISSION_MAX { get; set; }
    }
    #endregion

    #region ExceptionAutoPartipicationRanges
    public class ExceptionAutoPartipicationRanges
    {
        public int PRODUCT_ID { get; set; }
        public string BRANCH_ID { get; set; }
        public int AUTO_AGE_MIN { get; set; }
        public int AUTO_AGE_MAX { get; set; }
        public int PRODUCT_CATEGORY_ID { get; set; }
        public string PRODUCT_CATEGORY_NAME { get; set; }
        public decimal PARTIPICATION_RATE { get; set; }
    }
    #endregion

    #region LoanException
    public class LoanException
    {
        public List<ExceptionLoanPartipication> ExceptionLoanPartipicationList { get; set; }
        public List<ExceptionLoanPrepaiment> ExceptionLoanPrepaimentList { get; set; }
        public List<ExceptionProdOrg> ExceptionProdOrgList { get; set; }
        public List<ExceptionProdRanges> ExceptionProdRangesList { get; set; }
        public List<ExceptionAutoPartipicationRanges> ExceptionAutoPartipicationRangesList { get; set; }
    }
    #endregion

    #endregion

    #region IncomeType
    public class IncomeType
    {
        public int INCOME_TYPE_ID { get; set; }
        public string INCOME_TYPE_NAME { get; set; }
    }
    #endregion

    #region IncomeVerified
    public class IncomeVerified
    {
        public int INCOME_VERIFIED_ID { get; set; }
        public string INCOME_VERIFIED_NAME { get; set; }
    }
    #endregion

    #region InformationSource
    public class InformationSource
    {
        public int INFORMATION_SOURCE_ID { get; set; }
        public string INFORMATION_SOURCE_NAME { get; set; }
    }
    #endregion

    #region LoanAim
    public class LoanAim
    {
        public int LOAN_AIM_ID { get; set; }
        public string LOAN_AIM_NAME { get; set; }
    }
    #endregion

    #region LoanType
    public class LoanType
    {
        public int LOAN_TYPE_ID { get; set; }
        public string LOAN_TYPE_NAME { get; set; }
    }
    #endregion

    #region MaritalStatus
    public class MaritalStatus
    {
        public int MARITAL_STATUS_ID { get; set; }
        public string MARITAL_STATUS_NAME { get; set; }
    }

    #endregion

    #region Organization
    public class Organization
    {
        public int ORGANIZATION_ID { get; set; }
        public string ORGANIZATION_TAX_CODE { get; set; }
        //public string ORGANIZATION_TYPE { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public int CLIENT_CATEGORY_ID { get; set; }
        public string CLIENT_CATEGORY_NAME { get; set; }
        public int STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
        public int CLIENT_TYPE_ID { get; set; }
        public string CLIENT_TYPE_NAME { get; set; }
        public int SEB_CLASS_ID { get; set; }
    }
    #endregion

    #region OrgType
    public class OrgType
    {
        public string ORG_TYPE { get; set; }
        public string DESCRIP { get; set; }
    }
    #endregion

    #region PassportType
    public class PassportType
    {
        public int PASSPORT_TYPE_ID { get; set; }
        public string PASSPORT_TYPE_NAME { get; set; }
    }
    #endregion

    #region PenaltyCalculationType
    public class PenaltyCalculationType
    {
        public int PENALTY_CALCULATION_TYPE_ID { get; set; }
        public string PENALTY_CALCULATION_TYPE_NAME { get; set; }
    }
    #endregion

    #region PenaltySchema
    public class PenaltySchema
    {
        public int PENALTY_SCHEMA_ID { get; set; }
        public string PENALTY_SCHEMA_NAME { get; set; }
    }
    #endregion

    #region PmtFreqType
    public class PmtFreqType
    {
        public int PMT_FREQ_TYPE_ID { get; set; }
        public string PMT_FREQ_TYPE_NAME { get; set; }
        public decimal MONTHS { get; set; }
        public int PMT_INTERVAL_IS_IN_MONTHS { get; set; }
        public decimal PMT_INTERVAL { get; set; }
        public bool SHOW_PAYMENT_DAY_1 { get; set; }
        public bool SHOW_PAYMENT_DAY_2 { get; set; }
    }
    #endregion

    #region PrepaymentRescheduleType
    public class PrepaymentRescheduleType
    {
        public int PREPAYMENT_RESCHEDULE_TYPE_ID { get; set; }
        public string PREPAYMENT_RESCHEDULE_TYPE_NAME { get; set; }
    }
    #endregion

    #region == Product	==

    #region Product
    public class Product
    {
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
        public bool IS_CASH_COVER { get; set; }
        public int SCORING_TYPE_ID { get; set; }
        public int CALCULATOR_TYPE_ID { get; set; }
        public string CALCULATOR_TYPE_SYS_NAME { get; set; }
        public int REVIEW_PERIOD { get; set; }
        public int PRINT_TYPE_ID { get; set; }
        public int BASE_PRODUCT_ID { get; set; }
        public DateTime DATE_START { get; set; }
        public DateTime DATE_END { get; set; }
        public bool IS_ACTION { get; set; }
        public string IS_ACTION_NAME { get; set; }
        public decimal DISCOUNT { get; set; }
        public int ADMIN_FEE_TYPE { get; set; }
        public bool IS_CHECKED { get; set; }
    }
    #endregion

    #region ProductCategory
    public class RefProductCategory
    {
        public int PRODUCT_CATEGORY_ID { get; set; }
        public string PRODUCT_CATEGORY_NAME { get; set; }
        public string PRODUCT_CATEGORY_SYS_NAME { get; set; }
    }

    public class ProductCategory
    {
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int PRODUCT_CATEGORY_ID { get; set; }
        public string PRODUCT_CATEGORY_NAME { get; set; }
    }
    #endregion

    #region ProductCategoryMap
    public class ProductCategoryMap
    {
        public int MAP_ID { get; set; }
        public int BUSINESS_ID { get; set; }
        public string BUSINESS_NAME { get; set; }
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public bool IS_CASH_COVER { get; set; }
        public int PRODUCT_CATEGORY_ID { get; set; }
        public string PRODUCT_CATEGORY_NAME { get; set; }
        public int PRODUCT_EXTENDED_ID { get; set; }
        public string PRODUCT_EXTENDED_NAME { get; set; }
    }
    #endregion

    #region ProductDetails
    public class ProductDetails
    {
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public bool IS_CASH_COVER { get; set; }
        public int SCORING_TYPE_ID { get; set; }
        public decimal DISCOUNT { get; set; }
        public int CLIENT_TYPES { get; set; }
        public int SCHEDULE_TYPES { get; set; }
        public int PMT_INTERVAL_TYPES { get; set; }
        public string RESOURCES { get; set; }
        public int ENSURE_TYPES { get; set; }
        public int PREPAYMENT_RESCHEDULE_TYPE { get; set; }
        public decimal OVER_LIMIT_INTEREST_RATE { get; set; }
        public int LOAN_TYPE { get; set; }
        public int CREDIT_TYPE { get; set; }
        public bool IS_CARD { get; set; }
        public bool GENERIC_SCHEDULE { get; set; }
        public int PAYMENT_DAY_FROM { get; set; }
        public int PAYMENT_DAY_TO { get; set; }
        public bool HAS_PAYMENT_DAY { get; set; }
        public bool HAS_INTEREST_FREE_PERIOD { get; set; }
        public int INTEREST_FREE_PERIOD_FROM { get; set; }
        public int INTEREST_FREE_PERIOD_TO { get; set; }
        public int TERM_TYPE { get; set; }
        public bool CAN_HAVE_COBORROWER { get; set; }
        public bool HAS_GRACE_DAYS { get; set; }
        public bool HAS_PURPOSE { get; set; }
        public string ROLE_ID { get; set; }
        public bool HAS_GRACE_HOLIDAYS { get; set; }
        public string COLLATERALS { get; set; }
        public bool IS_INSTALLMENT { get; set; }
        public bool HAS_USAGE_PURPOSE { get; set; }
        public bool HAS_BUSINESSES { get; set; }
        public bool CONTROL_LIMITS { get; set; }

    }
    #endregion

    #region ProductGroups
    public class ProductGroups
    {
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public int BUSINESS_ID { get; set; }
        public string BUSINESS_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #region ProductProperties
    public class ProductProperties
    {
        public int PRODUCT_PROPERTIES_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public int BASE_PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string CCY { get; set; }
        public decimal MIN_AMOUNT { get; set; }
        public decimal MAX_AMOUNT { get; set; }
        public int MIN_PERIOD { get; set; }
        public int MAX_PERIOD { get; set; }
        public decimal INTEREST_RATE_FROM { get; set; }
        public decimal INTEREST_RATE_TO { get; set; }
        public decimal NOTUSED_RATE_FROM { get; set; }
        public decimal NOTUSED_RATE_TO { get; set; }
        public decimal PREPAYMENT_RATE_FROM { get; set; }
        public decimal PREPAYMENT_RATE_TO { get; set; }
        public decimal ADMIN_FEE_PERCENT_FROM { get; set; }
        public decimal ADMIN_FEE_PERCENT_TO { get; set; }
        public decimal ADMIN_FEE_MIN { get; set; }
        public decimal FEE1_PERCENT_FROM { get; set; }
        public decimal FEE1_PERCENT_TO { get; set; }
        public decimal FEE1_MIN { get; set; }
        public decimal FEE2_PERCENT_FROM { get; set; }
        public decimal FEE2_PERCENT_TO { get; set; }
        public decimal FEE2_MIN { get; set; }
        public decimal FEE3_PERCENT_FROM { get; set; }
        public decimal FEE3_PERCENT_TO { get; set; }
        public decimal FEE3_MIN { get; set; }
        public int GRACE_PERIOD_FROM { get; set; }
        public int DECISION_SOURCE { get; set; }
        public int COMMITTEE_ID { get; set; }
        public int GRACE_PERIOD_TO { get; set; }
        public int MIN_PREPAYMENT_COUNT { get; set; }
        public decimal MIN_PREPAYMENT_AMOUNT { get; set; }
        public int PENALTY_CALCULATION_TYPE { get; set; }
        public int PERCENT_PREPAYMENT_TYPE { get; set; }
        public decimal OVERPAY_PREPAYMENT_RATE_FROM { get; set; }
        public decimal OVERPAY_PREPAYMENT_RATE_TO { get; set; }
        public int OVERPAY_PENALTY_CALCULATION_TYPE { get; set; }
        public int OVERPAY_PERCENT_PREPAYMENT_TYPE { get; set; }
        public int PENALTY_SCHEMA_ID { get; set; }
        public decimal PENALTY_INEREST_RATE { get; set; }
        public decimal PENALTY_FIXED_AMOUNT { get; set; }
        public decimal PENALTY_ON_PAYMENT_IN_OTHER_BANK_FROM { get; set; }
        public decimal PENALTY_ON_PAYMENT_IN_OTHER_BANK_TO { get; set; }
        public int OB_OVERPAY_PERCENT_PREPAYMENT_TYPE { get; set; }
        public int OB_OVERPAY_PENALTY_CALCULATION_TYPE { get; set; }
        public decimal MAX_SALARY_AMOUNT { get; set; }
        public decimal GENERAL_AGGREMENT_AMOUNT { get; set; }
        public int GENERAL_AGGREMENT_PERIOD_YAER { get; set; }
        public decimal PARTICIPATION_MIN { get; set; }
        public decimal PARTICIPATION_MAX { get; set; }
        public int PRODUCT_CATEGORY_ID { get; set; }
        public string PRODUCT_CATEGORY_NAME { get; set; }
        public int PRODUCT_EXTENDED_ID { get; set; }
        public string PRODUCT_EXTENDED_NAME { get; set; }

    }
    #endregion

    #endregion

    #region BaseProductPurpose
    public class BaseProductPurpose
    {
        public int BASE_PRODUCT_ID { get; set; }
        public string PURPOSE_ID { get; set; }
    }
    #endregion BaseProductPurpose

    #region RealEstateType
    public class RealEstateType
    {
        public string REAL_ESTATE_TYPE_ID { get; set; }
        public string REAL_ESTATE_TYPE_NAME { get; set; }
        public int REAL_ESTATE_GROUP { get; set; }
    }
    #endregion RealEstateType


    #region RealEstateOwnerType
    public class RealEstateOwnerType
    {
        public int REAL_ESTATE_OWNER_TYPE_ID { get; set; }
        public string REAL_ESTATE_OWNER_TYPE_NAME { get; set; }
    }
    #endregion RealEstateOwnerType

    #region ClientPosition

    public class ClientPosition
    {
        public string POSITION_ID { get; set; }
        public string POSITION_NAME { get; set; }
    }
    #endregion

    #region RealEstateMaxLTV
    public class RealEstateMaxLTV
    {
        public int PRODUCT_ID { get; set; }
        public string CCY { get; set; }
        public decimal MAX_LTV { get; set; }
    }
    #endregion

    #region RealEstateInsurance
    public class RealEstateInsurance
    {
        public int PRODUCT_ID { get; set; }
        public int REAL_ESTATE_GROUP { get; set; }
        public int INSURANCE_LIFE { get; set; }
        public int INSURANCE_REAL_ESTATE { get; set; }
    }
    #endregion RealEstateInsurance

    #region RejectReason
    public class RejectReason
    {
        public int REJECT_REASON_ID { get; set; }
        public string REJECT_REASON_NAME { get; set; }
    }
    #endregion

    #region RelationshipType
    public class RelationshipType
    {
        public int RELATIONSHIP_TYPE_ID { get; set; }
        public string RELATIONSHIP_TYPE_NAME { get; set; }
    }
    #endregion

    #region RestructType
    public class RestructType
    {
        public int RESTRUCT_TYPE_ID { get; set; }
        public string RESTRUCT_TYPE_NAME { get; set; }
    }
    #endregion

    #region RoleType
    public class RoleType
    {
        public int ROLE_TYPE_ID { get; set; }
        public string ROLE_TYPE_NAME { get; set; }
    }
    #endregion

    #region ScheduleType
    public class ScheduleType
    {
        public int SCHEDULE_TYPE_ID { get; set; }
        public string SCHEDULE_TYPE_NAME { get; set; }
    }
    #endregion

    #region ScoringParamKoef
    public class ScoringParamCoef
    {
        public int REC_ID { get; set; }
        public int SCORING_TYPE_ID { get; set; }
        public int PARAM_ID { get; set; }
        public string PARAM_NAME { get; set; }
        public string PARAM_DESCRIP { get; set; }
        public int PARAM_VALUE_ID { get; set; }
        public int PARAM_VALUE { get; set; }
        public string PARAM_VALUE_NAME { get; set; }
        public double PARAM_KOEF { get; set; }
    }
    #endregion

    #region Sex
    public class Sex
    {
        public int SEX_ID { get; set; }
        public string SEX_NAME { get; set; }
    }
    #endregion

    #region StateVisibilityMap
    public class StateVisibilityMap
    {
        public int MAP_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public string WORK_PLACE_NAME { get; set; }
        public string WORK_PLACE_SYS_NAME { get; set; }
        public int STATE_ID { get; set; }
        public string APPLICATION_STATE_NAME { get; set; }
        public string APPLICATION_STATE_SYS_NAME { get; set; }
        public int EDITING_MODE { get; set; }
    }
    #endregion

    #region Status
    public class Status
    {
        public int STATUS_ID { get; set; }
        public string STATUS_NAME { get; set; }
    }
    #endregion

    #region ==== Sys ====

    #region SysCalcParameter
    public class SysCalcParameter
    {
        public int PRODUCT_ID { get; set; }
        public int CALCULATOR_TYPE_ID { get; set; }
        public string CALCULATOR_TYPE_SYS_NAME { get; set; }
        public int PARAMETER_ID { get; set; }
        public string PARAMETER_SYS_NAME { get; set; }
        public string PARAMETER_VALUE { get; set; }
    }
    #endregion

    #region SysEvent
    public class SysEvent
    {
        public int APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE_NAME { get; set; }
        public string APPLICATION_TYPE_SYSNAME { get; set; }
        public int EVENT_GROUP_ID { get; set; }
        public string EVENT_GROUP_NAME { get; set; }
        public string EVENT_GROUP_SYS_NAME { get; set; }
        public int EVENT_ID { get; set; }
        public string EVENT_NAME { get; set; }
        public string EVENT_SYS_NAME { get; set; }
    }
    #endregion

    #region SysEventGroup
    public class SysEventGroup
    {
        public int EVENT_GROUP_ID { get; set; }
        public string EVENT_GROUP_NAME { get; set; }
        public string EVENT_GROUP_SYS_NAME { get; set; }
        public int APPLICATION_TYPE_ID { get; set; }

    }
    #endregion

    #region SysParameter
    public class SysParameter
    {
        public int APPLICATION_TYPE_ID { get; set; }
        public int BUSINESS_ID { get; set; }
        public int PARAMETER_ID { get; set; }
        public string PARAMETER_NAME { get; set; }
        public string PARAMETER_SYS_NAME { get; set; }
        public string PARAMETER_VALUE { get; set; }
        public int INSURANCE_COMPANY_ID { get; set; }
    }
    #endregion

    #region == Sys User	==

    #region SysUser
    public class SysUser
    {
        public string BRANCH_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public int USER_ID { get; set; }
        public string LOGIN_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public bool IS_DISABLED { get; set; }
        public string DOMAIN_USER_NAME { get; set; }
        public string PASSWORD { get; set; }
        public DateTime PASSWORD_CHANGE_DATE { get; set; }
        public DateTime LAST_LOGIN { get; set; }
        public int LOGIN_COUNT { get; set; }
        public int USER_GROUP_ID { get; set; }
        public string USER_GROUP_SYSNAME { get; set; }
    }
    #endregion

    #region SysUserEvents
    public class SysUserEvents
    {
        public int APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE_NAME { get; set; }
        public string APPLICATION_TYPE_SYSNAME { get; set; }
        public int EVENT_GROUP_ID { get; set; }
        public string EVENT_GROUP_NAME { get; set; }
        public string EVENT_GROUP_SYS_NAME { get; set; }
        public int EVENT_ID { get; set; }
        public string EVENT_NAME { get; set; }
        public string EVENT_SYS_NAME { get; set; }
        public bool HAVE_RIGTH { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #region SysUserGroup
    public class SysUserGroup
    {
        public int USER_GROUP_ID { get; set; }
        public string USER_GROUP_NAME { get; set; }
        public bool EDITING_MODE { get; set; }
    }
    #endregion

    #region SysUserGroupEventMap
    public class SysUserGroupEventMap
    {
        public int MAP_ID { get; set; }
        public int USER_GROUP_ID { get; set; }
        public int EVENT_ID { get; set; }
        public string EVENT_NAME { get; set; }
        public int EVENT_GROUP_ID { get; set; }
        public string EVENT_GROUP_NAME { get; set; }
        public int APPLICATION_TYPE_ID { get; set; }
        public string APPLICATION_TYPE_NAME { get; set; }
        public string APPLICATION_TYPE_SYSNAME { get; set; }
    }
    #endregion

    #region SysUserGroupMap
    public class SysUserGroupMap
    {
        public int MAP_ID { get; set; }
        public int USER_GROUP_ID { get; set; }
        public string USER_GROUP_NAME { get; set; }
        public string USER_GROUP_SYSNAME { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #region SysUserParameters
    public class SysUserParameters
    {
        public SysUser User { get; set; }
        public List<SysUserWorkPlace> WorkPlaceList { get; set; }
        public List<UserCommittee> CommitteeList { get; set; }
        public List<CommitteeDelegate> CommitteeDelegateList { get; set; }
        public List<SysUserGroupMap> GroupList { get; set; }
        public List<SysUserEvents> EventsList { get; set; }
    }
    #endregion

    #region SysUserState
    public class SysUserState
    {
        public int USER_ID { get; set; }
        public bool IS_LOGIN { get; set; }
        public int ERROR { get; set; }
        public string ERROR_MESSAGE { get; set; }
    }
    #endregion

    #region SysUserWorkPlace
    public class SysUserWorkPlace
    {
        public int MAP_ID { get; set; }
        public string WORK_PLACE_ID { get; set; }
        public string WORK_PLACE_NAME { get; set; }
        public string WORK_PLACE_SYS_NAME { get; set; }
        public int USER_ID { get; set; }
        public string FULL_NAME { get; set; }
        public string BRANCH_ID { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #endregion

    #endregion

    #region TermType
    public class TermType
    {
        public int TERM_TYPE_ID { get; set; }
        public string TERM_TYPE_NAME { get; set; }
    }
    #endregion

    #region ValRates
    public class ValRates
    {
        public string ISO { get; set; }
        public int ITEMS { get; set; }
        public double AMOUNT { get; set; }
        public double AMOUNT_BUY { get; set; }
        public double AMOUNT_SELL { get; set; }
    }
    #endregion

    #region WorkPlace
    public class WorkPlace
    {
        public int WORK_PLACE_ID { get; set; }
        public string WORK_PLACE_NAME { get; set; }
        public string WORK_PLACE_SYS_NAME { get; set; }
    }
    #endregion

    #region YearMonth
    public class YearMonth
    {
        public int YEAR_MONTH_ID { get; set; }
        public string YEAR_MONTH_NAME { get; set; }
    }
    #endregion

    #region WorkDay
    public class WorkDay
    {
        public DateTime WORK_DAY { get; set; }
        public int DAY_SHIFT { get; set; }
    }
    #endregion

    #region InsuranceProdBAucts
    public class InsuranceProducts
    {
        public int INSURANCE_PRODUCT_ID { get; set; }
        public string INSURANCE_PRODUCT_NAME { get; set; }
        public int TYPE_ID { get; set; }
        public string LOAN_PRODUCT_ID { get; set; }
        public int CLIENT_TYPES { get; set; }
        public int COLLATERAL_TYPE { get; set; }
        public int COMPANY_ID { get; set; }
        public string ACCOUNT { get; set; }
        public int IS_HEAD_BRANCH { get; set; }
        public int AMOUNT_TYPE { get; set; }
        public decimal MIN_AMOUNT { get; set; }
        public decimal MAX_AMOUNT { get; set; }
        public decimal MIN_RATE_CLIENT { get; set; }
        public decimal MAX_RATE_CLIENT { get; set; }
        public int AUTO_GENERATE_POLICE { get; set; }
        public decimal RATE_MIN_AMOUNT_CLIENT { get; set; }
        public decimal MIN_RATE_BANK { get; set; }
        public decimal MAX_RATE_BANK { get; set; }
        public decimal RATE_MIN_AMOUNT_BANK { get; set; }
        public int AUTHORIZED { get; set; }
        public int ACC_ID { get; set; }
        public int ALLOW_BLANK_POLICE_NO { get; set; }
        public string DATA { get; set; }
        public string COMPANY_NAME { get; set; }
        public string PRODUCT_PROPERTY_ID { get; set; }
        public bool IS_ACTIVE { get; set; }
    }
    #endregion

    #region Disburs Type
    public class DisbursType
    {
        public int DISBURS_TYPE_ID { get; set; }
        public string DISBURS_TYPE_NAME { get; set; }
    }
    #endregion

    #region PictureObjectType
    public class PictureObjectType
    {
        public int OBJECT_TYPE_ID { get; set; }
        public string OBJECT_TYPE_NAME { get; set; }
        public string OBJECT_TYPE_SYSNAME { get; set; }
    }
    #endregion

    #region ProductAttribute
    public class ProductAttribute
    {
        public int PRODUCT_ID { get; set; }
        public int CLIENT_SALARY_CATEGORY { get; set; }
        public string ATTRIB_CODE { get; set; }
        public string ATTRIB_VALUE { get; set; }
        public string SYSTEM_DATA_TYPE { get; set; }
    }
    #endregion ProductAttribute

    #region AdditionalAttribute
    public class AdditionalAttribute
    {
        public int ATTRIBUTE_ID { set; get; }
        public string ATTRIBUTE_CODE { set; get; }
        public string ATTRIBUTE_NAME { set; get; }
        public string PRODUCT_ID { set; get; }
        public int TYPE { set; get; }
        public int CCY_TYPE { set; get; }
        public int PERIOD_TYPE { set; get; }
        public int INTERVAL_TYPE { set; get; }
        public int INTERVAL_STEP { set; get; }
        public bool IS_ACTIVE { set; get; }
        public bool IS_REQUIRED { set; get; }
        public string DEFAULT_VALUE { set; get; }
        public int ATTRIBUTE_TYPE_ID { set; get; }
        public string ATTRIBUTE_TYPE_NAME { set; get; }
        public string DATA_TYPE { set; get; }
    }
    #endregion ProductAttribute

    #region Instalments
    public class Installment
    {
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
        public string LOCATION { set; get; }
        public string ADDRESS { set; get; }
        public string CODE { set; get; }
        public string BANK_CODE { set; get; }
        public int ACCOUNT_ID { set; get; }
        public string CCY { set; get; }
        public string ACCOUNT_NO { set; get; }
        public string ACCOUNT_NAME { set; get; }
        public string TAX_CODE { set; get; }
        public decimal DISCOUNT { set; get; }
        public bool AUTHORIZED { set; get; }
        public string PRODUCTS { set; get; }
        public int PARENT_ID { set; get; }
        public int DISCOUNT_TYPE { set; get; }
        public bool IS_CHECKED { set; get; }
    }

    public class InstallmentForMap
    {
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
        public bool IS_CHECKED { set; get; }
    }

    public class InstallmentProductForMap
    {
        public int INSTALLMENT_ID { set; get; }
        public int PRODUCT_ID { set; get; }
        public string PRODUCT_NAME { set; get; }
        public decimal DISCOUNT { set; get; }
        public bool IS_STANDART { set; get; }
        public bool IS_USED { set; get; }
    }
    #endregion Installment

    #region InstalmentsUser
    public class InstalmentsUser
    {
        public int USER_ID { set; get; }
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
    }

    public class SaleUserInstalment
    {
        public int SALE_USER_ID { set; get; }
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
        public bool IS_ACTIVE { set; get; }
    }
    public class UserInstalmentsProduct
    {
        public int USER_ID { set; get; }
        public int INSTALLMENT_ID { set; get; }
        public int PRODUCT_ID { set; get; }
        public string PRODUCT_NAME { set; get; }
    }
    #endregion InstallmentUser

    #region InstalmentsUser
    public class ApplicationOwnerInstalment
    {
        //public int MAP_ID { set; get; }
        public int USER_ID { set; get; }
        //public string FULL_NAME { set; get; }
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
        public string ADDRESS { set; get; }

    }
    #endregion InstallmentUser

    #region ProductInstallment
    public class ProductInstallment
    {
        public int MAP_ID { set; get; }
        public int PRODUCT_ID { set; get; }
        public string PRODUCT_NAME { set; get; }
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
    }
    #endregion InstallmentUser

    #region ProductInstallmentDiscount
    public class ProductInstallmentDiscount
    {
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
        public int PRODUCT_ID { set; get; }
        public string PRODUCT_NAME { set; get; }
        public int MIN_PERIOD { set; get; }
        public int MAX_PERIOD { set; get; }
        public decimal DISCOUNT_RATE { set; get; }
        public int DISCOUNT_TYPE { set; get; }
        public int GRACE_PERIOD_FROM { set; get; }
        public int GRACE_PERIOD_TO { set; get; }
    }
    #endregion InstallmentUser

    #region === Product Action ===
    public class ProductActionBranch
    {
        public string BRANCH_ID { set; get; }
        public string BRANCH_NAME { set; get; }
        public int PRODUCT_ID { set; get; }
        public bool PRODUCT_USED { set; get; }
    }

    public class ProductActionInstalment
    {
        public int INSTALLMENT_ID { set; get; }
        public string INSTALLMENT_NAME { set; get; }
        public int PRODUCT_ID { set; get; }
        public bool PRODUCT_USED { set; get; }
    }

    public class PurchaseItemGroup
    {
        public int ITEM_GROUP_ID { set; get; }
        public string ITEM_GROUP_NAME { set; get; }
    }

    public class PurchaseItem
    {
        public int ITEM_GROUP_ID { set; get; }
        public string ITEM_GROUP_NAME { set; get; }
        public int ITEM_ID { set; get; }
        public string ITEM_NAME { set; get; }
    }

    #endregion

    #region === BlackList ===
    public class BlackList
    {
        public int REC_ID { set; get; }
        public int ITEM_ID { set; get; }
        public string ITEM_NAME { set; get; }
        public string ITEM_VALUE { set; get; }
        public int USER_ID { set; get; }
        public string USER_NAME { set; get; }
        public DateTime CHANGE_DATE { set; get; }
        public bool IS_ACTIVE { set; get; }
        public string REMARK { set; get; }
        public int RECORDER_TYPE_ID { set; get; }
        public string RECORDER_TYPE_NAME { set; get; }
    }

    public class BlackListItem
    {
        public int ITEM_ID { set; get; }
        public string ITEM_NAME { set; get; }
    }
    #endregion BlackList

    #region === AdminDebt ===
    #region AdminDebtTypes1
    public class AdminDebtTypes1
    {
        public int ID { get; set; }
        public int OBJECT_TYPE_ID { get; set; }
        public string DESCRIPTION { get; set; }

    }
    #endregion AdminDebtTypes1

    #region AdminDebtTypes2
    public class AdminDebtTypes2
    {
        public int ID { get; set; }
        public int OBJECT_TYPE_ID { get; set; }
        public string DESCRIPTION { get; set; }

    }
    #endregion AdminDebtTypes2

    #region AdminDebtObjects
    public class AdminDebtObjects
    {
        public int OBJECT_TYPE_ID { get; set; }
        public string OBJECT_TYPE_NAME { get; set; }
    }
    #endregion AdminDebtObjects
    #endregion === AdminDebt ===

    #region === AdminDebt NEW ===
    #region AdminDebtGroup
    public class AdminDebtGroup
    {
        public int GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }

    }
    #endregion AdminDebtGroup

    #region AdminDebtItem
    public class AdminDebtItem
    {
        public int GROUP_ID { get; set; }
        public int ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public decimal AMOUNT { get; set; }

    }

    public class AdminDebtItemFull
    {
        public int GROUP_ID { get; set; }
        public int ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public int LMS_OBJECT_TYPE_ID { get; set; }
        public int LMS_TYPE1_ID { get; set; }
        public int LMS_TYPE2_ID { get; set; }


    }
    #endregion AdminDebtItem
    public class CreditBank
    {
        public string BANK_CODE { get; set; }
        public string BANK_NAME { get; set; }
        public bool NEED_COMMENT { get; set; }
    }
    #endregion === AdminDebt NEW ===


    #region Banks

    #endregion Banks

    #region CashCoverScheduleType
    public class CashCoverScheduleType
    {
        public int SCHEDULE_ID { set; get; }
        public string SCHEDULE_NAME { set; get; }
        public string SCHEDULE_SYSNAME { set; get; }
    }
    #endregion CashCoverScheduleType

    #region MapScoringTypeProperty
    public class MapScoringTypeProperty
    {
        public int SCORING_TYPE_ID { set; get; }
        public int SCORING_PROPERTY_ID { set; get; }
        public string SCORING_PROPERTY_SYS_NAME { set; get; }
    }
    #endregion MapScoringTypeProperty

    #region SEB_CLASS
    public class SebClass
    {
        public int SEB_CLASS_ID { set; get; }
        public string SEB_CLASS_NAME { set; get; }
    }
    #endregion SEB_CLASS

    #region === MFiles ===
    public class MFBranchMap
    {
        public int BRANCH_ID { set; get; }
        public int MF_BRANCH_ID { set; get; }
        public string MF_BRANCH_NAME { set; get; }
    }

    public class MFUserMap
    {
        public int USER_ID { set; get; }
        public int MF_USER_ID { set; get; }
        public string MF_USER_NAME { set; get; }
    }

    public class MFCaseProductMap
    {
        public int BASE_PRODUCT_ID { set; get; }
        public int CASE_TEMPLATE_ID { set; get; }
    }

    public class MFCaseNotClosed
    {
        public int AdminID { get; set; }
        public int AppID { get; set; }
        public int CaseID { get; set; }
    }

    public class MFCaseNotCloseDet
    {
        public int APPLICATION_ID { get; set; }
        public int MFILE_CASE_ID { get; set; }
        public string CLIENT_NAME { get; set; }
        public string CLIENT_PERSONAL_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string BRANCH_NAME { get; set; }
    }
    #endregion

    #region NONSTANDARD_TYPE
    public class NonStandardType
    {
        public int NONSTANDARD_TYPE_ID { set; get; }
        public string NONSTANDARD_TYPE_NAME { set; get; }
    }
    #endregion NONSTANDARD_TYPE

    #region === XIRR ===
    public class CreditCardXIRR
    {
        public int APPLICATION_ID { get; set; }
        public decimal AMOUNT_LOAN { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public decimal AMOUNT_MAKING { get; set; }
        public decimal AMOUNT_CASH_COMMISSION { get; set; }
        public DateTime CLIENT_ACTIVITY_DATE { get; set; }
        public bool IS_LIMIT { get; set; }
        public int LIMIT_DAYS { get; set; }
        public decimal XIRR { get; set; }
        public decimal XIRR_15 { get; set; }
    }

    public class POSCardXIRR
    {
        public int APPLICATION_ID { get; set; }
        public decimal AMOUNT_LOAN { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public decimal COMMISSION_RATE { get; set; }
        public int LOAN_MONTHS { get; set; }
        public decimal AMOUNT_ACCOUNT { get; set; }
        public DateTime CLIENT_ACTIVITY_DATE { get; set; }
        public decimal XIRR { get; set; }
    }

    public class XIRR_Result
    {
        public int APPLICATION_ID { get; set; }
        public int XIRR_TYPE_ID { get; set; }
        public string XIRR_TYPE_NAME { get; set; }
        public string XIRR_TYPE_SYSNAME { get; set; }
        public decimal AMOUNT_LOAN { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public decimal AMOUNT_MAKING { get; set; }
        public decimal AMOUNT_CASH { get; set; }
        public bool IS_LIMIT { get; set; }
        public int LIMIT_DAYS { get; set; }
        public decimal XIRR { get; set; }
        public DateTime CLIENT_ACTIVITY_DATE { get; set; }
        public decimal CASH_RATE { get; set; }
        public decimal CASH_AMOUNT_MIN { get; set; }
    }

    public class OnlineXIRR
    {
        public int PRODUCT_ID { get; set; }
        public decimal AMOUNT_LOAN { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public int LOAN_MONTHS { get; set; }
        public DateTime DATE_START { get; set; }
        public DateTime DATE_END { get; set; }
        public decimal PMT_AMOUNT { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public decimal CALC_XIRR { get; set; }
    }

    #endregion

    #region INSURANCE_MANDATORY
    public class InsuranceMandatory
    {
        public int BASE_PRODUCT_ID { set; get; }
        public bool IS_LIFE { set; get; }
        public DateTime START_DATE { set; get; }
        public DateTime END_DATE { set; get; }
    }
    #endregion INSURANCE_MANDATORY

    #region POSITION_TYPE
    public class ClientPositionTtype
    {
        public int POSITION_TYPE_ID { set; get; }
        public string POSITION_TYPE_NAME { set; get; }

    }
    #endregion POSITION_TYPE

    #region InsuranceMandatoReesult
    public class InsuranceMandatoReesult
    {
        public bool IS_LIFE { set; get; }
        public int COMPANY_ID { set; get; }
        public decimal AMOUNT { set; get; }
        public decimal MIN_AMOUNT { set; get; }

    }
    #endregion InsuranceMandatoReesult

    #region Rus Scoring References
    public class Education
    {
        public int EDUCATION_ID { set; get; }
        public string EDUCATION_NAME { set; get; }

    }

    public class PostCategory
    {
        public int POST_CATEGORY_ID { set; get; }
        public string POST_CATEGORY_NAME { set; get; }

    }

    public class ActivityArea
    {
        public int ACTIVITY_AREA_ID { set; get; }
        public string ACTIVITY_AREA_NAME { set; get; }
    }

    public class EmploeeCount
    {
        public int EMPLOEE_COUNT_ID { set; get; }
        public string EMPLOEE_COUNT_NAME { set; get; }
    }
    #endregion Rus Scoring References

    #region === Virtual ===
    public class VirtualPercent
    {
        public int VIRTUAL_PRODUCT_ID { get; set; }
        public string CCY { get; set; }
        public decimal VIRTUAL_PERCENT { get; set; }
    }

    public class VirtualProduct
    {
        public int VIRTUAL_PRODUCT_ID { get; set; }
        public string VIRTUAL_PRODUCT_NAME { get; set; }
        public int VIRTUAL_PERIOD { get; set; }
    }
    #endregion
    #endregion

    #region === static references ===
    public class StaticReferences
    {
        public List<ApplicationControl> ControlList { get; set; }
        public List<ApplicationPage> PageList { get; set; }
        public List<Branch> BranchList { get; set; }
        public List<WorkPlace> WorkPlaceList { get; set; }
        public List<YearMonth> YearMonthList { get; set; }
        public List<Currency> CurrencyList { get; set; }
        public List<Organization> OrganizationList { get; set; }
        public List<IncomeType> IncomeTypeList { get; set; }
        public List<MaritalStatus> MaritalStatusList { get; set; }
        public List<Country> CountryList { get; set; }
        public List<PassportType> PassportTypeList { get; set; }
        public List<RelationshipType> RelationshipTypeList { get; set; }
        public List<CollateralType> CollateralTypeList { get; set; }
        public List<Bank> BankList { get; set; }
        public List<ApplicationStateMap> ApplicationStateMapList { get; set; }
        public List<Product> ProductList { get; set; }
        public List<LoanAim> LoanAimList { get; set; }
        public List<ProductProperties> ProductPropertiesList { get; set; }
        public List<RealEstateType> RealEstateTypeList { get; set; }
        public List<RealEstateMaxLTV> RealEstateMaxLTVList { get; set; }
        public List<RealEstateInsurance> RealEstateInsuranceList { get; set; }
        public List<CreditHistoryKind> CreditHistoryKindList { get; set; }
        public List<ClientType> ClientTypeList { get; set; }
        public List<ClientRank> ClientRankList { get; set; }
        public List<ScheduleType> ScheduleTypeList { get; set; }
        public List<PmtFreqType> PmtFreqTypeList { get; set; }
        public List<EnshureType> EnshureTypeList { get; set; }
        public List<PrepaymentRescheduleType> PrepaymentRescheduleTypeList { get; set; }
        public List<LoanType> LoanTypeList { get; set; }
        public List<CreditType> CreditTypeList { get; set; }
        public List<TermType> TermTypeList { get; set; }
        public List<RoleType> RoleTypeList { get; set; }
        public List<LoanCollateralType> LoanCollateralTypeList { get; set; }
        public List<PenaltySchema> PenaltySchemaList { get; set; }
        public List<PenaltyCalculationType> PenaltyCalculationTypeList { get; set; }
        public List<RejectReason> RejectReasonList { get; set; }
        public List<ApplicationState> ApplicationStateList { get; set; }
        public List<CommitteePropertiesMap> CommitteePropertiesMapList { get; set; }
        public List<ValRates> ValRatesList { get; set; }
        public List<ClientSalaryCategory> ClientSalaryCategoryList { get; set; }
        public List<City> CityList { get; set; }
        public List<CoborrowerRelationType> CoborrowerRelationTypeList { get; set; }
        public List<Status> StatusList { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public LoanException LoanException { get; set; }

        //public List<ExceptionLoanPartipication> ExceptionLoanPartipicationList { get; set; }
        //public List<ExceptionLoanPrepaiment> ExceptionLoanPrepaimentList { get; set; }
        //public List<ExceptionProdOrg> ExceptionProdOrgList { get; set; }
        //public List<ExceptionProdRanges> ExceptionProdRangesList { get; set; }
        //public List<ExceptionAutoPartipicationRanges> ExceptionAutoPartipicationRangesList { get; set; }

        public List<ProductDetails> ProductDetailsList { get; set; }
        public List<Sex> SexList { get; set; }
        public List<CrediInfoGrade> CrediInfoGradeList { get; set; }
        public List<IncomeVerified> IncomeVerifiedList { get; set; }
        public List<ScoringParamCoef> ScoringParamKoefList { get; set; }
        public List<ProductCategoryMap> ProductCategoryMapList { get; set; }
        public List<ProductCategory> ProductCategoryList { get; set; }
        public List<InformationSource> InformationSourceList { get; set; }
        public List<AutoDealer> AutoDealerList { get; set; }
        public List<InsuranceCompany> InsuranceCompanyList { get; set; }
        public List<InsuranceTariff> InsuranceTariffList { get; set; }
        public List<InsuranceDriverPassenger> InsuranceDriverPassengerList { get; set; }
        public List<InsuranceThirdParty> InsuranceThirdPartyList { get; set; }
        public List<SysParameter> SysParameterList { get; set; }
        public List<SysCalcParameter> SysCalcParameterList { get; set; }
        public List<DisbursType> DisbursTypeList { get; set; }
        public List<LoanProductException> LoanProductExceptionList { get; set; }
        public List<LoanProductExceptionParam> LoanProductExceptionParamList { get; set; }
        public List<SysUserGroup> SysUserGroupList { get; set; }
        public List<Committee> CommitteeList { get; set; }
        public List<CommitteeDelegate> CommitteeDelegateList { get; set; }
        public List<CommitteeUserMap> CommitteeUserMapList { get; set; }
        public List<PictureObjectType> PictureObjectTypeList { get; set; }
        public List<CreditInfoTranslate> CreditInfoTranslateList { get; set; }
        public List<AdditionalAttribute> AdditionalAttributeList { get; set; }
        public List<Installment> InstallmentList { get; set; }
        public List<PurchaseItemGroup> PurchaseItemGroupList { get; set; }
        public List<PurchaseItem> PurchaseItemList { get; set; }
        public List<ProductInstallmentDiscount> ProductInstallmentList { get; set; }
        public List<ProductInstallmentDiscount> ProductInstallmentDiscountList { get; set; }
        public List<InstalmentsUser> InstalmentsUserList { get; set; }
        public List<UserInstalmentsProduct> UserInstalmentsProductList { get; set; }
        public List<ClientSubType> ClientSubTypeList { get; set; }
        public List<BlackListItem> BlackListItemList { get; set; }
        public List<AdminDebtTypes1> AdminDebtTypes1List { get; set; }
        public List<AdminDebtTypes2> AdminDebtTypes2List { get; set; }
        public List<AdminDebtObjects> AdminDebtObjectsList { get; set; }
        public List<SpecialLoanTypes> SpecialLoanTypeList { get; set; }
        public List<CashCoverScheduleType> CashCoverScheduleTypeList { get; set; }
        public List<SebClass> SebClassList { get; set; }

        public List<MFBranchMap> MFBranchMapList { get; set; }
        public List<MFUserMap> MFUserMapList { get; set; }
        public List<MFCaseProductMap> MFCaseProductMapList { get; set; }
        public List<AccountProduct> AccountProductList { get; set; }
        public List<RestructType> RestructTypeList { get; set; }
        public List<NonStandardType> NonStandardTypeList { get; set; }
        public List<RefProductCategory> RefProductCategoryList { get; set; }
        public List<ProductRestructMap> ProductRestructMapList { get; set; }
        public List<InsuranceProducts> InsuranceProductsList { get; set; }
        public List<ClientPosition> ClientPositionList { get; set; }
        public List<ClientConversationalLanguage> ClientConversationalLanguageList { get; set; }
        public List<ClientPositionTtype> ClientPositionTtypeList { get; set; }
        public List<AdminDebtGroup> AdminDebtGroupList { get; set; }
        public List<AdminDebtItem> AdminDebtItemList { get; set; }
        public List<CreditBank> CreditBankList { get; set; }
        public List<Education> EducationList { get; set; }
        public List<PostCategory> PostCategoryList { get; set; }
        public List<ActivityArea> ActivityAreaList { get; set; }
        public List<EmploeeCount> EmploeeCountList { get; set; }
        public List<RealEstateOwnerType> RealEstateOwnerTypeList { get; set; }
        public List<VirtualProduct> VirtualProductList { get; set; }
        public List<VirtualPercent> VirtualPercentList { get; set; }
    }
    #endregion

    #region === LOAN CARD ===
    public class LoanCard
    {
        public LoanCardInfo LoanInfo { get; set; }
        public List<LoanCardSchedule> LoanScheduleList { get; set; }
        public List<LoanCardPayment> LoanPaymentList { get; set; }
        public LoanCardDebt LoanDebt { get; set; }
    }

    public class LoanCardInfo
    {
        public int LOAN_ID { get; set; }
        public decimal LOAN_AMOUNT { get; set; }
        public string CCY { get; set; }
        public string DATE_START { get; set; }
        public string DATE_END { get; set; }
        public int MONTH_COUNT { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public decimal COMMISSION_RATE { get; set; }
        public decimal PENALTY_INTEREST_RATE { get; set; }
        public decimal NOTUSED_INTEREST_RATE { get; set; }
        public decimal PREPAYMENT_INTEREST_RATE { get; set; }
        public string AGREEMENT_NO { get; set; }
        public string CLOSE_DATE { get; set; }
        public int CLIENT_ID { get; set; }
        public string CLIENT_NAME { get; set; }
        public string PERSONAL_ID { get; set; }
        public string ADDRESS_LEGAL { get; set; }
        public string ADDRESS_FACT { get; set; }
        public string PHONE { get; set; }
        public string PHONE_MOBILE { get; set; }
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int BRANCH_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public int RESPONSIBLE_USER_ID { get; set; }
        public string RESPONSIBLE_USER_NAME { get; set; }
        public string OPER_LAST_DATE { get; set; }
        public int OPER_COUNT { get; set; }
        public decimal PAID_PRINCIPAL { get; set; }
        public decimal PAID_INTEREST { get; set; }
        public decimal PAID_PENALTY { get; set; }
        public decimal PAID_FEE { get; set; }
        public decimal PAID_INSURANCE { get; set; }
        public decimal PAID_OTHER { get; set; }
        public bool IS_COLLATERAL { get; set; }
        public int OVERDUE_COUNT { get; set; }
        public decimal RESERVED_AMOUNT { get; set; }
        public decimal RESERVED_RATE { get; set; }
    }

    public class LoanCardSchedule
    {
        public int LOAN_ID { get; set; }
        public DateTime SCHEDULE_DATE { get; set; }
        public decimal PRINCIPAL_AMOUNT { get; set; }
        public decimal INTEREST_AMOUNT { get; set; }
        public decimal PMT_AMOUNT { get; set; }
        public decimal BALANCE { get; set; }
    }

    public class LoanCardPayment
    {
        public int LOAN_ID { get; set; }
        public DateTime OPER_DATE { get; set; }
        public decimal PAID_PRINCIPAL { get; set; }
        public decimal PAID_INTEREST { get; set; }
        public decimal PAID_PENALTY { get; set; }
        public decimal PAID_FEE { get; set; }
        public decimal PAID_INSURANCE { get; set; }
        public decimal PAID_OTHER { get; set; }
    }

    public class LoanCardDebt
    {
        public int LOAN_ID { get; set; }
        public DateTime CALC_DATE { get; set; }
        public decimal CURR_TOTAL_DEBT { get; set; }
        public decimal CURR_PRINCIPAL { get; set; }
        public decimal CURR_OVERDUE_PRINCIPAL { get; set; }
        public decimal CURR_WRITEOFF_PRINCIPAL { get; set; }
        public decimal CURR_INTERESET { get; set; }
        public decimal CURR_PENALTY { get; set; }
        public decimal CURR_INSURANCE { get; set; }
        public decimal CLOSE_TOTAL_DEBT { get; set; }
        public decimal CLOSE_PRINCIPAL { get; set; }
        public decimal CLOSE_OVERDUE_PRINCIPAL { get; set; }
        public decimal CLOSE_WRITEOFF_PRINCIPAL { get; set; }
        public decimal CLOSE_INTERESET { get; set; }
        public decimal CLOSE_PENALTY { get; set; }
        public decimal CLOSE_INSURANCE { get; set; }
        public DateTime DUE_DATE { get; set; }
        public DateTime NEXT_PROCESSING_DATE { get; set; }
        public bool IS_CARD { get; set; }
        public int OVER_DAYS { get; set; }
    }
    #endregion
}