using Model;
using Model.Database;
using PipelineWS.LocalModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Reflection;
using PipelineWS.OdbService;
using PipelineWS.CraService;
using System.Linq;
using Model.Exceptions;
using PipelineWS.LAPI.Core;
using System.Text;
using System.ServiceModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Web.Script.Serialization;
using PipelineWS.RSService;

namespace PipelineWS
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class PipelineService : System.Web.Services.WebService
    {
        #region Private Members

        private LogFile _Log;
        private DataGate _DGate;
        private AppSettings _AppSets;
        private DataTable mDataTable;
        private List<ValRates> _ValRatesList;

        #region =Alta LMS=
        LoanApplicationFull _LoanApplicationFull;
        PutApplicationRequest _PutApplicationRequest;
        PipelineService _PipelineService;
        PmtFreqType _PmtFreqType;
        InsuranceTariff _InsuranceTariff;
        InsuranceTariff[] _InsuranceTariffArray;
        PmtFreqType[] _PmtFreqTypeArray;
        InformationSource[] _InformationSourceArray;
        InformationSource _InformationSource;
        InsuranceDriverPassenger[] _InsuranceDriverPassengerArray;
        InsuranceDriverPassenger _InsuranceDriverPassenger;
        InsuranceThirdParty[] _InsuranceThirdPartyArray;
        InsuranceThirdParty _InsuranceThirdParty;
        EndOfMonthMode _EndOfMonthType;
        InsuranceProducts[] _InsuranceProductsArray;
        InsuranceProducts _InsuranceProducts;
        List<AdditionalAttribute> _AdditionalAttributeList;

        LoansServiceClient _lmsClient = new LoansServiceClient();
        InsurancesServiceClient _lmsClientInsurance = new InsurancesServiceClient();

        LoansServiceClient _LAPI = new LoansServiceClient();
        int _ApplicationId = 0;
        string _AgreementNo = "";
        string _LMSConnectionString = "";

        #endregion

        #endregion

        #region == class constructor ==
        public PipelineService()
        {
            _AppSets = new AppSettings();
            _DGate = new DataGate(this.GetType().Assembly);
            _DGate.ConnectionString = _AppSets["ConnString"];
            _LMSConnectionString = _AppSets["ConnStringLMS"];
            _Log = new LogFile(_AppSets["SMTPServer"], _AppSets["DeveloperEMail"], null, this.GetType().Assembly);
            _Log.AppName = "PipelineWS";
            _LAPI.Endpoint.Address = new EndpointAddress(new Uri(_AppSets["LAPI.URL"]));
            _lmsClient.Endpoint.Address = new EndpointAddress(new Uri(_AppSets["LAPI.URL"]));
            _lmsClientInsurance.Endpoint.Address = new EndpointAddress(new Uri(_AppSets["LAPI_INSURANCE.URL"]));
        }
        #endregion

        #region == Private Methods ==

        #region LogFault
        private Dictionary<string, string> LogFaunt(System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault, int mUserID, string MethodName)
        {
            Dictionary<string, string> mFaultDetailMetadata = null;
            if (fault.Detail != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(string.Format("Request Id: {0}{1}", fault.Detail.RequestId, Environment.NewLine));
                sb.AppendFormat(string.Format("Fault code: {0}{1}", fault.Detail.FaultCode, Environment.NewLine));
                sb.AppendFormat(string.Format("Fault data: {0}{1}{0}", Environment.NewLine, fault.Detail.ExtraData));

                if (fault.Detail.Metadata != null && fault.Detail.Metadata.Count > 0)
                {
                    mFaultDetailMetadata = new Dictionary<string, string>();
                    foreach (var item in fault.Detail.Metadata)
                    {
                        mFaultDetailMetadata.Add(item.Key, item.Value);
                        sb.AppendFormat(string.Format("FaultDetailMetadata - {0}: {1}{2}", item.Key, item.Value, Environment.NewLine));
                    }
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, MethodName + " LMS: " + sb.ToString());
                }
                else
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, MethodName + " LMS: " + fault.Message + sb.ToString());
                }

            }
            else
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, MethodName + " LMS2: " + fault.Message);
            }
            return mFaultDetailMetadata;
        }
        #endregion LogFault

        #region FillClass
        public object FillClass(object mClass, DataRow mDataRow)
        {
            PropertyInfo[] props = mClass.GetType().GetProperties();
            string mType;
            foreach (PropertyInfo prp in props)
            {
                mType = prp.PropertyType.FullName;
                switch (mType)
                {
                    case "System.String":
                        {
                            if (!DBNull.Value.Equals(mDataRow[prp.Name]))
                            {
                                prp.SetValue(mClass, Convert.ToString(mDataRow[prp.Name]), null);
                            }
                            else
                            {
                                prp.SetValue(mClass, "", null);
                            }
                            break;
                        }
                    case "System.Int16":
                    case "System.Int32":
                        {
                            if (!DBNull.Value.Equals(mDataRow[prp.Name]))
                            {
                                prp.SetValue(mClass, Convert.ToInt32(mDataRow[prp.Name]), null);
                            }
                            else
                            {
                                prp.SetValue(mClass, 0, null);
                            }
                            break;
                        }
                    case "System.Boolean":
                        {
                            if (!DBNull.Value.Equals(mDataRow[prp.Name]))
                            {
                                prp.SetValue(mClass, Convert.ToBoolean(mDataRow[prp.Name]), null);
                            }
                            else
                            {
                                prp.SetValue(mClass, false, null);
                            }
                            break;
                        }
                    case "System.Decimal":
                        {
                            if (!DBNull.Value.Equals(mDataRow[prp.Name]))
                            {
                                prp.SetValue(mClass, Convert.ToDecimal(mDataRow[prp.Name]), null);
                            }
                            else
                            {
                                prp.SetValue(mClass, 0.00m, null);
                            }
                            break;
                        }
                    case "System.Double":
                        {
                            if (!DBNull.Value.Equals(mDataRow[prp.Name]))
                            {
                                prp.SetValue(mClass, Convert.ToDouble(mDataRow[prp.Name]), null);
                            }
                            else
                            {
                                prp.SetValue(mClass, 0.00, null);
                            }
                            break;
                        }

                    case "System.DateTime":
                        {
                            if (!DBNull.Value.Equals(mDataRow[prp.Name]))
                            {
                                prp.SetValue(mClass, Convert.ToDateTime(mDataRow[prp.Name]), null);
                            }
                            else
                            {
                                prp.SetValue(mClass, Convert.ToDateTime("2000-01-01"), null);
                            }
                            break;
                        }
                    case "System.Byte[]":
                        {
                            if (!DBNull.Value.Equals(mDataRow[prp.Name]))
                            {
                                prp.SetValue(mClass, (Byte[])(mDataRow[prp.Name]), null);
                            }
                            else
                            {
                                prp.SetValue(mClass, null, null);
                            }
                            break;
                        }

                }

            }
            return mClass;
        }

        private decimal GetAdminFee(int mProductID, decimal mFee, string mFeeName)
        {
            decimal mAdminFee = 0;
            if (mFeeName == "AdministrationFee")
            {
                if (mProductID == 86 || mProductID == 87 || mProductID == 105)
                {
                    mAdminFee = 0;
                }
                else if (mProductID == 91 || mProductID == 92)
                {
                    mAdminFee = mFee;
                }
            }
            else
            {
                if (mProductID == 86 || mProductID == 87 || mProductID == 105)
                {
                    mAdminFee = mFee;
                }
                else if (mProductID == 105)
                {
                    mAdminFee = 0;
                }

                else if (mProductID == 91 || mProductID == 92)
                {
                    mAdminFee = 0;
                }
            }
            return mAdminFee;
        }

        #endregion

        #region CheckNull
        private string CheckNull(object param)
        {
            if (param == null)
                return "NULL";

            if (param is string)
                return StringFunctions.SqlQuoted((string)param);

            if (param is bool)
                return (((bool)param) ? 1 : 0).ToString();

            if (param is bool?)
                return (((bool)param) ? 1 : 0).ToString();

            if (param is DateTime)
                return StringFunctions.SqlDateQuoted((DateTime)param);

            if (param is DateTime?)
                return StringFunctions.SqlDateQuoted((DateTime)param);

            if (param is Enum)
                return ((int)param).ToString();

            return param.ToString();
        }

        private string CheckNull(object param, object defaultValue)
        {
            if (param != null && defaultValue != null && param.GetType() != defaultValue.GetType())
                throw new Exception("Parameters \"param\" and \"defaultValue\" must have the same types");

            return CheckNull((param == null) ? defaultValue : param);
        }
        #endregion

        #region CalcPMT
        public double CalcPMT(double mPV, double mNper, double mRatePerYear)
        {
            double a, b, x;
            double monthlyPayment;
            a = (1 + mRatePerYear / 1200);
            b = mNper;
            x = Math.Pow(a, b);
            x = 1 / x;
            x = 1 - x;
            monthlyPayment = (mPV) * (mRatePerYear / 1200) / x;
            return (monthlyPayment);
        }
        #endregion

        #region ConvertMoney
        public double ConvertMoney(double mAmmount, string mIsoFrom, string mIsoTo)
        {
            if (this._ValRatesList == null)
            {
                this._ValRatesList = this.ValRatesList();
            }

            if (mIsoTo == null || mIsoFrom == null || mIsoTo == "" || mIsoFrom == "")
            {
                mIsoTo = "GEL";
                mIsoFrom = "GEL";
            }
            double mRetValue = 0.00;
            int mIsoFromItems = 0, mIsoToItems = 0;
            double mIsoFromAmount = 0.00, mIsoToAmount = 0.00;
            try
            {
                ValRates mValRatesFrom = this._ValRatesList.First(item => item.ISO == mIsoFrom);
                mIsoFromItems = Convert.ToInt32(mValRatesFrom.ITEMS);
                mIsoFromAmount = Convert.ToDouble(mValRatesFrom.AMOUNT);
            }
            catch
            {
                mIsoFromItems = 1;
                mIsoFromAmount = 1;
            }
            try
            {
                ValRates mValRatesTo = this._ValRatesList.First(item => item.ISO == mIsoTo);
                mIsoToItems = Convert.ToInt32(mValRatesTo.ITEMS);
                mIsoToAmount = Convert.ToDouble(mValRatesTo.AMOUNT);
            }
            catch
            {
                mIsoToItems = 1;
                mIsoToAmount = 1;
            }

            mRetValue = mAmmount / mIsoFromItems * mIsoFromAmount / mIsoToAmount * mIsoToItems;

            return Math.Round(mRetValue, 2);
        }
        #endregion ConvertMoney

        #region LMSRequestHeaders
        private LAPI.Core.RequestHeaders LMSRequestHeadersGet()
        {
            var mRequestHeaders = new LAPI.Core.RequestHeaders();

            mRequestHeaders.ApplicationKey = "VTB Pipeline Application v1";
            mRequestHeaders.RequestId = Guid.NewGuid().ToString();
            mRequestHeaders.Timestamp = DateTime.Now;
            return mRequestHeaders;
        }
        private LAPI.Core.RequestHeaders LMSRequestHeadersGetI()
        {
            var mRequestHeaders = new LAPI.Core.RequestHeaders();

            mRequestHeaders.ApplicationKey = "VTB Pipeline Application v1";
            mRequestHeaders.RequestId = Guid.NewGuid().ToString();
            mRequestHeaders.Timestamp = DateTime.Now;
            return mRequestHeaders;
        }
        #endregion LMSRequestHeaders

        #region AcceleratorFinalResult
        private AcceleratorFinalResult AcceleratorFinalResultGet(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAUTOMATIC_ACCELERATOR @APPLICATION_ID = {0}", ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            AcceleratorFinalResult mClass = new AcceleratorFinalResult();
            mClass = (AcceleratorFinalResult)FillClass(mClass, mDataTable.Rows[0]);
            return mClass;
        }

        [WebMethod]
        public bool PostAccelerator(int ApplicationId, int UserId, string mAcceleratorFinalResultName)
        {
            bool mResult = false;
            if (UserId != Convert.ToInt32(_AppSets["PosCardLMSUser"]))
            {
                UserId = Convert.ToInt32(_AppSets["AcceleratorLMSUser"]);
            }
            try
            {
                if (mAcceleratorFinalResultName == "")
                {
                    mAcceleratorFinalResultName = this.AcceleratorFinalResultGet(ApplicationId).FINAL_RESULT_MANE;
                    this.ApplicationLogSet(ApplicationId, 1, UserId, "PostAccelerator FinalResultName = " + mAcceleratorFinalResultName);
                }

                LMSAnswer mLMSAnswer = new LMSAnswer();

                switch (mAcceleratorFinalResultName)
                {
                    // The following switch section causes an error.
                    case "APPROVED_LMS_UNCONFIRM":
                        mLMSAnswer = this.LMSAddApplication2(ApplicationId, UserId, (int)ApplicationStatus.New);
                        break;
                    case "APPROVED_LMS_CONFIRM":
                        mLMSAnswer = this.LMSAddApplication2(ApplicationId, UserId, (int)ApplicationStatus.Approved);
                        break;
                    case "APPROVED_LMS_DISBURS":
                        mLMSAnswer = this.LMSAddApplication2(ApplicationId, UserId, (int)ApplicationStatus.Approved);
                        break;
                    case "APPROVED_LMS_AUTHORIZE":
                        mLMSAnswer = this.LMSAddApplication2(ApplicationId, UserId, (int)ApplicationStatus.Approved);
                        break;
                }

                if (mLMSAnswer.Error == 0 && mLMSAnswer.LoanApplicationRecord != null && mLMSAnswer.LoanApplicationRecord.LMS_LOAN_ID > 0)
                {
                    this.ApplicationChangeStateAuto(ApplicationId, UserId, 111, 3100, 0, 0);
                    mResult = true;
                }
            }
            catch
            {
                mResult = false;
            }

            return mResult;
        }

        #endregion AcceleratorFinalResult

        #region CalcDTI
        [WebMethod]
        public double CalcDTI(int application_id)
        {
            LoanApplicationFull mLoanApplicationFull = this.LoanApplicationFullLoad(application_id, 433);
            return CalcDTI(mLoanApplicationFull);
        }

        public double CalcDTI(LoanApplicationFull _LoanApplicationFull)
        {

            double mTotalAmount = 0;

            double mPMT = 0;
            double mLOAN_PMT = 0;
            double mOVER_PMT = 0;
            double mDTI = 0;
            string mCurrency = "";

            mCurrency = _LoanApplicationFull.Record.LOAN_CURRENCY;

            foreach (ApplicationCurrentLoans mApplicationCurrentLoans in _LoanApplicationFull.CurrentLoanList)
            {
                if (!mApplicationCurrentLoans.CURRENT_LOAN_COVER && mApplicationCurrentLoans.IS_CALCULATION)
                {
                    mPMT = Convert.ToDouble(mApplicationCurrentLoans.CURRENT_LOAN_PMT);
                    mCurrency = mApplicationCurrentLoans.CURRENT_LOAN_CURRENCY;

                    mTotalAmount += this.ConvertMoney(mPMT, mCurrency, mCurrency);
                }
            }
            mLOAN_PMT = mTotalAmount;

            mTotalAmount = 0;
            foreach (ApplicationCurrentOverdrafts mApplicationCurrentOverdrafts in _LoanApplicationFull.CurrentOverdraftList)
            {
                if (!mApplicationCurrentOverdrafts.CURRENT_CREDIT_COVER)
                {
                    mPMT = Convert.ToDouble(mApplicationCurrentOverdrafts.CURRENT_CREDIT_CARD_LIMIT) * Convert.ToDouble(mApplicationCurrentOverdrafts.CURRENT_CREDIT_INTEREST_RATE) / 1200.00;
                    mCurrency = mApplicationCurrentOverdrafts.CURRENT_CREDIT_CURRENCY;
                    mTotalAmount += this.ConvertMoney(mPMT, mCurrency, mCurrency);
                }
            }
            mOVER_PMT = mTotalAmount;

            /**/

            double mTotalIncomeConvert = 0;
            double mMonthlyIncome = 0;
            string mMonthlyCurrency = "";
            double mOtherIncome = 0;
            string mOtherCurrency = "";

            try
            {
                mMonthlyIncome = Convert.ToDouble(_LoanApplicationFull.ClientInfo.CLIENT_MONTHLY_INCOME);
                mMonthlyCurrency = _LoanApplicationFull.ClientInfo.CLIENT_MONTHLY_INCOME_CURRENCY;

                mOtherIncome = Convert.ToDouble(_LoanApplicationFull.ClientInfo.CLIENT_OTH_MONTHLY_INCOME);
                mOtherCurrency = _LoanApplicationFull.ClientInfo.CLIENT_OTH_MONTHLY_INCOME_CURRENCY;

                mTotalIncomeConvert = ConvertMoney(mMonthlyIncome, mMonthlyCurrency, mCurrency) + ConvertMoney(mOtherIncome, mOtherCurrency, mCurrency);
            }
            catch
            {
                mTotalIncomeConvert = 0;
            }


            /**/

            double mLoanAmount = Convert.ToDouble(_LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED);
            double mLoanPeriod = Convert.ToDouble(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED);
            double mLoanInterest = Convert.ToDouble(_LoanApplicationFull.Record.INTEREST_RATE);
            mPMT = 0;
            mDTI = 0;
            try
            {
                if (mLoanInterest == 0 && mLoanPeriod > 0 && mLoanAmount > 0)
                {
                    mPMT = Math.Round(mLoanAmount / mLoanPeriod, 2);
                }
                else
                {
                    mPMT = this.CalcPMT(mLoanAmount, mLoanPeriod, mLoanInterest);
                }
                mDTI = Math.Round((mLOAN_PMT + mOVER_PMT + Convert.ToDouble(mPMT)) / Convert.ToDouble(mTotalIncomeConvert) * 100, 2);
            }
            catch
            {
                mPMT = 0;
                mDTI = 0;
            }

            return (mDTI);
        }
        #endregion

        #endregion

        #region ====TEST====

        [WebMethod]
        public CompaniesList test()
        {
            CompaniesList insuranceCopmanies = null;
            insuranceCopmanies = _lmsClientInsurance.ListCompanies(new ListCompaniesRequest { RequestHeaders = LMSRequestHeadersGetI() }).Result;
            return insuranceCopmanies;
        }

        #endregion ====TEST====

        #region == Loan Application ==

        #region == Loan Application Full ==
        [WebMethod]
        public LoanApplicationFull LoanApplicationFullLoad(int application_id, int user_id)
        {
            int application_type_id = 1;
            if (application_id == 0)
            {
                application_id = this.ApplicationCreate(user_id).APPLICATION_ID;
            }

            LoanApplicationFull mLoanApplicationFull = new LoanApplicationFull();
            if (application_id == 0)
            {
                return mLoanApplicationFull;
            }

            try
            {
                mLoanApplicationFull.Auto = ApplicationAutoGet(application_id);
                List<ApplicationClientInfo> mApplicationClientInfoList = ApplicationClientInfoList(0, application_id);
                if (mApplicationClientInfoList.Count > 0)
                {
                    mLoanApplicationFull.ClientInfo = mApplicationClientInfoList.First();
                }
                mLoanApplicationFull.ClientInfoFatca = this.ApplicationClientInfoFatcaGet(application_id);
                mLoanApplicationFull.AppLock = this.ApplicationLockCheck(application_type_id, application_id, user_id);
                mLoanApplicationFull.ApprovalHistoryList = this.ApplicationApprovalHistoryList(application_id);
                mLoanApplicationFull.ClientAssetsList = this.ApplicationClientAssetsList(0, application_id);
                mLoanApplicationFull.ClientContactsList = this.ApplicationClientContactsList(0, application_id);
                mLoanApplicationFull.ClientIsGuarantorList = this.ApplicationClientIsGuarantorList(application_id);
                //mLoanApplicationFull.CoborrowerList = this.ApplicationCoborrowerList("", application_id);
                mLoanApplicationFull.CommentList = this.ApplicationCommentList(application_id);
                mLoanApplicationFull.CurrentLoanList = this.ApplicationCurrentLoansList(application_id);
                mLoanApplicationFull.CurrentOverdraftList = this.ApplicationCurrentOverdraftsList(application_id);
                mLoanApplicationFull.DepositList = this.ApplicationDepositsList(application_id);
                mLoanApplicationFull.GuarantorList = this.ApplicationGuarantorList("", application_id);
                mLoanApplicationFull.GuarantorGuatantedLoanList = this.ApplicationGuarantorGuatantedLoansList(0, application_id, "");
                mLoanApplicationFull.GuarantorLoanList = this.ApplicationGuarantorLoansList(0, application_id, "");
                mLoanApplicationFull.HistoryList = this.ApplicationHistoryList(application_id);
                mLoanApplicationFull.PictureList = this.ApplicationPictureList(application_type_id, application_id);
                mLoanApplicationFull.RealEstate = this.ApplicationRealEstateGet(application_id);
                mLoanApplicationFull.RecomenderList = this.ApplicationRecomenderList(0, application_id);
                mLoanApplicationFull.ScoringList = this.ApplicationScoringList(application_id);
                mLoanApplicationFull.StopFactorCheckList = this.ApplicationStopFactorCheckLogList(application_id);
                mLoanApplicationFull.Record = this.ApplicationRec(application_id, user_id);
                try
                {
                    mLoanApplicationFull.ProductProperties = this.ProductPropertiesList().First(item => item.PRODUCT_ID == mLoanApplicationFull.Record.PRODUCT_ID && item.PRODUCT_CATEGORY_ID == mLoanApplicationFull.Record.PRODUCT_CATEGORY_ID && item.CCY == mLoanApplicationFull.Record.LOAN_CURRENCY && item.MIN_PERIOD < mLoanApplicationFull.Record.LOAN_DAYS_REQUESTED && item.MAX_PERIOD > mLoanApplicationFull.Record.LOAN_DAYS_REQUESTED);
                }
                catch
                {
                    try
                    {
                        mLoanApplicationFull.ProductProperties = this.ProductPropertiesList().First(item => item.PRODUCT_ID == mLoanApplicationFull.Record.PRODUCT_ID);
                    }
                    catch { }
                }
                try
                {
                    mLoanApplicationFull.CreditInfo = ApplicationCreditInfoList(application_id).First();
                }
                catch
                {
                    mLoanApplicationFull.CreditInfo = new ApplicationCreditInfo();
                }
                mLoanApplicationFull.AdditionalAttribules = ApplicationAdditionalAttributeList(application_id);
                mLoanApplicationFull.ApplicationPurchaseFull = ApplicationPurchaseFullGet(application_id);
                mLoanApplicationFull.ApplicationOwnerInstalmentList = this.ApplicationOwnerInstalmentList(application_id);
                mLoanApplicationFull.ApplicationAdminDebtsList = this.ApplicationAdminDebtsList(application_id);
                try
                {
                    mLoanApplicationFull.CashCover = this.AplicationCashCoverGet(application_id);
                }
                catch
                {
                    mLoanApplicationFull.CashCover = new CashCover();
                }

                mLoanApplicationFull.ApplicationUniversity = this.ApplicationUniversityGet(application_id);
                mLoanApplicationFull.ApplicationClientInfoEmployerList = this.ApplicationClientInfoEmployerList(application_id);
                mLoanApplicationFull.AppAdminDebtList = this.AppAdminDebtList(application_id);
            }
            catch { }
            return mLoanApplicationFull;
        }


        [WebMethod]
        public bool LoanApplicationFullSave(LoanApplicationFull mLoanApplicationFull, int user_id)
        {
            bool _result = false;

            try
            {
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "LoanApplicationFullSave start");
                int mApplicationID = mLoanApplicationFull.Record.APPLICATION_ID;

                this.ApplicationSave(mLoanApplicationFull.Record, user_id);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationSave");

                this.ApplicationAutoSave(mLoanApplicationFull.Auto, user_id);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationAutoSave");

                try
                {
                    this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientInfoSave start");
                    this.ApplicationClientInfoSave(mLoanApplicationFull.ClientInfo, user_id);
                    this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientInfoSave end");
                }
                catch (Exception ex)
                {
                    this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientInfoSave::"+ex.Message);
                }

                this.ApplicationClientInfoFatcaSet(mLoanApplicationFull.ClientInfoFatca);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientInfoFatcaSet");

                this.ApplicationClientAssetsListSave(mLoanApplicationFull.ClientAssetsList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientAssetsListSave");

                this.ApplicationClientContactsListSave(mLoanApplicationFull.ClientContactsList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientContactsListSave");

                this.ApplicationClientIsGuarantorListSave(mLoanApplicationFull.ClientIsGuarantorList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientIsGuarantorListSave");

                this.ApplicationCommentListSave(mLoanApplicationFull.CommentList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationCommentListSave");

                this.ApplicationCurrentLoansListSave(mLoanApplicationFull.CurrentLoanList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationCurrentLoansListSave");

                this.ApplicationCurrentOverdraftsListSave(mLoanApplicationFull.CurrentOverdraftList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationCurrentOverdraftsListSave");

                this.ApplicationDepositsListSave(mLoanApplicationFull.DepositList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationDepositsListSave");

                this.ApplicationGuarantorListSave(mLoanApplicationFull.GuarantorList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationGuarantorListSave");

                this.ApplicationGuarantorGuatantedLoansListSave(mLoanApplicationFull.GuarantorGuatantedLoanList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationGuarantorGuatantedLoansListSave");

                this.ApplicationGuarantorLoansListSave(mLoanApplicationFull.GuarantorLoanList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationGuarantorLoansListSave");

                this.ApplicationRealEstateSave(mLoanApplicationFull.RealEstate, user_id);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationRealEstateSave");

                this.ApplicationRecomenderListSave(mLoanApplicationFull.RecomenderList, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationRecomenderListSave");

                this.ApplicationCreditInfoSave(mLoanApplicationFull.CreditInfo);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationCreditInfoSave");

                this.ApplicationAdditionalAttributeListSave(mLoanApplicationFull.AdditionalAttribules, user_id, mApplicationID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationAdditionalAttributeListSave");

                this.ApplicationPurchaseSave(mApplicationID, mLoanApplicationFull.ApplicationPurchaseFull.ApplicationPurchase, mLoanApplicationFull.ApplicationPurchaseFull.ApplicationPurchaseItemList);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationPurchaseSave");

                this.ApplicationUniversitySave(mLoanApplicationFull.ApplicationUniversity);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationUniversitySave");

                this.ApplicationClientInfoEmployerListSet(mLoanApplicationFull.Record.APPLICATION_TYPE_ID, mLoanApplicationFull.ApplicationClientInfoEmployerList);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "ApplicationClientInfoEmployerListSet");

                this.ApplicationLockRemove(mLoanApplicationFull.Record.APPLICATION_TYPE_ID, mLoanApplicationFull.Record.APPLICATION_ID);
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 1, user_id, "LoanApplicationFullSave end");

                _result = true;
            }
            catch (Exception Ex)
            {
                this.ApplicationLogSet(mLoanApplicationFull.Record.APPLICATION_ID, 9, user_id, Ex.Message);
            }

            return _result;
        }
        #endregion

        #region == Application ==

        #region ApplicationCreate
        [WebMethod]
        public LoanApplicationRecord ApplicationCreate(int aplication_owner)
        {
            LoanApplicationRecord _class = new LoanApplicationRecord();
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CREATE @aplication_owner = {0}", aplication_owner);
            try
            {

                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    _class = (LoanApplicationRecord)FillClass(_class, _ds.Tables[0].Rows[0]);
                }
            }
            catch { }
            return _class;
        }

        [WebMethod]
        public LoanApplicationRecord ApplicationCreateByProduct(int aplication_owner, int product_id, string iso)
        {
            LoanApplicationRecord _class = new LoanApplicationRecord();
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CREATE @aplication_owner = {0},@product_id = {1},@iso = {2}", aplication_owner, product_id, StringFunctions.SqlQuoted(iso, true));
            try
            {

                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    _class = (LoanApplicationRecord)FillClass(_class, _ds.Tables[0].Rows[0]);
                }
            }
            catch { }
            return _class;
        }

        #endregion

        #region ApplicationList
        [WebMethod]
        public List<LoanApplications> ApplicationList(int UserId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_LIST @user_id = {0}, @is_archive = 0", UserId);
            mDataTable = _DGate.GetDataSet(mSQL, 300).Tables[0];
            List<LoanApplications> RetList = new List<LoanApplications>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanApplications mClass = new LoanApplications();
                mClass = (LoanApplications)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationList

        #region ApplicationListLinq
        [WebMethod]
        public List<LoanApplications> ApplicationListLinq(int UserId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_LIST @user_id = {0}, @is_archive = 0", UserId);
            mDataTable = _DGate.GetDataSet(mSQL, 300).Tables[0];
            List<LoanApplications> RetList = new List<LoanApplications>();

            RetList = (from DataRow dr in mDataTable.Rows
                       select new LoanApplications()
                       {
                           APPLICATION_ID = Convert.ToInt32(dr["APPLICATION_ID"]),
                           LOAN_AMOUNT_REQUESTED = Convert.ToDecimal(dr["LOAN_AMOUNT_REQUESTED"]),
                           LOAN_CURRENCY = Convert.ToString(dr["LOAN_CURRENCY"])
                       }).ToList();

            return RetList;
        }
        #endregion ApplicationListLinq

        #region product replace
        [WebMethod]
        public int ProductReplace(int base_product_id, int application_source_id, int product_id)
        {
            int retValue = 0;
            string mSQL = String.Format(@"exec dbo.pPRODUCT_REPLACE_GET @base_product_id = {0}, @application_source_id = {1}, @product_id = {2}", base_product_id, application_source_id, product_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["NEW_BASE_PRODUCT_ID"]);
                }
            }
            catch { }
            return retValue;
        }
        #endregion product replace

        #region ApplicationRestoreState
        [WebMethod]
        public List<ApplicationHistory> ApplicationRestoreState(int application_id, int rec_id)
        {
            List<ApplicationHistory> RetList = new List<ApplicationHistory>();
            string mSQL = String.Format(@"exec dbo.pAPPLICATION_RESTORE_STATE @application_id = {0}, @rec_id = {1} ", application_id, rec_id);
            try
            {
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                foreach (DataRow mDataRow in mDataTable.Rows)
                {
                    ApplicationHistory mClass = new ApplicationHistory();
                    mClass = (ApplicationHistory)FillClass(mClass, mDataRow);
                    RetList.Add(mClass);
                }

            }
            catch { }
            return RetList;
        }
        #endregion ApplicationRestoreState

        #region UserApplicationStat
        [WebMethod]
        public List<UserApplicationStat> UserApplicationStatList(int UserId)
        {
            string mSQL = String.Format("EXEC dbo.pUSER_APPLICATION_STAT @user_id = {0}", UserId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<UserApplicationStat> RetList = new List<UserApplicationStat>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                UserApplicationStat mClass = new UserApplicationStat();
                mClass = (UserApplicationStat)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion UserApplicationStat

        #region ApplicationRec
        [WebMethod]

        public LoanApplicationRecord ApplicationRec(int ApplicationId, int UserId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_RECORD_GET @application_id = {0},@user_id = {1}", ApplicationId, UserId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            LoanApplicationRecord mClass = new LoanApplicationRecord();
            try
            {
                mClass = (LoanApplicationRecord)FillClass(mClass, mDataTable.Rows[0]);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return mClass;
        }
        #endregion

        #region ApplicationSave
        [WebMethod]

        public bool ApplicationSave(LoanApplicationRecord _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_SET @application_id = {0},@application_state_id = {1},@branch_id = {2},@aplication_owner = {3},@product_id = {4},@loan_amount_requested = {5}
            ,@loan_days_requested = {6},@loan_purpose_description = {7},@loan_currency = {8},@loan_aim_id = {9},@loan_amount_scoring = {10},@loan_amount_approved = {11},@loan_amount_issued = {12}
            ,@loan_amount_disburs = {13}, @disburs_type_id = {14}
            ,@interest_rate = {15},@notused_rate = {16},@prepayment_rate = {17},@admin_fee_percent = {18},@admin_fee = {19},@fee1_percent = {20},@fee1_min = {21},@fee2_percent = {22},@fee2_min = {23}
            ,@min_prepayment_count = {24},@min_prepayment_amount = {25},@penalty_calculation_type = {26},@overpay_prepayment_rate = {27},@overpay_penalty_calculation_type = {28},@penalty_schema_id = {29}
            ,@penalty_on_payment_in_other_bank = {30},@ob_overpay_penalty_calculation_type = {31},@participation = {32},@max_salary_amount = {33},@general_aggrement_amount = {34}
            ,@general_aggrement_period_yaer = {35},@create_date = {36},@is_standart = {37},@is_archive = {38},@is_cash_cover = {39},@last_work_place_id = {40},@product_category_id = {41},@product_extended_id = {42}
            ,@client_type = {43},@schedule_type = {44},@pmt_interval_type = {45},@resource = {46},@ensure_type = {47}
            ,@prepayment_reschedule_type = {48},@over_limit_interest_rate = {49},@loan_type = {50},@credit_type = {51},@is_card = {52},@generic_schedule = {53},@payment_day_1 = {54},@payment_day_2 = {55},@end_of_month ={56}
            ,@has_payment_day = {57},@has_interest_free_period = {58},@interest_free_period = {59},@term_type = {60},@can_have_coborrower = {61},@has_grace_days = {62},@has_purpose = {63}
            ,@has_grace_holidays = {64},@collaterals = {65},@is_installment = {66},@has_usage_purpose = {67},@has_businesses = {68},@control_limits = {69},@lms_loan_id = {70},@user_id = {71}
            , @client_transh_amount = {72},@card_acc_id = {73},@reject_reason_id = {74},@payment_month = {75},@application_source_id = {76},@account_product_no = {77}, @nonstandard_type = {78}
            , @is_restruct = {79},@unplanned_debt_used_amount_ratio = {80}, @grace_period = {81}, @client_acc_id = {82}"
            , _class.APPLICATION_ID
            , _class.APPLICATION_STATE_ID
            , StringFunctions.SqlQuoted(_class.BRANCH_ID, true)
            , _class.APLICATION_OWNER
            , _class.PRODUCT_ID
            , _class.LOAN_AMOUNT_REQUESTED
            , _class.LOAN_DAYS_REQUESTED
            , StringFunctions.SqlQuoted(_class.LOAN_PURPOSE_DESCRIPTION, true)
            , StringFunctions.SqlQuoted(_class.LOAN_CURRENCY, true)
            , _class.LOAN_AIM_ID
            , _class.LOAN_AMOUNT_SCORING
            , _class.LOAN_AMOUNT_APPROVED
            , _class.LOAN_AMOUNT_ISSUED
            , _class.LOAN_AMOUNT_DISBURS
            , _class.DISBURS_TYPE_ID
            , _class.INTEREST_RATE
            , _class.NOTUSED_RATE
            , _class.PREPAYMENT_RATE
            , _class.ADMIN_FEE_PERCENT
            , _class.ADMIN_FEE
            , _class.FEE1_PERCENT
            , _class.FEE1_MIN
            , _class.FEE2_PERCENT
            , _class.FEE2_MIN
            , _class.MIN_PREPAYMENT_COUNT
            , _class.MIN_PREPAYMENT_AMOUNT
            , _class.PENALTY_CALCULATION_TYPE
            , _class.OVERPAY_PREPAYMENT_RATE
            , _class.OVERPAY_PENALTY_CALCULATION_TYPE
            , _class.PENALTY_SCHEMA_ID
            , _class.PENALTY_ON_PAYMENT_IN_OTHER_BANK
            , _class.OB_OVERPAY_PENALTY_CALCULATION_TYPE
            , _class.PARTICIPATION
            , _class.MAX_SALARY_AMOUNT
            , _class.GENERAL_AGGREMENT_AMOUNT
            , _class.GENERAL_AGGREMENT_PERIOD_YAER
            , StringFunctions.SqlDateQuoted(_class.CREATE_DATE)
            , Convert.ToInt32(_class.IS_STANDART)
            , Convert.ToInt32(_class.IS_ARCHIVE)
            , Convert.ToInt32(_class.IS_CASH_COVER)
            , _class.LAST_WORK_PLACE_ID
            , _class.PRODUCT_CATEGORY_ID
            , _class.PRODUCT_EXTENDED_ID
            , _class.CLIENT_TYPE
            , _class.SCHEDULE_TYPE
            , _class.PMT_INTERVAL_TYPE
            , StringFunctions.SqlQuoted(_class.RESOURCE, true)
            , _class.ENSURE_TYPE
            , _class.PREPAYMENT_RESCHEDULE_TYPE
            , _class.OVER_LIMIT_INTEREST_RATE
            , _class.LOAN_TYPE
            , _class.CREDIT_TYPE
            , Convert.ToInt32(_class.IS_CARD)
            , Convert.ToInt32(_class.GENERIC_SCHEDULE)
            , _class.PAYMENT_DAY_1
            , _class.PAYMENT_DAY_2
            , _class.END_OF_MONTH
            , Convert.ToInt32(_class.HAS_PAYMENT_DAY)
            , Convert.ToInt32(_class.HAS_INTEREST_FREE_PERIOD)
            , _class.INTEREST_FREE_PERIOD
            , _class.TERM_TYPE
            , _class.CAN_HAVE_COBORROWER
            , Convert.ToInt32(_class.HAS_GRACE_DAYS)
            , Convert.ToInt32(_class.HAS_PURPOSE)
            , Convert.ToInt32(_class.HAS_GRACE_HOLIDAYS)
            , StringFunctions.SqlQuoted(_class.COLLATERALS, true)
            , Convert.ToInt32(_class.IS_INSTALLMENT)
            , Convert.ToInt32(_class.HAS_USAGE_PURPOSE)
            , Convert.ToInt32(_class.HAS_BUSINESSES)
            , Convert.ToInt32(_class.CONTROL_LIMITS)
            , _class.LMS_LOAN_ID
            , user_id
            , _class.CLIENT_TRANSH_AMOUNT
            , _class.CARD_ACC_ID
            , _class.REJECT_REASON_ID
            , _class.PAYMENT_MONTH
            , _class.APPLICATION_SOURCE_ID
            , _class.ACCOUNT_PRODUCT_NO
            , _class.NONSTANDARD_TYPE
            , _class.IS_RESTRUCT
            , _class.UNPLANNED_DEBT_USED_AMOUNT_RATIO
            , _class.GRACE_PERIOD
            , _class.CLIENT_ACC_ID
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationDelete
        [WebMethod]

        public bool ApplicationDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_DEL @application_id = {0}"
            , application_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }

        #endregion

        #region ApplicationChangeState
        [WebMethod]

        public bool ApplicationChangeState(int application_id, int user_id, int application_state_id_new, int work_place_id_to, int future_work_place_id, int reject_reason_id)
        {
            bool _retValue = false;
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CHANGE_STATE @application_id = {0},@application_state_id_new = {1},@user_id = {2},@work_place_id_to = {3},@future_work_place_id = {4},@reject_reason_id = {5}"
                , application_id, application_state_id_new, user_id, work_place_id_to, future_work_place_id, reject_reason_id);
            mDataTable = _DGate.GetDataSet(mSQL, 300, true).Tables[0];
            if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
            {
                _retValue = true;
            }


            if (!this.PostAccelerator(application_id, user_id, ""))
            {
                _retValue = false;
            }

            return _retValue;
        }

        public bool ApplicationChangeStateAuto(int application_id, int user_id, int application_state_id_new, int work_place_id_to, int future_work_place_id, int reject_reason_id)
        {
            bool _retValue = false;
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CHANGE_STATE @application_id = {0},@application_state_id_new = {1},@user_id = {2},@work_place_id_to = {3},@future_work_place_id = {4},@reject_reason_id = {5}"
                , application_id, application_state_id_new, user_id, work_place_id_to, future_work_place_id, reject_reason_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
            {
                _retValue = true;
            }
            return _retValue;
        }

        #endregion

        #region ApplicationPOSSendToRisk
        [WebMethod]
        public POSAnswer ApplicationPOSSendToRisk(int application_id, int user_id)
        {
            DataSet mDs;
            POSAnswer mClass = new POSAnswer();
            string mSQL = String.Format("EXEC dbo.pPOS_SENDTO_RISK @application_id = {0},@user_id = {1}", application_id, user_id);
            mDs = _DGate.GetDataSet(mSQL, 300, true);
            if (mDs != null && mDs.Tables != null && mDs.Tables.Count > 0)
            {
                mDataTable = mDs.Tables[0];
                mClass = (POSAnswer)FillClass(mClass, mDataTable.Rows[0]);
                try
                {
                    mDataTable = mDs.Tables[1];
                    string mAcceleratorFinalResultName = mDataTable.Rows[0]["FINAL_RESULT_MANE"].ToString();
                    this.ApplicationLogSet(application_id, 1, user_id, "ApplicationPOSSendToRisk FinalResultName=" + mAcceleratorFinalResultName);
                    this.PostAccelerator(application_id, user_id, mAcceleratorFinalResultName);
                    mClass.APPLICATION_STATE_ID = this.ApplicationRec(application_id, user_id).APPLICATION_STATE_ID;
                }
                catch (Exception Ex)
                {
                    mClass.APPLICATION_STATE_ID = 0;
                    mClass.ERROR_MESSAGE = Ex.Message;
                }
            }
            else
            {
                mClass.APPLICATION_STATE_ID = 0;
                mClass.ERROR_MESSAGE = "Error sending to risk";

            }
            return mClass;



        }

        #endregion ApplicationPOSSendToRisk

        #region ApplicationLmsLoanIDSet
        [WebMethod]

        public bool ApplicationLmsLoanIDSet(int application_id, int user_id, int lms_loan_id)
        {
            bool _retValue = false;
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_LOAN_ID_SET @application_id = {0},@user_id = {1},@lms_loan_id = {2}"
                , application_id, user_id, lms_loan_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) > 0)
            {
                _retValue = true;
            }
            return _retValue;
        }

        #endregion

        #region ApplicationMailSend
        [WebMethod]

        public bool ApplicationMailSend(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CREDITCARD_EMAIL @application_id = {0}", application_id);
            try
            {
                _DGate.Execute(mSQL);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region ApplicationMailSend
        [WebMethod]

        public bool ApplicationMailClientHereSend(int application_id, int user_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_HERE_EMAIL @application_id = {0},@user_id = {1}", application_id, user_id);
            try
            {
                _DGate.Execute(mSQL);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region == ApplicationApprove ==
        #region ApplicationApprove
        [WebMethod]

        public bool ApplicationApprove(int application_id, int user_id, int committee_user_id, int decision_id, string descrip, decimal amount_approved)
        {
            bool retValue = false;
            string mSQL = String.Format(@"exec pAPPLICATION_APPROVAL @application_id = {0},@user_id = {1},@committee_user_id = {2},@decision_id = {3},@descrip = {4},@amount_approved = {5}"
            , application_id
            , user_id
            , committee_user_id
            , decision_id
            , StringFunctions.SqlQuoted(descrip, true)
            , amount_approved
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }

        [WebMethod]
        public int ApplicationApproveAutomaticaly(int application_id, int user_id, int approved, int reject_reason_id, int mBaseProductID)
        {
            int retValue = 0;
            DataSet _ds;
            string mSQL = String.Format(@"exec pAPPLICATION_APPROVAL_AUTOMATICALLY @application_id = {0}, @user_id = {1}, @is_approved = {2},@reject_reason_id = {3}"
            , application_id
            , user_id
            , approved
            , reject_reason_id
            );

            try
            {

                _ds = _DGate.GetDataSet(mSQL, 300);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = 1;
                }
            }
            catch { }

            if (retValue == 1 && approved == 1 && (mBaseProductID == 102 || mBaseProductID == 165 || mBaseProductID == 116))
            {
                int mDefaultAdminUserID = Convert.ToInt32(_AppSets["AcceleratorLMSUser"]);
                int mApplicationState = (int)ApplicationStatus.Approved;

                if (mBaseProductID == 102)
                {
                    mApplicationState = (int)ApplicationStatus.New;
                    mSQL = String.Format("EXEC pSYS_PARAMETERS_FOR_LMS_MOVE @application_id = {0}", application_id);
                    try
                    {
                        _ds = _DGate.GetDataSet(mSQL);
                        mDefaultAdminUserID = Convert.ToInt32(_ds.Tables[0].Rows[0]["DEFAULT_ADMIN_USER_ID"]);
                    }
                    catch { }
                }

                try
                {
                    LMSAnswer mLMSAnswer = this.LMSAddApplication2(application_id, mDefaultAdminUserID, mApplicationState);
                    LoanApplicationRecord mLoanApplicationRecord = mLMSAnswer.LoanApplicationRecord;
                    if (mLoanApplicationRecord.LMS_LOAN_ID > 0)
                    {
                        this.ApplicationChangeStateAuto(application_id, mDefaultAdminUserID, 111, 3100, 0, 0);
                        retValue = 2;
                    }
                }
                catch { }

            }
            else
            {
                if (retValue == 1)
                {
                    retValue = 2;
                }
            }
            return retValue;
        }

        [WebMethod]
        public bool ApplicationRiskManagerSet(int application_id, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"exec dbo.pAPPLICATION_RISK_MANAGER_SET @application_id = {0}, @risk_manager_id = {1}", application_id, user_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationApprovalHistoryList
        [WebMethod]
        public List<ApplicationApprovalHistory> ApplicationApprovalHistoryList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_APPROVAL_HISTORY @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationApprovalHistory> RetList = new List<ApplicationApprovalHistory>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationApprovalHistory mClass = new ApplicationApprovalHistory();
                mClass = (ApplicationApprovalHistory)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion
        #endregion

        #region ApplicationArchiveList
        [WebMethod]

        public List<LoanApplications> ApplicationArchiveList(int ApplicationId, int UserId, DateTime StartDate, DateTime EndDate, int ProductGroupId, int ProductId, string ClientPersonalId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_ARCHIVE_GET @application_id = {0},@user_id = {1},@is_archive = 1,@start_date = {2},@end_date = {3},@product_group_id = {4},@product_id = {5},@client_personal_id = {6}"
                , ApplicationId
                , UserId
                , StringFunctions.SqlDateQuoted(StartDate)
                , StringFunctions.SqlDateQuoted(EndDate)
                , ProductGroupId
                , ProductId
                , StringFunctions.SqlQuoted(ClientPersonalId, true)
                );
            mDataTable = _DGate.GetDataSet(mSQL, 60, true).Tables[0];
            List<LoanApplications> RetList = new List<LoanApplications>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanApplications mClass = new LoanApplications();
                mClass = (LoanApplications)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationPrintForm
        [WebMethod]
        public ApplicationPrintForm ApplicationPrintFormGet(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_PRINT_FORM_GET @application_id = {0}", ApplicationId);
            DataSet mDataSet = _DGate.GetDataSet(mSQL);

            ApplicationPrintForm mClass = new ApplicationPrintForm();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[0].Rows.Count > 0)
            {
                DataRow mDataRow = mDataSet.Tables[0].Rows[0];
                mClass = (ApplicationPrintForm)FillClass(mClass, mDataRow);
            }
            return mClass;
        }

        [WebMethod]
        public ApplicationPrintStudent ApplicationPrintStudentGet(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_PRINT_STUDENT @APPLICATION_ID = {0}", ApplicationId);
            DataSet mDataSet = _DGate.GetDataSet(mSQL);

            ApplicationPrintStudent mClass = new ApplicationPrintStudent();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[0].Rows.Count > 0)
            {
                DataRow mDataRow = mDataSet.Tables[0].Rows[0];
                mClass = (ApplicationPrintStudent)FillClass(mClass, mDataRow);
            }
            return mClass;
        }

        [WebMethod]
        public AppProtocol ApplicationPrintProtocol(int ApplicationId)
        {
            AppProtocol mAppProtocol = new AppProtocol();

            string mSQL = String.Format("EXEC dbo.pAPPLICATION_PRINT_PROTOCOL @application_id = {0}", ApplicationId);
            DataSet mDataSet = _DGate.GetDataSet(mSQL);

            // MAIN
            AppProtocolMain Main = new AppProtocolMain();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[0].Rows.Count > 0)
            {
                DataRow mMainRow = mDataSet.Tables[0].Rows[0];
                Main = (AppProtocolMain)FillClass(Main, mMainRow);
            }

            // REAL ESTATE
            List<AppProtocolRealEstate> RealEstateList = new List<AppProtocolRealEstate>();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow mRealEstateRow in mDataSet.Tables[1].Rows)
                {
                    AppProtocolRealEstate realEstate = new AppProtocolRealEstate();
                    realEstate = (AppProtocolRealEstate)FillClass(realEstate, mRealEstateRow);
                    RealEstateList.Add(realEstate);
                }
            }

            // AUTO
            List<AppProtocolAuto> AutoList = new List<AppProtocolAuto>();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow mAutoRow in mDataSet.Tables[2].Rows)
                {
                    AppProtocolAuto auto = new AppProtocolAuto();
                    auto = (AppProtocolAuto)FillClass(auto, mAutoRow);
                    AutoList.Add(auto);
                }
            }

            // CASH COVER
            List<AppProtocolUsedDepo> UsedDepoList = new List<AppProtocolUsedDepo>();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow mCashCoverRow in mDataSet.Tables[3].Rows)
                {
                    AppProtocolUsedDepo UsedDepo = new AppProtocolUsedDepo();
                    UsedDepo = (AppProtocolUsedDepo)FillClass(UsedDepo, mCashCoverRow);
                    UsedDepoList.Add(UsedDepo);
                }
            }

            List<AppProtocolCashCover> CashCoverList = new List<AppProtocolCashCover>();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[4].Rows.Count > 0)
            {
                foreach (DataRow mCashCoverRow in mDataSet.Tables[4].Rows)
                {
                    AppProtocolCashCover cashCover = new AppProtocolCashCover();
                    cashCover = (AppProtocolCashCover)FillClass(cashCover, mCashCoverRow);
                    CashCoverList.Add(cashCover);
                }
            }

            // GUARANTOR
            List<AppProtocolGuarantor> GuarantorList = new List<AppProtocolGuarantor>();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[5].Rows.Count > 0)
            {
                foreach (DataRow mGuarantorRow in mDataSet.Tables[5].Rows)
                {
                    AppProtocolGuarantor guarantor = new AppProtocolGuarantor();
                    guarantor = (AppProtocolGuarantor)FillClass(guarantor, mGuarantorRow);
                    GuarantorList.Add(guarantor);
                }
            }

            // COMMENT
            List<AppProtocolComment> CommentList = new List<AppProtocolComment>();
            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[6].Rows.Count > 0)
            {
                foreach (DataRow mGuarantorRow in mDataSet.Tables[6].Rows)
                {
                    AppProtocolComment comment = new AppProtocolComment();
                    comment = (AppProtocolComment)FillClass(comment, mGuarantorRow);
                    CommentList.Add(comment);
                }
            }

            mAppProtocol.Main = Main;
            mAppProtocol.RealEstateList = RealEstateList;
            mAppProtocol.AutoList = AutoList;
            mAppProtocol.UsedDepoList = UsedDepoList;
            mAppProtocol.CashCoverList = CashCoverList;
            mAppProtocol.GuarantorList = GuarantorList;
            mAppProtocol.CommentList = CommentList;

            return mAppProtocol;
        }
        #endregion

        #region Application Incone Avarage
        [WebMethod]
        public List<ApplicationIncomeAvarage> ApplicationIncomeAvarageList(int mApplicationId, string mPersonalID, int mIsmain)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_INCOME_AVG_LIST @application_id = {0}, @personal_id = {1}, @is_main = {2} ",
                mApplicationId,
                StringFunctions.SqlQuoted(mPersonalID),
                mIsmain
                );

            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationIncomeAvarage> RetList = new List<ApplicationIncomeAvarage>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationIncomeAvarage mClass = new ApplicationIncomeAvarage();
                mClass = (ApplicationIncomeAvarage)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]

        public bool ApplicationIncomeAvarageSet(int mApplicationId, string mPersonalID, int mIsMain, List<ApplicationIncomeAvarage> mApplicationIncomeAvarageList)
        {
            string mSQL = "";
            bool mRetValue = false;
            DataRow mDataRow;
            try
            {
                mSQL = String.Format("EXEC dbo.pAPPLICATION_INCOME_AVG_DEL @application_id = {0}, @personal_id = {1}, @is_main = {2}",
                    mApplicationId,
                    StringFunctions.SqlQuoted(mPersonalID),
                    mIsMain
                    );

                _DGate.Execute(mSQL);

                foreach (ApplicationIncomeAvarage mApplicationIncomeAvarage in mApplicationIncomeAvarageList)
                {
                    mSQL = String.Format("EXEC dbo.pAPPLICATION_INCOME_AVG_SET @application_id = {0}, @personal_id = {1}, @income_amount = {2}, @is_main = {3}",
                        mApplicationIncomeAvarage.APPLICATION_ID,
                        StringFunctions.SqlQuoted(mApplicationIncomeAvarage.PERSONAL_ID),
                        mApplicationIncomeAvarage.INCOME_AMOUNT,
                        mApplicationIncomeAvarage.IS_MAIN
                        );

                    mDataRow = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];
                    if (!Convert.ToBoolean(mDataRow["RESULT"]))
                    {
                        mRetValue = false;
                    }
                }

                mRetValue = true;
            }
            catch
            {
                mRetValue = false;
            }

            return mRetValue;
        }
        #endregion

        #region Application Client Side Log
        [WebMethod]

        public bool ApplicationLogSet(int mApplicationId, int mLogType, int mUserId, string mLogMessage)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pSYS_CLIENT_SIDE_LOG_SET @application_id = {0},@log_type = {1},@user_id = {2},@log_message = {3} ",
                    mApplicationId, mLogType, mUserId, StringFunctions.SqlQuoted(mLogMessage, true));

                _DGate.Execute(mSQL);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ApplicationOwnerUpdate
        [WebMethod]

        public bool ApplicationOwnerUpdate(int mApplicationId, int mAaplicationOwnerId)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_OWNER_UPDATE @application_id = {0},@aplication_owner_id = {1}",
                    mApplicationId, mAaplicationOwnerId);

                _DGate.Execute(mSQL);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationOwnerUpdate

        #endregion

        #region == ApplicationAuto ==
        #region ApplicationAutoDelete
        [WebMethod]
        public bool ApplicationAutoDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_AUTO_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationAutoGet
        [WebMethod]
        public ApplicationAuto ApplicationAutoGet(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_AUTO_GET @application_id = {0}", ApplicationId);
            ApplicationAuto mClass = new ApplicationAuto();
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            if (mDataTable.Rows.Count > 0)
            {
                DataRow mDataRow = mDataTable.Rows[0];
                mClass = (ApplicationAuto)FillClass(mClass, mDataRow);
            }
            return mClass;
        }
        #endregion

        #region ApplicationAutoSave
        [WebMethod]
        public bool ApplicationAutoSave(ApplicationAuto _class, int user_id)
        {
            bool retValue = false;
            if (_class.APPLICATION_ID == 0)
            {
                return true;
            }
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_AUTO_SET @rec_id = {0}, @application_id = {1}, @manufacturer = {2}, @model = {3}
                                , @release_date = {4}, @car_age = {5}, @vin_code = {6}, @dealer_id = {7}
                                , @discount_rate = {8}, @price_amount = {9} , @price_market_amount = {10}, @participation_amount = {11}
                                , @insurance_third_party_amount = {12}, @insurance_driver_passenger_amount = {13}, @insurance_rate = {14}, @insurance_amount = {15}
                                , @commission_shss_service_amount = {16}, @commission_auto_deal_amount = {17}, @commission_admin_amount = {18}, @loan_amount_per_month = {19}
                                , @insurance_amount_per_month = {20}, @calc_amount = {21}, @calc_interest = {22}, @custom_clearance_amount = {23}
                                , @treasury_amount = {24}, @grace_period= {25}, @insurance_company_id = {26}, @convert_rate = {27}
                                , @convert_amount = {28}, @paralel_loan_amount = {29}, @user_id = {30}
                                , @insurance_rate_id = {31}, @driver_passenger_insurance_id = {32}, @third_party_insurance_id = {33}
                                , @include_dealer_amount = {34}, @price_amount_real = {35}"
                , _class.REC_ID
                , _class.APPLICATION_ID
                , StringFunctions.SqlQuoted(_class.MANUFACTURER, true)
                , StringFunctions.SqlQuoted(_class.MODEL, true)
                , StringFunctions.SqlDateQuoted(_class.RELEASE_DATE)
                , _class.CAR_AGE
                , StringFunctions.SqlQuoted(_class.VIN_CODE, true)
                , _class.DEALER_ID
                , _class.DISCOUNT_RATE
                , _class.PRICE_AMOUNT
                , _class.PRICE_MARKET_AMOUNT
                , _class.PARTICIPATION_AMOUNT
                , _class.INSURANCE_THIRD_PARTY_AMOUNT
                , _class.INSURANCE_DRIVER_PASSENGER_AMOUNT
                , _class.INSURANCE_RATE
                , _class.INSURANCE_AMOUNT
                , _class.COMMISSION_SHSS_SERVICE_AMOUNT
                , _class.COMMISSION_AUTO_DEAL_AMOUNT
                , _class.COMMISSION_ADMIN_AMOUNT
                , _class.LOAN_AMOUNT_PER_MONTH
                , _class.INSURANCE_AMOUNT_PER_MONTH
                , _class.CALC_AMOUNT
                , _class.CALC_INTEREST
                , _class.CUSTOM_CLEARANCE_AMOUNT
                , _class.TREASURY_AMOUNT
                , _class.GRACE_PERIOD
                , _class.INSURANCE_COMPANY_ID
                , _class.CONVERT_RATE
                , _class.CONVERT_AMOUNT
                , _class.PARALEL_LOAN_AMOUNT
                , user_id
                , _class.INSURANCE_RATE_ID
                , _class.DRIVER_PASSENGER_INSURANCE_ID
                , _class.THIRD_PARTY_INSURANCE_ID
                , _class.INCLUDE_DEALER_AMOUNT
                , _class.PRICE_AMOUNT_REAL
                );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationClientAssets ==
        #region ApplicationClientAssetsDelete
        [WebMethod]

        public bool ApplicationClientAssetsDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_ASSETS_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationClientAssetsList
        [WebMethod]

        public List<ApplicationClientAssets> ApplicationClientAssetsList(int ClientAssetId, int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_ASSETS_GET @client_asset_id = {0},@application_id = {1}", ClientAssetId, ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientAssets> RetList = new List<ApplicationClientAssets>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientAssets mClass = new ApplicationClientAssets();
                mClass = (ApplicationClientAssets)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region ApplicationClientAssetsListSave
        [WebMethod]

        public bool ApplicationClientAssetsListSave(List<ApplicationClientAssets> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationClientAssetsDelete(application_id);
                foreach (ApplicationClientAssets _class in _classList)
                {
                    ApplicationClientAssetsSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationClientAssetsSave
        [WebMethod]

        public bool ApplicationClientAssetsSave(ApplicationClientAssets _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_ASSETS_SET @client_asset_id = {0},@application_id = {1},@client_asset_type_id = {2},@client_asset_description = {3},@client_asset_cost = {4},@client_asset_cost_ccy = {5},@user_id = {6}"
            , _class.CLIENT_ASSET_ID
            , _class.APPLICATION_ID
            , _class.CLIENT_ASSET_TYPE_ID
            , StringFunctions.SqlQuoted(_class.CLIENT_ASSET_DESCRIPTION, true)
            , _class.CLIENT_ASSET_COST
            , StringFunctions.SqlQuoted(_class.CLIENT_ASSET_COST_CCY, true)
            , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationClientContacts ==
        #region ApplicationClientContactsDelete
        [WebMethod]

        public bool ApplicationClientContactsDelete(int application_id, int client_contact_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_CONTACTS_DEL @application_id = {0},@client_contact_id = {1}", application_id, client_contact_id);
            try
            {

                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationClientContactsList
        [WebMethod]

        public List<ApplicationClientContacts> ApplicationClientContactsList(int ClientContactId, int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_CONTACTS_GET @client_contact_id = {0},@application_id = {1}", ClientContactId, ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientContacts> RetList = new List<ApplicationClientContacts>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientContacts mClass = new ApplicationClientContacts();
                mClass = (ApplicationClientContacts)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationClientContactsSave
        [WebMethod]

        public bool ApplicationClientContactsSave(ApplicationClientContacts _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_CONTACTS_SET @client_contact_id = {0},@application_id = {1},@contact_person_name = {2},@contact_person_surname = {3},@contact_person_phone = {4},@contact_person_mobile = {5},@contact_person_address = {6},@contact_person_relationship_type = {7},@contact_person_organization_name = {8},@user_id = {9}"
            , _class.CLIENT_CONTACT_ID
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.CONTACT_PERSON_NAME, true)
            , StringFunctions.SqlQuoted(_class.CONTACT_PERSON_SURNAME, true)
            , StringFunctions.SqlQuoted(_class.CONTACT_PERSON_PHONE, true)
            , StringFunctions.SqlQuoted(_class.CONTACT_PERSON_MOBILE, true)
            , StringFunctions.SqlQuoted(_class.CONTACT_PERSON_ADDRESS, true)
            , _class.CONTACT_PERSON_RELATIONSHIP_TYPE
            , StringFunctions.SqlQuoted(_class.CONTACT_PERSON_ORGANIZATION_NAME, true)
            , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #region ApplicationClientContactsSave
        [WebMethod]

        public bool ApplicationClientContactsListSave(List<ApplicationClientContacts> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationClientContactsDelete(application_id, 0);
                foreach (ApplicationClientContacts _class in _classList)
                {
                    ApplicationClientContactsSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region ApplicationLimit
        [WebMethod]
        public List<LimitChildren> ApplicationLimitChildrenList(int mApplication_id)
        {
            string mSQL = String.Format("EXEC dbo.pLIMIT_APPLICATION_CHILDREN_LIST @application_id = {0}", mApplication_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LimitChildren> RetList = new List<LimitChildren>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LimitChildren mClass = new LimitChildren();
                mClass = (LimitChildren)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public bool LimitScoring(int ApplicationId, decimal mIssuedAmount, int UserId)
        {
            bool mRetValue = false;
            try
            {
                string mSQL = String.Format("EXEC dbo.pLIMIT_SCORING_CALC @APPLICATION_ID = {0}, @ISSUED_AMOUNT = {1}, @USER_ID = {2}", ApplicationId, mIssuedAmount, UserId);
                DataSet mDS = _DGate.GetDataSet(mSQL, 300, true);
                mRetValue = true;
            }
            catch { }
            return mRetValue;
        }

        [WebMethod]
        public bool LimitScoringChildren(int mParentApplicationId, int mChildrenApplicationID, decimal mChildrenIssuedAmount, bool mIsActive, int mUserId)
        {
            bool mRetValue = false;
            try
            {
                string mSQL = String.Format("EXEC dbo.pLIMIT_SCORING_CALC_CHILDREN @PARENT_APPLICATION_ID = {0}, @CHILDREN_APPLICATION_ID = {1}, @CHILDREN_ISSUED_AMOUNT = {2}, @IS_ACTIVE = {3}, @USER_ID = {4}",
                    mParentApplicationId,
                    mChildrenApplicationID,
                    mChildrenIssuedAmount,
                    mIsActive,
                    mUserId);

                DataSet mDS = _DGate.GetDataSet(mSQL);
                mRetValue = true;
            }
            catch { }
            return mRetValue;
        }
        #endregion

        #region == ApplicationClientInfo ==
        #region ApplicationClientInfoDelete
        [WebMethod]

        public bool ApplicationClientInfoDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_INFO_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationClientInfoList
        [WebMethod]
        public List<ApplicationClientInfo> ApplicationClientInfoList(int clientInfoId, int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_INFO_GET @client_info_id = {0},@application_id = {1}", clientInfoId, ApplicationId);
            DataSet mDS = _DGate.GetDataSet(mSQL);

            mDataTable = mDS.Tables[0];
            List<ApplicationClientInfo> RetList = new List<ApplicationClientInfo>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientInfo mClass = new ApplicationClientInfo();
                List<ClientIncome> IncomeList = new List<ClientIncome>();

                mClass = (ApplicationClientInfo)FillClass(mClass, mDataRow);

                int appID = Convert.ToInt32(mDataRow["APPLICATION_ID"]);
                List<DataRow> IncomeRows = mDS.Tables[1].Select("APPLICATION_ID =" + appID.ToString()).ToList();
                if (IncomeRows.Count > 0)
                {
                    foreach (DataRow mRow in IncomeRows)
                    {
                        ClientIncome income = new ClientIncome();
                        income = (ClientIncome)FillClass(income, mRow);
                        IncomeList.Add(income);
                    }
                }

                mClass.ClientIncomeList = IncomeList;

                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationExtend
        [WebMethod]
        public ApplicationExtend ApplicationExtendFromLMS(int mApplication_id, int mClient_no)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_EXTEND_GET_FROM_LMS @application_id = {0}, @client_no = {1}", mApplication_id, mClient_no);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            ApplicationExtend mClass = new ApplicationExtend();
            if (mDataTable.Rows.Count > 0)
            {
                DataRow mDataRow = mDataTable.Rows[0];
                mClass = (ApplicationExtend)FillClass(mClass, mDataRow);
            }
            return mClass;
        }
        #endregion

        #region ApplicationClientInfoFatcaLoad
        [WebMethod]
        public ApplicationClientInfoFatca ApplicationClientInfoFatcaLoad(string personal_id)
        {
            UTFODBService _odb = new UTFODBService();

            ApplicationClientInfoFatca _class = new ApplicationClientInfoFatca();
            DataRow mDataRow = null;
            DataSet _ds = null;
            try
            {
                _odb.Url = _AppSets["ODBServive.URL"];
                ODBAnswer _answer = _odb.GetClientInfoFatcaByPID(personal_id);
                _ds = _answer.AnswerDataSet;
                if (_ds != null && _ds.Tables != null && _ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
                {
                    mDataRow = _ds.Tables[0].Rows[0];
                }
                _class.HAS_GREEN_CARD = (DBNull.Value.Equals(mDataRow["HAS_GREEN_CARD"])) ? 0 : Convert.ToInt32(mDataRow["HAS_GREEN_CARD"]);
                _class.LAST_UPDATE = (DBNull.Value.Equals(mDataRow["LAST_UPDATE"])) ? DateTime.MinValue : Convert.ToDateTime(mDataRow["LAST_UPDATE"]);
            }
            catch { }
            return _class;
        }
        #endregion ApplicationClientInfoFatcaLoad

        #region --real salary--
        [WebMethod]
        public RealSalary ClientRealSalary(string personal_id, int application_id)
        {
            RealSalary mRealSalary = new RealSalary();
            string mSQL = String.Format("EXEC dbo.pCLIENT_SALARY_GET @application_id = {0},@personal_id = {1}", application_id, StringFunctions.SqlQuoted(personal_id, false));
            try
            {
                mRealSalary.MEDIAN = Convert.ToDecimal(_DGate.GetDataSet(mSQL).Tables[0].Rows[0]["MEDIAN"]);
                mRealSalary.IS_VIP_OR_EMPLOYEE = Convert.ToBoolean(_DGate.GetDataSet(mSQL).Tables[0].Rows[0]["IS_VIP_OR_EMPLOYEE"]);
                mRealSalary.IS_INCASSO = Convert.ToBoolean(_DGate.GetDataSet(mSQL).Tables[0].Rows[0]["IS_INCASSO"]);
                mRealSalary.IS_SEQUESTRATION = Convert.ToBoolean(_DGate.GetDataSet(mSQL).Tables[0].Rows[0]["IS_SEQUESTRATION"]);
                mRealSalary.IS_PROBLEM = Convert.ToBoolean(_DGate.GetDataSet(mSQL).Tables[0].Rows[0]["IS_PROBLEM"]);
            }
            catch
            {
                mRealSalary.MEDIAN = 0;
                mRealSalary.IS_VIP_OR_EMPLOYEE = false;
                mRealSalary.IS_INCASSO = false;
                mRealSalary.IS_SEQUESTRATION = false;
                mRealSalary.IS_PROBLEM = false;
            }
            return mRealSalary;
        }
        #endregion --real salary--

        #region --CLIENT SALARY MEDIAN--
        [WebMethod]
        public decimal ClientSalaryMedian(string personal_id)
        {
            decimal mMedian = 0;
            string mSQL = String.Format("[dbo].[pCLIENT_SALARY_MEDIAN] @personal_id = {0}", StringFunctions.SqlQuoted(personal_id, false));
            try
            {
                mMedian = Convert.ToDecimal(_DGate.GetDataSet(mSQL).Tables[0].Rows[0]["MEDIAN"]);
            }
            catch
            { }
            return mMedian;
        }
        #endregion --CLIENT SALARY MEDIAN--

        #region --PreaprovedApplication--
        [WebMethod]
        public PreaprovedApplication PreaprovedApplicationGet(string personal_id)
        {
            string mSQL = String.Format("EXEC pPREAPPROVED_LOAN_INFO @personal_id = {0}", StringFunctions.CheckNull(personal_id));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            PreaprovedApplication mClass = new PreaprovedApplication();
            if (mDataTable.Rows.Count > 0)
            {
                DataRow mDataRow = mDataTable.Rows[0];
                mClass = (PreaprovedApplication)FillClass(mClass, mDataRow);
            }
            return mClass;
        }
        #endregion --PreaprovedApplication--

        #region --Fatca--
        [WebMethod]
        public FatcaODB ClientFatcaODBGet(string personal_id)
        {
            FatcaODB mFatcaODB = new FatcaODB();
            UTFODBService _odb = new UTFODBService();
            _odb.Url = _AppSets["ODBServive.URL"];
            ODBAnswer _answer = _odb.GetClientInfoFatcaByPID(personal_id);
            DataSet _ds = _answer.AnswerDataSet;

            if (_ds != null && _ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
            {
                mFatcaODB.CITIZENSHIP = Convert.ToString(_ds.Tables[0].Rows[0]["CITIZENSHIP"]);
                mFatcaODB.CLIENT_NO = Convert.ToInt32(_ds.Tables[0].Rows[0]["CLIENT_NO"]);
                mFatcaODB.COUNTRY_OF_BIRTH = Convert.ToString(_ds.Tables[0].Rows[0]["COUNTRY_OF_BIRTH"]);
                mFatcaODB.DOUBLE_CITIZENSHIP = Convert.ToString(_ds.Tables[0].Rows[0]["DOUBLE_CITIZENSHIP"]);
                mFatcaODB.GIIN_CODE = Convert.ToString(_ds.Tables[0].Rows[0]["GIIN_CODE"]);
                mFatcaODB.GIIN_STATUS = Convert.ToInt32(_ds.Tables[0].Rows[0]["GIIN_STATUS"]);
                mFatcaODB.HAS_GREEN_CARD = Convert.ToInt32(_ds.Tables[0].Rows[0]["HAS_GREEN_CARD"]);
                mFatcaODB.IS_COMPLATED = Convert.ToBoolean(_ds.Tables[0].Rows[0]["IS_COMPLATED"]);
                mFatcaODB.LAST_UPDATE = Convert.ToDateTime(_ds.Tables[0].Rows[0]["LAST_UPDATE"]);
                mFatcaODB.NPFFI = Convert.ToBoolean(_ds.Tables[0].Rows[0]["NPFFI"]);
                mFatcaODB.OTHER_TAX_STATUS = Convert.ToInt32(_ds.Tables[0].Rows[0]["OTHER_TAX_STATUS"]);
                mFatcaODB.RECALCITRANT = Convert.ToBoolean(_ds.Tables[0].Rows[0]["RECALCITRANT"]);
                mFatcaODB.STATUS = Convert.ToInt32(_ds.Tables[0].Rows[0]["STATUS"]);
            }
            return mFatcaODB;
        }
        #endregion --Fatca--

        #region --cra--
        [WebMethod]
        public PersonInfo CraInfoGet(string personal_id, int user_id, DateTime birth_date)
        {
            try
            {
                CRAService _cra = new CRAService();
                _cra.Url = _AppSets["CRAService.URL"];
                return _cra.GetDataByIDAndYearAlta(personal_id, birth_date.Date.Year, true, user_id, true);
            }
            catch
            {
                return new PersonInfo();
            }
        }

        #endregion --cra--

        #region ApplicationClientInfoLoad
        [WebMethod]
        public List<ApplicationClientInfo> ApplicationClientInfoLoad(string personal_id, DateTime mBirthDate)
        {
            return this.ApplicationClientInfoLoadWithSalary(personal_id, 0, mBirthDate);
        }

        #region ApplicationClientInfoLoadWithSalary

        [WebMethod]
        public List<ApplicationClientInfo> ApplicationClientInfoLoadWithSalary(string personal_id, int application_id, DateTime mBirthDate)
        {
            //bool goCRA = Convert.ToBoolean(_AppSets["goCRA"]);
            return this.ApplicationClientInfoLoadWithSalary2(personal_id, application_id, mBirthDate, false, true);
        }

        [WebMethod]
        public List<ApplicationClientInfo> ApplicationClientInfoLoadWithSalary2(string personal_id, int application_id, DateTime mBirthDate, bool goCRA, bool getSalary)
        {
            return ApplicationClientInfoLoadWithSalary3(personal_id, application_id, mBirthDate, goCRA, getSalary, 2);
        }

        [WebMethod]
        public List<ApplicationClientInfo> ApplicationClientInfoLoadWithSalary3(string personal_id, int application_id, DateTime mBirthDate, bool goCRA, bool getSalary, int odb_user_id)
        {
            //goCRA = Convert.ToBoolean(_AppSets["goCRA"]);

            if (odb_user_id == 0)
            {
                odb_user_id = 2;
            }

            UTFODBService _odb = new UTFODBService();
            _odb.Url = _AppSets["ODBServive.URL"];
            List<ApplicationClientInfo> RetList = new List<ApplicationClientInfo>();
            ApplicationClientInfo _class = new ApplicationClientInfo();
            _class.ClientIncomeList = new List<ClientIncome>();
            DataRow mDataRow = null;
            DataRow mDataRowSalary = null;
            DataSet _ds = null;
            List<City> cityList;
            decimal mMedian = 0;
            int mAccountProductNo = 0;
            bool mIs_vip_or_employee = false;
            RealSalary mRealSalary = new RealSalary();

            if (application_id != 0)
            {
                mRealSalary = this.ClientRealSalary(personal_id, application_id);
                try
                {
                    mMedian = mRealSalary.MEDIAN;
                    mIs_vip_or_employee = mRealSalary.IS_VIP_OR_EMPLOYEE;
                }
                catch
                {
                    mMedian = 0;
                    mIs_vip_or_employee = false;
                }
            }

            try
            {
                cityList = CityList();

                ODBAnswer _answer = _odb.GetClientInfoByPID(personal_id);
                _ds = _answer.AnswerDataSet;
                if (_ds != null && _ds.Tables != null && _ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
                {
                    mDataRow = _ds.Tables[0].Rows[0];
                }
            }
            catch
            {
                return RetList;
            }

            if (mDataRow != null)
            {
                foreach (DataRow _dr in _ds.Tables[0].Rows)
                {
                    _class.CLIENT_INFO_ID = 0;
                    _class.APPLICATION_ID = 0;
                    _class.CLIENT_NO = (DBNull.Value.Equals(_dr["CLIENT_NO"])) ? 0 : Convert.ToInt32(_dr["CLIENT_NO"]);
                    _class.IS_EMPLOYEE = Convert.ToBoolean(_dr["IS_EMPLOYEE"]);
                    _class.CLIENT_NAME = (DBNull.Value.Equals(_dr["FIRST_NAME"])) ? "" : Convert.ToString(_dr["FIRST_NAME"]);
                    _class.CLIENT_SURNAME = (DBNull.Value.Equals(_dr["LAST_NAME"])) ? "" : Convert.ToString(_dr["LAST_NAME"]);
                    _class.CLIENT_FATHER_NAME = (DBNull.Value.Equals(_dr["FATHERS_NAME"])) ? "" : Convert.ToString(_dr["FATHERS_NAME"]);
                    _class.CLIENT_CITIZENSHIP = (DBNull.Value.Equals(_dr["COUNTRY"])) ? "" : Convert.ToString(_dr["COUNTRY"]);
                    _class.CLIENT_PASSPORT_ID = (DBNull.Value.Equals(_dr["PASSPORT"])) ? "" : Convert.ToString(_dr["PASSPORT"]);
                    _class.CLIENT_PHONE_HOME = (DBNull.Value.Equals(_dr["PHONE1"])) ? "" : Convert.ToString(_dr["PHONE1"]);
                    _class.CLIENT_PHONE_MOB = (DBNull.Value.Equals(_dr["PHONE2"])) ? "" : Convert.ToString(_dr["PHONE2"]);
                    _class.CLEINT_LEGAL_ADDRESS = (DBNull.Value.Equals(_dr["ADDRESS_JUR"])) ? "" : Convert.ToString(_dr["ADDRESS_JUR"]);
                    _class.CLIENT_BIRHT_DATE = (DBNull.Value.Equals(_dr["BIRTH_DATE"])) ? DateTime.Now.Date : Convert.ToDateTime(_dr["BIRTH_DATE"]);
                    _class.CLIENT_AGE = (DBNull.Value.Equals(_dr["CLIENT_AGE"])) ? 0 : Convert.ToInt32(_dr["CLIENT_AGE"]);
                    _class.CLIENT_BIRHT_PLACE = (DBNull.Value.Equals(_dr["BIRTH_PLACE"])) ? "" : Convert.ToString(_dr["BIRTH_PLACE"]);
                    _class.PASSPORT_TYPE_ID = (DBNull.Value.Equals(_dr["PASSPORT_TYPE_ID"])) ? 0 : Convert.ToInt32(_dr["PASSPORT_TYPE_ID"]);
                    _class.CLIENT_PERSONAL_ID = (DBNull.Value.Equals(_dr["PERSONAL_ID"])) ? "" : Convert.ToString(_dr["PERSONAL_ID"]);
                    _class.CLIENT_GENDER = (DBNull.Value.Equals(_dr["MALE_FEMALE"])) ? 0 : Convert.ToInt32(_dr["MALE_FEMALE"]);
                    _class.CLIENT_ADDRESS = (DBNull.Value.Equals(_dr["ADDRESS_FACT"])) ? "" : Convert.ToString(_dr["ADDRESS_FACT"]);

                    _class.PASSPORT_COUNTRY = (DBNull.Value.Equals(_dr["PASSPORT_COUNTRY"])) ? "" : Convert.ToString(_dr["PASSPORT_COUNTRY"]);
                    _class.PASSPORT_ISSUE_DT = (DBNull.Value.Equals(_dr["PASSPORT_ISSUE_DT"])) ? DateTime.Now.Date : Convert.ToDateTime(_dr["PASSPORT_ISSUE_DT"]);
                    _class.PASSPORT_END_DATE = (DBNull.Value.Equals(_dr["PASSPORT_END_DATE"])) ? new DateTime(0001, 1, 1) : Convert.ToDateTime(_dr["PASSPORT_END_DATE"]);
                    if (_class.PASSPORT_END_DATE == new DateTime(0001, 1, 1))
                    {
                        _class.PASSPORT_IS_LIFE = true;
                    }
                    else
                    {
                        _class.PASSPORT_IS_LIFE = false;
                    }
                    _class.PASSPORT_REG_ORGAN = (DBNull.Value.Equals(_dr["PASSPORT_REG_ORGAN"])) ? "" : Convert.ToString(_dr["PASSPORT_REG_ORGAN"]);
                    _class.E_MAIL = (DBNull.Value.Equals(_dr["E_MAIL"])) ? "" : Convert.ToString(_dr["E_MAIL"]);

                    try
                    {
                        _class.CLIENT_CITY_ID = cityList.First(item => item.CITY_NAME == StringFunctions.ConvertToUnicode(_dr["CITY"].ToString())).CITY_ID;
                    }
                    catch
                    {
                        _class.CLIENT_CITY_ID = 0;
                    }
                    if (_class.IS_EMPLOYEE)
                    {
                        _class.CLIENT_ORGANIZATION_ID = Convert.ToInt32(_AppSets["VTBNumInOrgList"]);
                    }
                    else
                    {
                        _class.CLIENT_ORGANIZATION_ID = 0;
                    }
                    _class.CLIENT_ORGANIZATION_NAME = "";//(DBNull.Value.Equals(_dr["JOB_PLACE"])) ? "" : Convert.ToString(_dr["JOB_PLACE"]);
                    _class.CLIENT_WORK_EXPERIENCE = 0;
                    _class.CLIENT_WORK_EXPERIENCE_PERIOD = 12;
                    _class.CLIENT_MONTHLY_INCOME = (mIs_vip_or_employee) ? 0 : mMedian;
                    _class.REAL_SALARY = mMedian;
                    _class.CLIENT_MONTHLY_INCOME_CURRENCY = "GEL";
                    _class.CLIENT_OTH_MONTHLY_INCOME = 0;
                    _class.CLIENT_OTH_MONTHLY_INCOME_CURRENCY = "GEL";
                    _class.CLIENT_POSITION = (DBNull.Value.Equals(_dr["JOB_TITLE"])) ? "" : Convert.ToString(_dr["JOB_TITLE"]);
                    _class.CLIENT_TOTAL_WORK_EXPERIENCE = 0;
                    _class.CLIENT_TOTAL_WORK_EXPERIENCE_PERIOD = 12;
                    _class.CLIENT_INCOME_TYPE = 1;
                    _class.CLIENT_MARITAL_STATUS = (DBNull.Value.Equals(_dr["MARITAL_STATUS"])) ? 2 : Convert.ToInt32(_dr["MARITAL_STATUS"]); ;
                    _class.CLIENT_NUMBER_OF_CHILDREN = (DBNull.Value.Equals(_dr["NUMBER_OF_DEPEND_INCL_18"])) ? 0 : Convert.ToInt32(_dr["NUMBER_OF_DEPEND_INCL_18"]);
                    _class.CLIENT_SPOUSE_PLACE_OF_WORK = "";
                    _class.CLIENT_SPOUSE_INCOME = 0;
                    _class.CLIENT_SPOUSE_INCOME_CURRENCY = "GEL";
                    _class.CLIENT_SPOUSE_NAME = "";
                    _class.CLIENT_SPOUSE_SURNAME = "";
                    _class.CLIENT_TOTAL_FAMILY_MEMBERS = (DBNull.Value.Equals(_dr["FAM_NUMBER_MEMBERS"])) ? 0 : Convert.ToInt32(_dr["FAM_NUMBER_MEMBERS"]);
                    _class.CLIENT_SPOUSE_POSITION = "";
                    _class.CLIENT_SPOUSE_INCOME_TYPE = 1;
                    int rec_state = (DBNull.Value.Equals(_dr["REC_STATE"])) ? 2 : Convert.ToInt32(_dr["REC_STATE"]);
                    _class.IS_ACTIVE_IN_ODB = (rec_state == 1) ? true : false;
                    _class.IS_EMPLOYEE = Convert.ToBoolean(_dr["IS_EMPLOYEE"]);
                    _class.IS_INSIDER = Convert.ToBoolean(_dr["IS_INSIDER"]);
                    _class.CLIENT_CLASIFF = (DBNull.Value.Equals(_dr["CLASIFF"])) ? "" : Convert.ToString(_dr["CLASIFF"]);

                    _class.INFORMATION_SOURCE_ID = 0;

                    /*fatca*/
                    _class.NEW_CLIENT = 0;
                    _class.CITIZENSHIP = (DBNull.Value.Equals(_dr["CITIZENSHIP"])) ? "GE" : Convert.ToString(_dr["CITIZENSHIP"]);
                    _class.DOUBLE_CITIZENSHIP = (DBNull.Value.Equals(_dr["DOUBLE_CITIZENSHIP"])) ? "" : Convert.ToString(_dr["DOUBLE_CITIZENSHIP"]);
                    _class.COUNTRY_OF_BIRTH = (DBNull.Value.Equals(_dr["COUNTRY_OF_BIRTH"])) ? "GE" : Convert.ToString(_dr["COUNTRY_OF_BIRTH"]);
                    _class.RESIDENT_COUNTRY = (DBNull.Value.Equals(_dr["RESIDENT_COUNTRY"])) ? "" : Convert.ToString(_dr["RESIDENT_COUNTRY"]);
                    _class.FACT_COUNTRY = (DBNull.Value.Equals(_dr["FACT_COUNTRY"])) ? "GE" : Convert.ToString(_dr["FACT_COUNTRY"]);
                    _class.FACT_CITY = (DBNull.Value.Equals(_dr["FACT_CITY"])) ? "" : Convert.ToString(_dr["FACT_CITY"]);
                    if (_class.FACT_CITY == "")
                    {
                        _class.FACT_CITY = StringFunctions.ConvertToUnicode(_dr["CITY"].ToString());
                    }
                    _class.FACT_CITY_LAT = (DBNull.Value.Equals(_dr["FACT_CITY_LAT"])) ? "" : Convert.ToString(_dr["FACT_CITY_LAT"]);
                    /*end fatca*/

                    if (_class.CLIENT_NO > 0 && getSalary)
                    {

                        try
                        {
                            _class.ACCOUNT_PRODUCT_NO = this.AccountProductCheck(_class.CLIENT_NO);

                            string mSQL = String.Format("EXEC dbo.bank_salary_amounts @client_no = {0},@month = {1},@income_month = {2},@last_month = {3}", _class.CLIENT_NO, 6, 3, 3);
                            mDataRowSalary = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];
                            _class.CALC_INCOME = Convert.ToInt32(_dr["INCOME"]);
                            _class.CALC_SALARY = Convert.ToInt32(_dr["SALARY"]);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            if (_class.CLIENT_NO > 0)
            {
                goCRA = false;
            }

            if (goCRA)//სატესტოზე არ წავიდეს CRA-ში
            {
                goCRA = Convert.ToBoolean(_AppSets["goCRA"]);
            }

            try
            {
                PersonInfo _personInfo = new PersonInfo();

                if (goCRA)
                {
                    _personInfo = this.CraInfoGet(personal_id, odb_user_id, mBirthDate);
                }
                if (!String.IsNullOrEmpty(_personInfo.FirstName))
                {
                    if (_class.CLIENT_NO == 0)
                    {
                        _class.NEW_CLIENT = 1;
                    }
                    _class.CLIENT_NAME = _personInfo.FirstName;
                    _class.CLIENT_SURNAME = _personInfo.LastName;
                    if (_personInfo.MiddleName != "")
                    {
                        _class.CLIENT_FATHER_NAME = _personInfo.MiddleName;
                    }
                    _class.CLIENT_CITIZENSHIP = ContryCode2Get(_personInfo.CitizenshipCode);
                    _class.CITIZENSHIP = ContryCode2Get(_personInfo.CitizenshipCode);
                    if (_personInfo.DoubleCitizenshipCode.Length >= 2)
                    {
                        _class.DOUBLE_CITIZENSHIP = ContryCode2Get(_personInfo.DoubleCitizenshipCode);
                    }
                    _class.RESIDENT_COUNTRY = ContryCode2Get(_personInfo.CitizenshipCode);
                    _class.CLIENT_PASSPORT_ID = (_personInfo.IsIdCard == true) ? (_personInfo.IdCardSerial + _personInfo.IdCardNumber) : (_personInfo.IdCardSerial + _personInfo.PaspNumber);
                    _class.CLEINT_LEGAL_ADDRESS = _personInfo.LivingPlace;
                    _class.CLIENT_BIRHT_DATE = (DateTime)_personInfo.BirthDate;
                    _class.CLIENT_AGE = (int)(DateTime.Now.Subtract((DateTime)_personInfo.BirthDate).Days / 365.25);
                    if (_personInfo.BirthPlace != "")
                    {
                        _class.CLIENT_BIRHT_PLACE = _personInfo.BirthPlace;
                    }
                    _class.PASSPORT_TYPE_ID = (_personInfo.IsIdCard == true) ? 1 : 2;
                    _class.CLIENT_PERSONAL_ID = personal_id;//_personInfo.PrivateNumber;
                    _class.CLIENT_GENDER = 2 - (int)_personInfo.Gender;

                    try
                    {
                        _class.CLIENT_CITY_ID = cityList.First(item => item.CITY_NAME == _personInfo.BirthPlace.Replace("საქართველო, ", "")).CITY_ID;
                    }
                    catch
                    {
                        if (_class.CLIENT_NO == 0)
                        {
                            _class.CLIENT_CITY_ID = 0;
                        }
                    }

                    _class.PASSPORT_ISSUE_DT = (DateTime)_personInfo.IdCardIssueDate;

                    if (_personInfo.IdCardValidDate == null)
                    {
                        _class.PASSPORT_IS_LIFE = true;
                    }
                    else
                    {
                        try
                        {
                            _class.PASSPORT_END_DATE = (DateTime)_personInfo.IdCardValidDate;
                        }
                        catch { }
                        _class.PASSPORT_IS_LIFE = false;
                    }
                    _class.PASSPORT_REG_ORGAN = (_personInfo.IsIdCard) ? _personInfo.IdCardIssuer : _personInfo.PaspIssuer;
                }

                if (_class.CLIENT_NO == 0)
                {
                    _class.FACT_CITY = _personInfo.BirthPlace.Replace("საქართველო, ", "");
                    _class.CLIENT_ADDRESS = _personInfo.LivingPlace;

                    _class.CLIENT_INFO_ID = 0;
                    _class.APPLICATION_ID = 0;
                    _class.CLIENT_NO = 0;
                    _class.COUNTRY_OF_BIRTH = "GE";
                    _class.FACT_COUNTRY = "GE";
                    _class.FACT_CITY_LAT = "";
                    _class.NEW_CLIENT = 0;
                    _class.CLIENT_PHONE_HOME = "";
                    _class.CLIENT_PHONE_MOB = "";
                    _class.PASSPORT_COUNTRY = "GE";
                    _class.E_MAIL = "";
                    _class.CLIENT_ORGANIZATION_ID = 0;
                    _class.CLIENT_ORGANIZATION_NAME = "";
                    _class.CLIENT_WORK_EXPERIENCE = 0;
                    _class.CLIENT_WORK_EXPERIENCE_PERIOD = 12;
                    _class.CLIENT_MONTHLY_INCOME = 0;
                    _class.CLIENT_MONTHLY_INCOME_CURRENCY = "GEL";
                    _class.CLIENT_OTH_MONTHLY_INCOME = 0;
                    _class.CLIENT_OTH_MONTHLY_INCOME_CURRENCY = "GEL";
                    _class.CLIENT_POSITION = "";
                    _class.CLIENT_TOTAL_WORK_EXPERIENCE = 0;
                    _class.CLIENT_TOTAL_WORK_EXPERIENCE_PERIOD = 12;
                    _class.CLIENT_INCOME_TYPE = 1;
                    _class.CLIENT_MARITAL_STATUS = 0;
                    _class.CLIENT_NUMBER_OF_CHILDREN = 0;
                    _class.CLIENT_SPOUSE_PLACE_OF_WORK = "";
                    _class.CLIENT_SPOUSE_INCOME = 0;
                    _class.CLIENT_SPOUSE_INCOME_CURRENCY = "GEL";
                    _class.CLIENT_SPOUSE_NAME = "";
                    _class.CLIENT_SPOUSE_SURNAME = "";
                    _class.CLIENT_TOTAL_FAMILY_MEMBERS = 0;
                    _class.CLIENT_SPOUSE_POSITION = "";
                    _class.CLIENT_SPOUSE_INCOME_TYPE = 1;
                    _class.IS_ACTIVE_IN_ODB = false;
                    _class.IS_EMPLOYEE = false;
                    _class.INFORMATION_SOURCE_ID = 0;
                    _class.ACCOUNT_PRODUCT_NO = 0;

                }

            }
            catch { }

            RetList.Add(_class);
            return RetList;
        }

        #endregion ApplicationClientInfoLoadWithSalary

        [WebMethod]
        public List<ApplicationClientInfoEmployer> ApplicationClientInfoEmployerListLoad(int application_id, string personal_id)
        {
            UTFODBService _odb = new UTFODBService();
            _odb.Url = _AppSets["ODBServive.URL"];


            List<ApplicationClientInfoEmployer> mApplicationClientInfoEmployerList = new List<ApplicationClientInfoEmployer>();
            ApplicationClientInfoEmployer mApplicationClientInfoEmployer = new ApplicationClientInfoEmployer();
            ODBAnswer _answer = _odb.GetClientInfoEmployer(personal_id);
            DataSet _ds = _answer.AnswerDataSet;


            int c = _ds.Tables[0].Rows.Count;

            for (int i = 0; i < c; i++)
            {
                DataRow mRow = _ds.Tables[0].Rows[i];
                if (i == 0)
                {
                    mApplicationClientInfoEmployer.IS_MAIN = true;
                }
                else
                {
                    mApplicationClientInfoEmployer.IS_MAIN = false;
                }

                int mCLIENT_WORK_POSITION_TYPE_ID = Convert.ToInt32(mRow["CLIENT_WORK_POSITION_TYPE_ID"]);
                if (mCLIENT_WORK_POSITION_TYPE_ID == 1)
                {
                    mCLIENT_WORK_POSITION_TYPE_ID = 2;
                }
                else
                {
                    mCLIENT_WORK_POSITION_TYPE_ID = 1;
                }

                mApplicationClientInfoEmployer.APPLICATION_ID = application_id;
                mApplicationClientInfoEmployer.CLIENT_WORK_POSITION_DESCRIP = mRow["CLIENT_WORK_POSITION_DESCRIP"].ToString();
                mApplicationClientInfoEmployer.CLIENT_WORK_POSITION_TYPE_ID = Convert.ToInt32(mRow["CLIENT_WORK_POSITION_TYPE_ID"]);
                mApplicationClientInfoEmployer.CLIENT_WORK_PROFESSION_ID = Convert.ToInt32(mRow["CLIENT_WORK_PROFESSION_ID"]);
                mApplicationClientInfoEmployer.COMMENT = mRow["COMMENT"].ToString();
                mApplicationClientInfoEmployer.COMPANY_DESCRIP = mRow["COMPANY_DESCRIP"].ToString();
                mApplicationClientInfoEmployer.COMPANY_ID = mRow["COMPANY_ID"].ToString();
                mApplicationClientInfoEmployer.ISO = mRow["ISO"].ToString();
                mApplicationClientInfoEmployer.SALERY = Convert.ToDecimal(mRow["SALERY"]);
                mApplicationClientInfoEmployer.UNEMPLOYED = Convert.ToBoolean(mRow["UNEMPLOYED"]);
                mApplicationClientInfoEmployer.CLIENT_INCOME_TYPE_ID = 1;
                if (c == 1)
                {
                    mApplicationClientInfoEmployer.IS_MAIN = true;
                }

                mApplicationClientInfoEmployerList.Add(mApplicationClientInfoEmployer);
                mApplicationClientInfoEmployer.OTHER_SALERY = 0;
                mApplicationClientInfoEmployer.OTHER_SALERY_ISO = "GEL";
                mApplicationClientInfoEmployer.OTHER_SALERY_TYPE_ID = 1;
            }

            //this.ApplicationClientInfoEmployerDel(application_id);
            //this.ApplicationClientInfoEmployerListSet(application_id, mApplicationClientInfoEmployerList);

            return mApplicationClientInfoEmployerList;
        }

        #endregion


        [WebMethod]
        public bool test2()
        {
            return ApplicationClientInfoSave(this.ApplicationClientInfoList(0,1577280).First(),27);
        }


        #region ApplicationClientInfoSave
        [WebMethod]
        public bool ApplicationClientInfoSave(ApplicationClientInfo _class, int user_id)
        {
            bool retValue = false;

            try
            {

                if (_class == null)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, user_id, "ApplicationClientInfoSave: _class is null");
                }

                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_INFO_SET @client_info_id = {0},@application_id = {1},@client_no = {2},@client_name = {3},@client_surname = {4}
                ,@client_citizenship = {5},@client_passport_id = {6},@client_phone_home = {7},@client_phone_mob = {8},@cleint_legal_address = {9},@client_birht_date = {10},@client_age = {11}
                ,@passport_type_id = {12},@client_personal_id = {13},@client_gender = {14},@client_address = {15},@client_organization_id = {16},@client_work_experience = {17}
                ,@client_work_experience_period = {18},@client_monthly_income = {19},@client_monthly_income_currency = {20},@client_oth_monthly_income = {21},@client_oth_monthly_income_currency = {22}
                ,@client_position = {23},@client_total_work_experience = {24},@client_total_work_experience_period = {25},@client_income_type = {26},@client_marital_status = {27}
                ,@client_number_of_children = {28},@client_spouse_place_of_work= {29},@client_spouse_income = {30},@client_spouse_income_currency = {31},@client_spouse_name = {32}
                ,@client_spouse_surname = {33},@client_total_family_members = {34},@client_spouse_position = {35},@client_spouse_income_type = {36},@client_father_name = {37},@client_oth_income_type = {38}
                ,@client_organization_category = {39},@client_salary_category = {40},@client_rank_id={41},@client_city_id = {42},@client_transfers_period_month = {43}
                ,@client_total_family_income = {44},@client_total_family_income_currency = {45},@user_id = {46}
                ,@passport_country = {47}	,@passport_issue_dt ={48},@passport_end_date = {49},@passport_is_life = {50},@passport_reg_organ = {51}	,@information_source_id ={52}
                ,@e_mail = {53}, @client_birht_place = {54}, @client_organization_phone = {55},@client_sub_type_id = {56},@client_spouse_personal_id = {57}
                ,@current_negative_statuses = {58},@terminated_negative_statuses = {59},@positive_statuses = {60},@risk_grade = {61},@calc_income = {62},@calc_salary = {63}
                ,@max_positive_amount = {64}, @organization_name = {65}, @new_client = {66}, @citizenship = {67}, @double_citizenship = {68}, @country_of_birth = {69}, @resident_country = {70}
                ,@fact_country = {71}, @fact_city = {72}, @fact_city_lat = {73},@real_salary = {74},@seb_class_id = {75}, @risk_val_rate = {76}, @is_insider = {77}, @client_clasiff = {78}
                ,@current_calc_negative = {79},@insurance_company_id = {80},@payment_insurance_guid = {81},@conversational_language_id = {82},@client_income_approved = {83}
                ,@sp_head_name = {84}, @sp_head_phone = {85}, @sp_head_position = {86}, @sp_emploee_count = {87}
                ,@education_id = {88}, @post_category_id = {89}, @activity_area_id = {90}, @emploee_count_id = {91}"
            , _class.CLIENT_INFO_ID
            , _class.APPLICATION_ID
            , _class.CLIENT_NO
            , StringFunctions.SqlQuoted(_class.CLIENT_NAME, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_SURNAME, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_CITIZENSHIP, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_PASSPORT_ID, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_PHONE_HOME, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_PHONE_MOB, true)
            , StringFunctions.SqlQuoted(_class.CLEINT_LEGAL_ADDRESS, true)
            , StringFunctions.SqlDateQuoted(_class.CLIENT_BIRHT_DATE)
            , _class.CLIENT_AGE
            , _class.PASSPORT_TYPE_ID
            , StringFunctions.SqlQuoted(_class.CLIENT_PERSONAL_ID, true)
            , _class.CLIENT_GENDER
            , StringFunctions.SqlQuoted(_class.CLIENT_ADDRESS, true)
            , _class.CLIENT_ORGANIZATION_ID
            , _class.CLIENT_WORK_EXPERIENCE
            , _class.CLIENT_WORK_EXPERIENCE_PERIOD
            , _class.CLIENT_MONTHLY_INCOME
            , StringFunctions.SqlQuoted(_class.CLIENT_MONTHLY_INCOME_CURRENCY, true)
            , _class.CLIENT_OTH_MONTHLY_INCOME
            , StringFunctions.SqlQuoted(_class.CLIENT_OTH_MONTHLY_INCOME_CURRENCY, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_POSITION, true)
            , _class.CLIENT_TOTAL_WORK_EXPERIENCE
            , _class.CLIENT_TOTAL_WORK_EXPERIENCE_PERIOD
            , _class.CLIENT_INCOME_TYPE
            , _class.CLIENT_MARITAL_STATUS
            , _class.CLIENT_NUMBER_OF_CHILDREN
            , StringFunctions.SqlQuoted(_class.CLIENT_SPOUSE_PLACE_OF_WORK, true)
            , _class.CLIENT_SPOUSE_INCOME
            , StringFunctions.SqlQuoted(_class.CLIENT_SPOUSE_INCOME_CURRENCY, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_SPOUSE_NAME, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_SPOUSE_SURNAME, true)
            , _class.CLIENT_TOTAL_FAMILY_MEMBERS
            , StringFunctions.SqlQuoted(_class.CLIENT_SPOUSE_POSITION, true)
            , _class.CLIENT_SPOUSE_INCOME_TYPE
            , StringFunctions.SqlQuoted(_class.CLIENT_FATHER_NAME, true)
            , _class.CLIENT_OTH_INCOME_TYPE
            , StringFunctions.SqlQuoted(_class.CLIENT_ORGANIZATION_CATEGORY, true)
            , _class.CLIENT_SALARY_CATEGORY
            , _class.CLIENT_RANK_ID
            , _class.CLIENT_CITY_ID
            , _class.CLIENT_TRANSFERS_PERIOD_MONTH
            , _class.CLIENT_TOTAL_FAMILY_INCOME
            , StringFunctions.SqlQuoted(_class.CLIENT_TOTAL_FAMILY_INCOME_CURRENCY, true)
            , user_id

            , StringFunctions.SqlQuoted(_class.PASSPORT_COUNTRY, true)
            , StringFunctions.SqlDateQuoted(_class.PASSPORT_ISSUE_DT)
            , StringFunctions.SqlDateQuoted(_class.PASSPORT_END_DATE)
            , (_class.PASSPORT_IS_LIFE) ? 1 : 0
            , StringFunctions.SqlQuoted(_class.PASSPORT_REG_ORGAN, true)
            , _class.INFORMATION_SOURCE_ID
            , StringFunctions.SqlQuoted(_class.E_MAIL, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_BIRHT_PLACE, true)
            , StringFunctions.SqlQuoted(_class.CLIENT_ORGANIZATION_PHONE, true)
            , _class.CLIENT_SUB_TYPE_ID
            , StringFunctions.SqlQuoted(_class.CLIENT_SPOUSE_PERSONAL_ID, true)
            , _class.CURRENT_NEGATIVE_STATUSES
            , _class.TERMINATED_NEGATIVE_STATUSES
            , _class.POSITIVE_STATUSES
            , StringFunctions.SqlQuoted(_class.RISK_GRADE, true)
            , _class.CALC_INCOME
            , _class.CALC_SALARY
            , _class.MAX_POSITIVE_AMOUNT
            , StringFunctions.SqlQuoted(_class.ORGANIZATION_NAME, true)
            , _class.NEW_CLIENT
            , StringFunctions.SqlQuoted(_class.CITIZENSHIP, true)
            , StringFunctions.SqlQuoted(_class.DOUBLE_CITIZENSHIP, true)
            , StringFunctions.SqlQuoted(_class.COUNTRY_OF_BIRTH, true)
            , StringFunctions.SqlQuoted(_class.RESIDENT_COUNTRY, true)
            , StringFunctions.SqlQuoted(_class.FACT_COUNTRY, true)
            , StringFunctions.SqlQuoted(_class.FACT_CITY, true)
            , StringFunctions.SqlQuoted(_class.FACT_CITY_LAT, true)
            , _class.REAL_SALARY
            , _class.SEB_CLASS_ID
            , _class.RISK_VAL_RATE
            , (_class.IS_INSIDER) ? 1 : 0
            , StringFunctions.SqlQuoted(_class.CLIENT_CLASIFF, true)
            , _class.CURRENT_CALC_NEGATIVE
            , _class.INSURANCE_COMPANY_ID
            , StringFunctions.SqlQuoted(_class.PAYMENT_INSURANCE_GUID, true)
            , _class.CONVERS_LANG_ID
            , _class.CLIENT_INCOME_APPROVED
            , StringFunctions.SqlQuoted(_class.SP_HEAD_NAME, true)
            , StringFunctions.SqlQuoted(_class.SP_HEAD_PHONE, true)
            , StringFunctions.SqlQuoted(_class.SP_HEAD_POSITION, true)
            , _class.SP_EMPLOEE_COUNT

            , _class.EDUCATION_ID
            , _class.POST_CATEGORY_ID
            , _class.ACTIVITY_AREA_ID
            , _class.EMPLOEE_COUNT_ID
            );


                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch (Exception Ex)
            {
                try
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, user_id, "ApplicationClientInfoSave:" + Ex.Message);
                }
                catch { }
                var json = new JavaScriptSerializer().Serialize(_class);
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, user_id, "ApplicationClientInfoSave:" + json);
            }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationComment ==
        #region ApplicationCommentDelete
        [WebMethod]

        public bool ApplicationCommentDelete(int application_id, int rec_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_COMMENT_DEL @application_id = {0},@rec_id = {1}", application_id, rec_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationCommentList
        [WebMethod]

        public List<ApplicationComment> ApplicationCommentList(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_COMMENT_GET @application_id = {0}", ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationComment> RetList = new List<ApplicationComment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationComment mClass = new ApplicationComment();
                mClass = (ApplicationComment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<ApplicationComment> ApplicationCommentForRestruct(int loanId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_COMMENT_FOR_RESTRUCT @loan_id = {0}", loanId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationComment> RetList = new List<ApplicationComment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationComment mClass = new ApplicationComment();
                mClass = (ApplicationComment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationCommentSave
        [WebMethod]

        public int ApplicationCommentSave(int application_id, int user_id, string comment)
        {
            int retValue = 0;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_COMMENT_SET @application_id = {0}, @user_id = {1}, @comment = {2}"
            , application_id
            , user_id
            , StringFunctions.SqlQuoted(comment, true)
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["REC_ID"]);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #region ApplicationCommentListSave
        [WebMethod]

        public bool ApplicationCommentListSave(List<ApplicationComment> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationCommentDelete(application_id, 0);
                foreach (ApplicationComment _class in _classList)
                {
                    ApplicationCommentSave(_class.APPLICATION_ID, _class.COMMENT_USER_ID, _class.COMMENT);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationInsurance ==
        #region ApplicationInsuranceDelete
        [WebMethod]

        public bool ApplicationInsuranceDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_INSURANCES_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationInsuranceList
        [WebMethod]

        public List<ApplicationInsurance> ApplicationInsuranceList(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_INSURANCES_LIST @application_id = {0}", ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationInsurance> RetList = new List<ApplicationInsurance>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationInsurance mClass = new ApplicationInsurance();
                mClass = (ApplicationInsurance)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationInsuranceSave
        private bool ApplicationInsuranceSave(int application_id, int insurance_company_id, int insurance_product_id, string indurance_property_id, int insurance_type_id, decimal insurance_rate, decimal insurance_amount)
        {
            bool retValue = false;
            string mSQL = String.Format(@"pAPPLICATION_INSURANCES_SET @application_id = {0},@insurance_company_id = {1},@insurance_product_id = {2},@insurance_property_id = {3},@insurance_type_id = {4},@insurance_rate = {5},@insurance_amount = {6}"
            , application_id
            , insurance_company_id
            , insurance_product_id
            , StringFunctions.SqlQuoted(indurance_property_id, true)
            , insurance_type_id
            , insurance_rate
            , insurance_amount
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) > 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #region ApplicationInsuranceListSave
        [WebMethod]

        public bool ApplicationInsuranceListSave(List<ApplicationInsurance> _classList, int application_id)
        {
            bool retValue = true;
            try
            {
                this.ApplicationInsuranceDelete(application_id);
                foreach (ApplicationInsurance _class in _classList)
                {
                    this.ApplicationInsuranceSave(_class.APPLICATION_ID, _class.INSURANCE_COMPANY_ID, _class.INSURANCE_PRODUCT_ID, _class.INSURANCE_PROPERTY_ID, _class.INSURANCE_TYPE_ID, _class.INSURANCE_RATE, _class.INSURANCE_AMOUNT);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationAdditionalAttribute ==
        #region ApplicationAdditionalAttributeDelete
        [WebMethod]

        public bool ApplicationAdditionalAttributeDelete(int application_id, int rec_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_ADDITIONAL_ATTRIBUTES_DEL @application_id = {0},@rec_id = {1}", application_id, rec_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationAdditionalAttributeList
        [WebMethod]

        public List<ApplicationAdditionalAttribute> ApplicationAdditionalAttributeList(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADDITIONAL_ATTRIBUTES_LIST @application_id = {0}", ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationAdditionalAttribute> RetList = new List<ApplicationAdditionalAttribute>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationAdditionalAttribute mClass = new ApplicationAdditionalAttribute();
                mClass = (ApplicationAdditionalAttribute)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationAdditionalAttributeSave
        [WebMethod]

        public int ApplicationAdditionalAttributeSave(int application_id, int user_id, int attribute_id, string attribute_value)
        {
            int retValue = 0;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_ADDITIONAL_ATTRIBUTES_SET @application_id = {0}, @user_id = {1}, @attribute_id = {2},@attribute_value = {3}"
            , application_id
            , user_id
            , attribute_id
            , StringFunctions.SqlQuoted(attribute_value, true)
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["REC_ID"]);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationAdditionalAttributeListSave
        [WebMethod]

        public bool ApplicationAdditionalAttributeListSave(List<ApplicationAdditionalAttribute> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationAdditionalAttributeDelete(application_id, 0);
                foreach (ApplicationAdditionalAttribute _class in _classList)
                {
                    ApplicationAdditionalAttributeSave(_class.APPLICATION_ID, user_id, _class.ATTRIBUTE_ID, _class.ATTRIBUTE_VALUE);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region ApplicationApprovalStatus
        [WebMethod]
        public List<ApplicationApprovalStatus> ApplicationApprovalStatusList(string mApplicationIds)
        {
            string mSQL = String.Format("EXEC pAPPLICATION_APPROVAL_STATUS_LIST @ids = {0}", StringFunctions.SqlQuoted(mApplicationIds, true));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationApprovalStatus> RetList = new List<ApplicationApprovalStatus>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationApprovalStatus mClass = new ApplicationApprovalStatus();
                mClass = (ApplicationApprovalStatus)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationApprovalStatus

        #region ApplicationClientInfoFatca
        #region ApplicationClientInfoFatcaGet
        [WebMethod]
        public ApplicationClientInfoFatca ApplicationClientInfoFatcaGet(int mApplicationID)
        {
            ApplicationClientInfoFatca mClass = new ApplicationClientInfoFatca();
            try
            {
                string mSQL = String.Format("EXEC pAPPLICATION_CLIENT_INFO_FATCA_GET @application_id = {0}", mApplicationID);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                mClass = (ApplicationClientInfoFatca)FillClass(mClass, mDataTable.Rows[0]);
            }
            catch { }
            return mClass;
        }
        #endregion ApplicationClientInfoFatcaGet
        #region ApplicationClientInfoFatcaSet
        [WebMethod]
        public bool ApplicationClientInfoFatcaSet(ApplicationClientInfoFatca _class)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC pAPPLICATION_CLIENT_INFO_FATCA_SET @application_id = {0},@has_green_card = {1},@long_term_residence = {2}
                ,@canceled_us_citizenship = {3},@us_post_box = {4},@us_phone_or_fax = {5},@us_tax_residence = {6},@last_update = {7},@country_of_tax_residence = {8},@us_residence = {9}"
            , _class.APPLICATION_ID
            , _class.HAS_GREEN_CARD
            , _class.LONG_TERM_RESIDENCE
            , _class.CANCELED_US_CITIZENSHIP
            , _class.US_POST_BOX
            , _class.US_PHONE_OR_FAX
            , _class.US_TAX_RESIDENCE
            , StringFunctions.SqlDateQuoted(_class.LAST_UPDATE)
            , StringFunctions.SqlQuoted(_class.COUNTRY_OF_TAX_RESIDENCE, true)
            , _class.US_RESIDENCE
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion ApplicationClientInfoFatcaSet
        #endregion ApplicationClientInfoFatca

        #region == ApplicationClientInfoEmployer
        #region ApplicationClientInfoEmployerList
        [WebMethod]
        public List<ApplicationClientInfoEmployer> ApplicationClientInfoEmployerList(int mApplicationID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_INFO_EMPLOYER_LIST @application_id = {0}", mApplicationID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientInfoEmployer> RetList = new List<ApplicationClientInfoEmployer>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientInfoEmployer mClass = new ApplicationClientInfoEmployer();
                mClass = (ApplicationClientInfoEmployer)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationClientInfoEmployerList

        #region ApplicationClientInfoEmployerSet
        [WebMethod]
        public bool ApplicationClientInfoEmployerSet(ApplicationClientInfoEmployer _class)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC pAPPLICATION_CLIENT_INFO_EMPLOYER_SET @application_id = {0},@company_id = {1},@company_descrip = {2}
                ,@client_work_position_type_id = {3},@client_work_profession_id = {4},@client_work_position_descrip = {5},@salery = {6},@iso = {7}
                ,@comment = {8},@unemployed = {9},@company_phone = {10},@client_income_type_id = {11},@is_main = {12}
                ,@other_salery = {13},@other_salery_iso = {14},@other_salery_type_id = {15}"
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.COMPANY_ID, true)
            , StringFunctions.SqlQuoted(_class.COMPANY_DESCRIP, true)
            , _class.CLIENT_WORK_POSITION_TYPE_ID
            , _class.CLIENT_WORK_PROFESSION_ID
            , StringFunctions.SqlQuoted(_class.CLIENT_WORK_POSITION_DESCRIP, true)
            , _class.SALERY
            , StringFunctions.SqlQuoted(_class.ISO, true)
            , StringFunctions.SqlQuoted(_class.COMMENT, true)
            , _class.UNEMPLOYED
            , StringFunctions.SqlQuoted(_class.COMPANY_PHONE, true)
            , _class.CLIENT_INCOME_TYPE_ID
            , _class.IS_MAIN
            , _class.OTHER_SALERY
            , StringFunctions.SqlQuoted(_class.OTHER_SALERY_ISO, true)
            , _class.OTHER_SALERY_TYPE_ID
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion ApplicationClientInfoEmployerSet

        #region ApplicationClientInfoEmployerDel
        [WebMethod]
        public void ApplicationClientInfoEmployerDel(int mApplicationID)
        {
            try
            {
                string mSQL = String.Format("EXEC pAPPLICATION_CLIENT_INFO_EMPLOYER_DEL @application_id = {0}", mApplicationID);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            }
            catch { }
        }
        #endregion ApplicationClientInfoEmployerDel

        #region ApplicationClientInfoEmployerListSet
        [WebMethod]
        public void ApplicationClientInfoEmployerListSet(int application_id, List<ApplicationClientInfoEmployer> applicationClientInfoEmployerList)
        {
            if (application_id == 0 && applicationClientInfoEmployerList.Count > 0)
            {
                application_id = applicationClientInfoEmployerList[0].APPLICATION_ID;
            }
            this.ApplicationClientInfoEmployerDel(application_id);

            foreach (ApplicationClientInfoEmployer mApplicationClientInfoEmployer in applicationClientInfoEmployerList)
            {
                this.ApplicationClientInfoEmployerSet(mApplicationClientInfoEmployer);
            }
        }
        #endregion ApplicationClientInfoEmployerListSet

        #endregion == ApplicationClientInfoEmployer

        #region ClientAccounts
        [WebMethod]

        public List<ClientAccounts> ClientAccountsList(int client_no)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_ACCOUNTS @client_no = {0}", client_no);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientAccounts> RetList = new List<ClientAccounts>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientAccounts mClass = new ClientAccounts();
                mClass = (ClientAccounts)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ClientAccounts

        #region ApplicationClientAccount
        [WebMethod]

        public List<ApplicationClientAccount> ApplicationClientAccountList(int client_no)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_ACCOUNTS @client_no = {0}, @acc_types = '100,200'", client_no);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientAccount> RetList = new List<ApplicationClientAccount>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientAccount mClass = new ApplicationClientAccount();
                mClass = (ApplicationClientAccount)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationClientAccount

        #region ClientOverdueLoans
        [WebMethod]
        public int ClientOverdueLoans(string personal_id)
        {
            int ClientOverdueLoanCount = 0;
            string mSQL = String.Format("EXEC dbo.pCLIENT_OVERDUE_LOANS @personal_id = {0}", personal_id);
            try
            {
                ClientOverdueLoanCount = Convert.ToInt32(_DGate.GetDataSet(mSQL).Tables[0].Rows[0]["OVERDUE_LOANS"]);
            }
            catch
            {
                ClientOverdueLoanCount = -1;
            }
            return ClientOverdueLoanCount;
        }
        #endregion

        #region ApplicationControlList
        [WebMethod]

        public List<ApplicationControl> ApplicationControlList(int product_id)
        {
            string mSQL = String.Format("EXEC dbo.pCONTROL_LIST @product_id = {0}", product_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationControl> RetList = new List<ApplicationControl>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationControl mClass = new ApplicationControl();
                mClass = (ApplicationControl)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationPageList
        [WebMethod]

        public List<ApplicationPage> ApplicationPageList(int product_id)
        {
            string mSQL = String.Format("EXEC dbo.pPAGE_LIST @product_id = {0}", product_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationPage> RetList = new List<ApplicationPage>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationPage mClass = new ApplicationPage();
                mClass = (ApplicationPage)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region == ApplicationCreditInfo ==
        #region ApplicationCreditInfoDelete
        [WebMethod]

        public bool ApplicationCreditInfoDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CREDIT_INFO_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationCreditInfoList
        [WebMethod]

        public List<ApplicationCreditInfo> ApplicationCreditInfoList(int mApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CREDIT_INFO_GET @application_id = {0}", mApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationCreditInfo> RetList = new List<ApplicationCreditInfo>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationCreditInfo mClass = new ApplicationCreditInfo();
                mClass = (ApplicationCreditInfo)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationCreditInfoSave
        [WebMethod]

        public bool ApplicationCreditInfoSave(ApplicationCreditInfo _class)
        {
            bool retValue = false;
            string mSQL = String.Format(@"exec dbo.pAPPLICATION_CREDIT_INFO_SET @application_id = {0}, @credit_info_category_id = {1}, @credit_info_negative_count = {2}, 
                                                                                @credit_info_confirmation_id = {3}, @client_type = {4}, @non_qualif_worker = {5}"
            , _class.APPLICATION_ID
            , _class.CREDIT_INFO_CATEGORY_ID
            , _class.CREDIT_INFO_NEGATIVE_COUNT
            , _class.CREDIT_INFO_CONFIRMATION_ID
            , _class.CLIENT_TYPE
            , _class.NON_QUALIF_WORKER
            );

            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region === ClientLoans and Overdrafts ===

        #region == ApplicationCurrentLoans ==

        #region ApplicationCurrentLoansDelete
        [WebMethod]

        public bool ApplicationCurrentLoansDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CURRENT_LOANS_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationCurrentLoansList
        [WebMethod]
        public List<ApplicationCurrentLoans> ApplicationCurrentLoansList(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CURRENT_LOANS_GET @application_id = {0}", ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationCurrentLoans> RetList = new List<ApplicationCurrentLoans>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationCurrentLoans mClass = new ApplicationCurrentLoans();
                mClass = (ApplicationCurrentLoans)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationCurrentLoansListByClientNo
        [WebMethod]
        public List<ApplicationCurrentLoans> ApplicationCurrentLoansListByClientNo(int ApplicationId, int ClientNo)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CURRENT_LOANS_FROM_LMS_BY_CLIENT_NO @application_id = {0}, @client_no = {1}", ApplicationId, ClientNo);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationCurrentLoans> RetList = new List<ApplicationCurrentLoans>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationCurrentLoans mClass = new ApplicationCurrentLoans();
                mClass = (ApplicationCurrentLoans)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationCurrentLoansListByClientNo

        #region ApplicationCurrentLoansListFromLoans
        [WebMethod]

        public List<ApplicationCurrentLoans> ApplicationCurrentLoansListFromLoans(int ApplicationId, int ClientNo)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CURRENT_LOANS_FROM_LMS @application_id = {0},@client_no = {1}", ApplicationId, ClientNo);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationCurrentLoans> RetList = new List<ApplicationCurrentLoans>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationCurrentLoans mClass = new ApplicationCurrentLoans();
                mClass = (ApplicationCurrentLoans)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationCurrentLoansListSave
        [WebMethod]

        public bool ApplicationCurrentLoansListSave(List<ApplicationCurrentLoans> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationCurrentLoansDelete(application_id);
                foreach (ApplicationCurrentLoans _class in _classList)
                {
                    ApplicationCurrentLoansSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationCurrentLoansSave
        [WebMethod]

        public bool ApplicationCurrentLoansSave(ApplicationCurrentLoans _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CURRENT_LOANS_SET 
                  @current_loan_id = {0}, @application_id = {1}, @current_loan_bank_code = {2}, @current_loan_amount = {3}
                , @current_loan_currency = {4}, @current_loan_start_date = {5}, @current_loan_end_date = {6}, @current_loan_months = {7}
                , @current_loan_interest_rate = {8}, @current_loan_collateral_type_id = {9}, @current_loan_current_debt = {10}, @current_loan_overdue_debt = {11}
                , @current_loan_pmt = {12}, @current_loan_cover = {13}, @loan_id = {14}, @client_no = {15}
                , @loan_info_source_id = {16}, @total_debt = {17}, @is_restruct = {18}, @is_problem = {19}, @is_writeoff = {20}
                , @virtual_product_id = {21}, @virtual_period = {22}, @virtual_percent = {23}, @virtual_pmt = {24}, @user_id = {25}"
                , _class.CURRENT_LOAN_ID
                , _class.APPLICATION_ID
                , StringFunctions.SqlQuoted(_class.CURRENT_LOAN_BANK_CODE, true)
                , _class.CURRENT_LOAN_AMOUNT
                , StringFunctions.SqlQuoted(_class.CURRENT_LOAN_CURRENCY, true)
                , StringFunctions.SqlDateQuoted(_class.CURRENT_LOAN_START_DATE)
                , StringFunctions.SqlDateQuoted(_class.CURRENT_LOAN_END_DATE)
                , _class.CURRENT_LOAN_MONTHS
                , _class.CURRENT_LOAN_INTEREST_RATE
                , _class.CURRENT_LOAN_COLLATERAL_TYPE_ID
                , _class.CURRENT_LOAN_CURRENT_DEBT
                , _class.CURRENT_LOAN_OVERDUE_DEBT
                , _class.CURRENT_LOAN_PMT
                , _class.CURRENT_LOAN_COVER
                , _class.LOAN_ID
                , _class.CLIENT_NO
                , _class.LOAN_INFO_SOURCE_ID
                , _class.TOTAL_DEBT
                , _class.IS_RESTRUCT
                , _class.IS_PROBLEM
                , _class.IS_WRITEOFF
                , _class.VIRTUAL_LOAN_TYPE
                , _class.VIRTUAL_PERIOD
                , _class.VIRTUAL_PERCENT
                , _class.VIRTUAL_PMT
                , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #endregion

        #region == ApplicationCurrentOverdrafts ==
        #region ApplicationCurrentOverdraftsDelete
        [WebMethod]

        public bool ApplicationCurrentOverdraftsDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CURRENT_OVERDRAFTS_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationCurrentOverdraftsList
        [WebMethod]
        public List<ApplicationCurrentOverdrafts> ApplicationCurrentOverdraftsList(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CURRENT_OVERDRAFTS_GET @application_id = {0}", ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationCurrentOverdrafts> RetList = new List<ApplicationCurrentOverdrafts>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationCurrentOverdrafts mClass = new ApplicationCurrentOverdrafts();
                mClass = (ApplicationCurrentOverdrafts)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationCurrentOverdraftsList

        #region ApplicationCurrentOverdraftsListByClientNo
        [WebMethod]
        public List<ApplicationCurrentOverdrafts> ApplicationCurrentOverdraftsListByClientNo(int ApplicationId, int ClientNo)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CURRENT_OVERDRAFTS_FROM_ODB_BY_CLIENT_NO @application_id = {0} ,@client_no = {1}", ApplicationId, ClientNo);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationCurrentOverdrafts> RetList = new List<ApplicationCurrentOverdrafts>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationCurrentOverdrafts mClass = new ApplicationCurrentOverdrafts();
                mClass = (ApplicationCurrentOverdrafts)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationCurrentOverdraftsListByClientNo

        #region ApplicationCurrentOverdraftsListFromLoans
        [WebMethod]

        public List<ApplicationCurrentOverdrafts> ApplicationCurrentOverdraftsListFromLoans(int ApplicationId, int ClientNo)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CURRENT_OVERDRAFTS_FROM_ODB @application_id = {0},@client_no = {1}", ApplicationId, ClientNo);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationCurrentOverdrafts> RetList = new List<ApplicationCurrentOverdrafts>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationCurrentOverdrafts mClass = new ApplicationCurrentOverdrafts();
                mClass = (ApplicationCurrentOverdrafts)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationCurrentOverdraftsListSave
        [WebMethod]

        public bool ApplicationCurrentOverdraftsListSave(List<ApplicationCurrentOverdrafts> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationCurrentOverdraftsDelete(application_id);
                foreach (ApplicationCurrentOverdrafts _class in _classList)
                {
                    ApplicationCurrentOverdraftsSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationCurrentOverdraftsSave

        [WebMethod]
        public bool ApplicationCurrentOverdraftsSave(ApplicationCurrentOverdrafts _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CURRENT_OVERDRAFTS_SET 
                  @current_overdraft_id = {0}, @application_id = {1}, @current_credit_bank_code = {2}
                , @current_credit_card_limit = {3}, @current_credit_currency = {4}, @current_credit_interest_rate = {5}
                , @current_credit_cover = {6}, @client_no = {7}, @is_credit_card = {8}
                , @loan_id = {9}, @loan_info_source_id = {10}, @current_credit_overdue_debt = {11}
                , @total_debt = {12}, @is_restruct = {13}, @is_problem = {14}, @is_writeoff = {15}, @user_id = {16}"
            , _class.CURRENT_OVERDRAFT_ID
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.CURRENT_CREDIT_BANK_CODE, true)
            , _class.CURRENT_CREDIT_CARD_LIMIT
            , StringFunctions.SqlQuoted(_class.CURRENT_CREDIT_CURRENCY, true)
            , _class.CURRENT_CREDIT_INTEREST_RATE
            , _class.CURRENT_CREDIT_COVER
            , _class.CLIENT_NO
            , _class.IS_CREDIT_CARD
            , _class.LOAN_ID
            , _class.LOAN_INFO_SOURCE_ID
            , _class.CURRENT_CREDIT_OVERDUE_DEBT
            , _class.TOTAL_DEBT
            , _class.IS_RESTRUCT
            , _class.IS_PROBLEM
            , _class.IS_WRITEOFF
            , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationClientIsGuarantor ==
        #region ApplicationClientIsGuarantorDelete
        [WebMethod]
        public bool ApplicationClientIsGuarantorDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_IS_GUARANTOR_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationClientIsGuarantorList
        [WebMethod]
        public List<ApplicationClientIsGuarantor> ApplicationClientIsGuarantorList(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_IS_GUARANTOR_GET @application_id = {0}", ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientIsGuarantor> RetList = new List<ApplicationClientIsGuarantor>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientIsGuarantor mClass = new ApplicationClientIsGuarantor();
                mClass = (ApplicationClientIsGuarantor)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationClientIsGuarantorListByClientNo
        [WebMethod]

        public List<ApplicationClientIsGuarantor> ApplicationClientIsGuarantorListByClientNo(int ApplicationId, int ClientNo)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_IS_GUARANTOR_FROM_LMS_BY_CLIENT_NO @application_id = {0},@client_no = {1}", ApplicationId, ClientNo);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientIsGuarantor> RetList = new List<ApplicationClientIsGuarantor>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientIsGuarantor mClass = new ApplicationClientIsGuarantor();
                mClass = (ApplicationClientIsGuarantor)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationClientIsGuarantorListByClientNo

        #region ApplicationClientIsGuarantorListFromLoans
        [WebMethod]

        public List<ApplicationClientIsGuarantor> ApplicationClientIsGuarantorListFromLoans(int ApplicationId, int ClientNo)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_IS_GUARANTOR_FROM_LMS @application_id = {0},@client_no = {1}", ApplicationId, ClientNo);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientIsGuarantor> RetList = new List<ApplicationClientIsGuarantor>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientIsGuarantor mClass = new ApplicationClientIsGuarantor();
                mClass = (ApplicationClientIsGuarantor)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationClientIsGuarantorListSave
        [WebMethod]

        public bool ApplicationClientIsGuarantorListSave(List<ApplicationClientIsGuarantor> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationClientIsGuarantorDelete(application_id);
                foreach (ApplicationClientIsGuarantor _class in _classList)
                {
                    ApplicationClientIsGuarantorSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationClientIsGuarantorSave
        [WebMethod]

        public bool ApplicationClientIsGuarantorSave(ApplicationClientIsGuarantor _class, int user_id)
        {
            bool retValue = false;
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_IS_GUARANTOR_SET @client_is_guarantor_id = {0},@application_id = {1},@guarantor_name = {2}
                ,@guarantor_surname = {3},@guarantee_issuing_date = {4},@guarantee_end_date = {5},@guarantee_iso = {6}
                ,@guarantee_current_amount = {7},@guarantee_overdue_amount ={8},@guarantee_monthly_amount = {9},@guarantor_count = {10},@loan_info_source_id = {11}
                ,@loan_id = {12},@user_id = {13}"
                , _class.CLIENT_IS_GUARANTOR_ID
                , _class.APPLICATION_ID
                , StringFunctions.SqlQuoted(_class.GUARANTOR_NAME, true)
                , StringFunctions.SqlQuoted(_class.GUARANTOR_SURNAME, true)
                , StringFunctions.SqlDateQuoted(_class.GUARANTEE_ISSUING_DATE)
                , StringFunctions.SqlDateQuoted(_class.GUARANTEE_END_DATE)
                , StringFunctions.SqlQuoted(_class.GUARANTEE_ISO, true)
                , _class.GUARANTEE_CURRENT_AMOUNT
                , _class.GUARANTEE_OVERDUE_AMOUNT
                , _class.GUARANTEE_MONTHLY_AMOUNT
                , _class.GUARANTOR_COUNT
                , _class.LOAN_INFO_SOURCE_ID
                , _class.LOAN_ID
                , user_id
                );

                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch (Exception Ex)
            {
                _Log.ErrorLog(Ex.Message, Ex);
            }
            return retValue;
        }
        #endregion
        #endregion

        #region == ClientLoansOverdrafts ==
        [WebMethod]
        public ClientLoansOverdrafts ClientLoansOverdraftsGet(int ApplicationId, int mClientNo)
        {
            ClientLoansOverdrafts mClientLoansOverdrafts = new ClientLoansOverdrafts();
            mClientLoansOverdrafts.ApplicationCurrentLoansList = this.ApplicationCurrentLoansListByClientNo(ApplicationId, mClientNo);
            mClientLoansOverdrafts.ApplicationCurrentOverdraftsList = this.ApplicationCurrentOverdraftsListByClientNo(ApplicationId, mClientNo);
            mClientLoansOverdrafts.ApplicationClientIsGuarantorList = this.ApplicationClientIsGuarantorListByClientNo(ApplicationId, mClientNo);
            return mClientLoansOverdrafts;
        }

        #endregion == ClientLoansOverdrafts ==

        #endregion === ClientLoans and Overdrafts ===

        #region == ApplicationDeposits ==
        #region ApplicationDepositsDelete
        [WebMethod]

        public bool ApplicationDepositsDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_DEPOSITS_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationDepositsList
        [WebMethod]

        public List<ApplicationDeposits> ApplicationDepositsList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_DEPOSITS_GET @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationDeposits> RetList = new List<ApplicationDeposits>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationDeposits mClass = new ApplicationDeposits();
                mClass = (ApplicationDeposits)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region ApplicationDepositsListFromODB
        [WebMethod]

        public List<ApplicationDeposits> ApplicationDepositsListFromODB(int ApplicationId, int ClientNo)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_DEPOSITS_FROM_ODB @application_id = {0},@client_no = {1}", ApplicationId, ClientNo);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationDeposits> RetList = new List<ApplicationDeposits>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationDeposits mClass = new ApplicationDeposits();
                mClass = (ApplicationDeposits)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region ApplicationDepositsListSave
        [WebMethod]

        public bool ApplicationDepositsListSave(List<ApplicationDeposits> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationDepositsDelete(application_id);
                foreach (ApplicationDeposits _class in _classList)
                {
                    ApplicationDepositsSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationDepositsSave
        [WebMethod]

        public bool ApplicationDepositsSave(ApplicationDeposits _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_DEPOSITS_SET ,@deposit_id = {0},@application_id = {1},@depo_acc_id = {2},@amount = {3},@account_iban = {4},@iso = {5},@start_date = {6},@end_date = {7},@intrate = {8},@block_amount = {9},@deposit_reminded_amount = {10},@user_id = {11}"
            , _class.DEPOSIT_ID
            , _class.APPLICATION_ID
            , _class.DEPO_ACC_ID
            , _class.AMOUNT
            , StringFunctions.SqlQuoted(_class.ACCOUNT_IBAN, true)
            , StringFunctions.SqlQuoted(_class.CCY, true)
            , StringFunctions.SqlDateQuoted(_class.START_DATE)
            , StringFunctions.SqlDateQuoted(_class.END_DATE)
            , _class.INTRATE
            , _class.BLOCK_AMOUNT
            , _class.DEPOSIT_REMINDED_AMOUNT
            , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationGuarantor ==
        #region ApplicationGuarantorDelete
        [WebMethod]

        public bool ApplicationGuarantorDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_GUARANTOR_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationGuarantorList
        [WebMethod]

        public List<ApplicationGuarantor> ApplicationGuarantorList(string GuarantorGUID, int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_GUARANTOR_GET @guarantor_guid = {0} ,@application_id = {1}", StringFunctions.SqlQuoted(GuarantorGUID, false), ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationGuarantor> RetList = new List<ApplicationGuarantor>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationGuarantor mClass = new ApplicationGuarantor();
                mClass = (ApplicationGuarantor)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationGuarantorListSave
        [WebMethod]

        public bool ApplicationGuarantorListSave(List<ApplicationGuarantor> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationGuarantorDelete(application_id);
                foreach (ApplicationGuarantor _class in _classList)
                {
                    ApplicationGuarantorSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationGuarantorLoad
        [WebMethod]
        public List<ApplicationGuarantor> ApplicationGuarantorLoad(string personal_id)
        {
            return ApplicationGuarantorLoadWithSalary(personal_id, 0);
        }

        [WebMethod]
        public List<ApplicationGuarantor> ApplicationGuarantorLoadWithSalary(string personal_id, int application_id)
        {
            UTFODBService _odb = new UTFODBService();
            List<ApplicationGuarantor> RetList = new List<ApplicationGuarantor>();
            ApplicationGuarantor _class = new ApplicationGuarantor();
            DataRow mDataRow = null;
            DataSet _ds = null;
            List<City> mCityList = this.CityList();
            decimal mMedian = 0;
            bool mIs_vip_or_employee = false;

            if (application_id != 0)
            {
                RealSalary mRealSalary = new RealSalary();
                try
                {
                    mRealSalary = this.ClientRealSalary(personal_id, application_id);
                    mMedian = mRealSalary.MEDIAN;
                    mIs_vip_or_employee = mRealSalary.IS_VIP_OR_EMPLOYEE;
                }
                catch
                {
                    mMedian = 0;
                    mIs_vip_or_employee = false;
                }
            }

            try
            {
                _odb.Url = _AppSets["ODBServive.URL"];
                ODBAnswer _answer = _odb.GetClientInfoByPID(personal_id);
                _ds = _answer.AnswerDataSet;
                if (_ds != null && _ds.Tables != null && _ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
                {
                    mDataRow = _ds.Tables[0].Rows[0];
                }
            }
            catch
            {
                return RetList;
            }

            if (mDataRow != null)
            {
                foreach (DataRow _dr in _ds.Tables[0].Rows)
                {
                    _class.GUARANTOR_GUID = "";
                    _class.APPLICATION_ID = 0;
                    _class.GUARANTOR_CLIENT_NO = (DBNull.Value.Equals(_dr["CLIENT_NO"])) ? 0 : Convert.ToInt32(_dr["CLIENT_NO"]);
                    _class.IS_EMPLOYEE = Convert.ToBoolean(_dr["IS_EMPLOYEE"]);
                    _class.GUARANTOR_NAME = (DBNull.Value.Equals(_dr["FIRST_NAME"])) ? "" : Convert.ToString(_dr["FIRST_NAME"]);
                    _class.GUARANTOR_SURNAME = (DBNull.Value.Equals(_dr["LAST_NAME"])) ? "" : Convert.ToString(_dr["LAST_NAME"]);
                    _class.CLIENT_CITIZENSHIP = (DBNull.Value.Equals(_dr["COUNTRY"])) ? "" : Convert.ToString(_dr["COUNTRY"]);
                    _class.GUARANTOR_PHONE = (DBNull.Value.Equals(_dr["PHONE1"])) ? "" : Convert.ToString(_dr["PHONE1"]);
                    _class.GUARANTOR_MOBILE = (DBNull.Value.Equals(_dr["PHONE2"])) ? "" : Convert.ToString(_dr["PHONE2"]);

                    _class.GUARANTOR_LEGAL_ADDRESS = (DBNull.Value.Equals(_dr["ADDRESS_JUR"])) ? "" : Convert.ToString(_dr["ADDRESS_JUR"]);
                    _class.GUARANTOR_BIRHT_DATE = (DBNull.Value.Equals(_dr["BIRTH_DATE"])) ? Convert.ToDateTime(0) : Convert.ToDateTime(_dr["BIRTH_DATE"]);
                    _class.GUARANTOR_AGE = (DBNull.Value.Equals(_dr["CLIENT_AGE"])) ? 0 : Convert.ToInt32(_dr["CLIENT_AGE"]);
                    _class.GUARANTOR_BIRHT_PLACE = (DBNull.Value.Equals(_dr["BIRTH_PLACE"])) ? "" : Convert.ToString(_dr["BIRTH_PLACE"]);
                    _class.GUARANTOR_ADDRESS = (DBNull.Value.Equals(_dr["ADDRESS_FACT"])) ? "" : Convert.ToString(_dr["ADDRESS_FACT"]);
                    _class.GUARANTOR_GENDER = (DBNull.Value.Equals(_dr["MALE_FEMALE"])) ? 0 : Convert.ToInt32(_dr["MALE_FEMALE"]);

                    if (_class.IS_EMPLOYEE)
                        _class.GUARANTOR_ORGANIZATION_ID = Convert.ToInt32(_AppSets["VTBNumInOrgList"]);
                    else
                        _class.GUARANTOR_ORGANIZATION_ID = 0;

                    _class.GUARANTOR_ORGANIZATION_NAME = "";//(DBNull.Value.Equals(_dr["JOB_PLACE"])) ? "" : Convert.ToString(_dr["JOB_PLACE"]);
                    _class.GUARANTOR_WORK_EXPERIENCE = 0;
                    _class.GUARANTOR_EXPERIENCE_PERIOD_ID = 12;
                    _class.GUARANTOR_MONTHLY_INCOME = (mIs_vip_or_employee) ? 0 : mMedian;
                    _class.REAL_SALARY = mMedian;
                    _class.GUARANTOR_MONTHLY_INCOME_CURRENCY = "GEL";
                    _class.GUARANTOR_POSITION = (DBNull.Value.Equals(_dr["JOB_TITLE"])) ? "" : Convert.ToString(_dr["JOB_TITLE"]);
                    _class.GUARANTOR_TOTAL_WORK_EXPERIENCE = 0;
                    _class.TOTAL_WORK_EXPERIENCE_PERIOD = 12;
                    _class.GUARANTOR_INCOME_TYPE_ID = 1;
                    int rec_state = (DBNull.Value.Equals(_dr["REC_STATE"])) ? 2 : Convert.ToInt32(_dr["REC_STATE"]);
                    _class.IS_ACTIVE_IN_ODB = (rec_state == 1) ? true : false;


                    _class.GUARANTOR_PERSONAL_ID = (DBNull.Value.Equals(_dr["PERSONAL_ID"])) ? "" : Convert.ToString(_dr["PERSONAL_ID"]);
                    _class.PASSPORT_TYPE_ID = (DBNull.Value.Equals(_dr["PASSPORT_TYPE_ID"])) ? 0 : Convert.ToInt32(_dr["PASSPORT_TYPE_ID"]);
                    _class.GUARANTOR_PASSPORT_ID = (DBNull.Value.Equals(_dr["PASSPORT"])) ? "" : Convert.ToString(_dr["PASSPORT"]);

                    _class.PASSPORT_COUNTRY = (DBNull.Value.Equals(_dr["PASSPORT_COUNTRY"])) ? "" : Convert.ToString(_dr["PASSPORT_COUNTRY"]);
                    _class.PASSPORT_ISSUE_DT = (DBNull.Value.Equals(_dr["PASSPORT_ISSUE_DT"])) ? DateTime.Now.Date : Convert.ToDateTime(_dr["PASSPORT_ISSUE_DT"]);
                    _class.PASSPORT_END_DATE = (DBNull.Value.Equals(_dr["PASSPORT_END_DATE"])) ? new DateTime(0001, 1, 1) : Convert.ToDateTime(_dr["PASSPORT_END_DATE"]);
                    if (_class.PASSPORT_END_DATE == new DateTime(0001, 1, 1))
                    {
                        _class.PASSPORT_IS_LIFE = true;
                    }
                    else
                    {
                        _class.PASSPORT_IS_LIFE = false;
                    }
                    _class.PASSPORT_REG_ORGAN = (DBNull.Value.Equals(_dr["PASSPORT_REG_ORGAN"])) ? "" : Convert.ToString(_dr["PASSPORT_REG_ORGAN"]);
                    _class.E_MAIL = (DBNull.Value.Equals(_dr["E_MAIL"])) ? "" : Convert.ToString(_dr["E_MAIL"]);


                    try
                    {
                        _class.GUARANTOR_CITY_ID = mCityList.First(item => item.CITY_NAME == StringFunctions.ConvertToUnicode(_dr["CITY"].ToString())).CITY_ID;
                    }
                    catch
                    {
                        _class.GUARANTOR_CITY_ID = 0;
                    }
                    _class.GUARANTOR_MARITAL_STATUS = (DBNull.Value.Equals(_dr["MARITAL_STATUS"])) ? 0 : Convert.ToInt32(_dr["MARITAL_STATUS"]); ;

                    _class.IS_EMPLOYEE = Convert.ToBoolean(_dr["IS_EMPLOYEE"]);
                    _class.IS_INSIDER = Convert.ToBoolean(_dr["IS_INSIDER"]);
                    _class.CLIENT_CLASIFF = (DBNull.Value.Equals(_dr["CLASIFF"])) ? "SS" : Convert.ToString(_dr["CLASIFF"]);

                    RetList.Add(_class);
                }
            }
            else
            {
                _class.GUARANTOR_PERSONAL_ID = personal_id;
                _class.GUARANTOR_BIRHT_DATE = DateTime.Now.Date;
                _class.GUARANTOR_GUID = "";
                _class.APPLICATION_ID = 0;
                _class.GUARANTOR_CLIENT_NO = 0;
                _class.GUARANTOR_PHONE = "";
                _class.GUARANTOR_MOBILE = "";
                _class.GUARANTOR_ORGANIZATION_ID = 0;
                _class.GUARANTOR_ORGANIZATION_NAME = "";
                _class.GUARANTOR_WORK_EXPERIENCE = 0;
                _class.GUARANTOR_EXPERIENCE_PERIOD_ID = 12;
                _class.GUARANTOR_MONTHLY_INCOME = 0;
                _class.GUARANTOR_MONTHLY_INCOME_CURRENCY = "GEL";
                _class.GUARANTOR_POSITION = "";
                _class.GUARANTOR_TOTAL_WORK_EXPERIENCE = 0;
                _class.TOTAL_WORK_EXPERIENCE_PERIOD = 12;
                _class.GUARANTOR_INCOME_TYPE_ID = 1;
                _class.IS_ACTIVE_IN_ODB = false;
                _class.IS_EMPLOYEE = false;
                _class.PASSPORT_TYPE_ID = 1;
                _class.GUARANTOR_CITY_ID = 0;
                _class.PASSPORT_COUNTRY = "GE";
                _class.PASSPORT_IS_LIFE = false;

                RetList.Add(_class);
            }

            return RetList;
        }
        #endregion

        #region ApplicationGuarantorSave
        [WebMethod]

        public bool ApplicationGuarantorSave(ApplicationGuarantor _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_GUARANTOR_SET @guarantor_guid = {0},@application_id = {1},@guarantor_name = {2},@guarantor_surname = {3},@guarantor_phone = {4}
                ,@guarantor_mobile = {5},@guarantor_legal_address = {6},@guarantor_organization_id = {7},@guarantor_work_experience = {8},@guarantor_experience_period_id = {9},@guarantor_monthly_income = {10}
                ,@guarantor_monthly_income_currency = {11},@guarantor_birht_date = {12},@guarantor_age = {13},@guarantor_personal_id = {14},@guarantor_relationship_type_id = {15},@guarantor_address = {16}
                ,@guarantor_position = {17},@guarantor_total_work_experience = {18},@total_work_experience_period = {19},@guarantor_income_type_id = {20},@guarantor_client_no = {21},@user_id = {22}
                ,@guarantor_gender = {23},@guarantor_passport_id = {24},@guarantor_city_id = {25},@guarantor_marital_status = {26},@client_citizenship = {27}
                ,@passport_type_id = {28},@passport_country = {29},@passport_issue_dt = {30},@passport_end_date = {31},@passport_is_life = {32},@passport_reg_organ = {33}
                ,@e_mail = {34},@guarantor_birht_place = {35},@salary_category = {36},@current_negative_statuses = {37}, @terminated_negative_statuses = {38}
                ,@positive_statuses = {39},@risk_grade = {40},@calc_income = {41},@calc_salary = {42},@max_positive_amount = {43}, @real_salary = {44}
                , @family_marital_status = {45}, @family_spouse_personal_id = {46}, @family_spouse_name = {47}, @family_spouse_surname = {48}, @family_spouse_place_of_work = {49}, @family_spouse_position = {50}
	            , @family_spouse_income = {51}, @family_spouse_income_currency = {52}, @family_spouse_income_type = {53}, @family_number_of_children = {54}, @family_total_family_members = {55}
	            , @family_total_family_income = {56}, @family_total_family_income_currency = {57}, @guarantor_type_id = {58}, @is_insider = {59}, @client_clasiff = {60}
                , @education_id = {61}, @post_category_id = {62}, @activity_area_id = {63}, @emploee_count_id = {64}"
            , StringFunctions.SqlQuoted(_class.GUARANTOR_GUID, false)
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.GUARANTOR_NAME, true)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_SURNAME, true)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_PHONE, true)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_MOBILE, true)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_LEGAL_ADDRESS, true)
            , _class.GUARANTOR_ORGANIZATION_ID
            , _class.GUARANTOR_WORK_EXPERIENCE
            , _class.GUARANTOR_EXPERIENCE_PERIOD_ID
            , _class.GUARANTOR_MONTHLY_INCOME
            , StringFunctions.SqlQuoted(_class.GUARANTOR_MONTHLY_INCOME_CURRENCY, true)
            , StringFunctions.SqlDateQuoted(_class.GUARANTOR_BIRHT_DATE)
            , _class.GUARANTOR_AGE
            , StringFunctions.SqlQuoted(_class.GUARANTOR_PERSONAL_ID, true)
            , _class.GUARANTOR_RELATIONSHIP_TYPE_ID
            , StringFunctions.SqlQuoted(_class.GUARANTOR_ADDRESS, true)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_POSITION, true)
            , _class.GUARANTOR_TOTAL_WORK_EXPERIENCE
            , _class.TOTAL_WORK_EXPERIENCE_PERIOD
            , _class.GUARANTOR_INCOME_TYPE_ID
            , _class.GUARANTOR_CLIENT_NO
            , user_id
            , _class.GUARANTOR_GENDER
            , StringFunctions.SqlQuoted(_class.GUARANTOR_PASSPORT_ID, true)
            , _class.GUARANTOR_CITY_ID
            , _class.GUARANTOR_MARITAL_STATUS
            , StringFunctions.SqlQuoted(_class.CLIENT_CITIZENSHIP, true)
            , _class.PASSPORT_TYPE_ID
            , StringFunctions.SqlQuoted(_class.PASSPORT_COUNTRY, true)
            , StringFunctions.SqlDateQuoted(_class.PASSPORT_ISSUE_DT)
            , StringFunctions.SqlDateQuoted(_class.PASSPORT_END_DATE)
            , _class.PASSPORT_IS_LIFE
            , StringFunctions.SqlQuoted(_class.PASSPORT_REG_ORGAN, true)
            , StringFunctions.SqlQuoted(_class.E_MAIL, true)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_BIRHT_PLACE, true)
            , _class.SALARY_CATEGORY
            , _class.CURRENT_NEGATIVE_STATUSES
            , _class.TERMINATED_NEGATIVE_STATUSES
            , _class.POSITIVE_STATUSES
            , StringFunctions.SqlQuoted(_class.RISK_GRADE, true)
            , _class.CALC_INCOME
            , _class.CALC_SALARY
            , _class.MAX_POSITIVE_AMOUNT
            , _class.REAL_SALARY

            , _class.FAMILY_MARITAL_STATUS
            , StringFunctions.SqlQuoted(_class.FAMILY_SPOUSE_PERSONAL_ID)
            , StringFunctions.SqlQuoted(_class.FAMILY_SPOUSE_NAME, true)
            , StringFunctions.SqlQuoted(_class.FAMILY_SPOUSE_SURNAME, true)
            , StringFunctions.SqlQuoted(_class.FAMILY_SPOUSE_PLACE_OF_WORK, true)
            , StringFunctions.SqlQuoted(_class.FAMILY_SPOUSE_POSITION, true)
            , _class.FAMILY_SPOUSE_INCOME
            , StringFunctions.SqlQuoted(_class.FAMILY_SPOUSE_INCOME_CURRENCY)
            , _class.FAMILY_SPOUSE_INCOME_TYPE
            , _class.FAMILY_NUMBER_OF_CHILDREN
            , _class.FAMILY_TOTAL_FAMILY_MEMBERS
            , _class.FAMILY_TOTAL_FAMILY_INCOME
            , StringFunctions.SqlQuoted(_class.FAMILY_TOTAL_FAMILY_INCOME_CURRENCY, true)
            , _class.GUARANTOR_TYPE_ID
            , (_class.IS_INSIDER) ? 1 : 0
            , StringFunctions.SqlQuoted(_class.CLIENT_CLASIFF)

            , _class.EDUCATION_ID
            , _class.POST_CATEGORY_ID
            , _class.ACTIVITY_AREA_ID
            , _class.EMPLOEE_COUNT_ID
            );

            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #endregion

        #region == ApplicationGuarantorGuatantedLoans ==
        #region ApplicationGuarantorGuatantedLoansDelete
        [WebMethod]

        public bool ApplicationGuarantorGuatantedLoansDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_GUARANTOR_GUATANTED_LOANS_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationGuarantorGuatantedLoansList
        [WebMethod]

        public List<ApplicationGuarantorGuatantedLoans> ApplicationGuarantorGuatantedLoansList(int GuarantorGuatantedLoansId, int ApplicationId, string GuarantorGUID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_GUARANTOR_GUATANTED_LOANS_GET @guarantor_guatanted_loans_id  = {0} ,@application_id = {1},@guarantor_guid = {2}", GuarantorGuatantedLoansId, ApplicationId, StringFunctions.SqlQuoted(GuarantorGUID, false));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationGuarantorGuatantedLoans> RetList = new List<ApplicationGuarantorGuatantedLoans>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationGuarantorGuatantedLoans mClass = new ApplicationGuarantorGuatantedLoans();
                mClass = (ApplicationGuarantorGuatantedLoans)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }


        #endregion

        #region ApplicationGuarantorGuatantedLoansListFromLoans
        [WebMethod]

        public List<ApplicationGuarantorGuatantedLoans> ApplicationGuarantorGuatantedLoansListFromLoans(int clientID, int ApplicationId, string GuarantorGUID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_GUARANTOR_GUATANTED_LOANS_FROM_LMS @client_id  = {0} ,@application_id = {1},@guarantor_guid = {2}", clientID, ApplicationId, StringFunctions.SqlQuoted(GuarantorGUID, false));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationGuarantorGuatantedLoans> RetList = new List<ApplicationGuarantorGuatantedLoans>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationGuarantorGuatantedLoans mClass = new ApplicationGuarantorGuatantedLoans();
                mClass = (ApplicationGuarantorGuatantedLoans)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationGuarantorGuatantedLoansListSave
        [WebMethod]

        public bool ApplicationGuarantorGuatantedLoansListSave(List<ApplicationGuarantorGuatantedLoans> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationGuarantorGuatantedLoansDelete(application_id);
                foreach (ApplicationGuarantorGuatantedLoans _class in _classList)
                {
                    ApplicationGuarantorGuatantedLoansSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationGuarantorGuatantedLoansSave
        [WebMethod]

        public bool ApplicationGuarantorGuatantedLoansSave(ApplicationGuarantorGuatantedLoans _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_GUARANTOR_GUATANTED_LOANS_SET @guarantor_guatanted_loans_id = {0},@application_id = {1},@guarantor_guid = {2}
                ,@guarantor_guaranted_name = {3},@guarantor_guaranted_loan_currency = {4},@guarantor_guaranted_loan_current_debt = {5},@guarantor_guaranted_loan_overdue_debt = {6}
                ,@guarantor_guaranted_pmt = {7},@guarantor_count = {8},@loan_id = {9},@loan_info_source_id = {10},@user_id = {11}"
            , _class.GUARANTOR_GUATANTED_LOANS_ID
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.GUARANTOR_GUID, false)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_GUARANTED_NAME, true)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_GUARANTED_LOAN_CURRENCY, true)
            , _class.GUARANTOR_GUARANTED_LOAN_CURRENT_DEBT
            , _class.GUARANTOR_GUARANTED_LOAN_OVERDUE_DEBT
            , _class.GUARANTOR_GUARANTED_PMT
            , _class.GUARANTOR_COUNT
            , _class.LOAN_ID
            , _class.LOAN_INFO_SOURCE_ID
            , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationGuarantorLoans ==
        #region ApplicationGuarantorLoansDelete
        [WebMethod]

        public bool ApplicationGuarantorLoansDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_GUARANTOR_LOANS_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationGuarantorLoansList
        [WebMethod]

        public List<ApplicationGuarantorLoans> ApplicationGuarantorLoansList(int GuarantorLoansId, int ApplicationId, string GuarantorGUID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_GUARANTOR_LOANS_GET @guarantor_loans_id  = {0} ,@application_id = {1},@guarantor_guid = {2}", GuarantorLoansId, ApplicationId, StringFunctions.SqlQuoted(GuarantorGUID, false));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationGuarantorLoans> RetList = new List<ApplicationGuarantorLoans>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationGuarantorLoans mClass = new ApplicationGuarantorLoans();
                mClass = (ApplicationGuarantorLoans)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region ApplicationGuarantorLoansListFromLoans
        [WebMethod]

        public List<ApplicationGuarantorLoans> ApplicationGuarantorLoansListFromLoans(int ClientId, int ApplicationId, string GuarantorGUID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_GUARANTOR_LOANS_FROM_LMS @client_id  = {0} ,@application_id = {1},@guarantor_guid = {2}", ClientId, ApplicationId, StringFunctions.SqlQuoted(GuarantorGUID, false));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationGuarantorLoans> RetList = new List<ApplicationGuarantorLoans>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationGuarantorLoans mClass = new ApplicationGuarantorLoans();
                mClass = (ApplicationGuarantorLoans)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region ApplicationGuarantorLoansListSave
        [WebMethod]

        public bool ApplicationGuarantorLoansListSave(List<ApplicationGuarantorLoans> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationGuarantorLoansDelete(application_id);
                foreach (ApplicationGuarantorLoans _class in _classList)
                {
                    ApplicationGuarantorLoansSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationGuarantorLoansSave
        [WebMethod]

        public bool ApplicationGuarantorLoansSave(ApplicationGuarantorLoans _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_GUARANTOR_LOANS_SET 
                @guarantor_loans_id = {0}, @application_id = {1}, @guarantor_guid = {2}, @guarantor_credit_currency = {3}, 
                @guarantor_credit_current_debt = {4}, @guarantor_credit_overdue_debt = {5}, @guarantor_credit_pmt = {6}, @guarantor_crdit_card_limit = {7},
                @guarantor_credit_interest = {8}, @loan_id = {9}, @loan_info_source_id = {10}, @loan_cover = {11}, 
                @user_id = {12}"
            , _class.GUARANTOR_LOANS_ID
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.GUARANTOR_GUID, false)
            , StringFunctions.SqlQuoted(_class.GUARANTOR_CREDIT_CURRENCY, true)
            , _class.GUARANTOR_CREDIT_CURRENT_DEBT
            , _class.GUARANTOR_CREDIT_OVERDUE_DEBT
            , _class.GUARANTOR_CREDIT_PMT
            , _class.GUARANTOR_CRDIT_CARD_LIMIT
            , _class.GUARANTOR_CREDIT_INTEREST
            , _class.LOAN_ID
            , _class.LOAN_INFO_SOURCE_ID
            , _class.LOAN_COVER
            , user_id
            );


            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationHistory ==
        #region ApplicationHistoryDelete
        [WebMethod]

        public bool ApplicationHistoryDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_HISTORY_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }

        #endregion

        #region ApplicationHistoryList
        [WebMethod]

        public List<ApplicationHistory> ApplicationHistoryList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_HISTORY_LIST @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationHistory> RetList = new List<ApplicationHistory>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationHistory mClass = new ApplicationHistory();
                mClass = (ApplicationHistory)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationHistorySave
        [WebMethod]

        public bool ApplicationHistorySave(ApplicationHistory _class)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_HISTORY_SET @rec_id = {0},@application_id = {1},@oper_date = {2},@state_id = {3},@prev_state_id = {4},@user_id = {5},@descrip = {6}"
            , _class.REC_ID
            , _class.APPLICATION_ID
            , StringFunctions.SqlDateQuoted(_class.OPER_DATE)
            , _class.STATE_ID
            , _class.PREV_STATE_ID
            , _class.USER_ID
            , StringFunctions.SqlQuoted(_class.DESCRIP, true)
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationLock ==
        #region ApplicationLockCheck
        [WebMethod]

        public ApplicationLock ApplicationLockCheck(int mApplication_type, int mApplication_id, int mUser_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_LOCK_CHECK @application_type = {0},@application_id = {1}, @user_id = {2}", mApplication_type, mApplication_id, mUser_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            ApplicationLock mClass = new ApplicationLock();
            mClass = (ApplicationLock)FillClass(mClass, mDataTable.Rows[0]);
            return mClass;
        }
        #endregion

        #region ApplicationLockRemove
        [WebMethod]

        public ApplicationLock ApplicationLockRemove(int mApplication_type, int mApplication_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_LOCK @application_type = {0},@application_id = {1},@lock = 0", mApplication_type, mApplication_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            ApplicationLock mClass = new ApplicationLock();
            mClass = (ApplicationLock)FillClass(mClass, mDataTable.Rows[0]);
            return mClass;
        }
        #endregion

        #region ApplicationLockRemoveAll
        [WebMethod]

        public ApplicationLock ApplicationLockRemoveAll(int mUser_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_LOCK @user_id = {0},@lock = 0", mUser_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            ApplicationLock mClass = new ApplicationLock();
            mClass = (ApplicationLock)FillClass(mClass, mDataTable.Rows[0]);
            return mClass;
        }

        #endregion

        #region ApplicationLockSet
        #region application lock
        [WebMethod]

        public ApplicationLock ApplicationLockSet(int mApplication_type, int mApplication_id, int mLocked_by)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_LOCK @application_type = {0},@application_id = {1},@locked_by = {2},@lock = 1", mApplication_type, mApplication_id, mLocked_by);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            ApplicationLock mClass = new ApplicationLock();
            mClass = (ApplicationLock)FillClass(mClass, mDataTable.Rows[0]);
            return mClass;
        }
        #endregion
        #endregion
        #endregion

        #region == ApplicationPicture ==
        #region ApplicationPictureDelete
        [WebMethod]

        public bool ApplicationPictureDelete(int application_type_id, int application_id, int object_type_id, int object_id, int picture_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_PICTURE_DEL @application_type_id = {0}, @application_id = {1},@object_type_id = {2},@object_id = {3},@picture_id = {4}"
                , application_type_id, application_id, object_type_id, object_id, picture_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationPictureList
        [WebMethod]

        public List<ApplicationPicture> ApplicationPictureList(int application_type_id, int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_PICTURE_GET @application_type_id = {0}, @application_id = {1}", application_type_id, application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationPicture> RetList = new List<ApplicationPicture>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationPicture mClass = new ApplicationPicture();
                mClass = (ApplicationPicture)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationPictureListSave
        [WebMethod]

        public bool ApplicationPictureListSave(List<ApplicationPicture> _classList, int user_id, int application_type_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationPictureDelete(application_type_id, application_id, 0, 0, 0);
                foreach (ApplicationPicture _class in _classList)
                {
                    this.ApplicationPictureSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationPictureSave
        [WebMethod]

        public int ApplicationPictureSave(ApplicationPicture _class, int user_id)
        {
            int retValue = 0;
            //string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_PICTURE_SET @picture_id = {0},@application_type_id = {1},@application_id = {2}, @object_id = {3},@object_type_id = {4},@picture_name = {5},@user_id = {6},@image "
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_PICTURE_SET {0},{1},{2}, {3},{4},{5},{6},@image "
            , _class.PICTURE_ID
            , _class.APPLICATION_TYPE_ID
            , _class.APPLICATION_ID
            , _class.OBJECT_ID
            , _class.OBJECT_TYPE_ID
            , StringFunctions.SqlQuoted(_class.PICTURE_NAME, true)
            , user_id
            );
            try
            {
                if (_Log != null && _class.PICTURE == null)
                {
                    _Log.ErrorLog("Picture is null", null);
                }
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL, 300, true, _class.PICTURE);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationFile ==
        #region ApplicationFileDelete
        [WebMethod]
        public bool ApplicationFileDelete(int File_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"dbo.pAPPLICATION_FILES_DEL  @file_id = {0}", File_id);
            try
            {
                _DGate.Execute(mSQL);
                retValue = true;
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationFileDeleteAll
        [WebMethod]
        public bool ApplicationFileDeleteAll(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"dbo.pAPPLICATION_FILES_DEL_ALL  @application_id = {0}", @application_id);
            try
            {
                _DGate.Execute(mSQL);
                retValue = true;
            }
            catch { }
            return retValue;
        }
        #endregion
        #region ApplicationFileList
        [WebMethod]

        public List<ApplicationFile> ApplicationFileList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_FILES_LIST @application_type_id = 1,@application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationFile> RetList = new List<ApplicationFile>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationFile mClass = new ApplicationFile();
                mClass = (ApplicationFile)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationFileListSave
        [WebMethod]

        public bool ApplicationFileListSave(List<ApplicationFile> _classList, int application_id, int user_id)
        {
            bool retValue = true;
            try
            {
                ApplicationFileDeleteAll(application_id);
                foreach (ApplicationFile _class in _classList)
                {
                    this.ApplicationFileSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationFileSave
        [WebMethod]

        public int ApplicationFileSave(ApplicationFile _class, int user_id)
        {
            int retValue = 0;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_FILES_SET {0}, 1, {1}, {2}, {3}, @file_data "
            , _class.FILE_UID
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.UFILE_NAME, true)
            , StringFunctions.SqlQuoted(_class.FILE_CONTENT_TYPE, true)
            , _class.FILE_DATA
            );
            try
            {
                if (_Log != null && _class.FILE_DATA == null)
                {
                    _Log.ErrorLog("File is null", null);
                }
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL, 30, true, _class.FILE_DATA, "varbinary");
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == Application Admin Debts ==

        #region ApplicationAdminDebts

        #region ApplicationAdminDebtsList
        [WebMethod]
        public List<ApplicationAdminDebts> ApplicationAdminDebtsList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADMINISTRATION_DEBTS_LIST @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationAdminDebts> RetList = new List<ApplicationAdminDebts>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationAdminDebts mClass = new ApplicationAdminDebts();
                mClass = (ApplicationAdminDebts)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationAdminDebtsList

        #region ApplicationAdminDebtsDel
        [WebMethod]
        public bool ApplicationAdminDebtsDel(int application_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADMINISTRATION_DEBTS_DEL @application_id = {0}", application_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public bool ApplicationAdminDebtsDelOne(int debt_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADMINISTRATION_DEBTS_DEL_ONE @debt_id = {0}", debt_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationAdminDebtsDel

        #region ApplicationAdminDebtsSet
        [WebMethod]
        public bool ApplicationAdminDebtsSet(ApplicationAdminDebts _class)
        {
            try
            {
                _class.REG_DATE = DateTime.Now.Date;
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_ADMINISTRATION_DEBTS_SET @application_id = {0} ,@client_id = {1} ,@debt_type1_id = {2} ,@debt_type2_id = {3} ,@object_type_id = {4} ,
                                                @object_id = {5} ,@reg_date = {6} ,@exec_date = {7} ,@state = {8} ,@comment = {9}"
                    , _class.APPLICATION_ID
                    , _class.CLIENT_ID
                    , _class.DEBT_TYPE1_ID
                    , _class.DEBT_TYPE2_ID
                    , _class.OBJECT_TYPE_ID
                    , _class.OBJECT_ID
                    , StringFunctions.SqlDateQuoted(_class.REG_DATE)
                    , StringFunctions.SqlDateQuoted(_class.EXEC_DATE)
                    , _class.STATE
                    , StringFunctions.SqlQuoted(_class.COMMENT, true)
                    );
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationPurchaseItemsSet

        #region ApplicationAdminDebtsListSave
        [WebMethod]
        public bool ApplicationAdminDebtsListSave(int application_id, List<ApplicationAdminDebts> ApplicationAdminDebtsList)
        {
            this.ApplicationPurchaseItemDel(application_id);
            ApplicationAdminDebtsList.ForEach(item =>
            {
                this.ApplicationAdminDebtsSet(item);
            }
                );

            return true;
        }
        #endregion ApplicationAdminDebtsListSave

        #endregion ApplicationAdminDebts

        #region AdminDebtTypes1
        [WebMethod]
        public List<PipelineWS.LocalModel.AdminDebtTypes1> AdminDebtTypes1List()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ADMIN_DEBT_TYPES1_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.AdminDebtTypes1> RetList = new List<PipelineWS.LocalModel.AdminDebtTypes1>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.AdminDebtTypes1 mClass = new PipelineWS.LocalModel.AdminDebtTypes1();
                mClass = (PipelineWS.LocalModel.AdminDebtTypes1)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion AdminDebtTypes1

        #region AdminDebtTypes2
        [WebMethod]
        public List<PipelineWS.LocalModel.AdminDebtTypes2> AdminDebtTypes2List()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ADMIN_DEBT_TYPES2_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.AdminDebtTypes2> RetList = new List<PipelineWS.LocalModel.AdminDebtTypes2>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.AdminDebtTypes2 mClass = new PipelineWS.LocalModel.AdminDebtTypes2();
                mClass = (PipelineWS.LocalModel.AdminDebtTypes2)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion AdminDebtTypes2

        #region AdminDebtObjects
        [WebMethod]
        public List<AdminDebtObjects> AdminDebtObjectsList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ADMIN_DEBT_OBJECTS_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<AdminDebtObjects> RetList = new List<AdminDebtObjects>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                AdminDebtObjects mClass = new AdminDebtObjects();
                mClass = (AdminDebtObjects)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion AdminDebtObjects

        #endregion == Application Admin Debts ==

        #region == Application Admin Debts NEW ==

        #region AppAdminDebts

        #region AppAdminDebtsList
        [WebMethod]
        public List<AppAdminDebt> AppAdminDebtList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADMIN_DEBTS_LIST @application_id = {0}", application_id);
            List<AppAdminDebt> RetList = new List<AppAdminDebt>();
            try
            {
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                foreach (DataRow mDataRow in mDataTable.Rows)
                {
                    AppAdminDebt mClass = new AppAdminDebt();
                    mClass = (AppAdminDebt)FillClass(mClass, mDataRow);
                    RetList.Add(mClass);
                }
                //this.ApplicationLogSet(application_id, 1, 0, mSQL +  " - " + mDataTable.Rows.Count.ToString());
            }
            catch (Exception ex)
            {
                this.ApplicationLogSet(application_id, 1, 0, "AppAdminDebtList" + ex.Message);
                RetList = new List<AppAdminDebt>();
            }
            return RetList;
        }

        [WebMethod]
        public List<AppAdminDebt> AppAdminDebtForRestruct(int loan_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADMIN_DEBTS_FOR_RESTRUCT @loan_id = {0}", loan_id);
            List<AppAdminDebt> RetList = new List<AppAdminDebt>();
            try
            {
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                foreach (DataRow mDataRow in mDataTable.Rows)
                {
                    AppAdminDebt mClass = new AppAdminDebt();
                    mClass = (AppAdminDebt)FillClass(mClass, mDataRow);
                    RetList.Add(mClass);
                }
            }
            catch (Exception ex)
            {
                RetList = new List<AppAdminDebt>();
            }
            return RetList;
        }
        #endregion AppAdminDebtsList

        #region AppAdminDebtsDel

        [WebMethod]
        public bool AppAdminDebtsDel(int application_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADMIN_DEBTS_DELETE @application_id = {0}", application_id);
                _DGate.Execute(mSQL);
                this.ApplicationLogSet(application_id, 1, 0, mSQL);
            }
            catch (Exception ex)
            {
                this.ApplicationLogSet(application_id, 1, 0, "AppAdminDebtsDel" + ex.Message);
                return false;
            }

            return true;
        }

        #endregion AppAdminDebtsDel

        #region AppAdminDebtsSet
        [WebMethod]
        public bool AppAdminDebtsSet(int application_id, AppAdminDebt _class)
        {
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_ADMIN_DEBTS_INSERT @APPLICATION_ID = {0},@ADMIN_GROUP_ID = {1}, @ADMIN_ITEM_ID = {2}, @AMOUNT = {3}, @BANK_BIC_CODE = {4}
                                            , @BANK_COMMENT = {5}, @COMMENT = {6}, @DEBT_DATE = {7}, @LMS_DUE_DAYS = {8}, @TRANSH_TEXT = {9}, @USER_ID = {10}"
                    , application_id
                    , _class.ADMIN_GROUP_ID
                    , _class.ADMIN_ITEM_ID
                    , _class.AMOUNT
                    , StringFunctions.SqlQuoted(_class.BANK_BIC_CODE, true)
                    , StringFunctions.SqlQuoted(_class.BANK_COMMENT, true)
                    , StringFunctions.SqlQuoted(_class.COMMENT, true)
                    , StringFunctions.SqlDateQuoted(_class.DEBT_DATE)
                    , _class.LMS_DUE_DAYS
                    , StringFunctions.SqlQuoted(_class.TRANSH_TEXT, true)
                    , _class.USER_ID
                );
                _DGate.Execute(mSQL);
                this.ApplicationLogSet(application_id, 1, 0, mSQL);
                return true;
            }
            catch (Exception ex)
            {

                this.ApplicationLogSet(application_id, 1, 0, "AppAdminDebtsSet:" + ex.Message);
                return false;
            }
        }
        #endregion AppAdminDebtsSet

        #region AppAdminDebtsListSave
        [WebMethod]
        public bool AppAdminDebtsListSave(int application_id, List<AppAdminDebt> AppAdminDebtsList)
        {
            this.AppAdminDebtsDel(application_id);
            AppAdminDebtsList.ForEach(item =>
            {
                this.AppAdminDebtsSet(application_id, item);
            }
            );
            return true;
        }
        #endregion AppAdminDebtsListSave

        #endregion AppAdminDebts

        #region == Admin Debts NEW ==

        [WebMethod]
        public List<AdminDebtGroup> AdminDebtGroupList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ADMIN_DEBT_GROUP");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<AdminDebtGroup> RetList = new List<AdminDebtGroup>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                AdminDebtGroup mClass = new AdminDebtGroup();
                mClass = (AdminDebtGroup)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<AdminDebtItem> AdminDebtItemList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ADMIN_DEBT_ITEM");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<AdminDebtItem> RetList = new List<AdminDebtItem>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                AdminDebtItem mClass = new AdminDebtItem();
                mClass = (AdminDebtItem)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<AdminDebtItemFull> AdminDebtItemFullList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ADMIN_DEBT_ITEM_FULL");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<AdminDebtItemFull> RetList = new List<AdminDebtItemFull>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                AdminDebtItemFull mClass = new AdminDebtItemFull();
                mClass = (AdminDebtItemFull)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion == Admin Debts NEW ==

        #endregion == Application Admin Debts NEW ==

        #region == ApplicationRealEstate ALL ==
        #region == ApplicationRealEstate ==
        #region ApplicationRealEstateDelete
        [WebMethod]
        public bool ApplicationRealEstateDelete(int application_id)
        {
            bool retValue = false;

            retValue = this.ApplicationRealEstateMainDelete(application_id);
            retValue = this.ApplicationRealEstateCollateralDelete(application_id);

            return retValue;
        }
        #endregion

        #region ApplicationRealEstateGet
        [WebMethod]
        public ApplicationRealEstate ApplicationRealEstateGet(int mApplicationId)
        {
            ApplicationRealEstate mApplicationRealEstate = new ApplicationRealEstate();
            mApplicationRealEstate.Main = this.ApplicationRealEstateMainGet(mApplicationId);
            mApplicationRealEstate.CollateralList = this.ApplicationRealEstateCollateralGet(mApplicationId);

            return mApplicationRealEstate;
        }
        #endregion

        #region ApplicationRealEstateSave
        [WebMethod]
        public bool ApplicationRealEstateSave(ApplicationRealEstate mApplicationRealEstate, int mUserID)
        {
            bool retValue = false;

            if (mApplicationRealEstate != null)
            {
                try
                {
                    retValue = this.ApplicationRealEstateMainSave(mApplicationRealEstate.Main, mUserID);

                    retValue = this.ApplicationRealEstateCollateralDelete(mApplicationRealEstate.Main.APPLICATION_ID);
                    retValue = this.ApplicationRealEstateCollateralSave(mApplicationRealEstate.CollateralList, mUserID);
                }
                catch { }
            }
            else
            {
                retValue = true;
            }

            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationRealEstateMain ==
        #region ApplicationRealEstateMainDelete
        private bool ApplicationRealEstateMainDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_REAL_ESTATE_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationRealEstateMainGet
        private ApplicationRealEstateMain ApplicationRealEstateMainGet(int mApplicationId)
        {
            ApplicationRealEstateMain RetValue = new ApplicationRealEstateMain();

            string mSQL = String.Format("EXEC dbo.pAPPLICATION_REAL_ESTATE_GET @application_id = {0}", mApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            if (mDataTable.Rows.Count > 0)
            {
                RetValue = (ApplicationRealEstateMain)FillClass(RetValue, mDataTable.Rows[0]);
            }

            return RetValue;
        }
        #endregion

        #region ApplicationRealEstateMainSave
        private bool ApplicationRealEstateMainSave(ApplicationRealEstateMain mRealEstate, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_REAL_ESTATE_SET 
                 @rec_id = {0}, @application_id = {1}, @primary_amount = {2}, @cover_amount = {3},
	             @project_amount = {4}, @market_amount = {5}, @liquidation_rate = {6}, @liquidation_amount = {7},
	             @partipication_rate = {8}, @partipication_amount = {9}, @price_amount = {10}, @commission_rate = {11},
                 @commission_amount = {12}, @final_amount = {13}, @insurance_life_yn = {14}, @insurance_life_amount = {15}, @insurance_real_estae_yn = {16},
	             @insurance_real_estae_amount = {17}, @ltv_coefficient = {18}, @cltv_coefficient = {19}, @developer_discount_rate = {20},
	             @loan_amount = {21}, @loan_interest = {22}, @loan_period = {23}, @general_period = {24}, @general_amount = {25}, 
                 @comment = {26}, @insurance_company_id = {27}, @calc_interest = {28}, @is_offset = {29}, @offset_interest = {30}, @is_fast_disburst = {31}, 
                 @fast_disburst_fee = {32}, @project_iso = {33}, @user_id = {34}, @primary_amount_real = {35}"
            , mRealEstate.REC_ID, mRealEstate.APPLICATION_ID, mRealEstate.PRIMARY_AMOUNT, mRealEstate.COVER_AMOUNT
            , mRealEstate.PROJECT_AMOUNT, mRealEstate.MARKET_AMOUNT, mRealEstate.LIQUIDATION_RATE, mRealEstate.LIQUIDATION_AMOUNT
            , mRealEstate.PARTIPICATION_RATE, mRealEstate.PARTIPICATION_AMOUNT, mRealEstate.PRICE_AMOUNT, mRealEstate.COMMISSION_RATE
            , mRealEstate.COMMISSION_AMOUNT, mRealEstate.FINAL_AMOUNT, mRealEstate.INSURANCE_LIFE_YN, mRealEstate.INSURANCE_LIFE_AMOUNT, mRealEstate.INSURANCE_REAL_ESTAE_YN
            , mRealEstate.INSURANCE_REAL_ESTAE_AMOUNT, mRealEstate.LTV_COEFFICIENT, mRealEstate.CLTV_COEFFICIENT, mRealEstate.DEVELOPER_DISCOUNT_RATE
            , mRealEstate.LOAN_AMOUNT, mRealEstate.LOAN_INTEREST, mRealEstate.LOAN_PERIOD, mRealEstate.GENERAL_PERIOD, mRealEstate.GENERAL_AMOUNT
            , StringFunctions.SqlQuoted(mRealEstate.COMMENT, true), mRealEstate.INSURANCE_COMPANY_ID
            , mRealEstate.CALC_INTEREST, mRealEstate.IS_OFFSET, mRealEstate.OFFSET_INTEREST, mRealEstate.IS_FAST_DISBURST, mRealEstate.FAST_DISBURST_FEE
            , StringFunctions.SqlQuoted(mRealEstate.PROJECT_ISO, true), user_id, mRealEstate.PRIMARY_AMOUNT_REAL);

            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationRealEstateCollateral ==
        #region ApplicationRealEstateCollateralDelete
        private bool ApplicationRealEstateCollateralDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_REAL_ESTATE_COLLATERAL_DEL @application_id = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationRealEstateCollateralGet
        private List<ApplicationRealEstateCollateral> ApplicationRealEstateCollateralGet(int mApplicationId)
        {
            List<ApplicationRealEstateCollateral> mApplicationRealEstateCollateralList = new List<ApplicationRealEstateCollateral>();

            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_REAL_ESTATE_COLLATERAL_GET @application_id = {0}", mApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationRealEstateCollateral mClass = new ApplicationRealEstateCollateral();
                mClass = (ApplicationRealEstateCollateral)FillClass(mClass, mDataRow);
                mApplicationRealEstateCollateralList.Add(mClass);
            }

            return mApplicationRealEstateCollateralList;
        }
        #endregion

        #region ApplicationRealEstateCollateralSave
        private bool ApplicationRealEstateCollateralSave(List<ApplicationRealEstateCollateral> mCollatrealList, int user_id)
        {
            bool retValue = false;

            DataSet mDataSet;
            foreach (ApplicationRealEstateCollateral mCollateral in mCollatrealList)
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_REAL_ESTATE_COLLATERAL_SET
	                @application_id = {0}, @real_estate_type_id = {1}, @owner_personal_id = {2}, @owner_name = {3},
	                @cadastre_code = {4}, @real_estate_address = {5}, @real_estate_area = {6}, @is_used = {7},
	                @used_amount = {8}, @is_cover = {9}, @market_amount_usd = {10}, @liquidation_rate = {11},
	                @liquidation_amount_usd = {12}, @market_amount = {13}, @liquidation_amount = {14}, @comment = {15}, 
                    @appraiser_name = {16}, @valuation_date = {17}, @extract_date = {18}, @registration_number = {19}, 
                    @registration_date = {20}, @active_space = {21}, @city_name = {22}, @share_count = {23}, 
                    @floor_level = {24}, @apartment_number = {25},@real_estate_owner_type_id = {26}, @user_id = {27}"
                , mCollateral.APPLICATION_ID
                , StringFunctions.SqlQuoted(mCollateral.REAL_ESTATE_TYPE_ID, true)
                , StringFunctions.SqlQuoted(mCollateral.OWNER_PERSONAL_ID, true)
                , StringFunctions.SqlQuoted(mCollateral.OWNER_NAME, true)
                , StringFunctions.SqlQuoted(mCollateral.CADASTRE_CODE, true)
                , StringFunctions.SqlQuoted(mCollateral.REAL_ESTATE_ADDRESS, true)
                , mCollateral.REAL_ESTATE_AREA
                , mCollateral.IS_USED
                , mCollateral.USED_AMOUNT
                , mCollateral.IS_COVER
                , mCollateral.MARKET_AMOUNT_USD
                , mCollateral.LIQUIDATION_RATE
                , mCollateral.LIQUIDATION_AMOUNT_USD
                , mCollateral.MARKET_AMOUNT
                , mCollateral.LIQUIDATION_AMOUNT
                , StringFunctions.SqlQuoted(mCollateral.COMMENT, true)
                , StringFunctions.SqlQuoted(mCollateral.APPRAISER_NAME, true)
                , StringFunctions.SqlDateQuoted(mCollateral.VALUATION_DATE)
                , StringFunctions.SqlDateQuoted(mCollateral.EXTRACT_DATE)
                , StringFunctions.SqlQuoted(mCollateral.REGISTRATION_NUMBER, true)
                , StringFunctions.SqlDateQuoted(mCollateral.REGISTRATION_DATE)
                , mCollateral.ACTIVE_SPACE
                , StringFunctions.SqlQuoted(mCollateral.CITY_NAME, true)
                , mCollateral.SHARE_COUNT
                , mCollateral.FLOOR_LEVEL
                , StringFunctions.SqlQuoted(mCollateral.APARTMENT_NUMBER, true)
                , mCollateral.REAL_ESTATE_OWNER_TYPE_ID
                , user_id
                );

                mDataSet = _DGate.GetDataSet(mSQL);
                if (mDataSet != null && mDataSet.Tables != null && mDataSet.Tables[0].Rows.Count > 0)
                {
                    retValue = true;
                }

            }
            return retValue;
        }
        #endregion
        #endregion
        #endregion

        #region == ApplicationRecomender ==
        #region ApplicationRecomenderDelete
        [WebMethod]

        public bool ApplicationRecomenderDelete(int application_id, int recomender_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_RECOMENDER_DEL @application_id = {0},@recomender_id = {1}", application_id, recomender_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationRecomenderList
        [WebMethod]

        public List<ApplicationRecomender> ApplicationRecomenderList(int RecomenderId, int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_RECOMENDER_GET @recomender_id = {0} ,@application_id = {1}", RecomenderId, ApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationRecomender> RetList = new List<ApplicationRecomender>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationRecomender mClass = new ApplicationRecomender();
                mClass = (ApplicationRecomender)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationRecomenderSave
        [WebMethod]

        public bool ApplicationRecomenderSave(ApplicationRecomender _class, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_RECOMENDER_SET @recomender_id = {0},@application_id = {1},@recommendator_name = {2},@recommendator_surname = {3},@recommendator_phone = {4},@recommendator_mobile = {5},@recommendator_relationship_type_id = {6},@user_id = {7}"
            , _class.RECOMENDER_ID
            , _class.APPLICATION_ID
            , StringFunctions.SqlQuoted(_class.RECOMMENDATOR_NAME, true)
            , StringFunctions.SqlQuoted(_class.RECOMMENDATOR_SURNAME, true)
            , StringFunctions.SqlQuoted(_class.RECOMMENDATOR_PHONE, true)
            , StringFunctions.SqlQuoted(_class.RECOMMENDATOR_MOBILE, true)
            , _class.RECOMMENDATOR_RELATIONSHIP_TYPE_ID
            , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationRecomenderListSave
        [WebMethod]

        public bool ApplicationRecomenderListSave(List<ApplicationRecomender> _classList, int user_id, int application_id)
        {
            bool retValue = true;
            try
            {
                ApplicationRecomenderDelete(application_id, 0);
                foreach (ApplicationRecomender _class in _classList)
                {
                    ApplicationRecomenderSave(_class, user_id);
                }
            }
            catch { }
            return retValue;
        }
        #endregion
        #endregion

        #region == ApplicationState ==
        #region ApplicationStateList
        [WebMethod]

        public List<PipelineWS.LocalModel.ApplicationState> ApplicationStateList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_APPLICATION_STATE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.ApplicationState> RetList = new List<PipelineWS.LocalModel.ApplicationState>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.ApplicationState mClass = new PipelineWS.LocalModel.ApplicationState();
                mClass = (PipelineWS.LocalModel.ApplicationState)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationStateMapList
        [WebMethod]

        public List<ApplicationStateMap> ApplicationStateMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_APPLICATION_STATE_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationStateMap> RetList = new List<ApplicationStateMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationStateMap mClass = new ApplicationStateMap();
                mClass = (ApplicationStateMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationStateWorkPlaceToMapList
        [WebMethod]

        public List<ApplicationStateWorkPlaceToMap> ApplicationStateWorkPlaceToMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_APPLICATION_STATE_WORK_PLACE_TO");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationStateWorkPlaceToMap> RetList = new List<ApplicationStateWorkPlaceToMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationStateWorkPlaceToMap mClass = new ApplicationStateWorkPlaceToMap();
                mClass = (ApplicationStateWorkPlaceToMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationStateVisibilityMapList
        [WebMethod]

        public List<StateVisibilityMap> StateVisibilityMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_STATE_VISIBILITY");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<StateVisibilityMap> RetList = new List<StateVisibilityMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                StateVisibilityMap mClass = new StateVisibilityMap();
                mClass = (StateVisibilityMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ProductRestructMapList
        [WebMethod]
        public List<ProductRestructMap> ProductRestructMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_PRODUCT_RESTRUCT_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductRestructMap> RetList = new List<ProductRestructMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductRestructMap mClass = new ProductRestructMap();
                mClass = (ProductRestructMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #endregion

        #region ApplicationException
        [WebMethod]

        public List<LoanProductException> LoanExceptionList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_EXCEPTION_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LoanProductException> RetList = new List<LoanProductException>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanProductException mClass = new LoanProductException();
                mClass = (LoanProductException)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]

        public List<LoanProductExceptionParam> LoanProductExceptionParamList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_EXCEPTION_PARAM_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LoanProductExceptionParam> RetList = new List<LoanProductExceptionParam>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanProductExceptionParam mClass = new LoanProductExceptionParam();
                mClass = (LoanProductExceptionParam)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]

        public LoanProductExceptionValue LoanProductExceptionValues(string mStoredProcedure)
        {
            string mSQL = String.Format(mStoredProcedure);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            LoanProductExceptionValue mLoanProductExceptionValue = new LoanProductExceptionValue();
            mLoanProductExceptionValue.EXCEPTION_VALUE_FROM = Convert.ToDecimal(mDataTable.Rows[0][0]);
            mLoanProductExceptionValue.EXCEPTION_VALUE_TO = Convert.ToDecimal(mDataTable.Rows[0][1]);

            return mLoanProductExceptionValue;
        }
        #endregion

        #region ApplicationPurchaseItems

        #region ApplicationPurchaseItemsList
        [WebMethod]
        public List<ApplicationPurchaseItem> ApplicationPurchaseItemList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_PURCHASE_ITEMS_GET @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationPurchaseItem> RetList = new List<ApplicationPurchaseItem>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationPurchaseItem mClass = new ApplicationPurchaseItem();
                mClass = (ApplicationPurchaseItem)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationPurchaseItemsDel
        [WebMethod]
        public bool ApplicationPurchaseItemDel(int application_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_PURCHASE_ITEMS_DEL @application_id = {0}", application_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationPurchaseItemsDel

        #region ApplicationPurchaseItemsSet
        [WebMethod]
        public bool ApplicationPurchaseItemSet(ApplicationPurchaseItem _class)
        {
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_PURCHASE_ITEMS_SET 
                          @application_id = {0}, @item_group_id = {1}, @item_id = {2}
                        , @item_descrip = {3}, @item_amount = {4}, @item_amount_real = {5}"
                        , _class.APPLICATION_ID
                        , _class.ITEM_GROUP_ID
                        , _class.ITEM_ID
                        , StringFunctions.SqlQuoted(_class.ITEM_DESCRIP, true)
                        , _class.ITEM_AMOUNT
                        , _class.ITEM_AMOUNT_REAL
                    );
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationPurchaseItemsSet

        #region ApplicationPurchaseItemsListSave
        [WebMethod]
        public bool ApplicationPurchaseItemListSave(int application_id, List<ApplicationPurchaseItem> ApplicationPurchaseItemList)
        {
            this.ApplicationPurchaseItemDel(application_id);
            ApplicationPurchaseItemList.ForEach(item =>
            {
                this.ApplicationPurchaseItemSet(item);
            }
                );

            return true;
        }
        #endregion ApplicationPurchaseItemsListSave


        #endregion ApplicationPurchaseItems

        #region ApplicationPurchase

        #region ApplicationPurchaseGet
        [WebMethod]
        public ApplicationPurchase ApplicationPurchaseGet(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_PURCHASE_GET @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            ApplicationPurchase mClass = new ApplicationPurchase();
            try
            {
                mClass = (ApplicationPurchase)FillClass(mClass, mDataTable.Rows[0]);
            }
            catch { }
            return mClass;
        }
        #endregion ApplicationPurchaseGet

        #region ApplicationPurchaseDel
        [WebMethod]
        public bool ApplicationPurchaseDel(int application_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_PURCHASE_DEL @application_id = {0}", application_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationPurchaseDel

        #region ApplicationPurchaseSet
        [WebMethod]
        public bool ApplicationPurchaseSet(ApplicationPurchase _class)
        {
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_PURCHASE_SET 
                      @application_id = {0}, @installment_id = {1}, @product_id = {2}, @loan_currency = {3}
                    , @product_category_id = {4}, @participation_rate = {5}, @total_item_amount = {6}, @total_item_amount_real = {7}
                    , @participation_amount = {8}, @payd_amount = {9}, @loan_amount = {10}, @loan_interest = {11}
                    , @loan_period = {12}, @discount_amount = {13}, @commission_amount = {14}, @amount_per_month = {15}
                    , @over_pay_amount = {16}"
                    , _class.APPLICATION_ID
                    , _class.INSTALLMENT_ID
                    , _class.PRODUCT_ID
                    , StringFunctions.SqlQuoted(_class.LOAN_CURRENCY)
                    , _class.PRODUCT_CATEGORY_ID
                    , _class.PARTICIPATION_RATE
                    , _class.TOTAL_ITEM_AMOUNT
                    , _class.TOTAL_ITEM_AMOUNT_REAL
                    , _class.PARTICIPATION_AMOUNT
                    , _class.PAYD_AMOUNT
                    , _class.LOAN_AMOUNT
                    , _class.LOAN_INTEREST
                    , _class.LOAN_PERIOD
                    , _class.DISCOUNT_AMOUNT
                    , _class.COMMISSION_AMOUNT
                    , _class.AMOUNT_PER_MONTH
                    , _class.OVER_PAY_AMOUNT
                    );
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationPurchaseSet

        #region ApplicationPurchaseSave
        [WebMethod]
        public bool ApplicationPurchaseSave(int application_id, ApplicationPurchase mApplicationPurchase, List<ApplicationPurchaseItem> mApplicationPurchaseItemList)
        {
            this.ApplicationPurchaseSet(mApplicationPurchase);
            this.ApplicationPurchaseItemListSave(application_id, mApplicationPurchaseItemList);
            return true;
        }
        #endregion ApplicationPurchaseSave

        #region ApplicationPurchaseFull
        [WebMethod]
        public ApplicationPurchaseFull ApplicationPurchaseFullGet(int application_id)
        {
            ApplicationPurchaseFull mApplicationPurchaseFull = new ApplicationPurchaseFull();
            mApplicationPurchaseFull.ApplicationPurchase = this.ApplicationPurchaseGet(application_id);
            mApplicationPurchaseFull.ApplicationPurchaseItemList = this.ApplicationPurchaseItemList(application_id);

            return mApplicationPurchaseFull;
        }

        #endregion ApplicationPurchaseFull

        #endregion ApplicationPurchase

        #region ApplicationClientVerifyDetail

        #region ApplicationClientVerifyDetailList
        [WebMethod]
        public List<ApplicationClientVerifyDetail> ApplicationClientVerifyDetailList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_VERIFY_DET_LIST @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationClientVerifyDetail> RetList = new List<ApplicationClientVerifyDetail>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationClientVerifyDetail mClass = new ApplicationClientVerifyDetail();
                mClass = (ApplicationClientVerifyDetail)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ApplicationClientVerifyDetailDel
        [WebMethod]
        public bool ApplicationClientVerifyDetailDel(int application_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_VERIFY_DET_DEL @application_id = {0}", application_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationClientVerifyDetailDel

        #region ApplicationClientVerifyDetailSet
        [WebMethod]
        public bool ApplicationClientVerifyDetailSet(ApplicationClientVerifyDetail _class)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_VERIFY_DET_SET  @application_id = {0},@relationship_id = {1},@person_name = {2},@person_phone = {3},@phone_verify = {4},@phone_time = {5},@record_type = {6}"
                    , _class.APPLICATION_ID
                    , _class.RELATIONSHIP_ID
                    , StringFunctions.SqlQuoted(_class.PERSON_NAME, true)
                    , StringFunctions.SqlQuoted(_class.PERSON_PHONE, true)
                    , _class.PHONE_VERIFY
                    , StringFunctions.SqlDateTimeQuoted(_class.PHONE_TIME)
                    , _class.RECORD_TYPE
                    );
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationClientVerifyDetailSet

        #region ApplicationClientVerifyDetailListSave
        [WebMethod]
        public bool ApplicationClientVerifyDetailListSave(int application_id, List<ApplicationClientVerifyDetail> mApplicationClientVerifyDetailList)
        {
            this.ApplicationClientVerifyDetailDel(application_id);
            mApplicationClientVerifyDetailList.ForEach(item =>
            {
                this.ApplicationClientVerifyDetailSet(item);
            }
                );

            return true;
        }
        #endregion ApplicationClientVerifyDetailListSave


        #endregion ApplicationClientVerifyDetail

        #region ApplicationClientVerify

        #region ApplicationClientVerifyGet
        [WebMethod]
        public ApplicationClientVerify ApplicationClientVerifyGet(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_VERIFY_GET @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            ApplicationClientVerify mClass = new ApplicationClientVerify();
            try
            {
                mClass = (ApplicationClientVerify)FillClass(mClass, mDataTable.Rows[0]);
            }
            catch { }
            return mClass;
        }
        #endregion ApplicationClientVerifyGet

        #region ApplicationClientVerifyDel
        [WebMethod]
        public bool ApplicationClientVerifyDel(int application_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_VERIFY_DEL @application_id = {0}", application_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationClientVerifyDel

        #region ApplicationClientVerifySet
        [WebMethod]
        public bool ApplicationClientVerifySet(ApplicationClientVerify _class)
        {
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_VERIFY_SET 
                        @application_id = {0}
                        ,@work_org_id = {1}
                        ,@work_verify = {2}
                        ,@work_time = {3}
                        ,@post_name = {4}
                        ,@post_verify = {5}
                        ,@post_time = {6}
                        ,@income_amount = {7}
                        ,@income_amount_fact = {8}
                        ,@income_verify = {9}
                        ,@income_time = {10}
                        ,@experience = {11}
                        ,@experience_fact = {12}
                        ,@experience_verify = {13}
                        ,@experience_time = {14}
                        ,@fiz_debtor_verify = {15}
                        ,@fiz_debtor_time = {16}
                        ,@fiz_tax_pledge_verify = {17}
                        ,@fiz_tax_pledge_time = {18}
                        ,@fiz_commercial_verify = {19}
                        ,@fiz_commercial_time = {20}
                        ,@juridical_debtor_verify = {21}
                        ,@juridical_debtor_time = {22}
                        ,@juridical_tax_pledge_verify = {23}
                        ,@juridical_tax_pledge_time = {24}
                        ,@juridical_commercial_verify = {25}
                        ,@juridical_commercial_time = {26}
                        ,@remark = {27}
                        ,@is_not_automatic = {28}"
                    , _class.APPLICATION_ID
                    , _class.WORK_ORG_ID
                    , _class.WORK_VERIFY
                    , StringFunctions.SqlQuoted(_class.WORK_TIME)
                    , StringFunctions.SqlQuoted(_class.POST_NAME, true)
                    , _class.POST_VERIFY
                    , StringFunctions.SqlQuoted(_class.POST_TIME)
                    , _class.INCOME_AMOUNT
                    , _class.INCOME_AMOUNT_FACT
                    , _class.INCOME_VERIFY
                    , StringFunctions.SqlQuoted(_class.INCOME_TIME)
                    , _class.EXPERIENCE
                    , _class.EXPERIENCE_FACT
                    , _class.EXPERIENCE_VERIFY
                    , StringFunctions.SqlQuoted(_class.EXPERIENCE_TIME)
                    , _class.FIZ_DEBTOR_VERIFY
                    , StringFunctions.SqlQuoted(_class.FIZ_DEBTOR_TIME)
                    , _class.FIZ_TAX_PLEDGE_VERIFY
                    , StringFunctions.SqlQuoted(_class.FIZ_TAX_PLEDGE_TIME)
                    , _class.FIZ_COMMERCIAL_VERIFY
                    , StringFunctions.SqlQuoted(_class.FIZ_COMMERCIAL_TIME)

                    , _class.JURIDICAL_DEBTOR_VERIFY
                    , StringFunctions.SqlQuoted(_class.JURIDICAL_DEBTOR_TIME)
                    , _class.JURIDICAL_TAX_PLEDGE_VERIFY
                    , StringFunctions.SqlQuoted(_class.JURIDICAL_TAX_PLEDGE_TIME)
                    , _class.JURIDICAL_COMMERCIAL_VERIFY
                    , StringFunctions.SqlQuoted(_class.JURIDICAL_COMMERCIAL_TIME)

                    , StringFunctions.SqlQuoted(_class.REMARK, true)
                    , _class.IS_NOT_AUTOMATIC
                    );
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion ApplicationClientVerifySet

        #region ApplicationClientVerifySave
        [WebMethod]
        public bool ApplicationClientVerifySave(int application_id, ApplicationClientVerify mApplicationClientVerify, List<ApplicationClientVerifyDetail> mApplicationClientVerifyDetailList)
        {
            this.ApplicationClientVerifySet(mApplicationClientVerify);
            this.ApplicationClientVerifyDetailListSave(application_id, mApplicationClientVerifyDetailList);
            return true;
        }
        #endregion ApplicationClientVerifySave

        #region ApplicationClientVerifyFull
        [WebMethod]
        public ApplicationClientVerifyFull ApplicationClientVerifyFullGet(int application_id)
        {
            ApplicationClientVerifyFull mApplicationClientVerifyFull = new ApplicationClientVerifyFull();
            mApplicationClientVerifyFull.ApplicationClientVerify = this.ApplicationClientVerifyGet(application_id);
            mApplicationClientVerifyFull.ApplicationClientVerifyDetailList = this.ApplicationClientVerifyDetailList(application_id);

            return mApplicationClientVerifyFull;
        }

        #endregion ApplicationClientVerifyFull

        #endregion ApplicationClientVerify

        #region ApplicationEmail
        #region ApplicationEmailSet
        [WebMethod]
        public bool ApplicationEmailSet(int mApplication_id, int mEmailType_id)
        {
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_EMAIL_SET @application_id = {0},@email_type_id = {1}", mApplication_id, mEmailType_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            }
            catch
            { }
            return true;
        }
        #endregion ApplicationEmailSet
        #endregion ApplicationEmail

        #region ApplicationSMS
        #region ApplicationSMSSet
        [WebMethod]
        public bool ApplicationSMSSet(int mApplication_id, int mSMSType_id)
        {
            bool result = false;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_SEND_SMS @application_id = {0},@sms_type_id = {1},@message = NULL,@need_rs = 1", mApplication_id, mSMSType_id);
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToBoolean(mDataTable.Rows[0]["ANSWER"]))
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        #endregion ApplicationSMSSet
        #endregion ApplicationSMS


        #region ApplicationBlackList
        [WebMethod]

        public List<ApplicationBlackList> ApplicationBlackListList(int APPLICATION_ID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_BLACK_LIST @APPLICATION_ID = {0}", APPLICATION_ID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationBlackList> RetList = new List<ApplicationBlackList>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationBlackList mClass = new ApplicationBlackList();
                mClass = (ApplicationBlackList)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationBlackList

        #region ApplicationRelated
        [WebMethod]

        public List<ApplicationRelated> ApplicationRelatedList(int APPLICATION_ID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_RELATED_LIST @APPLICATION_ID = {0}", APPLICATION_ID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationRelated> RetList = new List<ApplicationRelated>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationRelated mClass = new ApplicationRelated();
                mClass = (ApplicationRelated)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationRelated

        #region == CashCover ==

        #region =CashCoverGet=
        [WebMethod]
        public CashCover CashCoverGet(string mPersonalId)
        {
            CashCover mCashCover = new CashCover();
            List<CashCoverClient> mCashCoverClientList = new List<CashCoverClient>();
            List<CashCoverDeposit> mCashCoverDepositList = new List<CashCoverDeposit>();
            List<CashCoverDepoLoanMap> mCashCoverDepoLoanMapList = new List<CashCoverDepoLoanMap>();
            List<CashCoverLoan> mCashCoverLoanList = new List<CashCoverLoan>();
            CashCoverAttributes mCashCoverAttributes = new CashCoverAttributes();

            string mSQL = String.Format("EXEC dbo.pCASH_COVER_DEPO_LOAN_LIST @PERSONAL_ID = {0}", StringFunctions.SqlQuoted(mPersonalId, true));

            // == 1. CashCoverClient
            DataSet mDs = _DGate.GetDataSet(mSQL, 300);
            mDataTable = mDs.Tables[0];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverClient mClass = new CashCoverClient();
                mClass = (CashCoverClient)FillClass(mClass, mDataRow);
                mCashCoverClientList.Add(mClass);
            }

            // == 2. CashCoverDeposit
            mDataTable = mDs.Tables[1];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverDeposit mClass = new CashCoverDeposit();
                mClass = (CashCoverDeposit)FillClass(mClass, mDataRow);
                mCashCoverDepositList.Add(mClass);
            }

            // == 3. CashCoverDepoLoanMap
            mDataTable = mDs.Tables[2];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverDepoLoanMap mClass = new CashCoverDepoLoanMap();
                mClass = (CashCoverDepoLoanMap)FillClass(mClass, mDataRow);
                mCashCoverDepoLoanMapList.Add(mClass);
            }

            // == 4. CashCoverLoan
            mDataTable = mDs.Tables[3];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverLoan mClass = new CashCoverLoan();
                mClass = (CashCoverLoan)FillClass(mClass, mDataRow);
                mCashCoverLoanList.Add(mClass);
            }

            mCashCover.CashCoverClientList = mCashCoverClientList;
            mCashCover.CashCoverDepositList = mCashCoverDepositList;
            mCashCover.CashCoverDepoLoanMapList = mCashCoverDepoLoanMapList;
            mCashCover.CashCoverLoanList = mCashCoverLoanList;

            return mCashCover;
        }

        [WebMethod]
        public CashCover AplicationCashCoverGet(int mApplicationID)
        {
            CashCover mCashCover = new CashCover();
            List<CashCoverClient> mCashCoverClientList = new List<CashCoverClient>();
            List<CashCoverDeposit> mCashCoverDepositList = new List<CashCoverDeposit>();
            List<CashCoverDepoLoanMap> mCashCoverDepoLoanMapList = new List<CashCoverDepoLoanMap>();
            List<CashCoverLoan> mCashCoverLoanList = new List<CashCoverLoan>();
            CashCoverAttributes mCashCoverAttributes = new CashCoverAttributes();

            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_GET @APPLICATION_ID = {0}", mApplicationID);


            DataSet mDs = _DGate.GetDataSet(mSQL, 300);

            // == 1. CashCoverAttributes
            mDataTable = mDs.Tables[0];
            if (mDataTable.Rows.Count > 0)
            {
                mCashCoverAttributes = (CashCoverAttributes)FillClass(mCashCoverAttributes, mDataTable.Rows[0]);
            }
            mCashCover.CashCoverAttributes = mCashCoverAttributes;
            // == 2. CashCoverClient
            mDataTable = mDs.Tables[1];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverClient mClass = new CashCoverClient();
                mClass = (CashCoverClient)FillClass(mClass, mDataRow);
                mCashCoverClientList.Add(mClass);
            }

            // == 3. CashCoverDeposit
            mDataTable = mDs.Tables[2];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverDeposit mClass = new CashCoverDeposit();
                mClass = (CashCoverDeposit)FillClass(mClass, mDataRow);
                mCashCoverDepositList.Add(mClass);
            }

            // == 4. CashCoverDepoLoanMap
            mDataTable = mDs.Tables[3];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverDepoLoanMap mClass = new CashCoverDepoLoanMap();
                mClass = (CashCoverDepoLoanMap)FillClass(mClass, mDataRow);
                mCashCoverDepoLoanMapList.Add(mClass);
            }

            // == 5. CashCoverLoan
            mDataTable = mDs.Tables[4];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverLoan mClass = new CashCoverLoan();
                mClass = (CashCoverLoan)FillClass(mClass, mDataRow);
                mCashCoverLoanList.Add(mClass);
            }

            mCashCover.CashCoverClientList = mCashCoverClientList;
            mCashCover.CashCoverDepositList = mCashCoverDepositList;
            mCashCover.CashCoverDepoLoanMapList = mCashCoverDepoLoanMapList;
            mCashCover.CashCoverLoanList = mCashCoverLoanList;

            return mCashCover;
        }


        [WebMethod]
        public CashCover AplicationCashCoverLinkedAmountGet(int mApplicationID)
        {
            CashCover mCashCover = new CashCover();
            List<CashCoverClient> mCashCoverClientList = new List<CashCoverClient>();
            List<CashCoverDeposit> mCashCoverDepositList = new List<CashCoverDeposit>();
            List<CashCoverDepoLoanMap> mCashCoverDepoLoanMapList = new List<CashCoverDepoLoanMap>();
            List<CashCoverLoan> mCashCoverLoanList = new List<CashCoverLoan>();
            CashCoverAttributes mCashCoverAttributes = new CashCoverAttributes();

            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_LINKED_AMOUNT_GET @APPLICATION_ID = {0}", mApplicationID);


            DataSet mDs = _DGate.GetDataSet(mSQL, 300);

            // == 1. CashCoverDeposit
            mDataTable = mDs.Tables[0];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverDeposit mClass = new CashCoverDeposit();
                mClass = (CashCoverDeposit)FillClass(mClass, mDataRow);
                mCashCoverDepositList.Add(mClass);
            }

            // == 2. CashCoverDepoLoanMap
            mDataTable = mDs.Tables[1];
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverDepoLoanMap mClass = new CashCoverDepoLoanMap();
                mClass = (CashCoverDepoLoanMap)FillClass(mClass, mDataRow);
                mCashCoverDepoLoanMapList.Add(mClass);
            }

            mCashCover.CashCoverClientList = mCashCoverClientList;
            mCashCover.CashCoverDepositList = mCashCoverDepositList;
            mCashCover.CashCoverDepoLoanMapList = mCashCoverDepoLoanMapList;
            mCashCover.CashCoverLoanList = mCashCoverLoanList;

            return mCashCover;
        }

        #endregion =CashCoverGet=

        #region =CashCoverDelete=
        public bool CashCoverDelete(int mApplicationID, CashCover mCashCover)
        {
            bool mResult = true;
            this.CashCoverClientListDelete(mApplicationID);
            this.CashCoverDepositListDelete(mApplicationID);
            this.CashCoverDepoLoanMapListDelete(mApplicationID);
            this.CashCoverLoanListDelete(mApplicationID);
            this.CashCoverAttributesDelete(mApplicationID);
            return mResult;
        }
        #endregion =CashCoverDelete=

        #region =CashCoverSet=
        [WebMethod]
        public bool CashCoverSet(CashCover mCashCover)
        {
            int mApplicationID = mCashCover.CashCoverAttributes.APPLICATION_ID;
            this.CashCoverDelete(mApplicationID, mCashCover);
            bool mResult = true;

            this.CashCoverClientListSet(mApplicationID, mCashCover.CashCoverClientList);
            this.CashCoverDepositListSet(mApplicationID, mCashCover.CashCoverDepositList);
            this.CashCoverDepoLoanMapListSet(mApplicationID, mCashCover.CashCoverDepoLoanMapList);
            this.CashCoverLoansListSet(mApplicationID, mCashCover.CashCoverLoanList);
            this.CashCoverAttributesSet(mApplicationID, mCashCover.CashCoverAttributes);
            return mResult;
        }
        #endregion =CashCoverSet=

        #region =CashCoverClients=
        public bool CashCoverClientDelete(int mApplicationID)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_CLIENT_DEL @APPLICATION_ID  = {0}", mApplicationID);
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }

        public bool CashCoverClientSet(int mApplicationID, CashCoverClient mCashCoverClients)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_CLIENT_SET @APPLICATION_ID  = {0},@CLIENT_NO = {1}, @CLIENT_NAME = {2} , @PERSONAL_ID = {3},@IS_VIP_CLIENT = {4}"
                    , mApplicationID
                    , mCashCoverClients.CLIENT_NO
                    , StringFunctions.SqlQuoted(mCashCoverClients.CLIENT_NAME, true)
                    , StringFunctions.SqlQuoted(mCashCoverClients.PERSONAL_ID, true)
                    , Convert.ToInt32(mCashCoverClients.IS_VIP_CLIENT)
                    );
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverClientListDelete(int mApplicationID)
        {
            bool mResult = true;
            this.CashCoverClientDelete(mApplicationID);
            return mResult;
        }
        public bool CashCoverClientListSet(int mApplicationID, List<CashCoverClient> mCashCoverClientsList)
        {
            bool mResult = true;
            foreach (CashCoverClient mCashCoverClients in mCashCoverClientsList)
            {
                this.CashCoverClientSet(mApplicationID, mCashCoverClients);
            }
            return mResult;
        }
        #endregion =CashCoverClients=

        #region =CashCoverDeposits=
        public bool CashCoverDepositDelete(int mApplicationID)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_DEPOSIT_DEL @APPLICATION_ID  = {0}", mApplicationID);
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverDepositSet(int mApplicationID, CashCoverDeposit mCashCoverDeposits)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CASH_COVER_DEPOSIT_SET @APPLICATION_ID  = {0},@DEPO_ID = {1}, @DEPO_CLIENT_ID = {2}, @DEPO_PROD_ID = {3}
                    , @DEPO_CCY = {4}, @DEPO_AMOUNT = {5}, @DEPO_INTEREST_AMOUNT = {6}, @DEPO_START_DATE = {7}, @DEPO_END_DATE = {8}, @DEPO_INTEREST_RATE = {9}, @IS_DEPO_USED = {10},@DEPO_AGREEMENT_NO = {11}"
                    , mApplicationID
                    , mCashCoverDeposits.DEPO_ID
                    , mCashCoverDeposits.DEPO_CLIENT_ID
                    , mCashCoverDeposits.DEPO_PROD_ID
                    , StringFunctions.SqlQuoted(mCashCoverDeposits.DEPO_CCY, true)
                    , mCashCoverDeposits.DEPO_AMOUNT
                    , mCashCoverDeposits.DEPO_INTEREST_AMOUNT
                    , StringFunctions.SqlDateQuoted(mCashCoverDeposits.DEPO_START_DATE)
                    , StringFunctions.SqlDateQuoted(mCashCoverDeposits.DEPO_END_DATE)
                    , mCashCoverDeposits.DEPO_INTEREST_RATE
                    , (mCashCoverDeposits.IS_DEPO_USED) ? 1 : 0
                    , StringFunctions.SqlQuoted(mCashCoverDeposits.DEPO_AGREEMENT_NO, true)
                    );
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverDepositListDelete(int mApplicationID)
        {
            bool mResult = true;
            this.CashCoverDepositDelete(mApplicationID);
            return mResult;
        }
        public bool CashCoverDepositListSet(int mApplicationID, List<CashCoverDeposit> mCashCoverDepositsList)
        {
            bool mResult = true;
            foreach (CashCoverDeposit mCashCoverDeposits in mCashCoverDepositsList)
            {
                this.CashCoverDepositSet(mApplicationID, mCashCoverDeposits);
            }
            return mResult;
        }
        #endregion  =CashCoverDeposits=

        #region =CashCoverDepoLoanMap=
        public bool CashCoverDepoLoanMapDelete(int mApplicationID)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_DEPO_LOAN_MAP_DEL @APPLICATION_ID  = {0}", mApplicationID);
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverDepoLoanMapSet(int mApplicationID, CashCoverDepoLoanMap mCashCoverDepoLoanMap)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CASH_COVER_DEPO_LOAN_MAP_SET @APPLICATION_ID  = {0}, @LOAN_ID = {1}, @LOAN_CLIENT_ID = {2}, @LOAN_AMOUNT = {3}, @LOAN_DEBT_AMOUNT = {4}
                    , @LOAN_CCY = {5}, @LOAN_END_DATE = {6} , @DEPO_ID = {7}, @DEPO_OWNER_ID = {8}, @DEPO_AMOUNT = {9}, @DEPO_CCY = {10}"
                    , mApplicationID
                    , mCashCoverDepoLoanMap.LOAN_ID
                    , mCashCoverDepoLoanMap.LOAN_CLIENT_ID
                    , mCashCoverDepoLoanMap.LOAN_AMOUNT
                    , mCashCoverDepoLoanMap.LOAN_DEBT_AMOUNT
                    , StringFunctions.SqlQuoted(mCashCoverDepoLoanMap.LOAN_CCY, true)
                    , StringFunctions.SqlDateQuoted(mCashCoverDepoLoanMap.LOAN_END_DATE)
                    , mCashCoverDepoLoanMap.DEPO_ID
                    , mCashCoverDepoLoanMap.DEPO_OWNER_ID
                    , mCashCoverDepoLoanMap.DEPO_AMOUNT
                    , StringFunctions.SqlQuoted(mCashCoverDepoLoanMap.DEPO_CCY, true)
                    );
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverDepoLoanMapListDelete(int mApplicationID)
        {
            bool mResult = true;
            this.CashCoverDepoLoanMapDelete(mApplicationID);
            return mResult;
        }
        public bool CashCoverDepoLoanMapListSet(int mApplicationID, List<CashCoverDepoLoanMap> mCashCoverDepoLoanMapList)
        {
            bool mResult = true;
            foreach (CashCoverDepoLoanMap mCashCoverDepoLoanMap in mCashCoverDepoLoanMapList)
            {
                this.CashCoverDepoLoanMapSet(mApplicationID, mCashCoverDepoLoanMap);
            }
            return mResult;
        }
        #endregion =CashCoverLoans=

        #region =CashCoverLoans=
        public bool CashCoverLoanDelete(int mApplicationID)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_LOAN_DEL @APPLICATION_ID  = {0}", mApplicationID);
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverLoanSet(int mApplicationID, CashCoverLoan mCashCoverLoans)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CASH_COVER_LOAN_SET @APPLICATION_ID  = {0}, @LOAN_ID = {1}, @LOAN_CLIENT_ID = {2}, @LOAN_AMOUNT = {3}, @LOAN_DEBT_AMOUNT = {4}
                        , @LOAN_OVER_AMOUNT = {5}, @LOAN_CCY = {6}, @LOAN_END_DATE = {7}, @IS_LOAN_COVER = {8}, @DEPO_OWNER_ID = {9}, @LOAN_DEBT_AMOUNT_CUR = {10}, @LOAN_DEBT_AMOUNT_END = {11}"
                    , mApplicationID
                    , mCashCoverLoans.LOAN_ID
                    , mCashCoverLoans.LOAN_CLIENT_ID
                    , mCashCoverLoans.LOAN_AMOUNT
                    , mCashCoverLoans.LOAN_DEBT_AMOUNT
                    , mCashCoverLoans.LOAN_OVER_AMOUNT
                    , StringFunctions.SqlQuoted(mCashCoverLoans.LOAN_CCY, true)
                    , StringFunctions.SqlDateQuoted(mCashCoverLoans.LOAN_END_DATE)
                    , mCashCoverLoans.IS_LOAN_COVER ? 1 : 0
                    , mCashCoverLoans.DEPO_OWNER_ID
                    , mCashCoverLoans.LOAN_DEBT_AMOUNT_CUR
                    , mCashCoverLoans.LOAN_DEBT_AMOUNT_END
                    );
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverLoanListDelete(int mApplicationID)
        {
            bool mResult = true;
            this.CashCoverLoanDelete(mApplicationID);
            return mResult;
        }
        public bool CashCoverLoansListSet(int mApplicationID, List<CashCoverLoan> mCashCoverLoansList)
        {
            bool mResult = true;
            foreach (CashCoverLoan mCashCoverLoans in mCashCoverLoansList)
            {
                this.CashCoverLoanSet(mApplicationID, mCashCoverLoans);
            }
            return mResult;
        }
        #endregion =CashCoverLoans=

        #region =CashCoverAttributes=
        public bool CashCoverAttributesDelete(int mApplicationID)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_CASH_COVER_ATTRIBUTES_DEL @APPLICATION_ID  = {0}", mApplicationID);
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        public bool CashCoverAttributesSet(int mApplicationID, CashCoverAttributes mCashCoverAttributes)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CASH_COVER_ATTRIBUTES_SET @APPLICATION_ID  = {0}, @PRODUCT_ID = {1}, @SCHEDULE_TYPE_ID = {2} , @LOAN_CURRENCY = {3}, @USE_DEPO_INTEREST = {4}
                , @USE_MAX_LIMIT = {5}, @LOAN_AMOUNT = {6}, @LOAN_INTEREST_RATE = {7}, @LOAN_START_DATE = {8}, @LOAN_END_DATE = {9}, @LOAN_MAX_DATE = {10}"
                    , mApplicationID
                    , mCashCoverAttributes.PRODUCT_ID
                    , mCashCoverAttributes.SCHEDULE_TYPE_ID
                    , mCashCoverAttributes.LOAN_CURRENCY
                    , (mCashCoverAttributes.USE_DEPO_INTEREST) ? 1 : 0
                    , (mCashCoverAttributes.USE_MAX_LIMIT) ? 1 : 0
                    , mCashCoverAttributes.LOAN_AMOUNT
                    , mCashCoverAttributes.LOAN_INTEREST_RATE
                    , StringFunctions.SqlDateQuoted(mCashCoverAttributes.LOAN_START_DATE)
                    , StringFunctions.SqlDateQuoted(mCashCoverAttributes.LOAN_END_DATE)
                    , StringFunctions.SqlDateQuoted(mCashCoverAttributes.LOAN_MAX_DATE)
                    );
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }
        #endregion =CashCoverAttributes=

        #endregion == CashCover ==

        #region == M-File attributes ==

        [WebMethod]
        public List<MFileAttributes> ApplicationMFileAttributesList(int APPLICATION_ID)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_MFILE_ATTRIB_USING @APPLICATION_ID = {0}", APPLICATION_ID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<MFileAttributes> RetList = new List<MFileAttributes>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                MFileAttributes mClass = new MFileAttributes();
                mClass = (MFileAttributes)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        private string ApplicationMFileAttributeGet(int APPLICATION_ID, int ATTR_OBJECT_ID)
        {
            List<MFileAttributes> mMFileAttributesList = this.ApplicationMFileAttributesList(APPLICATION_ID);
            string mMFileAttribute = "";
            try
            {
                mMFileAttribute = mMFileAttributesList.First(x => x.ATTR_OBJECT_ID == ATTR_OBJECT_ID).DOC_ENTRY_NO;
            }
            catch { }
            return mMFileAttribute;
        }


        [WebMethod]
        public bool MFileAttributeSet(int mApplicationID, List<MFileAttributes> mMFileAttributesList)
        {
            bool mResult = true;

            try
            {
                this.MFileAttributeDelete(mApplicationID);

                foreach (MFileAttributes mMFileAttributes in mMFileAttributesList)
                {
                    this.MFileAttributeInsert(mMFileAttributes);
                }
            }
            catch
            {
                mResult = false;
            }

            return mResult;
        }

        private bool MFileAttributeDelete(int mApplicationID)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_MFILE_ATTRIB_DEL @APPLICATION_ID = {0}", mApplicationID);
                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }

        private bool MFileAttributeInsert(MFileAttributes mMFileAttributes)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_MFILE_ATTRIB_SET @APPLICATION_ID = {0}, @DOC_TYPE_ID = {1}, @DOC_ENTRY_NO = {2}",
                    mMFileAttributes.APPLICATION_ID,
                    mMFileAttributes.DOC_TYPE_ID,
                    StringFunctions.SqlQuoted(mMFileAttributes.DOC_ENTRY_NO, true));

                _DGate.GetDataSet(mSQL);
            }
            catch
            {
                mResult = false;
            }

            return mResult;
        }

        [WebMethod]
        public List<MFCaseNotCloseDet> GetCaseNotClosedDetList(List<MFCaseNotClosed> caseNotClosedList)
        {
            List<MFCaseNotCloseDet> RetList;
            string mSQL = "";

            if (caseNotClosedList.Count > 0)
            {
                //1. ძველი მონაცემების წაშლა
                MFCaseNotClosed mCaseNotClosed = caseNotClosedList.First();
                int mAdminID = mCaseNotClosed.AdminID;
                mSQL = String.Format("EXEC dbo.pMFILES_CASE_NOT_CLOSED_DELETE @ADMIN_ID = {0}", mAdminID);
                _DGate.GetDataSet(mSQL);

                //2. ახალი მონაცემების დამატება
                foreach (MFCaseNotClosed caseNot in caseNotClosedList)
                {
                    mSQL = String.Format("EXEC dbo.pMFILES_CASE_NOT_CLOSED_INSERT @ADMIN_ID = {0}, @APPLICATION_ID = {1}, @MFILE_CASE_ID = {2}",
                        mAdminID, caseNot.AppID, caseNot.CaseID);

                    _DGate.GetDataSet(mSQL, false);
                }
                _DGate.DisConnect();

                //3. დეტალური მონაცემების სია
                mSQL = String.Format("EXEC dbo.tMFILES_CASE_NOT_CLOSED_LIST @ADMIN_ID = {0}", mAdminID);
                DataTable mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

                RetList = new List<MFCaseNotCloseDet>();
                foreach (DataRow mDataRow in mDataTable.Rows)
                {
                    MFCaseNotCloseDet mClass = new MFCaseNotCloseDet();
                    mClass = (MFCaseNotCloseDet)FillClass(mClass, mDataRow);
                    RetList.Add(mClass);
                }
            }
            else
            {
                RetList = null;
            }

            return RetList;
        }

        #endregion == M-File attrib ==

        #region == ApplicationUniversity ==
        #region ApplicationUniversityDelete
        [WebMethod]
        public bool ApplicationUniversityDelete(int application_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_UNIVERSITY_DEL @APPLICATION_ID = {0}", application_id);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region ApplicationUniversityGet
        [WebMethod]
        public ApplicationUniversity ApplicationUniversityGet(int ApplicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_UNIVERSITY_GET @APPLICATION_ID = {0}", ApplicationId);
            ApplicationUniversity mClass = new ApplicationUniversity();
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            if (mDataTable.Rows.Count > 0)
            {
                DataRow mDataRow = mDataTable.Rows[0];
                mClass = (ApplicationUniversity)FillClass(mClass, mDataRow);
            }
            return mClass;
        }
        #endregion

        #region ApplicationUniversitySave
        [WebMethod]
        public bool ApplicationUniversitySave(ApplicationUniversity _class)
        {
            bool retValue = false;
            if (_class != null && _class.APPLICATION_ID > 0 && !String.IsNullOrEmpty(_class.UNIVERSITY_NAME))
            {
                string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_UNIVERSITY_SET @APPLICATION_ID = {0}, @UNIVERSITY_NAME = {1}, @FACULTY = {2}, @COURSE = {3}
                                , @DEBT_AMOUNT = {4}, @DEBT_DATE_START = {5}, @DEBT_DATE_END = {6}, @IS_ACADEMIC_DEBT = {7}"
                    , _class.APPLICATION_ID
                    , StringFunctions.SqlQuoted(_class.UNIVERSITY_NAME, true)
                    , StringFunctions.SqlQuoted(_class.FACULTY, true)
                    , _class.COURSE
                    , _class.DEBT_AMOUNT
                    , StringFunctions.SqlDateQuoted(_class.DEBT_DATE_START)
                    , StringFunctions.SqlDateQuoted(_class.DEBT_DATE_END)
                    , _class.IS_ACADEMIC_DEBT
                    );
                try
                {
                    DataSet _ds;
                    _ds = _DGate.GetDataSet(mSQL);
                    if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                    {
                        retValue = true;
                    }
                }
                catch (Exception ex)
                {
                    this.ApplicationLogSet(_class.APPLICATION_ID, 1, -1, ex.Message);
                }
            }
            else
            {
                retValue = true;
            }
            return retValue;
        }
        #endregion
        #endregion
        #endregion == Loan Application ==

        #region ==== out methods ====
        [WebMethod]

        public List<ClientApplicationState> ClientApplicationStateList(int clientNo, int sourceId)
        {
            string mSQL = String.Format("EXEC dbo.pCLIENT_APPLICATIONS_LIST @client_no = {0}, @source_id = {1}", clientNo, sourceId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientApplicationState> RetList = new List<ClientApplicationState>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientApplicationState mClass = new ClientApplicationState();
                mClass = (ClientApplicationState)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ==== out methods ====

        #region ==== Scoring ====

        #region ==Application Minimal Requests ==
        #region ApplicationMinimalRequest
        [WebMethod]
        public LoanApplicationRecord ApplicationMinimalRequest(int ApplicationId, int UserId)
        {
            return this.ApplicationMinimalRequestAuto(ApplicationId, UserId, false);
        }
        #endregion

        #region ApplicationMinimalRequestAuto
        [WebMethod]
        public LoanApplicationRecord ApplicationMinimalRequestAuto(int ApplicationId, int UserId, bool isAccelerator)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CHECK_MINIMAL_REQUESTS @application_id = {0},@user_id = {1}", ApplicationId, UserId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            LoanApplicationRecord mClass = new LoanApplicationRecord();
            mClass = (LoanApplicationRecord)FillClass(mClass, mDataTable.Rows[0]);
            if (!isAccelerator)
            {
                return mClass;
            }

            this.PostAccelerator(ApplicationId, UserId, "");
            return this.ApplicationRec(ApplicationId, UserId);
        }
        #endregion

        #region ApplicationStopFactorCheckLogList
        [WebMethod]

        public List<ApplicationStopFactorCheckLog> ApplicationStopFactorCheckLogList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_STOP_FACTOR_CHECK_LOG @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationStopFactorCheckLog> RetList = new List<ApplicationStopFactorCheckLog>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationStopFactorCheckLog mClass = new ApplicationStopFactorCheckLog();
                mClass = (ApplicationStopFactorCheckLog)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion
        #endregion

        #region == Application Scoring ==

        #region ApplicationScoring
        [WebMethod]
        public bool ApplicationScoring(int ApplicationId, int UserId)
        {
            bool mRetValue = false;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_SCORING_CALC_ALL @APPLICATION_ID = {0}, @USER_ID = {1}", ApplicationId, UserId);
                DataSet mDS = _DGate.GetDataSet(mSQL);
                mRetValue = true;
            }
            catch { }
            return mRetValue;
        }

        #endregion
        

        #region ApplicationScoringList
        [WebMethod]
        public List<ApplicationScoring> ApplicationScoringList(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_SCORING_LOG_GET @APPLICATION_ID = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ApplicationScoring> RetList = new List<ApplicationScoring>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationScoring mClass = new ApplicationScoring();
                mClass = (ApplicationScoring)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region ScoringParamCoefList
        [WebMethod]

        public List<ScoringParamCoef> ScoringParamCoefList()
        {
            string mSQL = String.Format("EXEC dbo.pSCORING_PARAM_KOEF");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ScoringParamCoef> RetList = new List<ScoringParamCoef>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ScoringParamCoef mClass = new ScoringParamCoef();
                mClass = (ScoringParamCoef)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #endregion
        #endregion

        #region ==== REFERENCES ====

        #region == Auto references ==
        #region AutoDealerList
        [WebMethod]

        public List<AutoDealer> AutoDealerList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_AUTO_DEALER_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<AutoDealer> RetList = new List<AutoDealer>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                AutoDealer mClass = new AutoDealer();
                mClass = (AutoDealer)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region AutoInsuranceDriverPassengerList
        [WebMethod]

        public List<InsuranceDriverPassenger> AutoInsuranceDriverPassengerList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_AUTO_INSURANCE_DRIVER_PASSENGER_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InsuranceDriverPassenger> RetList = new List<InsuranceDriverPassenger>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InsuranceDriverPassenger mClass = new InsuranceDriverPassenger();
                mClass = (InsuranceDriverPassenger)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region AutoInsuranceCompany
        [WebMethod]
        public List<InsuranceCompany> InsuranceCompanyList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_INSURANCE_COMPANY_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InsuranceCompany> RetList = new List<InsuranceCompany>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InsuranceCompany mClass = new InsuranceCompany();
                mClass = (InsuranceCompany)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion


        #region LMSInsuranceTypeList
        [WebMethod]
        public List<LMSInsuranceType> LMSInsuranceTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_INSURANCE_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LMSInsuranceType> RetList = new List<LMSInsuranceType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LMSInsuranceType mClass = new LMSInsuranceType();
                mClass = (LMSInsuranceType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region AutoInsuranceTariffList
        [WebMethod]

        public List<InsuranceTariff> AutoInsuranceTariffList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_AUTO_INSURANCE_TARIFF_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InsuranceTariff> RetList = new List<InsuranceTariff>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InsuranceTariff mClass = new InsuranceTariff();
                mClass = (InsuranceTariff)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region AutoInsuranceThirdPartyList
        [WebMethod]

        public List<InsuranceThirdParty> AutoInsuranceThirdPartyList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_AUTO_INSURANCE_THIRD_PARTY_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InsuranceThirdParty> RetList = new List<InsuranceThirdParty>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InsuranceThirdParty mClass = new InsuranceThirdParty();
                mClass = (InsuranceThirdParty)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion
        #endregion

        #region BankList
        [WebMethod]

        public List<Bank> BankList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_BANK_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Bank> RetList = new List<Bank>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Bank mClass = new Bank();
                mClass = (Bank)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<CreditBank> CreditBankList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CREDIT_BANKS");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CreditBank> RetList = new List<CreditBank>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CreditBank mClass = new CreditBank();
                mClass = (CreditBank)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region AccountProductList
        [WebMethod]

        public List<AccountProduct> AccountProductList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ACCOUNT_PRODUCT_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<AccountProduct> RetList = new List<AccountProduct>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                AccountProduct mClass = new AccountProduct();
                mClass = (AccountProduct)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion AccountProductList

        #region AccountProductCheck
        [WebMethod]

        public int AccountProductCheck(int client_no)
        {
            string mSQL = String.Format("EXEC dbo.pREF_ACCOUNT_PRODUCT_CHECK @client_no = {0}", client_no);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            return Convert.ToInt32(mDataTable.Rows[0]["PRODUCT_NO"]);
        }
        #endregion AccountProductCheck

        #region BranchList
        [WebMethod]

        public List<PipelineWS.LocalModel.Branch> BranchList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_BRANCH_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.Branch> RetList = new List<PipelineWS.LocalModel.Branch>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.Branch mClass = new PipelineWS.LocalModel.Branch();
                mClass = (PipelineWS.LocalModel.Branch)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region BusinessList
        [WebMethod]

        public List<PipelineWS.LocalModel.Business> BusinessList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_BUSINESS_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.Business> RetList = new List<PipelineWS.LocalModel.Business>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.Business mClass = new PipelineWS.LocalModel.Business();
                mClass = (PipelineWS.LocalModel.Business)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CityList
        [WebMethod]

        public List<City> CityList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CITY_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<City> RetList = new List<City>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                City mClass = new City();
                mClass = (City)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientIncomeODBList
        [WebMethod]

        public List<ClientIncomeODB> ClientIncomeODBList(int mClientNo, int mMonth, int mHideEmploee, int mHideVip)
        {
            string mSQL = String.Format("exec dbo.bank_client_income @client_no = {0}, @month = {1}, @hide_employee = {2}, @hide_vip  = {3} ", mClientNo, mMonth, mHideEmploee, mHideVip);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientIncomeODB> RetList = new List<ClientIncomeODB>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientIncomeODB mClass = new ClientIncomeODB();
                mClass = (ClientIncomeODB)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientRelationList
        [WebMethod]

        public ClientRelations ClientRelationList(int mClientNo)
        {
            ClientRelations mClientRelations = new ClientRelations();
            ClientRelationClientInfo mClientRelationClientInfo = new ClientRelationClientInfo();
            string mSQL = String.Format("EXEC dbo.bank_client_relations @client_no =  {0}", mClientNo);
            DataSet mDataSet = _DGate.GetDataSet(mSQL);
            DataTable mDataTable = mDataSet.Tables[0];
            List<ClientRelation> RetList = new List<ClientRelation>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientRelation mClass = new ClientRelation();
                mClass = (ClientRelation)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            mClientRelations.ClientRelationList = RetList;
            mClientRelationClientInfo.IS_INSIDER = Convert.ToBoolean(mDataSet.Tables[1].Rows[0]["IS_INSIDER"]);
            mClientRelations.ClientRelationClientInfo = mClientRelationClientInfo;
            return mClientRelations;
        }
        #endregion

        #region ClientApprovalRateList
        [WebMethod]

        public List<ClientApprovalRate> ClientApprovalRateList(string personal_id)
        {
            string mSQL = String.Format("EXEC dbo.pLoanARFormGet @personal_id =  {0}", StringFunctions.SqlQuoted(personal_id));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientApprovalRate> RetList = new List<ClientApprovalRate>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientApprovalRate mClass = new ClientApprovalRate();
                mClass = (ClientApprovalRate)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ClientApprovalRateList

        #region == Client ==
        #region ClientActivesTypeList
        [WebMethod]

        public List<ClientActivesType> ClientActivesTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CLIENT_ACTIVES_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientActivesType> RetList = new List<ClientActivesType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientActivesType mClass = new ClientActivesType();
                mClass = (ClientActivesType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientLastApplicationsGet
        [WebMethod]

        public List<LoanApplicationRecord> ClientLastApplicationsGet(int ApplicationId, int UserID, int ClientNO)
        {
            string mSQL = String.Format("EXEC dbo.pCLIENT_APPLICATIONS_GET @application_id = {0},@user_id = {1},@client_no = {2}", ApplicationId, UserID, ClientNO);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LoanApplicationRecord> RetList = new List<LoanApplicationRecord>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanApplicationRecord mClass = new LoanApplicationRecord();
                mClass = (LoanApplicationRecord)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientRankList
        [WebMethod]

        public List<ClientRank> ClientRankList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CLIENT_RANK_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientRank> RetList = new List<ClientRank>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientRank mClass = new ClientRank();
                mClass = (ClientRank)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientSalaryCategoryList
        [WebMethod]

        public List<ClientSalaryCategory> ClientSalaryCategoryList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CLIENT_SALARY_CATEGORY_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientSalaryCategory> RetList = new List<ClientSalaryCategory>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientSalaryCategory mClass = new ClientSalaryCategory();
                mClass = (ClientSalaryCategory)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientTypeList
        [WebMethod]

        public List<ClientType> ClientTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CLIENT_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientType> RetList = new List<ClientType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientType mClass = new ClientType();
                mClass = (ClientType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CoborrowerRelationTypeList
        [WebMethod]
        public List<CoborrowerRelationType> CoborrowerRelationTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_COBORROWER_RELATION_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CoborrowerRelationType> RetList = new List<CoborrowerRelationType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CoborrowerRelationType mClass = new CoborrowerRelationType();
                mClass = (CoborrowerRelationType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientSubTypeList
        [WebMethod]
        public List<ClientSubType> ClientSubTypeList()
        {
            string mSQL = String.Format("EXEC pREF_CLIENT_SUBTYPE @all = 0");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientSubType> RetList = new List<ClientSubType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientSubType mClass = new ClientSubType();
                mClass = (ClientSubType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ClientSubTypeList

        #region ClientConversationalLanguageList
        [WebMethod]
        public List<ClientConversationalLanguage> ClientConversationalLanguageList()
        {
            string mSQL = String.Format("EXEC pREF_CLIENT_CONVERSATIONAL_LANGUAGES");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientConversationalLanguage> RetList = new List<ClientConversationalLanguage>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientConversationalLanguage mClass = new ClientConversationalLanguage();
                mClass = (ClientConversationalLanguage)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ClientSubTypeList

        #region ClientMaxAge
        [WebMethod]
        public int ClientMaxAgeGet(int productID)
        {
            int RetValue = 0;

            string mSQL = String.Format(@"EXEC dbo.pCLIENT_MAX_AGE_GET @PRODUCT_ID = {0}", productID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            if (mDataTable != null && mDataTable.Rows[0] != null)
            {
                DataRow mDataRow = mDataTable.Rows[0];

                RetValue = Convert.ToInt32(mDataRow["CLIENT_MAX_AGE"]);
            }

            return RetValue;
        }
        #endregion
        #endregion

        #region == Collateral ==
        #region CollateralTypeList
        [WebMethod]

        public List<PipelineWS.LocalModel.CollateralType> CollateralTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_COLLATERAL_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.CollateralType> RetList = new List<PipelineWS.LocalModel.CollateralType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.CollateralType mClass = new PipelineWS.LocalModel.CollateralType();
                mClass = (PipelineWS.LocalModel.CollateralType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region LoanCollateralTypeList
        [WebMethod]

        public List<LoanCollateralType> LoanCollateralTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_LOAN_COLLATERAL_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LoanCollateralType> RetList = new List<LoanCollateralType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanCollateralType mClass = new LoanCollateralType();
                mClass = (LoanCollateralType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #endregion

        #region == Committee ==
        #region CommitteeDelegateList
        [WebMethod]

        public List<CommitteeDelegate> CommitteeDelegateList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_COMMITTEE_DELEGATE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CommitteeDelegate> RetList = new List<CommitteeDelegate>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CommitteeDelegate mClass = new CommitteeDelegate();
                mClass = (CommitteeDelegate)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CommitteeDelegateSave
        [WebMethod]

        public bool CommitteeDelegateSave(CommitteeDelegate _class)
        {
            bool retValue = false;
            string _procedure = "";
            if (_class.IS_ACTIVE)
            {
                _procedure = "pSYS_USER_COMMITTEE_DELEGATE_SET";
            }
            else
            {
                _procedure = "pSYS_USER_COMMITTEE_DELEGATE_DEL";
            }
            string mSQL = String.Format(@"EXEC dbo." + _procedure + " @id = {0},@user_id = {1}"
            , _class.COMMITTEE_MAP_ID
            , _class.USER_ID

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region CommitteeList
        [WebMethod]

        public List<Committee> CommitteeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_COMMITTEE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Committee> RetList = new List<Committee>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Committee mClass = new Committee();
                mClass = (Committee)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CommitteePropertiesList
        [WebMethod]

        public List<CommitteeProperties> CommitteePropertiesList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_COMMITTEE_PROPERTIES");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CommitteeProperties> RetList = new List<CommitteeProperties>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CommitteeProperties mClass = new CommitteeProperties();
                mClass = (CommitteeProperties)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CommitteePropertiesMapList
        [WebMethod]

        public List<CommitteePropertiesMap> CommitteePropertiesMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_COMMITTEE_PROPERTIES");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CommitteePropertiesMap> RetList = new List<CommitteePropertiesMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CommitteePropertiesMap mClass = new CommitteePropertiesMap();
                mClass = (CommitteePropertiesMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region CommitteeUserList
        [WebMethod]

        public List<CommitteeUserMap> CommitteeUserList(int CommitteeId)
        {
            string mSQL = String.Format("EXEC dbo.pREF_COMMITTEE_USER_MAP_LIST @committee_id = {0}", CommitteeId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CommitteeUserMap> RetList = new List<CommitteeUserMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CommitteeUserMap mClass = new CommitteeUserMap();
                mClass = (CommitteeUserMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region UserCommitteeList
        [WebMethod]

        public List<UserCommittee> UserCommitteeList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_COMMITTEE_MAP_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<UserCommittee> RetList = new List<UserCommittee>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                UserCommittee mClass = new UserCommittee();
                mClass = (UserCommittee)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region UserCommitteeSave
        [WebMethod]

        public bool UserCommitteeSave(UserCommittee _class)
        {
            bool retValue = false;
            string _procedure = "";
            if (_class.IS_ACTIVE)
            {
                _procedure = "pSYS_USER_COMMITTEE_MAP_SET";
            }
            else
            {
                _procedure = "pSYS_USER_COMMITTEE_MAP_DEL";
            }
            string mSQL = String.Format(@"EXEC dbo." + _procedure + " @id = {0},@user_id = {1}"
            , _class.COMMITTEE_ID
            , _class.USER_ID

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #endregion

        #region CountryList
        [WebMethod]

        public List<PipelineWS.LocalModel.Country> CountryList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_COUNTRY_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.Country> RetList = new List<PipelineWS.LocalModel.Country>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.Country mClass = new PipelineWS.LocalModel.Country();
                mClass = (PipelineWS.LocalModel.Country)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CountryCodeMapList
        [WebMethod]

        public List<CountryCodeMap> CountryCodeMapList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_COUNTRY_CODES_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CountryCodeMap> RetList = new List<CountryCodeMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CountryCodeMap mClass = new CountryCodeMap();
                mClass = (CountryCodeMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        public string ContryCode2Get(string mContryCode3)
        {
            string result = "";
            try
            {
                result = CountryCodeMapList().Find(x => x.CODE3 == mContryCode3).CODE2;
            }
            catch
            {
                result = mContryCode3.Substring(0, 2);
            }
            return result;

        }
        #endregion

        #region == CrediInfo ==
        #region CRInfo

        #endregion

        #region CrediInfoGradeList
        [WebMethod]

        public List<CrediInfoGrade> CrediInfoGradeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CREDIT_INFO");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CrediInfoGrade> RetList = new List<CrediInfoGrade>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CrediInfoGrade mClass = new CrediInfoGrade();
                mClass = (CrediInfoGrade)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CreditHistoryKindList
        [WebMethod]

        public List<CreditHistoryKind> CreditHistoryKindList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CREDIT_HISTORY_KIND_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CreditHistoryKind> RetList = new List<CreditHistoryKind>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CreditHistoryKind mClass = new CreditHistoryKind();
                mClass = (CreditHistoryKind)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion
        #endregion

        #region CreditTypeList
        [WebMethod]

        public List<PipelineWS.LocalModel.CreditType> CreditTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CREDIT_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.CreditType> RetList = new List<PipelineWS.LocalModel.CreditType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.CreditType mClass = new PipelineWS.LocalModel.CreditType();
                mClass = (PipelineWS.LocalModel.CreditType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region CurrencyList
        [WebMethod]

        public List<PipelineWS.LocalModel.Currency> CurrencyList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CURRENCIES_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.Currency> RetList = new List<PipelineWS.LocalModel.Currency>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.Currency mClass = new PipelineWS.LocalModel.Currency();
                mClass = (PipelineWS.LocalModel.Currency)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region EmployeeList
        [WebMethod]

        public List<Employee> EmployeeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_EMPLOYEE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Employee> RetList = new List<Employee>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Employee mClass = new Employee();
                mClass = (Employee)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region EnshureTypeList
        [WebMethod]

        public List<EnshureType> EnshureTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ENSURE_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<EnshureType> RetList = new List<EnshureType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                EnshureType mClass = new EnshureType();
                mClass = (EnshureType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region == Loan Exception ==
        #region ExceptionLoanPartipicationList
        [WebMethod]

        public List<ExceptionLoanPartipication> ExceptionLoanPartipicationList()
        {
            string mSQL = String.Format("EXEC dbo.pEXCEPTION_LOAN_PARTIPICATION_GET");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ExceptionLoanPartipication> RetList = new List<ExceptionLoanPartipication>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionLoanPartipication mClass = new ExceptionLoanPartipication();
                mClass = (ExceptionLoanPartipication)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ExceptionLoanPrepaimentList
        [WebMethod]

        public List<ExceptionLoanPrepaiment> ExceptionLoanPrepaimentList()
        {
            string mSQL = String.Format("EXEC dbo.pEXCEPTION_LOAN_PREPAIMENT_GET");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ExceptionLoanPrepaiment> RetList = new List<ExceptionLoanPrepaiment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionLoanPrepaiment mClass = new ExceptionLoanPrepaiment();
                mClass = (ExceptionLoanPrepaiment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ExceptionProdOrgList
        [WebMethod]

        public List<ExceptionProdOrg> ExceptionProdOrgList()
        {
            string mSQL = String.Format("EXEC dbo.pEXCEPTION_PROD_ORG_GET");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ExceptionProdOrg> RetList = new List<ExceptionProdOrg>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionProdOrg mClass = new ExceptionProdOrg();
                mClass = (ExceptionProdOrg)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ExceptionProdRangestList
        [WebMethod]

        public List<ExceptionProdRanges> ExceptionProdRangesList()
        {
            string mSQL = String.Format("EXEC dbo.pEXCEPTION_PROD_RANGES_GET");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ExceptionProdRanges> RetList = new List<ExceptionProdRanges>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionProdRanges mClass = new ExceptionProdRanges();
                mClass = (ExceptionProdRanges)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region LoanExceptionGet
        [WebMethod]

        public LoanException LoanExceptionGet()
        {
            LoanException _LoanException = new LoanException();
            string mSQL = String.Format("EXEC dbo.pLOAN_EXCEPTION_GET");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ExceptionLoanPartipication> ExceptionLoanPartipicationList = new List<ExceptionLoanPartipication>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionLoanPartipication ExceptionLoanPartipicationClass = new ExceptionLoanPartipication();
                ExceptionLoanPartipicationClass = (ExceptionLoanPartipication)FillClass(ExceptionLoanPartipicationClass, mDataRow);
                ExceptionLoanPartipicationList.Add(ExceptionLoanPartipicationClass);
            }
            _LoanException.ExceptionLoanPartipicationList = ExceptionLoanPartipicationList;

            mDataTable = _DGate.GetDataSet(mSQL).Tables[1];
            List<ExceptionLoanPrepaiment> ExceptionLoanPrepaimentList = new List<ExceptionLoanPrepaiment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionLoanPrepaiment ExceptionLoanPrepaimentClass = new ExceptionLoanPrepaiment();
                ExceptionLoanPrepaimentClass = (ExceptionLoanPrepaiment)FillClass(ExceptionLoanPrepaimentClass, mDataRow);
                ExceptionLoanPrepaimentList.Add(ExceptionLoanPrepaimentClass);
            }
            _LoanException.ExceptionLoanPrepaimentList = ExceptionLoanPrepaimentList;

            mDataTable = _DGate.GetDataSet(mSQL).Tables[2];
            List<ExceptionProdOrg> ExceptionProdOrgList = new List<ExceptionProdOrg>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionProdOrg ExceptionProdCommissionClass = new ExceptionProdOrg();
                ExceptionProdCommissionClass = (ExceptionProdOrg)FillClass(ExceptionProdCommissionClass, mDataRow);
                ExceptionProdOrgList.Add(ExceptionProdCommissionClass);
            }
            _LoanException.ExceptionProdOrgList = ExceptionProdOrgList;

            mDataTable = _DGate.GetDataSet(mSQL).Tables[3];
            List<ExceptionProdRanges> ExceptionProdRangesList = new List<ExceptionProdRanges>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionProdRanges ExceptionProdInterestClass = new ExceptionProdRanges();
                ExceptionProdInterestClass = (ExceptionProdRanges)FillClass(ExceptionProdInterestClass, mDataRow);
                ExceptionProdRangesList.Add(ExceptionProdInterestClass);
            }
            _LoanException.ExceptionProdRangesList = ExceptionProdRangesList;

            mDataTable = _DGate.GetDataSet(mSQL).Tables[4];
            List<ExceptionAutoPartipicationRanges> ExceptionAutoPartipicationRangesList = new List<ExceptionAutoPartipicationRanges>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ExceptionAutoPartipicationRanges ExceptionAutoPartipicationRangesClass = new ExceptionAutoPartipicationRanges();
                ExceptionAutoPartipicationRangesClass = (ExceptionAutoPartipicationRanges)FillClass(ExceptionAutoPartipicationRangesClass, mDataRow);
                ExceptionAutoPartipicationRangesList.Add(ExceptionAutoPartipicationRangesClass);
            }
            _LoanException.ExceptionAutoPartipicationRangesList = ExceptionAutoPartipicationRangesList;

            return _LoanException;
        }
        #endregion
        #endregion

        #region IncomeTypeList
        [WebMethod]

        public List<IncomeType> IncomeTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_INCOME_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<IncomeType> RetList = new List<IncomeType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                IncomeType mClass = new IncomeType();
                mClass = (IncomeType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region IncomeVerifiedList
        [WebMethod]

        public List<IncomeVerified> IncomeVerifiedList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_INCOME_VERIFIED");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<IncomeVerified> RetList = new List<IncomeVerified>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                IncomeVerified mClass = new IncomeVerified();
                mClass = (IncomeVerified)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region InformationSourceList
        [WebMethod]

        public List<InformationSource> InformationSourceList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_INFORMATION_SOURCE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InformationSource> RetList = new List<InformationSource>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InformationSource mClass = new InformationSource();
                mClass = (InformationSource)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region LoanAimList
        [WebMethod]

        public List<LoanAim> LoanAimList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_LOAN_AIM_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LoanAim> RetList = new List<LoanAim>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanAim mClass = new LoanAim();
                mClass = (LoanAim)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region LoanTypeList
        [WebMethod]

        public List<PipelineWS.LocalModel.LoanType> LoanTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_LOAN_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.LoanType> RetList = new List<PipelineWS.LocalModel.LoanType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.LoanType mClass = new PipelineWS.LocalModel.LoanType();
                mClass = (PipelineWS.LocalModel.LoanType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region MaritalStatusList
        [WebMethod]

        public List<MaritalStatus> MaritalStatusList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_MARITAL_STATUS");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<MaritalStatus> RetList = new List<MaritalStatus>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                MaritalStatus mClass = new MaritalStatus();
                mClass = (MaritalStatus)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region == OrganizationSave ==
        #region NewOrganizationSave
        [WebMethod]

        public int NewOrganizationSave(string organizationName, string organizationTaxCode, int sebClassId)
        {
            int retValue = 0;
            string mSQL = String.Format(@"EXEC dbo.pREF_NEW_ORGANIZATION_SET @organization_name = {0},@organization_tax_code = {1},@seb_class_id = {2}"
            , StringFunctions.SqlQuoted(organizationName, true), StringFunctions.SqlQuoted(organizationTaxCode, true), sebClassId
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["REC_ID"]);
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region OrgTypeList
        [WebMethod]

        public List<OrgType> OrgTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ORG_TYPES_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<OrgType> RetList = new List<OrgType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                OrgType mClass = new OrgType();
                mClass = (OrgType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region OrganizationList
        [WebMethod]
        public List<Organization> OrganizationList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ORGANIZATION_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Organization> RetList = new List<Organization>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Organization mClass = new Organization();
                mClass = (Organization)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region OrganizationListSearch
        [WebMethod]
        public List<Organization> OrganizationListSearch(string organization_name, string organization_tax_code)
        {
            string mSQL = String.Format("EXEC dbo.pREF_ORGANIZATION_SEARCH @organization_name = {0}, @organization_tax_code = {1}", StringFunctions.SqlQuoted(organization_name, true), StringFunctions.SqlQuoted(organization_tax_code, true));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Organization> RetList = new List<Organization>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Organization mClass = new Organization();
                mClass = (Organization)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion


        #endregion

        #region PassportTypeList
        [WebMethod]

        public List<PassportType> PassportTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CLIENT_DOCUMENT_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PassportType> RetList = new List<PassportType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PassportType mClass = new PassportType();
                mClass = (PassportType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region PenaltyCalculationTypeList
        [WebMethod]

        public List<PipelineWS.LocalModel.PenaltyCalculationType> PenaltyCalculationTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PENALTY_CALCULATION_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.PenaltyCalculationType> RetList = new List<PipelineWS.LocalModel.PenaltyCalculationType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.PenaltyCalculationType mClass = new PipelineWS.LocalModel.PenaltyCalculationType();
                mClass = (PipelineWS.LocalModel.PenaltyCalculationType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region PenaltySchemaList
        [WebMethod]

        public List<PipelineWS.LocalModel.PenaltySchema> PenaltySchemaList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PENALTY_SCHEMA_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.PenaltySchema> RetList = new List<PipelineWS.LocalModel.PenaltySchema>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.PenaltySchema mClass = new PipelineWS.LocalModel.PenaltySchema();
                mClass = (PipelineWS.LocalModel.PenaltySchema)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region PmtFreqTypeList
        [WebMethod]

        public List<PmtFreqType> PmtFreqTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PMT_FREQ_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PmtFreqType> RetList = new List<PmtFreqType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PmtFreqType mClass = new PmtFreqType();
                mClass = (PmtFreqType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion

        #region PrepaymentRescheduleTypeList
        [WebMethod]

        public List<PrepaymentRescheduleType> PrepaymentRescheduleTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PREPAYMENT_RESCHEDULE_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PrepaymentRescheduleType> RetList = new List<PrepaymentRescheduleType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PrepaymentRescheduleType mClass = new PrepaymentRescheduleType();
                mClass = (PrepaymentRescheduleType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region  ==== Product ====
        #region ProductCategoryList
        [WebMethod]
        public List<RefProductCategory> RefProductCategoryList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PRODUCT_CATEGORY_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RefProductCategory> RetList = new List<RefProductCategory>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RefProductCategory mClass = new RefProductCategory();
                mClass = (RefProductCategory)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }


        [WebMethod]
        public List<ProductCategory> ProductCategoryList()
        {
            string mSQL = String.Format("EXEC dbo.pPRODUCT_CATEGORY_GET");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductCategory> RetList = new List<ProductCategory>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductCategory mClass = new ProductCategory();
                mClass = (ProductCategory)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ProductCategoryList

        #region ProductCategoryMapList
        [WebMethod]
        public List<ProductCategoryMap> ProductCategoryMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_PRODUCT_CATEGORY");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductCategoryMap> RetList = new List<ProductCategoryMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductCategoryMap mClass = new ProductCategoryMap();
                mClass = (ProductCategoryMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ProductCategoryMapList

        #region ProductDetailsList
        [WebMethod]

        public List<ProductDetails> ProductDetailsList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PRODUCT_DETAILS_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductDetails> RetList = new List<ProductDetails>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductDetails mClass = new ProductDetails();
                mClass = (ProductDetails)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ProductDetailsList

        #region ProductGroupsList
        [WebMethod]
        public List<ProductGroups> ProductGroupsList(int user_id)
        {
            string mSQL = String.Format("EXEC dbo.pREF_PRODUCT_GROUPS_LIST @user_id = {0}", user_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductGroups> RetList = new List<ProductGroups>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductGroups mClass = new ProductGroups();
                mClass = (ProductGroups)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ProductGroupsList

        #region  =Product class=

        #region ProductList
        [WebMethod]
        public List<LocalModel.Product> ProductList(int mIS_ACTIVE)
        {
            string mSQL = String.Format("EXEC dbo.pREF_PRODUCT_LIST @is_active = {0}", mIS_ACTIVE);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            var RetList = new List<LocalModel.Product>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                var mClass = new LocalModel.Product();
                mClass = (LocalModel.Product)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ProductList

        #region ProductSave
        [WebMethod]
        public int ProductSave(LocalModel.Product _class, int user_id)
        {
            int retValue = 0;
            string mSQL = String.Format(@"EXEC pREF_PRODUCTS_SET @product_id = {0},@product_name = {1},@product_group_id = {2},@is_active = {3},@is_cash_cover = {4}
                                        ,@review_period = {5},@base_product_id  = {6},@date_start = {7},@date_end = {8},@discount = {9},@admin_fee_type = {10},@user_id = {11}"
                , _class.PRODUCT_ID
                , StringFunctions.SqlQuoted(_class.PRODUCT_NAME, true)
                , _class.PRODUCT_GROUP_ID
                , Convert.ToInt32(_class.IS_ACTIVE)
                , Convert.ToInt32(_class.IS_CASH_COVER)
                , _class.REVIEW_PERIOD
                , _class.BASE_PRODUCT_ID
                , StringFunctions.SqlDateQuoted(_class.DATE_START)
                , StringFunctions.SqlDateQuoted(_class.DATE_END)
                , _class.DISCOUNT
                , _class.ADMIN_FEE_TYPE
                , user_id
                );

            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]);
                }
            }
            catch { }
            return retValue;
        }

        [WebMethod]
        public int ProductMapSave(int mOLD_PRODUCT_ID, int mNEW_PRODUCT_ID)
        {
            int retValue = 0;

            string mSQL = String.Format(@"EXEC dbo.pUTIL_CREATE_PRODUCT @OLD_PRODUCT_ID = {0}, @NEW_PRODUCT_ID = {1}", mOLD_PRODUCT_ID, mNEW_PRODUCT_ID);
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]);
                }
            }
            catch { }

            return retValue;
        }
        #endregion ProductSave

        #endregion  =Product class=

        #region == ProductProperties ==

        #region ProductPropertiesDelete
        [WebMethod]
        public bool ProductPropertiesDelete(int product_id, int user_id)
        {
            string mSQL = String.Format(@"EXEC dbo.pPRODUCT_PROPERTIES_DEL @product_id = {0},@user_id = {1}"
            , product_id
            , user_id
            );
            try
            {
                _DGate.Execute(mSQL);
            }
            catch { }
            return true;
        }
        #endregion

        #region ProductPropertiesList
        [WebMethod]
        public List<PipelineWS.LocalModel.ProductProperties> ProductPropertiesList()
        {
            string mSQL = String.Format("EXEC dbo.pPRODUCT_PROPERTIES_GET");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.ProductProperties> RetList = new List<PipelineWS.LocalModel.ProductProperties>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.ProductProperties mClass = new PipelineWS.LocalModel.ProductProperties();
                mClass = (PipelineWS.LocalModel.ProductProperties)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ProductPropertiesSave
        [WebMethod]
        public bool ProductPropertiesSave(PipelineWS.LocalModel.ProductProperties _class, int product_id, int user_id)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC pPRODUCT_PROPERTIES_SET @product_id = {0},@ccy = {1},@min_amount  = {2},@max_amount  = {3},@min_period = {4},@max_period = {5}
            ,@interest_rate_from  = {6},@interest_rate_to  = {7},@notused_rate_from  = {8},@notused_rate_to  = {9},@prepayment_rate_from  = {10}
            ,@prepayment_rate_to  = {11},@admin_fee_percent_from  = {12},@admin_fee_percent_to  = {13},@admin_fee_min  = {14},@fee1_percent_from  = {15}
            ,@fee1_percent_to  = {16},@fee1_min  = {17},@fee2_percent_from  = {18},@fee2_percent_to  = {19},@fee2_min  = {20},@fee3_percent_from  = {21}
            ,@fee3_percent_to  = {22},@fee3_min  = {23},@grace_period_from = {24},@grace_period_to = {25},@decision_source  = {26},@committee_id = {27}
            ,@min_prepayment_count  = {28},@min_prepayment_amount  = {29},@penalty_calculation_type  = {30},@percent_prepayment_type  = {31}
            ,@overpay_prepayment_rate_from  = {32},@overpay_prepayment_rate_to  = {33},@overpay_penalty_calculation_type  = {34},@overpay_percent_prepayment_type  = {35}
            ,@penalty_schema_id = {36},@penalty_on_payment_in_other_bank_from  = {37},@penalty_on_payment_in_other_bank_to  = {38},@ob_overpay_percent_prepayment_type  = {39}
            ,@ob_overpay_penalty_calculation_type  = {40},@max_salary_amount  = {41},@general_aggrement_amount  = {42},@general_aggrement_period_yaer = {43}
            ,@participation_min  = {44},@participation_max  = {45},@product_category_id  = {46},@product_extended_id  = {47},@user_id = {48}"
            , product_id
            , StringFunctions.SqlQuoted(_class.CCY, true)
            , _class.MIN_AMOUNT
            , _class.MAX_AMOUNT
            , _class.MIN_PERIOD
            , _class.MAX_PERIOD
            , _class.INTEREST_RATE_FROM
            , _class.INTEREST_RATE_TO
            , _class.NOTUSED_RATE_FROM
            , _class.NOTUSED_RATE_TO
            , _class.PREPAYMENT_RATE_FROM
            , _class.PREPAYMENT_RATE_TO
            , _class.ADMIN_FEE_PERCENT_FROM
            , _class.ADMIN_FEE_PERCENT_TO
            , _class.ADMIN_FEE_MIN
            , _class.FEE1_PERCENT_FROM
            , _class.FEE1_PERCENT_TO
            , _class.FEE1_MIN
            , _class.FEE2_PERCENT_FROM
            , _class.FEE2_PERCENT_TO
            , _class.FEE2_MIN
            , _class.FEE3_PERCENT_FROM
            , _class.FEE3_PERCENT_TO
            , _class.FEE3_MIN
            , _class.GRACE_PERIOD_FROM
            , _class.DECISION_SOURCE
            , _class.COMMITTEE_ID
            , _class.GRACE_PERIOD_TO
            , _class.MIN_PREPAYMENT_COUNT
            , _class.MIN_PREPAYMENT_AMOUNT
            , _class.PENALTY_CALCULATION_TYPE
            , _class.PERCENT_PREPAYMENT_TYPE
            , _class.OVERPAY_PREPAYMENT_RATE_FROM
            , _class.OVERPAY_PREPAYMENT_RATE_TO
            , _class.OVERPAY_PENALTY_CALCULATION_TYPE
            , _class.OVERPAY_PERCENT_PREPAYMENT_TYPE
            , _class.PENALTY_SCHEMA_ID
            , _class.PENALTY_ON_PAYMENT_IN_OTHER_BANK_FROM
            , _class.PENALTY_ON_PAYMENT_IN_OTHER_BANK_TO
            , _class.OB_OVERPAY_PERCENT_PREPAYMENT_TYPE
            , _class.OB_OVERPAY_PENALTY_CALCULATION_TYPE
            , _class.MAX_SALARY_AMOUNT
            , _class.GENERAL_AGGREMENT_AMOUNT
            , _class.GENERAL_AGGREMENT_PERIOD_YAER
            , _class.PARTICIPATION_MIN
            , _class.PARTICIPATION_MAX
            , _class.PRODUCT_CATEGORY_ID
            , _class.PRODUCT_EXTENDED_ID
            , user_id
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion ProductPropertiesSave

        #region ProductPropertiesListSave
        [WebMethod]
        public bool ProductPropertiesListSave(List<PipelineWS.LocalModel.ProductProperties> _class, int product_id, int user_id)
        {
            this.ProductPropertiesDelete(product_id, user_id);
            _class.ForEach(item =>
            {
                this.ProductPropertiesSave(item, product_id, user_id);
            }
            );

            return true;
        }
        #endregion ProductPropertiesListSave

        #endregion  == ProductProperties ==

        #endregion ==== Product ====

        #region === Product Action ===

        #region =ProductActionBranch=

        #region =ProductActionBranchList=
        [WebMethod]
        public List<ProductActionBranch> ProductActionBranchList(int mProduct_id)
        {
            string mSQL = String.Format("EXEC  dbo.pPRODUCT_ACTION_BRANCHES @product_id = {0}", mProduct_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductActionBranch> RetList = new List<ProductActionBranch>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductActionBranch mClass = new ProductActionBranch();
                mClass = (ProductActionBranch)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion =ProductActionBranchList=

        #region =ProductActionBranchDel=
        [WebMethod]
        public void ProductActionBranchDelete(int mProduct_id)
        {
            string mSQL = String.Format("EXEC  dbo.pPRODUCT_ACTION_BRANCH_DEL @product_id = {0}", mProduct_id);
            _DGate.Execute(mSQL);
        }
        #endregion =ProductActionBranchDelete=

        #region =ProductActionBranchSave=
        [WebMethod]
        public void ProductActionBranchSave(int mProduct_id, string mBranch_id)
        {
            string mSQL = String.Format("EXEC  dbo.pPRODUCT_ACTION_BRANCH_SET @product_id = {0},@branch_id = {1}", mProduct_id, mBranch_id);
            _DGate.Execute(mSQL);
        }
        #endregion =ProductActionBranchSaveList=

        #region =ProductActionBranchSaveList=
        [WebMethod]
        public void ProductActionBranchSaveList(List<ProductActionBranch> mProductActionBranch, int mProduct_id)
        {
            this.ProductActionBranchDelete(mProduct_id);
            mProductActionBranch.ForEach(item =>
            {
                if (item.PRODUCT_USED)
                {
                    this.ProductActionBranchSave(mProduct_id, item.BRANCH_ID);
                }
            });
        }
        #endregion =ProductActionBranchSaveList=

        #endregion =ProductActionBranch=

        #region =ProductActionInstalment=

        #region =ProductActionInstalmentList=

        [WebMethod]
        public List<ProductActionInstalment> ProductActionInstalmentList(int mProduct_id)
        {
            string mSQL = String.Format("EXEC  dbo.pPRODUCT_ACTION_INSTALMENT @product_id = {0}", mProduct_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductActionInstalment> RetList = new List<ProductActionInstalment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductActionInstalment mClass = new ProductActionInstalment();
                mClass = (ProductActionInstalment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion =ProductActionInstalmentList=

        #region =ProductActionInstalmentDel=
        [WebMethod]
        public void ProductActionInstalmentDelete(int mProduct_id)
        {
            string mSQL = String.Format("EXEC  dbo.pPRODUCT_ACTION_INSTALLMENT_DEL @product_id = {0}", mProduct_id);
            _DGate.Execute(mSQL);
        }
        #endregion =ProductActionInstalmentDelete=

        #region =ProductActionInstalmentSave=
        [WebMethod]
        public void ProductActionInstalmentSave(int mProduct_id, int mInstallment_id)
        {
            string mSQL = String.Format("EXEC  dbo.pPRODUCT_ACTION_INSTALLMENT_SET @product_id = {0},@installment_id = {1}", mProduct_id, mInstallment_id);
            _DGate.Execute(mSQL);
        }
        #endregion =ProductActionInstalmentSaveList=

        #region =ProductActionInstalmentSaveList=
        [WebMethod]
        public void ProductActionInstalmentSaveList(List<ProductActionInstalment> mProductActionInstalment, int mProduct_id)
        {
            this.ProductActionInstalmentDelete(mProduct_id);
            mProductActionInstalment.ForEach(item =>
            {
                if (item.PRODUCT_USED)
                {
                    this.ProductActionInstalmentSave(mProduct_id, item.INSTALLMENT_ID);
                }
            });
        }
        #endregion =ProductActionInstalmentSaveList=

        #endregion =ProductActionInstalment=

        #region == ProductAction Save ==
        [WebMethod]
        public bool ProductActionSave(LocalModel.Product mProduct, List<ProductProperties> mProductPropertiesList, int mUser_id)
        {
            int mProduct_id = this.ProductSave(mProduct, mUser_id);
            this.ProductPropertiesListSave(mProductPropertiesList, mProduct_id, mUser_id);
            this.ProductMapSave(mProduct.BASE_PRODUCT_ID, mProduct_id);

            //this.ProductActionBranchSaveList(mProductActionBranchList, mProduct_id);
            //this.ProductActionInstalmentSaveList(mProductActionInstalmentList, mProduct_id);

            return true;
        }
        #endregion == ProductAction Save ==

        #endregion === ProductAction Action ===

        #region RealEstateTypeList
        [WebMethod]

        public List<RealEstateType> RealEstateTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_REAL_ESTATE_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RealEstateType> RetList = new List<RealEstateType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RealEstateType mClass = new RealEstateType();
                mClass = (RealEstateType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region RealEstateOwnerTypeList
        [WebMethod]

        public List<RealEstateOwnerType> RealEstateOwnerTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_REAL_ESTATE_OWNER_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RealEstateOwnerType> RetList = new List<RealEstateOwnerType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RealEstateOwnerType mClass = new RealEstateOwnerType();
                mClass = (RealEstateOwnerType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ClientPositionList
        [WebMethod]
        public List<ClientPosition> ClientPositionList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CLIENT_POSITION_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientPosition> RetList = new List<ClientPosition>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientPosition mClass = new ClientPosition();
                mClass = (ClientPosition)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region RealEstateMaxLTV
        [WebMethod]
        public List<RealEstateMaxLTV> RealEstateMaxLTVList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_PRODUCT_MAX_LTV_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RealEstateMaxLTV> RetList = new List<RealEstateMaxLTV>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RealEstateMaxLTV mClass = new RealEstateMaxLTV();
                mClass = (RealEstateMaxLTV)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<RealEstateInsurance> RealEstateInsuranceList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_REAL_ESTATE_INSURANCE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RealEstateInsurance> RetList = new List<RealEstateInsurance>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RealEstateInsurance mClass = new RealEstateInsurance();
                mClass = (RealEstateInsurance)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region RejectReasonList
        [WebMethod]

        public List<RejectReason> RejectReasonList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_REJECT_REASON_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RejectReason> RetList = new List<RejectReason>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RejectReason mClass = new RejectReason();
                mClass = (RejectReason)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region RelationshipTypeList
        [WebMethod]

        public List<RelationshipType> RelationshipTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_RELATIONSHIP_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RelationshipType> RetList = new List<RelationshipType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RelationshipType mClass = new RelationshipType();
                mClass = (RelationshipType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region RestructTypeList
        [WebMethod]

        public List<RestructType> RestructTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_RESTRUCT_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RestructType> RetList = new List<RestructType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RestructType mClass = new RestructType();
                mClass = (RestructType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region RoleTypeList
        [WebMethod]

        public List<RoleType> RoleTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ROLE_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<RoleType> RetList = new List<RoleType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                RoleType mClass = new RoleType();
                mClass = (RoleType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ScheduleTypeList
        [WebMethod]

        public List<PipelineWS.LocalModel.ScheduleType> ScheduleTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_SCHEDULE_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.ScheduleType> RetList = new List<PipelineWS.LocalModel.ScheduleType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.ScheduleType mClass = new PipelineWS.LocalModel.ScheduleType();
                mClass = (PipelineWS.LocalModel.ScheduleType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SexList
        [WebMethod]

        public List<Sex> SexList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_SEX");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Sex> RetList = new List<Sex>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Sex mClass = new Sex();
                mClass = (Sex)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region StatusList
        [WebMethod]

        public List<Status> StatusList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_STATUS");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Status> RetList = new List<Status>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Status mClass = new Status();
                mClass = (Status)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region == BlackList ==
        #region BlackList
        [WebMethod]
        public List<BlackList> BlackList()
        {
            string mSQL = String.Format("EXEC dbo.pBLACK_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<BlackList> RetList = new List<BlackList>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                BlackList mClass = new BlackList();
                mClass = (BlackList)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion BlackList

        #region BlackListItem
        [WebMethod]
        public List<BlackListItem> BlackListItemList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_BLACK_LIST_ITEM_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<BlackListItem> RetList = new List<BlackListItem>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                BlackListItem mClass = new BlackListItem();
                mClass = (BlackListItem)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion BlackListItem

        #region BlackListSave
        [WebMethod]
        public bool BlackListSave(int RecID, int ItemID, string ItemValue, int UserID, string Remark, int TypeID)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pBLACK_LIST_SET @REC_ID = {0}, @ITEM_ID = {1}, @ITEM_VALUE = {2}, @USER_ID = {3}, @REMARK = {4}, @RECORDER_TYPE_ID = {5}"
                , RecID
                , ItemID
                , StringFunctions.SqlQuoted(ItemValue, true)
                , UserID
                , StringFunctions.SqlQuoted(Remark, true)
                , TypeID
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion BlackListSave

        #region BlackListDel
        [WebMethod]
        public bool BlackListDel(int RecID, int UserID)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pBLACK_LIST_DEL @REC_ID = {0},@USER_ID = {1}"
                , RecID
                , UserID
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion BlackListDel
        #endregion == BlackList ==

        #region DisbursType List
        [WebMethod]

        public List<DisbursType> DisbursTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_DISBURS_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<DisbursType> RetList = new List<DisbursType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                DisbursType mClass = new DisbursType();
                mClass = (DisbursType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ====== Sys =======

        #region SysCalcParameterList
        [WebMethod]

        public List<SysCalcParameter> SysCalcParameterList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_CALC_PARAMETERS_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysCalcParameter> RetList = new List<SysCalcParameter>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysCalcParameter mClass = new SysCalcParameter();
                mClass = (SysCalcParameter)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysEventGroupList
        [WebMethod]

        public List<SysEventGroup> SysEventGroupList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_EVENT_GROUP_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysEventGroup> RetList = new List<SysEventGroup>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysEventGroup mClass = new SysEventGroup();
                mClass = (SysEventGroup)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysEventList
        [WebMethod]

        public List<SysEvent> SysEventList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_EVENT_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysEvent> RetList = new List<SysEvent>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysEvent mClass = new SysEvent();
                mClass = (SysEvent)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysGroupEventMapList
        [WebMethod]

        public List<SysUserGroupEventMap> SysGroupEventMapList(int UserGroupId)
        {
            string mSQL = String.Format("EXEC dbo.pSYS_GROUP_EVENT_MAP_LIST @user_group_id = {0}", UserGroupId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysUserGroupEventMap> RetList = new List<SysUserGroupEventMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserGroupEventMap mClass = new SysUserGroupEventMap();
                mClass = (SysUserGroupEventMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysParameterList
        [WebMethod]

        public List<SysParameter> SysParameterList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_PARAMETERS_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysParameter> RetList = new List<SysParameter>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysParameter mClass = new SysParameter();
                mClass = (SysParameter)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ==== Sys User ====
        #region == SysUser managment ==
        #region SysUserBlock
        [WebMethod]
        public SysUserState SysUserBlock(SysUser _class)
        {
            SysUserState mSysUserState = new SysUserState();
            mSysUserState.IS_LOGIN = false;

            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_BLOCK @user_id = {0},@block = {1}"
            , _class.USER_ID
            , 1

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    mSysUserState.USER_ID = Convert.ToInt32(_ds.Tables[0].Rows[0]["USER_ID"]);
                    mSysUserState.ERROR = Convert.ToInt32(_ds.Tables[0].Rows[0]["ERROR"]);
                    mSysUserState.ERROR_MESSAGE = _ds.Tables[0].Rows[0]["ERROR_MESSAGE"].ToString();
                    if (mSysUserState.ERROR == 0 && mSysUserState.USER_ID > 0)
                    {
                        mSysUserState.IS_LOGIN = true;
                    }

                }
            }
            catch { }


            return mSysUserState;
        }
        #endregion

        #region SysUserUnblock
        [WebMethod]

        public SysUserState SysUserUnblock(SysUser _class)
        {
            SysUserState mSysUserState = new SysUserState();
            mSysUserState.IS_LOGIN = false;

            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_BLOCK @user_id = {0},@block = {1}"
            , _class.USER_ID
            , 0

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    mSysUserState.USER_ID = Convert.ToInt32(_ds.Tables[0].Rows[0]["USER_ID"]);
                    mSysUserState.ERROR = Convert.ToInt32(_ds.Tables[0].Rows[0]["ERROR"]);
                    mSysUserState.ERROR_MESSAGE = _ds.Tables[0].Rows[0]["ERROR_MESSAGE"].ToString();
                    if (mSysUserState.ERROR == 0 && mSysUserState.USER_ID > 0)
                    {
                        mSysUserState.IS_LOGIN = true;
                    }

                }
            }
            catch { }


            return mSysUserState;
        }
        #endregion

        #region SysUserChangePassword
        [WebMethod]
        public SysUserState SysUserChangePassword(SysUser _class)
        {
            SysUserState mSysUserState = new SysUserState();
            mSysUserState.IS_LOGIN = false;

            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_CHANGE_PASS @user_id = {0},@pass = {1}"
            , _class.USER_ID
            , StringFunctions.SqlQuoted(_class.PASSWORD, true)

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    mSysUserState.USER_ID = Convert.ToInt32(_ds.Tables[0].Rows[0]["USER_ID"]);
                    mSysUserState.ERROR = Convert.ToInt32(_ds.Tables[0].Rows[0]["ERROR"]);
                    mSysUserState.ERROR_MESSAGE = _ds.Tables[0].Rows[0]["ERROR_MESSAGE"].ToString();
                    if (mSysUserState.ERROR == 0 && mSysUserState.USER_ID > 0)
                    {
                        mSysUserState.IS_LOGIN = true;
                    }

                }
            }
            catch { }


            return mSysUserState;
        }
        #endregion

        #region SysUserResetPassword
        [WebMethod]
        public string SysUserResetPassword(SysUser _class)
        {
            string mResult = "";
            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_RESET_PASS @user_id = {0}"
            , _class.USER_ID
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    mResult = _ds.Tables[0].Rows[0]["DEFAULT_PASSWORD"].ToString();
                }
            }
            catch { }
            return mResult;
        }
        #endregion

        #region SysUserLogin
        [WebMethod]

        public SysUserState SysUserLogin(SysUser _class)
        {
            SysUserState mSysUserState = new SysUserState();
            mSysUserState.IS_LOGIN = false;

            try
            {
                string mSQL = String.Format(@"EXEC dbo.pSYS_USER_LOGIN @login = {0},@pass = {1}"
                , StringFunctions.SqlQuoted(_class.LOGIN_NAME, true)
                , StringFunctions.SqlQuoted(_class.PASSWORD, true)

                );
                try
                {
                    DataSet _ds;
                    _ds = _DGate.GetDataSet(mSQL);
                    if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                    {
                        mSysUserState.USER_ID = Convert.ToInt32(_ds.Tables[0].Rows[0]["USER_ID"]);
                        mSysUserState.ERROR = Convert.ToInt32(_ds.Tables[0].Rows[0]["ERROR"]);
                        mSysUserState.ERROR_MESSAGE = _ds.Tables[0].Rows[0]["ERROR_MESSAGE"].ToString();
                        if (mSysUserState.ERROR == 0 && mSysUserState.USER_ID > 0)
                        {
                            mSysUserState.IS_LOGIN = true;
                        }

                    }
                }
                catch { }
            }
            catch { }

            return mSysUserState;
        }

        [WebMethod]

        public SysUserState SysUserLogin2(string mUser, string mPass)
        {
            SysUserState mSysUserState = new SysUserState();
            mSysUserState.IS_LOGIN = false;

            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_LOGIN @login = {0},@pass = {1}"
            , StringFunctions.SqlQuoted(mUser, true)
            , StringFunctions.SqlQuoted(mPass, true)

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    mSysUserState.USER_ID = Convert.ToInt32(_ds.Tables[0].Rows[0]["USER_ID"]);
                    mSysUserState.ERROR = Convert.ToInt32(_ds.Tables[0].Rows[0]["ERROR"]);
                    mSysUserState.ERROR_MESSAGE = _ds.Tables[0].Rows[0]["ERROR_MESSAGE"].ToString();
                    if (mSysUserState.ERROR == 0 && mSysUserState.USER_ID > 0)
                    {
                        mSysUserState.IS_LOGIN = true;
                    }

                }
            }
            catch { }


            return mSysUserState;
        }

        #endregion

        #region SysUserLoad
        [WebMethod]
        public SysUser SysUserLoad(int userID)
        {
            SysUser mSysUser = new SysUser();

            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_GET @user_id = {0}"
            , CheckNull(userID)
            );

            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                SysUser mClass = new SysUser();
                mSysUser = (SysUser)FillClass(mSysUser, _ds.Tables[0].Rows[0]);
            }
            catch { }
            return mSysUser;
        }


        [WebMethod]
        public SysUser TechUserLoad(int branchID)
        {
            SysUser mSysUser = new SysUser();

            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_GET_TECH @BRANCH_ID = {0}", this.CheckNull(branchID));
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                SysUser mClass = new SysUser();
                mSysUser = (SysUser)FillClass(mSysUser, _ds.Tables[0].Rows[0]);
            }
            catch { }

            return mSysUser;
        }

        #endregion

        #region SysUserSave
        [WebMethod]

        public bool SysUserSave(SysUser _class)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pSYS_USER_SET @branch_id = {0},@user_id = {1},@login_name = {2},@full_name = {3},@is_disabled = {4},@domain_user_name = {5},@password = {6},@password_change_date = {7},@last_login = {8},@login_count = {9},@need_rs = 1"
            , StringFunctions.SqlQuoted(_class.BRANCH_ID, true)
            , _class.USER_ID
            , StringFunctions.SqlQuoted(_class.LOGIN_NAME, true)
            , StringFunctions.SqlQuoted(_class.FULL_NAME, true)
            , _class.IS_DISABLED
            , StringFunctions.SqlQuoted(_class.DOMAIN_USER_NAME, true)
            , StringFunctions.SqlQuoted(_class.PASSWORD, true)
            , StringFunctions.SqlDateQuoted(_class.PASSWORD_CHANGE_DATE)
            , StringFunctions.SqlDateQuoted(_class.LAST_LOGIN)
            , _class.LOGIN_COUNT

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region SysUserAddToGroup
        [WebMethod]

        public string SysUserAddToGroup(SysUser _class, int mGroupId)
        {
            string retValue = "";
            string mSQL = String.Format(@"EXEC dbo.pUTIL_USER_ADDTOGROUP @user_id = {0},@user_group = {1}"
            , _class.USER_ID
            , mGroupId
            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = _ds.Tables[0].Rows[0]["DEFAULT_PASSWORD"].ToString();
                }
            }
            catch { }
            return retValue;
        }
        #endregion


        #endregion

        #region == SysUser params ==
        #region SysUserEventsList
        [WebMethod]

        public List<SysUserEvents> SysUserEventsList(int userID)
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_EVENT_MAP_LIST @user_id = {0}", userID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysUserEvents> RetList = new List<SysUserEvents>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserEvents mClass = new SysUserEvents();
                mClass = (SysUserEvents)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysUserGroupList
        [WebMethod]

        public List<SysUserGroup> SysUserGroupList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_GROUP_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysUserGroup> RetList = new List<SysUserGroup>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserGroup mClass = new SysUserGroup();
                mClass = (SysUserGroup)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysUserGroupMapList
        [WebMethod]

        public List<SysUserGroupMap> SysUserGroupMapList(int UserID)
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_GROUP_MAP_LIST @user_id = {0}", UserID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysUserGroupMap> RetList = new List<SysUserGroupMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserGroupMap mClass = new SysUserGroupMap();
                mClass = (SysUserGroupMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysUserGroupMapeSave
        [WebMethod]

        public bool SysUserGroupMapeSave(SysUserGroupMap _class)
        {
            bool retValue = false;
            string _procedure = "";
            if (_class.IS_ACTIVE)
            {
                _procedure = "pSYS_USER_GROUP_MAP_SET";
            }
            else
            {
                _procedure = "pSYS_USER_GROUP_MAP_DEL";
            }
            string mSQL = String.Format(@"EXEC dbo." + _procedure + " @id = {0},@user_id = {1}"
            , _class.USER_GROUP_ID
            , _class.USER_ID

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region SysUserList
        [WebMethod]

        public List<SysUser> SysUserList(int mUserGroupID)
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_LIST @user_group_id = {0}", mUserGroupID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysUser> RetList = new List<SysUser>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUser mClass = new SysUser();
                mClass = (SysUser)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysUserParametersLoad
        [WebMethod]

        public SysUserParameters SysUserParametersLoad(int user_id)
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_PARAMETERS @user_id = {0}", user_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            SysUserParameters _SysUserParameters = new SysUserParameters();

            if (mDataTable.Rows.Count == 0)
            {
                return _SysUserParameters;
            }
            /*User*/

            SysUser mUser = new SysUser();
            mUser = (SysUser)FillClass(mUser, mDataTable.Rows[0]);
            _SysUserParameters.User = mUser;
            /*WorkPlace*/
            mDataTable = _DGate.GetDataSet(mSQL).Tables[1];
            List<SysUserWorkPlace> _SysUserWorkPlaceList = new List<SysUserWorkPlace>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserWorkPlace mWorkPlace = new SysUserWorkPlace();
                mWorkPlace = (SysUserWorkPlace)FillClass(mWorkPlace, mDataRow);
                _SysUserWorkPlaceList.Add(mWorkPlace);
            }
            _SysUserParameters.WorkPlaceList = _SysUserWorkPlaceList;
            /*Committee*/
            mDataTable = _DGate.GetDataSet(mSQL).Tables[2];
            List<UserCommittee> _UserCommitteeList = new List<UserCommittee>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                UserCommittee mUserCommittee = new UserCommittee();
                mUserCommittee = (UserCommittee)FillClass(mUserCommittee, mDataRow);
                _UserCommitteeList.Add(mUserCommittee);
            }
            _SysUserParameters.CommitteeList = _UserCommitteeList;

            /*CommitteeDelegate*/
            mDataTable = _DGate.GetDataSet(mSQL).Tables[3];
            List<CommitteeDelegate> _UserCommitteeDelegateList = new List<CommitteeDelegate>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CommitteeDelegate mCommitteeDelegate = new CommitteeDelegate();
                mCommitteeDelegate = (CommitteeDelegate)FillClass(mCommitteeDelegate, mDataRow);
                _UserCommitteeDelegateList.Add(mCommitteeDelegate);
            }
            _SysUserParameters.CommitteeDelegateList = _UserCommitteeDelegateList;

            /*Groups*/
            mDataTable = _DGate.GetDataSet(mSQL).Tables[4];
            List<SysUserGroupMap> _SysUserGroupMapList = new List<SysUserGroupMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserGroupMap mSysUserGroupMap = new SysUserGroupMap();
                mSysUserGroupMap = (SysUserGroupMap)FillClass(mSysUserGroupMap, mDataRow);
                _SysUserGroupMapList.Add(mSysUserGroupMap);
            }
            _SysUserParameters.GroupList = _SysUserGroupMapList;
            /*Events*/
            mDataTable = _DGate.GetDataSet(mSQL).Tables[5];
            List<SysUserEvents> _SysUserEventsList = new List<SysUserEvents>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserEvents mSysUserEvents = new SysUserEvents();
                mSysUserEvents = (SysUserEvents)FillClass(mSysUserEvents, mDataRow);
                _SysUserEventsList.Add(mSysUserEvents);
            }
            _SysUserParameters.EventsList = _SysUserEventsList;
            return _SysUserParameters;
        }
        #endregion

        #region SysUserParametersSave
        [WebMethod]

        public bool SysUserParametersSave(SysUserParameters sysUserParameters)
        {

            SysUserSave(sysUserParameters.User);

            for (int i = 0; i < sysUserParameters.WorkPlaceList.Count; i++)
            {
                SysUserWorkPlaceSave(sysUserParameters.WorkPlaceList[i]);
            }
            for (int i = 0; i < sysUserParameters.CommitteeList.Count; i++)
            {
                UserCommitteeSave(sysUserParameters.CommitteeList[i]);
            }
            for (int i = 0; i < sysUserParameters.CommitteeList.Count; i++)
            {
                CommitteeDelegateSave(sysUserParameters.CommitteeDelegateList[i]);
            }
            for (int i = 0; i < sysUserParameters.CommitteeList.Count; i++)
            {
                SysUserGroupMapeSave(sysUserParameters.GroupList[i]);
            }

            return true;
        }
        #endregion

        #region SysUserWorkPlaceList
        [WebMethod]

        public List<SysUserWorkPlace> SysUserWorkPlaceList()
        {
            string mSQL = String.Format("EXEC dbo.pSYS_USER_WORK_PLACE_MAP_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SysUserWorkPlace> RetList = new List<SysUserWorkPlace>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SysUserWorkPlace mClass = new SysUserWorkPlace();
                mClass = (SysUserWorkPlace)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region SysUserWorkPlaceSave
        [WebMethod]

        public bool SysUserWorkPlaceSave(SysUserWorkPlace _class)
        {
            bool retValue = false;
            string _procedure = "";
            if (_class.IS_ACTIVE)
            {
                _procedure = "pSYS_USER_WORK_PLACE_MAP_SET";
            }
            else
            {
                _procedure = "pSYS_USER_WORK_PLACE_MAP_DEL";
            }
            string mSQL = String.Format(@"EXEC dbo." + _procedure + " @id = {0},@user_id = {1}"
            , _class.WORK_PLACE_ID
            , _class.USER_ID

            );
            try
            {
                DataSet _ds;
                _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #endregion
        #endregion

        #endregion

        #region TermTypeList
        [WebMethod]

        public List<PipelineWS.LocalModel.TermType> TermTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_TERM_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PipelineWS.LocalModel.TermType> RetList = new List<PipelineWS.LocalModel.TermType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PipelineWS.LocalModel.TermType mClass = new PipelineWS.LocalModel.TermType();
                mClass = (PipelineWS.LocalModel.TermType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region ValRatesList
        [WebMethod]

        public List<ValRates> ValRatesList()
        {
            string mSQL = String.Format("EXEC dbo.pVAL_RATES_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ValRates> RetList = new List<ValRates>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ValRates mClass = new ValRates();
                mClass = (ValRates)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region WorkPlaceList
        [WebMethod]

        public List<WorkPlace> WorkPlaceList(string work_place_sys_name, int current_work_place)
        {
            string mSQL = String.Format("EXEC dbo.pREF_WORK_PLACE_LIST @work_place_sys_name = {0},@current_work_place = {1}", StringFunctions.SqlQuoted(work_place_sys_name), current_work_place);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<WorkPlace> RetList = new List<WorkPlace>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                WorkPlace mClass = new WorkPlace();
                mClass = (WorkPlace)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region YearMonthList
        [WebMethod]

        public List<YearMonth> YearMonthList()
        {
            string mSQL = String.Format("EXEC dbo.pYEAR_MONTH_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<YearMonth> RetList = new List<YearMonth>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                YearMonth mClass = new YearMonth();
                mClass = (YearMonth)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region WorkDay
        [WebMethod]

        public WorkDay WorkDayGet(DateTime mDT)
        {
            string mSQL = String.Format("EXEC dbo.vtb_work_day_get @dt = {0},@day_shift = 0", StringFunctions.SqlDateQuoted(mDT));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            WorkDay mClass = new WorkDay();
            mClass = (WorkDay)FillClass(mClass, mDataTable.Rows[0]);
            return mClass;
        }

        [WebMethod]

        public WorkDay WorkDaysAdd(DateTime mDT, int mDaysToAdd)
        {
            string mSQL = String.Format("EXEC dbo.work_days_add @dt = {0},@days_to_add = {1}, @need_rs = 1", StringFunctions.SqlDateQuoted(mDT), mDaysToAdd);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            WorkDay mClass = new WorkDay();
            mClass = (WorkDay)FillClass(mClass, mDataTable.Rows[0]);
            return mClass;
        }
        #endregion

        #region SebClass
        [WebMethod]

        public List<SebClass> SebClassList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_SEB_CLASS_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SebClass> RetList = new List<SebClass>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SebClass mClass = new SebClass();
                mClass = (SebClass)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion SebClass

        #region InsuranceProductsList
        [WebMethod]

        public List<InsuranceProducts> InsuranceProductsList()
        {
            List<InsuranceProducts> RetList = new List<InsuranceProducts>();
            try
            {
                string mSQL = String.Format("EXEC dbo.pREF_INSURANCE_PRODUCTS");
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                foreach (DataRow mDataRow in mDataTable.Rows)
                {
                    InsuranceProducts mClass = new InsuranceProducts();
                    mClass = (InsuranceProducts)FillClass(mClass, mDataRow);
                    RetList.Add(mClass);
                }
            }
            catch { }
            return RetList;
        }
        #endregion

        #region PictureObjectTypeList
        [WebMethod]

        public List<PictureObjectType> PictureObjectTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_OBJECT_TYPE_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PictureObjectType> RetList = new List<PictureObjectType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PictureObjectType mClass = new PictureObjectType();
                mClass = (PictureObjectType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion PictureObjectTypeList

        #region BaseProductPurposeList
        [WebMethod]
        public List<BaseProductPurpose> BaseProductPurposeList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_BASE_PRODUCT_PURPOSE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<BaseProductPurpose> RetList = new List<BaseProductPurpose>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                BaseProductPurpose mClass = new BaseProductPurpose();
                mClass = (BaseProductPurpose)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion BaseProductPurposeList

        #region ProductAttributeList
        [WebMethod]

        public List<ProductAttribute> ProductAttributeList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_PRODUCT_ATTRIBUTES_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductAttribute> RetList = new List<ProductAttribute>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductAttribute mClass = new ProductAttribute();
                mClass = (ProductAttribute)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]

        public List<ProductAttribute> ApplicationProductAttributesGet(int application_id)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_PRODUCT_ATTRIBUTES_GET @application_id = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductAttribute> RetList = new List<ProductAttribute>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductAttribute mClass = new ProductAttribute();
                mClass = (ProductAttribute)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }


        private ProductAttribute GetProductAttribute(int mProductID, string mAttribCode, int mClientSalaryCategory)
        {
            List<ProductAttribute> mProductAttributeList = this.ProductAttributeList();
            ProductAttribute mProductAttribute = null;

            try
            {
                mProductAttribute = mProductAttributeList.Where(item => item.PRODUCT_ID == mProductID && item.ATTRIB_CODE == mAttribCode && item.CLIENT_SALARY_CATEGORY == mClientSalaryCategory).First();
            }
            catch { }

            return mProductAttribute;
        }

        #endregion ProductAttributeList

        #region AdditionalAttributeList
        [WebMethod]
        public List<AdditionalAttribute> AdditionalAttributeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ADDITIONAL_ATTRIBUTES_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<AdditionalAttribute> RetList = new List<AdditionalAttribute>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                AdditionalAttribute mClass = new AdditionalAttribute();
                mClass = (AdditionalAttribute)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        private AdditionalAttribute GetProductAttribute(string mAttribCode)
        {
            List<AdditionalAttribute> mAdditionalAttributeList = this.AdditionalAttributeList();
            AdditionalAttribute mAdditionalAttribute = null;

            try
            {
                mAdditionalAttribute = mAdditionalAttributeList.Where(item => item.ATTRIBUTE_CODE == mAttribCode).First();
            }
            catch { }

            return mAdditionalAttribute;
        }

        #endregion AdditionalAttributeList

        #region === CreditInfo ===

        #region CreditInfoTranslate
        [WebMethod]
        public List<CreditInfoTranslate> CreditInfoTranslateList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CREDIT_INFO_TRANSLATE_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CreditInfoTranslate> RetList = new List<CreditInfoTranslate>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CreditInfoTranslate mClass = new CreditInfoTranslate();
                mClass = (CreditInfoTranslate)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion CreditInfoTranslate

        #endregion === End CreditInfo ===

        #region Installment
        [WebMethod]
        public List<Installment> InstallmentList()
        {
            string mSQL = String.Format("EXEC dbo.pINSTALLMENTS_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Installment> RetList = new List<Installment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Installment mClass = new Installment();
                mClass = (Installment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<Installment> InstallmentListByTaxCode(string taxCode)
        {
            string mSQL = String.Format("EXEC dbo.pINSTALLMENTS_LIST_BY_TAX_CODE @TAX_CODE = {0}", StringFunctions.SqlQuoted(taxCode, true));
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Installment> RetList = new List<Installment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Installment mClass = new Installment();
                mClass = (Installment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        #endregion Installment

        #region MAP Installment Product
        [WebMethod]
        public List<InstallmentForMap> ForMapInstallmentList()
        {
            string mSQL = String.Format("EXEC dbo.pFOR_MAP_INSTALLMENTS_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InstallmentForMap> RetList = new List<InstallmentForMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InstallmentForMap mClass = new InstallmentForMap();
                mClass = (InstallmentForMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<InstallmentProductForMap> ForMapInstallmentProduct(int mInstallmentID)
        {
            string mSQL = String.Format("EXEC dbo.pFOR_MAP_INSTALLMENT_PRODUCT @INSTALLMENT_ID = {0}", mInstallmentID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InstallmentProductForMap> RetList = new List<InstallmentProductForMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InstallmentProductForMap mClass = new InstallmentProductForMap();
                mClass = (InstallmentProductForMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public bool MapInstallmentProducDelete(int installmentId)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pFOR_MAP_INSTALLMENT_PRODUCT_DELETE @INSTALLMENT_ID = {0}", installmentId);

            try
            {
                DataSet _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }

        [WebMethod]
        public bool MapInstallmentProductInsert(List<InstallmentProductForMap> InstallmentProductList)
        {
            InstallmentProductForMap InstallmentProduct = InstallmentProductList.First();
            this.MapInstallmentProducDelete(InstallmentProduct.INSTALLMENT_ID);

            foreach (InstallmentProductForMap item in InstallmentProductList)
            {
                string mSQL = String.Format("EXEC dbo.pFOR_MAP_INSTALLMENT_PRODUCT_INSERT @INSTALLMENT_ID = {0}, @PRODUCT_ID = {1}", item.INSTALLMENT_ID, item.PRODUCT_ID);
                _DGate.GetDataSet(mSQL);
            }

            return true;
        }

        [WebMethod]
        public List<LocalModel.Product> InstallmentProductByCategory(int installmentId, int categoryId)
        {
            string mSQL = String.Format("EXEC dbo.pINSTALLMENT_PRODUCT_BY_CATEGORY @INSTALLMENT_ID = {0}, @CATEGORY_ID = {1} ", installmentId, categoryId);

            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<LocalModel.Product> RetList = new List<LocalModel.Product>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LocalModel.Product mClass = new LocalModel.Product();
                mClass = (LocalModel.Product)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }

            return RetList;
        }
        #endregion MAP Installment Product

        #region PurchaseItemGroupList
        [WebMethod]
        public List<PurchaseItemGroup> PurchaseItemGroupList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PURCHASE_ITEM_GROUP_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PurchaseItemGroup> RetList = new List<PurchaseItemGroup>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PurchaseItemGroup mClass = new PurchaseItemGroup();
                mClass = (PurchaseItemGroup)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion PurchaseItemGroupList

        #region PurchaseItemList
        [WebMethod]
        public List<PurchaseItem> PurchaseItemList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PURCHASE_ITEM_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PurchaseItem> RetList = new List<PurchaseItem>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PurchaseItem mClass = new PurchaseItem();
                mClass = (PurchaseItem)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion PurchaseItemList

        #region InstalmentsUser
        [WebMethod]
        public List<InstalmentsUser> InstalmentsUserList(int mUser_id)
        {
            string mSQL = String.Format("EXEC dbo.pMAP_USER_INSTALLMENT_LIST @user_id = {0} ", mUser_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InstalmentsUser> RetList = new List<InstalmentsUser>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InstalmentsUser mClass = new InstalmentsUser();
                mClass = (InstalmentsUser)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<SaleUserInstalment> SaleUserInstalmentList(int mSaleUserID)
        {
            string mSQL = String.Format("EXEC dbo.pSALE_USER_INSTALLMENT_LIST @sale_user_id = {0} ", mSaleUserID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SaleUserInstalment> RetList = new List<SaleUserInstalment>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SaleUserInstalment mClass = new SaleUserInstalment();
                mClass = (SaleUserInstalment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<UserInstalmentsProduct> UserInstalmentsProductList(int mUser_id)
        {
            string mSQL = String.Format("EXEC dbo.pMAP_USER_INSTALLMENT_PRODUCT_LIST @user_id = {0} ", mUser_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<UserInstalmentsProduct> RetList = new List<UserInstalmentsProduct>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                UserInstalmentsProduct mClass = new UserInstalmentsProduct();
                mClass = (UserInstalmentsProduct)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public bool UserInstalmentsProductListSave(int mUser_id, List<SaleUserInstalment> mSaleUserInstalmentList)
        {
            string mSQL = String.Format("EXEC dbo.pMAP_USER_INSTALLMENT_DEL @USER_ID = {0}", mUser_id);
            _DGate.Execute(mSQL);
            List<SaleUserInstalment> _SaleUserInstalmentList = mSaleUserInstalmentList.Where(e => e.IS_ACTIVE).ToList();
            foreach (SaleUserInstalment mSaleUserInstalment in _SaleUserInstalmentList)
            {
                mSQL = String.Format("EXEC dbo.pMAP_USER_INSTALLMENT_SET @USER_ID = {0}	, @INSTALLMENT_ID = {1}", mUser_id, mSaleUserInstalment.INSTALLMENT_ID);
                _DGate.Execute(mSQL);
            }
            return true;
        }

        #endregion InstalmentsUser

        #region ApplicationOwnerInstalment
        [WebMethod]
        public List<ApplicationOwnerInstalment> ApplicationOwnerInstalmentList(int mApplicationId)
        {
            List<ApplicationOwnerInstalment> RetList = new List<ApplicationOwnerInstalment>();

            string mSQL = String.Format("EXEC dbo.pAPPLICATION_OWNER_INSTALLMENT_LIST @application_id = {0} ", mApplicationId);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ApplicationOwnerInstalment mClass = new ApplicationOwnerInstalment();
                mClass = (ApplicationOwnerInstalment)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ApplicationOwnerInstalment

        #region ProductInstallment
        [WebMethod]
        public List<ProductInstallmentDiscount> ProductInstallmentList()
        {
            string mSQL = String.Format("EXEC dbo.pPRODUCT_INSTALLMENT_LIST ");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductInstallmentDiscount> RetList = new List<ProductInstallmentDiscount>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductInstallmentDiscount mClass = new ProductInstallmentDiscount();
                mClass = (ProductInstallmentDiscount)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion InstalmentsUser

        #region MapProductInstallmentDiscount
        [WebMethod]
        public List<ProductInstallmentDiscount> ProductInstallmentDiscountList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_PRODUCT_INSTALLMENT_DISCOUNT_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ProductInstallmentDiscount> RetList = new List<ProductInstallmentDiscount>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ProductInstallmentDiscount mClass = new ProductInstallmentDiscount();
                mClass = (ProductInstallmentDiscount)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion InstalmentsUser


        #region CashCoverScheduleType
        [WebMethod]
        public List<CashCoverScheduleType> CashCoverScheduleTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_CASH_COVER_SCHEDULE_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<CashCoverScheduleType> RetList = new List<CashCoverScheduleType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                CashCoverScheduleType mClass = new CashCoverScheduleType();
                mClass = (CashCoverScheduleType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion CashCoverScheduleType

        #region MFiles
        [WebMethod]
        public List<MFBranchMap> MFBranchMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_MFILES_BRANCH_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<MFBranchMap> RetList = new List<MFBranchMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                MFBranchMap mClass = new MFBranchMap();
                mClass = (MFBranchMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<MFUserMap> MFUserMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_MFILES_USER_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<MFUserMap> RetList = new List<MFUserMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                MFUserMap mClass = new MFUserMap();
                mClass = (MFUserMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<MFCaseProductMap> MFCaseProductMapList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_MFILES_CASE_PRODUCT_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<MFCaseProductMap> RetList = new List<MFCaseProductMap>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                MFCaseProductMap mClass = new MFCaseProductMap();
                mClass = (MFCaseProductMap)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion MFiles

        #region NONSTANDARD_TYPE
        [WebMethod]

        public List<NonStandardType> NonStandardTypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_NONSTANDARD_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<NonStandardType> RetList = new List<NonStandardType>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                NonStandardType mClass = new NonStandardType();
                mClass = (NonStandardType)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion NONSTANDARD_TYPE_ID


        #region INSURANCE_MANDATORY
        [WebMethod]

        public List<InsuranceMandatory> InsuranceMandatoryList()
        {
            string mSQL = String.Format("EXEC dbo.pMAP_INSURANCE_MANDATORY");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<InsuranceMandatory> RetList = new List<InsuranceMandatory>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                InsuranceMandatory mClass = new InsuranceMandatory();
                mClass = (InsuranceMandatory)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        public InsuranceMandatoReesult LifeInsuranceMandatoryGet(int application_id)
        {
            string mSQL = String.Format("EXEC pINSURANCE_MANDATORY_GET @application_id = {0}", application_id);
            InsuranceMandatoReesult mClass = new InsuranceMandatoReesult();
            mClass.IS_LIFE = false;
            mClass.COMPANY_ID = 0;
            try
            {
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                mClass = (InsuranceMandatoReesult)FillClass(mClass, mDataTable.Rows[0]);
            }
            catch { }
            return mClass;
        }

        #endregion INSURANCE_MANDATORY

        #region ClientPositionTtype
        [WebMethod]

        public List<ClientPositionTtype> ClientPositionTtypeList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_POSITION_TYPE_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ClientPositionTtype> RetList = new List<ClientPositionTtype>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ClientPositionTtype mClass = new ClientPositionTtype();
                mClass = (ClientPositionTtype)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion ClientPositionTtype

        #region Rus Scoring References
        [WebMethod]
        public List<Education> EducationList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_EDUCATION_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<Education> RetList = new List<Education>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                Education mClass = new Education();
                mClass = (Education)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<PostCategory> PostCategoryList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_POST_CATEGORY_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<PostCategory> RetList = new List<PostCategory>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                PostCategory mClass = new PostCategory();
                mClass = (PostCategory)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<ActivityArea> ActivityAreaList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_ACTIVITY_AREA_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<ActivityArea> RetList = new List<ActivityArea>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                ActivityArea mClass = new ActivityArea();
                mClass = (ActivityArea)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<EmploeeCount> EmploeeCountList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_EMPLOEE_COUNT_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<EmploeeCount> RetList = new List<EmploeeCount>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                EmploeeCount mClass = new EmploeeCount();
                mClass = (EmploeeCount)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion Rus Scoring References

        #endregion

        #region == Loan Restruct ==
        [WebMethod]
        public List<LoanRestruct> LoanRestructList(int UserId)
        {
            string mSQL = String.Format("EXEC dbo.pLOAN_RESTRUCT_LIST @user_id = {0}", UserId);
            mDataTable = _DGate.GetDataSet(mSQL, 300).Tables[0];
            List<LoanRestruct> RetList = new List<LoanRestruct>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                LoanRestruct mClass = new LoanRestruct();
                mClass = (LoanRestruct)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }

            return RetList;
        }

        [WebMethod]
        public LoanRestruct LoanRestructRecord(int RecId)
        {
            LoanRestruct _class = new LoanRestruct();
            string mSQL = String.Format(@"EXEC dbo.pLOAN_RESTRUCT_RECORD @rec_id = {0}", RecId);
            try
            {
                DataSet _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    _class = (LoanRestruct)FillClass(_class, _ds.Tables[0].Rows[0]);
                }
            }
            catch { }
            return _class;
        }

        [WebMethod]
        public LoanRestruct LoanRestructLMS(int LoanId, int UserId)
        {
            LoanRestruct _class = new LoanRestruct();
            string mSQL = String.Format(@"EXEC dbo.pLOAN_RESTRUCT_LMS_GET @LOAN_ID = {0}, @USER_ID = {1}", LoanId, UserId);
            try
            {
                DataSet _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0)
                {
                    _class = (LoanRestruct)FillClass(_class, _ds.Tables[0].Rows[0]);
                }
            }
            catch { }
            return _class;
        }

        [WebMethod]

        public bool LoanRestructSave(LoanRestruct loanRestruct)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pLOAN_RESTRUCT_SET 
                  @REC_ID = {0}, @LOAN_ID = {1}, @CLIENT_ID = {2}, @PERSONAL_ID = {3}, @BIRTH_DATE = {4}, @PRODUCT_ID = {5}
	            , @LOAN_AMOUNT = {6}, @LOAN_CCY = {7}, @INTEREST_RATE = {8}, @COMMISSION_RATE = {9}, @DATE_START = {10}
	            , @DATE_END = {11}, @PMT_AMOUNT = {12}, @PAYD_PERIOD = {13}, @SCHEDULE_FIRST_DATE = {14}, @DEBT_AMOUNT = {15}
	            , @OVERDUE_AMOUNT = {16}, @ORG_CATEGORY = {17}, @IS_SALARY = {18}, @GENERAL_AMOUNT = {19}, @GENERAL_END_YEAR = {20}
	            , @IS_COMMERCIAL = {21}, @RESTRUCT_TYPE_ID = {22}, @RESTRUCT_DESCRIP = {23}, @COLLATERAL_DESCRIP = {24}, @CREATE_DATE = {25}
                , @CREATE_USER_ID = {26}, @CREATE_BRANCH_ID = {27}, @DECISION_ID = {28}, @RISK_DESCRIP = {29}, @DECISION_USER_ID = {30}
                , @DECISION_DATE = {31}, @IS_COMPLETED = {32}, @COMPLETED_USER_ID = {33}, @COMPLETED_DATE = {34}"
                , loanRestruct.REC_ID
                , loanRestruct.LOAN_ID
                , loanRestruct.CLIENT_ID
                , StringFunctions.SqlQuoted(loanRestruct.PERSONAL_ID, true)
                , StringFunctions.SqlDateQuoted(loanRestruct.BIRTH_DATE)
                , loanRestruct.PRODUCT_ID

                , loanRestruct.LOAN_AMOUNT
                , StringFunctions.SqlQuoted(loanRestruct.LOAN_CCY, true)
                , loanRestruct.INTEREST_RATE
                , loanRestruct.COMMISSION_RATE
                , StringFunctions.SqlDateQuoted(loanRestruct.DATE_START)

                , StringFunctions.SqlDateQuoted(loanRestruct.DATE_END)
                , loanRestruct.PMT_AMOUNT
                , loanRestruct.PAYD_PERIOD
                , StringFunctions.SqlDateQuoted(loanRestruct.SCHEDULE_FIRST_DATE)
                , loanRestruct.DEBT_AMOUNT

                , loanRestruct.OVERDUE_AMOUNT
                , loanRestruct.ORG_CATEGORY
                , loanRestruct.IS_SALARY
                , loanRestruct.GENERAL_AMOUNT
                , loanRestruct.GENERAL_END_YEAR

                , loanRestruct.IS_COMMERCIAL
                , loanRestruct.RESTRUCT_TYPE_ID
                , StringFunctions.SqlQuoted(loanRestruct.RESTRUCT_DESCRIP, true)
                , StringFunctions.SqlQuoted(loanRestruct.COLLATERAL_DESCRIP, true)
                , StringFunctions.SqlDateQuoted(loanRestruct.CREATE_DATE)

                , loanRestruct.CREATE_USER_ID
                , loanRestruct.CREATE_BRANCH_ID
                , loanRestruct.DECISION_ID
                , StringFunctions.SqlQuoted(loanRestruct.RISK_DESCRIP, true)
                , loanRestruct.DECISION_USER_ID

                , StringFunctions.SqlDateQuoted(loanRestruct.DECISION_DATE)
                , loanRestruct.IS_COMPLETED
                , loanRestruct.COMPLETED_USER_ID
                , StringFunctions.SqlDateQuoted(loanRestruct.COMPLETED_DATE)
            );

            try
            {
                DataSet _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }

        [WebMethod]
        public bool LoanRestructDelete(int recId)
        {
            bool retValue = false;
            string mSQL = String.Format(@"EXEC dbo.pLOAN_RESTRUCT_DEL @REC_ID = {0}", recId);

            try
            {
                DataSet _ds = _DGate.GetDataSet(mSQL);
                if (_ds != null && _ds.Tables != null && _ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(_ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    retValue = true;
                }
            }
            catch { }
            return retValue;
        }
        #endregion

        #region static references
        [WebMethod]
        public StaticReferences StaticReferencesLoad()
        {
            StaticReferences _StaticReferences = new StaticReferences();

            _StaticReferences.ControlList = this.ApplicationControlList(0);
            _StaticReferences.PageList = this.ApplicationPageList(0);
            _StaticReferences.BranchList = this.BranchList();
            _StaticReferences.WorkPlaceList = this.WorkPlaceList("", 0);
            _StaticReferences.YearMonthList = this.YearMonthList();
            _StaticReferences.CurrencyList = this.CurrencyList();
            _StaticReferences.OrganizationList = this.OrganizationList();
            _StaticReferences.IncomeTypeList = this.IncomeTypeList();
            _StaticReferences.MaritalStatusList = this.MaritalStatusList();
            _StaticReferences.CountryList = this.CountryList();
            _StaticReferences.PassportTypeList = this.PassportTypeList();
            _StaticReferences.RelationshipTypeList = this.RelationshipTypeList();
            _StaticReferences.CollateralTypeList = this.CollateralTypeList();
            _StaticReferences.BankList = this.BankList();
            _StaticReferences.ApplicationStateMapList = this.ApplicationStateMapList();
            _StaticReferences.ProductList = this.ProductList(-1);
            _StaticReferences.LoanAimList = this.LoanAimList();
            _StaticReferences.ProductPropertiesList = this.ProductPropertiesList();
            _StaticReferences.RealEstateTypeList = this.RealEstateTypeList();
            _StaticReferences.RealEstateMaxLTVList = this.RealEstateMaxLTVList();
            _StaticReferences.CreditHistoryKindList = this.CreditHistoryKindList();
            _StaticReferences.ClientTypeList = this.ClientTypeList();
            _StaticReferences.ClientRankList = this.ClientRankList();
            _StaticReferences.ScheduleTypeList = this.ScheduleTypeList();
            _StaticReferences.PmtFreqTypeList = this.PmtFreqTypeList();
            _StaticReferences.EnshureTypeList = this.EnshureTypeList();
            _StaticReferences.PrepaymentRescheduleTypeList = this.PrepaymentRescheduleTypeList();
            _StaticReferences.LoanTypeList = this.LoanTypeList();
            _StaticReferences.CreditTypeList = this.CreditTypeList();
            _StaticReferences.TermTypeList = this.TermTypeList();
            _StaticReferences.RoleTypeList = this.RoleTypeList();
            _StaticReferences.LoanCollateralTypeList = this.LoanCollateralTypeList();
            _StaticReferences.PenaltySchemaList = this.PenaltySchemaList();
            _StaticReferences.PenaltyCalculationTypeList = this.PenaltyCalculationTypeList();
            _StaticReferences.RejectReasonList = this.RejectReasonList();
            _StaticReferences.ApplicationStateList = this.ApplicationStateList();
            _StaticReferences.CommitteePropertiesMapList = this.CommitteePropertiesMapList();
            _StaticReferences.ValRatesList = this.ValRatesList();
            _StaticReferences.ClientSalaryCategoryList = this.ClientSalaryCategoryList();
            _StaticReferences.CityList = this.CityList();
            _StaticReferences.CoborrowerRelationTypeList = this.CoborrowerRelationTypeList();
            _StaticReferences.StatusList = this.StatusList();
            _StaticReferences.EmployeeList = this.EmployeeList();
            _StaticReferences.LoanException = this.LoanExceptionGet();
            _StaticReferences.ProductDetailsList = this.ProductDetailsList();
            _StaticReferences.SexList = this.SexList();
            _StaticReferences.CrediInfoGradeList = this.CrediInfoGradeList();
            _StaticReferences.IncomeVerifiedList = this.IncomeVerifiedList();
            _StaticReferences.ScoringParamKoefList = this.ScoringParamCoefList();
            _StaticReferences.ProductCategoryMapList = this.ProductCategoryMapList();
            _StaticReferences.ProductCategoryList = this.ProductCategoryList();
            _StaticReferences.InformationSourceList = this.InformationSourceList();
            _StaticReferences.AutoDealerList = this.AutoDealerList();
            _StaticReferences.InsuranceCompanyList = this.InsuranceCompanyList();
            _StaticReferences.InsuranceTariffList = this.AutoInsuranceTariffList();
            _StaticReferences.InsuranceDriverPassengerList = this.AutoInsuranceDriverPassengerList();
            _StaticReferences.InsuranceThirdPartyList = this.AutoInsuranceThirdPartyList();
            _StaticReferences.SysParameterList = this.SysParameterList();
            _StaticReferences.SysCalcParameterList = this.SysCalcParameterList();
            _StaticReferences.DisbursTypeList = this.DisbursTypeList();
            _StaticReferences.LoanProductExceptionList = this.LoanExceptionList();
            _StaticReferences.LoanProductExceptionParamList = this.LoanProductExceptionParamList();
            _StaticReferences.SysUserGroupList = this.SysUserGroupList();
            _StaticReferences.CommitteeList = this.CommitteeList();
            _StaticReferences.CommitteeDelegateList = this.CommitteeDelegateList();
            _StaticReferences.CommitteeUserMapList = this.CommitteeUserList(0);
            _StaticReferences.PictureObjectTypeList = this.PictureObjectTypeList();
            _StaticReferences.CreditInfoTranslateList = this.CreditInfoTranslateList();
            _StaticReferences.AdditionalAttributeList = this.AdditionalAttributeList();
            _StaticReferences.InstallmentList = this.InstallmentList();
            _StaticReferences.PurchaseItemGroupList = this.PurchaseItemGroupList();
            _StaticReferences.PurchaseItemList = this.PurchaseItemList();
            _StaticReferences.ProductInstallmentList = this.ProductInstallmentList();
            _StaticReferences.ProductInstallmentDiscountList = this.ProductInstallmentDiscountList();

            _StaticReferences.InstalmentsUserList = this.InstalmentsUserList(0);
            _StaticReferences.UserInstalmentsProductList = this.UserInstalmentsProductList(0);
            _StaticReferences.ClientSubTypeList = this.ClientSubTypeList();
            _StaticReferences.BlackListItemList = this.BlackListItemList();
            _StaticReferences.AdminDebtTypes1List = this.AdminDebtTypes1List();
            _StaticReferences.AdminDebtTypes2List = this.AdminDebtTypes2List();
            _StaticReferences.AdminDebtObjectsList = this.AdminDebtObjectsList();
            _StaticReferences.SpecialLoanTypeList = this.SpecialLoanTypesGet();
            _StaticReferences.CashCoverScheduleTypeList = this.CashCoverScheduleTypeList();
            _StaticReferences.SebClassList = this.SebClassList();
            _StaticReferences.MFBranchMapList = this.MFBranchMapList();
            _StaticReferences.MFUserMapList = this.MFUserMapList();
            _StaticReferences.MFCaseProductMapList = this.MFCaseProductMapList();
            _StaticReferences.RealEstateInsuranceList = this.RealEstateInsuranceList();
            _StaticReferences.AccountProductList = this.AccountProductList();
            _StaticReferences.RestructTypeList = this.RestructTypeList();
            _StaticReferences.NonStandardTypeList = this.NonStandardTypeList();
            _StaticReferences.RefProductCategoryList = this.RefProductCategoryList();
            _StaticReferences.ProductRestructMapList = this.ProductRestructMapList();
            _StaticReferences.InsuranceProductsList = this.InsuranceProductsList();
            _StaticReferences.ClientPositionList = this.ClientPositionList();
            _StaticReferences.ClientConversationalLanguageList = this.ClientConversationalLanguageList();
            _StaticReferences.ClientPositionTtypeList = this.ClientPositionTtypeList();
            _StaticReferences.AdminDebtGroupList = this.AdminDebtGroupList();
            _StaticReferences.AdminDebtItemList = this.AdminDebtItemList();
            _StaticReferences.CreditBankList = this.CreditBankList();
            _StaticReferences.EducationList = this.EducationList();
            _StaticReferences.PostCategoryList = this.PostCategoryList();
            _StaticReferences.ActivityAreaList = this.ActivityAreaList();
            _StaticReferences.EmploeeCountList = this.EmploeeCountList();
            _StaticReferences.RealEstateOwnerTypeList = this.RealEstateOwnerTypeList();
            _StaticReferences.VirtualProductList = this.VirtualProductList();
            _StaticReferences.VirtualPercentList = this.VirtualPercentList();

            return _StaticReferences;
        }

        #endregion

        #region === alta LMS ===

        #region LMS private methods

        #region --LMSCangeLoanAmount--
        [WebMethod]
        public bool LMSCangeLoanAmount(int loan_id, decimal new_amount, int user_id, string branch_id)
        {
            try
            {
                var changeLoanAmount = new ChangeLoanAmountRequest();

                var loan = _lmsClient.GetLoan(new GetLoanRequest
                {
                    RequestHeaders = LMSRequestHeadersGet(),
                    LoanId = loan_id,
                    ControlFlags = LoanControlFlags.Basic
                }).Result;

                var changeLoanAmountResult = _lmsClient.ChangeLoanAmount(new ChangeLoanAmountRequest
                {
                    RequestHeaders = LMSRequestHeadersGet(),
                    Creator = new LAPI.Core.UserAndDeptId { UserIdentification = new LAPI.Core.UserIdentification { Id = user_id } },
                    LoanId = loan_id,
                    LoanAmount = new_amount,
                    Version = loan.Version.Value,
                    Flags = LoanOperationFlags.Default,
                    Description = "თანხის ცვლილება",
                    ChangeUnusedPrincipal = true
                });

                if (!string.IsNullOrEmpty(changeLoanAmountResult.OperationId))
                    _lmsClient.ExecuteOperationAction(new DefaultOperationActionRequest
                    {
                        RequestHeaders = LMSRequestHeadersGet(),
                        OperationId = changeLoanAmountResult.OperationId,
                        ETag = changeLoanAmountResult.ETag,
                        Action = LoanOperationAction.IncrementAuthorization,
                        User = new LAPI.Core.UserAndDeptId { UserIdentification = new LAPI.Core.UserIdentification { Id = user_id } }
                    });
            }
            catch (Exception Ex)
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, user_id, Ex.Message);
                return false;
            }
            finally
            {

            }
            return true;
        }

        #region MFileLinkAttributeSet
        
        public bool MFileLinkAttributeSet(int link_id, Attributes mAttributes)
        {
            foreach (KeyValuePair<string, string> attribute in mAttributes)
            {
                string mSQL = String.Format(String.Format("pLMS_MFILE_COLLATERAL_ATTRINUTE_SET @link_id = {0}, @application_mfile_attribute = {1},@application_mfile_attribute_name = {2}", link_id, StringFunctions.SqlQuoted(attribute.Key, true), StringFunctions.SqlQuoted(attribute.Value, true)));
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            }
            return true;
        }
        #endregion MFileLinkAttributeSet


        [WebMethod]
        public int SaveClientToODB(int application_id)
        {
            int mResult = 0;
            try
            {
                string mSQL = String.Format("EXEC dbo.pODB_CLIENT_CREATE_FROM_APPLICATION @application_id = {0}, @need_rs = 1", application_id);
                DataSet mDS = _DGate.GetDataSet(mSQL);
                if (mDS != null && mDS.Tables.Count > 0 && mDS.Tables[0].Rows.Count > 0)
                {
                    mResult = Convert.ToInt32(mDS.Tables[0].Rows[0]["CLIENT_NO"]);
                }
            }
            catch { }
            return mResult;
        }

        #endregion --LMSCangeLoanAmount--

        #region update additional attrinute
        [WebMethod]
        public int UpdateAdditionalAttrinute(int application_id, string attribute_code, string attribute_value)
        {
            int mResult = 0;
            try
            {
                string mSQL = String.Format("EXEC dbo.pAPPLICATION_ADDITIONAL_ATTRIBUTE_UPDATE @application_id = {0}, @attribute_code = {1}, @attribute_value = {2}", application_id, CheckNull(attribute_code), CheckNull(attribute_value));
                DataSet mDS = _DGate.GetDataSet(mSQL);
                if (mDS != null && mDS.Tables.Count > 0 && mDS.Tables[0].Rows.Count > 0)
                {
                    mResult = Convert.ToInt32(mDS.Tables[0].Rows[0]["ANSWER"]);
                }
            }
            catch { }
            return mResult;
        }
        #endregion update additional attrinute


        #region LmsAgreementSpecialParamsAdd

        [WebMethod]
        public bool LmsAgreementSpecialParamsAdd(int application_id, string param_value)
        {
            bool result = true;
            if (param_value.Length > 0)
            {
                try
                {
                    string mSQL = String.Format("EXEC dbo.pLMS_AGREEMENT_SPECIAL_PARAMS_ADD @application_id = {0}, @param_value = {1}", application_id,
                        StringFunctions.SqlQuoted(param_value, true));
                    _DGate.Execute(mSQL);
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }
        #endregion LmsAgreementSpecialParamsAdd

        #region Admin Debts
        private void LMSAdminDebtsAdd(LoanApplicationFull loanApplicationFull, int LMSLoanID, int mUserID, LAPI.Core.UserAndDeptId mUserAndDeptId)
        {
            var adminDebts = new PutAdministrationDebtRequest();

            /*OLD Administration Depts*/
            foreach (var applicationAdminDebts in loanApplicationFull.ApplicationAdminDebtsList)
            {
                var mTtimespan = (DateTime.Now - applicationAdminDebts.REG_DATE);
                if (mTtimespan.TotalDays > 365)
                {
                    mTtimespan = (DateTime.Now - loanApplicationFull.Record.CREATE_DATE);
                }
                var adminDebt = new AdministrationDebt();
                adminDebt.CustomerId = loanApplicationFull.ClientInfo.CLIENT_NO;
                adminDebt.Note = applicationAdminDebts.COMMENT;
                adminDebt.TypeId = applicationAdminDebts.DEBT_TYPE1_ID;
                adminDebt.KindId = applicationAdminDebts.DEBT_TYPE2_ID;
                adminDebt.DueDate = applicationAdminDebts.EXEC_DATE.AddDays(mTtimespan.TotalDays);
                adminDebt.ObjectId = LMSLoanID;
                adminDebt.ObjectType = (AdministrationDebt.DebtObjectType)applicationAdminDebts.OBJECT_TYPE_ID;
                adminDebt.RegistrationDate = DateTime.Now.Date;
                adminDebt.State = AdministrationDebt.DebtState.UnderControl;
                adminDebt.Id = null;


                adminDebts.Debt = adminDebt;
                adminDebts.RequestHeaders = this.LMSRequestHeadersGet();
                adminDebts.Creator = mUserAndDeptId;
                try
                {
                    _lmsClient.PutAdministrationDebt(adminDebts);
                }
                catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
                {
                    this.LogFaunt(fault, mUserID, "PutAdministrationDebt");
                }
                catch (Exception Ex)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutAdministrationDebt: " + Ex.Message);
                }
            }

            /*NEW Admin Depts*/
            foreach (var appAdminDebts in loanApplicationFull.AppAdminDebtList.Where(ad => ad.ADMIN_GROUP_ID != 1 && ad.ADMIN_GROUP_ID != 6).ToList())
            {

                List<AdminDebtItemFull> adminDebtItemFullList = this.AdminDebtItemFullList();
                AdminDebtItemFull adminDebtFullItem = adminDebtItemFullList.FindLast(item => item.GROUP_ID == appAdminDebts.ADMIN_GROUP_ID && item.ITEM_ID == appAdminDebts.ADMIN_ITEM_ID);

                AdministrationDebt adminDebt = new AdministrationDebt();
                adminDebt.CustomerId = loanApplicationFull.ClientInfo.CLIENT_NO;
                adminDebt.Note = appAdminDebts.LMS_COMMENT;
                adminDebt.TypeId = adminDebtFullItem.LMS_TYPE1_ID;
                adminDebt.KindId = adminDebtFullItem.LMS_TYPE2_ID;
                adminDebt.DueDate = this.WorkDaysAdd(DateTime.Now.Date, appAdminDebts.LMS_DUE_DAYS).WORK_DAY;
                adminDebt.ObjectId = LMSLoanID;
                adminDebt.ObjectType = (AdministrationDebt.DebtObjectType)adminDebtFullItem.LMS_OBJECT_TYPE_ID;
                adminDebt.RegistrationDate = DateTime.Now.Date;
                adminDebt.State = AdministrationDebt.DebtState.UnderControl;
                adminDebt.Id = null;

                adminDebts.Debt = adminDebt;
                adminDebts.RequestHeaders = this.LMSRequestHeadersGet();
                adminDebts.Creator = mUserAndDeptId;
                try
                {
                    _lmsClient.PutAdministrationDebt(adminDebts);
                }
                catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
                {
                    this.LogFaunt(fault, mUserID, "PutAdministrationDebt");
                }
                catch (Exception Ex)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutAdministrationDebt: " + Ex.Message);
                }
            }
        }



        //[WebMethod]
        //public void LMSAdminDebtsAddMissingList()
        //{
        //    string sql =
        //        @"SELECT DISTINCT A.APPLICATION_ID ,A.LMS_LOAN_ID,I.CLIENT_NO,A.APLICATION_OWNER
        //            FROM dbo.tSYS_CLIENT_SIDE_LOG L
        //                INNER JOIN dbo.tAPPLICATION A ON A.APPLICATION_ID = L.APPLICATION_ID
        //                INNER JOIN dbo.tAPPLICATION_CLIENT_INFO I ON I.APPLICATION_ID = A.APPLICATION_ID
        //            WHERE LOG_MESSAGE = 'LMSAdminDebtsAdd: Object reference not set to an instance of an object.' 
        //            AND A.LMS_LOAN_ID > 0 ";

        //    mDataTable = _DGate.GetDataSet(sql).Tables[0];

        //    foreach (DataRow row in mDataTable.Rows)
        //    {
        //        int application_id = Convert.ToInt32(row["APPLICATION_ID"]);
        //        LMSAdminDebtsAddMissing(application_id, Convert.ToInt32(row["LMS_LOAN_ID"]), Convert.ToInt32(row["CLIENT_NO"]), Convert.ToInt32(row["APLICATION_OWNER"]));
        //    }

        //}

        //[WebMethod]
        //public bool LMSAdminDebtsAddMissing(int application_id,int loan_id,int client_no,int owner)
        //{
        //    bool result = false;


        //    var mUserAndDeptId = new LAPI.Core.UserAndDeptId();
        //    var mUserIdentification = new LAPI.Core.UserIdentification();

        //    mUserIdentification.Id = owner;
        //    mUserAndDeptId.UserIdentification = mUserIdentification;

        //    var adminDebts = new PutAdministrationDebtRequest();

        //    /*NEW Admin Depts*/
        //    foreach (var appAdminDebts in AppAdminDebtList(application_id).Where(ad => ad.ADMIN_GROUP_ID != 1 && ad.ADMIN_GROUP_ID != 6).ToList())
        //    {
        //        List<AdminDebtItemFull> adminDebtItemFullList = this.AdminDebtItemFullList();
        //        AdminDebtItemFull adminDebtFullItem = adminDebtItemFullList.FindLast(item => item.GROUP_ID == appAdminDebts.ADMIN_GROUP_ID && item.ITEM_ID == appAdminDebts.ADMIN_ITEM_ID);

        //        AdministrationDebt adminDebt = new AdministrationDebt();
        //        adminDebt.CustomerId = client_no;
        //        adminDebt.Note = appAdminDebts.LMS_COMMENT;
        //        adminDebt.TypeId = adminDebtFullItem.LMS_TYPE1_ID;
        //        adminDebt.KindId = adminDebtFullItem.LMS_TYPE2_ID;
        //        adminDebt.DueDate = this.WorkDaysAdd(DateTime.Now.Date, appAdminDebts.LMS_DUE_DAYS).WORK_DAY;
        //        adminDebt.ObjectId = loan_id;
        //        adminDebt.ObjectType = (AdministrationDebt.DebtObjectType)adminDebtFullItem.LMS_OBJECT_TYPE_ID;
        //        adminDebt.RegistrationDate = DateTime.Now.Date;
        //        adminDebt.State = AdministrationDebt.DebtState.UnderControl;
        //        adminDebt.Id = null;

        //        adminDebts.Debt = adminDebt;
        //        adminDebts.RequestHeaders = this.LMSRequestHeadersGet();
        //        adminDebts.Creator = mUserAndDeptId;
        //        try
        //        {
        //            _lmsClient.PutAdministrationDebt(adminDebts);
        //            result = true;
        //        }
        //        catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
        //        {
        //            this.LogFaunt(fault, 2, "PutAdministrationDebt");
        //        }
        //        catch (Exception Ex)
        //        {
        //            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, 23, "PutAdministrationDebt: " + Ex.Message);
        //        }
        //    }
        //    return result;
        //}


        //[WebMethod]
        //public void LMSAdminDebtsAdd2(int application_id)
        //{
        //    LoanApplicationFull loanApplicationFull = this.LoanApplicationFullLoad(application_id, 433);

        //    var mUserAndDeptId = new LAPI.Core.UserAndDeptId();
        //    var mUserIdentification = new LAPI.Core.UserIdentification();

        //    mUserIdentification.Id = loanApplicationFull.Record.APLICATION_OWNER;
        //    mUserAndDeptId.UserIdentification = mUserIdentification;

        //    var adminDebts = new PutAdministrationDebtRequest();

        //    int LMSLoanID = loanApplicationFull.Record.LMS_LOAN_ID;
        //    int mUserID = loanApplicationFull.Record.APLICATION_OWNER;



        //    foreach (var applicationAdminDebts in loanApplicationFull.ApplicationAdminDebtsList)
        //    {
        //        DateTime m111Date = loanApplicationFull.HistoryList.First(h => h.STATE_ID == 111).OPER_DATE.Date;
        //        var adminDebt = new AdministrationDebt();
        //        var mTtimespan = (m111Date - loanApplicationFull.Record.CREATE_DATE);

        //        adminDebt.CustomerId = loanApplicationFull.ClientInfo.CLIENT_NO;
        //        adminDebt.Note = applicationAdminDebts.COMMENT;
        //        adminDebt.TypeId = applicationAdminDebts.DEBT_TYPE1_ID;
        //        adminDebt.KindId = applicationAdminDebts.DEBT_TYPE2_ID;
        //        adminDebt.DueDate = applicationAdminDebts.EXEC_DATE.AddDays(mTtimespan.TotalDays);
        //        adminDebt.ObjectId = LMSLoanID;
        //        adminDebt.ObjectType = (AdministrationDebt.DebtObjectType)applicationAdminDebts.OBJECT_TYPE_ID;
        //        adminDebt.RegistrationDate = m111Date;
        //        adminDebt.State = AdministrationDebt.DebtState.UnderControl;
        //        adminDebt.Id = null;


        //        adminDebts.Debt = adminDebt;
        //        adminDebts.RequestHeaders = this.LMSRequestHeadersGet();
        //        adminDebts.Creator = mUserAndDeptId;
        //        try
        //        {
        //            _lmsClient.PutAdministrationDebt(adminDebts);
        //        }
        //        catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
        //        {
        //            this.LogFaunt(fault, mUserID, "PutAdministrationDebt");
        //        }
        //        catch (Exception Ex)
        //        {
        //            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutAdministrationDebt: " + Ex.Message);
        //        }


        //    }
        //}
        #endregion Admin Debts

        #region LMSAddClientBusiness
        private bool LMSAddClientBusiness(int client_id, string business_id, string name, string location, string address, string city)
        {
            string mSQL = String.Format("EXEC dbo.pLMS_CLIENT_BUSINESSES_ADD @client_id = {0},  @business_id = {1},  @name = {2},  @location = {3},  @address = {4},  @city = {5}"
                , client_id
                , StringFunctions.SqlQuoted(business_id, true)
                , StringFunctions.SqlQuoted(name, true)
                , StringFunctions.SqlQuoted(location, true)
                , StringFunctions.SqlQuoted(address, true)
                , StringFunctions.SqlQuoted(city, true)
                );
            try
            {
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion LMSAddClientBusiness

        #region LMSAddReportParameters
        private bool LMSAddReportParameters(int loan_id, int type_id, string param_name, string param_value)
        {
            string mSQL = String.Format("EXEC dbo.pLMS_AGREEMENT_PARAMS_SET @loan_id = {0},  @type_id = {1},  @param_name = {2},  @param_value = {3}"
                , loan_id
                , type_id
                , StringFunctions.SqlQuoted(param_name, true)
                , StringFunctions.SqlQuoted(param_value, true)
                );
            try
            {
                mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
                if (Convert.ToInt32(mDataTable.Rows[0]["ANSWER"]) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion LMSAddReportParameters

        #region VtbConnectionManager
        //private VtbConnectionManager GetConnection()
        //{
        //    return new VtbConnectionManager(_LMSConnectionString);
        //}
        #endregion

        #region GetFirstPaymentDate
        [WebMethod]
        public DateTime GetFirstPaymentDateTest(DateTime mStartDate, DateTime mEndDate, byte mFirstPaymentDay, int mFirstPaymentMonth)
        {
            return GetFirstPaymentDate(mStartDate, mEndDate, mFirstPaymentDay, null, false, mFirstPaymentMonth);
        }

        private DateTime GetFirstPaymentDate(DateTime mStartDate, DateTime mEndDate, byte? mFirstPaymentDay, byte? mSecondPaymentDay, bool mEndOfMonth, int mFirstPaymentMonth)
        {
            int mMinDaysDiff = Convert.ToInt32(_AppSets["FirstPaymentStep"]);
            DateTime mStartDateWithStep = mStartDate.AddDays(mMinDaysDiff);
            DateTime value, mFirstPaymentDate;
            value = DateTime.Now.Date;
            if (mFirstPaymentDay == 0) { mFirstPaymentDay = null; }
            if (mSecondPaymentDay == 0) { mSecondPaymentDay = null; }
            if (mFirstPaymentDay != null && mSecondPaymentDay != null)
            {
                if (mFirstPaymentDay > mSecondPaymentDay)
                {
                    byte? tempPaymentDay = mFirstPaymentDay;
                    mFirstPaymentDay = mSecondPaymentDay;
                    mSecondPaymentDay = tempPaymentDay;
                }
            }
            if (mFirstPaymentDay == null && mSecondPaymentDay != null)
            {
                mFirstPaymentDay = mSecondPaymentDay;
                mSecondPaymentDay = null;
            }

            int mYear = mStartDate.Year;
            int mMonth = 0;
            if (mFirstPaymentMonth == 0)
            {
                mMonth = mStartDate.Month;
            }
            else
            {
                if (mFirstPaymentMonth < mStartDate.Month)
                {
                    mYear += 1;
                }
                mMonth = mFirstPaymentMonth;
            }

            if (mFirstPaymentDay == null && mSecondPaymentDay == null)
            {
                if (!mEndOfMonth)
                {
                    return mEndDate;
                }
                else
                {
                    mFirstPaymentDate = this.EndOfMonth(new DateTime(mYear, mMonth, 1));
                    if (mFirstPaymentDate < mStartDateWithStep)
                    {
                        value = this.GetNextPaymentDate(mFirstPaymentDate, 0, mEndOfMonth);
                    }
                    else
                    {
                        value = mFirstPaymentDate;
                    }
                    return value;
                }
            }

            if (mFirstPaymentDay <= DateTime.DaysInMonth(mYear, mMonth))
            {
                mFirstPaymentDate = new DateTime(mYear, mMonth, Convert.ToInt32(mFirstPaymentDay));
            }
            else
            {
                mFirstPaymentDate = new DateTime(mYear, mMonth, DateTime.DaysInMonth(mYear, mMonth));
            }

            value = mFirstPaymentDate;
            while (value < mStartDateWithStep)
            {
                value = this.GetNextPaymentDate(mFirstPaymentDate, (int)mFirstPaymentDay, mEndOfMonth);
                mFirstPaymentDate = value;
            }
            return value;
        }

        private DateTime GetNextPaymentDate(DateTime mPaymentDate, int mPaymentDay, bool mEndOfMonth)
        {
            mPaymentDate = mPaymentDate.AddMonths(1);
            if (!mEndOfMonth)
            {
                try
                {
                    mPaymentDate = new DateTime(mPaymentDate.Year, mPaymentDate.Month, mPaymentDay);
                }
                catch
                {

                    try
                    {
                        mPaymentDate = new DateTime(mPaymentDate.Year, mPaymentDate.Month, mPaymentDay - 1);
                    }
                    catch
                    {
                        try
                        {
                            mPaymentDate = new DateTime(mPaymentDate.Year, mPaymentDate.Month, mPaymentDay - 2);
                        }
                        catch
                        {
                            mPaymentDate = new DateTime(mPaymentDate.Year, mPaymentDate.Month, mPaymentDay - 3);
                        }
                    }
                }
            }
            return mPaymentDate;
        }
        private DateTime EndOfMonth(DateTime mDt)
        {

            return new DateTime(mDt.Year, mDt.Month, DateTime.DaysInMonth(mDt.Year, mDt.Month));
        }

        #endregion

        #region InitReferences
        private void InitReferences()
        {
            _PipelineService = new PipelineService();
            _InsuranceTariffArray = this.AutoInsuranceTariffList().ToArray();
            _PmtFreqTypeArray = this.PmtFreqTypeList().ToArray();
            _InformationSourceArray = this.InformationSourceList().ToArray();
            _InsuranceDriverPassengerArray = this.AutoInsuranceDriverPassengerList().ToArray();
            _InsuranceThirdPartyArray = this.AutoInsuranceThirdPartyList().ToArray();
            _InsuranceProductsArray = this.InsuranceProductsList().ToArray();
            _AdditionalAttributeList = this.AdditionalAttributeList();
        }
        #endregion

        #region = special loan types =
        private SpecialLoanTypes SpecialLoanTypesGet(int mBaseProductID, int mProductID)
        {
            SpecialLoanTypes mSpecialLoanTypes = new SpecialLoanTypes();
            mSpecialLoanTypes.IsCard = false;
            mSpecialLoanTypes.IsOverdraft = false;
            mSpecialLoanTypes.IsOverdraftStandard = false;
            mSpecialLoanTypes.IsOverdraftPreapproved = false;
            mSpecialLoanTypes.IsCreditCard = false;
            mSpecialLoanTypes.IsCreditCardStandard = false;
            mSpecialLoanTypes.IsCreditCardGold = false;
            mSpecialLoanTypes.IsCreditCardGoldPreaproved = false;
            mSpecialLoanTypes.IsCreditCardPreapproved = false;
            mSpecialLoanTypes.IsPOS = false;
            mSpecialLoanTypes.IsPOSOnline = false;
            mSpecialLoanTypes.IsGarantee = false;
            mSpecialLoanTypes.IsGaranteeGeocell = false;
            mSpecialLoanTypes.IsGaranteeBeeline = false;

            mSpecialLoanTypes.IsAutoNew = false;
            mSpecialLoanTypes.IsAuto = false;
            mSpecialLoanTypes.IsToyotaAutoNew = false;
            mSpecialLoanTypes.IsAutoUsed = false;
            mSpecialLoanTypes.IsClearenceCustomer = false;
            mSpecialLoanTypes.IsClearence = false;
            mSpecialLoanTypes.IsClearenceDealer = false;

            mSpecialLoanTypes.IsRealEstate = false;
            mSpecialLoanTypes.IsIpoPurchase = false;
            mSpecialLoanTypes.IsIpoBuilding = false;
            mSpecialLoanTypes.IsIpoRepairing = false;
            mSpecialLoanTypes.IsIpoRefinancing = false;
            mSpecialLoanTypes.IsIpoUniversal = false;
            mSpecialLoanTypes.IsIpoExpress = false;
            mSpecialLoanTypes.IsIpoGEL = false;
            mSpecialLoanTypes.IsIpoCollaborative = false;
            mSpecialLoanTypes.IsPOSCard = false;
            mSpecialLoanTypes.IsStudent = false;

            try
            {
                List<SpecialLoanTypes> mSpecialLoanTypesList = this.SpecialLoanTypesGet();
                mSpecialLoanTypes = mSpecialLoanTypesList.First(item => item.PRODUCT_ID == mProductID);
            }
            catch (Exception ex)
            {
                string mError = ex.Message;
            }
            return mSpecialLoanTypes;
        }

        [WebMethod]
        public List<SpecialLoanTypes> SpecialLoanTypesGet()
        {
            string mSQL = String.Format("EXEC dbo.pREF_SPECIAL_PRODUCTS");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<SpecialLoanTypes> RetList = new List<SpecialLoanTypes>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                SpecialLoanTypes mClass = new SpecialLoanTypes();
                mClass = (SpecialLoanTypes)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion = special loan types =

        #region = CollateralAddOwnerAsGuarantor =
        private bool CollateralAddOwnerAsGuarantor(int mApplicationId, string branch_id, decimal mAgreementAmount, string mDescription, int mOwnerClientId, string mCCY, int mResponsibleUserId, LAPI.Core.UserAndDeptId mUserAndDeptId, string mApplicationMFileAttribute)
        {
            string mCollateralType = "0305";

            List<Collateral> mCL = this.LmsGetCollateral((int)mOwnerClientId, mCollateralType, "", "", null);
            Collateral mCollateral = mCL.Find(item => item.OwnerId == mOwnerClientId && item.CollateralType == mCollateralType);//TODO

            AmountAndCurrency mCollateralAmount = new AmountAndCurrency { Amount = mAgreementAmount, Ccy = mCCY };


            /*Attributes*/
            Attributes mLinkAttribites = null;
            
            if (mApplicationMFileAttribute != "")
            {
                mLinkAttribites = new Attributes(); 
                mLinkAttribites.Add("Collateral M-file number", mApplicationMFileAttribute);
            }

            if (mCollateral != null)
            {
                this.PutCollateralLink(mApplicationId, null, mCollateral, mUserAndDeptId, mCollateralAmount, mLinkAttribites);
                return true;
            }


            mCollateral = new Collateral();


            PutCollateralRequest mPutCollateralRequest = new PutCollateralRequest();

            var mAttributesAttributeList = new LAPI.Core.Attributes();
            mCollateral.Attributes = mAttributesAttributeList;

            mCollateral.Amount = mCollateralAmount;
            mCollateral.BranchId = _LoanApplicationFull.Record.BRANCH_ID;
            mCollateral.CollateralType = mCollateralType;
            mCollateral.Comment = mDescription;
            mCollateral.CoOwners = new List<CollateralCoOwner>();//TODO
            mCollateral.OwnerId = mOwnerClientId;
            mCollateral.State = CollateralState.Current;
            mPutCollateralRequest.Collateral = mCollateral;
            mPutCollateralRequest.RequestHeaders = this.LMSRequestHeadersGet();
            mPutCollateralRequest.Creator = mUserAndDeptId;

            PutCollateralResponse mPutCollateralResponse = this.PutCollateral(mPutCollateralRequest, mUserAndDeptId);
            mCollateral.CollateralId = mPutCollateralResponse.CollateralId;

            PutCollateralLinkResponse mPutCollateralLinkResponse = this.PutCollateralLink(mApplicationId, null, mCollateral, mUserAndDeptId, mCollateralAmount, mLinkAttribites);

            return true;
        }
        #endregion = CollateralAddOwnerAsGuarantor =

        #region LAPI

        private List<Collateral> LmsGetCollateral(int mOwnerId, string mCollateralType, string mVINCode, string mCadastreCode, int? mDepositIdentifier)
        {
            List<Collateral> mCollateralList = null;
            try
            {
                mCollateralList = new List<Collateral>();
                ListCollateralsQuery mListCollateralsQuery = new ListCollateralsQuery();
                ListCollateralsRequest mListCollateralsRequest = new ListCollateralsRequest();

                if (mVINCode != "")
                {
                    mListCollateralsQuery.VINCode = mVINCode.ToString();
                }
                else if (mCadastreCode != "")
                {
                    mListCollateralsQuery.CadastreCode = mCadastreCode.ToString();
                }
                else
                {
                    mListCollateralsQuery.OwnerId = mOwnerId;
                    mListCollateralsQuery.CollateralType = mCollateralType;
                }

                mListCollateralsQuery.DepositId = mDepositIdentifier;
                mListCollateralsQuery.ControlFlags = CollateralControlFlags.Basic;
                mListCollateralsRequest.Query = mListCollateralsQuery;
                mListCollateralsRequest.RequestHeaders = this.LMSRequestHeadersGet();

                ListCollateralsResponse mListCollateralsResponse = _LAPI.ListCollaterals(mListCollateralsRequest);
                CollateralsList mCollateralsList = mListCollateralsResponse.Result;
                mCollateralList = mCollateralsList;
            }
            catch (Exception ex)
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, 2, ex.Message);
            }

            return mCollateralList;
        }

        #endregion LAPI

        #endregion LMS private methods

        #region LMSAddApplicationBeeline

        [WebMethod]
        public int LMSAddApplicationBeeline(int mApplicationID, int mUserID, string BeelinePhone)
        {

            int mBeelinePhoneID = 0;
            int LMSApplicationID = 0;

            List<AdditionalAttribute> mAdditionalAttributeList = this.AdditionalAttributeList();
            mBeelinePhoneID = mAdditionalAttributeList.Find(item => item.ATTRIBUTE_CODE == "GUARANTEE_PHONE_NUMBER").ATTRIBUTE_ID;

            List<ApplicationAdditionalAttribute> mApplicationAdditionalAttributeList = this.ApplicationAdditionalAttributeList(mApplicationID);
            if (mApplicationAdditionalAttributeList.FindIndex(item => item.ATTRIBUTE_ID == mBeelinePhoneID) < 0)
            {
                this.ApplicationAdditionalAttributeSave(mApplicationID, mUserID, mBeelinePhoneID, BeelinePhone);
            }
            LMSAnswer mLMSAnswer = LMSAddApplication2(mApplicationID, mUserID, (int)ApplicationStatus.New);
            LMSApplicationID = this.ApplicationRec(mApplicationID, mUserID).LMS_LOAN_ID;
            if (mLMSAnswer.Error == 0 && LMSApplicationID != 0)
            {
                this.ApplicationChangeState(mApplicationID, mUserID, 111, 3100, 0, 0);
            }
            else
            {
                LMSApplicationID = -1;
            }

            return LMSApplicationID;
        }

        #endregion LMSAddApplicationBeeline

        #region LMSAddApplication

        [WebMethod]
        public LMSAnswer LMSAddApplication(int mApplicationID, int mUserID)
        {
            return LMSAddApplication2(mApplicationID, mUserID, (int)ApplicationStatus.New);
        }

        [WebMethod]
        public LMSAnswer LMSAddApplication2(int mApplicationID, int mUserID, int mApplicationState)
        {
            #region private members

            LMSAnswer mLMSAnswer = new LMSAnswer();
            mLMSAnswer.Error = 0;
            mLMSAnswer.ErrorMessage = "";
            int? mCreditLineId = null;
            CashCover mCurentCashCover = null;
            List<CashCoverDepoLoanMap> mCashCoverDepoLoanMapODBList = new List<CashCoverDepoLoanMap>();

            #endregion private members

            #region pipeline data

            _LoanApplicationFull = LoanApplicationFullLoad(mApplicationID, mUserID);

            #region -save client to odb-
            //if (_LoanApplicationFull.ClientInfo.CLIENT_NO == 0)
            //{
            //    _LoanApplicationFull.ClientInfo.CLIENT_NO = this.SaveClientToODB(mApplicationID);
            //}
            bool is_nonclient = false;
            try
            {
                if (_LoanApplicationFull.AdditionalAttribules.Where(a => a.ATTRIBUTE_CODE == "Non Client").ToList().First().ATTRIBUTE_VALUE != "")
                {
                    is_nonclient = true;
                }
            }
            catch { }

            if (!is_nonclient)
            {
                _LoanApplicationFull.ClientInfo.CLIENT_NO = this.SaveClientToODB(mApplicationID);
            }
            #endregion -save client to odb-

            #region -product change-

            try
            {
                int mNewBaseProductID = this.ProductReplace(_LoanApplicationFull.Record.BASE_PRODUCT_ID, _LoanApplicationFull.Record.APPLICATION_SOURCE_ID, _LoanApplicationFull.Record.PRODUCT_ID);
                if (mNewBaseProductID != 0)
                {
                    _LoanApplicationFull.Record.BASE_PRODUCT_ID = mNewBaseProductID;
                }
            }
            catch { }

            #endregion -product change-

            #region ApplicationState

            SpecialLoanTypes mSpecialLoanTypes = this.SpecialLoanTypesGet(_LoanApplicationFull.Record.BASE_PRODUCT_ID, _LoanApplicationFull.Record.PRODUCT_ID);
            if (mSpecialLoanTypes.IsGarantee || mSpecialLoanTypes.IsPOSCard)
            {
                mApplicationState = (int)ApplicationStatus.Approved;
            }

            ApplicationStatus mAltaApplicationState = (ApplicationStatus)mApplicationState;

            #endregion ApplicationState

            this.InitReferences();
            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "Pipeline data done");

            #endregion pipeline data

            #region fatca check
            if (_LoanApplicationFull.ClientInfoFatca.HAS_GREEN_CARD == 1
                || _LoanApplicationFull.ClientInfoFatca.US_RESIDENCE == 1
                || _LoanApplicationFull.ClientInfoFatca.US_TAX_RESIDENCE == 1
                || _LoanApplicationFull.ClientInfoFatca.CANCELED_US_CITIZENSHIP == 1
                || _LoanApplicationFull.ClientInfoFatca.COUNTRY_OF_TAX_RESIDENCE == "US"
                || _LoanApplicationFull.ClientInfoFatca.LONG_TERM_RESIDENCE == 1
                || _LoanApplicationFull.ClientInfoFatca.US_PHONE_OR_FAX == 1
                || _LoanApplicationFull.ClientInfoFatca.US_POST_BOX == 1
                )
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "Fatca match");
            }
            if (_LoanApplicationFull.ClientInfo.COUNTRY_OF_BIRTH == "US"
                || _LoanApplicationFull.ClientInfo.FACT_COUNTRY == "US"
                || _LoanApplicationFull.ClientInfo.PASSPORT_COUNTRY == "US"
                || _LoanApplicationFull.ClientInfo.RESIDENT_COUNTRY == "US"
                || _LoanApplicationFull.ClientInfo.CLIENT_CITIZENSHIP == "US"
                || _LoanApplicationFull.ClientInfo.CITIZENSHIP == "US"
                || _LoanApplicationFull.ClientInfo.DOUBLE_CITIZENSHIP == "US"
                )
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "US match");
            }

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "fatca check done");
            #endregion fatca check

            #region ====<LOAN APPLICATION>====

            #region Application

            var mProductId = mSpecialLoanTypes.IsOverdraftPreapproved ? _LoanApplicationFull.Record.PRODUCT_ID : _LoanApplicationFull.Record.BASE_PRODUCT_ID;

            PipelineWS.LAPI.Core.LoanProduct loanProduct = null;
            var mUserAndDeptId = new LAPI.Core.UserAndDeptId();
            var mUserIdentification = new LAPI.Core.UserIdentification();
            var mUserAndDeptIdI = new LAPI.Core.UserAndDeptId();
            var mUserIdentificationI = new LAPI.Core.UserIdentification();

            mUserIdentification.Id = _LoanApplicationFull.Record.APLICATION_OWNER;
            mUserAndDeptId.UserIdentification = mUserIdentification;
            mUserIdentificationI.Id = _LoanApplicationFull.Record.APLICATION_OWNER;
            mUserAndDeptIdI.UserIdentification = mUserIdentificationI;


            RequestHeaders mRequestHeaders = LMSRequestHeadersGet();

            loanProduct = _lmsClient.GetLoanProduct(new GetLoanProductRequest
            {
                RequestHeaders = mRequestHeaders,
                LoanProductId = mProductId,
                ControlFlags = LoanProductControlFlags.Basic | LoanProductControlFlags.ApplicationAttributes | LoanProductControlFlags.Attributes | /*LoanProductControlFlags.ExtraExpanses |*/ LoanProductControlFlags.LoanRangeConditions | LoanProductControlFlags.Resources | LoanProductControlFlags.ScheduleTypes

            }).Result;

            #region general parameters

            _PmtFreqType = Array.FindLast(_PmtFreqTypeArray, s => s.PMT_FREQ_TYPE_ID == _LoanApplicationFull.Record.PMT_INTERVAL_TYPE);
            if (_LoanApplicationFull.Record.END_OF_MONTH)
            {
                if (_PmtFreqType.SHOW_PAYMENT_DAY_2)
                {
                    _EndOfMonthType = EndOfMonthMode.SecondPaymentDay;
                }
                else
                {
                    _EndOfMonthType = EndOfMonthMode.PaymentDay;
                }
            }
            else
            {
                _EndOfMonthType = EndOfMonthMode.None;
            }

            _PutApplicationRequest = new PutApplicationRequest
            {
                Application = new Application { AgreementNo = "", Attributes = new LAPI.Core.Attributes() },
                RequestHeaders = this.LMSRequestHeadersGet(),
                Creator = new LAPI.Core.UserAndDeptId { UserIdentification = new LAPI.Core.UserIdentification { Id = mUserID } }

            };

            _PutApplicationRequest.Application.Id = null;
            _PutApplicationRequest.Application.AgreementNo = "";


            _PutApplicationRequest.Application.ProductId = mProductId;

            _PutApplicationRequest.Application.BorrowerId = _LoanApplicationFull.ClientInfo.CLIENT_NO;
            if (mSpecialLoanTypes.IsRealEstate && _LoanApplicationFull.GuarantorList.Count > 0)
            {
                try
                {
                    _PutApplicationRequest.Application.Coborrowers.Add(_LoanApplicationFull.GuarantorList.Find(item => item.GUARANTOR_TYPE_ID == 2).GUARANTOR_CLIENT_NO);
                }
                catch { }
            }

            /*ყველა განვადების ბარათისთვის ერთი მომხმარებელი იწერება*/
            if (mSpecialLoanTypes.IsPOSCard)
            {
                _PutApplicationRequest.Application.PosBranchId = _AppSets["PosCardLMSBranch"];
                _PutApplicationRequest.Application.ResponsibleUserId = Convert.ToInt32(_AppSets["PosCardLMSUser"]);
            }
            else if (_LoanApplicationFull.Record.APPLICATION_SOURCE_ID == 102)
            {
                _PutApplicationRequest.Application.PosBranchId = _AppSets["PosCardInBranchLMSBranch"];
                _PutApplicationRequest.Application.ResponsibleUserId = Convert.ToInt32(_AppSets["PosCardInBranchLMSUser"]);
            }
            else
            {
                _PutApplicationRequest.Application.PosBranchId = _LoanApplicationFull.Record.BRANCH_ID;
                _PutApplicationRequest.Application.ResponsibleUserId = _LoanApplicationFull.Record.APLICATION_OWNER;
            }

            AmountAndCurrency mAmountAndCurrency = new AmountAndCurrency { Amount = _LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED, Ccy = _LoanApplicationFull.Record.LOAN_CURRENCY };


            _PutApplicationRequest.Application.CreditLineId = mCreditLineId;
            _PutApplicationRequest.Application.State = mAltaApplicationState;
            _PutApplicationRequest.Application.RegistrationDate = _LoanApplicationFull.Record.CREATE_DATE;
            _PutApplicationRequest.Application.Authorities = new List<Authority> { new Authority { Role = AuthorityRole.Operator, UserId = (int?)mUserID } };
            _PutApplicationRequest.Application.Amount = mAmountAndCurrency;


            #endregion general parameters

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "general parameters done");

            //TODO FROM ALTA  check
            #region IntervalType

            DateTime mStartDate = DateTime.Now.Date;

            /* ალტამ ასე აიღეთო, მაგრამ თუ დღე არ დაიხურა არასწორად აიღებს.
            mStartDate = (DateTime)_LAPI.GetEnvironmentVariables(new GetEnvironmentVariablesRequest()
            {
                RequestHeaders = LMSRequestHeadersGet()
            }).Result.OpenDay;
            */

            LAPI.Core.Period mPeriod = new LAPI.Core.Period { Start = mStartDate };
            _PutApplicationRequest.Application.Term = mPeriod;

            PaymentInterval mIntervalType = null;

            if (mSpecialLoanTypes.IsGaranteeGeocell)
            {
                mIntervalType = new PaymentInterval { Type = PaymentIntervalType.Day, Value = 0 };
            }
            else
            {
                mIntervalType = new PaymentInterval { Type = (PaymentIntervalType)_PmtFreqType.PMT_INTERVAL_IS_IN_MONTHS, Value = _PmtFreqType.PMT_INTERVAL };
            }
            var scheduleDebtGenerator = new ScheduleDebtGenerator
            {
                ScheduleBuildMode = ScheduleBuildMode.Automatic,
                PaymentInterval = mIntervalType,
                EndOfMonth = _EndOfMonthType
            };
            var unplannedDebtGenerator = new UnplannedDebtGenerator
            {
                PaymentInterval = mIntervalType,
                EndOfMonth = _EndOfMonthType,
                BalanceZero = 0,
                DebtSource = UnplannedDebtSource.PrincipalAmount,
                UsedAmountRatio = _LoanApplicationFull.Record.UNPLANNED_DEBT_USED_AMOUNT_RATIO
            };

            int pmtCount = 1;

            if (mSpecialLoanTypes.IsGaranteeGeocell)
            {
                pmtCount = _LoanApplicationFull.Record.LOAN_DAYS_REQUESTED;
            }
            else
            {
                if (mIntervalType.Value == 0)
                {
                    pmtCount = 1;
                }
                else
                {
                    Double mPmtCountDouble;
                    int mPmtCountInt;
                    if (_PmtFreqType.PMT_INTERVAL_IS_IN_MONTHS == 1)
                    {
                        mPmtCountDouble = Convert.ToDouble(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED / Convert.ToDouble(mIntervalType.Value));
                        mPmtCountInt = Convert.ToInt32(mPmtCountDouble);
                        if (mPmtCountDouble > mPmtCountInt)
                        {
                            pmtCount = mPmtCountInt + 1;
                        }
                        else
                        {
                            pmtCount = mPmtCountInt;
                        }
                    }
                    else
                    {
                        TimeSpan timespan = (TimeSpan)(_PutApplicationRequest.Application.Term.Start - _PutApplicationRequest.Application.Term.End);
                        mPmtCountDouble = Convert.ToDouble(Convert.ToDecimal(timespan.TotalDays) / mIntervalType.Value);
                        mPmtCountInt = Convert.ToInt32(mPmtCountDouble);
                        if (mPmtCountDouble > mPmtCountInt)
                        {
                            pmtCount = mPmtCountInt + 1;
                        }
                        else
                        {
                            pmtCount = mPmtCountInt;
                        }
                    }
                }
            }
            scheduleDebtGenerator.PmtCount = pmtCount;

            #endregion IntervalType

            #region --check is card -- LMSCangeLoanAmount --

            if (mSpecialLoanTypes.IsCard)
            {
                int mLoan_id = 0;
                bool mLMSIsCreditCardStandard = false;
                bool mLMSIsCreditCardGold = false;
                try
                {
                    string mSQL = String.Format("EXEC pCLIENT_CARD_LOANS_GET @client_id = {0},@product_id = {1}, @acc_id = {2}", _LoanApplicationFull.ClientInfo.CLIENT_NO, _LoanApplicationFull.Record.BASE_PRODUCT_ID, _LoanApplicationFull.Record.CARD_ACC_ID);
                    DataTable mCLIENT_CARD_LOANS = _DGate.GetDataSet(mSQL).Tables[0];
                    mLoan_id = Convert.ToInt32(mCLIENT_CARD_LOANS.Rows[0]["LOAN_ID"]);
                    mLMSIsCreditCardStandard = Convert.ToBoolean(mCLIENT_CARD_LOANS.Rows[0]["IsCreditCardStandard"]);
                    mLMSIsCreditCardGold = Convert.ToBoolean(mCLIENT_CARD_LOANS.Rows[0]["IsCreditCardGold"]);
                }
                catch { }

                if (mLoan_id > 0)
                {
                    if ((mSpecialLoanTypes.IsCreditCardGold || mSpecialLoanTypes.IsCreditCardGoldPreaproved) && mLMSIsCreditCardStandard && !mLMSIsCreditCardGold)
                    {
                        // როცა ახალი საკრედიტო ბარათის პროდუქტი გოლდია, ხოლო ლმს-ში სტანდარტი , ამ შემთხვევაში უნდა გაიხნას ახალი სესხი (და არა ლიმიტის ცვლილება)
                    }
                    else
                    {
                        //წინააღმდეგ შემთხვევაში უნდა მოხდეს ლიმიტის ცვლილება
                        bool mCangeLoanAmountResult = false;
                        try
                        {
                            mCangeLoanAmountResult = this.LMSCangeLoanAmount(mLoan_id, (decimal)_PutApplicationRequest.Application.Amount.Amount, (int)_PutApplicationRequest.Application.ResponsibleUserId, _PutApplicationRequest.Application.PosBranchId);
                        }
                        catch (Exception Ex)
                        {
                            /*თუ შეცდომა მოხდა*/
                            mLMSAnswer.Error = -2;
                            mLMSAnswer.ErrorMessage = "შეცდომა ლიმიტის გაზრდისას:" + Ex.Message;
                        }

                        if (mCangeLoanAmountResult)
                        {
                            /*თუ ლიმიტის ცვლილება წარმატებით დამთავრდა*/
                            _LoanApplicationFull.Record.LMS_LOAN_ID = mLoan_id;
                            this.ApplicationLmsLoanIDSet(mApplicationID, mUserID, mLoan_id);
                        }
                        else
                        {
                            if (mLMSAnswer.Error == 0)
                            {
                                /*თუ შეცდომა არ მოხდა, მაგრამ false დაბრუნდა*/
                                mLMSAnswer.Error = -3;
                                mLMSAnswer.ErrorMessage = "ვერ მოხერხდა ლიმიტის გაზრდა";
                            }
                        }

                        mLMSAnswer.LoanApplicationRecord = _LoanApplicationFull.Record;
                        return mLMSAnswer;
                    }
                }
            }

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "check is card done");

            #endregion --check is card --

            #region other parameters

            #region PaymentDay,EndDate

            #region old variant

            //var TempEndDate = DateTime.Now.Date;
            //if (mIntervalType.Value == 0.5m)
            //{
            //    TempEndDate = _PutApplicationRequest.Application.Term.Start.AddMonths(Convert.ToInt32(pmtCount / 2));
            //}
            //else
            //{
            //    TempEndDate = _PutApplicationRequest.Application.Term.Start.AddMonths(pmtCount);
            //}
            //_PutApplicationRequest.Application.Term.End = TempEndDate;

            //if (!mSpecialLoanTypes.IsGaranteeGeocell)
            //{
            //    scheduleDebtGenerator.PaymentDay = (byte?)_LoanApplicationFull.Record.PAYMENT_DAY_1;
            //    scheduleDebtGenerator.SecondPaymentDay = (byte?)_LoanApplicationFull.Record.PAYMENT_DAY_2;
            //    unplannedDebtGenerator.PaymentDay = (byte?)_LoanApplicationFull.Record.PAYMENT_DAY_1;
            //}

            //if (!mSpecialLoanTypes.IsGaranteeGeocell && !mSpecialLoanTypes.IsPOSCard)
            //{
            //    scheduleDebtGenerator.FirstPaymentDate = GetFirstPaymentDate((DateTime)_PutApplicationRequest.Application.Term.Start, (DateTime)TempEndDate, scheduleDebtGenerator.PaymentDay, scheduleDebtGenerator.SecondPaymentDay, _LoanApplicationFull.Record.END_OF_MONTH, _LoanApplicationFull.Record.PAYMENT_MONTH);
            //}

            //if (mSpecialLoanTypes.IsGaranteeGeocell)//ჯეოსელის გარანტია
            //{
            //    _PutApplicationRequest.Application.Term.End = _PutApplicationRequest.Application.Term.Start.AddDays(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED == 11 ? 360 : 720);
            //}
            //else if (mSpecialLoanTypes.IsCashCover)
            //{
            //    _PutApplicationRequest.Application.Term.End = _PipelineService.WorkDayGet(_PutApplicationRequest.Application.Term.Start.AddDays(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED)).WORK_DAY;
            //}
            //else if (mSpecialLoanTypes.IsPOS || mSpecialLoanTypes.IsGaranteeBeeline)
            //{
            //    _PutApplicationRequest.Application.Term.End = scheduleDebtGenerator.FirstPaymentDate.Value.AddMonths(pmtCount - 1);
            //}
            //else
            //{
            //    _PutApplicationRequest.Application.Term.End = _PipelineService.WorkDayGet(_PutApplicationRequest.Application.Term.Start.AddMonths(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED)).WORK_DAY;
            //}

            //_PutApplicationRequest.Application.Term.End = _PipelineService.WorkDayGet(_PutApplicationRequest.Application.Term.End).WORK_DAY;//შემდეგი სამუშაო დღე

            //if (mSpecialLoanTypes.IsCashCover || mSpecialLoanTypes.IsPOS || mSpecialLoanTypes.IsGaranteeBeeline)
            //{
            //    scheduleDebtGenerator.LastPaymentDate = _PutApplicationRequest.Application.Term.End;
            //}
            //else if (mSpecialLoanTypes.IsGaranteeGeocell || mSpecialLoanTypes.IsPOSCard)
            //{
            //    scheduleDebtGenerator.LastPaymentDate = null;
            //}
            //else
            //{
            //    if (mIntervalType.Value == 0.5m)
            //    {
            //        scheduleDebtGenerator.LastPaymentDate = _PutApplicationRequest.Application.Term.Start.AddMonths(Convert.ToInt32(pmtCount / 2));
            //    }
            //    else
            //    {
            //        scheduleDebtGenerator.LastPaymentDate = _PutApplicationRequest.Application.Term.Start.AddMonths((int)pmtCount);
            //    }
            //}

            //_PutApplicationRequest.Application.Term.End = _PipelineService.WorkDayGet(_PutApplicationRequest.Application.Term.End).WORK_DAY;//შემდეგი სამუშაო დღე
            //if (scheduleDebtGenerator.LastPaymentDate != null)
            //    scheduleDebtGenerator.LastPaymentDate = _PipelineService.WorkDayGet((DateTime)scheduleDebtGenerator.LastPaymentDate).WORK_DAY;//შემდეგი სამუშაო დღე

            //if (mSpecialLoanTypes.IsCard || mSpecialLoanTypes.IsGarantee || mSpecialLoanTypes.IsPOSCard)
            //{
            //    scheduleDebtGenerator.FirstDisbursementAmount = 0;
            //}
            //else
            //{
            //    if (_LoanApplicationFull.Record.LOAN_AMOUNT_DISBURS == 0)
            //    {
            //        scheduleDebtGenerator.FirstDisbursementAmount = _LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED;
            //    }
            //    else
            //    {
            //        scheduleDebtGenerator.FirstDisbursementAmount = _LoanApplicationFull.Record.LOAN_AMOUNT_DISBURS;
            //    }
            //}


            #endregion old variant

            #region new variant
            var TempEndDate = DateTime.Now.Date;
            if (mIntervalType.Value == 0.5m)
            {
                TempEndDate = _PutApplicationRequest.Application.Term.Start.AddMonths(Convert.ToInt32(pmtCount / 2));
            }
            else
            {
                TempEndDate = _PutApplicationRequest.Application.Term.Start.AddMonths(pmtCount);
            }
            _PutApplicationRequest.Application.Term.End = TempEndDate;

            if (!mSpecialLoanTypes.IsGaranteeGeocell)
            {
                scheduleDebtGenerator.PaymentDay = (byte?)_LoanApplicationFull.Record.PAYMENT_DAY_1;
                scheduleDebtGenerator.SecondPaymentDay = (byte?)_LoanApplicationFull.Record.PAYMENT_DAY_2;
                unplannedDebtGenerator.PaymentDay = (byte?)_LoanApplicationFull.Record.PAYMENT_DAY_1;
            }

            if (!mSpecialLoanTypes.IsGaranteeGeocell && !mSpecialLoanTypes.IsPOSCard)
            {
                scheduleDebtGenerator.FirstPaymentDate = GetFirstPaymentDate((DateTime)_PutApplicationRequest.Application.Term.Start, (DateTime)TempEndDate, scheduleDebtGenerator.PaymentDay, scheduleDebtGenerator.SecondPaymentDay, _LoanApplicationFull.Record.END_OF_MONTH, _LoanApplicationFull.Record.PAYMENT_MONTH);
            }

            if (mSpecialLoanTypes.IsGaranteeGeocell)//ჯეოსელის გარანტია
            {
                _PutApplicationRequest.Application.Term.End = _PutApplicationRequest.Application.Term.Start.AddDays(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED == 11 ? 360 : 720);
            }
            else if (mSpecialLoanTypes.IsCashCover)
            {
                _PutApplicationRequest.Application.Term.End = _PipelineService.WorkDayGet(_PutApplicationRequest.Application.Term.Start.AddDays(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED)).WORK_DAY;
            }
            else if (!mSpecialLoanTypes.IsPOSCard && !mSpecialLoanTypes.IsCard)
            {
                if (mIntervalType.Value == 0.5m)
                {
                    _PutApplicationRequest.Application.Term.End = scheduleDebtGenerator.FirstPaymentDate.Value.AddMonths(Convert.ToInt32(pmtCount / 2) - 1);
                }
                else
                {
                    _PutApplicationRequest.Application.Term.End = scheduleDebtGenerator.FirstPaymentDate.Value.AddMonths(pmtCount - 1);
                }
                //_PutApplicationRequest.Application.Term.End = scheduleDebtGenerator.FirstPaymentDate.Value.AddMonths(pmtCount - 1);
            }

            _PutApplicationRequest.Application.Term.End = _PipelineService.WorkDayGet(_PutApplicationRequest.Application.Term.End).WORK_DAY;//შემდეგი სამუშაო დღე

            if (mSpecialLoanTypes.IsGaranteeGeocell || mSpecialLoanTypes.IsPOSCard)
            {
                scheduleDebtGenerator.LastPaymentDate = null;
            }
            else
            {
                scheduleDebtGenerator.LastPaymentDate = _PutApplicationRequest.Application.Term.End;
            }

            if (scheduleDebtGenerator.LastPaymentDate != null)
                scheduleDebtGenerator.LastPaymentDate = _PipelineService.WorkDayGet((DateTime)scheduleDebtGenerator.LastPaymentDate).WORK_DAY;//შემდეგი სამუშაო დღე

            if (mSpecialLoanTypes.IsCard || mSpecialLoanTypes.IsGarantee || mSpecialLoanTypes.IsPOSCard)
            {
                scheduleDebtGenerator.FirstDisbursementAmount = 0;
            }
            else
            {
                if (_LoanApplicationFull.Record.LOAN_AMOUNT_DISBURS == 0)
                {
                    scheduleDebtGenerator.FirstDisbursementAmount = _LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED;
                }
                else
                {
                    scheduleDebtGenerator.FirstDisbursementAmount = _LoanApplicationFull.Record.LOAN_AMOUNT_DISBURS;
                }
            }

            #endregion new variant


            #endregion PaymentDay,EndDate

            #region CreditLine
            if (mSpecialLoanTypes.IsRealEstate && _LoanApplicationFull.RealEstate.Main.GENERAL_AMOUNT > 0)
            {
                string mApplicationMFileAttribute_general = ApplicationMFileAttributeGet(_LoanApplicationFull.Record.APPLICATION_ID, 3); ;

                ListCreditLinesQuery mListCreditLinesQuery = new ListCreditLinesQuery();
                mListCreditLinesQuery.BorrowerId = _LoanApplicationFull.ClientInfo.CLIENT_NO;
                mListCreditLinesQuery.ControlFlags = CreditLineControlFlags.Basic;


                ListCreditLinesRequest mListCreditLinesRequest = new ListCreditLinesRequest();
                mListCreditLinesRequest.Query = mListCreditLinesQuery;
                mListCreditLinesRequest.RequestHeaders = this.LMSRequestHeadersGet();

                ListCreditLinesResponse mListCreditLinesResponse = _LAPI.ListCreditLines(mListCreditLinesRequest);

                var mCreditLinesList = mListCreditLinesResponse.Result.Where(x => x.Status != CreditLineStatus.Closed && x.Status != CreditLineStatus.Rejected).ToList();

                if (mCreditLinesList.Count > 0)
                {
                    try
                    {
                        _PutApplicationRequest.Application.CreditLineId = mCreditLinesList.First().Id;
                    }
                    catch (Exception ex)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "get creditlineid:" + ex.Message);
                    }
                }
                else
                {
                    mCreditLineId = null;

                    LAPI.Core.IntList mCoborrowerIds = null;

                    if (_PutApplicationRequest.Application.Coborrowers == null || _PutApplicationRequest.Application.Coborrowers.Count == 0)
                    {
                        mCoborrowerIds = new LAPI.Core.IntList { Capacity = 1 };
                        try
                        {
                            mCoborrowerIds.Add(_LoanApplicationFull.GuarantorList.Find(item => item.GUARANTOR_TYPE_ID == 2).GUARANTOR_CLIENT_NO);
                        }
                        catch
                        { }
                    }

                    var mAuthorities = new List<Authority>
                    {
                        new Authority
                        {
                            Role = AuthorityRole.PrimaryResponsible,
                            UserId = _PutApplicationRequest.Application.ResponsibleUserId
                        },
                        new Authority
                        {
                            Role = AuthorityRole.Operator,
                            UserId = mUserID
                    }
                    };

                    var mCreditLineAttributes = new LAPI.Core.Attributes();
                    mCreditLineAttributes.Add("Line M-file number", mApplicationMFileAttribute_general);

                    var mCreditLine = new LAPI.Core.CreditLine
                    {
                        //Id = null,//TODO
                        //ETag = "",//TODO
                        BorrowerId = _LoanApplicationFull.ClientInfo.CLIENT_NO,
                        BranchId = Convert.ToInt32(_LoanApplicationFull.Record.BRANCH_ID),
                        //AgreementNo = "",//TODO
                        Amount = new AmountAndCurrency { Amount = _LoanApplicationFull.RealEstate.Main.GENERAL_AMOUNT, Ccy = _LoanApplicationFull.Record.LOAN_CURRENCY },
                        Term = new LAPI.Core.Period { Start = (DateTime)_PutApplicationRequest.Application.Term.Start, End = (DateTime)_PutApplicationRequest.Application.Term.End },
                        Coborrowers = mCoborrowerIds,
                        Status = CreditLineStatus.New,
                        RegistrationDate = (DateTime)_PutApplicationRequest.Application.Term.Start,
                        Authorities = mAuthorities,
                        Attributes = mCreditLineAttributes
                    };

                    PutCreditLineRequest mPutCreditLineRequest = new PutCreditLineRequest();
                    mPutCreditLineRequest.Creator = new LAPI.Core.UserAndDeptId { UserIdentification = new LAPI.Core.UserIdentification { Id = mUserID } };

                    mPutCreditLineRequest.CreditLine = mCreditLine;
                    mPutCreditLineRequest.RequestHeaders = this.LMSRequestHeadersGet();
                    try
                    {
                        PutCreditLineResponse mPutCreditLineResponse = _LAPI.PutCreditLine(mPutCreditLineRequest);
                        _PutApplicationRequest.Application.CreditLineId = mPutCreditLineResponse.CreditLineId;
                    }
                    catch (Exception ex)
                    {
                        _PutApplicationRequest.Application.CreditLineId = null;
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "set creditline:" + ex.Message);
                    }
                }

            }
            #endregion CreditLine

            #region InstallmentDetails

            if (mSpecialLoanTypes.IsPOS)
            {
                _PutApplicationRequest.Application.InstallmentDetails = new InstallmentDetails { ObjectId = _LoanApplicationFull.ApplicationPurchaseFull.ApplicationPurchase.INSTALLMENT_ID };

            }

            if (_LoanApplicationFull.Record.BASE_PRODUCT_ID == 95)
            {
                _PutApplicationRequest.Application.InstallmentDetails = new InstallmentDetails { ObjectId = 35 };

            }
            #endregion InstallmentDetails

            #region Fees And Penalties

            FeeAndPenaltyAccrualPolicy feeAndPenaltyAccrualPolicy = new FeeAndPenaltyAccrualPolicy();
            decimal adminFee = 0;
            decimal trancheFee = 0;
            if (mSpecialLoanTypes.IsGarantee)
            {
                adminFee = _LoanApplicationFull.Record.ADMIN_FEE_PERCENT;
            }
            else if (mSpecialLoanTypes.IsRealEstate)
            {
                trancheFee = _LoanApplicationFull.Record.ADMIN_FEE_PERCENT;
            }
            else
            {
                adminFee = this.GetAdminFee((int)_PutApplicationRequest.Application.ProductId, _LoanApplicationFull.Record.ADMIN_FEE_PERCENT, "AdministrationFee");
                trancheFee = this.GetAdminFee((int)_PutApplicationRequest.Application.ProductId, _LoanApplicationFull.Record.FEE1_PERCENT, "Fee1");
            }

            if (mSpecialLoanTypes.IsPOS)
            {
                if (_LoanApplicationFull.ApplicationPurchaseFull.ApplicationPurchase.COMMISSION_AMOUNT < _LoanApplicationFull.Record.FEE1_MIN)
                    trancheFee = _LoanApplicationFull.Record.FEE1_MIN / Convert.ToDecimal(_LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED) * 100.00m;
                else
                    trancheFee = (_LoanApplicationFull.ApplicationPurchaseFull.ApplicationPurchase.COMMISSION_AMOUNT / _LoanApplicationFull.ApplicationPurchaseFull.ApplicationPurchase.LOAN_AMOUNT) * 100.00m;
            }
            else
            {
                if (adminFee != 0)
                {
                    if (((Convert.ToDecimal(adminFee) * Convert.ToDecimal(_LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED)) / 100.00m) < _LoanApplicationFull.Record.ADMIN_FEE)
                        adminFee = _LoanApplicationFull.Record.ADMIN_FEE / Convert.ToDecimal(_LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED) * 100.00m;
                }

                if (trancheFee != 0)
                {
                    if (((Convert.ToDecimal(trancheFee) * Convert.ToDecimal(_LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED)) / 100.00m) < _LoanApplicationFull.Record.FEE1_MIN)
                        trancheFee = _LoanApplicationFull.Record.FEE1_MIN / Convert.ToDecimal(_LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED) * 100.00m;
                }
            }


            //if (adminFee != 0)
            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.ApplicationApproval] = new GenericFees
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.LoanAmount,
                Schemes = new List<GenericFee> { new GenericFee { Rate = adminFee } }
            };

            //if (trancheFee != 0)
            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.TrancheDisburse] = new GenericFees
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.LoanAmount,
                Schemes = new List<GenericFee> { new GenericFee { Rate = trancheFee } }
            };

            //if (_LoanApplicationFull.Record.PREPAYMENT_RATE > 0)
            //{
            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.PrepaymentOwnFunds] = new PrepaymentFeesBySteps
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.Balance,
                Schemes = new List<SchemeByScheduleStep> { new SchemeByScheduleStep { FromScheduleSteps = 0, Fee = new GenericFee { Rate = _LoanApplicationFull.Record.PREPAYMENT_RATE } } }
            };

            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.PrepaymentAndCloseOwnFunds] = new GenericFees
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.Balance,
                Schemes = new List<GenericFee> { new GenericFee { Rate = _LoanApplicationFull.Record.PREPAYMENT_RATE } }
            };
            //}

            //if (_LoanApplicationFull.Record.OVERPAY_PREPAYMENT_RATE > 0)
            //{
            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.PrepaymentFromNewLoan] = new PrepaymentFeesBySteps
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.Balance,
                Schemes = new List<SchemeByScheduleStep> { new SchemeByScheduleStep { FromScheduleSteps = 0, Fee = new GenericFee { Rate = _LoanApplicationFull.Record.OVERPAY_PREPAYMENT_RATE } } }
            };

            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.PrepaymentAndCloseNewLoan] = new GenericFees
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.Balance,
                Schemes = new List<GenericFee> { new GenericFee { Rate = _LoanApplicationFull.Record.OVERPAY_PREPAYMENT_RATE } }
            };
            //}

            //if (_LoanApplicationFull.Record.PENALTY_ON_PAYMENT_IN_OTHER_BANK > 0)
            //{
            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.PrepaymentFromOtherBank] = new PrepaymentFeesBySteps
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.Balance,
                Schemes = new List<SchemeByScheduleStep> { new SchemeByScheduleStep { FromScheduleSteps = 0, Fee = new GenericFee { Rate = _LoanApplicationFull.Record.PENALTY_ON_PAYMENT_IN_OTHER_BANK } } }
            };

            feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.PrepaymentAndCloseOtherBank] = new GenericFees
            {
                FeeCalculationBaseDebt = FeeCalculationBaseDebt.Balance,
                Schemes = new List<GenericFee> { new GenericFee { Rate = _LoanApplicationFull.Record.PENALTY_ON_PAYMENT_IN_OTHER_BANK } }
            };
            //}

            if (_LoanApplicationFull.Record.PENALTY_SCHEMA_ID > 0)
            {
                feeAndPenaltyAccrualPolicy[FeeAndPenaltyChargeType.OverduePenalty] = new PenaltyFees { Id = _LoanApplicationFull.Record.PENALTY_SCHEMA_ID };
            }

            _PutApplicationRequest.Application.FeeAndPenaltyAccrualPolicy = feeAndPenaltyAccrualPolicy;
            #endregion

            #region PurposeId
            string mPurposeId = "040600";
            List<BaseProductPurpose> mBaseProductPurposeList = this.BaseProductPurposeList();
            BaseProductPurpose mBaseProductPurpose = null;
            try
            {
                mBaseProductPurpose = mBaseProductPurposeList.First(item => item.BASE_PRODUCT_ID == _LoanApplicationFull.Record.BASE_PRODUCT_ID);
            }
            catch { }

            if (mBaseProductPurpose != null)
            {
                mPurposeId = mBaseProductPurpose.PURPOSE_ID;
            }

            _PutApplicationRequest.Application.PurposeId = mPurposeId;

            _PutApplicationRequest.Application.InterestAccrualPolicy = new InterestAccrualPolicy { InterestRate = _LoanApplicationFull.Record.INTEREST_RATE, NuInterestRate = _LoanApplicationFull.Record.NOTUSED_RATE, OverlimitInterestRate = _LoanApplicationFull.Record.OVER_LIMIT_INTEREST_RATE, InterestFreePeriod = (uint)_LoanApplicationFull.Record.INTEREST_FREE_PERIOD };

            _PutApplicationRequest.Application.Resources = loanProduct.FinanceResources;

            //_PutApplicationRequest.Application.State = ApplicationStatus.New;
            _PutApplicationRequest.Application.PrepaymentPolicy = new PrepaymentPolicy
            {
                MinPrepaymentCount = (byte)_LoanApplicationFull.Record.MIN_PREPAYMENT_COUNT,
                MinPrepaymentAmount = _LoanApplicationFull.Record.MIN_PREPAYMENT_AMOUNT,
                //TODO ასაღებია პროდუქტიდან
                PrepaymentRescheduleStrategy = PrepaymentRescheduleStrategy.BySamePeriod
            };

            string mCity = "თბილისი";
            try
            {
                mCity = this.CityList().First(item => item.CITY_ID == _LoanApplicationFull.ClientInfo.CLIENT_CITY_ID).CITY_NAME;
            }
            catch { }

            this.LMSAddClientBusiness(_LoanApplicationFull.ClientInfo.CLIENT_NO, "0133", "29 - ფიზიკური პირი", "0200", _LoanApplicationFull.ClientInfo.CLEINT_LEGAL_ADDRESS, mCity);

            #endregion PurposeId

            #region loanCover
            //სესხის ავტომატური გადაფარვა ამოგვაღებინა ბექმა
            //int loan2Cover = 0;
            //try
            //{
            //    loan2Cover = _LoanApplicationFull.CurrentLoanList.OrderBy(o => o.CURRENT_LOAN_AMOUNT).ToList().First(item => item.CURRENT_LOAN_COVER).LOAN_ID;
            //}
            //catch { }

            //if (!mSpecialLoanTypes.IsPOS && !mSpecialLoanTypes.IsPOSCard && loan2Cover != 0)
            //{
            //    _PutApplicationRequest.Application.Extension = new ApplicationExtension
            //    {
            //        RefinancedLoanIds = new List<int> { loan2Cover }
            //    };
            //}

            #endregion loanCover

            #region Collateral Count, EnsureType



            int mColateralCount = 0;
            if (mSpecialLoanTypes.IsAuto || mSpecialLoanTypes.IsClearence)
            {
                mColateralCount = 1;
            }
            if (_LoanApplicationFull.GuarantorList.Count > 0)
            {
                mColateralCount = _LoanApplicationFull.GuarantorList.Count;
            }
            if (_LoanApplicationFull.RealEstate.CollateralList.Count > 0)
            {
                if (mSpecialLoanTypes.IsIpoCollaborative)
                {
                    mColateralCount = 0;
                }
                else
                {
                    mColateralCount = _LoanApplicationFull.RealEstate.CollateralList.Count;
                }
            }
            mCurentCashCover = this.AplicationCashCoverLinkedAmountGet(_LoanApplicationFull.Record.APPLICATION_ID);
            if (_LoanApplicationFull.CashCover.CashCoverDepositList != null && _LoanApplicationFull.CashCover.CashCoverDepositList.Count > 0)
            {
                mColateralCount = mCurentCashCover.CashCoverDepositList.Count;
            }

            if (mColateralCount > 0)
                _PutApplicationRequest.Application.EnsureType = EnsureType.Full;
            else
                _PutApplicationRequest.Application.EnsureType = EnsureType.None;

            #endregion Collateral Count


            if (mSpecialLoanTypes.IsGarantee)
                _PutApplicationRequest.Application.Extension = new ApplicationExtension { GuaranteePurposeId = 1 };// გარანტია - აკრედიტივის შემთხვევაში pipeline.GUARANTEE_TYPES

            _PutApplicationRequest.Application.Attributes.Add("LoanGroupId", "-1");//ეს ერთი ჯგუფია მატრო ცნობარში

            List<ApplicationGuarantor> mApplicationCoborrowerList = _LoanApplicationFull.GuarantorList.Where(item => item.GUARANTOR_TYPE_ID == 2).ToList();
            if (mApplicationCoborrowerList.Count > 0)
            {
                _PutApplicationRequest.Application.Coborrowers = new LAPI.Core.IntList { mApplicationCoborrowerList[0].GUARANTOR_CLIENT_NO };
            }

            //ApplicationRequest.PrepaymentRescheduleType = PrepaymentRescheduleTypes.PrepaymentRescheduleByPMT;

            _PutApplicationRequest.Application.Kind = (LoanKind?)_LoanApplicationFull.Record.LOAN_TYPE;
            _PutApplicationRequest.Application.IsReusable = _LoanApplicationFull.Record.CREDIT_TYPE == 2/*Reusable*/;
            _PutApplicationRequest.Application.UsageType = _LoanApplicationFull.Record.CREDIT_TYPE == 0/*Single*/ ? LoanUsageType.Entire : LoanUsageType.Partial;
            _PutApplicationRequest.Application.IsCard = _LoanApplicationFull.Record.IS_CARD;

            if (_LoanApplicationFull.Auto.DEALER_ID != 0)
            {
                _PutApplicationRequest.Application.InstallmentDetails = new InstallmentDetails { ObjectId = _LoanApplicationFull.Auto.DEALER_ID };
            }
            //if (_LoanApplicationFull.Record.IS_CASH_COVER)    //ALTA warning: დასაშვებია მხოლოდ ალტას ქეშქავერის შემთხვევაში
            //{
            //    _ApplicationRequest.Application.Markers = new string[] { "CashCover" };
            //}

            #endregion other parameters

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "other parameters done");

            #endregion Application

            #region ==Schedules==

            List<DateTime> scheduleDates = new List<DateTime>();

            scheduleDebtGenerator.ScheduleTypeId = _LoanApplicationFull.Record.SCHEDULE_TYPE;
            scheduleDebtGenerator.EndOfMonth = _EndOfMonthType;
            scheduleDebtGenerator.GenericSchedule = _LoanApplicationFull.Record.GENERIC_SCHEDULE;

            if (mSpecialLoanTypes.IsCard)
            {
                if (mSpecialLoanTypes.IsOverdraft)
                {
                    unplannedDebtGenerator.GraceDays = Convert.ToByte(_AppSets["GraceDays119"]);
                }
                else if (mSpecialLoanTypes.IsCreditCard)
                {
                    unplannedDebtGenerator.GraceDays = (byte)_LoanApplicationFull.Record.INTEREST_FREE_PERIOD;
                }
                //TODO UnplannedDebtCalculationSettings
                _PutApplicationRequest.Application.CashflowScheduler = unplannedDebtGenerator;
            }
            else
            {
                _PutApplicationRequest.Application.CashflowScheduler = scheduleDebtGenerator;
            }

            scheduleDebtGenerator.ExtraScheduleParameters = new ExtraScheduleBuildParamteters();
            DistributedInterestList mDistributedInterestList = new DistributedInterestList();
            int mGraceStep = (mIntervalType.Value == 0.5m) ? _LoanApplicationFull.Record.GRACE_PERIOD * 2 : _LoanApplicationFull.Record.GRACE_PERIOD;

            if (mSpecialLoanTypes.IsClearenceDealer && _LoanApplicationFull.Auto.GRACE_PERIOD > 0)
            {
                scheduleDebtGenerator.ExtraScheduleParameters.GracePayments[ScheduleElementType.Principal] =
                    new LAPI.Core.GraceSettings
                    {
                        Mode = LAPI.Core.GraceMode.FirstPayments,
                        Step = _LoanApplicationFull.Auto.GRACE_PERIOD
                    };
            }
            else if (mSpecialLoanTypes.IsCashCover)
            {
                scheduleDebtGenerator.PmtCount = Convert.ToInt32(Math.Round(Convert.ToDouble(scheduleDebtGenerator.PmtCount / 30.40), 0));

                var mGraceSettings = new LAPI.Core.GraceSettings
                {
                    Mode = LAPI.Core.GraceMode.FirstPayments,
                    Step = scheduleDebtGenerator.PmtCount
                };

                if (_LoanApplicationFull.CashCover.CashCoverAttributes.SCHEDULE_TYPE_ID == 1)
                {
                    if (scheduleDebtGenerator.ExtraScheduleParameters.GracePayments == null)
                        scheduleDebtGenerator.ExtraScheduleParameters.GracePayments = new GracePayments();
                    scheduleDebtGenerator.ExtraScheduleParameters.GracePayments[ScheduleElementType.Principal | ScheduleElementType.Interest] = mGraceSettings;

                }
                else if (_LoanApplicationFull.CashCover.CashCoverAttributes.SCHEDULE_TYPE_ID == 2)
                {
                    if (scheduleDebtGenerator.ExtraScheduleParameters.GracePayments == null)
                        scheduleDebtGenerator.ExtraScheduleParameters.GracePayments = new GracePayments();

                    scheduleDebtGenerator.ExtraScheduleParameters.GracePayments[ScheduleElementType.Principal] = mGraceSettings;
                }
            }
            else if (!mSpecialLoanTypes.IsGaranteeGeocell)
            {
                /*გადანაწილებული პროცენტის გრაფიკის აგება*/
                if (_LoanApplicationFull.Record.GRACE_PERIOD > 0)
                {
                    scheduleDebtGenerator.ScheduleBuildMode = ScheduleBuildMode.DistributedInterests;
                    scheduleDebtGenerator.ExtraScheduleParameters.GracePayments = new GracePayments{
                        {
                            ScheduleElementType.Principal,  new LAPI.Core.GraceSettings
                            {
                                Mode = LAPI.Core.GraceMode.FirstPayments,
                                Step = mGraceStep
                            }
                         }};
                }

            }



            FillScheduleResponse loanScheduleResponse = new FillScheduleResponse();
            List<RepaymentScheduleSegment> dateSegments;

            if (!mSpecialLoanTypes.IsCard)
            {
                DateTime mDate = DateTime.Now.Date;
                DateTime? msgdEndDate = null;
                int? msgdPmtCount = null;
                byte? mPaymentDay = null;
                byte? mSecondPaymentDay = null;
                DateTime? mFirstPaymentDate = null;

                if (mSpecialLoanTypes.IsPOS || mSpecialLoanTypes.IsGaranteeBeeline)
                {
                    mDate = (DateTime)scheduleDebtGenerator.FirstPaymentDate;
                    mFirstPaymentDate = scheduleDebtGenerator.FirstPaymentDate;
                    msgdPmtCount = scheduleDebtGenerator.PmtCount;
                }
                else if (mSpecialLoanTypes.IsGaranteeGeocell)
                {
                    mDate = (DateTime)_PutApplicationRequest.Application.Term.Start;
                    msgdEndDate = _PutApplicationRequest.Application.Term.End;
                    mFirstPaymentDate = scheduleDebtGenerator.FirstPaymentDate;

                    scheduleDebtGenerator.ExtraScheduleParameters.GracePayments = new GracePayments();
                    scheduleDebtGenerator.ExtraScheduleParameters.GracePayments[ScheduleElementType.Principal | ScheduleElementType.Interest] = new LAPI.Core.GraceSettings { Mode = LAPI.Core.GraceMode.FirstPayments, Step = scheduleDebtGenerator.PmtCount };

                }
                else if (mSpecialLoanTypes.IsCashCover)
                {
                    mDate = (DateTime)_PutApplicationRequest.Application.Term.Start;
                    msgdEndDate = _PutApplicationRequest.Application.Term.End;
                }
                else
                {
                    mDate = (DateTime)_PutApplicationRequest.Application.Term.Start;
                    mPaymentDay = scheduleDebtGenerator.PaymentDay;
                    mSecondPaymentDay = scheduleDebtGenerator.SecondPaymentDay;
                    mFirstPaymentDate = scheduleDebtGenerator.FirstPaymentDate;
                    msgdEndDate = _PutApplicationRequest.Application.Term.End;
                    msgdPmtCount = null;
                }

                #region scheduleDates

                dateSegments = new List<RepaymentScheduleSegment>
                {
                    new RepaymentScheduleSegment
                    {
                        Date = mDate,
                        PaymentInterval = scheduleDebtGenerator.PaymentInterval,
                        EndOfMonth = (mPaymentDay == 0) ?EndOfMonthMode.PaymentDay : EndOfMonthMode.None,
                        PaymentDay = (mPaymentDay == 0) ? null : mPaymentDay,
                        SecondPaymentDay = mSecondPaymentDay
                    }
                };



                try
                {
                    scheduleDates = _lmsClient.GenerateScheduleDates(new GenerateScheduleDatesRequest
                    {
                        RequestHeaders = LMSRequestHeadersGet(),
                        StartDate = _PutApplicationRequest.Application.Term.Start,
                        EndDate = msgdEndDate,
                        PaymentCount = msgdPmtCount,
                        ScheduleTypeId = scheduleDebtGenerator.ScheduleTypeId.Value,
                        Segments = dateSegments,
                        FirstPaymentDate = mFirstPaymentDate,// scheduleDebtGenerator.FirstPaymentDate,
                        LastPaymentDate = scheduleDebtGenerator.LastPaymentDate
                    }).Result.OrderBy(dt => dt.Date).ToList();
                }
                catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
                {
                    this.LogFaunt(fault, mUserID, "GenerateScheduleDates");
                }
                catch (Exception Ex)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "GenerateScheduleDates: " + Ex.Message);
                }

                if (!mSpecialLoanTypes.IsPOS)
                {
                    int mLoanScheduleCount = scheduleDates.Count;
                    if (mLoanScheduleCount > 2)
                    {
                        DateTime mLoanScheduleItemLast = scheduleDates[mLoanScheduleCount - 1];
                        DateTime mLoanScheduleItemPreLast = scheduleDates[mLoanScheduleCount - 2];
                        TimeSpan mTimeSpanSchedule = mLoanScheduleItemLast.Date - mLoanScheduleItemPreLast.Date;
                        if (mTimeSpanSchedule.TotalDays < 15 && mLoanScheduleItemLast.Date.Month == mLoanScheduleItemPreLast.Date.Month)
                        {
                            scheduleDates.Remove(mLoanScheduleItemPreLast);
                        }
                    }
                }
                /*გადანაწილებული პროცენტის გრაფიკის აგება*/

                if (_LoanApplicationFull.Record.GRACE_PERIOD > 0 && _LoanApplicationFull.Record.INTEREST_RATE != 0)
                {
                    for (int i = 0; i <= mGraceStep - 1; i++)

                        mDistributedInterestList.Add(new DistributedInterest
                        {
                            DistributionSourceDate = scheduleDates[i],
                            DestionationItemInfo = new DistributedInterestDestionationItemInfo
                            {
                                DistributeToRestSchedule = true
                            }
                        });
                    scheduleDebtGenerator.ExtraScheduleParameters.DistributedInterests = mDistributedInterestList;
                }

                #endregion scheduleDates



                /*TODO VTB loanCalculator.CalculateLoanSchedule( garantiis dros gadaecemoda AltaSoft.Core.Pipeline.LoanType.Guarantee , amis magivrad*/
                ScheduleElementType mElements = (mSpecialLoanTypes.IsGarantee) ? ScheduleElementType.Interest : ScheduleElementType.Principal | ScheduleElementType.Interest | ScheduleElementType.Balance;
                loanScheduleResponse = _lmsClient.FillSchedule(new FillScheduleRequest
                {
                    RequestHeaders = LMSRequestHeadersGet(),
                    ScheduleDates = scheduleDates,
                    GraceDates = scheduleDebtGenerator.ExtraScheduleParameters.GracePayments?.ToDictionary(i => i.Key, i => i.Value),
                    Segments = dateSegments,
                    InterestRate = _PutApplicationRequest.Application.InterestAccrualPolicy.InterestRate.Value,
                    ScheduleTypeId = scheduleDebtGenerator.ScheduleTypeId.Value,
                    ScheduledDisbursements = new Dictionary<DateTime, decimal>() { { _PutApplicationRequest.Application.Term.Start, _PutApplicationRequest.Application.Amount.Amount } },
                    DistributedInterests = mDistributedInterestList,
                    Elements = mElements
                });

                if (_PutApplicationRequest.Application.Extension == null)
                    _PutApplicationRequest.Application.Extension = new ApplicationExtension();

                _PutApplicationRequest.Application.Extension.PmtAmountRegular = loanScheduleResponse.Pmt;
                _PutApplicationRequest.Schedule = loanScheduleResponse.Result;

            }

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "schedules done");
            #endregion ==Schedules==

            #region Attributes

            // NEW VARIANT ATTRIBUTES
            List<ProductAttribute> mApplicationProductAttributeList = this.ApplicationProductAttributesGet(_LoanApplicationFull.Record.APPLICATION_ID);
            foreach (ProductAttribute productAttribute in mApplicationProductAttributeList)
            {
                _PutApplicationRequest.Application.Attributes.Add(productAttribute.ATTRIB_CODE, productAttribute.ATTRIB_VALUE);
            }

            // ADDITIONAL VARIANT ATTRIBUTES
            List<ApplicationAdditionalAttribute> mAttributsToAdd = _LoanApplicationFull.AdditionalAttribules.Where(item => item.ATTRIBUTE_TYPE_ID == 1 && !item.ATTRIBUTE_CODE.Contains("OFFSET_")).ToList();
            mAttributsToAdd.ForEach(item =>
            {
                _PutApplicationRequest.Application.Attributes.Add(item.ATTRIBUTE_CODE, item.ATTRIBUTE_VALUE);
            });

            // ADDITIONAL OFFSET ATTRIBUTES
            if (_LoanApplicationFull.RealEstate.Main.IS_OFFSET)
            {
                int mOffsetAttributeCount = _LoanApplicationFull.AdditionalAttribules.Where(item => item.ATTRIBUTE_TYPE_ID == 1 && item.ATTRIBUTE_CODE.Contains("OFFSET_")).Count();
                if (mOffsetAttributeCount != 3)
                {
                    mLMSAnswer.LoanApplicationRecord = this.ApplicationRec(mApplicationID, mUserID);
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, String.Format("IS_OFFSET: შეუვსებელი ოფსეტის ატრიბუტები"));
                    return mLMSAnswer;
                }

                mAttributsToAdd = _LoanApplicationFull.AdditionalAttribules.Where(item => item.ATTRIBUTE_TYPE_ID == 1 && item.ATTRIBUTE_CODE.Contains("OFFSET_")).ToList();
                mAttributsToAdd.ForEach(item =>
                {
                    _PutApplicationRequest.Application.Attributes.Add(item.ATTRIBUTE_CODE, item.ATTRIBUTE_VALUE);
                });
            }

            //MFile Attributes
            string mApplicationMFileAttribute_installment = "";
            mApplicationMFileAttribute_installment = ApplicationMFileAttributeGet(_LoanApplicationFull.Record.APPLICATION_ID, 1);
            if (mApplicationMFileAttribute_installment != "")
            {
                _PutApplicationRequest.Application.Attributes.Add("Loan M-file number", mApplicationMFileAttribute_installment);
            }

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "attributes done");
            #endregion attributes

            #region ExtraExpanses
            /*
                ID	NAME
                1	წარმოსადგენი დოკუმენტაციის ხარჯი
                31	სანოტარო ხარჯები (ლარში)
                32	შსს–ის მომსახურების სააგენტოს ხარჯი (ლარში)
                33	მიმდინარე ანგარიშის მომსახურების საკომისიო (ლარში)
                34	საჯარო რეესტრში უზრუნველყოფის რეგისტრაციის ხარჯი (ლარში)
                35	ავტომობილის შეფასების ხარჯი
                36	ბარათით მომსახურების წლიური საკომისიო
                37	განაღდების საკომისიო (ბანკში ან ბანკომატში)
             * */

            List<ApplicationAdditionalAttribute> mExtraExpansesToAdd = _LoanApplicationFull.AdditionalAttribules.Where(item => item.ATTRIBUTE_TYPE_ID == 2).ToList();
            var expanses = new List<ExtraExpanse>();

            mExtraExpansesToAdd.ForEach(item =>
            {
                ExtraExpanseAppearance? mPeriodType = null;
                PeriodIntervalType? mExtraExpanseIntervalTypes = null;
                if (item.PERIOD_TYPE == 0)
                {
                    mPeriodType = ExtraExpanseAppearance.SpecificDate;
                }
                else if (item.PERIOD_TYPE == 1)
                {
                    mPeriodType = ExtraExpanseAppearance.LoanStartDate;
                }
                else if (item.PERIOD_TYPE == 2)
                {
                    mPeriodType = ExtraExpanseAppearance.LoanEndDate;
                }
                else if (item.PERIOD_TYPE == 3)
                {
                    mPeriodType = ExtraExpanseAppearance.Periodic;
                }

                if (item.INTERVAL_TYPE == 1)
                {
                    mExtraExpanseIntervalTypes = PeriodIntervalType.Month;
                }
                else
                {
                    mExtraExpanseIntervalTypes = null;
                }

                expanses.Add(new ExtraExpanse
                {
                    TypeId = Convert.ToInt32(item.ATTRIBUTE_CODE),
                    Amount = new AmountAndCurrency { Amount = Convert.ToDecimal(item.ATTRIBUTE_VALUE), Ccy = "GEL" },
                    Appearance = mPeriodType,
                    IntervalStep = (item.INTERVAL_STEP == 0) ? null : (byte?)item.INTERVAL_STEP,
                    Interval = mExtraExpanseIntervalTypes,
                    OneTimeFeeDate = DateTime.Now.Date,
                });
            });

            if (mSpecialLoanTypes.IsAuto || mSpecialLoanTypes.IsClearence)
            {
                int index = mExtraExpansesToAdd.FindIndex(item => item.ATTRIBUTE_CODE == "32");
                if (index < 0)
                {
                    decimal mCOMMISSION_SHSS_AMOUNT = Convert.ToDecimal(this.SysCalcParameterList().First(item => item.PRODUCT_ID == _LoanApplicationFull.Record.PRODUCT_ID && item.PARAMETER_SYS_NAME == "COMMISSION_SHSS_AMOUNT").PARAMETER_VALUE);
                    expanses.Add(new ExtraExpanse
                    {
                        TypeId = 32,
                        Amount = new AmountAndCurrency
                        {
                            Amount = Convert.ToDecimal(mCOMMISSION_SHSS_AMOUNT),
                            Ccy = "GEL"
                        },
                        Appearance = ExtraExpanseAppearance.SpecificDate,
                        IntervalStep = null,
                        Interval = null,
                        OneTimeFeeDate = DateTime.Now.Date,
                    });
                }
            }

            if (mSpecialLoanTypes.IsAutoUsed)
            {//TODO გადასატანია პროცედურაში
                int index = mExtraExpansesToAdd.FindIndex(item => item.ATTRIBUTE_CODE == "38");
                if (index < 0 && _LoanApplicationFull.Auto.INCLUDE_DEALER_AMOUNT)
                {
                    decimal mCOMMISSION_AUTO_DEAL_MIN_AMOUNT = Convert.ToDecimal(this.SysCalcParameterList().First(item => item.PRODUCT_ID == _LoanApplicationFull.Record.PRODUCT_ID && item.PARAMETER_SYS_NAME == "COMMISSION_AUTO_DEAL_MIN_AMOUNT").PARAMETER_VALUE);
                    expanses.Add(new ExtraExpanse
                    {
                        TypeId = 38,
                        Amount = new AmountAndCurrency
                        {
                            Amount = Convert.ToDecimal(mCOMMISSION_AUTO_DEAL_MIN_AMOUNT),
                            Ccy = "GEL"
                        },
                        Appearance = ExtraExpanseAppearance.SpecificDate,
                        IntervalStep = null,
                        Interval = null,
                        OneTimeFeeDate = DateTime.Now.Date,
                    });
                }
            }

            if (mSpecialLoanTypes.IsClearence)
            {
                int index = mExtraExpansesToAdd.FindIndex(item => item.ATTRIBUTE_CODE == "40");
                if (index < 0)
                {
                    expanses.Add(new ExtraExpanse
                    {
                        TypeId = 40,
                        Amount = new AmountAndCurrency
                        {
                            Amount = Convert.ToDecimal(_LoanApplicationFull.Auto.TREASURY_AMOUNT),
                            Ccy = "GEL"
                        },
                        Appearance = ExtraExpanseAppearance.SpecificDate,
                        IntervalStep = null,
                        Interval = null,
                        OneTimeFeeDate = DateTime.Now.Date,
                    });
                }
            }


            if (mSpecialLoanTypes.IsCreditCard)
            {
                try
                {
                    ApplicationAdditionalAttribute mApplicationAdditionalAttribute = mExtraExpansesToAdd.First(item => item.ATTRIBUTE_CODE == "36");
                    expanses.Add(new ExtraExpanse
                    {
                        TypeId = 36,
                        Amount = new AmountAndCurrency
                        {
                            Amount = Convert.ToDecimal(mApplicationAdditionalAttribute.ATTRIBUTE_VALUE),
                            Ccy = "GEL"
                        },
                        Appearance = ExtraExpanseAppearance.SpecificDate,
                        OneTimeFeeDate = DateTime.Now.Date,
                    });
                }
                catch { }
            }

            if (mSpecialLoanTypes.IsCreditCardGold || mSpecialLoanTypes.IsCreditCardGoldPreaproved)
            {
                if (mExtraExpansesToAdd.Where(item => item.ATTRIBUTE_CODE == "36").ToList().Count == 0)
                {
                    if (expanses.Where(it => it.TypeId == 36).ToList().Count == 0)
                    {
                        expanses.Add(new ExtraExpanse
                        {
                            TypeId = 36,
                            Amount = new AmountAndCurrency
                            {
                                Amount = Convert.ToDecimal(50),//TODO inteface
                                Ccy = "GEL"
                            },
                            Appearance = ExtraExpanseAppearance.SpecificDate,
                            OneTimeFeeDate = DateTime.Now.Date,
                        });
                    }
                }
            }

            if (mSpecialLoanTypes.IsCreditCardPreapproved || mSpecialLoanTypes.IsCreditCardStandard)
            {
                if (mExtraExpansesToAdd.Where(item => item.ATTRIBUTE_CODE == "36").ToList().Count == 0)
                {
                    if (expanses.Where(it => it.TypeId == 36).ToList().Count == 0)
                    {
                        expanses.Add(new ExtraExpanse
                        {
                            TypeId = 36,
                            Amount = new AmountAndCurrency
                            {
                                Amount = Convert.ToDecimal(30),//TODO inteface
                                Ccy = "GEL"
                            },
                            Appearance = ExtraExpanseAppearance.SpecificDate,
                            OneTimeFeeDate = DateTime.Now.Date,
                        });
                    }
                }
            }



            //try
            //{
            //    if (mSpecialLoanTypes.IsPOS && _LoanApplicationFull.Record.PRODUCT_ID != 224)
            //    {
            //        //განვადების სიცოცხლის დაზღვევა - 2 ლარი
            //        expanses.Add(new ExtraExpanse
            //        {
            //            TypeId = 43,
            //            Amount = new AmountAndCurrency
            //            {
            //                Amount = Convert.ToDecimal(2),
            //                Ccy = "GEL"
            //            },
            //            Interval = PeriodIntervalType.Month,
            //            IntervalStep = 1,
            //            Appearance = ExtraExpanseAppearance.Periodic,

            //        });
            //    }
            //}
            //catch { }

            _PutApplicationRequest.ExtraExpanses = expanses;

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "extraexpanses done");
            #endregion ExtraExpanses

            #region PutApplication
            PutApplicationResponse mPutApplicationResponse = null;
            try
            {
                if (_LoanApplicationFull.Record.APPLICATION_SOURCE_ID == 1 && _LoanApplicationFull.Record.BASE_PRODUCT_ID == 102 && _LoanApplicationFull.Record.LMS_LOAN_ID > 0)
                {
                    mLMSAnswer.LoanApplicationRecord = this.ApplicationRec(mApplicationID, mUserID);
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, String.Format("PutApplication: პარალელური გამოძახება: APPLICATION_SOURCE_ID == 1,BASE_PRODUCT_ID == 102, LMS_LOAN_ID = {0}", _LoanApplicationFull.Record.LMS_LOAN_ID));
                    return mLMSAnswer;
                }
                mPutApplicationResponse = _lmsClient.PutApplication(_PutApplicationRequest);
            }
            catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
            {
                this.LogFaunt(fault, mUserID, "PutApplication");
            }
            catch (Exception Ex)
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutApplication: " + Ex.Message);
            }
            #endregion PutApplication

            #endregion ====<LOAN APPLICATION>====

            #region after Application Add

            if (mPutApplicationResponse != null && mPutApplicationResponse.ApplicationId > 0)
            {
                _ApplicationId = mPutApplicationResponse.ApplicationId;
            }
            _AgreementNo = _PutApplicationRequest.Application.AgreementNo;

            /*თუ LMS-ში დაემატა აპლიკაციას ვუწერთ LMS-ის _ApplicationId-ს*/
            if (_ApplicationId > 0)
            {
                this.ApplicationLmsLoanIDSet(mApplicationID, mUserID, _ApplicationId);
                try
                {
                    this.LMSAdminDebtsAdd(_LoanApplicationFull, _ApplicationId, mUserID, mUserAndDeptId);
                }
                catch (Exception Ex)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "LMSAdminDebtsAdd: " + Ex.Message);
                }
            }
            else
            {
                mLMSAnswer.LoanApplicationRecord = this.ApplicationRec(mApplicationID, mUserID);
                return mLMSAnswer;
            }

            /*ხელშეკრულების განსაკუთრებული პირობები*/
            string specialParamValue = "";
            try
            {
                foreach (var appAdminDebts in _LoanApplicationFull.AppAdminDebtList.Where(ad => ad.ADMIN_GROUP_ID != 1 && ad.ADMIN_GROUP_ID != 6).ToList())
                {
                    specialParamValue += appAdminDebts.LMS_COMMENT + "\r\n";
                }

                if (specialParamValue.Length > 0)
                {
                    specialParamValue += "." + this.AdminDebtItemList().FindLast(i => i.GROUP_ID == 6).ITEM_NAME + "." + "\r\n";
                }

                //foreach (var appAdminDebts in _LoanApplicationFull.AppAdminDebtList.Where(ad => ad.ADMIN_GROUP_ID == 6).ToList())
                //{
                //    specialParamValue += "." + appAdminDebts.ADMIN_ITEM_NAME + "." + "\r\n";
                //}
                this.LmsAgreementSpecialParamsAdd(_ApplicationId, specialParamValue);
            }
            catch (Exception Ex)
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "LmsAgreementSpecialParamsAdd: " + Ex.Message);
            }
            /*end ხელშეკრულების განსაკუთრებული პირობები*/

            #region ==life insurance==

            List<LMSInsuranceType> mLMSInsuranceTypeList = this.LMSInsuranceTypeList();
            bool mIsInsuranceLifeMandatory = false;
            int mIsInsuranceLifeMandatoryCompany = 0;
            decimal mIsInsuranceLifeMandatoryAmount = 0;
            decimal mIsInsuranceLifeMandatoryMinAmount = 0;

            try
            {
                InsuranceMandatoReesult mInsuranceMandatoReesult = this.LifeInsuranceMandatoryGet(_LoanApplicationFull.Record.APPLICATION_ID);
                mIsInsuranceLifeMandatory = mInsuranceMandatoReesult.IS_LIFE;
                mIsInsuranceLifeMandatoryCompany = mInsuranceMandatoReesult.COMPANY_ID;
                mIsInsuranceLifeMandatoryAmount = mInsuranceMandatoReesult.AMOUNT;
                mIsInsuranceLifeMandatoryMinAmount = mInsuranceMandatoReesult.MIN_AMOUNT;
            }
            catch { }

            byte? mLifeInsuranceTypeID = null;

            if (mIsInsuranceLifeMandatoryCompany == 0)
                mLifeInsuranceTypeID = Convert.ToByte(mLMSInsuranceTypeList.First(ins => ins.SYS_CODE == "life").INSURANCE_TYPE_ID);
            else
                mLifeInsuranceTypeID = Convert.ToByte(mLMSInsuranceTypeList.First(ins => ins.SYS_CODE == "vtb life fee").INSURANCE_TYPE_ID);


            if ((mSpecialLoanTypes.IsRealEstate && _LoanApplicationFull.RealEstate.Main.INSURANCE_LIFE_YN) || mIsInsuranceLifeMandatory/* სავალდებულო სიცოცხლის დაღზვევა ზოგიერთი პროდუქტისთვის*/)
            {
                try
                {
                    int mInsuranceCompanyID = 0;
                    if (mSpecialLoanTypes.IsRealEstate)
                    {
                        mInsuranceCompanyID = (_LoanApplicationFull.RealEstate.Main.INSURANCE_COMPANY_ID == 0) ? 1 : _LoanApplicationFull.RealEstate.Main.INSURANCE_COMPANY_ID;
                    }
                    else if (mIsInsuranceLifeMandatory /* სავალდებულო სიცოცხლის დაღზვევა ზოგიერთი პროდუქტისთვის*/)
                    {
                        mInsuranceCompanyID = mIsInsuranceLifeMandatoryCompany;
                    }
                    else
                    {
                        mInsuranceCompanyID = (_LoanApplicationFull.ClientInfo.INSURANCE_COMPANY_ID == 0) ? 1 : _LoanApplicationFull.ClientInfo.INSURANCE_COMPANY_ID;
                    }
                    _InsuranceProducts = Array.FindLast(_InsuranceProductsArray, s => s.IS_ACTIVE && s.COMPANY_ID == mInsuranceCompanyID && s.TYPE_ID == mLifeInsuranceTypeID && s.LOAN_PRODUCT_ID.Contains(_LoanApplicationFull.Record.BASE_PRODUCT_ID.ToString()));

                    if (_InsuranceProducts == null)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "ამ კომპანიის სიცოცხლის დაზღვევა ამ პროდუქტისთვის არ არის გაწერილი LMS-ში");
                        return mLMSAnswer;
                    }
                    decimal? mRate = _InsuranceProducts.MAX_RATE_CLIENT;

                    var mInsurancePolicy = new LAPI.Core.Policy
                    {
                        CompanyId = _InsuranceProducts.COMPANY_ID,
                        Term = new Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                        State = PolicyState.New, /*new*/
                        ProductId = (int)_InsuranceProducts.INSURANCE_PRODUCT_ID,
                        LoanId = _ApplicationId,
                    };

                    try
                    {
                        Attributes mInsuranceAttributes = new Attributes();
                        if (mIsInsuranceLifeMandatoryCompany == 0)
                        {
                        mInsuranceAttributes.Add("PRODUCT_SALES_MANAGER", mUserAndDeptIdI.UserIdentification.Id.ToString());
                        }

                        InsuranceAccrualSchedule mInsuranceAccrualSchedule = new InsuranceAccrualSchedule();

                        /* 3 თვიანი დაზღვევის საჩუქარი
                        int LifuInsuranceGiftMonth = 3;
                        int mscheduleDatesIndexToStart = (mIntervalType.Value == 0.5m) ? (LifuInsuranceGiftMonth * 2) -1 : (LifuInsuranceGiftMonth - 1);

                        try
                        {
                            mInsuranceAccrualSchedule.Add(scheduleDates[0], false);
                            mInsuranceAccrualSchedule.Add(scheduleDates[mscheduleDatesIndexToStart], true);
                        }
                        catch
                        { }
                        */

                        decimal? mFixedAmount = null;
                        decimal? mMinAmount = null;
                        LAPI.Core.InsuranceCalculationType mCalculationType = LAPI.Core.InsuranceCalculationType.LoanBalance;
                        if (mIsInsuranceLifeMandatoryAmount != 0)
                        {
                            mCalculationType = LAPI.Core.InsuranceCalculationType.Fixed;
                            mFixedAmount = mIsInsuranceLifeMandatoryAmount;
                            mMinAmount = mIsInsuranceLifeMandatoryMinAmount;
                        }

                        mInsurancePolicy.Id = _lmsClientInsurance.PutPolicy(new PutPolicyRequest
                        {
                            RequestHeaders = LMSRequestHeadersGetI(),
                            Creator = mUserAndDeptIdI,
                            Policy = mInsurancePolicy,
                            Insurances = new InsurancesList { new LAPI.Core.Insurance()
                                {
                                    AccrualSchedule = mInsuranceAccrualSchedule,
                                    PaymentSchedule = new InsurancePaymentSchedule(),
                                    Rate = mRate,

                                    CalculationType = mCalculationType,
                                    Amount = mFixedAmount,
                                    MinAmount = mMinAmount,

                                    TypeId = mLifeInsuranceTypeID,
                                    State = InsuranceState.New,
                                    Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                                    LoanId = _ApplicationId,
                                    ExtraId = null,
                                    Attributes = mInsuranceAttributes,
                                    ProductPropertyId = _InsuranceProducts.PRODUCT_PROPERTY_ID,
                                    ClientId = _PutApplicationRequest.Application.BorrowerId
                                }}
                        }).PolicyId;
                    }
                    catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
                    {
                        this.LogFaunt(fault, mUserID, "PutPolicy");
                    }
                    catch (Exception Ex)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutPolicy: " + Ex.Message);
                    }
                }
                catch (Exception Ex)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "insuranceRepository.ApplyChange: " + Ex.Message);
                }

            }
            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "life insurance done");
            #endregion life insurance

            #region Payment insurance

            byte? mPaymentInsuranceTypeID = null;
            try
            {
                mPaymentInsuranceTypeID = Convert.ToByte(mLMSInsuranceTypeList.First(insu => insu.SYS_CODE == "payment").INSURANCE_TYPE_ID);
            }
            catch { }

            if (mPaymentInsuranceTypeID != null /*&& _LoanApplicationFull.Record.GRACE_PERIOD == 0*/)
            {
                try
                {
                    int mInsuranceCompanyID = _LoanApplicationFull.ClientInfo.INSURANCE_COMPANY_ID;
                    _InsuranceProducts = null;

                    _InsuranceProducts = Array.FindLast(_InsuranceProductsArray, s => s.IS_ACTIVE && s.COMPANY_ID == mInsuranceCompanyID && s.TYPE_ID == mPaymentInsuranceTypeID && s.LOAN_PRODUCT_ID.Contains(_LoanApplicationFull.Record.BASE_PRODUCT_ID.ToString()) && s.PRODUCT_PROPERTY_ID == _LoanApplicationFull.ClientInfo.PAYMENT_INSURANCE_GUID);
                    if (_InsuranceProducts != null)
                    {
                        decimal? mRate = _InsuranceProducts.MAX_RATE_CLIENT;
                        decimal? mMinAmount = _InsuranceProducts.MIN_AMOUNT;

                        var mInsurancePolicy = new LAPI.Core.Policy
                        {
                            CompanyId = _InsuranceProducts.COMPANY_ID,
                            Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                            State = PolicyState.New,
                            ProductId = (int)_InsuranceProducts.INSURANCE_PRODUCT_ID,
                            LoanId = _ApplicationId,

                        };
                        try
                        {
                            LAPI.Core.Attributes mInsuranceAttributes = new LAPI.Core.Attributes();
                            mInsuranceAttributes.Add("PRODUCT_SALES_MANAGER", mUserAndDeptIdI.UserIdentification.Id.ToString());

                            mInsurancePolicy.Id = _lmsClientInsurance.PutPolicy(new PutPolicyRequest
                            {
                                RequestHeaders = LMSRequestHeadersGetI(),
                                Creator = mUserAndDeptIdI,
                                Policy = mInsurancePolicy,
                                Insurances = new InsurancesList { new LAPI.Core.Insurance()
                                {
                                    AccrualSchedule = new InsuranceAccrualSchedule(),
                                    //Amount = _PutApplicationRequest.Application.Amount.Amount,
                                    PaymentSchedule = new InsurancePaymentSchedule(),
                                    Rate = mRate,
                                    MinAmount = mMinAmount,
                                    CalculationType = LAPI.Core.InsuranceCalculationType.ScheduledRepayment,
                                    TypeId = mPaymentInsuranceTypeID,
                                    State = InsuranceState.New,
                                    Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                                    LoanId = _ApplicationId,
                                    ExtraId = null,
                                    Attributes = mInsuranceAttributes,
                                    ProductPropertyId = _InsuranceProducts.PRODUCT_PROPERTY_ID,
                                    ClientId = _PutApplicationRequest.Application.BorrowerId
                                }}
                            }).PolicyId;
                        }
                        catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
                        {
                            this.LogFaunt(fault, mUserID, "PutPolicy");
                        }
                        catch (Exception Ex)
                        {
                            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutPolicy: " + Ex.Message);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "insuranceRepository.ApplyChange: " + Ex.Message);
                }
            }

            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "Payment insurance done");
            #endregion Payment insurance

            try
            {
                string special_params_1 = _LoanApplicationFull.AdditionalAttribules.First(item => item.ATTRIBUTE_CODE == "SPECIAL_PARAMS_1").ATTRIBUTE_VALUE;
                if (special_params_1.Length > 0)
                {
                    AdditionalAttribute mAdditionalAttribute = _AdditionalAttributeList.First(item => item.ATTRIBUTE_CODE == "SPECIAL_PARAMS_1");
                    this.LMSAddReportParameters(_ApplicationId, mAdditionalAttribute.TYPE, "SPECIAL_PARAMS_1", special_params_1);
                }
            }
            catch
            {
            }


            #endregion after Application Add

            #region =Fill collateralFullRequest ==

            #region =Main parameters=

            List<Collateral> mCollateralList = new List<Collateral>();


            string mCollateralType = "";
            string mDescription = "";
            string mCadasreCode = "";

            string mCCY = _LoanApplicationFull.Record.LOAN_CURRENCY;

            decimal mAgreementAmount = 0;
            decimal mLinkedAmount = 0;
            string mLinkedCCY = "";
            decimal mMarketValue = 0;
            decimal mDiscount = 0;
            int mOwnerClientId = 0;
            decimal mLoanAmount = Convert.ToDecimal(_LoanApplicationFull.Record.LOAN_AMOUNT_ISSUED);
            decimal mInterestRate = Convert.ToDecimal(_LoanApplicationFull.Record.INTEREST_RATE);
            decimal mDays = 0;

            DateTime mEndDate = mStartDate.AddMonths(_LoanApplicationFull.Record.LOAN_DAYS_REQUESTED);
            TimeSpan mTimeSpan = (TimeSpan)(mEndDate - mStartDate);
            mDays = Convert.ToDecimal(mTimeSpan.TotalDays);

            if (mSpecialLoanTypes.IsCashCover)// CashCover-ის დროს მითითებული გვაქვს დღეები და არა თვეები
            {
                mTimeSpan = (TimeSpan)(_PutApplicationRequest.Application.Term.End - _PutApplicationRequest.Application.Term.Start);
                mDays = Convert.ToDecimal(mTimeSpan.TotalDays);
            }

            //collateralFullRequest.UserId = mUserID;


            if (mSpecialLoanTypes.IsAuto || mSpecialLoanTypes.IsClearence)
            {
                mAgreementAmount = Convert.ToDecimal(_LoanApplicationFull.Auto.PRICE_AMOUNT);
                mMarketValue = Convert.ToDecimal(_LoanApplicationFull.Auto.PRICE_MARKET_AMOUNT);
                mDiscount = Convert.ToDecimal(_LoanApplicationFull.Auto.DISCOUNT_RATE);
                mDescription = _LoanApplicationFull.Auto.MANUFACTURER + " " + _LoanApplicationFull.Auto.MODEL;
                mOwnerClientId = _LoanApplicationFull.ClientInfo.CLIENT_NO;
            }

            #endregion =Main parameters=

            //Collateral ციკლი
            for (int i = 0; i < mColateralCount; i++)
            {

                if (_LoanApplicationFull.GuarantorList.Count > 0)
                {
                    mCollateralType = "0305";
                }
                if (mCurentCashCover.CashCoverDepositList != null && mCurentCashCover.CashCoverDepositList.Count > 0)
                {
                    mCollateralType = "05";
                }
                if (mSpecialLoanTypes.IsAutoNew || mSpecialLoanTypes.IsClearenceDealer)
                {
                    mCollateralType = "0901";
                }
                if (mSpecialLoanTypes.IsAutoUsed || mSpecialLoanTypes.IsClearenceCustomer)
                {
                    mCollateralType = "0902";
                }
                if (mSpecialLoanTypes.IsRealEstate)
                {
                    mCollateralType = _LoanApplicationFull.RealEstate.CollateralList[i].REAL_ESTATE_TYPE_ID; ;
                }


                /*MFile Attribute*/
                string mApplicationMFileAttribute_collateral = "";

                if (mCollateralType == "0305")
                {
                    mApplicationMFileAttribute_collateral = ApplicationMFileAttributeGet(_LoanApplicationFull.Record.APPLICATION_ID, 2);
                }
                else if (mCollateralType == "0901" || (mCollateralType == "05"))
                {
                    mApplicationMFileAttribute_collateral = ApplicationMFileAttributeGet(_LoanApplicationFull.Record.APPLICATION_ID, 4);
                }
                else
                {
                    mApplicationMFileAttribute_collateral = ApplicationMFileAttributeGet(_LoanApplicationFull.Record.APPLICATION_ID, 5);
                }
                /*END MFile Attribute*/

                if (mSpecialLoanTypes.IsAuto || mSpecialLoanTypes.IsClearence)
                {
                    mAgreementAmount = mMarketValue;
                }
                else if (mSpecialLoanTypes.IsCashCover)
                {
                    if (mCurentCashCover.CashCoverDepositList[i].IS_DEPO_USED)
                    {
                        mOwnerClientId = mCurentCashCover.CashCoverDepositList[i].DEPO_CLIENT_ID;
                        mDiscount = 0;
                        mDescription = mCurentCashCover.CashCoverDepositList[i].DEPO_CLIENT_NAME + "(დეპოზიტი)";
                        mCCY = mCurentCashCover.CashCoverDepositList[i].DEPO_CCY;
                        mAgreementAmount = mCurentCashCover.CashCoverDepositList[i].DEPO_AMOUNT;
                        mLinkedAmount = mCurentCashCover.CashCoverDepositList[i].LINKED_AMOUNT;
                        mLinkedCCY = mCurentCashCover.CashCoverDepositList[i].DEPO_CCY;

                        mMarketValue = mAgreementAmount;

                        if (_PutApplicationRequest.Application.BorrowerId != mOwnerClientId)
                        {
                            decimal mAgreementAmountGuarant = mLoanAmount + mLoanAmount * mInterestRate / 100.00m / 365.00m * mDays;
                            this.CollateralAddOwnerAsGuarantor(_ApplicationId, _LoanApplicationFull.Record.BRANCH_ID, Math.Round(mAgreementAmountGuarant, 2), mCurentCashCover.CashCoverDepositList[i].DEPO_CLIENT_NAME, mOwnerClientId, _PutApplicationRequest.Application.Amount.Ccy, _LoanApplicationFull.Record.APLICATION_OWNER, mUserAndDeptId, mApplicationMFileAttribute_collateral);
                        }
                    }
                    else
                    {
                        mCollateralType = "";
                    }
                }
                else if (mSpecialLoanTypes.IsRealEstate)
                {
                    mCollateralType = _LoanApplicationFull.RealEstate.CollateralList[i].REAL_ESTATE_TYPE_ID;
                    mAgreementAmount = Convert.ToDecimal(_LoanApplicationFull.RealEstate.CollateralList[i].LIQUIDATION_AMOUNT);
                    mMarketValue = Convert.ToDecimal(_LoanApplicationFull.RealEstate.CollateralList[i].MARKET_AMOUNT);
                    mDiscount = Convert.ToDecimal(_LoanApplicationFull.RealEstate.CollateralList[i].LIQUIDATION_AMOUNT_USD);
                    mCadasreCode = _LoanApplicationFull.RealEstate.CollateralList[i].CADASTRE_CODE;
                    mDescription = _LoanApplicationFull.RealEstate.CollateralList[i].REAL_ESTATE_TYPE_NAME;

                    mLinkedAmount = mAgreementAmount;
                    mLinkedCCY = mCCY;

                    try
                    {
                        mOwnerClientId = _LoanApplicationFull.RealEstate.CollateralList[i].OWNER_CLIENT_ID;
                    }
                    catch
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "collateral OWNER_CLIENT_ID not found");
                        mOwnerClientId = 0;
                    }

                    if (mOwnerClientId == 0)
                    {
                        mOwnerClientId = _LoanApplicationFull.ClientInfo.CLIENT_NO;
                    }
                }
                else
                {
                    mAgreementAmount = mLoanAmount + mLoanAmount * mInterestRate / 100.00m / 365.00m * mDays;
                    mMarketValue = mAgreementAmount;
                    mDiscount = 0;
                    mLinkedAmount = mAgreementAmount;
                    mLinkedCCY = mCCY;
                }

                if (_LoanApplicationFull.GuarantorList.Where(item => item.GUARANTOR_TYPE_ID == 1 || item.GUARANTOR_TYPE_ID == 3).ToList().Count > 0)
                {
                    ApplicationGuarantor mApplicationGuarantor = _LoanApplicationFull.GuarantorList[i];
                    mDescription = mApplicationGuarantor.GUARANTOR_SURNAME + " " + mApplicationGuarantor.GUARANTOR_NAME + " " + mApplicationGuarantor.GUARANTOR_ADDRESS;
                    mOwnerClientId = mApplicationGuarantor.GUARANTOR_CLIENT_NO;
                }

                if (mOwnerClientId == 0)
                {
                    mOwnerClientId = _LoanApplicationFull.ClientInfo.CLIENT_NO;
                }

                string mVINCode = "";
                string mCadastreCode = "";
                int? mDepositIdentifier = null;

                ApplicationRealEstateCollateral mApplicationRealEstateCollateral = null;

                if (mCollateralType == "0901" || mCollateralType == "0902")
                {
                    mVINCode = _LoanApplicationFull.Auto.VIN_CODE;
                }

                if (mCollateralType == "05")
                {
                    mDepositIdentifier = (int)mCurentCashCover.CashCoverDepositList[i].DEPO_ID;
                }

                if (mSpecialLoanTypes.IsRealEstate)
                {
                    mCadastreCode = _LoanApplicationFull.RealEstate.CollateralList[i].CADASTRE_CODE;
                }

                /*არსებოსბს თუ არა*/
                Collateral mCollateral = null;
                List<Collateral> mCL = null;
                try
                {
                    List<Collateral> mCollateralListExisting = this.LmsGetCollateral((int)mOwnerClientId, mCollateralType, mVINCode, mCadastreCode, mDepositIdentifier);
                    if (mCollateralListExisting.Count > 0)
                        mCollateral = mCollateralListExisting.First();

                    if (mCollateral.State == CollateralState.Closed)
                    {
                        mCollateral.State = CollateralState.Current;
                        PostCollateralResponse mPostCollateralResponse = _LAPI.PostCollateral(new PostCollateralRequest { Collateral = mCollateral, Creator = mUserAndDeptId, RequestHeaders = this.LMSRequestHeadersGet() });
                    }
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }

                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "collateral existing check done");

                AmountAndCurrency mCollateralAmount = new AmountAndCurrency { Amount = Math.Round((decimal)mAgreementAmount, 2), Ccy = mCCY };
                AmountAndCurrency mCollateralLinkAmount = new AmountAndCurrency { Amount = Math.Round((decimal)mLinkedAmount, 2), Ccy = mLinkedCCY };

                /*თუ არ არსებობს*/
                if (mCollateral == null && mCollateralType != "")
                {
                    mCollateral = new Collateral();
                    PutCollateralRequest mPutCollateralRequest = new PutCollateralRequest();

                    if (mCollateralType != "0305")
                    {
                        mCollateral.Amount = mCollateralAmount;
                    }
                    mCollateral.BranchId = _LoanApplicationFull.Record.BRANCH_ID;
                    mCollateral.CollateralType = mCollateralType;
                    mCollateral.Comment = mDescription;
                    mCollateral.CoOwners = new List<CollateralCoOwner>();//TODO
                    mCollateral.OwnerId = mOwnerClientId;
                    mCollateral.State = CollateralState.Current;

                    mPutCollateralRequest.Collateral = mCollateral;
                    mPutCollateralRequest.RequestHeaders = this.LMSRequestHeadersGet();
                    mPutCollateralRequest.Creator = mUserAndDeptId;

                    if (mCollateralType == "0901" || mCollateralType == "0902")
                    {
                        mCollateral.MarketValue = mMarketValue;
                        mCollateral.Discount = Math.Round(mDiscount, 2);
                    }

                    var mAttributesAttributeList = new LAPI.Core.Attributes();

                    if (mCollateralType == "0901" || mCollateralType == "0902")
                    {
                        mAttributesAttributeList.Add("$AUTO_COLOR", "-");
                        mAttributesAttributeList.Add("$AUTO_MODEL", _LoanApplicationFull.Auto.MANUFACTURER + " / " + _LoanApplicationFull.Auto.MODEL);
                        mAttributesAttributeList.Add("$AUTO_VEHICLE_TYPE", "მსუბუქი");
                        mAttributesAttributeList.Add("$AUTO_VIN_CODE", _LoanApplicationFull.Auto.VIN_CODE);


                        mAttributesAttributeList.Add("$AUTO_REG_NO", _LoanApplicationFull.Auto.VIN_CODE);
                        mAttributesAttributeList.Add("$AUTO_TECH_PASSPORT_ID", "1");

                        mAttributesAttributeList.Add("$AUTO_PRODUCTION_YEAR", _LoanApplicationFull.Auto.RELEASE_DATE.Year.ToString());
                        mAttributesAttributeList.Add("$AUTO_REG_ISSUE_DATE", DateTime.Now.Date.ToString("s"));

                        if (mSpecialLoanTypes.IsAuto || mSpecialLoanTypes.IsClearence)
                        {
                            mAttributesAttributeList.Add("COST", _LoanApplicationFull.Auto.PRICE_MARKET_AMOUNT.ToString());
                            mAttributesAttributeList.Add("DOC_NO", "1111");
                        }
                    }

                    if (mCollateralType == "05")
                    {
                        mAttributesAttributeList.Add("$DEPOSIT_ID", mCurentCashCover.CashCoverDepositList[i].DEPO_ID.ToString());//აქ შენახულია DEPO_ID//ანგარიშის ACC_ID ბლოკირებული ანგარიშის შემთხვეაში, ხოლო DEPO_ID დეპოზიტის შემთხვევაში
                    }

                    if (mSpecialLoanTypes.IsRealEstate)
                    {
                        mCollateral.MarketValue = mMarketValue;
                        mCollateral.Discount = mDiscount;
                        mCollateral.OwnerId = mOwnerClientId;


                        mApplicationRealEstateCollateral = _LoanApplicationFull.RealEstate.CollateralList[i];

                        mAttributesAttributeList.Add("$HYPOTEC_CADASTRE_CODE", mApplicationRealEstateCollateral.CADASTRE_CODE);
                        mAttributesAttributeList.Add("$HYPOTEC_AREA", mApplicationRealEstateCollateral.REAL_ESTATE_AREA.ToString());
                        mAttributesAttributeList.Add("$ADDRESS", mApplicationRealEstateCollateral.REAL_ESTATE_ADDRESS);
                        mAttributesAttributeList.Add("$DESCRIP", mApplicationRealEstateCollateral.REAL_ESTATE_TYPE_NAME);

                        mAttributesAttributeList.Add("$HYPOTEC_AGR_DATE", mApplicationRealEstateCollateral.REGISTRATION_DATE.ToString("s"));
                        mAttributesAttributeList.Add("$HYPOTEC_AGR_REG_NO", mApplicationRealEstateCollateral.REGISTRATION_NUMBER);
                        mAttributesAttributeList.Add("$HYPOTEC_OWNING_PART_OF", mApplicationRealEstateCollateral.SHARE_COUNT.ToString());

                        mAttributesAttributeList.Add("$VALUE_DATE", mApplicationRealEstateCollateral.VALUATION_DATE.ToString("s"));
                        mAttributesAttributeList.Add("VALUE_DATE", mApplicationRealEstateCollateral.VALUATION_DATE.ToString("s"));

                        mAttributesAttributeList.Add("$HYPOTEC_PREP_DATE", mApplicationRealEstateCollateral.EXTRACT_DATE.ToString("s"));
                        mAttributesAttributeList.Add("$HYPOTEC_APPRAISER", mApplicationRealEstateCollateral.APPRAISER_NAME.ToString());
                        mAttributesAttributeList.Add("$HYPOTEC_PRIMARY_APPRAISAL_LIQUIDATION_AMOUNT", mApplicationRealEstateCollateral.LIQUIDATION_AMOUNT_USD.ToString());
                        mAttributesAttributeList.Add("$HYPOTEC_PRIMARY_APPRAISAL_LIQUIDATION_CCY", "USD");
                        mAttributesAttributeList.Add("$HYPOTEC_PRIMARY_APPRAISAL_MARKET_AMOUNT", mApplicationRealEstateCollateral.MARKET_AMOUNT.ToString());
                        mAttributesAttributeList.Add("$HYPOTEC_PRIMARY_APPRAISAL_MARKET_CCY", "USD");
                        mAttributesAttributeList.Add("$HYPOTEC_", mApplicationRealEstateCollateral.CITY_NAME);
                        mAttributesAttributeList.Add("$HYPOTEC_FLOOR", mApplicationRealEstateCollateral.FLOOR_LEVEL.ToString());
                        mAttributesAttributeList.Add("$HYPOTEC_FLAT", mApplicationRealEstateCollateral.APARTMENT_NUMBER.ToString());
                        mAttributesAttributeList.Add("COST_REALE STATE", _LoanApplicationFull.RealEstate.Main.PRIMARY_AMOUNT.ToString());
                        if (mCollateral.CollateralType == "01" || mCollateral.CollateralType == "0102")
                        {
                            mAttributesAttributeList.Add("$HYPOTEC_DWELLING_AREA", mApplicationRealEstateCollateral.ACTIVE_SPACE.ToString());
                        }
                    }
                    mCollateral.Attributes = mAttributesAttributeList;

                    /*ახალი უზრუნველყოფის რეგისტრაცია*/
                    PutCollateralResponse mPutCollateralResponse = this.PutCollateral(mPutCollateralRequest, mUserAndDeptId);
                    try
                    {
                        mCollateral.CollateralId = mPutCollateralResponse.CollateralId;
                    }
                    catch (Exception ex)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, ex.Message);
                    }

                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "mPutCollateral done");
                }

                /*აქ უკვე არსებობს, ვამატებთ კავშირებს*/
                int? mInsuranceExtraId = null;
                if (mCollateralType != "" && mCollateral.CollateralId != null)
                {
                    /*Attributes*/
                    Attributes mLinkAttributes = null;

                    if (mApplicationMFileAttribute_collateral != "")
                    {
                        mLinkAttributes = new Attributes();
                        mLinkAttributes.Add("Collateral M-file number", mApplicationMFileAttribute_collateral);
                    }

                    if (_LoanApplicationFull.RealEstate.CollateralList.Count > 0)
                    {
                        if (_LoanApplicationFull.RealEstate.CollateralList[i].REAL_ESTATE_OWNER_TYPE_ID == 7)
                        {
                            if (mLinkAttributes == null)
                            {
                                mLinkAttributes = new Attributes();
                            }
                            mLinkAttributes.Add("Third party-owned immovable property", "კი");
                        }
                    }

                    PutCollateralLinkResponse mPutCollateralLinkResponse = this.PutCollateralLink(_ApplicationId, _PutApplicationRequest.Application.CreditLineId, mCollateral, mUserAndDeptId, mCollateralLinkAmount, mLinkAttributes);
                    try
                    {
                        mCashCoverDepoLoanMapODBList.Add(new CashCoverDepoLoanMap
                        {
                            APPLICATION_ID = _LoanApplicationFull.Record.APPLICATION_ID,
                            LOAN_ID = _ApplicationId,
                            DEPO_ID = mCurentCashCover.CashCoverDepositList[i].DEPO_ID,
                            DEPO_CCY = mCurentCashCover.CashCoverDepositList[i].DEPO_CCY,
                            DEPO_OWNER_ID = mCurentCashCover.CashCoverDepositList[i].DEPO_CLIENT_ID,
                            LINKED_AMOUNT = mCurentCashCover.CashCoverDepositList[i].LINKED_AMOUNT
                        });
                    }
                    catch { }

                    try
                    {
                        if (mPutCollateralLinkResponse != null)
                        {
                            mInsuranceExtraId = mPutCollateralLinkResponse.CollateralLinkId;
                        }
                    }
                    catch { }
                }

                if (mCollateralType != "" && mInsuranceExtraId == null)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "mPutCollateralLink error");
                    mLMSAnswer.Error = -10;
                    mLMSAnswer.ErrorMessage = "mPutCollateralLink-ის შეცდომა";
                    return mLMSAnswer;
                }
                else
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "mPutCollateralLink done");
                }

                #region Collateral Insurance

                #region auto insurance
                Byte? mAutoInsuranceTypeID = Convert.ToByte(mLMSInsuranceTypeList.First(ins => ins.SYS_CODE == "vehicle").INSURANCE_TYPE_ID);
                Byte? mAutoInsurancePaTypeID = null;
                Byte? mAutoInsuranceTplTypeID = null;

                if ((mSpecialLoanTypes.IsAuto && !mSpecialLoanTypes.IsToyotaAutoNew) || mSpecialLoanTypes.IsClearence)
                {
                    //if (_LoanApplicationFull.Record.LOAN_CURRENCY == "USD")
                    //{
                    try
                    {
                        int mInsuranceCompanyID = (_LoanApplicationFull.Auto.INSURANCE_COMPANY_ID == 0) ? 1 : _LoanApplicationFull.Auto.INSURANCE_COMPANY_ID;

                        _InsuranceTariff = Array.FindLast(_InsuranceTariffArray, s => s.INSURANCE_COMPANY_ID == mInsuranceCompanyID && s.INSURANCE_RATE_ID == _LoanApplicationFull.Auto.INSURANCE_RATE_ID);
                        string mInsuranceTariffPropertyId = _InsuranceTariff.PRODUCT_PROPERTY_GUID;

                        _InsuranceDriverPassenger = Array.FindLast(_InsuranceDriverPassengerArray, s => s.INSURANCE_COMPANY_ID == mInsuranceCompanyID && s.DRIVER_PASSENGER_INSURANCE_ID == _LoanApplicationFull.Auto.DRIVER_PASSENGER_INSURANCE_ID);
                        string mInsuranceDriverPassengerPropertyId = (_InsuranceDriverPassenger == null) ? "" : _InsuranceDriverPassenger.PRODUCT_PROPERTY_GUID;

                        _InsuranceThirdParty = Array.FindLast(_InsuranceThirdPartyArray, s => s.THIRD_PARTY_INSURANCE_ID == _LoanApplicationFull.Auto.THIRD_PARTY_INSURANCE_ID);
                        string mInsuranceThirdPartyPropertyId = (_InsuranceThirdParty == null) ? "" : _InsuranceThirdParty.PRODUCT_PROPERTY_GUID;

                        _InsuranceProducts = Array.FindLast(_InsuranceProductsArray, s => s.IS_ACTIVE && s.COMPANY_ID == mInsuranceCompanyID && s.TYPE_ID == mAutoInsuranceTypeID && s.PRODUCT_PROPERTY_ID == mInsuranceTariffPropertyId);
                        string mProductPropertyId = _InsuranceProducts.PRODUCT_PROPERTY_ID;

                        decimal mBankRate = Convert.ToDecimal(_LoanApplicationFull.Auto.INSURANCE_RATE);
                        decimal mInsuranceAmount = Convert.ToDecimal(_LoanApplicationFull.Auto.PRICE_MARKET_AMOUNT);

                        try
                        {
                            mAutoInsurancePaTypeID = Convert.ToByte(_InsuranceProductsArray.First(prod => prod.PRODUCT_PROPERTY_ID == mInsuranceDriverPassengerPropertyId).TYPE_ID);
                        }
                        catch { }
                        try
                        {
                            mAutoInsuranceTplTypeID = Convert.ToByte(_InsuranceProductsArray.First(prod => prod.PRODUCT_PROPERTY_ID == mInsuranceThirdPartyPropertyId).TYPE_ID);
                        }
                        catch { }

                        var mInsurancePolicy = new LAPI.Core.Policy
                        {
                            CompanyId = _InsuranceProducts.COMPANY_ID,
                            Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                            State = PolicyState.New, /*new*/
                            ProductId = (int)_InsuranceProducts.INSURANCE_PRODUCT_ID,
                            LoanId = _ApplicationId,

                        };

                        LAPI.Core.Attributes mInsuranceAttributes = new LAPI.Core.Attributes();
                        mInsuranceAttributes.Add("PRODUCT_SALES_MANAGER", mUserAndDeptIdI.UserIdentification.Id.ToString());

                        InsurancesList mInsurancesList = new InsurancesList();
                        mInsurancesList.Add(new LAPI.Core.Insurance()
                        {
                            AccrualSchedule = new InsuranceAccrualSchedule(),
                            Amount = (decimal)mInsuranceAmount,
                            PaymentSchedule = new InsurancePaymentSchedule(),
                            Rate = (decimal)mBankRate,
                            CalculationType = InsuranceCalculationType.MarketValue,
                            TypeId = mAutoInsuranceTypeID,
                            State = InsuranceState.New,
                            Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                            LoanId = _ApplicationId,
                            ExtraId = mInsuranceExtraId,
                            Attributes = mInsuranceAttributes,
                            ProductPropertyId = mInsuranceTariffPropertyId,
                            ClientId = _PutApplicationRequest.Application.BorrowerId
                        });

                        if (mInsuranceDriverPassengerPropertyId != "" && Convert.ToDecimal(_InsuranceDriverPassenger.DRIVER_PASSENGER_INSURANCE_VALUE) > 0)
                        {
                            mInsurancesList.Add(new LAPI.Core.Insurance()
                            {
                                AccrualSchedule = new InsuranceAccrualSchedule(),
                                Amount = Convert.ToDecimal(_InsuranceDriverPassenger.DRIVER_PASSENGER_INSURANCE_VALUE),
                                PaymentSchedule = new InsurancePaymentSchedule(),
                                Rate = null,
                                CalculationType = InsuranceCalculationType.FlatAnnual,
                                TypeId = mAutoInsurancePaTypeID, 
                                State = InsuranceState.New,
                                Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                                LoanId = _ApplicationId,
                                ExtraId = mInsuranceExtraId,
                                Attributes = mInsuranceAttributes,
                                ProductPropertyId = mInsuranceDriverPassengerPropertyId,
                                ClientId = _PutApplicationRequest.Application.BorrowerId
                            });
                        }

                        if (mInsuranceThirdPartyPropertyId != "" && Convert.ToDecimal(_InsuranceThirdParty.THIRD_PARTY_INSURANCE_VALUE) > 0)
                        {
                            mInsurancesList.Add(new LAPI.Core.Insurance()
                            {
                                AccrualSchedule = new InsuranceAccrualSchedule(),
                                Amount = Convert.ToDecimal(_InsuranceThirdParty.THIRD_PARTY_INSURANCE_VALUE),
                                PaymentSchedule = new InsurancePaymentSchedule(),
                                Rate = null,
                                CalculationType = InsuranceCalculationType.FlatAnnual,
                                TypeId = mAutoInsuranceTplTypeID,
                                State = InsuranceState.New,
                                Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                                LoanId = _ApplicationId,
                                ExtraId = mInsuranceExtraId,
                                Attributes = mInsuranceAttributes,
                                ProductPropertyId = mInsuranceThirdPartyPropertyId,
                                ClientId = _PutApplicationRequest.Application.BorrowerId
                            });
                        }

                        try
                        {
                            mInsurancePolicy.Id = _lmsClientInsurance.PutPolicy(new PutPolicyRequest { RequestHeaders = LMSRequestHeadersGetI(), Creator = mUserAndDeptIdI, Policy = mInsurancePolicy, Insurances = mInsurancesList }).PolicyId;
                        }
                        catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
                        {
                            this.LogFaunt(fault, mUserID, "PutPolicy");
                        }
                        catch (Exception Ex)
                        {
                            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutPolicy: " + Ex.Message);
                        }


                        //mInsuranceExtraData.Mpa.Limit = Convert.ToDecimal(_InsuranceDriverPassenger.DRIVER_PASSENGER_INSURANCE_LIMIT);
                        //mInsuranceExtraData.Mpa.Name = _InsuranceDriverPassenger.DRIVER_PASSENGER_INSURANCE_ATTRIBUTE_NAME;
                        //mInsuranceExtraData.Mpa.Premium = Convert.ToDecimal(_InsuranceDriverPassenger.DRIVER_PASSENGER_INSURANCE_VALUE);
                        //mInsuranceExtraData.Mpa.TypeKey = "insurance_vehicle_pa";

                        //mInsuranceExtraData.Mtpl.Limit = Convert.ToDecimal(_InsuranceThirdParty.THIRD_PARTY_INSURANCE_LIMIT);
                        //mInsuranceExtraData.Mtpl.Name = _InsuranceThirdParty.THIRD_PARTY_INSURANCE_ATTRIBUTE_NAME;
                        //mInsuranceExtraData.Mtpl.Premium = Convert.ToDecimal(_InsuranceThirdParty.THIRD_PARTY_INSURANCE_VALUE);
                        //mInsuranceExtraData.Mtpl.TypeKey = "insurance_vehicle_tpl";

                    }
                    catch (Exception Ex)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "insuranceRepository.ApplyChange: " + Ex.Message);
                    }

                    //}
                }
                #endregion auto

                #region RealEstate insurance
                Byte? mREInsuranceTypeID = Convert.ToByte(mLMSInsuranceTypeList.First(ins => ins.SYS_CODE == "property").INSURANCE_TYPE_ID);

                if (mSpecialLoanTypes.IsRealEstate && _LoanApplicationFull.RealEstate.Main.INSURANCE_REAL_ESTAE_YN)
                {
                    try
                    {
                        int mInsuranceCompanyID = (_LoanApplicationFull.RealEstate.Main.INSURANCE_COMPANY_ID == 0) ? 1 : _LoanApplicationFull.RealEstate.Main.INSURANCE_COMPANY_ID;
                        _InsuranceProducts = Array.FindLast(_InsuranceProductsArray, s => s.IS_ACTIVE && s.COMPANY_ID == mInsuranceCompanyID && s.TYPE_ID == mREInsuranceTypeID && s.LOAN_PRODUCT_ID.Contains(_LoanApplicationFull.Record.BASE_PRODUCT_ID.ToString()));

                        decimal? mRate = _InsuranceProducts.MAX_RATE_CLIENT / _LoanApplicationFull.RealEstate.CollateralList.Count;
                        decimal mBankRate = _InsuranceProducts.MAX_RATE_BANK;
                        decimal mInsuranceAmount = Math.Round(Convert.ToDecimal(_LoanApplicationFull.RealEstate.Main.MARKET_AMOUNT) * mBankRate / 100, 2);


                        var mInsurancePolicy = new LAPI.Core.Policy
                        {
                            CompanyId = _InsuranceProducts.COMPANY_ID,
                            Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                            State = PolicyState.New, /*new*/
                            ProductId = (int)_InsuranceProducts.INSURANCE_PRODUCT_ID,
                            LoanId = _ApplicationId,

                        };

                        ProductControlFlags mProductControlFlags = ProductControlFlags.Basic | ProductControlFlags.Conditions;


                        ListProductsRequest mListProductsRequest = new ListProductsRequest { RequestHeaders = LMSRequestHeadersGetI(), Query = new ListProductsQuery { CompanyId = mInsuranceCompanyID, IsAuthorized = true, ControlFlags = mProductControlFlags } };
                        ListProductsResponse mListProductsResponse = _lmsClientInsurance.ListProducts(mListProductsRequest);

                        ProductsList mProductsList = mListProductsResponse.Result;

                        int policyClientId = _PutApplicationRequest.Application.BorrowerId.Value;
                        int OWNER_CLIENT_ID = 0;
                        try
                        {

                            OWNER_CLIENT_ID = _LoanApplicationFull.RealEstate.CollateralList[0].OWNER_CLIENT_ID;

                            if (OWNER_CLIENT_ID > 0 && _PutApplicationRequest.Application.BorrowerId.Value != OWNER_CLIENT_ID)
                            {
                                policyClientId = OWNER_CLIENT_ID;
                            }
                        }
                        catch { }

                        try
                        {
                            LAPI.Core.Attributes mInsuranceAttributes = new LAPI.Core.Attributes();
                            mInsuranceAttributes.Add("PRODUCT_SALES_MANAGER", mUserAndDeptIdI.UserIdentification.Id.ToString());

                            mInsurancePolicy.Id = _lmsClientInsurance.PutPolicy(new PutPolicyRequest
                            {
                                RequestHeaders = LMSRequestHeadersGetI(),
                                Creator = mUserAndDeptIdI,
                                Policy = mInsurancePolicy,
                                Insurances = new InsurancesList {
                                new LAPI.Core.Insurance()
                                {
                                    AccrualSchedule = new InsuranceAccrualSchedule(),
                                    PaymentSchedule = new InsurancePaymentSchedule(),
                                    Rate = (decimal)mRate,
                                    CalculationType = InsuranceCalculationType.LoanBalance,
                                    TypeId = mREInsuranceTypeID,
                                    State = InsuranceState.New,
                                    Term = new LAPI.Core.Period { Start = _PutApplicationRequest.Application.Term.Start, End = _PutApplicationRequest.Application.Term.End },
                                    LoanId = _ApplicationId,
                                    ExtraId = mInsuranceExtraId,
                                    Attributes = mInsuranceAttributes,
                                    ProductPropertyId = _InsuranceProducts.PRODUCT_PROPERTY_ID,
                                    ClientId = policyClientId
                                }
                            }
                            }).PolicyId;
                        }
                        catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
                        {
                            this.LogFaunt(fault, mUserID, "PutPolicy");
                        }
                        catch (Exception Ex)
                        {
                            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutPolicy: " + Ex.Message);
                        }

                    }
                    catch (Exception Ex)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "insuranceRepository.ApplyChange: " + Ex.Message);
                    }

                }


                #endregion RealEstate

                #endregion Collateral Insurance

                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "collateral insurance done");
            }

            if (mCollateralType == "05")
            {
                /*ძველი ლინკების გადალინკვა*/
                this.LMSCashCoverRelink(mUserAndDeptId, mCurentCashCover);

                /*ოდბ გადასაბლოკი სიის მომზადება*/
                foreach (CashCoverDepoLoanMap mCashCoverDepoLoanMap in mCurentCashCover.CashCoverDepoLoanMapList)
                {
                    mCashCoverDepoLoanMapODBList.Add(mCashCoverDepoLoanMap);
                }
                List<CashCoverDepoLoanMap> mCashCoverDepoLoanMapODBReversedList = new List<CashCoverDepoLoanMap>();
                /*სიის გადაბრუნება*/
                for (int i = mCashCoverDepoLoanMapODBList.Count - 1; i >= 0; i--)
                {
                    mCashCoverDepoLoanMapODBReversedList.Add(mCashCoverDepoLoanMapODBList[i]);
                }
                /*ოდბ ძველი ბლოკების გადაბლოკვა*/
                this.ODBCashCoverRelink(mCashCoverDepoLoanMapODBReversedList);

                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "CashCover Collateral Relink done");
            }


            this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "collateralFullRequest done");



            #endregion =Fill collateralFullRequest ==

            #region create account

            if (_ApplicationId > 0 && !mSpecialLoanTypes.IsPOSOnline)
            {
                UTFODBService _odb = new UTFODBService();
                _odb.Url = _AppSets["ODBServive.URL"];
                _odb.Timeout = 300000;

                ODBAnswer _ODBAnswer = new ODBAnswer();

                if (mSpecialLoanTypes.IsPOS)
                {
                    try
                    {
                        _ODBAnswer = _odb.CreateAccountForLoans2(_ApplicationId, 102);
                    }
                    catch (Exception Ex)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, Ex.Message);
                    }
                }
                else if (!mSpecialLoanTypes.IsCard && !mSpecialLoanTypes.IsGarantee)
                {
                    try
                    {
                        _ODBAnswer = _odb.CreateAccountForLoans2(_ApplicationId, _LoanApplicationFull.Record.ACCOUNT_PRODUCT_NO);
                    }
                    catch (Exception Ex)
                    {
                        this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, Ex.Message);
                    }
                }
            }
            #endregion create account

            mLMSAnswer.LoanApplicationRecord = this.ApplicationRec(mApplicationID, mUserID);

            return mLMSAnswer;
        }

        #region CASH COVER RELINK
        private bool ODBCashCoverRelink(List<CashCoverDepoLoanMap> mCashCoverDepoLoanMapList)
        {
            bool mResult = true;

            if (mCashCoverDepoLoanMapList.Count > 0)
            {
                this.ODBCashCoverDelByApplicationId(mCashCoverDepoLoanMapList[0].DEPO_ID, mCashCoverDepoLoanMapList[0].APPLICATION_ID);
            }

            foreach (CashCoverDepoLoanMap mCashCoverDepoLoanMap in mCashCoverDepoLoanMapList)
            {
                try
                {
                    string mSQL = String.Format(@"EXEC [dbo].[pDEPO_LOCK_AMOUNT] @loan_id  = {0},@depo_id = {1},@link_amount = {2},@application_id = {3}"
                        , mCashCoverDepoLoanMap.LOAN_ID
                        , mCashCoverDepoLoanMap.DEPO_ID
                        , mCashCoverDepoLoanMap.LINKED_AMOUNT
                        , mCashCoverDepoLoanMap.APPLICATION_ID
                        );
                    DataSet mDs = _DGate.GetDataSet(mSQL);
                    if (mDs == null || mDs.Tables.Count == 0 || mDs.Tables[0].Rows.Count == 0 || Convert.ToInt32(mDs.Tables[0].Rows[0]["BLOCK_ID"]) == 0)
                    {
                        mResult = false;
                    }

                }
                catch
                {
                    mResult = false;
                }
            }
            return mResult;
        }

        private bool ODBCashCoverDelByApplicationId(int mDepoID, int mApplicationId)
        {
            bool mResult = true;
            try
            {
                string mSQL = String.Format(@"EXEC [dbo].[pDEPO_LOCK_DELETE] @depo_id = {0}, @application_id  = {1}", mDepoID, mApplicationId);
                DataSet mDs = _DGate.GetDataSet(mSQL);
                if (mDs == null || mDs.Tables.Count == 0 || mDs.Tables[0].Rows.Count == 0 || Convert.ToInt32(mDs.Tables[0].Rows[0]["BLOCK_ID"]) == 0)
                {
                    mResult = false;
                }

            }
            catch
            {
                mResult = false;
            }
            return mResult;
        }

        private bool LMSCashCoverRelink(UserAndDeptId mUserAndDeptId, CashCover mCurentCashCover)
        {
            bool mResult = true;


            foreach (CashCoverDepoLoanMap mCashCoverDepoLoanMap in mCurentCashCover.CashCoverDepoLoanMapList)
            {
                try
                {
                    Collateral mCollateral = this.LmsGetCollateral(mCashCoverDepoLoanMap.DEPO_OWNER_ID, "05", "", "", mCashCoverDepoLoanMap.DEPO_ID).First();
                    AmountAndCurrency mCollateralAmount = new AmountAndCurrency { Amount = mCashCoverDepoLoanMap.LINKED_AMOUNT, Ccy = mCashCoverDepoLoanMap.DEPO_CCY };
                    this.PostCollateralLink(mCashCoverDepoLoanMap.LOAN_ID, mCollateral, mUserAndDeptId, mCollateralAmount);
                }
                catch
                {
                    mResult = false;
                }


            }
            return mResult;
        }
        #endregion CASH COVER RELINK

        #endregion LMSAddApplication

        #endregion === alta LMS ===

        #region create instant card
        [WebMethod]
        public InstantCardAnswer InstantCardCreate(int application_id, string instant_card_virtual_account, string instant_card_no4, string holder_label, int acc_client_no, int user_id, string phone)
        {
            InstantCardAnswer mInstantCardAnswer = new InstantCardAnswer();
            string mSQL = String.Format(@"EXEC dbo.pBANK_INSTANT_CARD_CREATE @virtual_account = {0},@card_no4 = {1},@holder_label = {2}
                ,@acc_client_no = {3},@application_id = {4},@phone = {5},@user_id = {6}"
                , CheckNull(instant_card_virtual_account)
                , CheckNull(instant_card_no4)
                , CheckNull(holder_label)
                , acc_client_no
                , application_id
                , CheckNull(phone)
                , user_id
                );
            try
            {
                DataSet mDataSet = _DGate.GetDataSet(mSQL, 600, false);
                DataTable mDataTabel = mDataSet.Tables[0];
                mInstantCardAnswer.ERROR_CODE = Convert.ToInt32(mDataTabel.Rows[0]["ERROR_CODE"]);
                mInstantCardAnswer.ERROR_MESSAGE = mDataTabel.Rows[0]["ERROR_MESSAGE"].ToString();
                mInstantCardAnswer.ACCOUNT_IBAN = mDataTabel.Rows[0]["ACCOUNT_IBAN"].ToString();
            }
            catch (Exception ex)
            {
                ApplicationLogSet(application_id, 10, user_id, ex.Message);
                mInstantCardAnswer.ERROR_CODE = -99;
                mInstantCardAnswer.ERROR_MESSAGE = ex.Message;
                mInstantCardAnswer.ACCOUNT_IBAN = "";
            }
            return mInstantCardAnswer;
        }
        #endregion create instant card

        #region Agreement
        [WebMethod]
        public AgreementOverdraft GetAgreementOverdraft(int application_id)
        {
            string mSQL = String.Format("EXEC  dbo.pAGREEMENT_OVERDRAFT @APPLICATION_ID = {0}", application_id);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            AgreementOverdraft mClass = new AgreementOverdraft();
            mClass = (AgreementOverdraft)FillClass(mClass, mDataTable.Rows[0]);

            return mClass;
        }
        #endregion

        #region PutCollateral, Link Collcateral
        public PutCollateralResponse PutCollateral(PutCollateralRequest mPutCollateralRequest, LAPI.Core.UserAndDeptId mUserAndDeptId)
        {
            int mUserID = (int)mUserAndDeptId.UserIdentification.Id;
            PutCollateralResponse mPutCollateralResponse = null;
            try
            {
                mPutCollateralResponse = _LAPI.PutCollateral(mPutCollateralRequest);
            }
            catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
            {
                this.LogFaunt(fault, mUserID, "PutCollateral");
            }
            catch (Exception Ex)
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutCollateral: " + Ex.Message);
            }

            return mPutCollateralResponse;
        }

        public PutCollateralLinkResponse PutCollateralLink(int mApplicationId, int? mCreditLineId, Collateral mCollateral, LAPI.Core.UserAndDeptId mUserAndDeptId, AmountAndCurrency mCollateralAmount, Attributes mLinkAttributes)
        {

            PutCollateralLinkResponse mPutCollateralLinkResponse = null;
            AmountAndCurrency mLinkAmount = new AmountAndCurrency { Amount = Math.Round((decimal)mCollateralAmount.Amount, 2), Ccy = mCollateralAmount.Ccy };
            int? mCollateralLinkId = null;
            int mUserID = (int)mUserAndDeptId.UserIdentification.Id;

            /*search for links*/
            ListLinkedCollateralsResponse listLinkedCollateralsResponse = _LAPI.ListLinkedCollaterals(new ListLinkedCollateralsRequest
            {
                RequestHeaders = this.LMSRequestHeadersGet(),
                Query = new ListLinkedCollateralsQuery
                {
                    CollateralIds = new List<int> { (int)mCollateral.CollateralId },
                    ControlFlags = LinkedCollateralControlFlags.Basic
                }
            });

            if (listLinkedCollateralsResponse.Result != null && listLinkedCollateralsResponse.Result.Count > 0)
            {
                try
                {
                    mCollateralLinkId = (int)listLinkedCollateralsResponse.Result.ToList().First(x => (mCreditLineId != null && x.CreditLineId == mCreditLineId) || x.ApplicationId == mApplicationId).Id;
                    if (mCollateralLinkId != null)
                    {
                        mPutCollateralLinkResponse = new PutCollateralLinkResponse { CollateralLinkId = (int)mCollateralLinkId };
                        return mPutCollateralLinkResponse;
                    }
                }
                catch { }
            }

            /*if not found*/
            CollateralLink mCollateralLink = new CollateralLink
            {
                CollateralId = mCollateral.CollateralId,
                /* must be only one : CreditLineId and ApplicationId*/
                ApplicationId = (mCreditLineId == null) ? (int?)mApplicationId : null,
                CreditLineId = mCreditLineId,
                AgreementNo = mCollateral.AgreementNo,
                LinkAmount = (mCollateral.CollateralType == "0305" || mCollateral.CollateralType == "05") ? mLinkAmount : null,
                LinkDate = DateTime.Now.Date,
                State = CollateralLinkState.Current,
                CloseDate = null,
                Comment = ""
            };


            /*Attribute*/
            mCollateralLink.Attributes = mLinkAttributes;

            PutCollateralLinkRequest mPutCollateralLinkRequest = new PutCollateralLinkRequest
            {
                Collateral = mCollateralLink,
                RequestHeaders = this.LMSRequestHeadersGet(),
                Creator = mUserAndDeptId
            };

            /*serializer*/
            //XmlSerializer xsSubmit = new XmlSerializer(typeof(PutCollateralLinkRequest));
            //var subReq = mPutCollateralLinkRequest;
            //using (StringWriter sww = new StringWriter())
            //using (XmlWriter writer = XmlWriter.Create(sww))
            //{
            //    xsSubmit.Serialize(writer, subReq);
            //    var xml = sww.ToString(); // Your XML
            //}

            try
            {
                mPutCollateralLinkResponse = _LAPI.PutCollateralLink(mPutCollateralLinkRequest);
                this.MFileLinkAttributeSet(mPutCollateralLinkResponse.CollateralLinkId, mCollateralLink.Attributes);
            }
            catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
            {
                this.LogFaunt(fault, mUserID, "PutCollateralLink");
                try
                {
                    var xml = fault.Message;
                    xml += " CollateralId = " + mCollateral.CollateralId.ToString();
                    xml += " ApplicationId = " + mApplicationId.ToString();
                    if (mCreditLineId != null)
                    {
                        xml += " CreditLineId = " + mCreditLineId.ToString();
                    }
                    xml += " AgreementNo = " + ((String.IsNullOrEmpty(mCollateral.AgreementNo)) ? "" : mCollateral.AgreementNo);
                    xml += " LinkAmount = " + mLinkAmount.Amount.ToString();

                    if (String.IsNullOrEmpty(xml))
                    {
                        xml = fault.Message;
                    }

                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutCollateralLink LMS: " + xml.ToString());
                }
                catch (Exception ex)
                {
                    this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutCollateralLink LMS: " + ex.Message);
                }

            }
            catch (Exception Ex)
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PutCollateralLink: " + Ex.Message);
            }
            return mPutCollateralLinkResponse;
        }

        public PostCollateralLinkResponse PostCollateralLink(int mLoanId, Collateral mCollateral, LAPI.Core.UserAndDeptId mUserAndDeptId, AmountAndCurrency mCollateralAmount)
        {

            PostCollateralLinkResponse mPostCollateralLinkResponse = null;
            DeleteLinkCollateralResponse mDeleteLinkCollateralResponse = null;
            int? mCollateralLinkId = null;
            ulong? mCollateralLinkVersion = null;
            string mCollateralLinkAgreementNo = "";
            AmountAndCurrency mLinkAmount = new AmountAndCurrency { Amount = Math.Round((decimal)mCollateralAmount.Amount, 2), Ccy = mCollateralAmount.Ccy };

            /*search for links*/
            ListLinkedCollateralsResponse listLinkedCollateralsResponse = _LAPI.ListLinkedCollaterals(new ListLinkedCollateralsRequest
            {
                RequestHeaders = this.LMSRequestHeadersGet(),
                Query = new ListLinkedCollateralsQuery
                {
                    CollateralIds = new List<int> { (int)mCollateral.CollateralId },
                    ApplicationId = mLoanId,
                    ControlFlags = LinkedCollateralControlFlags.Basic
                }
            });


            /*collateral link params*/
            CollateralLink oldCollateralLink = new CollateralLink();
            if (listLinkedCollateralsResponse.Result != null && listLinkedCollateralsResponse.Result.Count > 0)
            {
                try
                {
                    oldCollateralLink = listLinkedCollateralsResponse.Result.ToList().First(x => x.ApplicationId == mLoanId);

                    mCollateralLinkId = (int)oldCollateralLink.Id;
                    mCollateralLinkVersion = oldCollateralLink.Version;
                    mCollateralLinkAgreementNo = oldCollateralLink.AgreementNo;
                }
                catch { }
            }

            int mUserID = (int)mUserAndDeptId.UserIdentification.Id;
            CollateralLink mCollateralLink = new CollateralLink
            {
                CollateralId = mCollateral.CollateralId,
                ApplicationId = mLoanId,
                AgreementNo = mCollateralLinkAgreementNo,
                Id = mCollateralLinkId,
                LinkAmount = mLinkAmount,
                LinkDate = DateTime.Now.Date,
                State = CollateralLinkState.Current,
                Version = mCollateralLinkVersion,
                Comment = "Relink",
                CloseDate = null
            };

            PostCollateralLinkRequest mPostCollateralLinkRequest = new PostCollateralLinkRequest
            {
                Collateral = mCollateralLink,
                RequestHeaders = this.LMSRequestHeadersGet(),
                Creator = mUserAndDeptId
            };

            try
            {
                if (mLinkAmount.Amount > 0)
                {
                    mPostCollateralLinkResponse = _LAPI.PostCollateralLink(mPostCollateralLinkRequest);
                }
                //else
                //{
                //    mDeleteLinkCollateralResponse = _LAPI.DeleteCollateralLink(new DeleteCollateralLinkRequest { LinkId = (int)mCollateralLinkId, RequestHeaders = this.LMSRequestHeadersGet(), Version = mCollateralLinkVersion, User = mUserAndDeptId });
                //}
            }
            catch (System.ServiceModel.FaultException<LAPI.Core.ApiFault> fault)
            {
                this.LogFaunt(fault, mUserID, "PostCollateralLink");
            }
            catch (Exception Ex)
            {
                this.ApplicationLogSet(_LoanApplicationFull.Record.APPLICATION_ID, 10, mUserID, "PostCollateralLink: " + Ex.Message);
            }
            return mPostCollateralLinkResponse;
        }

        #endregion PutCollateral, Link Collcateral

        #region === XIRR ===
        [WebMethod]
        public CreditCardXIRR GetCreditCardXIRR(int appID, bool is_bank)
        {
            string mSQL = String.Format("EXEC dbo.pXIRR_CREDIT_CARD @APPLICATION_ID = {0}, @IS_BANK = {1}", appID, is_bank);
            DataRow mDataRow = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];

            CreditCardXIRR mClass = new CreditCardXIRR();
            if (mDataRow != null)
            {
                mClass = (CreditCardXIRR)FillClass(mClass, mDataRow);
            }

            return mClass;
        }

        [WebMethod]
        public POSCardXIRR GetPOSCardXIRR(int appID)
        {
            string mSQL = String.Format("EXEC dbo.pXIRR_POS_CARD @APPLICATION_ID = {0}", appID);
            DataRow mDataRow = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];

            POSCardXIRR mClass = new POSCardXIRR();
            if (mDataRow != null)
            {
                mClass = (POSCardXIRR)FillClass(mClass, mDataRow);
            }

            return mClass;
        }

        [WebMethod]
        public POSCardXIRR GetPOSSagaXIRR(int appID)
        {
            string mSQL = String.Format("EXEC dbo.pXIRR_POS_SAGA @APPLICATION_ID = {0}", appID);
            DataRow mDataRow = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];

            POSCardXIRR mClass = new POSCardXIRR();
            if (mDataRow != null)
            {
                mClass = (POSCardXIRR)FillClass(mClass, mDataRow);
            }

            return mClass;
        }

        [WebMethod]
        public OnlineXIRR CalcOnlineXIRR(int product_id, decimal loan_amount, string ccy, int loan_months)
        {
            string mSQL = String.Format("EXEC dbo.pXIRR_ONLINE_APPLICATION @product_id = {0}, @loan_amount = {1}, @ccy = {2}, @loan_months = {3}", product_id, loan_amount, ccy, loan_months);
            DataRow mDataRow = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];

            OnlineXIRR mClass = new OnlineXIRR();
            if (mDataRow != null)
            {
                mClass = (OnlineXIRR)FillClass(mClass, mDataRow);
            }

            return mClass;
        }

        [WebMethod]
        public OnlineXIRR CalcPosOnlineXIRR(int installment_id, int product_id, int category_id, decimal loan_amount, decimal item_amount, int loan_months, int grace_period)
        {
            string mSQL = String.Format(@"EXEC dbo.pXIRR_ONLINE_POS
                      @installment_id = {0}, @product_id = {1}, @category_id = {2}
	                , @loan_amount = {3}, @item_amount = {4}, @loan_months = {5},  @grace_period = {6}"
                    , installment_id, product_id, category_id
                    , loan_amount, item_amount, loan_months, grace_period
                    );

            DataRow mDataRow = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];

            OnlineXIRR mClass = new OnlineXIRR();
            if (mDataRow != null)
            {
                mClass = (OnlineXIRR)FillClass(mClass, mDataRow);
            }

            return mClass;
        }
        #endregion

        #region === XIRR NBG ===
        [WebMethod]
        public List<XIRR_Result> XIRRCreditCard(int appID)
        {
            List<XIRR_Result> XIRR_ResultList = new List<XIRR_Result>();

            string mSQL = String.Format("EXEC dbo.pXIRR_NBG_CREDIT_CARD @APPLICATION_ID = {0}", appID);
            DataTable mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                XIRR_Result mClass = new XIRR_Result();
                if (mDataRow != null)
                {
                    mClass = (XIRR_Result)FillClass(mClass, mDataRow);
                }

                XIRR_ResultList.Add(mClass);
            }

            return XIRR_ResultList;
        }

        [WebMethod]
        public List<XIRR_Result> XIRROverdraft(int appID)
        {
            List<XIRR_Result> XIRR_ResultList = new List<XIRR_Result>();

            string mSQL = String.Format("EXEC dbo.pXIRR_NBG_OVERDRAFT @APPLICATION_ID = {0}", appID);
            DataTable mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                XIRR_Result mClass = new XIRR_Result();
                if (mDataRow != null)
                {
                    mClass = (XIRR_Result)FillClass(mClass, mDataRow);
                }

                XIRR_ResultList.Add(mClass);
            }

            return XIRR_ResultList;
        }

        [WebMethod]
        public List<XIRR_Result> XIRRPosCard(int appID)
        {
            List<XIRR_Result> XIRR_ResultList = new List<XIRR_Result>();

            string mSQL = String.Format("EXEC dbo.pXIRR_NBG_POS_CARD @APPLICATION_ID = {0}", appID);
            DataTable mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                XIRR_Result mClass = new XIRR_Result();
                if (mDataRow != null)
                {
                    mClass = (XIRR_Result)FillClass(mClass, mDataRow);
                }

                XIRR_ResultList.Add(mClass);
            }

            return XIRR_ResultList;
        }

        [WebMethod]
        public List<XIRR_Result> XIRRPosExpress(int appID, bool HaveInsurance)
        {
            List<XIRR_Result> XIRR_ResultList = new List<XIRR_Result>();

            string mSQL = String.Format("EXEC dbo.pXIRR_NBG_POS_EXPRESS @APPLICATION_ID = {0}, @HAVE_INSURANCE = {1}", appID, HaveInsurance);
            DataTable mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                XIRR_Result mClass = new XIRR_Result();
                if (mDataRow != null)
                {
                    mClass = (XIRR_Result)FillClass(mClass, mDataRow);
                }

                XIRR_ResultList.Add(mClass);
            }

            return XIRR_ResultList;
        }
        #endregion

        #region === LOAN CARD ===
        [WebMethod]
        public LoanCard GetLoanCard(int mLoanID)
        {
            string mSQL = "";
            LoanCard returnLoanCard = new LoanCard();

            // 1. სესხის ინფორმაცია
            LoanCardInfo mLoanInfo = new LoanCardInfo();
            mSQL = String.Format("EXEC dbo.pLOAN_CARD_INFO @loan_id = {0}", mLoanID);
            DataRow mRowInfo = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];
            if (mRowInfo != null)
            {
                mLoanInfo = (LoanCardInfo)FillClass(mLoanInfo, mRowInfo);
            }
            returnLoanCard.LoanInfo = mLoanInfo;

            // 2. სესხის გრაფიკი
            List<LoanCardSchedule> mLoanScheduleList = new List<LoanCardSchedule>();
            mSQL = String.Format("EXEC dbo.pLOAN_CARD_SCHEDULE @loan_id = {0}", mLoanID);
            DataTable mTableSchedule = _DGate.GetDataSet(mSQL).Tables[0];
            foreach (DataRow mRowSchedule in mTableSchedule.Rows)
            {
                if (mRowSchedule != null)
                {
                    LoanCardSchedule mLoanSchedule = new LoanCardSchedule();
                    mLoanSchedule = (LoanCardSchedule)FillClass(mLoanSchedule, mRowSchedule);
                    mLoanScheduleList.Add(mLoanSchedule);
                }
            }
            returnLoanCard.LoanScheduleList = mLoanScheduleList;

            // 3. სესხის ოპერაციები
            List<LoanCardPayment> mLoanCardPaymentList = new List<LoanCardPayment>();
            mSQL = String.Format("EXEC dbo.pLOAN_CARD_PAYMENTS @loan_id = {0}", mLoanID);
            DataTable mTablePayments = _DGate.GetDataSet(mSQL).Tables[0];
            foreach (DataRow mRowPayment in mTablePayments.Rows)
            {
                if (mRowPayment != null)
                {
                    LoanCardPayment mLoanCardPayment = new LoanCardPayment();
                    mLoanCardPayment = (LoanCardPayment)FillClass(mLoanCardPayment, mRowPayment);
                    mLoanCardPaymentList.Add(mLoanCardPayment);
                }
            }
            returnLoanCard.LoanPaymentList = mLoanCardPaymentList;

            // 4. დავალიანება
            LoanCardDebt mLoanDebt = new LoanCardDebt();
            mSQL = String.Format("EXEC dbo.pLOAN_CARD_DEBT_LMS @loan_id = {0}, @is_detail = 1", mLoanID);
            DataRow mRowDebt = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];
            if (mRowDebt != null)
            {
                mLoanDebt = (LoanCardDebt)FillClass(mLoanDebt, mRowDebt);
            }
            returnLoanCard.LoanDebt = mLoanDebt;

            return returnLoanCard;
        }

        [WebMethod]
        public LoanCardDebt GetLoanCardDebt(int mLoanID)
        {
            string mSQL = "";
            LoanCardDebt mLoanDebt = new LoanCardDebt();

            mSQL = String.Format("EXEC dbo.pLOAN_CARD_DEBT_LMS @loan_id = {0}, @is_detail = 1", mLoanID);
            DataRow mRowDebt = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];
            if (mRowDebt != null)
            {
                mLoanDebt = (LoanCardDebt)FillClass(mLoanDebt, mRowDebt);
            }

            return mLoanDebt;
        }

        #endregion

        #region === CLIENT INCOME ===
        [WebMethod]
        public List<ClientIncome> ClientIncomeList(int applicationId)
        {
            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_INCOME_LIST @APPLICATION_ID = {0}", applicationId);
            DataSet mDataSet = _DGate.GetDataSet(mSQL);

            List<ClientIncome> RetList = new List<ClientIncome>();

            if (mDataSet.Tables.Count > 0 && mDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow mDataRow in mDataSet.Tables[0].Rows)
                {
                    ClientIncome mClass = new ClientIncome();

                    mClass = (ClientIncome)FillClass(mClass, mDataRow);
                    RetList.Add(mClass);
                }
            }

            return RetList;
        }

        [WebMethod]
        public bool ClientIncomeSave(int mAPPLICATION_ID, List<ClientIncome> mClientIncomeList)
        {
            bool mRetValue = false;

            string mSQL = String.Format("EXEC dbo.pAPPLICATION_CLIENT_INCOME_DEL @APPLICATION_ID = {0}", mAPPLICATION_ID);

            try
            {
                DataSet ds;
                ds = _DGate.GetDataSet(mSQL);
                if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["ANSWER"]) != 0)
                {
                    mRetValue = true;
                }
            }
            catch { }

            if (mRetValue)
            {
                foreach (ClientIncome clientIncome in mClientIncomeList)
                {
                    mRetValue = this.ClientIncomeRecordSave(clientIncome);
                }
            }

            return mRetValue;
        }

        private bool ClientIncomeRecordSave(ClientIncome clientIncome)
        {
            bool mRetValue = false;

            string mSQL = String.Format(@"EXEC dbo.pAPPLICATION_CLIENT_INCOME_SET
	          @APPLICATION_ID = {0}, @INCOME_TYPE_ID = {1}, @COMMON_ORG_NAME = {2}, @COMMON_ORG_ACTIVITY_NAME = {3}
            , @COMMON_ORG_ADDRESS = {4}, @COMMON_ORG_PHONE = {5}, @COMMON_ORG_TAX_CODE = {6}, @SALARY_POSITION_ID = {7}
            , @SALARY_POSITION_DESCRIP = {8}, @SALARY_WORK_EXPERIENCE = {9}, @SALARY_HEAD_NAME = {10}, @SALARY_HEAD_POSITION = {11}
            , @SALARY_HEAD_PHONE = {12}, @RENT_REAL_ESTATE_TYPE_ID = {13}, @RENT_REAL_ESTATE_ADDRESS = {14}, @RENT_REAL_ESTATE_OWNER = {15}
            , @RENT_RELATION_ID = {16}, @RENT_LESSOR_NAME = {17}, @RENT_LESSOR_PHONE = {18}, @DIVIDEND_ORG_PROFIT = {19}
	        , @DIVIDEND_CUSTOMER_SHARE = {20}, @BUSINESS_ACTIVITY_DURATION = {21}, @BUSINESS_CLIENT_PERCENTAGE = {22}, @BUSINESS_EMPLOYEE_NUMBER = {23}
	        , @BUSINESS_INCOME_PER_DAY = {24}, @BUSINESS_INCOME_PER_MONTH = {25}, @PROF_PROFESSION_NAME = {26}, @PROF_CONTACT_PERSON_NAME1 = {27}
	        , @PROF_CONTACT_PERSON_PHONE1 = {28}, @PROF_CONTACT_PERSON_NAME2 = {29}, @PROF_CONTACT_PERSON_PHONE2 = {30}, @COMMON_INCOME_AMOUNT = {31}",
            clientIncome.APPLICATION_ID,
            clientIncome.INCOME_TYPE_ID,
            StringFunctions.SqlQuoted(clientIncome.COMMON_ORG_NAME, true),
            StringFunctions.SqlQuoted(clientIncome.COMMON_ORG_ACTIVITY_NAME, true),

            StringFunctions.SqlQuoted(clientIncome.COMMON_ORG_ADDRESS, true),
            StringFunctions.SqlQuoted(clientIncome.COMMON_ORG_PHONE, true),
            StringFunctions.SqlQuoted(clientIncome.COMMON_ORG_TAX_CODE, true),
            clientIncome.SALARY_POSITION_ID,

            StringFunctions.SqlQuoted(clientIncome.SALARY_POSITION_DESCRIP, true),
            clientIncome.SALARY_WORK_EXPERIENCE,
            StringFunctions.SqlQuoted(clientIncome.SALARY_HEAD_NAME, true),
            StringFunctions.SqlQuoted(clientIncome.SALARY_HEAD_POSITION, true),

            StringFunctions.SqlQuoted(clientIncome.SALARY_HEAD_PHONE, true),
            StringFunctions.SqlQuoted(clientIncome.RENT_REAL_ESTATE_TYPE_ID, true),
            StringFunctions.SqlQuoted(clientIncome.RENT_REAL_ESTATE_ADDRESS, true),
            StringFunctions.SqlQuoted(clientIncome.RENT_REAL_ESTATE_OWNER, true),

            clientIncome.RENT_RELATION_ID,
            StringFunctions.SqlQuoted(clientIncome.RENT_LESSOR_NAME, true),
            StringFunctions.SqlQuoted(clientIncome.RENT_LESSOR_PHONE, true),
            clientIncome.DIVIDEND_ORG_PROFIT,

            clientIncome.DIVIDEND_CUSTOMER_SHARE,
            clientIncome.BUSINESS_ACTIVITY_DURATION,
            clientIncome.BUSINESS_CLIENT_PERCENTAGE,
            clientIncome.BUSINESS_EMPLOYEE_NUMBER,

            clientIncome.BUSINESS_INCOME_PER_DAY,
            clientIncome.BUSINESS_INCOME_PER_MONTH,
            StringFunctions.SqlQuoted(clientIncome.PROF_PROFESSION_NAME, true),
            StringFunctions.SqlQuoted(clientIncome.PROF_CONTACT_PERSON_NAME1, true),

            StringFunctions.SqlQuoted(clientIncome.PROF_CONTACT_PERSON_PHONE1, true),
            StringFunctions.SqlQuoted(clientIncome.PROF_CONTACT_PERSON_NAME2, true),
            StringFunctions.SqlQuoted(clientIncome.PROF_CONTACT_PERSON_PHONE2, true),
            clientIncome.COMMON_INCOME_AMOUNT
            );

            DataSet mDataSet = _DGate.GetDataSet(mSQL);
            if (mDataSet != null && mDataSet.Tables != null && mDataSet.Tables[0].Rows.Count > 0 && Convert.ToInt32(mDataSet.Tables[0].Rows[0]["ANSWER"]) != 0)
            {
                mRetValue = true;
            }

            return mRetValue;
        }
        #endregion

        #region === Scoring ===
        [WebMethod]
        public FA_ScoringResult CalcScoringDet(FA_ScoringParam scorParam, FA_Application appParam, FA_Client client,
            List<FA_Guarantor> guarantorList, List<FA_CreditInfo> creditInfoList)
        {
            // === 1. ცვლადების განსაზრვრა ===
            FA_ScoringResult scoringResult = new FA_ScoringResult();

            List<FA_ScoringLog> ScoringLogList = new List<FA_ScoringLog>();
            decimal SCORING_LIMIT = 0;
            decimal XIRR = 0;

            FA_ScoringLog scoringLog = new FA_ScoringLog();

            FA_CreditInfo creditInfo;
            FA_Dictionary dictionary = new FA_Dictionary();
            FA_HelperCoef coef = new FA_HelperCoef();
            FA_HelperParam helper = new FA_HelperParam();

            int scoringTypeID = this.Helper_ProductByCode(appParam.PRODUCT_ID).SCORING_TYPE_ID;

            if (scoringTypeID == 0)
            {
                scoringResult.ScoringValue = 0;
                scoringResult.XIRRValue = 0;
                scoringResult.ScoringLogList = new List<FA_ScoringLog>();
                return scoringResult;
            }

            List<ScoringParamCoef> ScoringParamCoefList = this.ScoringParamCoefList();
            List<MapScoringTypeProperty> ScoringTypePropertyList = this.MapScoringTypePropertyList(scoringTypeID);

            FA_Guarantor coborrower = new FA_Guarantor();
            try
            {
                coborrower = guarantorList.First(x => x.GuarantorType == enumClientType.Coborrower);
            }
            catch
            {
                coborrower = null;
            }

            //თანამსესხებელი არ არსებობს
            if (scorParam.ScorCalcType == enumScoringCalcType.CoborrowerScoring && coborrower == null)
            {
                scoringResult.XIRRValue = 0;
                scoringResult.ScoringValue = 0;

                return scoringResult;
            }

            if (scorParam.ScorCalcType == enumScoringCalcType.ClientScoring)
            {
                try
                {
                    creditInfo = creditInfoList.First(x => x.ClientType == enumClientType.Borrower);
                }
                catch
                {
                    creditInfo = null;
                }

                helper.INCOME_AMOUNT_GEL = client.INCOME_AMOUNT_GEL;
                helper.LIABILITY_AMOUNT_GEL = client.LIABILITY_AMOUNT_GEL;
                helper.SEX_ID = client.SEX_ID;
                helper.MARITAL_STATUS_ID = client.MARITAL_STATUS_ID;
                helper.WORK_EXPERIENCE_TOTAL = client.WORK_EXPERIENCE_TOTAL;
                helper.WORK_EXPERIENCE_CURRENT = client.WORK_EXPERIENCE_CURRENT;
                helper.AGE = client.AGE;
                helper.ORGANIZATION_CATEGORY = client.ORGANIZATION_CATEGORY;
                helper.SALARY_STATUS_ID = client.SALARY_STATUS_ID;
                helper.INCOME_PER_PERSON = client.INCOME_PER_PERSON;
                helper.GUARANTEE_AMOUNT = client.GUARANTEE_AMOUNT;
            }
            else
            {
                try
                {
                    creditInfo = creditInfoList.First(x => x.ClientType == enumClientType.Coborrower);
                }
                catch
                {
                    creditInfo = null;
                }

                helper.INCOME_AMOUNT_GEL = coborrower.INCOME_AMOUNT_GEL;
                helper.LIABILITY_AMOUNT_GEL = coborrower.LIABILITY_AMOUNT_GEL;
                helper.SEX_ID = coborrower.SEX_ID;
                helper.MARITAL_STATUS_ID = coborrower.MARITAL_STATUS_ID;
                helper.WORK_EXPERIENCE_TOTAL = coborrower.WORK_EXPERIENCE_TOTAL;
                helper.WORK_EXPERIENCE_CURRENT = coborrower.WORK_EXPERIENCE_CURRENT;
                helper.AGE = coborrower.AGE;
                helper.ORGANIZATION_CATEGORY = coborrower.ORGANIZATION_CATEGORY;
                helper.SALARY_STATUS_ID = coborrower.SALARY_STATUS_ID;
                helper.INCOME_PER_PERSON = coborrower.INCOME_PER_PERSON;
                helper.GUARANTEE_AMOUNT = coborrower.GUARANTEE_AMOUNT;
            }

            if (creditInfo == null)
            {
                scoringResult.ScoringValue = 0;
                scoringResult.ScoringLogList = new List<FA_ScoringLog>();
                scoringResult.XIRRValue = 0;
                return scoringResult;
            }


            //1.2. ცვლადების ინიციალიზაცია
            helper.BASE_PRODUCT_ID = this.Helper_ProductByCode(appParam.PRODUCT_ID).BASE_PRODUCT_ID;
            helper.SCORING_TYPE_ID = scoringTypeID;
            helper.IS_TARGETED_HYPOTEC = (helper.BASE_PRODUCT_ID == 61 || helper.BASE_PRODUCT_ID == 63 || helper.BASE_PRODUCT_ID == 65 || helper.BASE_PRODUCT_ID == 67) ? true : false; //TODO: უკეთესად ხომ არ შეიძლება
            helper.IS_POS_CARD = (helper.BASE_PRODUCT_ID == 179) ? true : false; //TODO: უკეთესად ხომ არ შეიძლება (SepecialProducts)

            //Convert.ToDecimal(this.ConvertMoney(Convert.ToDouble(clientParam.INCOME_AMOUNT_GEL), appParam.LOAN_CURRENCY, "GEL"));
            //helper.AGE_NAME = clientParam.AGE.ToString() + " წელი";
            //helper.WORK_TIME = clientParam.WORK_EXPERIENCE_TOTAL.ToString() + " თვე";
            //helper.MARITAL_NAME = ""; //TODO from ScoringParamKoefList
            //helper.EMPLOYMENT_TIME_AVG_NAME = ""; //TODO

            helper.COBORROWER_INCOME_AMOUNT = 0;

            //helper.INCOME_VERIFIED_NAME = dictionary.ConfirmList.First(x => x.Key == creditInfo.INCOME_VERIFIED).Value;
            //helper.QUALIFICATION_WORKER = dictionary.QualificationWorkerList.First(x => x.Key == creditInfo.QUALIFICATION_WORKER).Value;

            // საწყისი პარამეტრების ლოგირება
            scoringLog = this.InitScoringLog(101, appParam.LOAN_AMOUNT.ToString("N2"), appParam.LOAN_AMOUNT.ToString("N2"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            scoringLog = this.InitScoringLog(102, appParam.LOAN_NPER.ToString("N0") + " თვე", appParam.LOAN_NPER.ToString("N0"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            scoringLog = this.InitScoringLog(103, appParam.LOAN_INTEREST_RATE.ToString("N2"), appParam.LOAN_INTEREST_RATE.ToString("N2"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            scoringLog = this.InitScoringLog(104, appParam.LOAN_CURRENCY, appParam.LOAN_CURRENCY, scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            scoringLog = this.InitScoringLog(105, appParam.COVER_AMOUNT.ToString("N2"), appParam.COVER_AMOUNT.ToString("N2"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            #region ===== 3.კოეფიციენტების გამოთვლა =====
            // --- 3.1.კრედიტ–ინფო(PARAM_ID = 4) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "CREDIT_INFO_SCOR");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_CREDIT_INFO = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "CREDIT_INFO", creditInfo.CREDIT_INFO_CATEGORY_ID, enumCoefOperType.EQ, true);
                if ((scoringTypeID == 7 || scoringTypeID == 8) && creditInfo.CREDIT_INFO_CATEGORY_ID == 12) //იპოთეკაა და კრედიტ–ინფო გრეიდი E3
                {
                    coef.COEF_CREDIT_INFO = 0.85;
                }

                helper.APP_VALUE_NAME = this.GetParamValueName(ScoringParamCoefList, scoringTypeID, "CREDIT_INFO", creditInfo.CREDIT_INFO_CATEGORY_ID);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.APP_VALUE_NAME, coef.COEF_CREDIT_INFO.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_CREDIT_INFO = 1.00;
            }

            // --- 3.2.სქესი(PARAM_ID = 15) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "SEX");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_SEX = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "SEX", helper.SEX_ID, enumCoefOperType.EQ, true);
                helper.APP_VALUE_NAME = (helper.SEX_ID == 0) ? "მდედრობითი" : "მამრობითი";

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.APP_VALUE_NAME, coef.COEF_SEX.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_SEX = 1.00;
            }

            // --- 3.3.ოჯახური მდგომარეობა(PARAM_ID = 11) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "MARITAL_STATUS");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_MARITAL = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "MARITAL_STATUS", helper.MARITAL_STATUS_ID, enumCoefOperType.EQ, true);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.MARITAL_STATUS_ID.ToString(), coef.COEF_MARITAL.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_MARITAL = 1.00;
            }

            // --- 3.4.სამუშაო სტაჟი(PARAM_ID = 6) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "EMPLOYMENT_TIME");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_WORK_EXPERIENCE = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "EMPLOYMENT_TIME", helper.WORK_EXPERIENCE_TOTAL, enumCoefOperType.LE, false);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.WORK_EXPERIENCE_TOTAL.ToString(), coef.COEF_WORK_EXPERIENCE.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_WORK_EXPERIENCE = 1.00;
            }

            // --- 3.5.ასაკი(PARAM_ID = 2) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "AGE");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_AGE = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "AGE", helper.AGE, enumCoefOperType.LE, false);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.AGE.ToString("N0"), coef.COEF_AGE.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_AGE = 1.00;
            }

            // --- 3.6.LTV(PARAM_ID = 10) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "LTV");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                helper.LTV = appParam.LOAN_AMOUNT / appParam.REAL_ESTATE_AMOUNT;
                coef.COEF_LTV = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "LTV", helper.LTV, enumCoefOperType.GE, true);

                if (helper.BASE_PRODUCT_ID == 61) //TODO: უკეთესად ხომ არ შეიძლება (SepecialProducts)
                {
                    if (helper.LTV >= 0.01m && helper.LTV <= 55.00m)
                    {
                        coef.COEF_LTV = 1.10;
                    }
                    else if (helper.LTV >= 55.01m && helper.LTV <= 65.00m)
                    {
                        coef.COEF_LTV = 1.05;
                    }
                    else if (helper.LTV >= 65.01m && helper.LTV <= 75.00m)
                    {
                        coef.COEF_LTV = 1.00;
                    }
                    else if (helper.LTV >= 75.01m && helper.LTV <= 85.00m)
                    {
                        coef.COEF_LTV = 0.95;
                    }
                    else if (helper.LTV >= 85.01m && helper.LTV <= 100.00m)
                    {
                        coef.COEF_LTV = 0.90;
                    }
                }

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.LTV.ToString("N2"), coef.COEF_LTV.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);

            }
            else
            {
                coef.COEF_LTV = 1.00;
            }

            // --- 3.7.ჯამური შემოსავალი(PARAM_ID = 8) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "INCOME_AMOUNT");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_INCOME = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "INCOME", helper.INCOME_AMOUNT_GEL, enumCoefOperType.LE, false);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.INCOME_AMOUNT_GEL.ToString("N2"), coef.COEF_INCOME.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_INCOME = 1.00;
            }

            // ---3.8.კატეგორია(PARAM_ID = 13) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "CATEGORY");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                int CategoryID = GetParamValueByName(ScoringParamCoefList, scoringTypeID, 13, helper.ORGANIZATION_CATEGORY);
                coef.COEF_CATEGORY = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "CATEGORY", CategoryID, enumCoefOperType.EQ, true);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.ORGANIZATION_CATEGORY, coef.COEF_CATEGORY.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_CATEGORY = 1.00;
            }

            // --- 3.9.სტატუსი(PARAM_ID = 14) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "STATUS");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_SALARY_STATUS = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "STATUS", helper.SALARY_STATUS_ID, enumCoefOperType.EQ, true);
                helper.APP_VALUE_NAME = this.GetParamValueName(ScoringParamCoefList, scoringTypeID, "STATUS", helper.SALARY_STATUS_ID);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.APP_VALUE_NAME, coef.COEF_SALARY_STATUS.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_SALARY_STATUS = 1.00;
            }

            // ---3.10.სესხის ვადა(PARAM_ID = 12) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "MATURITY");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_MATURITY = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "MATURITY", appParam.LOAN_NPER, enumCoefOperType.LT, false);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, appParam.LOAN_NPER.ToString("N0"), coef.COEF_MATURITY.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_MATURITY = 1.00;
            }

            // --- 3.11.თანამონაწილეობა(PARAM_ID = 5) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "DOWN_PAYMENT");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_DOWN_PAYMENT = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "DOWN_PAYMENT", appParam.PARTICIPATION_RATE, enumCoefOperType.LE, false);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, appParam.PARTICIPATION_RATE.ToString("N2"), coef.COEF_DOWN_PAYMENT.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_DOWN_PAYMENT = 1.00;
            }

            // --- 3.12.დადასტურება(PARAM_ID = 3) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "INCOME_VERIFIED");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_INCOME_VERIFIED = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "INCOME_VERIFIED", creditInfo.INCOME_VERIFIED, enumCoefOperType.EQ, true);
                helper.APP_VALUE_NAME = this.GetParamValueName(ScoringParamCoefList, scoringTypeID, "INCOME_VERIFIED", creditInfo.INCOME_VERIFIED);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.APP_VALUE_NAME, coef.COEF_INCOME_VERIFIED.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_INCOME_VERIFIED = 1.00;
            }

            // --- 3.13.შემოსავალი 1 წევრზე(PARAM_ID = 9) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "INCOME_PER_PERSON");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_INCOME_PER_PERSON = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "INCOME_PER_PERSON", helper.INCOME_PER_PERSON, enumCoefOperType.GE, true);

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.INCOME_PER_PERSON.ToString("N2"), coef.COEF_INCOME_PER_PERSON.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_INCOME_PER_PERSON = 1.00;
            }

            // --- 3.14.უძრავი ქონება(PARAM_ID = 1) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "ADDITIONAL_CAPITAL");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_ADDITIONAL_CAPITAL = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "ADDITIONAL_CAPITAL", Convert.ToDecimal(appParam.IS_ADDITIONAL_CAPITAL), enumCoefOperType.EQ, true);
                helper.APP_VALUE_NAME = (appParam.IS_ADDITIONAL_CAPITAL) ? "დიახ" : "არა";

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.APP_VALUE_NAME, coef.COEF_ADDITIONAL_CAPITAL.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_ADDITIONAL_CAPITAL = 1.00;
            }

            // --– 3.15.სამუშაო გამოცდილება - საშუალო(PARAM_ID = 7) –––
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "EMPLOYMENT_TIME_AVERAGE");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_EMPLOYMENT_TIME_TOTAL = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "EMPLOYMENT_TIME_TOTAL", helper.WORK_EXPERIENCE_TOTAL, enumCoefOperType.LE, false);
                coef.COEF_EMPLOYMENT_TIME_CURRENT = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "EMPLOYMENT_TIME_TOTAL", helper.WORK_EXPERIENCE_CURRENT, enumCoefOperType.LE, false);

                coef.COEF_EMPLOYMENT_TIME_AVG = (coef.COEF_EMPLOYMENT_TIME_TOTAL + coef.COEF_EMPLOYMENT_TIME_CURRENT) / 2;

                helper.APP_VALUE_NAME = helper.WORK_EXPERIENCE_TOTAL.ToString() + " თვე (" + coef.COEF_EMPLOYMENT_TIME_TOTAL.ToString() + ") / "
                                    + helper.WORK_EXPERIENCE_CURRENT.ToString() + " თვე (" + coef.COEF_EMPLOYMENT_TIME_CURRENT.ToString() + ")";

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.APP_VALUE_NAME, coef.COEF_EMPLOYMENT_TIME_AVG.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_EMPLOYMENT_TIME_AVG = 1.00;
            }

            // --- 3.16.არაკვალიფიცირებული თანამშრომელი(PARAM_ID = 16) ---
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "NON_QUALIFICATION_WORKER");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                coef.COEF_NON_QUALIFICATION_WORKER = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "NON_QUALIFICATION_WORKER", Convert.ToDecimal(creditInfo.IS_NON_QUALIFICATION_WORKER), enumCoefOperType.EQ, true);
                helper.APP_VALUE_NAME = (creditInfo.IS_NON_QUALIFICATION_WORKER) ? "არაკვალიფიცირებული" : "კვალიფიცირებული";

                scoringLog = this.InitScoringLog(helper.SCORING_PROPERTY_ID, helper.APP_VALUE_NAME, coef.COEF_NON_QUALIFICATION_WORKER.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            else
            {
                coef.COEF_NON_QUALIFICATION_WORKER = 1.00;
            }

            // -- 4.3.კლიენტის ვალდებულებები – KOEF_DTI_LIABILITY --
            double COEF_DTI_BASE = this.GetDTIBase(scoringTypeID);
            double COEF_MULTIPLIED = coef.COEF_CREDIT_INFO * coef.COEF_SEX * coef.COEF_MARITAL * coef.COEF_WORK_EXPERIENCE *
                                    coef.COEF_AGE * coef.COEF_INCOME * coef.COEF_SALARY_STATUS * coef.COEF_CATEGORY *
                                    coef.COEF_MATURITY * coef.COEF_DOWN_PAYMENT * coef.COEF_INCOME_VERIFIED * coef.COEF_INCOME_PER_PERSON *
                                    coef.COEF_ADDITIONAL_CAPITAL * coef.COEF_EMPLOYMENT_TIME_AVG * coef.COEF_NON_QUALIFICATION_WORKER;
            double COEF_DTI_SCORING_CALC = COEF_DTI_BASE * COEF_MULTIPLIED;

            scoringLog = this.InitScoringLog(301, COEF_DTI_BASE.ToString("N4"), COEF_DTI_BASE.ToString("N4"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);
            #endregion ===== End Coefs =====

            #region ===== 4.კლიენტის ვალდებულებები =====
            double COEF_DTI_LIABILITY = 0;

            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "LIABILITIES");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                COEF_DTI_LIABILITY = Convert.ToDouble(Math.Round(helper.LIABILITY_AMOUNT_GEL / helper.INCOME_AMOUNT_GEL, 4));

                scoringLog = this.InitScoringLog(303, "", COEF_DTI_LIABILITY.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }

            // -- 4.4.პირველადი PMT --
            // -- COEF_DTI_SCORING_AFTER
            double COEF_DTI_SCORING_AFTER = Math.Round(COEF_DTI_SCORING_CALC * coef.COEF_LTV, 4);
            if (COEF_MULTIPLIED <= 0.6) //კოეფიციენტების ნამრავლი ნაკლებია 0.6
            {
                COEF_DTI_SCORING_AFTER = 0;
            }

            scoringLog = this.InitScoringLog(302, "", COEF_DTI_SCORING_AFTER.ToString("N4"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            // -- COEF_DTI_ASL 
            double COEF_DTI_ASL = COEF_DTI_SCORING_AFTER - COEF_DTI_LIABILITY;

            scoringLog = this.InitScoringLog(304, "", COEF_DTI_ASL.ToString("N4"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            // -- P M T
            decimal INCOME_AMOUNT = Convert.ToDecimal(this.ConvertMoney(Convert.ToDouble(helper.INCOME_AMOUNT_GEL), "GEL", appParam.LOAN_CURRENCY));
            helper.PMT = Convert.ToDecimal(Math.Round(INCOME_AMOUNT * Convert.ToDecimal(COEF_DTI_ASL), 4));
            helper.PMT = (helper.PMT > INCOME_AMOUNT) ? 0 : helper.PMT;

            // -- 4.5.კლიენტის თავდებობა - PMT–ს დაზუსტება
            decimal LIABILITY_AMOUNT = 0;
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "GUARANTEE");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                LIABILITY_AMOUNT = Convert.ToDecimal(this.ConvertMoney(Convert.ToDouble(helper.LIABILITY_AMOUNT_GEL), "GEL", appParam.LOAN_CURRENCY));
                if (INCOME_AMOUNT - (helper.PMT + helper.GUARANTEE_AMOUNT) < 0)
                {
                    helper.PMT = INCOME_AMOUNT - (LIABILITY_AMOUNT + helper.GUARANTEE_AMOUNT);
                }
            }

            helper.PMT = (helper.PMT > 0) ? helper.PMT : 0;

            scoringLog = this.InitScoringLog(307, "", helper.PMT.ToString("N4"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            scoringLog = this.InitScoringLog(308, "", COEF_MULTIPLIED.ToString("N6"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            // -- 4.7.PV გამოთვლა
            decimal PV_CURR = appParam.LOAN_AMOUNT;
            decimal PV_PREV = 0;

            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "LTV");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                // -- 4.7.1.PV გამოთვლა იტერაციის მეთოდით
                decimal PMT = 0;
                int N = 0;

                if (coef.COEF_LTV > 0)
                {
                    coef.COEF_LTV = this.GetParamCoef(ScoringParamCoefList, scoringTypeID, "LTV", helper.LTV * 100, enumCoefOperType.LE, false);
                    while (Math.Abs(PV_CURR - PV_PREV) > 0.01m)
                    {
                        PMT = helper.PMT;
                        PV_PREV = PV_CURR;

                        PV_CURR = Convert.ToDecimal(this.PresentValue(Convert.ToDouble(PMT), Convert.ToDouble(appParam.LOAN_INTEREST_RATE / 12.00m), appParam.LOAN_NPER));
                    }
                }
                else
                {
                    PMT = 0;
                    PV_CURR = 0;
                }
            }
            else
            {
                // -- 4.7.2. PV-ს გამოთვლა
                if (scoringTypeID != 5)
                {
                    PV_CURR = Convert.ToDecimal(this.PresentValue(Convert.ToDouble(helper.PMT), Convert.ToDouble(appParam.LOAN_INTEREST_RATE / 12.00m), appParam.LOAN_NPER));
                }
                else
                {
                    PV_CURR = helper.PMT / (appParam.LOAN_INTEREST_RATE / 1200.00m + 0.05m);
                }
            }

            // -- 4.7.3.PV - ს დამრგვალება
            decimal MIN_ROUNDED_AMOUNT = (appParam.LOAN_AMOUNT * 0.02m < 50.00m) ? Math.Round(appParam.LOAN_AMOUNT * 0.02m, 2) : 50m;
            if (PV_CURR < appParam.LOAN_AMOUNT && appParam.LOAN_AMOUNT - PV_CURR <= MIN_ROUNDED_AMOUNT)
            {
                PV_CURR = Math.Round(appParam.LOAN_AMOUNT, 0);
            }
            else
            {
                PV_CURR = Math.Round(PV_CURR, 0);
            }

            //-- === 5.ყველა მაქსიმალური მნიშვნელობა ===
            List<decimal> MaxValuesList = new List<decimal>();

            // --5.2.მსესხებლის PV ===
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "PRESENT_VALUE");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                MaxValuesList.Add(PV_CURR);

                scoringLog = this.InitScoringLog(401, "", PV_CURR.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }

            // -- 5.3. მაქსიმალური თანხა პროდუქტით ===
            decimal MIN_PROD_LIMIT = 0;
            decimal MAX_PROD_LIMIT = 0;
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "MAX_PRODUCT_AMOUNT");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                FA_ProductLimit ProdLimit = this.ProductLimit(appParam.PRODUCT_ID, appParam.LOAN_CURRENCY, appParam.PRODUCT_CATEGORY_ID);
                MIN_PROD_LIMIT = ProdLimit.MIN_PROD_LIMIT;

                if (ProdLimit.MAX_SALARY_COUNT > 0)
                {
                    MAX_PROD_LIMIT = (ProdLimit.MAX_PROD_LIMIT < Math.Round(ProdLimit.MAX_SALARY_COUNT * INCOME_AMOUNT, 2)) ? ProdLimit.MAX_PROD_LIMIT : Math.Round(ProdLimit.MAX_SALARY_COUNT * INCOME_AMOUNT, 2);
                }
                else
                {
                    MAX_PROD_LIMIT = ProdLimit.MAX_PROD_LIMIT;
                }

                MaxValuesList.Add(MAX_PROD_LIMIT);

                scoringLog = this.InitScoringLog(402, "", MAX_PROD_LIMIT.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }

            // --5.4. მაქსიმალური LTV ===
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "MAX_LTV_AMOUNT");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                decimal MAX_LTV_LIMIT = appParam.REAL_ESTATE_AMOUNT * Convert.ToDecimal(this.GetMaxLTV(appParam.PRODUCT_ID));
                MaxValuesList.Add(MAX_LTV_LIMIT);

                scoringLog = this.InitScoringLog(403, "", MAX_LTV_LIMIT.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }

            // -- 5.5. გარანტორების მაქსიმალური თანხა ===
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "MAX_GUARANTEE_AMOUNT");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                List<FA_Guarantor> OnlyGuarantorList = guarantorList.Where(x => x.GuarantorType == enumClientType.Guarantor).ToList();

                decimal GUARANTOR_PMT_SUM = OnlyGuarantorList.Sum(item => item.GUARANTOR_PMT);
                int GUARANTOR_COUNT = OnlyGuarantorList.Count();
                decimal GUARANTOR_PMT = Math.Round(GUARANTOR_PMT_SUM / GUARANTOR_COUNT, 2);

                decimal MAX_GUARANTORS_LIMIT = Convert.ToDecimal(this.PresentValue(Convert.ToDouble(GUARANTOR_PMT), Convert.ToDouble(appParam.LOAN_INTEREST_RATE / 12.00m), appParam.LOAN_NPER));
                MaxValuesList.Add(MAX_GUARANTORS_LIMIT);

                scoringLog = this.InitScoringLog(404, "", MAX_GUARANTORS_LIMIT.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }

            //-- 5.6. მაქსიმალური ლიმიტი პროდუქტით ===
            helper.SCORING_PROPERTY_ID = this.GetScoringPropertyID(ScoringTypePropertyList, scoringTypeID, "MAX_PRODUCT_LIMIT");
            if (helper.SCORING_PROPERTY_ID != 0)
            {
                decimal PRODUCT_LIMIT_DEBT = appParam.EXISTS_PRODUCT_DEBT;
                decimal MAX_PRODUCT_LIMIT = (MAX_PROD_LIMIT - PRODUCT_LIMIT_DEBT > 0) ? MAX_PROD_LIMIT - PRODUCT_LIMIT_DEBT : 0;
                decimal UNION_LIMIT = Convert.ToDecimal(this.ConvertMoney(Convert.ToDouble(this.ProductLimitUnion(helper.BASE_PRODUCT_ID)), "GEL", appParam.LOAN_CURRENCY));
                MAX_PRODUCT_LIMIT = (MAX_PRODUCT_LIMIT < UNION_LIMIT) ? MAX_PRODUCT_LIMIT : UNION_LIMIT;
                MaxValuesList.Add(MAX_PRODUCT_LIMIT);

                scoringLog = this.InitScoringLog(405, "", MAX_PRODUCT_LIMIT.ToString("N4"), scorParam.ScorCalcType);
                ScoringLogList.Add(scoringLog);
            }
            #endregion ===== End კლიენტის ვალდებულებები =====

            //-- === 6.XIRR ===
            FA_XIIRParam xirrParam = new FA_XIIRParam();
            xirrParam.AMOUNT_LOAN = appParam.LOAN_AMOUNT;
            xirrParam.INTEREST_RATE = appParam.LOAN_INTEREST_RATE;
            xirrParam.LOAN_MONTHS = appParam.LOAN_NPER;
            xirrParam.DATE_START = appParam.CREATE_DATE;
            xirrParam.COMMISSION_RATE = appParam.COMMISSION_RATE;
            xirrParam.AMOUNT_DISCOUNT = appParam.AMOUNT_DISCOUNT;

            double APP_XIRR = this.ScoringXIRR(xirrParam);
            XIRR = Convert.ToDecimal(APP_XIRR);

            scoringLog = this.InitScoringLog(504, "", APP_XIRR.ToString("N4"), scorParam.ScorCalcType);
            ScoringLogList.Add(scoringLog);

            //-- === 7.სქორინგ–ლიმიტი ===
            SCORING_LIMIT = MaxValuesList.Min();
            if ((helper.SCORING_TYPE_ID == 4 || helper.SCORING_TYPE_ID == 6 || helper.IS_TARGETED_HYPOTEC == true) && SCORING_LIMIT > appParam.LOAN_AMOUNT)
            {
                SCORING_LIMIT = appParam.LOAN_AMOUNT;
            }

            if (helper.SCORING_TYPE_ID == 4 && (appParam.LOAN_AMOUNT - SCORING_LIMIT <= MIN_ROUNDED_AMOUNT))
            {
                SCORING_LIMIT = appParam.LOAN_AMOUNT;
            }

            //--თუ სქორინგის თანხა ნაკლებია მოთხოვნილ თანხაზე, მაშინ სქორინგი = 0
            if ((helper.SCORING_TYPE_ID == 4 || helper.SCORING_TYPE_ID == 6 || helper.IS_TARGETED_HYPOTEC == true) && helper.IS_POS_CARD && SCORING_LIMIT < appParam.LOAN_AMOUNT)
            {
                SCORING_LIMIT = 0;
            }

            //--თუ მიღებული სქორინგ–ლიმიტი ნაკლებია პროდუქტის მინიმალურ თანხაზე
            if (SCORING_LIMIT < MIN_PROD_LIMIT)
            {
                SCORING_LIMIT = 0;
            }

            //--ჯეოსელის განვადება(გამონაკლისი: 50000 USD)
            decimal LIABILITY_AMOUNT_USD = 0;
            if (helper.SCORING_TYPE_ID == 6 && SCORING_LIMIT == 0)
            {
                LIABILITY_AMOUNT_USD = Convert.ToDecimal(this.ConvertMoney(Convert.ToDouble(client.LIABILITY_AMOUNT_GEL), "GEL", "USD"));

                if (client.NEGATIVE_STATUSES_COUNT == 0 && LIABILITY_AMOUNT_USD >= 50000.00m)
                {
                    SCORING_LIMIT = appParam.LOAN_AMOUNT;
                }

                //--ჯეოსელის განვადება(გამონაკლისი: მოთხოვნილი თანხა <= 350)
                if (appParam.LOAN_AMOUNT <= 350)
                {
                    SCORING_LIMIT = appParam.LOAN_AMOUNT;
                }
            }

            //--ექსპრეს განვადება(გამონაკლისი: 50000 USD)
            if (helper.BASE_PRODUCT_ID == 102 && SCORING_LIMIT == 0)
            {
                LIABILITY_AMOUNT_USD = Convert.ToDecimal(this.ConvertMoney(Convert.ToDouble(client.LIABILITY_AMOUNT_GEL), "GEL", "USD"));

                if (appParam.LOAN_AMOUNT <= 350 || (LIABILITY_AMOUNT_USD >= 50000.00m && appParam.LOAN_AMOUNT <= 3000.00m))
                {
                    SCORING_LIMIT = appParam.LOAN_AMOUNT;
                }
            }

            // ===== Final result ======
            scoringResult.ScoringValue = SCORING_LIMIT;
            scoringResult.XIRRValue = XIRR;
            scoringResult.ScoringLogList = ScoringLogList;
            return scoringResult;
        }

        [WebMethod]
        public int TechUser(int mBranchID)
        {
            int RetValue = 0;

            return RetValue;
        }

        #region -- Scoring - Private --
        private LocalModel.Product Helper_ProductByCode(int productId)
        {
            LocalModel.Product LoanProduct = new LocalModel.Product();

            try
            {
                LoanProduct = this.ProductList(1).First(x => x.PRODUCT_ID == productId);
            }
            catch { }

            return LoanProduct;
        }

        private List<MapScoringTypeProperty> MapScoringTypePropertyList(int scoringTypeID)
        {
            string mSQL = String.Format("EXEC dbo.pMAP_SCORING_TYPE_PROPERTY_LIST @SCORING_TYPE_ID = {0}", scoringTypeID);
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<MapScoringTypeProperty> RetList = new List<MapScoringTypeProperty>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                MapScoringTypeProperty mClass = new MapScoringTypeProperty();
                mClass = (MapScoringTypeProperty)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        private int GetScoringPropertyID(List<MapScoringTypeProperty> ScoringPropertyList, int scoringTypeID, string propertySysname)
        {
            int propID = 0;
            try
            {
                propID = ScoringPropertyList.Find(x => x.SCORING_TYPE_ID == scoringTypeID && x.SCORING_PROPERTY_SYS_NAME == propertySysname).SCORING_PROPERTY_ID;
            }
            catch { }

            return propID;
        }

        private double GetParamCoef(List<ScoringParamCoef> ScoringParamCoefList, int scoringTypeID, string paramName, decimal paramValue, enumCoefOperType OperType, bool isAscending)
        {
            double paramCoef = 0;
            List<ScoringParamCoef> ParamCoefList = ScoringParamCoefList.Where(x => x.SCORING_TYPE_ID == scoringTypeID && x.PARAM_NAME == paramName).ToList();

            try
            {
                if (OperType == enumCoefOperType.EQ)
                {
                    if (isAscending)
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE == paramValue).ToList().OrderBy(y => y.PARAM_VALUE).ToList();
                    }
                    else
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE == paramValue).ToList().OrderByDescending(y => y.PARAM_VALUE).ToList();
                    }
                }
                else if (OperType == enumCoefOperType.GE)
                {
                    if (isAscending)
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE >= paramValue).ToList().OrderBy(y => y.PARAM_VALUE).ToList();
                    }
                    else
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE >= paramValue).ToList().OrderByDescending(y => y.PARAM_VALUE).ToList();
                    }
                }
                else if (OperType == enumCoefOperType.GT)
                {
                    if (isAscending)
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE > paramValue).ToList().OrderBy(y => y.PARAM_VALUE).ToList();
                    }
                    else
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE > paramValue).ToList().OrderByDescending(y => y.PARAM_VALUE).ToList();
                    }
                }
                else if (OperType == enumCoefOperType.LE)
                {
                    if (isAscending)
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE <= paramValue).ToList().OrderBy(y => y.PARAM_VALUE).ToList();
                    }
                    else
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE <= paramValue).ToList().OrderByDescending(y => y.PARAM_VALUE).ToList();
                    }
                }
                else if (OperType == enumCoefOperType.LT)
                {
                    if (isAscending)
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE < paramValue).ToList().OrderBy(y => y.PARAM_VALUE).ToList();
                    }
                    else
                    {
                        ParamCoefList = ParamCoefList.Where(x => x.PARAM_VALUE < paramValue).ToList().OrderByDescending(y => y.PARAM_VALUE).ToList();
                    }
                }
            }
            catch { }

            try
            {
                paramCoef = ParamCoefList.First(x => x.PARAM_VALUE_ID >= 0).PARAM_KOEF;
            }
            catch
            {
                paramCoef = 0;
            }

            return paramCoef;
        }

        private string GetParamValueName(List<ScoringParamCoef> ScoringParamCoefList, int scoringTypeID, string paramName, int paramValue)
        {
            string paramValueName = "";
            try
            {
                paramValueName = ScoringParamCoefList.First(x => x.SCORING_TYPE_ID == scoringTypeID && x.PARAM_NAME == paramName && x.PARAM_VALUE == paramValue).PARAM_VALUE_NAME;
            }
            catch { }

            return paramValueName;
        }

        private int GetParamValueByName(List<ScoringParamCoef> ScoringParamCoefList, int scoringTypeID, int paramID, string paramValueName)
        {
            int paramValue = 0;
            try
            {
                paramValue = ScoringParamCoefList.First(x => x.SCORING_TYPE_ID == scoringTypeID
                                                              && x.PARAM_ID == paramID
                                                              && x.PARAM_VALUE_NAME == paramValueName).PARAM_VALUE;
            }
            catch { }

            return paramValue;
        }

        private FA_ScoringLog InitScoringLog(int PropertyId, string appValue, string scorValue, enumScoringCalcType scorType)
        {
            FA_ScoringLog scoringLog = new FA_ScoringLog();
            scoringLog.SCORING_PROPERTY_ID = PropertyId;
            scoringLog.APPLICATION_VALUE = appValue;
            scoringLog.SCORING_VALUE = scorValue;
            scoringLog.SCORING_CALC_TYPE = (int)scorType;

            return scoringLog;
        }

        private double GetDTIBase(int scoringTypeID)
        {
            double retValue = 0;

            string mSQL = String.Format("SELECT DTI_BASE FROM dbo.tREF_SCORING_TYPE WHERE SCORING_TYPE_ID = {0}", scoringTypeID);

            DataTable mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            if (mDataTable != null)
            {
                DataRow mDataRow = mDataTable.Rows[0];
                if (mDataRow != null)
                {
                    try
                    {
                        retValue = Convert.ToDouble(mDataRow["DTI_BASE"]);
                    }
                    catch { }
                }
            }

            return retValue;
        }

        private double GetMaxLTV(int productID)
        {
            double retValue = 0;

            string mSQL = String.Format("SELECT MIN(L.MAX_LTV) AS MAX_LTV FROM dbo.tMAP_PRODUCT_MAX_LTV L (NOLOCK) WHERE L.PRODUCT_ID = {0}", productID);

            DataTable mDataTable = _DGate.GetDataSet(mSQL).Tables[0];

            if (mDataTable != null)
            {
                DataRow mDataRow = mDataTable.Rows[0];
                if (mDataRow != null)
                {
                    try
                    {
                        retValue = Convert.ToDouble(mDataRow["MAX_LTV"]);
                    }
                    catch { }
                }
            }

            return retValue;
        }


        public FA_ProductLimit ProductLimit(int mProdId, string mCCY, int mCategId)
        {
            string mSQL = String.Format(@"EXEC dbo.pPRODUCT_PROPERTIES_FOR_SCORING
                                        @PRODUCT_ID = {0}, @LOAN_CURRENCY = {1}, @PRODUCT_CATEGORY_ID = {2}, @PRODUCT_EXTENDED_ID = {3}",
                                        mProdId, StringFunctions.SqlQuoted(mCCY), mCategId, 0);

            DataRow mDataRow = _DGate.GetDataSet(mSQL).Tables[0].Rows[0];

            FA_ProductLimit mClass = new FA_ProductLimit();
            mClass = (FA_ProductLimit)FillClass(mClass, mDataRow);

            return mClass;
        }

        private double PresentValue(double mPMT, double mRatePerMonth, int mNPER)
        {
            double PresentValue = 0;

            if (mRatePerMonth > 0)
            {
                PresentValue = mPMT * (Math.Pow(1 + mRatePerMonth / 100.00, mNPER) - 1) / (((mRatePerMonth / 100.00)) * Math.Pow(1 + mRatePerMonth / 100, mNPER));
            }
            else
            {
                PresentValue = mPMT * mNPER;
            }

            return PresentValue;
        }

        private decimal ProductLimitUnion(int mBASE_PRODUCT_ID)
        {
            return 1000000.00m; //TODO
        }

        private double ScoringXIRR(FA_XIIRParam xirrParam)
        {
            List<FA_ScheduleXIRR> TableXIRR = new List<FA_ScheduleXIRR>();
            DateTime POINT_DATE;
            decimal POINT_PAYMENT = 0;

            DateTime DATE_END = xirrParam.DATE_START.AddMonths(xirrParam.LOAN_MONTHS);
            decimal AMOUNT_COMMISSION = 0;
            decimal AMOUNT_DISCOUNT = 0;
            decimal AMOUNT_ACCOUNT = 1.00m;
            decimal AMOUNT_INSURANCE = 2.00m;

            AMOUNT_COMMISSION = xirrParam.AMOUNT_LOAN * xirrParam.COMMISSION_RATE / 100.00m;
            AMOUNT_DISCOUNT = xirrParam.AMOUNT_DISCOUNT;

            //-- === XIRR - ცხრილი ===
            //--1.სესხის გაცემა - პირველი ათვისება
            FA_ScheduleXIRR schedule = new FA_ScheduleXIRR();
            POINT_DATE = xirrParam.DATE_START;
            POINT_PAYMENT = xirrParam.AMOUNT_LOAN * (-1) + AMOUNT_COMMISSION + AMOUNT_DISCOUNT;

            schedule.PAYMENT_DATE = POINT_DATE;
            schedule.PAYMENT = Convert.ToDouble(POINT_PAYMENT);
            TableXIRR.Add(schedule);

            //--2.შუა წერტილები
            DateTime POINT_DATE_LAST = new DateTime(1900, 1, 1);
            POINT_DATE = POINT_DATE.AddDays(30);
            while (POINT_DATE < DATE_END)
            {
                if (POINT_DATE < DATE_END)
                {
                    POINT_DATE_LAST = POINT_DATE;
                }

                schedule = new FA_ScheduleXIRR();
                POINT_PAYMENT = Convert.ToDecimal(this.CalcPMT(Convert.ToDouble(xirrParam.AMOUNT_LOAN), xirrParam.LOAN_MONTHS, Convert.ToDouble(xirrParam.INTEREST_RATE))) + AMOUNT_ACCOUNT + AMOUNT_INSURANCE;

                schedule.PAYMENT_DATE = POINT_DATE;
                schedule.PAYMENT = Convert.ToDouble(POINT_PAYMENT);
                TableXIRR.Add(schedule);

                POINT_DATE = POINT_DATE.AddDays(30);
            }

            TableXIRR.RemoveAll(x => x.PAYMENT_DATE == POINT_DATE_LAST);

            //--3.ბოლო წერტილი
            schedule = new FA_ScheduleXIRR();
            POINT_DATE = DATE_END;
            POINT_PAYMENT = Convert.ToDecimal(this.CalcPMT(Convert.ToDouble(xirrParam.AMOUNT_LOAN), xirrParam.LOAN_MONTHS, Convert.ToDouble(xirrParam.INTEREST_RATE))) + AMOUNT_ACCOUNT + AMOUNT_INSURANCE;

            schedule.PAYMENT_DATE = POINT_DATE;
            schedule.PAYMENT = Convert.ToDouble(POINT_PAYMENT);
            TableXIRR.Add(schedule);

            //-- === CALC ===
            DateTime calcFirstPayDate;
            calcFirstPayDate = TableXIRR.Min(x => x.PAYMENT_DATE);

            double nRate = 0.1;
            double nLastRate = 0;
            double nRateStep = 0.1;
            double nResidual = 10;
            double nLastResidual = 1;
            int n = 0;
            nLastRate = nRate;

            while (n < 100 && Math.Abs((nLastResidual - nResidual) / nLastResidual) > Math.Pow(10, -8))
            {
                nLastResidual = nResidual;
                nResidual = 0;

                foreach (FA_ScheduleXIRR point in TableXIRR)
                {
                    nResidual += point.PAYMENT / Math.Pow(1 + nRate, this.DayCountXIRR(xirrParam.DATE_START, point.PAYMENT_DATE));
                }

                nLastRate = nRate;
                if (nResidual >= 0)
                {
                    nRate += nRateStep;
                }
                else
                {
                    nRateStep = nRateStep / 2.0;
                    nRate -= nRateStep;
                }

                n += 1;
            }

            return nLastRate * 100.00;
        }

        private double DayCountXIRR(DateTime mStartDate, DateTime mEndDate)
        {
            double Result = 0;

            DateTime EndYear_Start = new DateTime(mStartDate.Year, 12, 31);
            DateTime StartYear_End = new DateTime(mEndDate.Year, 01, 01);
            double Days_Start = (EndYear_Start - mStartDate).TotalDays;
            double Days_End = (mEndDate - StartYear_End).TotalDays;
            double DaysInStartYear = (mStartDate.Year % 4 == 0) ? 366 : 365;
            double DaysInEndYear = (mEndDate.Year % 4 == 0) ? 366 : 365;
            int Years = mEndDate.Year - mStartDate.Year - 1;

            Result = Days_Start / DaysInStartYear + Days_End / DaysInEndYear + Years;
            return Result;
        }
        #endregion -- Scoring - Private --
        #endregion

        #region === Virtual Info ===
        [WebMethod]
        public List<VirtualPercent> VirtualPercentList()
        {
            string mSQL = String.Format(@"EXEC dbo.pVIRTUAL_INTEREST_RATE");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<VirtualPercent> RetList = new List<VirtualPercent>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                VirtualPercent mClass = new VirtualPercent();
                mClass = (VirtualPercent)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }

        [WebMethod]
        public List<VirtualProduct> VirtualProductList()
        {
            string mSQL = String.Format("EXEC dbo.pREF_PRODUCT_VIRTUAL_LIST");
            mDataTable = _DGate.GetDataSet(mSQL).Tables[0];
            List<VirtualProduct> RetList = new List<VirtualProduct>();
            foreach (DataRow mDataRow in mDataTable.Rows)
            {
                VirtualProduct mClass = new VirtualProduct();
                mClass = (VirtualProduct)FillClass(mClass, mDataRow);
                RetList.Add(mClass);
            }
            return RetList;
        }
        #endregion

        #region === RS ===
        [WebMethod]
        public RSError GetPayerSmsCode(string saidCode, string fullname, string mobile, string email)
        {
            RSService.RSSErviceClient rs = new RSSErviceClient();
            RSError rsError = rs.GetPayerSmsCode(new GetPayerSmsCodeRequest { email = email, fullname = fullname, mobile = mobile, saidCode = saidCode }).GetPayerSmsCodeResult;
            return rsError;
        }
        [WebMethod]
        public PayerInfoData GetPayerInfoData(string saidCode, string smsCode, int applicationId, int userId)
        {
            RSService.RSSErviceClient rs = new RSSErviceClient();
            PayerInfoData payerInfoData = rs.GetPayerInfoData(new GetPayerInfoDataRequest { applicationId = applicationId, saidCode = saidCode, smsCode = smsCode, userId = userId }).GetPayerInfoDataResult;
            return payerInfoData;
        }
        [WebMethod]
        public PayerInfoData GetPayerInfoDataLocal(string saidCode, int applicationId)
        {
            RSService.RSSErviceClient rs = new RSSErviceClient();
            PayerInfoData payerInfoData = rs.GetPayerInfoDataLocal(new GetPayerInfoDataLocalRequest { applicationId = applicationId, saidCode = saidCode }).GetPayerInfoDataLocalResult;
            return payerInfoData;
        }
        #endregion === RS ===
    }
}