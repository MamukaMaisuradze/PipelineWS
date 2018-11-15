using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineWS.LocalModel
{
    public enum enumScoringCalcType : Int32
    {
        ClientScoring = 1,
        CoborrowerScoring = 2
    }

    public enum enumClientType : Int32
    {
        Borrower = 0,
        Guarantor = 1,
        Coborrower = 2,
        Parent = 3
    }

    public enum enumCoefOperType : Int32
    {
        EQ = 0,
        GE = 1,
        GT = 2,
        LE = 3,
        LT = 4
    }

    public class FA_ScoringParam
    {
        public enumScoringCalcType ScorCalcType { get; set; }
    }

    public class FA_Application
    {
        public int PRODUCT_ID { get; set; }
        public decimal LOAN_AMOUNT { get; set; }
        public decimal LOAN_INTEREST_RATE { get; set; }
        public decimal COMMISSION_RATE { get; set; }
        public int LOAN_NPER { get; set; }
        public string LOAN_CURRENCY { get; set; }
        public decimal REAL_ESTATE_AMOUNT { get; set; }
        public decimal PARTICIPATION_RATE { get; set; }
        public bool IS_ADDITIONAL_CAPITAL { get; set; }
        public DateTime CREATE_DATE { get; set; }

        // --------------------------
        public int PRODUCT_CATEGORY_ID { get; set; }
        public decimal RISK_VAL_RATE { get; set; }
        public decimal EXISTS_PRODUCT_DEBT { get; set; }
        public decimal AMOUNT_DISCOUNT { get; set; }
        public decimal COVER_AMOUNT { get; set; }
    }

    public class FA_Client
    {
        public int CLIENT_ID { get; set; }
        public int SEX_ID { get; set; }
        public int AGE { get; set; }
        public int MARITAL_STATUS_ID { get; set; }
        public int WORK_EXPERIENCE_TOTAL { get; set; }
        public int WORK_EXPERIENCE_CURRENT { get; set; }
        public int SALARY_STATUS_ID { get; set; } //TODO
        public decimal INCOME_AMOUNT_GEL { get; set; }
        public decimal INCOME_PER_PERSON { get; set; }
        public string ORGANIZATION_CATEGORY { get; set; }
        public int NEGATIVE_STATUSES_COUNT { get; set; }
        public decimal LIABILITY_AMOUNT_GEL { get; set; }
        public decimal GUARANTEE_AMOUNT { get; set; }
    }

    public class FA_CreditInfo
    {
        public enumClientType ClientType { get; set; }
        public int CREDIT_INFO_CATEGORY_ID { get; set; }
        public int INCOME_VERIFIED { get; set; }
        public bool IS_NON_QUALIFICATION_WORKER { get; set; }
    }

    public class FA_Guarantor
    {
        public int GuarantorID { get; set; } // TODO: 
        public string PERSONAL_ID { get; set; }
        public enumClientType GuarantorType { get; set; }
        public decimal INCOME_AMOUNT_GEL { get; set; }
        public decimal LIABILITY_AMOUNT_GEL { get; set; }
        public int SEX_ID { get; set; }
        public int MARITAL_STATUS_ID { get; set; }
        public int WORK_EXPERIENCE_TOTAL { get; set; }
        public int WORK_EXPERIENCE_CURRENT { get; set; }
        public int AGE { get; set; }
        public string ORGANIZATION_CATEGORY { get; set; }
        public int SALARY_STATUS_ID { get; set; } //TODO
        public decimal INCOME_PER_PERSON { get; set; }
        public decimal GUARANTEE_AMOUNT { get; set; }
        public decimal GUARANTOR_PMT { get; set; }
    }

    public class FA_HelperParam
    {
        public int SCORING_PROPERTY_ID { get; set; }
        public string APP_VALUE_NAME { get; set; }

        public int BASE_PRODUCT_ID { get; set; }
        public int NEGATIVE_COUNT { get; set; }
        public int SCORING_TYPE_ID { get; set; }
        
        public decimal PMT { get; set; }
        public decimal INCOME_AMOUNT_GEL { get; set; }
        public decimal LIABILITY_AMOUNT_GEL { get; set; }
        public string LOAN_NPER { get; set; }
        public decimal PRODUCT_LIMIT { get; set; }
        public decimal COVER_AMOUNT { get; set; }
        public decimal RISK_VAL_RATE { get; set; }
        public decimal LIMIT_MAX_AMOUNT { get; set; }
        public string QUALIFICATION_WORKER { get; set; }
        public bool IS_TARGETED_HYPOTEC { get; set; }
        public bool IS_POS_CARD { get; set; }
        public decimal APP_XIRR { get; set; }

        // –––––––––––––––––––––––––––––––––––––
        public decimal COBORROWER_INCOME_AMOUNT { get; set; }
        public int COBORROWER_COUNT { get; set; }
        public decimal LTV { get; set; }
        public int SEX_ID { get; set; }
        public int MARITAL_STATUS_ID { get; set; }
        public int WORK_EXPERIENCE_TOTAL { get; set; }
        public int WORK_EXPERIENCE_CURRENT { get; set; }
        public int AGE { get; set; }
        public string ORGANIZATION_CATEGORY { get; set; }
        public int SALARY_STATUS_ID { get; set; }
        public decimal INCOME_PER_PERSON { get; set; }
        public decimal GUARANTEE_AMOUNT { get; set; }
    }


    public class FA_HelperCoef
    {
        public double COEF_CREDIT_INFO { get; set; }
        public double COEF_SEX { get; set; }
        public double COEF_MARITAL { get; set; }
        public double COEF_WORK_EXPERIENCE { get; set; }
        public double COEF_AGE { get; set; }
        public double COEF_LTV { get; set; }
        public double COEF_INCOME { get; set; }
        public double COEF_SALARY_STATUS { get; set; }
        public double COEF_CATEGORY { get; set; }
        public double COEF_MATURITY { get; set; }
        public double COEF_DOWN_PAYMENT { get; set; }
        public double COEF_INCOME_VERIFIED { get; set; }
        public double COEF_INCOME_PER_PERSON { get; set; }
        public double COEF_ADDITIONAL_CAPITAL { get; set; }
        public double COEF_EMPLOYMENT_TIME_AVG { get; set; }
        public double COEF_EMPLOYMENT_TIME_TOTAL { get; set; }
        public double COEF_EMPLOYMENT_TIME_CURRENT { get; set; }
        public double COEF_NON_QUALIFICATION_WORKER { get; set; }
    }

    public class FA_ScoringResult
    {
        public decimal ScoringValue { get; set; }
        public decimal XIRRValue { get; set; }
        public List<FA_ScoringLog> ScoringLogList { get; set; }
    }

    public class FA_ScoringLog
    {
        public int SCORING_PROPERTY_ID { get; set; }
        public string APPLICATION_VALUE { get; set; }
        public string SCORING_VALUE { get; set; }
        public int SCORING_CALC_TYPE { get; set; }
    }

    public class FA_XIIRParam
    {
        public decimal AMOUNT_LOAN { get; set; }
        public int LOAN_MONTHS { get; set; }
        public decimal INTEREST_RATE { get; set; }
        public DateTime DATE_START { get; set; }
        public decimal COMMISSION_RATE { get; set; }
        public decimal AMOUNT_DISCOUNT { get; set; }
        //public decimal DISCOUNT_RATE { get; set; }
        //public decimal PRICE_AMOUNT { get; set; }
    }

    public class FA_ProductLimit
    {
        public decimal MIN_PROD_LIMIT { get; set; }
        public decimal MAX_PROD_LIMIT { get; set; }
        public decimal MAX_SALARY_COUNT { get; set; }
    }

    public class FA_ScheduleXIRR
    {
        public DateTime PAYMENT_DATE { get; set; }
        public double PAYMENT { get; set; }
    }

    public class FA_Dictionary
    {
        private List<KeyValuePair<int, string>> _ConfirmList = new List<KeyValuePair<int, string>>();
        private List<KeyValuePair<bool, string>> _QualificationWorkerList = new List<KeyValuePair<bool, string>>();

        public FA_Dictionary()
        {
            _ConfirmList.Add(new KeyValuePair<int, string>(0, "არა"));
            _ConfirmList.Add(new KeyValuePair<int, string>(1, "დიახ"));
            _ConfirmList.Add(new KeyValuePair<int, string>(2, "არაფორმალურად"));

            _QualificationWorkerList.Add(new KeyValuePair<bool, string>(false, "არაკვალიფიცირებული"));
            _QualificationWorkerList.Add(new KeyValuePair<bool, string>(true, "კვალიფიცირებული"));
        }

        public List<KeyValuePair<int, string>> ConfirmList
        {
            get
            {
                return _ConfirmList;
            }

            set
            {
                _ConfirmList = value;
            }
        }

        public List<KeyValuePair<bool, string>> QualificationWorkerList
        {
            get
            {
                return _QualificationWorkerList;
            }

            set
            {
                _QualificationWorkerList = value;
            }
        }
    }
}
