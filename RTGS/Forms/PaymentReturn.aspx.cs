using RTGS.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RTGS.Forms
{
    public partial class PaymentReturn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            string InwardID = Request.QueryString["InwardID"];

            FloraSoft.BankSettingsDB db0 = new FloraSoft.BankSettingsDB();
            FloraSoft.BankSettings bs = db0.GetBankSettings();

            if (Request.QueryString["FormName"] == "Pacs.008")
            {
                RTGSImporter.Pacs008 pacs = new RTGSImporter.Pacs008();
                RTGSImporter.TeamRedDB db = new RTGSImporter.TeamRedDB();

                pacs = db.GetSingleInward08(InwardID);
                lblAccountNo.Text = pacs.CdtrAcctOthrId;
                lblSettlmentAmount.Text = string.Format("{0:N}", pacs.IntrBkSttlmAmt);
                lblCCY.Text = pacs.Ccy;
                lblToBranch.Text = pacs.ToBranch;
                lblReceivingBank.Text = pacs.FrBank;
                lblReceivingBranch.Text = pacs.FrBranch;
                //lblRoutingNo.Text = pacs.FrBranch;
               // lblToBranch.Text = pacs.ToBranch;
                //lblToBranchID.Text = pacs.To;
                lblReceiverName.Text = pacs.DbtrNm;
                lblReceiverAccountNo.Text = pacs.DbtrAcctOthrId;
            }

            if (Request.QueryString["FormName"] == "Pacs.009")
            {
                RTGSImporter.Pacs009 pacs = new RTGSImporter.Pacs009();
                RTGSImporter.TeamBlueDB db = new RTGSImporter.TeamBlueDB();

                pacs = db.GetSingleInward09(InwardID);
                lblAccountNo.Text = pacs.CdtrAcctId;
                lblSettlmentAmount.Text = string.Format("{0:N}", pacs.IntrBkSttlmAmt);
                lblCCY.Text = pacs.IntrBkSttlmCcy;
                lblReceivingBank.Text = pacs.FrBankBIC;
                lblReceivingBranch.Text = pacs.FrBranch;
                //lblRoutingNo.Text = pacs.FrBranchID;
               // lblToBranch.Text = pacs.ToBranch;
               // lblToBranchID.Text = pacs.ToBranchID;
                lblReceiverName.Text = pacs.DbtrNm;
                lblReceiverAccountNo.Text = pacs.DbtrAcctId;
            }
        }


        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Request.Params["FormName"] == "Pacs.009")
            {
                ProcessPacs009();
            }
            else
            {
                ProcessPacs008();
            }
            Response.Redirect("../InwardList.aspx");
        }

        private void ProcessPacs008()
        {
            string InwardID = Request.QueryString["InwardID"];

            string Credt = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss:fff");

            FloraSoft.BankSettingsDB db0 = new FloraSoft.BankSettingsDB();
            FloraSoft.BankSettings bs = db0.GetBankSettings();

            RTGSImporter.Pacs008 pacs8 = new RTGSImporter.Pacs008();
            RTGSImporter.Pacs004 pacs4 = new RTGSImporter.Pacs004();

            RTGSImporter.TeamRedDB db = new RTGSImporter.TeamRedDB();
            RTGSImporter.TeamGreenDB db4 = new RTGSImporter.TeamGreenDB();

            pacs8 = db.GetSingleInward08(InwardID);


            pacs4.FrBICFI = pacs8.CdtrAgtBICFI;
            pacs4.ToBICFI = "BBHOBDDHRTG";
            //pacs4.BizMsgIdr = ;
            pacs4.MsgDefIdr = "pacs.004.001.04";
            pacs4.BizSvc = "RTGS";
            pacs4.CreDt = Credt + "Z";
            //pacs4.MsgId = ;
            pacs4.CreDtTm = Credt;
            pacs4.NbOfTxs = 1;
            pacs4.OrgnlMsgId = pacs8.MsgId;
            pacs4.OrgnlMsgNmId = pacs8.MsgDefIdr;
            pacs4.OrgnlCreDtTm = pacs8.CreDtTm;
            //pacs4.RtrRsnOrgtrNm = ;
            pacs4.RtrRsnCd = ddlReturnReason.SelectedItem.Value;
            pacs4.RtrRsnPrtry = ddlReturnReason.SelectedItem.Text;
            pacs4.RtrRsnAddtlInf = "NONE";
            //pacs4.RtrId = ;
            pacs4.OrgnlInstrId = pacs8.InstrId;
            pacs4.OrgnlEndToEndId = pacs8.EndToEndId;
            pacs4.OrgnlTxId = pacs8.TxId;
            pacs4.RtrdIntrBkSttlmCcy = pacs8.Ccy;
            pacs4.RtrdIntrBkSttlmAmt = pacs8.IntrBkSttlmAmt;
            pacs4.TxInfIntrBkSttlmDt = pacs8.IntrBkSttlmDt;

            pacs4.ChrgBr = pacs8.ChrgBr;
            //pacs4.ChrgsInfBICFI = ;
            //pacs4.ChrgsInfNm = ;
            //pacs4.ChrgsInfBranchId = ;

            pacs4.InstgAgtBICFI = pacs8.InstdAgtBICFI;
            pacs4.InstdAgtBICFI = pacs8.InstgAgtBICFI;
            pacs4.TxRefIntrBkSttlmCcy = pacs8.Ccy;
            pacs4.TxRefIntrBkSttlmAmt = pacs8.IntrBkSttlmAmt;
            pacs4.TxRefIntrBkSttlmDt = pacs8.IntrBkSttlmDt;
            pacs4.SvcLvlPrtry = pacs8.SvcLvlPrtry;
            pacs4.LclInstrmPrtry = pacs8.LclInstrmPrtry;
            pacs4.CtgyPurpPrtry = pacs8.CtgyPurpPrtry;
            //pacs4.PmtMtd = ;
            //pacs4.RmtInfUstrd = pacs8.Ustrd;
            //pacs4.DbtrNm = pacs8.CdtrNm;
            //pacs4.DbtrNmPstlAdr = pacs8.CdtrPstlAdr;
            //pacs4.DbtrNmStrtNm = pacs8.CdtrStrtNm;
            //pacs4.DbtrNmTwnNm = pacs8.CdtrTwnNm;
            //pacs4.DbtrNmCtry = pacs8.CdtrCtry;
            ////pacs4.DbtrNmAdrLine = ;
            //pacs4.DbtrAcctId = pacs8.CdtrAcctOthrId;
            //pacs4.DbtrAcctTpPrtry = pacs8.CdtrAcctPrtry;
            //pacs4.DbtrAgtBICFINm = pacs8.CdtrAgtBICFI;
            //pacs4.DbtrAgtBICFI = pacs8.CdtrAgtBICFI;

            pacs4.RmtInfUstrd = pacs8.Ustrd;
            pacs4.DbtrNm = pacs8.DbtrNm;
            pacs4.DbtrNmPstlAdr = pacs8.DbtrPstlAdr;
            pacs4.DbtrNmStrtNm = pacs8.DbtrStrtNm;
            pacs4.DbtrNmTwnNm = pacs8.DbtrTwnNm;
            pacs4.DbtrNmCtry = pacs8.DbtrCtry;
            pacs4.DbtrAcctId = pacs8.DbtrAcctOthrId;
            pacs4.DbtrAcctTpPrtry = pacs8.DbtrAgtAcctPrtry;
            pacs4.DbtrAgtBICFINm = pacs8.DbtrAgtBICFI;
            pacs4.DbtrAgtBICFI = pacs8.DbtrAgtBICFI;
            pacs4.DbtrAgtBranchId = pacs8.DbtrAgtBranchId;
            pacs4.DbtrAgtAcctId = pacs8.DbtrAgtAcctOthrId;
            pacs4.DbtrAgtAcctPrtry = pacs8.DbtrAgtAcctPrtry;
            pacs4.CdtrAgtBICFI = pacs8.CdtrAgtBICFI;
            pacs4.CdtrAgtNm = pacs8.CdtrAgtNm;
            pacs4.CdtrAgtBranchId = pacs8.CdtrAgtBranchId;
            pacs4.CdtrAgtAcctId = pacs8.CdtrAgtAcctOthrId;
            pacs4.CdtrAgtAcctTpPrtry = pacs8.CdtrAgtAcctPrtry;
            pacs4.CdtrNm = pacs8.CdtrNm;
            pacs4.CdtrAdrLine = pacs8.CdtrAdrLine;
            pacs4.CdtrAcctId = pacs8.CdtrAcctOthrId;






            //pacs4.DbtrAgtBranchId = pacs8.CdtrAgtBranchId;
            //pacs4.DbtrAgtAcctId = pacs8.DbtrAgtAcctOthrId;
            //pacs4.DbtrAgtAcctPrtry = pacs8.DbtrAgtAcctPrtry;
            //pacs4.CdtrAgtBICFI = pacs8.DbtrAgtBICFI;
            //pacs4.CdtrAgtNm = pacs8.DbtrAgtNm;
            //pacs4.CdtrAgtBranchId = pacs8.DbtrAgtBranchId;
            //pacs4.CdtrAgtAcctId = pacs8.CdtrAgtAcctOthrId;
            //pacs4.CdtrAgtAcctTpPrtry = pacs8.DbtrAgtAcctPrtry;
            //pacs4.CdtrNm = pacs8.DbtrNm;
            //pacs4.CdtrAdrLine = pacs8.DbtrAdrLine;
            //pacs4.CdtrAcctId = pacs8.DbtrAcctOthrId;

            pacs4.InwardID = pacs8.PacsID;
            pacs4.DeptId = Int32.Parse(Request.Cookies["DeptID"].Value);
            pacs4.Maker = Request.Cookies["UserName"].Value;
            pacs4.MakerIP = HttpContext.Current.Request.UserHostAddress;

            db4.InsertOutward004(pacs4);

            db.ReturnInward08(pacs8.PacsID, Int32.Parse(Request.Cookies["DeptID"].Value), pacs4.Maker, pacs4.MakerIP);
        }

        private void ProcessPacs009()
        {
            string InwardID = Request.QueryString["InwardID"];

            FloraSoft.BankSettingsDB db0 = new FloraSoft.BankSettingsDB();
            FloraSoft.BankSettings bs = db0.GetBankSettings();

            RTGSImporter.Pacs009 pacs9 = new RTGSImporter.Pacs009();
            RTGSImporter.Pacs004 pacs4 = new RTGSImporter.Pacs004();

            RTGSImporter.TeamBlueDB db9 = new RTGSImporter.TeamBlueDB();
            RTGSImporter.TeamGreenDB db4 = new RTGSImporter.TeamGreenDB();

            pacs9 = db9.GetSingleInward09(InwardID);


            pacs4.FrBICFI = bs.BIC;
            pacs4.ToBICFI = "BBHOBDDHRTG";
            //pacs4.BizMsgIdr = ;
            pacs4.MsgDefIdr = "pacs.004.001.04";
            pacs4.BizSvc = "RTGS";
            pacs4.CreDt = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //pacs4.MsgId = ;
            pacs4.CreDtTm = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            pacs4.NbOfTxs = 1;
            pacs4.OrgnlMsgId = pacs9.MsgId;
            pacs4.OrgnlMsgNmId = pacs9.MsgDefIdr;
            pacs4.OrgnlCreDtTm = pacs9.CreDtTm;
            //pacs4.RtrRsnOrgtrNm = ;
          ///  pacs4.RtrRsnCd = ddlReturnReason.SelectedItem.Value;
            pacs4.RtrRsnPrtry = ddlReturnReason.SelectedItem.Text;
            pacs4.RtrRsnAddtlInf = "NONE";
            //pacs4.RtrId = ;
            pacs4.OrgnlInstrId = pacs9.InstrId;
            pacs4.OrgnlEndToEndId = pacs9.EndToEndId;
            pacs4.OrgnlTxId = pacs9.TxId;
            pacs4.RtrdIntrBkSttlmCcy = pacs9.IntrBkSttlmCcy;
            pacs4.RtrdIntrBkSttlmAmt = pacs9.IntrBkSttlmAmt;
            pacs4.TxInfIntrBkSttlmDt = pacs9.IntrBkSttlmDt;

            //pacs4.ChrgBr = pacs9.;
            //pacs4.ChrgsInfBICFI = ;
            //pacs4.ChrgsInfNm = ;
            //pacs4.ChrgsInfBranchId = ;

            pacs4.InstgAgtBICFI = pacs9.InstdAgtBICFI;
            pacs4.InstdAgtBICFI = pacs9.InstgAgtBICFI;
            pacs4.TxRefIntrBkSttlmCcy = pacs9.IntrBkSttlmCcy;
            pacs4.TxRefIntrBkSttlmAmt = pacs9.IntrBkSttlmAmt;
            pacs4.TxRefIntrBkSttlmDt = pacs9.IntrBkSttlmDt;
            pacs4.SvcLvlPrtry = Int32.Parse(pacs9.SvcLvlPrtry);
            pacs4.LclInstrmPrtry = pacs9.LclInstrmPrtry;
            pacs4.CtgyPurpPrtry = pacs9.CtgyPurpPrtry;
            //pacs4.PmtMtd = ;
            //pacs4.RmtInfUstrd = pacs9.;
            pacs4.DbtrNm = pacs9.CdtrNm;
            //pacs4.DbtrNmPstlAdr = pacs9.;
            //pacs4.DbtrNmStrtNm = pacs9.CdtrN;
            //pacs4.DbtrNmTwnNm = pacs9.CdtrTwnNm;
            //pacs4.DbtrNmCtry = pacs9.CdtrCtry;
            //pacs4.DbtrNmAdrLine = ;
            pacs4.DbtrAcctId = pacs9.CdtrAcctId;
            pacs4.DbtrAcctTpPrtry = pacs9.CdtrAcctTp;
            pacs4.DbtrAgtBICFINm = pacs9.CdtrAgtBICFI;
            pacs4.DbtrAgtBICFI = pacs9.CdtrAgtBICFI;
            pacs4.DbtrAgtBranchId = pacs9.CdtrAgtBranchId;
            if (pacs4.DbtrAgtBranchId == "")
            {
                pacs4.DbtrAgtBranchId = pacs9.CdtrBranchId;
            }
            pacs4.DbtrAgtAcctId = pacs9.CdtrAgtAcctId;
            pacs4.DbtrAgtAcctPrtry = pacs9.CdtrAgtAcctTp;
            pacs4.CdtrAgtBICFI = pacs9.DbtrBICFI;
            pacs4.CdtrAgtNm = pacs9.CdtrAgtBICFI;
            pacs4.CdtrAgtBranchId = pacs9.CdtrAgtBranchId;
            //pacs4.CdtrAgtAcctId = pacs9.D;
            //pacs4.CdtrAgtAcctTpPrtry = pacs9.DbtrAgtAcctPrtry;
            pacs4.CdtrNm = pacs9.DbtrNm;
            //pacs4.CdtrAdrLine = pacs9.DbtrAdrLine;
            pacs4.CdtrAcctId = pacs9.DbtrAcctId;
            //pacs4.CdtrAgtAcctTpPrtry = pacs9.;
            pacs4.DeptId = Int32.Parse(Request.Cookies["DeptID"].Value); 
            pacs4.Maker = Request.Cookies["UserName"].Value;
            pacs4.MakerIP = HttpContext.Current.Request.UserHostAddress;

           /// db4.InsertOutward004(pacs4);

            db9.ReturnInward09(pacs9.PacsID, Int32.Parse(Request.Cookies["DeptID"].Value), pacs4.Maker, pacs4.MakerIP);
        }

        protected void btnCancelTrans_Click(object sender, EventArgs e)
        {
            Response.Redirect("../InwardList.aspx");
        }

    }
}