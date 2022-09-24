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
    public partial class PaymentReturnChecker : System.Web.UI.Page
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
            string OutwardID = Request.QueryString["OutwardID"];

            RTGSImporter.TeamGreenDB db = new RTGSImporter.TeamGreenDB();
            RTGSImporter.Pacs004 pacs = db.GetSingleOutward04(OutwardID);

            lblAccountNo.Text           = pacs.CdtrAcctId;
            lblSettlmentAmount.Text     = string.Format("{0:N}", pacs.TxRefIntrBkSttlmAmt);
            lblCCY.Text                 = pacs.TxRefIntrBkSttlmCcy;
            lblReceivingBank.Text       = pacs.DbtrAgtBICFINm;
            lblReceivingBranch.Text     = pacs.FrBranch;
            lblReceiverName.Text        = pacs.DbtrNm;
            lblReceiverAccountNo.Text   = pacs.DbtrAcctId;
            lblReturnReason.Text        = pacs.RtrRsnPrtry;

            lblMsg.Text = "<a target=\"_new\" href=\"Inward08Long.aspx?InwardID=" + pacs.InwardID + "\">REVERSAL OF " + pacs.OrgnlMsgId + "</a>";
            
            string RoleCD = Request.Cookies["RoleCD"].Value;
            Decimal TransLimit = Decimal.Parse(Request.Cookies["TransLimit"].Value);

            if ((RoleCD == "RTCK") && (pacs.StatusID == 2))
            {
                ButtonPanel.Visible = true;
            }
            if ((RoleCD == "RTAU") && (pacs.StatusID == 3))
            {
                ButtonPanel.Visible = true;
            }
            if (TransLimit < pacs.TxRefIntrBkSttlmAmt)
            {
                ButtonPanel.Visible = false;
            }
        }
        protected void btnSend_Click(object sender, EventArgs e)
        {
            string OutwardID = Request.Params["OutwardID"];
            string RoleCD = Request.Cookies["RoleCD"].Value;

            RTGSImporter.TeamGreenDB db = new RTGSImporter.TeamGreenDB();
            if (RoleCD == "RTCK")
            {
                db.ApproveOutward04(OutwardID, Request.Cookies["UserName"].Value, HttpContext.Current.Request.UserHostAddress);
            }
            if (RoleCD == "RTAU")
            {
                db.AuthOutward04(OutwardID, Request.Cookies["UserName"].Value, HttpContext.Current.Request.UserHostAddress);
            }
            Response.Redirect("../OutwardListChecker.aspx");
        }
        protected void btnCancelTrans_Click(object sender, EventArgs e)
        {
            Response.Redirect("../OutwardListChecker.aspx");
        }
        protected void btnRejectTransaction_Click(object sender, EventArgs e)
        {
            RTGSImporter.TeamGreenDB db = new RTGSImporter.TeamGreenDB();
            db.RejectOutward04(Request.QueryString["OutwardID"], Request.Cookies["UserName"].Value, HttpContext.Current.Request.UserHostAddress);
            Response.Redirect("../OutwardListChecker.aspx");
        }
    }
}