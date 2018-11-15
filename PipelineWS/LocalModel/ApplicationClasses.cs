using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using PipelineWS.LocalModel;


namespace PipelineWS.LocalModel
{
    #region ====[ Applications:LoanApplication ]====

    #region === LoanApplication Full Class ===
    public class LoanApplicationFull
    {
        #region === Private Members ===
        private LoanApplicationRecord _Record = new LoanApplicationRecord();
        private List<ApplicationApprovalHistory> _ApprovalHistoryList;
        private ApplicationAuto _Auto;
        private List<ApplicationClientAssets> _ClientAssetsList;
        private List<ApplicationClientContacts> _ClientContactsList;
        private ApplicationClientInfo _ClientInfo;
        private ApplicationClientInfoFatca _ClientInfoFatca;
        private List<ApplicationClientIsGuarantor> _ClientIsGuarantorList;
        private List<ApplicationCoborrower> _CoborrowerList;
        private List<ApplicationComment> _CommentList;
        private List<ApplicationCurrentLoans> _CurrentLoanList;
        private List<ApplicationCurrentOverdrafts> _CurrentOverdraftList;
        private List<ApplicationDeposits> _DepositList;
        private List<ApplicationGuarantor> _GuarantorList;
        private List<ApplicationGuarantorGuatantedLoans> _GuarantorGuatantedLoanList;
        private List<ApplicationGuarantorLoans> _GuarantorLoanList;
        private List<ApplicationHistory> _HistoryList;
        private ApplicationLock _AppLock;
        private List<ApplicationPicture> _PictureList;
        private ApplicationRealEstate _RealEstate;
        private List<ApplicationRecomender> _RecomenderList;
        private List<ApplicationScoring> _ScoringList;
        private List<ApplicationStopFactorCheckLog> _StopFactorCheckList;
        private ApplicationCreditInfo _CreditInfo;
        private ProductDetails _ProductDetails;
        private ProductProperties _ProductProperties;
        private List<ApplicationAdditionalAttribute> _AdditionalAttribules;
        private ApplicationPurchaseFull _ApplicationPurchaseFull;
        private List<ApplicationOwnerInstalment> _ApplicationOwnerInstalmentList;
        private List<ApplicationAdminDebts> _ApplicationAdminDebtsList;
        private CashCover _CashCover;
        private ApplicationUniversity _ApplicationUniversity;
        private List<ApplicationClientInfoEmployer> _ApplicationClientInfoEmployerList;
        private List<AppAdminDebt> _AppAdminDebtList;

        #endregion

        #region === Public Properties ===
        public LoanApplicationRecord Record
        {
            get { return _Record; }
            set { _Record = value; }
        }

        public List<ApplicationApprovalHistory> ApprovalHistoryList
        {
            get { return _ApprovalHistoryList; }
            set { _ApprovalHistoryList = value; }
        }

        public ApplicationAuto Auto
        {
            get { return _Auto; }
            set { _Auto = value; }
        }

        public List<ApplicationClientAssets> ClientAssetsList
        {
            get { return _ClientAssetsList; }
            set { _ClientAssetsList = value; }
        }

        public List<ApplicationClientContacts> ClientContactsList
        {
            get { return _ClientContactsList; }
            set { _ClientContactsList = value; }
        }

        public ApplicationClientInfo ClientInfo
        {
            get { return _ClientInfo; }
            set { _ClientInfo = value; }
        }

        public ApplicationClientInfoFatca ClientInfoFatca
        {
            get { return _ClientInfoFatca; }
            set { _ClientInfoFatca = value; }
        }

        public List<ApplicationClientIsGuarantor> ClientIsGuarantorList
        {
            get { return _ClientIsGuarantorList; }
            set { _ClientIsGuarantorList = value; }
        }

        public List<ApplicationCoborrower> CoborrowerList
        {
            get { return _CoborrowerList; }
            set { _CoborrowerList = value; }
        }

        public List<ApplicationComment> CommentList
        {
            get { return _CommentList; }
            set { _CommentList = value; }
        }

        public List<ApplicationCurrentLoans> CurrentLoanList
        {
            get { return _CurrentLoanList; }
            set { _CurrentLoanList = value; }
        }

        public List<ApplicationCurrentOverdrafts> CurrentOverdraftList
        {
            get { return _CurrentOverdraftList; }
            set { _CurrentOverdraftList = value; }
        }

        public List<ApplicationDeposits> DepositList
        {
            get { return _DepositList; }
            set { _DepositList = value; }
        }

        public List<ApplicationGuarantor> GuarantorList
        {
            get { return _GuarantorList; }
            set { _GuarantorList = value; }
        }

        public List<ApplicationGuarantorGuatantedLoans> GuarantorGuatantedLoanList
        {
            get { return _GuarantorGuatantedLoanList; }
            set { _GuarantorGuatantedLoanList = value; }
        }

        public List<ApplicationGuarantorLoans> GuarantorLoanList
        {
            get { return _GuarantorLoanList; }
            set { _GuarantorLoanList = value; }
        }

        public List<ApplicationHistory> HistoryList
        {
            get { return _HistoryList; }
            set { _HistoryList = value; }
        }

        public ApplicationLock AppLock
        {
            get { return _AppLock; }
            set { _AppLock = value; }
        }

        public List<ApplicationPicture> PictureList
        {
            get { return _PictureList; }
            set { _PictureList = value; }
        }

        public ApplicationRealEstate RealEstate
        {
            get { return _RealEstate; }
            set { _RealEstate = value; }
        }

        public List<ApplicationRecomender> RecomenderList
        {
            get { return _RecomenderList; }
            set { _RecomenderList = value; }
        }

        public List<ApplicationScoring> ScoringList
        {
            get { return _ScoringList; }
            set { _ScoringList = value; }
        }

        public List<ApplicationStopFactorCheckLog> StopFactorCheckList
        {
            get { return _StopFactorCheckList; }
            set { _StopFactorCheckList = value; }
        }

        public ApplicationCreditInfo CreditInfo
        {
            get { return _CreditInfo; }
            set { _CreditInfo = value; }
        }

        public ProductDetails ProductDetails
        {
            get { return _ProductDetails; }
            set { _ProductDetails = value; }
        }

        public ProductProperties ProductProperties
        {
            get { return _ProductProperties; }
            set { _ProductProperties = value; }
        }
        public List<ApplicationAdditionalAttribute> AdditionalAttribules
        {
            get { return _AdditionalAttribules; }
            set { _AdditionalAttribules = value; }
        }
        public ApplicationPurchaseFull ApplicationPurchaseFull
        {
            get { return _ApplicationPurchaseFull; }
            set { _ApplicationPurchaseFull = value; }
        }

        public List<ApplicationOwnerInstalment> ApplicationOwnerInstalmentList
        {
            get { return _ApplicationOwnerInstalmentList; }
            set { _ApplicationOwnerInstalmentList = value; }
        }
        public List<ApplicationAdminDebts> ApplicationAdminDebtsList
        {
            get { return _ApplicationAdminDebtsList; }
            set { _ApplicationAdminDebtsList = value; }
        }

        public CashCover CashCover
        {
            get { return _CashCover; }
            set { _CashCover = value; }
        }

        public ApplicationUniversity ApplicationUniversity
        {
            get { return _ApplicationUniversity; }
            set { _ApplicationUniversity = value; }
        }

        public List<ApplicationClientInfoEmployer> ApplicationClientInfoEmployerList
        {
            get { return _ApplicationClientInfoEmployerList; }
            set { _ApplicationClientInfoEmployerList = value; }
        }

        public List<AppAdminDebt> AppAdminDebtList
        {
            get { return _AppAdminDebtList; }
            set { _AppAdminDebtList = value; }
        }

        #endregion
    }
    #endregion

    #region ==== LoanApplication ====
    #region LoanApplicationRecord
    public class LoanApplicationRecord
    {
        public int APPLICATION_TYPE_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int APPLICATION_STATE_ID { get; set; }
        public string APPLICATION_STATE_NAME { get; set; }
        public int APLICATION_OWNER { get; set; }
        public string APLICATION_OWNER_NAME { get; set; }
        public string BRANCH_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }

        public int BASE_PRODUCT_ID { get; set; }

