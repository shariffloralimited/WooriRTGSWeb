using RTGSImporter;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace RTGS.Forms
{
    public partial class Inward09Long : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!base.IsPostBack)
            {
                this.LoadData();
            }
        }

        private void LoadData()
        {
            string inwardID = base.Request.Params["InwardID"];
            TeamBlueDB teamBlueDB = new TeamBlueDB();
            Pacs009 singleInward = teamBlueDB.GetSingleInward09(inwardID);
            this.lblFrBICFI.Text = singleInward.FrBICFI;
            this.lblToBICFI.Text = singleInward.ToBICFI;
            this.lblBizMsgIdr.Text = singleInward.BizMsgIdr;
            this.lblMsgDefIdr.Text = singleInward.MsgDefIdr;
            this.lblBizSvc.Text = singleInward.BizSvc;
            this.lblCreDt.Text = singleInward.CreDt;
            this.lblMsgId.Text = singleInward.MsgId;
            this.lblCreDtTm.Text = singleInward.CreDtTm;
            this.lblNbOfTxs.Text = singleInward.NbOfTxs.ToString();
            this.lblInstgAgtBICFI.Text = singleInward.InstgAgtBICFI;
            this.lblInstgAgtNm.Text = singleInward.InstgAgtNm;
            this.lblInstgAgtBranchId.Text = singleInward.InstgAgtBranchId;
            this.lblInstdAgtBICFI.Text = singleInward.InstdAgtBICFI;
            this.lblInstdAgtNm.Text = singleInward.InstdAgtNm;
            this.lblInstdAgtBranchId.Text = singleInward.InstdAgtBranchId;
            this.lblIntrmyAgt1BICFI.Text = singleInward.IntrmyAgt1BICFI;
            this.lblIntrmyAgt1Nm.Text = singleInward.IntrmyAgt1Nm;
            this.lblIntrmyAgt1BranchId.Text = singleInward.IntrmyAgt1BranchId;
            this.lblIntrmyAgt1AcctId.Text = singleInward.IntrmyAgt1AcctId;
            this.lblIntrmyAgt1AcctTp.Text = singleInward.IntrmyAgt1AcctTp;
            this.lblLclInstrmPrtry.Text = singleInward.LclInstrmPrtry;
            this.lblSvcLvlPrtry.Text = singleInward.SvcLvlPrtry;
            this.lblCtgyPurpPrtry.Text = singleInward.CtgyPurpPrtry;
            this.lblInstrId.Text = singleInward.InstrId;
            this.lblTxId.Text = singleInward.TxId;
            this.lblEndToEndId.Text = singleInward.EndToEndId;
            this.lblIntrBkSttlmCcy.Text = singleInward.IntrBkSttlmCcy;
            this.lblIntrBkSttlmAmt.Text = singleInward.IntrBkSttlmAmt.ToString();
            this.lblIntrBkSttlmDt.Text = singleInward.IntrBkSttlmDt;
            this.lblDbtrBICFI.Text = singleInward.DbtrBICFI;
            this.lblDbtrNm.Text = singleInward.DbtrNm;
            this.lblDbtrBranchId.Text = singleInward.DbtrBranchId;
            this.lblDbtrAcctId.Text = singleInward.DbtrAcctId;
            this.lblDbtrAcctTp.Text = singleInward.DbtrAcctTp;
            this.lblCdtrAgtBICFI.Text = singleInward.CdtrAgtBICFI;
            this.lblCdtrAgtBranchId.Text = singleInward.CdtrAgtBranchId;
            this.lblCdtrAgtAcctId.Text = singleInward.CdtrAgtAcctId;
            this.lblCdtrAgtAcctTp.Text = singleInward.CdtrAgtAcctTp;
            this.lblCdtrBICFI.Text = singleInward.CdtrBICFI;
            this.lblCdtrNm.Text = singleInward.CdtrNm;
            this.lblCdtrBranchId.Text = singleInward.CdtrBranchId;
            this.lblCdtrAcctId.Text = singleInward.CdtrAcctId;
            this.lblCdtrAcctTp.Text = singleInward.CdtrAcctTp;
            this.lblInstrInf.Text = singleInward.InstrInf;
            this.lblPmntRsn.Text = singleInward.PmntRsn;
            string value = base.Request.Cookies["RoleCD"].Value;
            if (value == "RTMK" && singleInward.StatusID == 3)
            {
                this.ButtonPanel.Visible = true;
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("Outward09ShortMaker.aspx?FormName=Pacs.009&InwardID=" + base.Request.Params["InwardID"]);
        }

        protected void btnCancelTrans_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("../InwardList.aspx");
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            TeamBlueDB teamBlueDB = new TeamBlueDB();
            string inwardID = base.Request.Params["InwardID"];
            string text = this.lblCdtrAcctId.Text;
            if (text != "")
            {
                teamBlueDB.TransferInward09(inwardID, text, base.Request.Cookies["UserName"].Value, HttpContext.Current.Request.UserHostAddress);
                base.Response.Redirect("../InwardList.aspx");
            }
        }
    }
}

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                LoadData();
//            }
//        }
//        private void LoadData()
//        {
//            string InwardID = Request.Params["InwardID"];

