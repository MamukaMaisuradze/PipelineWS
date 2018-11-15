using System;
using System.Collections.Generic;
using System.Web.Services;
using System.IO;

using Model;
using MF_Wrapper.Bank;
using MFiles.Mfws.Structs;
using Stimulsoft.Report;

namespace PipelineWS
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class MFilesService : System.Web.Services.WebService
    {
        #region === Private Members ===
        private AppSettings CurAppSettings = new AppSettings();

        private string CurURL = "";
        private string CurUserName = "";
        private string CurPassword = "";
        private string CurVaultGuid = "";
        #endregion

        #region === Class Constructor ===
        public MFilesService()
        {
            this.CurURL = CurAppSettings["MFilesURL"];
            this.CurUserName = CurAppSettings["MFilesUserName"];
            this.CurPassword = CurAppSettings["MFilesPassword"];
            this.CurVaultGuid = CurAppSettings["MFilesVaultGuid"];

            this.Authentication();
        }
        #endregion

        #region === Private Methods ===
        private void Authentication()
        {
            BankAuthentication bankAuthentication = new BankAuthentication(this.CurURL, this.CurUserName, this.CurPassword, this.CurVaultGuid);
            BankFunc.URL = this.CurURL;
            BankFunc.Authentication = bankAuthentication.Authentication;
        }
        #endregion

        #region === HELPERS ===
        [WebMethod]
        public List<PropertyDef> Helper_GetPropertyDefList()
        {
            return BankFunc.GetPropertyDefList();
        }

        [WebMethod]
        public byte[] Print_CheckList(List<MFCheckList> reportdata)
        {
            var dataStream = new MemoryStream();
            using (var ms = new MemoryStream())
            {
                var report = new StiReport { ReportAuthor = "JSC VTB Bank Georgia" };
                report.Load(@"StimulSoft\Reports\RptMFiles_CheckList.mrt");
                report.RegBusinessObject("MFCheckList", reportdata);
                report.Render(false);
                report.ExportDocument(StiExportFormat.Pdf, ms);
                ms.Position = 0;
                ms.CopyTo(dataStream);
                dataStream.Position = 0;

                return dataStream.ToArray();
            }
        }
        #endregion

        #region === CLIENT ===
        [WebMethod]
        public ObjectVersion Client_Create(BankClient bankClient)
        {
            BankClientClass bankClientClass = new BankClientClass();

            string PID = bankClient.PersonalID;
            ObjectVersion objectVersion = bankClientClass.SearchClientByPersonalID(PID);
            if (objectVersion != null)
            {
                return objectVersion;
            }
            else
            {
                return bankClientClass.CreateClient(bankClient).ObjectVersion;
            }
        }

        [WebMethod]
        public ObjectVersion ClientCaseRetail_Create(BankClient bankClient, BankCaseRetail CaseRetail)
        {
            ObjectVersion RetValue = new ObjectVersion();

            BankClientClass bankClientClass = new BankClientClass();

            string PID = bankClient.PersonalID;
            ObjectVersion objectVersion = bankClientClass.SearchClientByPersonalID(PID);
            if (objectVersion != null)
            {
                RetValue = objectVersion;
            }
            else
            {
                RetValue =  bankClientClass.CreateClient(bankClient).ObjectVersion;
            }

            CaseRetail.Client = Convert.ToInt32(RetValue.DisplayID);

            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();

            string mAppID = CaseRetail.ApplicationID;
            RetValue = this.CaseRetail_SearchByApplicationID(mAppID);

            if (RetValue == null)
            {
                RetValue = bankCaseRetailClass.CreateCaseRetail(CaseRetail).ObjectVersion;
            }

            return RetValue;
        }

        [WebMethod]
        public List<CaseInfo> Client_GetCaseInfo(string clientID)
        {
            BankClientClass bankClientClass = new BankClientClass();
            return bankClientClass.GetCaseInfoList(clientID);
        }

        [WebMethod]
        public ObjectVersion Client_SearchByPersonalID(string personalID)
        {
            BankClientClass bankClientClass = new BankClientClass();
            return bankClientClass.SearchClientByPersonalID(personalID);
        }

        //[WebMethod]
        //public ObjectVersion Client_IsDeleted(string personalID)
        //{
        //    BankClientClass bankClientClass = new BankClientClass();
        //    return bankClientClass.IsClientDeleted(personalID);
        //}
        #endregion

        #region === CASE SME ===
        [WebMethod]
        public BankObjectVersion CaseSME_Create(BankCaseSME CaseSME)
        {
            BankCaseSMEClass bankCaseSMEClass = new BankCaseSMEClass();
            return bankCaseSMEClass.CreateCaseSME(CaseSME);
        }

        [WebMethod]
        public ObjectVersion CaseSME_SearchByCaseNo(string caseNo)
        {
            //TODO: caseNo-ს მაგივრად უნდა იყოს ApplicationID (String)

            BankCaseSMEClass bankCaseSMEClass = new BankCaseSMEClass();
            return bankCaseSMEClass.SearchCaseByCaseNo(caseNo);
        }

        [WebMethod]
        public ObjectVersion CaseSME_UpdateCurrentStatus(int CaseID, object NewValue)
        {
            BankCaseSMEClass bankCaseSMEClass = new BankCaseSMEClass();
            return bankCaseSMEClass.UpdateCurrentStatus(CaseID, NewValue);
        }

        [WebMethod]
        public ObjectVersion CaseSME_UpdateEndDate(int CaseID, object NewValue)
        {
            BankCaseSMEClass bankCaseSMEClass = new BankCaseSMEClass();
            return bankCaseSMEClass.UpdateEndDate(CaseID, NewValue);
        }
        #endregion

        #region === CASE RETAIL ===
        [WebMethod]
        public BankObjectVersion CaseRetail_Create(BankCaseRetail CaseRetail)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.CreateCaseRetail(CaseRetail);
        }

        [WebMethod]
        public List<CaseInfo> CaseRetail_GetCaseInfo(string ApplicationID)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.GetCaseInfoList(ApplicationID);
        }

        [WebMethod]
        public List<CaseNotClosed> CaseRetail_CaseNotClosedList(string AdminID)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.CaseNotClosedList(AdminID);
        }

        [WebMethod]
        public ObjectVersion CaseRetail_SearchByCaseNo(string caseNo)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.SearchCaseByCaseNo(caseNo);
        }

        [WebMethod]
        public ObjectVersion CaseRetail_SearchByApplicationID(string appID)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.SearchCaseByApplicationID(appID);
        }

        [WebMethod]
        public ObjectVersion CaseRetail_UpdateCurrentStatus(int CaseID, object NewValue)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.UpdateCurrentStatus(CaseID, NewValue);
        }

        [WebMethod]
        public ObjectVersion CaseRetail_UpdateEndDate(int CaseID, object NewValue)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.UpdateEndDate(CaseID, NewValue);
        }

        [WebMethod]
        public ObjectVersion CaseRetail_UpdateWorkflowState(int CaseID, int NewValue)
        {
            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.UpdateWorkflowState(CaseID, NewValue);
        }

        [WebMethod]
        public ObjectVersion CaseRetail_Close(int CaseID, int CaseStageID)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            ObjectVersion objVer = bankCaseStageClass.UpdateWorkflowState(CaseStageID, 276);

            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            return bankCaseRetailClass.UpdateWorkflowState(CaseID, 256);
        }
        #endregion

        #region === STAGE ===
        [WebMethod]
        public BankObjectVersion Stage_Create(BankCaseStage CaseStage)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            return bankCaseStageClass.CreateCaseStage(CaseStage);
        }

        [WebMethod]
        public ObjectVersion Stage_SearchByID(int caseStagID)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            return bankCaseStageClass.SearchCaseStageByID(caseStagID);
        }

        [WebMethod]
        public ObjectVersion Stage_UpdateCurrentStatus(int CaseStageID, BankStatuses NewValue)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            return bankCaseStageClass.UpdateCurrentStatus(CaseStageID, NewValue);
        }

        [WebMethod]
        public ObjectVersion Stage_UpdateWorkflowState(int CaseStageID, object NewValue)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            return bankCaseStageClass.UpdateWorkflowState(CaseStageID, NewValue);
        }

        [WebMethod]
        public ObjectVersion Stage_UpdateResponsibleUser(int CaseStageID, object NewValue)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            return bankCaseStageClass.UpdateResponsibleUser(CaseStageID, NewValue);
        }

        [WebMethod]
        public ObjectVersion Stage_UpdateRegistratorUser(int CaseStageID, object NewValue)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            return bankCaseStageClass.UpdateRegistratorUser(CaseStageID, NewValue);
        }

        [WebMethod]
        public bool Stage_IsClosed(int CaseStageID)
        {
            BankCaseStageClass bankCaseStageClass = new BankCaseStageClass();
            return bankCaseStageClass.IsClosed(CaseStageID);
        }
        #endregion

        #region === DOC ENTRY ===
        [WebMethod]
        public ObjectVersion DocEntry_SearchByID(int docEntryID)
        {
            BankDocumentEntryClass bankDocumentEntryClass = new BankDocumentEntryClass();
            return bankDocumentEntryClass.SearchDocumentEntryByID(docEntryID);
        }

        [WebMethod]
        public ObjectVersion DocEntry_SearchByNo(string docEntryNo)
        {
            BankDocumentEntryClass bankDocumentEntryClass = new BankDocumentEntryClass();
            return bankDocumentEntryClass.SearchDocumentEntryByNo(docEntryNo);
        }

        [WebMethod]
        public ObjectVersion DocEntry_UpdateIsRequired(int docEntryID, object NewValue)
        {
            BankDocumentEntryClass bankDocumentEntryClass = new BankDocumentEntryClass();
            return bankDocumentEntryClass.UpdateIsRequired(docEntryID, NewValue);
        }

        [WebMethod]
        public ObjectVersion DocEntry_UpdateIsRequiredForArchivist(int caseID, int adminID, int docEntryID, bool isArchivist)
        {
            ObjectVersion objectVersion = null;

            BankCaseRetailClass bankCaseRetailClass = new BankCaseRetailClass();
            ObjectVersion ov = bankCaseRetailClass.UpdateCreditAdministrator(caseID, adminID);

            if (ov != null)
            {
                BankDocumentEntryClass bankDocumentEntryClass = new BankDocumentEntryClass();
                objectVersion = bankDocumentEntryClass.UpdateIsRequiredForArchivist(docEntryID, isArchivist);
            }

            return objectVersion;
        }

        [WebMethod]
        public ObjectVersion DocEntry_UpdateCurrentStatus(int docEntryID, BankStatuses NewValue)
        {
            BankDocumentEntryClass bankDocumentEntryClass = new BankDocumentEntryClass();
            return bankDocumentEntryClass.UpdateCurrentStatus(docEntryID, NewValue);
        }
        #endregion

        #region === DOCUMENT===
        [WebMethod]
        public BankObjectVersion Document_Create(BankDocument document, string FileFullPath)
        {
            BankDocumentClass bankDocumentClass = new BankDocumentClass();
            return bankDocumentClass.CreateDocument(document, FileFullPath);
        }

        [WebMethod]
        public BankObjectVersion Document_CreateByStream(BankDocument document, string fileName, byte[] fileStream)
        {
            BankDocumentClass bankDocumentClass = new BankDocumentClass();
            return bankDocumentClass.CreateDocumentByStream(document, fileName, fileStream);
        }

        [WebMethod]
        public ObjectVersion Document_SearchByID(int documID)
        {
            BankDocumentClass bankDocumentClass = new BankDocumentClass();
            return bankDocumentClass.SearchdocumentByID(documID);
        }

        [WebMethod]
        public ObjectVersion Document_UpdateCurrentStatus(int CaseStageID, BankStatuses NewValue)
        {
            BankDocumentClass bankDocumentClass = new BankDocumentClass();
            return bankDocumentClass.UpdateCurrentStatus(CaseStageID, NewValue);
        }

        [WebMethod]
        public bool Document_Delete(int documentID)
        {
            BankDocumentClass bankDocumentClass = new BankDocumentClass();
            return bankDocumentClass.DeleteDocument(documentID);
        }

        [WebMethod]
        public byte[] Document_Download(string path)
        {
            BankDocumentClass bankDocumentClass = new BankDocumentClass();

            byte[] result = bankDocumentClass.DownloadFile(path);
            return result;
        }
        #endregion
    }

    public class MFCheckList
    {
        public string ApplicationID { get; set; }
        public DateTime PrintDate { get; set; }
        public string CaseName { get; set; }
        public string ProductName { get; set; }
        public string ClientPID { get; set; }
        public string ClientName { get; set; }
        public string CreditManagerName { get; set; }
        public string DocEntryName { get; set; }
        public string DocEntryID { get; set; }
        public int DocCount { get; set; }
    }
}