        public decimal LOAN_AMOUNT_REQUESTED { get; set; }
        public int LOAN_DAYS_REQUESTED { get; set; }
        public string LOAN_PURPOSE_DESCRIPTION { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public int LOAN_AIM_ID { get; set; }
        public string LOAN_AIM_NAME { get; set; }
        public decimal LOAN_AMOUNT_SCORING { get; set; }
        public decimal LOAN_AMOUNT_APPROVED { get; set; }
        public decimal LOAN_AMOUNT_ISSUED { get; set; }
        public decimal LOAN_AMOUNT_DISBURS { get; set; }
        public decimal CLIENT_TRANSH_AMOUNT { get; set; }
        public int DISBURS_TYPE_ID { get; set; }

        public decimal INTEREST_RATE { get; set; }
        public decimal ADMIN_FEE_PERCENT { get; set; }
        public decimal ADMIN_FEE { get; set; }
        public decimal OVERPAY_PREPAYMENT_RATE { get; set; }
        public decimal PENALTY_ON_PAYMENT_IN_OTHER_BANK { get; set; }

        public decimal NOTUSED_RATE { get; set; }
        public decimal PREPAYMENT_RATE { get; set; }
        public decimal FEE1_PERCENT { get; set; }
        public decimal FEE1_MIN { get; set; }
        public decimal FEE2_PERCENT { get; set; }
        public decimal FEE2_MIN { get; set; }
        public int MIN_PREPAYMENT_COUNT { get; set; }
        public decimal MIN_PREPAYMENT_AMOUNT { get; set; }
        public int PENALTY_CALCULATION_TYPE { get; set; }
        public int OVERPAY_PENALTY_CALCULATION_TYPE { get; set; }
        public int PENALTY_SCHEMA_ID { get; set; }
        public int OB_OVERPAY_PENALTY_CALCULATION_TYPE { get; set; }

        public decimal PARTICIPATION { get; set; }
        public decimal MAX_SALARY_AMOUNT { get; set; }
        public decimal GENERAL_AGGREMENT_AMOUNT { get; set; }
        public decimal GENERAL_AGGREMENT_PERIOD_YAER { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public int CLIENT_NO { get; set; }
        public string CLIENT_NAME { get; set; }
        public bool IS_STANDART { get; set; }
        public bool IS_ARCHIVE { get; set; }
        public bool IS_CASH_COVER { get; set; }
        public int LAST_WORK_PLACE_ID { get; set; }
        public string LAST_WORK_PLACE_NAME { get; set; }
        public int IS_MY_GROUP { get; set; }
        public int IS_MINE { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public string WORK_PLACE_NAME { get; set; }
        public int EDITING_MODE { get; set; }
        public int STATE_MODE { get; set; }
        public string ACT_PAS { get; set; }
        public int LMS_LOAN_ID { get; set; }
        //ApplicationMain
        public int PRODUCT_CATEGORY_ID { get; set; }
        public int PRODUCT_EXTENDED_ID { get; set; }
        public int CLIENT_ORGANIZATION_ID { get; set; }
        public bool SCORING_PASSED { get; set; }
        public string MINIMAL_REQUESTS_STATE { get; set; }
        public string SCORING_STATE { get; set; }
        public string COMMITTEE_COMPLETED_STATE { get; set; }
        //ApplicationDetails

        public int CLIENT_TYPE { get; set; }
        public int SCHEDULE_TYPE { get; set; }
        public int PMT_INTERVAL_TYPE { get; set; }
        public string RESOURCE { get; set; }
        public int ENSURE_TYPE { get; set; }
        public int PREPAYMENT_RESCHEDULE_TYPE { get; set; }
        public decimal OVER_LIMIT_INTEREST_RATE { get; set; }
        public int LOAN_TYPE { get; set; }
        public int CREDIT_TYPE { get; set; }
        public bool IS_CARD { get; set; }
        public bool GENERIC_SCHEDULE { get; set; }
        public int PAYMENT_DAY_1 { get; set; }
        public int PAYMENT_DAY_2 { get; set; }
        public int PAYMENT_MONTH { get; set; }
        public bool END_OF_MONTH { get; set; }
        public bool HAS_PAYMENT_DAY { get; set; }
        public bool HAS_INTEREST_FREE_PERIOD { get; set; }
        public int INTEREST_FREE_PERIOD { get; set; }
        public int TERM_TYPE { get; set; }
        public bool CAN_HAVE_COBORROWER { get; set; }
        public bool HAS_GRACE_DAYS { get; set; }
        public bool HAS_PURPOSE { get; set; }
        public bool HAS_GRACE_HOLIDAYS { get; set; }
        public string COLLATERALS { get; set; }
        public bool IS_INSTALLMENT { get; set; }
        public bool HAS_USAGE_PURPOSE { get; set; }
        public bool HAS_BUSINESSES { get; set; }
        public bool CONTROL_LIMITS { get; set; }

        public int COMMITTEE_USER_COUNT { get; set; }
        public int AUTH_COUNT { get; set; }
        public bool HAVE_REJECT { get; set; }
        public bool DATA_COMPLETED { get; set; }
        public bool NEED_COMMETTEE_ACTION { get; set; }

        public int MINIMAL_REQUESTS { get; set; }
        public int SCORING_COMPLETED { get; set; }
        public int COMMITTEE_APPROVE { get; set; }
        public int CREDIT_MODULE_COMPLETED { get; set; }

        public int CARD_ACC_ID { get; set; }
        public string CARD_ACC_IBAN { get; set; }

        public int REJECT_REASON_ID { get; set; }
        public string REJECT_REASON_NAME { get; set; }
        public int APPLICATION_SOURCE_ID { get; set; }
        public int ACCOUNT_PRODUCT_NO { get; set; }
        public int ACCOUNT_PRODUCT_NO_ODB { get; set; }
        public int NONSTANDARD_TYPE { get; set; }
        public bool IS_RESTRUCT { get; set; }
        public decimal UNPLANNED_DEBT_USED_AMOUNT_RATIO { get; set; }
        public int GRACE_PERIOD { get; set; }
        public int CLIENT_ACC_ID { get; set; }
    }
    #endregion

    #region LoanApplications
    public class LoanApplications
    {
        public int APPLICATION_TYPE_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int APPLICATION_STATE_ID { get; set; }
        public string APPLICATION_STATE_NAME { get; set; }
        public int APLICATION_OWNER { get; set; }
        public string APLICATION_OWNER_NAME { get; set; }
        public string BRANCH_ID { get; set; }
        //public string BRANCH_NAME { get; set; }
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_CATEGORY_NAME { get; set; }
        public decimal LOAN_AMOUNT_REQUESTED { get; set; }
        public int LOAN_DAYS_REQUESTED { get; set; }
        //public string LOAN_PURPOSE_DESCRIPTION { get; set; }
        public string LOAN_CURRENCY { get; set; }
        //public int LOAN_AIM_ID { get; set; }
        //public string LOAN_AIM_NAME { get; set; }
        //public decimal LOAN_AMOUNT_SCORING { get; set; }
        public decimal LOAN_AMOUNT_APPROVED { get; set; }
        public decimal LOAN_AMOUNT_ISSUED { get; set; }
        //PUBLIC decimal INTEREST_RATE { GET; SET; }
        public int CLIENT_NO { get; set; }
        public string CLIENT_NAME { get; set; }
        public string CLIENT_PERSONAL_ID { get; set; }
        public bool IS_STANDART { get; set; }
        public bool IS_RESTRUCT { get; set; }
        //public bool IS_CASH_COVER { get; set; }
        //public int WORK_PLACE_ID { get; set; }
        public string WORK_PLACE_NAME { get; set; }
        public int EDITING_MODE { get; set; }
        public int STATE_MODE { get; set; }
        public string ACT_PAS { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public int REVIEW_DAYS { get; set; }
        //PUBLIC int LMS_LOAN_ID { GET; SET; }
        //PUBLIC int PARENT_APPLICATION_ID { GET; SET; }

        //PUBLIC int MINIMAL_REQUESTS { GET; SET; }
        //public int SCORING_COMPLETED { get; set; }
        public int COMMITTEE_APPROVE { get; set; }
        public int CREDIT_MODULE_COMPLETED { get; set; }

        public int RISK_MANAGER_ID { get; set; }
        public string RISK_MANAGER_NAME { get; set; }
        public bool BACKOFFICE_EMAIL_SENT_STATUS { get; set; }
        public bool BACKOFFICE_EMAIL_CLIENT_SENT_STATUS { get; set; }
        public string INSTALLMENT_NAME { get; set; }
        public string ORGANIZATION_TAX_CODE { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public string CLIENT_CLASIFF { get; set; }
        public int APPLICATION_SOURCE_ID { get; set; }
        public bool SMS_SENT_STATUS { get; set; }
    }
    #endregion

    #region UserApplicationStatictic
    public class UserApplicationStat
    {
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int PRODUCT_GROUP_ID { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public int ALL_APPLICATIONS { get; set; }
        public int ACTIVE_APPLICATIONS { get; set; }
    }
    #endregion

    #region ApplicationApprovalHistory
    public class ApplicationApprovalHistory
    {
        public int MAP_ID { get; set; }
        public int COMMITTEE_ID { get; set; }
        public string COMMITTEE_NANE { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public int COMMITTEE_USER_ID { get; set; }
        public string COMMITTEE_USER_NAME { get; set; }
        public int WORK_PLACE_ID { get; set; }
        public int IS_COMMITTEE_MEMBER { get; set; }
        public string IS_COMMITTEE_MEMBER_NAME { get; set; }
        public int DECISION_ID { get; set; }
        public string APPROVAL_DECISION_NAME { get; set; }
        public string DESCRIP { get; set; }
        public int COMMITTEE_USER_COUNT { get; set; }
        public int AUTH_COUNT { get; set; }
        public bool HAVE_REJECT { get; set; }
        public decimal LIMIT_REMAINDED { get; set; }
    }
    #endregion

    #region ApplicationAuto	
    public class ApplicationAuto
    {
       

        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public DateTime RELEASE_DATE { get; set; }
        public int CAR_AGE { get; set; }
        public string VIN_CODE { get; set; }
        public int DEALER_ID { get; set; }
        public double DISCOUNT_RATE { get; set; }
        public double PRICE_AMOUNT { get; set; }
        public double PRICE_AMOUNT_REAL { get; set; }
        public double PRICE_MARKET_AMOUNT { get; set; }
        public double PARTICIPATION_AMOUNT { get; set; }
        public double INSURANCE_THIRD_PARTY_AMOUNT { get; set; }
        public double INSURANCE_DRIVER_PASSENGER_AMOUNT { get; set; }
        public double INSURANCE_RATE { get; set; }
        public double INSURANCE_AMOUNT { get; set; }
        public double COMMISSION_SHSS_SERVICE_AMOUNT { get; set; }
        public double COMMISSION_AUTO_DEAL_AMOUNT { get; set; }
        public double COMMISSION_ADMIN_AMOUNT { get; set; }
        public double LOAN_AMOUNT_PER_MONTH { get; set; }
        public double INSURANCE_AMOUNT_PER_MONTH { get; set; }
        public double CALC_AMOUNT { get; set; }
        public double CALC_INTEREST { get; set; }
        public int APPLICATION_TYPE_ID { get; set; }
        public int OBJECT_TYPE_ID { get; set; }
        public double CUSTOM_CLEARANCE_AMOUNT { get; set; }
        public double TREASURY_AMOUNT { get; set; }
        public int GRACE_PERIOD { get; set; }
        public int INSURANCE_COMPANY_ID { get; set; }
        public decimal CONVERT_RATE { get; set; }
        public decimal CONVERT_AMOUNT { get; set; }
        public decimal PARALEL_LOAN_AMOUNT { get; set; }
        public int INSURANCE_RATE_ID { get; set; }
        public int DRIVER_PASSENGER_INSURANCE_ID { get; set; }
        public int THIRD_PARTY_INSURANCE_ID { get; set; }
        public bool IS_ARCHIVE { get; set; }
        public bool INCLUDE_DEALER_AMOUNT { get; set; }

    }
    #endregion

    #region ApplicationClientAssets	
    public class ApplicationClientAssets
    {
        public int CLIENT_ASSET_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int CLIENT_ASSET_TYPE_ID { get; set; }
        public string CLIENT_ASSET_TYPE_NAME { get; set; }
        public string CLIENT_ASSET_DESCRIPTION { get; set; }
        public decimal CLIENT_ASSET_COST { get; set; }
        public string CLIENT_ASSET_COST_CCY { get; set; }

    }
    #endregion

    #region ApplicationClientContacts	
    public class ApplicationClientContacts
    {
        public int CLIENT_CONTACT_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string CONTACT_PERSON_NAME { get; set; }
        public string CONTACT_PERSON_SURNAME { get; set; }
        public string CONTACT_PERSON_PHONE { get; set; }
        public string CONTACT_PERSON_MOBILE { get; set; }
        public string CONTACT_PERSON_ADDRESS { get; set; }
        public int CONTACT_PERSON_RELATIONSHIP_TYPE { get; set; }
        public string CONTACT_PERSON_RELATIONSHIP_TYPE_NAME { get; set; }
        public string CONTACT_PERSON_ORGANIZATION_NAME { get; set; }
    }
    #endregion

    #region ApplicationClientInfo	
    public class ApplicationClientInfo
    {
        public int CLIENT_INFO_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int CLIENT_NO { get; set; }
        public string CLIENT_NAME { get; set; }
        public string CLIENT_SURNAME { get; set; }
        public string CLIENT_FATHER_NAME { get; set; }
        public string CLIENT_CITIZENSHIP { get; set; }
        public string CLIENT_PASSPORT_ID { get; set; }
        public string CLIENT_PHONE_HOME { get; set; }
        public string CLIENT_PHONE_MOB { get; set; }
        public string CLEINT_LEGAL_ADDRESS { get; set; }
        public DateTime CLIENT_BIRHT_DATE { get; set; }
        public int CLIENT_AGE { get; set; }
        public string CLIENT_BIRHT_PLACE { get; set; }
        public int PASSPORT_TYPE_ID { get; set; }
        public string CLIENT_PERSONAL_ID { get; set; }
        public int CLIENT_GENDER { get; set; }
        public int CLIENT_CITY_ID { get; set; }
        public string CLIENT_ADDRESS { get; set; }
        public int CLIENT_ORGANIZATION_ID { get; set; }
        public string CLIENT_ORGANIZATION_NAME { get; set; }
        public string CLIENT_ORGANIZATION_CATEGORY { get; set; }
        public string CLIENT_ORGANIZATION_TAX_CODE { get; set; }
        public decimal CLIENT_WORK_EXPERIENCE { get; set; }
        public int CLIENT_WORK_EXPERIENCE_PERIOD { get; set; }
        public decimal CLIENT_MONTHLY_INCOME { get; set; }
        public string CLIENT_MONTHLY_INCOME_CURRENCY { get; set; }
        public decimal CLIENT_OTH_MONTHLY_INCOME { get; set; }
        public string CLIENT_OTH_MONTHLY_INCOME_CURRENCY { get; set; }
        public string CLIENT_POSITION { get; set; }
        public int CLIENT_RANK_ID { get; set; }
        public int CLIENT_TOTAL_WORK_EXPERIENCE { get; set; }
        public int CLIENT_TOTAL_WORK_EXPERIENCE_PERIOD { get; set; }
        public int CLIENT_INCOME_TYPE { get; set; }
        public int CLIENT_OTH_INCOME_TYPE { get; set; }
        public int CLIENT_TRANSFERS_PERIOD_MONTH { get; set; }
        public int CLIENT_SALARY_CATEGORY { get; set; }
        public int CLIENT_MARITAL_STATUS { get; set; }
        public int CLIENT_NUMBER_OF_CHILDREN { get; set; }
        public string CLIENT_SPOUSE_PERSONAL_ID { get; set; }
        public string CLIENT_SPOUSE_PLACE_OF_WORK { get; set; }
        public decimal CLIENT_SPOUSE_INCOME { get; set; }
        public string CLIENT_SPOUSE_INCOME_CURRENCY { get; set; }
        public string CLIENT_SPOUSE_NAME { get; set; }
        public string CLIENT_SPOUSE_SURNAME { get; set; }
        public decimal CLIENT_TOTAL_FAMILY_INCOME { get; set; }
        public string CLIENT_TOTAL_FAMILY_INCOME_CURRENCY { get; set; }
        public int CLIENT_TOTAL_FAMILY_MEMBERS { get; set; }
        public string CLIENT_SPOUSE_POSITION { get; set; }
        public int CLIENT_SPOUSE_INCOME_TYPE { get; set; }
        public bool IS_EMPLOYEE { get; set; }
        public bool IS_ACTIVE_IN_ODB { get; set; }

        public string PASSPORT_COUNTRY { get; set; }
        public DateTime PASSPORT_ISSUE_DT { get; set; }
        public DateTime PASSPORT_END_DATE { get; set; }
        public bool PASSPORT_IS_LIFE { get; set; }
        public string PASSPORT_REG_ORGAN { get; set; }
        public int INFORMATION_SOURCE_ID { get; set; }
        public string E_MAIL { get; set; }
        public string CLIENT_ORGANIZATION_PHONE { get; set; }
        public int CLIENT_SUB_TYPE_ID { get; set; }
        public int CURRENT_NEGATIVE_STATUSES { get; set; }
        public int TERMINATED_NEGATIVE_STATUSES { get; set; }
        public int POSITIVE_STATUSES { get; set; }
        public string RISK_GRADE { get; set; }
        public decimal CALC_INCOME { get; set; }
        public decimal CALC_SALARY { get; set; }
        public decimal MAX_POSITIVE_AMOUNT { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public int NEW_CLIENT { get; set; }
        public string CITIZENSHIP { get; set; }
        public string DOUBLE_CITIZENSHIP { get; set; }
        public string COUNTRY_OF_BIRTH { get; set; }
        public string RESIDENT_COUNTRY { get; set; }
        public string FACT_COUNTRY { get; set; }
        public string FACT_CITY { get; set; }
        public string FACT_CITY_LAT { get; set; }
        public decimal REAL_SALARY { get; set; }
        public int SEB_CLASS_ID { get; set; }
        public decimal RISK_VAL_RATE { get; set; }
        public bool IS_INSIDER { get; set; }
        public string CLIENT_CLASIFF { get; set; }
        public int ACCOUNT_PRODUCT_NO { get; set; }
        public int CURRENT_CALC_NEGATIVE { get; set; }
        public int INSURANCE_COMPANY_ID { get; set; }
        public string PAYMENT_INSURANCE_GUID { get; set; }
        public int CONVERS_LANG_ID { get; set; }
        public List<ClientIncome> ClientIncomeList { get; set; }
        public bool CLIENT_INCOME_APPROVED { get; set; }

        public string SP_HEAD_NAME { get; set; }
        public string SP_HEAD_PHONE { get; set; }
        public string SP_HEAD_POSITION { get; set; }
        public int SP_EMPLOEE_COUNT { get; set; }

        public int EDUCATION_ID { get; set; }
        public int POST_CATEGORY_ID { get; set; }
        public int ACTIVITY_AREA_ID { get; set; }
        public int EMPLOEE_COUNT_ID { get; set; }
    }
    #endregion ApplicationClientInfo

    #region ApplicationClientInfoFatca
    public class ApplicationClientInfoFatca
    {
        public int APPLICATION_ID { get; set; }
        public int US_RESIDENCE { get; set; }
        public int HAS_GREEN_CARD { get; set; }
        public int LONG_TERM_RESIDENCE { get; set; }
        public int CANCELED_US_CITIZENSHIP { get; set; }
        public int US_POST_BOX { get; set; }
        public int US_PHONE_OR_FAX { get; set; }
        public int US_TAX_RESIDENCE { get; set; }
        public string COUNTRY_OF_TAX_RESIDENCE { get; set; }
        public DateTime LAST_UPDATE { get; set; }
    }
    #endregion ApplicationClientInfoFatca

    #region ApplicationClientInfoEmployer

    public class ApplicationClientInfoEmployer
    {
        public int APPLICATION_ID { get; set; }
        public string COMPANY_ID { get; set; }
        public string COMPANY_DESCRIP { get; set; }
        public int CLIENT_WORK_POSITION_TYPE_ID { get; set; }
        public int CLIENT_WORK_PROFESSION_ID { get; set; }
        public string CLIENT_WORK_POSITION_DESCRIP { get; set; }
        public Decimal SALERY { get; set; }
        public string ISO { get; set; }
        public string COMMENT { get; set; }
        public bool UNEMPLOYED { get; set; }
        public string COMPANY_PHONE { get; set; }
        public int CLIENT_INCOME_TYPE_ID { get; set; }
        public bool IS_MAIN { get; set; }
        public Decimal OTHER_SALERY { get; set; }
        public string OTHER_SALERY_ISO { get; set; }
        public int OTHER_SALERY_TYPE_ID { get; set; }
    }

    #endregion ApplicationClientInfoEmployer

    #region ApplicationCoborrower	
    public class ApplicationCoborrower
    {
        public string COBORROWER_GUID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int COBORROWER_CLIENT_NO { get; set; }
        public string COBORROWER_NAME { get; set; }
        public string COBORROWER_SURNAME { get; set; }
        public string COBORROWER_PHONE { get; set; }
        public string COBORROWER_MOBILE { get; set; }
        public string COBORROWER_LEGAL_ADDRESS { get; set; }
        public int COBORROWER_ORGANIZATION_ID { get; set; }
        public string COBORROWER_ORGANIZATION_NAME { get; set; }
        public decimal COBORROWER_WORK_EXPERIENCE { get; set; }
        public int COBORROWER_EXPERIENCE_PERIOD_ID { get; set; }
        public decimal COBORROWER_MONTHLY_INCOME { get; set; }
        public string COBORROWER_MONTHLY_INCOME_CURRENCY { get; set; }
        public DateTime COBORROWER_BIRHT_DATE { get; set; }
        public int COBORROWER_AGE { get; set; }
        public string COBORROWER_PERSONAL_ID { get; set; }
        public int COBORROWER_RELATIONSHIP_TYPE_ID { get; set; }
        public string COBORROWER_RELATIONSHIP_TYPE_NAME { get; set; }
        public string COBORROWER_ADDRESS { get; set; }
        public string COBORROWER_POSITION { get; set; }
        public decimal COBORROWER_TOTAL_WORK_EXPERIENCE { get; set; }
        public int TOTAL_WORK_EXPERIENCE_PERIOD { get; set; }
        public int COBORROWER_INCOME_TYPE_ID { get; set; }
        public bool IS_ACTIVE_IN_ODB { get; set; }
        public bool IS_EMPLOYEE { get; set; }

    }
    #endregion

    #region ApplicationComment	
    public class ApplicationComment
    {
        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int COMMENT_USER_ID { get; set; }
        public string COMMENT_USER_NAME { get; set; }
        public DateTime COMMENT_DATE { get; set; }
        public string COMMENT { get; set; }
        public bool CAN_DELETE { get; set; }
    }
    #endregion

    #region ApplicationCreditInfo
    public class ApplicationCreditInfo
    {
        public int APPLICATION_ID { get; set; }
        public int CREDIT_INFO_CATEGORY_ID { get; set; }
        public int CREDIT_INFO_NEGATIVE_COUNT { get; set; }
        public int CREDIT_INFO_CONFIRMATION_ID { get; set; }
        public int CLIENT_TYPE { get; set; }
        public bool NON_QUALIF_WORKER { get; set; }
    }
    #endregion

    #region Application Incone Avarage
    public class ApplicationIncomeAvarage
    {
        public int APPLICATION_ID { get; set; }
        public string PERSONAL_ID { get; set; }
        public decimal INCOME_AMOUNT { get; set; }
        public int IS_MAIN { get; set; }
    }
    #endregion

    #region == ClientLoans and Overdrafts

    #region ApplicationCurrentLoans
    public class ApplicationCurrentLoans
    {
        public int CURRENT_LOAN_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string CURRENT_LOAN_BANK_CODE { get; set; }
        public bool IS_MY_BANK { get; set; }
        public string CURRENT_LOAN_BANK_NAME { get; set; }
        public decimal CURRENT_LOAN_AMOUNT { get; set; }
        public string CURRENT_LOAN_CURRENCY { get; set; }
        public DateTime CURRENT_LOAN_START_DATE { get; set; }
        public DateTime CURRENT_LOAN_END_DATE { get; set; }
        public int CURRENT_LOAN_MONTHS { get; set; }
        public decimal CURRENT_LOAN_INTEREST_RATE { get; set; }
        public int CURRENT_LOAN_COLLATERAL_TYPE_ID { get; set; }
        public string CURRENT_LOAN_COLLATERAL_TYPE_NAME { get; set; }
        public decimal CURRENT_LOAN_CURRENT_DEBT { get; set; }
        public decimal CURRENT_LOAN_OVERDUE_DEBT { get; set; }
        public decimal CURRENT_LOAN_PMT { get; set; }
        public bool CURRENT_LOAN_COVER { get; set; }
        public int LOAN_ID { get; set; }
        public int CLIENT_NO { get; set; }
        public bool IS_CALCULATION { get; set; }
        public bool IS_NEW_DATA { get; set; }
        public int LOAN_INFO_SOURCE_ID { get; set; }
        public string LOAN_INFO_SOURCE_NAME { get; set; }
        public decimal TOTAL_DEBT { get; set; }
        public bool IS_RESTRUCT { get; set; }
        public bool IS_PROBLEM { get; set; }
        public bool IS_WRITEOFF { get; set; }
        public int VIRTUAL_LOAN_TYPE { get; set; }
        public string VIRTUAL_PRODUCT_NAME { get; set; }
        public int VIRTUAL_PERIOD { get; set; }
        public decimal VIRTUAL_PERCENT { get; set; }
        public decimal VIRTUAL_PMT { get; set; }
    }
    #endregion

    #region ApplicationCurrentOverdrafts	
    public class ApplicationCurrentOverdrafts
    {
        public int CURRENT_OVERDRAFT_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string CURRENT_CREDIT_BANK_CODE { get; set; }
        public bool IS_MY_BANK { get; set; }
        public string CURRENT_CREDIT_BANK_NAME { get; set; }
        public decimal CURRENT_CREDIT_CARD_LIMIT { get; set; }
        public string CURRENT_CREDIT_CURRENCY { get; set; }
        public decimal CURRENT_CREDIT_INTEREST_RATE { get; set; }
        public bool CURRENT_CREDIT_COVER { get; set; }
        public int CLIENT_NO { get; set; }
        public bool IS_CREDIT_CARD { get; set; }
        public bool IS_NEW_DATA { get; set; }
        public int LOAN_INFO_SOURCE_ID { get; set; }
        public string LOAN_INFO_SOURCE_NAME { get; set; }
        public int LOAN_ID { get; set; }
        public decimal CURRENT_CREDIT_OVERDUE_DEBT { get; set; }
        public decimal TOTAL_DEBT { get; set; }
        public bool IS_RESTRUCT { get; set; }
        public bool IS_PROBLEM { get; set; }
        public bool IS_WRITEOFF { get; set; }
        public int VIRTUAL_LOAN_TYPE { get; set; }
        public string VIRTUAL_PRODUCT_NAME { get; set; }
        public int VIRTUAL_PERIOD { get; set; }
        public decimal VIRTUAL_PERCENT { get; set; }
        public decimal VIRTUAL_PMT { get; set; }
    }
    #endregion

    #region ApplicationClientIsGuarantor
    public class ApplicationClientIsGuarantor
    {
        public int CLIENT_IS_GUARANTOR_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int LOAN_ID { get; set; }
        public string GUARANTOR_NAME { get; set; }
        public string GUARANTOR_SURNAME { get; set; }
        public DateTime GUARANTEE_ISSUING_DATE { get; set; }
        public DateTime GUARANTEE_END_DATE { get; set; }
        public string GUARANTEE_ISO { get; set; }
        public decimal GUARANTEE_CURRENT_AMOUNT { get; set; }
        public decimal GUARANTEE_OVERDUE_AMOUNT { get; set; }
        public decimal GUARANTEE_MONTHLY_AMOUNT { get; set; }
        public int GUARANTOR_COUNT { get; set; }
        public bool IS_NEW_DATA { get; set; }
        public int LOAN_INFO_SOURCE_ID { get; set; }
        public string LOAN_INFO_SOURCE_NAME { get; set; }
    }
    #endregion

    #region ClientLoansOverdrafts
    public class ClientLoansOverdrafts
    {
        public List<ApplicationCurrentLoans> ApplicationCurrentLoansList { get; set; }
        public List<ApplicationCurrentOverdrafts> ApplicationCurrentOverdraftsList { get; set; }
        public List<ApplicationClientIsGuarantor> ApplicationClientIsGuarantorList { get; set; }
    }
    #endregion ClientLoansOverdrafts

    #endregion == ClientLoans and Overdrafts

    #region ApplicationDeposits
    public class ApplicationDeposits
    {
        public int DEPOSIT_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int DEPO_ACC_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public string ACCOUNT_IBAN { get; set; }
        public string CCY { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public decimal INTRATE { get; set; }
        public decimal BLOCK_AMOUNT { get; set; }
        public decimal DEPOSIT_REMINDED_AMOUNT { get; set; }
    }
    #endregion

    #region == ApplicationGuarantor ==

    #region ApplicationGuarantor
    public class ApplicationGuarantor
    {
        public string GUARANTOR_GUID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int GUARANTOR_CLIENT_NO { get; set; }
        public string GUARANTOR_NAME { get; set; }
        public string GUARANTOR_SURNAME { get; set; }
        public string GUARANTOR_PHONE { get; set; }
        public string GUARANTOR_MOBILE { get; set; }
        public string GUARANTOR_LEGAL_ADDRESS { get; set; }
        public int GUARANTOR_ORGANIZATION_ID { get; set; }
        public string GUARANTOR_ORGANIZATION_NAME { get; set; }
        public decimal GUARANTOR_WORK_EXPERIENCE { get; set; }
        public int GUARANTOR_EXPERIENCE_PERIOD_ID { get; set; }
        public decimal GUARANTOR_MONTHLY_INCOME { get; set; }
        public string GUARANTOR_MONTHLY_INCOME_CURRENCY { get; set; }
        public DateTime GUARANTOR_BIRHT_DATE { get; set; }
        public int GUARANTOR_AGE { get; set; }
        public string GUARANTOR_BIRHT_PLACE { get; set; }
        public string GUARANTOR_PERSONAL_ID { get; set; }
        public int GUARANTOR_RELATIONSHIP_TYPE_ID { get; set; }
        public string GUARANTOR_RELATIONSHIP_TYPE_NAME { get; set; }
        public string GUARANTOR_ADDRESS { get; set; }
        public string GUARANTOR_POSITION { get; set; }
        public decimal GUARANTOR_TOTAL_WORK_EXPERIENCE { get; set; }
        public int TOTAL_WORK_EXPERIENCE_PERIOD { get; set; }
        public int GUARANTOR_INCOME_TYPE_ID { get; set; }
        public bool IS_ACTIVE_IN_ODB { get; set; }
        public bool IS_EMPLOYEE { get; set; }

        public int GUARANTOR_GENDER { get; set; }
        public string GUARANTOR_PASSPORT_ID { get; set; }
        public int GUARANTOR_CITY_ID { get; set; }
        public int GUARANTOR_MARITAL_STATUS { get; set; }

        public string CLIENT_CITIZENSHIP { get; set; }
        public int PASSPORT_TYPE_ID { get; set; }
        public string PASSPORT_COUNTRY { get; set; }
        public DateTime PASSPORT_ISSUE_DT { get; set; }
        public DateTime PASSPORT_END_DATE { get; set; }
        public bool PASSPORT_IS_LIFE { get; set; }
        public string PASSPORT_REG_ORGAN { get; set; }
        public string E_MAIL { get; set; }
        public int SALARY_CATEGORY { get; set; }
        public int CURRENT_NEGATIVE_STATUSES { get; set; }
        public int TERMINATED_NEGATIVE_STATUSES { get; set; }
        public int POSITIVE_STATUSES { get; set; }
        public string RISK_GRADE { get; set; }
        public decimal CALC_INCOME { get; set; }
        public decimal CALC_SALARY { get; set; }
        public decimal MAX_POSITIVE_AMOUNT { get; set; }
        public decimal REAL_SALARY { get; set; }

        public int FAMILY_MARITAL_STATUS { get; set; }
        public string FAMILY_SPOUSE_PERSONAL_ID { get; set; }
        public string FAMILY_SPOUSE_NAME { get; set; }
        public string FAMILY_SPOUSE_SURNAME { get; set; }
        public string FAMILY_SPOUSE_PLACE_OF_WORK { get; set; }
        public string FAMILY_SPOUSE_POSITION { get; set; }
        public decimal FAMILY_SPOUSE_INCOME { get; set; }
        public string FAMILY_SPOUSE_INCOME_CURRENCY { get; set; }
        public int FAMILY_SPOUSE_INCOME_TYPE { get; set; }
        public int FAMILY_NUMBER_OF_CHILDREN { get; set; }
        public int FAMILY_TOTAL_FAMILY_MEMBERS { get; set; }
        public decimal FAMILY_TOTAL_FAMILY_INCOME { get; set; }
        public string FAMILY_TOTAL_FAMILY_INCOME_CURRENCY { get; set; }

        public int GUARANTOR_TYPE_ID { get; set; }
        public bool IS_INSIDER { get; set; }
        public string CLIENT_CLASIFF { get; set; }

        public int EDUCATION_ID { get; set; }
        public int POST_CATEGORY_ID { get; set; }
        public int ACTIVITY_AREA_ID { get; set; }
        public int EMPLOEE_COUNT_ID { get; set; }
    }
    #endregion

    #region ApplicationGuarantorGuatantedLoans	
    public class ApplicationGuarantorGuatantedLoans
    {
        public int GUARANTOR_GUATANTED_LOANS_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int LOAN_ID { get; set; }
        public string GUARANTOR_GUID { get; set; }
        public string GUARANTOR_NAME { get; set; }
        public string GUARANTOR_GUARANTED_NAME { get; set; }
        public string GUARANTOR_GUARANTED_LOAN_CURRENCY { get; set; }
        public decimal GUARANTOR_GUARANTED_LOAN_CURRENT_DEBT { get; set; }
        public decimal GUARANTOR_GUARANTED_LOAN_OVERDUE_DEBT { get; set; }
        public decimal GUARANTOR_GUARANTED_PMT { get; set; }
        public decimal GUARANTOR_COUNT { get; set; }
        public bool IS_NEW_DATA { get; set; }
        public int LOAN_INFO_SOURCE_ID { get; set; }
        public string LOAN_INFO_SOURCE_NAME { get; set; }

    }
    #endregion

    #region ApplicationGuarantorLoans	
    public class ApplicationGuarantorLoans
    {
        public int GUARANTOR_LOANS_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string GUARANTOR_GUID { get; set; }
        public int LOAN_ID { get; set; }
        public string GUARANTOR_NAME { get; set; }
        public string GUARANTOR_CREDIT_CURRENCY { get; set; }
        public decimal GUARANTOR_CREDIT_CURRENT_DEBT { get; set; }
        public decimal GUARANTOR_CREDIT_OVERDUE_DEBT { get; set; }
        public decimal GUARANTOR_CREDIT_PMT { get; set; }
        public decimal GUARANTOR_CRDIT_CARD_LIMIT { get; set; }
        public decimal GUARANTOR_CREDIT_INTEREST { get; set; }
        public bool IS_NEW_DATA { get; set; }
        public int LOAN_INFO_SOURCE_ID { get; set; }
        public string LOAN_INFO_SOURCE_NAME { get; set; }
        public bool LOAN_COVER { get; set; }
    }


    #endregion

    #endregion

    #region ApplicationHistory	
    public class ApplicationHistory
    {
        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public DateTime OPER_DATE { get; set; }
        public int STATE_ID { get; set; }
        public string CURRENT_STATE_NAME { get; set; }
        public int PREV_STATE_ID { get; set; }
        public string PREVIOUS_STATE_NAME { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public int USER_BRANCH_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public int USER_WORK_PLACE_ID { get; set; }
        public string USER_WORK_PLACE_NAME { get; set; }
        public int WORK_PLACE_ID_TO { get; set; }
        public string WORK_PLACE_TO_NAME { get; set; }
        public int FUTURE_WORK_PLACE_ID { get; set; }
        public string FUTURE_WORK_PLACE_NAME { get; set; }
        public string DESCRIP { get; set; }
    }
    #endregion

    #region ApplicationLock	
    public class ApplicationLock
    {
        public int LOCKED_BY { get; set; }
        public string LOCKED_BY_NAME { get; set; }
        public string LOCKED_BY_BRANCH { get; set; }
        public string LOCKED_BY_WORK_PLACE { get; set; }
    }
    #endregion

    #region ApplicationPicture	
    public class ApplicationPicture
    {
        public int PICTURE_ID { get; set; }
        public int APPLICATION_TYPE_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int OBJECT_ID { get; set; }
        public int OBJECT_TYPE_ID { get; set; }
        public byte[] PICTURE { get; set; }
        public string PICTURE_NAME { get; set; }
    }
    #endregion

    #region ApplicationFile
    public class ApplicationFile
    {
        public string FILE_UID { get; set; }
        public int APPLICATION_TYPE_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string UFILE_NAME { get; set; }
        public string FILE_CONTENT_TYPE { get; set; }
        public byte[] FILE_DATA { get; set; }
    }
    #endregion ApplicationFile

    #region == Application Admin Debts ==
    #region ApplicationAdminDebts
    public class ApplicationAdminDebts
    {
        public int DEBT_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int CLIENT_ID { get; set; }
        public int DEBT_TYPE1_ID { get; set; }
        public int DEBT_TYPE2_ID { get; set; }
        public int OBJECT_TYPE_ID { get; set; }
        public int OBJECT_ID { get; set; }
        public DateTime REG_DATE { get; set; }
        public DateTime EXEC_DATE { get; set; }
        public int STATE { get; set; }
        public string COMMENT { get; set; }
        public string DEBT_TYPE1_NAME { get; set; }
        public string DEBT_TYPE2_NAME { get; set; }
        public string OBJECT_TYPE_NAME { get; set; }
    }
    #endregion ApplicationAdminDebts

    #region AppAdminDebt NEW
    public class AppAdminDebt
    {
        public int REC_ID { set; get; }
        public int APPLICATION_ID { set; get; }
        public int ADMIN_GROUP_ID { set; get; }
        public string ADMIN_GROUP_NAME { set; get; }
        public int ADMIN_ITEM_ID { set; get; }
        public string ADMIN_ITEM_NAME { set; get; }
        public decimal AMOUNT { set; get; }
        public string BANK_BIC_CODE { set; get; }
        public string BANK_NAME { set; get; }
        public string BANK_COMMENT { set; get; }
        public string COMMENT { set; get; }
        public DateTime DEBT_DATE { set; get; }
        public int LMS_DUE_DAYS { set; get; }
        public string LMS_COMMENT { set; get; }
        public string TRANSH_TEXT { set; get; }
        public int USER_ID { set; get; }
    }
    #endregion AppAdminDebt NEW
    #endregion == Application Admin Debts ==

    #region ApplicationRealEstate
    public class ApplicationRealEstate
    {
        public ApplicationRealEstateMain Main { get; set; }
        public List<ApplicationRealEstateCollateral> CollateralList { get; set; }
    }

    public class ApplicationRealEstateMain
    {
        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public decimal PRIMARY_AMOUNT { get; set; }
        public decimal PRIMARY_AMOUNT_REAL { get; set; }
        public decimal COVER_AMOUNT { get; set; }
        public decimal PROJECT_AMOUNT { get; set; }
        public decimal MARKET_AMOUNT { get; set; }
        public decimal LIQUIDATION_RATE { get; set; }
        public decimal LIQUIDATION_AMOUNT { get; set; }
        public decimal PARTIPICATION_RATE { get; set; }
        public decimal PARTIPICATION_AMOUNT { get; set; }
        public decimal PRICE_AMOUNT { get; set; }
        public decimal COMMISSION_RATE { get; set; }
        public decimal COMMISSION_AMOUNT { get; set; }
        public decimal FINAL_AMOUNT { get; set; }
        public bool INSURANCE_LIFE_YN { get; set; }
        public decimal INSURANCE_LIFE_AMOUNT { get; set; }
        public bool INSURANCE_REAL_ESTAE_YN { get; set; }
        public decimal INSURANCE_REAL_ESTAE_AMOUNT { get; set; }
        public decimal LTV_COEFFICIENT { get; set; }
        public decimal CLTV_COEFFICIENT { get; set; }
        public decimal DEVELOPER_DISCOUNT_RATE { get; set; }
        public decimal LOAN_AMOUNT { get; set; }
        public decimal LOAN_INTEREST { get; set; }
        public int LOAN_PERIOD { get; set; }
        public int GENERAL_PERIOD { get; set; }
        public decimal GENERAL_AMOUNT { get; set; }
        public string COMMENT { get; set; }
        public int INSURANCE_COMPANY_ID { get; set; }
        public decimal CALC_INTEREST { get; set; }
        public bool IS_OFFSET { get; set; }
        public decimal OFFSET_INTEREST { get; set; }
        public bool IS_FAST_DISBURST { get; set; }
        public decimal FAST_DISBURST_FEE { get; set; }
        public string PROJECT_ISO { get; set; }

    }

    public class ApplicationRealEstateCollateral
    {
        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string REAL_ESTATE_TYPE_ID { get; set; }
        public string REAL_ESTATE_TYPE_NAME { get; set; }
        public string OWNER_PERSONAL_ID { get; set; }
        public int OWNER_CLIENT_ID { get; set; }
        public string OWNER_NAME { get; set; }
        public string CADASTRE_CODE { get; set; }
        public string REAL_ESTATE_ADDRESS { get; set; }
        public decimal REAL_ESTATE_AREA { get; set; }
        public bool IS_USED { get; set; }
        public decimal USED_AMOUNT { get; set; }
        public bool IS_COVER { get; set; }
        public decimal MARKET_AMOUNT_USD { get; set; }
        public decimal LIQUIDATION_RATE { get; set; }
        public decimal LIQUIDATION_AMOUNT_USD { get; set; }
        public decimal MARKET_AMOUNT { get; set; }
        public decimal LIQUIDATION_AMOUNT { get; set; }
        public string COMMENT { get; set; }
        public string APPRAISER_NAME { get; set; }
        public DateTime VALUATION_DATE { get; set; }
        public DateTime EXTRACT_DATE { get; set; }
        public string REGISTRATION_NUMBER { get; set; }
        public DateTime REGISTRATION_DATE { get; set; }
        public decimal ACTIVE_SPACE { get; set; }
        public string CITY_NAME { get; set; }
        public int SHARE_COUNT { get; set; }
        public int FLOOR_LEVEL { get; set; }
        public string APARTMENT_NUMBER { get; set; }
        public int REAL_ESTATE_OWNER_TYPE_ID { get; set; }
    }
    #endregion

    #region ApplicationRecomender	
    public class ApplicationRecomender
    {
        public int RECOMENDER_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public string RECOMMENDATOR_NAME { get; set; }
        public string RECOMMENDATOR_SURNAME { get; set; }
        public string RECOMMENDATOR_PHONE { get; set; }
        public string RECOMMENDATOR_MOBILE { get; set; }
        public int RECOMMENDATOR_RELATIONSHIP_TYPE_ID { get; set; }
        public string RECOMMENDATOR_RELATIONSHIP_TYPE_NAME { get; set; }

    }
    #endregion

    #region ApplicationScoring	
    public class ApplicationScoring
    {
        public int LOG_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int SCORING_PROPERTY_GROUP_ID { get; set; }
        public string SCORING_PROPERTY_GROUP_NAME { get; set; }
        public int SCORING_PROPERTY_ID { get; set; }
        public string SCORING_PROPERTY_NAME { get; set; }
        public string APPLICATION_VALUE { get; set; }
        public string SCORING_VALUE { get; set; }
        public DateTime SCORING_DATE { get; set; }
        public int SCORING_USER_ID { get; set; }
        public int SCORING_TYPE { get; set; }
        public int SCORING_COEF_TYPE { get; set; }
    }
    #endregion

    #region ApplicationStopFactorCheckLog	
    public class ApplicationStopFactorCheckLog
    {
        public int STOP_FACTOR_CHECK_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int STOP_FACTOR_CHECK_TYPE_ID { get; set; }
        public string STOP_FACTOR_CHECK_TYPE_NAME { get; set; }
        public string POSSIBLE_VALUES { get; set; }
        public string APPLICATION_VALUE { get; set; }
        public bool PASSED { get; set; }
        public string PASSED_NAME { get; set; }
        public DateTime CHECK_DATE { get; set; }
    }
    #endregion

    #region ApplicationBlackList
    public class ApplicationBlackList
    {
        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string DESCRIP { get; set; }
        public string REMARK { get; set; }

    }
    #endregion ApplicationBlackList

    #region ApplicationRelated
    public class ApplicationRelated
    {
        public int CUR_APPLICATION_ID { get; set; }
        public string CUR_PERSONAL_ID { get; set; }
        public string CUR_CLIENT_NAME { get; set; }
        public string CUR_ROLE_NAME { get; set; }
        public int REL_APPLICATION_ID { get; set; }
        public string REL_APPLICATION_STATE_NAME { get; set; }
        public string REL_ROLE_NAME { get; set; }
    }
    #endregion ApplicationRelated

    #region ApplicationInsurance
    public class ApplicationInsurance
    {
        public int APPLICATION_INSURANCE_ID { set; get; }
        public int APPLICATION_ID { set; get; }
        public int INSURANCE_COMPANY_ID { set; get; }
        public int INSURANCE_PRODUCT_ID { set; get; }
        public string INSURANCE_PROPERTY_ID { set; get; }
        public int INSURANCE_TYPE_ID { set; get; }
        public decimal INSURANCE_RATE { set; get; }
        public decimal INSURANCE_AMOUNT { set; get; }
    }
    #endregion ApplicationInsurance

    #region Exception
    #region ApplicationException
    public class LoanProductException
    {
        public int EXCEPTION_ID { get; set; }
        public string EXCEPTION_DESCRIP { get; set; }
        public string STOPED_PROCEDURE_NAME { get; set; }
        public string CHANGES_PROPERTY_FROM { get; set; }
        public string CHANGES_PROPERTY_TO { get; set; }
    }
    #endregion

    #region ApplicationExceptionParam
    public class LoanProductExceptionParam
    {
        public int EXCEPTION_ID { get; set; }
        public int PARAM_ID { get; set; }
        public string PROC_PARAM_NAME { get; set; }
        public string VALUE_PROPERTY_NAME { get; set; }
        public string VALUE_PROPERTY_TYPE { get; set; }
    }
    #endregion

    #region ApplicationExceptionValue
    public class LoanProductExceptionValue
    {
        public decimal EXCEPTION_VALUE_FROM { get; set; }
        public decimal EXCEPTION_VALUE_TO { get; set; }
    }
    #endregion
    #endregion

    #region ApplicationPrintForm
    public class ApplicationPrintForm
    {
        public string CLIENT_NAME { get; set; }
        public string CLIENT_FIRST_NAME { get; set; }
        public string CLIENT_LAST_NAME { get; set; }
        public DateTime CLIENT_BIRHT_DATE { get; set; }
        public int CLIENT_AGE { get; set; }
        public string CLIENT_PASSPORT_ID { get; set; }
        public string CLIENT_PERSONAL_ID { get; set; }
        public string CLIENT_PHONE_HOME { get; set; }
        public string CLIENT_PHONE_MOB { get; set; }
        public string CLIENT_CITIZENSHIP { get; set; }
        public string CLIENT_GENDER_NAME { get; set; }
        public string CLIENT_EMAIL { get; set; }
        public string CLEINT_LEGAL_ADDRESS { get; set; }
        public string CLIENT_ADDRESS { get; set; }
        public string PASSPORT_TYPE_NAME { get; set; }
        public bool IS_RESIDENCE_CARD { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public string CLIENT_POSITION { get; set; }
        public string ORGANIZATION_ADDRESS { get; set; }
        public int CLIENT_WORK_EXPERIENCE { get; set; }
        public string CLIENT_WORK_EXPERIENCE_PERIOD { get; set; }
        public int CLIENT_TOTAL_WORK_EXPERIENCE { get; set; }
        public string CLIENT_TOTAL_WORK_EXPERIENCE_PERIOD { get; set; }
        public decimal CLIENT_MONTHLY_INCOME { get; set; }
        public string CLIENT_MONTHLY_INCOME_CURRENCY { get; set; }
        public string CLIENT_MONTHLY_INCOME_TYPE { get; set; }
        public decimal CLIENT_OTH_MONTHLY_INCOME { get; set; }
        public string CLIENT_OTH_MONTHLY_INCOME_CURRENCY { get; set; }
        public string CLIENT_OTH_MONTHLY_INCOME_TYPE { get; set; }
        public string CLIENT_SALARY_CATEGORY_NAME { get; set; }
        public string ORGANIZATION_TAX_CODE { get; set; }
        public string CLIENT_PHONE_WORK { get; set; }
        public string CLENT_EXTRA_INFO { get; set; }
        public string MARITAL_STATUS_NAME { get; set; }
        public string CLIENT_SPOUSE_NAME { get; set; }
        public int CLIENT_NUMBER_OF_CHILDREN { get; set; }
        public int CLIENT_TOTAL_FAMILY_MEMBERS { get; set; }
        public string CLIENT_SPOUSE_PLACE_OF_WORK { get; set; }
        public string CLIENT_SPOUSE_POSITION { get; set; }
        public decimal CLIENT_SPOUSE_INCOME { get; set; }
        public string CLIENT_SPOUSE_INCOME_CURRENCY { get; set; }
        public string CLIENT_SPOUSE_INCOME_TYPE { get; set; }
        public decimal CLIENT_TOTAL_FAMILY_INCOME { get; set; }
        public string CLIENT_TOTAL_FAMILY_INCOME_CURRENCY { get; set; }
        public string AUTO_TYPE { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string MANUFACTURER_MODEL { get; set; }
        public decimal PRICE_AMOUNT { get; set; }
        public string PRICE_AMOUNT_CURRENCY { get; set; }
        public DateTime RELEASE_DATE { get; set; }
        public int CAR_AGE { get; set; }
        public string VIN_CODE { get; set; }
        public string RELATIONSHIP_TYPE_NAME_1 { get; set; }
        public string CONTACT_PERSON_NAME_1 { get; set; }
        public string CONTACT_PERSON_MOBILE_1 { get; set; }
        public string CONTACT_PERSON_ADDRESS_1 { get; set; }
        public string CONTACT_PERSON_ORGANIZATION_NAME_1 { get; set; }
        public string RELATIONSHIP_TYPE_NAME_2 { get; set; }
        public string CONTACT_PERSON_NAME_2 { get; set; }
        public string CONTACT_PERSON_MOBILE_2 { get; set; }
        public string CONTACT_PERSON_ADDRESS_2 { get; set; }
        public string CONTACT_PERSON_ORGANIZATION_NAME_2 { get; set; }
        public decimal CASH_AMOUNT { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public decimal PARTICIPATION_AMOUNT { get; set; }
        public decimal PARTICIPATION_PERCENT { get; set; }
        public decimal LOAN_DAYS_REQUESTED { get; set; }
        public string LOAN_DAYS_REQUESTED_PERIOD { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public DateTime FIRST_PAIMENT_DATE { get; set; }
        public int PAYMENT_DAY_1 { get; set; }
        public int PAYMENT_DAY_2 { get; set; }
        public decimal COMMISSION_ADMIN_PECENT { get; set; }
        public decimal COMMISSION_ADMIN_AMOUNT { get; set; }
        public decimal LOAN_AMOUNT_REQUESTED { get; set; }
        public string PAYMENT_FREQUENCY { get; set; }
        public decimal INSURANCE_THIRD_PARTY_AMOUNT { get; set; }
        public decimal INSURANCE_DRIVER_PASSENGER_AMOUNT { get; set; }
        public decimal INSURANCE_RATE { get; set; }
        public decimal INSURANCE_AMOUNT { get; set; }
        public decimal INSURANCE_AMOUNT_PER_MONTH { get; set; }
        public decimal COMMISSION_SHSS_SERVICE_AMOUNT { get; set; }
        public decimal PMT { get; set; }
        public decimal OVERPAID_AMOUNT_WITHOUT_INSURANCE { get; set; }
        public decimal OVERPAID_AMOUNT_WITH_INSURANCE { get; set; }
        public decimal COMMISSION_AUTO_DEAL_AMOUNT { get; set; }
        public string INFORMATION_SOURCE_NAME { get; set; }
        public string LOAN_AIM_NAME { get; set; }
        public string LOAN_PURPOSE_DESCRIPTION { get; set; }
        public string BRANCH_NAME { get; set; }
        public DateTime PRINT_DATE { get; set; }
        public string APPLICATION_OWNER_NAME { get; set; }
        public string CARD_ACCOUNT { get; set; }
        public string CARD_ISO { get; set; }
        public string PRODUCT_CATEGORY_NAME { get; set; }
        public string COWORKER_PERSON_NAME { get; set; }
        public string COWORKER_PERSON_MOBILE { get; set; }
        public string COWORKER_PERSON_ADDRESS { get; set; }
        public string COWORKER_PERSON_ORGANIZATION_NAME { get; set; }
        public decimal CUSTOM_CLEARANCE_AMOUNT { get; set; }
        public decimal TREASURY_AMOUNT { get; set; }
        public string GRACE_PERIOD_NAME { get; set; }

        // COBORROWER
        public string COBORROWER_NAME { get; set; }
        public DateTime COBORROWER_BIRHT_DATE { get; set; }
        public string COBORROWER_PASSPORT_ID { get; set; }
        public string COBORROWER_PERSONAL_ID { get; set; }
        public string COBORROWER_ORGANIZATION_NAME { get; set; }
        public string COBORROWER_POSITION { get; set; }
        public string COBORROWER_TOTAL_WORK_EXPERIENCE { get; set; }
        public string COBORROWER_RELATIONSHIP_TYPE_NAME { get; set; }
        public decimal COBORROWER_MONTHLY_INCOME { get; set; }
        public string COBORROWER_MONTHLY_INCOME_CURRENCY { get; set; }
        public string COBORROWER_INCOME_TYPE_NAME { get; set; }
        public string COBORROWER_SALARY_CATEGORY_NAME { get; set; }
        public decimal COBORROWER_OTHER_INCOME { get; set; }
        public string COBORROWER_OTHER_INCOME_CURRENCY { get; set; }

        // REAL_ESTATE
        public decimal REAL_ESTATE_PROJECT_AMOUNT { get; set; }
        public decimal REAL_ESTATE_PARTIPICATION_AMOUNT { get; set; }
        public string REAL_ESTATE_DESCRIP { get; set; }
        public bool IS_OFFSET { get; set; }
        public string PROJECT_ISO { get; set; }
    }

    public class ApplicationPrintStudent
    {
        public string CLIENT_NAME { get; set; }
        public string CLIENT_BIRHT_DATE { get; set; }
        public string CLIENT_AGE { get; set; }
        public string CLIENT_PASSPORT_ID { get; set; }
        public string CLIENT_PERSONAL_ID { get; set; }
        public string CLIENT_PHONES { get; set; }
        public string CLIENT_GENDER_NAME { get; set; }
        public string CLEINT_LEGAL_ADDRESS { get; set; }
        public string CLIENT_ADDRESS { get; set; }
        public string UNIVERSITY_NAME { get; set; }
        public string FACULTY { get; set; }
        public string COURSE { get; set; }
        public string DEBT_AMOUNT { get; set; }
        public string ACADEMIC_DEBT_NAME { get; set; }
        public string DEBT_DATE_START { get; set; }
        public string DEBT_DATE_END { get; set; }
        public string PARENT_NAME_1 { get; set; }
        public string PARENT_BIRHT_DATE_1 { get; set; }
        public string PARENT_PASSPORT_ID_1 { get; set; }
        public string PARENT_PERSONAL_ID_1 { get; set; }
        public string PARENT_PHONE_1 { get; set; }
        public string PARENT_RELATIONSHIP_NAME_1 { get; set; }
        public string PARENT_LEGAL_ADDRESS_1 { get; set; }
        public string PARENT_ADDRESS_1 { get; set; }
        public string PARENT_ORGANIZATION_NAME_1 { get; set; }
        public string PARENT_POSITION_1 { get; set; }
        public string PARENT_MONTHLY_INCOME_1 { get; set; }
        public string PARENT_MONTHLY_INCOME_CURRENCY_1 { get; set; }
        public string PARENT_ORGANIZATION_ADDRESS_1 { get; set; }
        public string PARENT_NAME_2 { get; set; }
        public string PARENT_BIRHT_DATE_2 { get; set; }
        public string PARENT_PASSPORT_ID_2 { get; set; }
        public string PARENT_PERSONAL_ID_2 { get; set; }
        public string PARENT_PHONE_2 { get; set; }
        public string PARENT_RELATIONSHIP_NAME_2 { get; set; }
        public string PARENT_LEGAL_ADDRESS_2 { get; set; }
        public string PARENT_ADDRESS_2 { get; set; }
        public string PARENT_ORGANIZATION_NAME_2 { get; set; }
        public string PARENT_POSITION_2 { get; set; }
        public string PARENT_MONTHLY_INCOME_2 { get; set; }
        public string PARENT_MONTHLY_INCOME_CURRENCY_2 { get; set; }
        public string PARENT_ORGANIZATION_ADDRESS_2 { get; set; }
        public string LOAN_AMOUNT { get; set; }
        public string LOAN_CCY { get; set; }
        public string LOAN_MONTH_PERIOD { get; set; }
        public string LOAN_AIM_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string BRANCH_NAME { get; set; }
        public string APPLICATION_OWNER_NAME { get; set; }
    }

    public class AppProtocolMain
    {
        public string CLIENT_NAME { get; set; }
        public string CLIENT_FIRST_NAME { get; set; }
        public string CLIENT_LAST_NAME { get; set; }
        public string CLIENT_PERSONAL_ID { get; set; }
        public string APP_TYPE_NAME { get; set; }
        public string CLIENT_ORGANIZATION_CATEGORY { get; set; }
        public string CLIENT_SALARY_CATEGORY_NAME { get; set; }
        public string CLIENT_RANK_NAME { get; set; }
        public string ISSUING_TYPE_NAME { get; set; }
        public decimal PROJECT_AMOUNT { get; set; }
        public decimal PARTIPICATION_AMOUNT { get; set; }
        public decimal REQUESTED_AMOUNT { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public int LOAN_DAYS_REQUESTED { get; set; }
        public decimal COMMISSION_ADMIN_AMOUNT { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public decimal LOAN_PMT { get; set; }
        public string DISBURS_FREQUENCY { get; set; }
        public string PAYMENT_FREQUENCY { get; set; }
        public int SCHEDULE_FIRST_DAY { get; set; }
        public string LOAN_STATUS_NAME { get; set; }
        public string RESTRUCT_TYPE_NAME { get; set; }
        public string CLASSIFICATION_NAME { get; set; }
        public decimal GENERAL_AMOUNT { get; set; }
        public string GENERAL_CCY { get; set; }
        public int GENERAL_PERIOD_YEAR { get; set; }
        public DateTime GENERAL_END_DATE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string INSURANCE_TYPE_NAME { get; set; }
        public decimal LOAN_CLTV { get; set; }
        public decimal LOAN_DTI { get; set; }

        //HELPER PROPERTIES
        public int PRODUCT_GROUP_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public int BASE_PRODUCT_ID { get; set; }
    }

    public class AppProtocolRealEstate
    {
        public string COLLATERAL_TYPE_NAME { get; set; }
        public string COLLATERAL_SUBTYPE_NAME { get; set; }
        public string OWNER_NAME { get; set; }
        public string COLLATERAL_ADDRESS { get; set; }
        public string CADASTRE_CODE { get; set; }
    }

    public class AppProtocolAuto
    {
        public string AUTO_TYPE_NAME { get; set; }
        public string AUTO_PRODUCT_NAME { get; set; }
        public string MANUFACTURER_MODEL { get; set; }
        public string RELEASE_DATE { get; set; }
        public string PRICE_AMOUNT { get; set; }
        public string VIN_CODE { get; set; }

        public decimal INSURANCE_THIRD_PARTY_AMOUNT { get; set; }
        public decimal INSURANCE_RATE { get; set; }
        public decimal INSURANCE_DRIVER_PASSENGER_AMOUNT { get; set; }
        public string INSURANCE_COMPANY_NAME { get; set; }
        public decimal COMMISSION_AUTO_DEAL_AMOUNT { get; set; }
        public decimal COMMISSION_SHSS_SERVICE_AMOUNT { get; set; }
    }

    public class AppProtocolUsedDepo
    {
        public string DEPO_AGREEMENT_NO { get; set; }
        public int DEPO_CLIENT_ID { get; set; }
        public string DEPO_CLIENT_NAME { get; set; }
        public string DEPO_CCY { get; set; }
        public decimal DEPO_AMOUNT { get; set; }
        public DateTime DEPO_START_DATE { get; set; }
        public DateTime DEPO_END_DATE { get; set; }
        public decimal DEPO_INTEREST_RATE { get; set; }
    }

    public class AppProtocolCashCover
    {
        public int LOAN_ID { get; set; }
        public string LOAN_CLIENT_NAME { get; set; }
        public decimal LOAN_AMOUNT { get; set; }
        public decimal LOAN_DEBT_AMOUNT { get; set; }
        public string LOAN_CCY { get; set; }
        public DateTime LOAN_END_DATE { get; set; }
        public int DEPO_ID { get; set; }
        public string DEPO_OWNER_NAME { get; set; }
        public decimal DEPO_AMOUNT { get; set; }
        public string DEPO_CCY { get; set; }
    }

    public class AppProtocolGuarantor
    {
        public string GUARANTOR_TYPE_NAME { get; set; }
        public string GUARANTOR_NAME { get; set; }
        public string GUARANTOR_SURNAME { get; set; }
        public string GUARANTOR_PERSONAL_ID { get; set; }
        public string RELATIONSHIP_TYPE_NAME { get; set; }
    }

    public class AppProtocolComment
    {
        public int APPLICATION_ID { get; set; }
        public int COMMENT_USER_ID { get; set; }
        public string COMMENT_USER_NAME { get; set; }
        public DateTime COMMENT_DATE { get; set; }
        public string COMMENT { get; set; }
        public int COMMENT_TYPE { get; set; }
    }

    public class AppProtocol
    {
        public AppProtocolMain Main { get; set; }
        public List<AppProtocolRealEstate> RealEstateList { get; set; }
        public List<AppProtocolAuto> AutoList { get; set; }
        public List<AppProtocolUsedDepo> UsedDepoList { get; set; }
        public List<AppProtocolCashCover> CashCoverList { get; set; }
        public List<AppProtocolGuarantor> GuarantorList { get; set; }
        public List<AppProtocolComment> CommentList { get; set; }
    }
    #endregion

    #region ClientIncomeODB
    public class ClientIncomeODB
    {
        public DateTime DOC_DATE { get; set; }
        public string ACCOUNT_IBAN { get; set; }
        public string OP_CODE { get; set; }
        public string ISO { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal AMOUNT_EQU { get; set; }
        public string DESCRIP { get; set; }
        public bool IS_HIDEN { get; set; }
        public bool IS_SELECTED { get; set; }
    }
    #endregion ClientIncomeODB

    #region ClientRelation
    public class ClientRelation
    {
        public int CLIENT_NO { get; set; }
        public string CLIENT_NAME { get; set; }
        public bool CLIENT_IS_INSIDER { get; set; }
        public int REL_CLIENT_NO { get; set; }
        public string REL_CLIENT_NAME { get; set; }
        public bool RELATED_CLIENT_IS_INSIDER { get; set; }
        public int CLIENT_RELATION_TYPE_ID { get; set; }
        public string CLIENT_RELATION_TYPE { get; set; }
        public string VALUE { get; set; }
        public string VALUE_TYPE { get; set; }
        public string VALUE_NAME { get; set; }
    }

    public class ClientRelationClientInfo
    {
        public bool IS_INSIDER { get; set; }
    }
    public class ClientRelations
    {
        private List<ClientRelation> _ClientRelationList;

        public List<ClientRelation> ClientRelationList
        {
            get { return _ClientRelationList; }
            set { _ClientRelationList = value; }
        }
        private ClientRelationClientInfo _ClientRelationClientInfo;

        public ClientRelationClientInfo ClientRelationClientInfo
        {
            get { return _ClientRelationClientInfo; }
            set { _ClientRelationClientInfo = value; }
        }

    }


    #endregion ClientIncomeODB

    #region ClientApprovalRate
    public class ClientApprovalRate
    {
        public int ID { get; set; }
        public string PERSONAL_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public decimal LOAN_AMOUNT { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public int LOAN_TYPE_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public int STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public DateTime PUB_DATE { get; set; }
        public string COMMENT { get; set; }
        public int APPLICATION_ID { get; set; }
        public int PARENT_APPLICATION_ID { get; set; }
    }
    #endregion ClientApprovalRate

    #region ApplicationAdditionalAttribute
    public class ApplicationAdditionalAttribute
    {
        public int REC_ID { set; get; }
        public int APPLICATION_ID { set; get; }
        public string ATTRIBUTE_VALUE { set; get; }
        public int ATTRIBUTE_ID { set; get; }
        public string ATTRIBUTE_CODE { set; get; }
        public string ATTRIBUTE_NAME { set; get; }
        //public string PRODUCT_ID { set; get; }
        //public int TYPE { set; get; }
        //public int CCY_TYPE { set; get; }
        public int PERIOD_TYPE { set; get; }
        public int INTERVAL_TYPE { set; get; }
        public int INTERVAL_STEP { set; get; }
        //public bool IS_ACTIVE { set; get; }
        //public bool IS_REQUIRED { set; get; }
        //public string DEFAULT_VALUE { set; get; }
        public int ATTRIBUTE_TYPE_ID { set; get; }
        public string ATTRIBUTE_TYPE_NAME { set; get; }
        //public string DATA_TYPE { set; get; }
    }
    #endregion ApplicationAdditionalAttribute

    #region ClientAccounts
    public class ClientAccounts
    {
        public int CLIENT_NO { set; get; }
        public int ACC_ID { set; get; }
        public int DEPT_NO { set; get; }
        public int BRANCH_ID { set; get; }
        public string ACCOUNT_IBAN { set; get; }
        public string ISO { set; get; }
        public string DESCRIP { set; get; }
        DateTime DATE_OPEN { set; get; }
        decimal MIN_AMOUNT { set; get; }
        public int ACC_TYPE { set; get; }
        public string ACC_TYPE_NAME { set; get; }
        public int ACC_SUBTYPE { set; get; }
        public string ACC_SUBTYPE_NAME { set; get; }
    }
    #endregion ClientAccounts

    #region ApplicationClientAccount
    public class ApplicationClientAccount
    {
        public int ACC_ID { set; get; }
        public string ACCOUNT_IBAN { set; get; }
        public string ISO { set; get; }
        public string ACCOUNT_DESCRIP { set; get; }
        public int ACC_TYPE { set; get; }
        public int ACC_SUBTYPE { set; get; }
    }
    #endregion ApplicationClientAccount

    #region Agreement
    public class AgreementOverdraft
    {
        public int APPLICATION_ID { set; get; }
        public string CLIENT_SURNAME { set; get; }
        public string CLIENT_NAME { set; get; }
        public decimal LOAN_AMOUNT { set; get; }
        public string LOAN_CURRENCY { set; get; }
        public decimal INTEREST_RATE { set; get; }
        public DateTime DATE_START { set; get; }
        public DateTime DATE_END { set; get; }
        public int DAYS_COUNT { set; get; }
        public decimal COMMISSION_RATE_BANK { set; get; }
        public decimal COMMISSION_RATE_OTHER { set; get; }
        public decimal OVERDUE_RATE { set; get; }
        public decimal EFFECTIVE_BANK { set; get; }
        public decimal EFFECTIVE_OTHER { set; get; }
        public decimal EFFECTIVE_DEVAL_BANK { set; get; }
        public decimal EFFECTIVE_DEVAL_OTHER { set; get; }
    }
    #endregion Agreement

    #region ApplicationPurchase

    #region ApplicationPurchaseItem
    public class ApplicationPurchaseItem
    {
        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int ITEM_GROUP_ID { get; set; }
        public string ITEM_GROUP_NAME { get; set; }
        public int ITEM_ID { get; set; }
        public string ITEM_DESCRIP { get; set; }
        public string ITEM_NAME { get; set; }
        public decimal ITEM_AMOUNT { get; set; }
        public decimal ITEM_AMOUNT_REAL { get; set; }
    }
    #endregion ApplicationPurchaseItem

    #region ApplicationPurchase
    public class ApplicationPurchase
    {
        public int APPLICATION_ID { get; set; }
        public int INSTALLMENT_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public int PRODUCT_CATEGORY_ID { get; set; }
        public decimal PARTICIPATION_RATE { get; set; }
        public decimal TOTAL_ITEM_AMOUNT { get; set; }
        public decimal TOTAL_ITEM_AMOUNT_REAL { get; set; }
        public decimal PARTICIPATION_AMOUNT { get; set; }
        public decimal PAYD_AMOUNT { get; set; }
        public decimal LOAN_AMOUNT { get; set; }
        public decimal LOAN_INTEREST { get; set; }
        public int LOAN_PERIOD { get; set; }
        public decimal DISCOUNT_AMOUNT { get; set; }
        public decimal COMMISSION_AMOUNT { get; set; }
        public decimal AMOUNT_PER_MONTH { get; set; }
        public decimal OVER_PAY_AMOUNT { get; set; }
    }
    #endregion ApplicationPurchase

    #region ApplicationPurchaseFull
    public class ApplicationPurchaseFull
    {
        public ApplicationPurchase ApplicationPurchase { get; set; }
        public List<ApplicationPurchaseItem> ApplicationPurchaseItemList { get; set; }
    }
    #endregion ApplicationPurchaseFull

    #endregion ApplicationPurchase

    #region ApplicationClientVerify

    #region ApplicationClientVerifyDetail
    public class ApplicationClientVerifyDetail
    {
        public int REC_ID { get; set; }
        public int APPLICATION_ID { get; set; }
        public int RELATIONSHIP_ID { get; set; }
        public string RELATIONSHIP_TYPE_NAME { get; set; }
        public string PERSON_NAME { get; set; }
        public string PERSON_PHONE { get; set; }
        public int PHONE_VERIFY { get; set; }
        public string PHONE_VERIFY_NAME { get; set; }
        public DateTime PHONE_TIME { get; set; }
        public int RECORD_TYPE { get; set; }
    }
    #endregion ApplicationClientVerifyDetail

    #region ApplicationClientVerify
    public class ApplicationClientVerify
    {
        public int APPLICATION_ID { set; get; }
        public int WORK_ORG_ID { set; get; }
        public string ORGANIZATION_NAME { set; get; }
        public string ORGANIZATION_TAX_CODE { set; get; }
        public int WORK_VERIFY { set; get; }
        public string WORK_TIME { set; get; }
        public string POST_NAME { set; get; }
        public int POST_VERIFY { set; get; }
        public string POST_TIME { set; get; }
        public decimal INCOME_AMOUNT { set; get; }
        public decimal INCOME_AMOUNT_FACT { set; get; }
        public int INCOME_VERIFY { set; get; }
        public string INCOME_TIME { set; get; }
        public int EXPERIENCE { set; get; }
        public int EXPERIENCE_FACT { set; get; }
        public int EXPERIENCE_VERIFY { set; get; }
        public string EXPERIENCE_TIME { set; get; }
        public int FIZ_DEBTOR_VERIFY { set; get; }
        public string FIZ_DEBTOR_TIME { set; get; }
        public int FIZ_TAX_PLEDGE_VERIFY { set; get; }
        public string FIZ_TAX_PLEDGE_TIME { set; get; }
        public int FIZ_COMMERCIAL_VERIFY { set; get; }
        public string FIZ_COMMERCIAL_TIME { set; get; }
        public int JURIDICAL_DEBTOR_VERIFY { set; get; }
        public string JURIDICAL_DEBTOR_TIME { set; get; }
        public int JURIDICAL_TAX_PLEDGE_VERIFY { set; get; }
        public string JURIDICAL_TAX_PLEDGE_TIME { set; get; }
        public int JURIDICAL_COMMERCIAL_VERIFY { set; get; }
        public string JURIDICAL_COMMERCIAL_TIME { set; get; }
        public string REMARK { set; get; }
        public bool IS_NOT_AUTOMATIC { set; get; }
    }
    #endregion ApplicationClientVerify

    #region ApplicationClientVerifyFull
    public class ApplicationClientVerifyFull
    {
        public ApplicationClientVerify ApplicationClientVerify { get; set; }
        public List<ApplicationClientVerifyDetail> ApplicationClientVerifyDetailList { get; set; }
    }
    #endregion ApplicationClientVerifyFull

    #endregion ApplicationClientVerify

    #region == POS Answer ==
    public class POSAnswer
    {
        public int APPLICATION_STATE_ID { get; set; }
        public string ERROR_MESSAGE { get; set; }
    }
    #endregion == POS Answer ==

    #region == InstantCardAnswer ==
    public class InstantCardAnswer
    {
        public int ERROR_CODE { get; set; }
        public string ERROR_MESSAGE { get; set; }
        public string ACCOUNT_IBAN { get; set; }
    }
    #endregion == InstantCardAnswer ==

    #region == LIMIT ==
    public class LimitChildren
    {
        public int APPLICATION_ID { get; set; }
        public int PARENT_APPLICATION_ID { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public decimal LOAN_AMOUNT_REQUESTED { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public decimal LOAN_AMOUNT_SCORING { get; set; }
        public decimal LOAN_AMOUNT_ISSUED { get; set; }
        public bool IS_ACTIVE { get; set; }
        public int LMS_LOAN_ID { get; set; }
    }
    #endregion

    #region == APP EXTEND ==
    public class ApplicationExtend
    {
        public int APPLICATION_ID { get; set; }
        public bool IS_CREDIT_CARD { get; set; }
        public string CREDIT_CARD_CCY { get; set; }
        public bool IS_OVERDRAFT { get; set; }
        public string OVERDRAFT_CCY { get; set; }
    }
    #endregion

    #region == LMS Answer ==
    public class LMSAnswer
    {
        public LoanApplicationRecord LoanApplicationRecord { get; set; }
        public int Error { get; set; }
        public string ErrorMessage { get; set; }
    }
    #endregion

    #region ApplicationApproval
    public class ApplicationApprovalStatus
    {
        public int APPLICATION_ID { get; set; }
        public int APPLICATION_STATUS_ID { get; set; }
        public int LMS_LOAN_ID { get; set; }
        public string REJECT_REASON_NAME { set; get; }
    }
    #endregion ApplicationApproval

    #region RealSalary
    public class RealSalary
    {
        public decimal MEDIAN { set; get; }
        public bool IS_VIP_OR_EMPLOYEE { set; get; }
        public bool IS_INCASSO { set; get; }
        public bool IS_SEQUESTRATION { set; get; }
        public bool IS_PROBLEM { set; get; }
    }
    #endregion RealSalary

    #region PreaprovedApplication
    public class PreaprovedApplication
    {
        public string PERSONAL_ID { set; get; }
        public decimal LIMIT { set; get; }
        public DateTime END_DATE { set; get; }
        public int PRODUCT_ID { set; get; }
        public string PRODUCT_NAME { set; get; }
        public string PREAPPROVAL_GENERATION { set; get; }
        public string PREAPPROVAL_SOURCE { set; get; }
        public int GENERATION_ID { set; get; }
    }
    #endregion PreaprovedApplication

    #region FatcaODB
    public class FatcaODB
    {
        public int CLIENT_NO { set; get; }
        public int STATUS { set; get; }
        public string GIIN_CODE { set; get; }
        public int GIIN_STATUS { set; get; }
        public bool NPFFI { set; get; }
        public bool IS_COMPLATED { set; get; }
        public bool RECALCITRANT { set; get; }
        public DateTime LAST_UPDATE { set; get; }
        public int OTHER_TAX_STATUS { set; get; }
        public string CITIZENSHIP { set; get; }
        public string DOUBLE_CITIZENSHIP { set; get; }
        public string COUNTRY_OF_BIRTH { set; get; }
        public int HAS_GREEN_CARD { set; get; }
    }
    #endregion FatcaODB

    #region = special loan types =
    public class SpecialLoanTypes
    {
        public int PRODUCT_ID { set; get; }
        public int BASE_PRODUCT_ID { set; get; }
        public bool IsCard { set; get; }
        public bool IsCreditCard { set; get; }
        public bool IsCreditCardStandard { set; get; }
        public bool IsCreditCardGold { set; get; }
        public bool IsCreditCardGoldPreaproved { set; get; }
        public bool IsCreditCardPreapproved { set; get; }
        public bool IsOverdraft { set; get; }
        public bool IsOverdraftStandard { set; get; }
        public bool IsOverdraftPreapproved { set; get; }
        public bool IsPOS { set; get; }
        public bool IsPOSOnline { set; get; }
        public bool IsGarantee { set; get; }
        public bool IsGaranteeGeocell { set; get; }
        public bool IsGaranteeBeeline { set; get; }
        // ავტო სესხები
        public bool IsAuto { set; get; }
        public bool IsAutoNew { set; get; }
        public bool IsAutoUsed { set; get; }
        public bool IsClearence { set; get; }
        public bool IsClearenceCustomer { set; get; }
        public bool IsClearenceDealer { set; get; }
        public bool IsAutoUsedDealer { set; get; }
        public bool IsToyotaAutoNew { set; get; }

        public bool IsCashCover { set; get; }

        public bool IsRealEstate { set; get; }
        public bool IsIpoPurchase { set; get; }
        public bool IsIpoBuilding { set; get; }
        public bool IsIpoRepairing { set; get; }
        public bool IsIpoRefinancing { set; get; }
        public bool IsIpoUniversal { set; get; }
        public bool IsIpoExpress { set; get; }
        public bool IsIpoGEL { set; get; }
        public bool IsIpoCollaborative { set; get; }
        public bool IsPOSCard { set; get; }
        public bool IsStudent { set; get; }
    }

    #endregion = special loan types =

    #region == CASH COVER ==
    #region CASH COVER CLIENTS
    public class CashCoverClient
    {
        public int APPLICATION_ID { set; get; }
        public int CLIENT_NO { set; get; }
        public string CLIENT_NAME { set; get; }
        public string PERSONAL_ID { set; get; }
        public bool IS_VIP_CLIENT { set; get; }
    }
    #endregion CACH COVER CLIENTS

    #region CASH COVER DEPOSITS
    public class CashCoverDeposit
    {
        public int APPLICATION_ID { set; get; }
        public bool IS_DEPO_USED { set; get; }
        public int DEPO_ID { set; get; }
        public string DEPO_AGREEMENT_NO { set; get; }
        public int DEPO_CLIENT_ID { set; get; }
        public string DEPO_CLIENT_NAME { set; get; }
        public string DEPO_PERSONAL_ID { set; get; }
        public int DEPO_PROD_ID { set; get; }
        public string DEPO_CCY { set; get; }
        public decimal DEPO_AMOUNT { set; get; }
        public decimal DEPO_INTEREST_AMOUNT { set; get; }
        public DateTime DEPO_START_DATE { set; get; }
        public DateTime DEPO_END_DATE { set; get; }
        public decimal DEPO_INTEREST_RATE { set; get; }
        public int DEPO_INTEREST_TYPE { set; get; }
        public string DEPO_INTEREST_TYPE_NAME { set; get; }
        public decimal LINKED_AMOUNT { set; get; }
    }
    #endregion CASH COVER DEPOSITS

    #region CASH COVER DEPO LOAN MAP
    public class CashCoverDepoLoanMap
    {
        public int APPLICATION_ID { set; get; }
        public int LOAN_ID { set; get; }
        public int LOAN_CLIENT_ID { set; get; }
        public string LOAN_CLIENT_NAME { set; get; }
        public decimal LOAN_AMOUNT { set; get; }
        public decimal LOAN_DEBT_AMOUNT { set; get; }
        public string LOAN_CCY { set; get; }
        public DateTime LOAN_END_DATE { set; get; }
        public int DEPO_OWNER_ID { set; get; }
        public string DEPO_OWNER_NAME { set; get; }
        public int DEPO_ID { set; get; }
        public decimal DEPO_AMOUNT { set; get; }
        public string DEPO_CCY { set; get; }
        public decimal LINKED_AMOUNT { set; get; }
    }
    #endregion CASH COVER DEPO LOAN MAP

    #region CASH COVER LOANS
    public class CashCoverLoan
    {
        public int APPLICATION_ID { set; get; }
        public bool IS_LOAN_COVER { set; get; }
        public int LOAN_ID { set; get; }
        public int LOAN_CLIENT_ID { set; get; }
        public string LOAN_CLIENT_NAME { set; get; }
        public decimal LOAN_AMOUNT { set; get; }
        public decimal LOAN_DEBT_AMOUNT { set; get; }
        public decimal LOAN_DEBT_AMOUNT_CUR { set; get; }
        public decimal LOAN_DEBT_AMOUNT_END { set; get; }
        public decimal LOAN_OVER_AMOUNT { set; get; }
        public string LOAN_CCY { set; get; }
        public DateTime LOAN_END_DATE { set; get; }
        public int DEPO_OWNER_ID { set; get; }
    }

    public class CashCoverAttributes
    {
        public int APPLICATION_ID { set; get; }
        public int PRODUCT_ID { get; set; }
        public int SCHEDULE_TYPE_ID { get; set; }
        public string LOAN_CURRENCY { get; set; }

        public bool USE_DEPO_INTEREST { get; set; }
        public bool USE_MAX_LIMIT { get; set; }

        public decimal LOAN_AMOUNT { get; set; }
        public decimal LOAN_INTEREST_RATE { get; set; }
        public DateTime LOAN_START_DATE { get; set; }
        public DateTime LOAN_END_DATE { get; set; }
        public DateTime LOAN_MAX_DATE { get; set; }
    }
    #endregion CASH COVER LOANS

    #region = CASH COVER =
    public class CashCover
    {
        public List<CashCoverClient> CashCoverClientList { set; get; }
        public List<CashCoverDeposit> CashCoverDepositList { set; get; }
        public List<CashCoverDepoLoanMap> CashCoverDepoLoanMapList { set; get; }
        public List<CashCoverLoan> CashCoverLoanList { set; get; }
        public CashCoverAttributes CashCoverAttributes { set; get; }
    }
    #endregion = CASH COVER =
    #endregion == CASH COVER ==

    #region == M-FILES ==
    public class MFileAttributes
    {
        public int APPLICATION_ID { get; set; }
        public int DOC_TYPE_ID { get; set; }
        public string DOC_ENTRY_NO { get; set; }
        public int ATTR_OBJECT_ID { get; set; }
    }
    #endregion == M-FILES ==

    #region == UNIVERSITY ==
    public class ApplicationUniversity
    {
        public int APPLICATION_ID { get; set; }
        public string UNIVERSITY_NAME { get; set; }
        public string FACULTY { get; set; }
        public int COURSE { get; set; }
        public decimal DEBT_AMOUNT { get; set; }
        public DateTime DEBT_DATE_START { get; set; }
        public DateTime DEBT_DATE_END { get; set; }
        public bool IS_ACADEMIC_DEBT { get; set; }
    }
    #endregion
    #endregion

    #region == Loan Restruct ==
    public class LoanRestruct
    {
        public int REC_ID { get; set; }
        public int LOAN_ID { get; set; }
        public string LOAN_STATUS { get; set; }
        public int CLIENT_ID { get; set; }
        public string CLIENT_NAME { get; set; }
        public string PERSONAL_ID { get; set; }
        public DateTime BIRTH_DATE { get; set; }
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public decimal LOAN_AMOUNT { get; set; }
        public string LOAN_CCY { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public decimal COMMISSION_RATE { get; set; }
        public DateTime DATE_START { get; set; }
        public DateTime DATE_END { get; set; }
        public decimal PMT_AMOUNT { get; set; }
        public int PAYD_PERIOD { get; set; }
        public DateTime SCHEDULE_FIRST_DATE { get; set; }
        public decimal DEBT_AMOUNT { get; set; }
        public decimal OVERDUE_AMOUNT { get; set; }
        public int ORG_CATEGORY { get; set; }
        public bool IS_SALARY { get; set; }
        public decimal GENERAL_AMOUNT { get; set; }
        public int GENERAL_END_YEAR { get; set; }
        public bool IS_COMMERCIAL { get; set; }
        public string COMMERCIAL_NAME { get; set; }
        public int RESTRUCT_TYPE_ID { get; set; }
        public string RESTRUCT_TYPE_NAME { get; set; }
        public string RESTRUCT_DESCRIP { get; set; }
        public string COLLATERAL_DESCRIP { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public int CREATE_USER_ID { get; set; }
        public string CREATE_USER_NAME { get; set; }
        public int CREATE_BRANCH_ID { get; set; }
        public string CREATE_BRANCH_NAME { get; set; }
        public int DECISION_ID { get; set; }
        public string DECISION_NAME { get; set; }
        public string RISK_DESCRIP { get; set; }
        public int DECISION_USER_ID { get; set; }
        public DateTime DECISION_DATE { get; set; }
        public bool IS_COMPLETED { get; set; }
        public string COMPLETED_NAME { get; set; }
        public int COMPLETED_USER_ID { get; set; }
        public DateTime COMPLETED_DATE { get; set; }
        public int LOAN_LIST_TYPE { get; set; }
        public bool IS_INSERT { get; set; }
        public bool IS_EDIT { get; set; }
        public bool IS_DELETE { get; set; }
        public bool IS_VIEW { get; set; }
    }
    #endregion

    #region ==== out methods ====
    public class ClientApplicationState
    {
        public DateTime CREATE_DATE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_GROUP_NAME { get; set; }
        public int APPLICATION_STATE_ID { get; set; }
        public string APPLICATION_STATE_NAME { get; set; }
    }
    #endregion ==== out methods ====

    #region === CLIENT INCOME ===
    public class ClientIncome
    {
        public int APPLICATION_ID { get; set; }
        public int INCOME_TYPE_ID { get; set; }
        public string INCOME_TYPE_NAME { get; set; }
        public string COMMON_ORG_NAME { get; set; }
        public string COMMON_ORG_ACTIVITY_NAME { get; set; }
        public string COMMON_ORG_ADDRESS { get; set; }
        public string COMMON_ORG_PHONE { get; set; }
        public string COMMON_ORG_TAX_CODE { get; set; }
        public int SALARY_POSITION_ID { get; set; }
        public string SALARY_POSITION_DESCRIP { get; set; }
        public int SALARY_WORK_EXPERIENCE { get; set; }
        public string SALARY_HEAD_NAME { get; set; }
        public string SALARY_HEAD_POSITION { get; set; }
        public string SALARY_HEAD_PHONE { get; set; }
        public string RENT_REAL_ESTATE_TYPE_ID { get; set; }
        public string RENT_REAL_ESTATE_ADDRESS { get; set; }
        public string RENT_REAL_ESTATE_OWNER { get; set; }
        public int RENT_RELATION_ID { get; set; }
        public string RENT_LESSOR_NAME { get; set; }
        public string RENT_LESSOR_PHONE { get; set; }
        public decimal DIVIDEND_ORG_PROFIT { get; set; }
        public decimal DIVIDEND_CUSTOMER_SHARE { get; set; }
        public int BUSINESS_ACTIVITY_DURATION { get; set; }
        public decimal BUSINESS_CLIENT_PERCENTAGE { get; set; }
        public int BUSINESS_EMPLOYEE_NUMBER { get; set; }
        public decimal BUSINESS_INCOME_PER_DAY { get; set; }
        public decimal BUSINESS_INCOME_PER_MONTH { get; set; }
        public string PROF_PROFESSION_NAME { get; set; }
        public string PROF_CONTACT_PERSON_NAME1 { get; set; }
        public string PROF_CONTACT_PERSON_PHONE1 { get; set; }
        public string PROF_CONTACT_PERSON_NAME2 { get; set; }
        public string PROF_CONTACT_PERSON_PHONE2 { get; set; }
        public decimal COMMON_INCOME_AMOUNT { get; set; }
    }
    #endregion
    #endregion
}