//            RTGSImporter.TeamBlueDB db = new RTGSImporter.TeamBlueDB();
//            RTGSImporter.Pacs009 pacs = db.GetSingleInward09(InwardID);


//            lblFrBICFI.Text = pacs.FrBICFI;
//            lblToBICFI.Text = pacs.ToBICFI;
//            lblBizMsgIdr.Text = pacs.BizMsgIdr;
//            lblMsgDefIdr.Text = pacs.MsgDefIdr;
//            lblBizSvc.Text = pacs.BizSvc;
//            lblCreDt.Text = pacs.CreDt;
//            lblMsgId.Text = pacs.MsgId;
//            lblCreDtTm.Text = pacs.CreDtTm;
//            lblNbOfTxs.Text = pacs.NbOfTxs.ToString();
//            lblInstgAgtBICFI.Text = pacs.InstgAgtBICFI;
//            lblInstgAgtNm.Text = pacs.InstgAgtNm;
//            lblInstgAgtBranchId.Text = pacs.InstgAgtBranchId;
//            lblInstdAgtBICFI.Text = pacs.InstdAgtBICFI;
//            lblInstdAgtNm.Text = pacs.InstdAgtNm;
//            lblInstdAgtBranchId.Text = pacs.InstdAgtBranchId;
//            lblIntrmyAgt1BICFI.Text = pacs.IntrmyAgt1BICFI;
//            lblIntrmyAgt1Nm.Text = pacs.IntrmyAgt1Nm;
//            lblIntrmyAgt1BranchId.Text = pacs.IntrmyAgt1BranchId;
//            lblIntrmyAgt1AcctId.Text = pacs.IntrmyAgt1AcctId;
//            lblIntrmyAgt1AcctTp.Text = pacs.IntrmyAgt1AcctTp;
//            lblLclInstrmPrtry.Text = pacs.LclInstrmPrtry;
//            lblSvcLvlPrtry.Text = pacs.SvcLvlPrtry;
//            lblCtgyPurpPrtry.Text = pacs.CtgyPurpPrtry;
//            lblInstrId.Text = pacs.InstrId;
//            lblTxId.Text = pacs.TxId;
//            lblEndToEndId.Text = pacs.EndToEndId;
//            lblIntrBkSttlmCcy.Text = pacs.IntrBkSttlmCcy;
//            lblIntrBkSttlmAmt.Text = pacs.IntrBkSttlmAmt.ToString();
//            lblIntrBkSttlmDt.Text = pacs.IntrBkSttlmDt;
//            lblDbtrBICFI.Text = pacs.DbtrBICFI;
//            lblDbtrNm.Text = pacs.DbtrNm;
//            lblDbtrBranchId.Text = pacs.DbtrBranchId;
//            lblDbtrAcctId.Text = pacs.DbtrAcctId;
//            lblDbtrAcctTp.Text = pacs.DbtrAcctTp;
//            lblCdtrAgtBICFI.Text = pacs.CdtrAgtBICFI;
//            lblCdtrAgtBranchId.Text = pacs.CdtrAgtBranchId;
//            lblCdtrAgtAcctId.Text = pacs.CdtrAgtAcctId;
//            lblCdtrAgtAcctTp.Text = pacs.CdtrAgtAcctTp;
//            lblCdtrBICFI.Text = pacs.CdtrBICFI;
//            lblCdtrNm.Text = pacs.CdtrNm;
//            lblCdtrBranchId.Text = pacs.CdtrBranchId;
//            lblCdtrAcctId.Text = pacs.CdtrAcctId;
//            lblCdtrAcctTp.Text = pacs.CdtrAcctTp;
//            lblInstrInf.Text = pacs.InstrInf;
//            lblPmntRsn.Text = pacs.PmntRsn;

//            string RoleCD = Request.Cookies["RoleCD"].Value;
//            if ((RoleCD == "RTMK") && (pacs.StatusID == 3))
//            {
//                ButtonPanel.Visible = true;
//            }
//        }

//        protected void btnSend_Click(object sender, EventArgs e)
//        {
//            Response.Redirect("Outward09ShortMaker.aspx?FormName=Pacs.009&InwardID=" + Request.Params["InwardID"]);
//        }

//        protected void btnCancelTrans_Click(object sender, EventArgs e)
//        {
//            Response.Redirect("../InwardList.aspx");
//        }

//        protected void btnReturn_Click(object sender, EventArgs e)
//        {
//            RTGSImporter.TeamBlueDB db = new RTGSImporter.TeamBlueDB();
//            string InwardID = Request.Params["InwardID"];
//            //string NewAccountNo = txtNewAccountNo.Text;
//            string NewAccountNo = lblCdtrAcctId.Text;

//            if (NewAccountNo != "")
//            {
//                db.TransferInward09(InwardID, NewAccountNo, Request.Cookies["UserName"].Value, HttpContext.Current.Request.UserHostAddress);
//                Response.Redirect("../InwardList.aspx");
//            }
//        }

//    }
//}