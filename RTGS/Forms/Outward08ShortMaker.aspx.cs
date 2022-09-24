using FloraSoft;
using FMPS.BLL;
using RTGS.RTGSWS;
using RTGSImporter;
using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace RTGS.Forms
{
    public partial class Outward08ShortMaker : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (!Global.cancontinue)
                {
                    HttpContext.Current.Response.End();
                }
            }
            catch
            {
                HttpContext.Current.Response.End();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BankSettingsDB bankSettingsDB = new BankSettingsDB();
            BankSettings bankSettings = bankSettingsDB.GetBankSettings();
            if (bankSettings.SkipCBS)
            {
                this.ActNameDiv.Visible = true;
                this.btnGetInfo.Visible = false;
            }
            else
            {
                this.ActNameDiv.Visible = false;
                this.btnGetInfo.Visible = true;
            }
            if (base.Request.Cookies["RoleCD"].Value != "RTMK")
            {
                base.Response.Redirect("../AccessDenied.aspx");
            }
            if (!base.IsPostBack)
            {
                this.CustomDutyPanel.Visible = false;
                this.UnstructDiv.Visible = true;
            }
            if (!base.IsPostBack)
            {
                this.BindTransType();
                this.BindSendBranch();
                this.Banks();
                this.BindBranches(this.ddListReceivingBank.SelectedValue);
            }
            this.lblMsg.Text = "";
        }

        private bool ValidateCBSAccount()
        {
            string value = base.Request.Cookies["BranchCD"].Value;
            string b = this.txtAccountNo.Text.Trim().Substring(0, 3);
            string selectedValue = this.ddlCurrency.SelectedValue;
            bool result;
            if (selectedValue != "BDT" && value != b)
            {
                this.lblMsg.Text = "Foreign Currency transaction is not allowed from this Branch";
                result = false;
            }
            else if (this.lblCCY.Text == "")
            {
                this.lblMsg.Text = "Account Verification Needed.";
                result = false;
            }
            else if (this.lblAccountNo.Text != this.txtAccountNo.Text)
            {
                this.lblMsg.Text = "Account Verification Needed.";
                result = false;
            }
            else if (this.lblCCY.Text != this.ddlCurrency.SelectedValue)
            {
                this.lblMsg.Text = "Wrong Currency.";
                result = false;
            }
            else
            {
                decimal d = decimal.Parse(this.lblCurrentBalance.Text);
                decimal d2 = decimal.Parse(this.txtSettlmentAmount.Text);
                if (d2 > d)
                {
                    this.lblMsg.Text = "Insufficient Balance.";
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            BankSettingsDB bankSettingsDB = new BankSettingsDB();
            BankSettings bankSettings = bankSettingsDB.GetBankSettings();
            if (!bankSettings.SkipCBS)
            {
                if (!this.ValidateCBSAccount())
                {
                    return;
                }
            }
            if (this.Page.IsValid)
            {
                if (this.ddlCtgyPurpPrtry.SelectedValue != "031" && this.ddlCtgyPurpPrtry.SelectedItem.Value != "041" && this.ddlCtgyPurpPrtry.SelectedItem.Value != "040")
                {
                    CCYDB cCYDB = new CCYDB();
                    decimal minLimit = cCYDB.GetMinLimit(this.ddlCurrency.SelectedValue, "pacs.008");
                    decimal d = decimal.Parse(this.txtSettlmentAmount.Text);
                    if (d < minLimit)
                    {
                        this.lblMsg.Text = "Sending amount is less then Minimum Limit";
                        return;
                    }
                    if (d > new decimal(100000000000L))
                    {
                        this.lblMsg.Text = "Maximum Amount: 1000 Crore.";
                        return;
                    }
                }
                else if (!this.ChkChargeWaived.Checked)
                {
                    this.lblMsg.Text = "Charges must be waived for Govt Payment, Custom Duty Payment, TAX Payment And VAT Payment";
                    return;
                }
                Pacs008 pacs = new Pacs008();
                string text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                pacs.FrBICFI = bankSettings.BIC;
                pacs.ToBICFI = "BBHOBDDHRTG";
                pacs.MsgDefIdr = "pacs.008.001.04";
                pacs.BizSvc = "RTGS";
                pacs.CreDt = text + "Z";
                pacs.CreDtTm = text;
                pacs.BtchBookg = "false";
                pacs.NbOfTxs = 1;
                pacs.ClrChanl = "RTGS";
                pacs.SvcLvlPrtry = 75;
                pacs.LclInstrmPrtry = "RTGS_SSCT";
                pacs.CtgyPurpPrtry = this.ddlCtgyPurpPrtry.SelectedValue;
                pacs.Ccy = this.ddlCurrency.SelectedValue;
                pacs.IntrBkSttlmAmt = decimal.Parse(this.txtSettlmentAmount.Text);
                pacs.IntrBkSttlmDt = DateTime.Today.ToString("yyyy-MM-dd");
                pacs.ChrgBr = this.radioChargeBearer1.SelectedValue;
                pacs.InstgAgtBICFI = bankSettings.BIC;
                pacs.InstgAgtNm = bankSettings.BIC;
                pacs.InstgAgtBranchId = this.ddlSendBranch.SelectedValue;
                pacs.InstdAgtBICFI = this.ddListReceivingBank.SelectedValue;
                pacs.InstdAgtNm = this.ddListReceivingBank.SelectedValue;
                pacs.InstdAgtBranchId = this.ddListBranch.SelectedValue;
                if (!bankSettings.SkipCBS)
                {
                    pacs.DbtrNm = this.lblAccountName.Text;
                }
                else
                {
                    pacs.DbtrNm = this.txtAccountName.Text;
                }
                pacs.DbtrAcctOthrId = this.txtAccountNo.Text;
                pacs.DbtrAgtBICFI = bankSettings.BIC;
                pacs.DbtrAgtNm = bankSettings.BIC;
                pacs.DbtrAgtBranchId = this.ddlSendBranch.SelectedValue;
                pacs.CdtrAgtBICFI = this.ddListReceivingBank.SelectedValue;
                pacs.CdtrAgtNm = this.ddListReceivingBank.SelectedValue;
                pacs.CdtrAgtBranchId = this.ddListBranch.SelectedValue;
                pacs.CdtrNm = this.txtReceiverName.Text;
                pacs.CdtrAcctOthrId = this.txtReceiverAccountNo.Text;
                if (pacs.CtgyPurpPrtry == "041")
                {
                    pacs.Ustrd = string.Concat(new string[]
					{
						this.txtCustomOfficeCD.Text,
						" ",
						this.txtRegYr.Text,
						" ",
						this.txRegNumber.Text,
						" ",
						this.txtDeclarantCD.Text,
						" ",
						this.txtCustomerMobile.Text
					});
                }
                else
                {
                    pacs.Ustrd = this.txtReasonForPayment.Text;
                }
                pacs.DeptId = int.Parse(base.Request.Cookies["DeptID"].Value);
                pacs.Maker = base.Request.Cookies["UserName"].Value;
                pacs.MakerIP = HttpContext.Current.Request.UserHostAddress;
                pacs.BrnchCD = base.Request.Cookies["BranchCD"].Value;
                pacs.ChargeWaived = this.ChkChargeWaived.Checked;
                TeamRedDB teamRedDB = new TeamRedDB();
                string str = teamRedDB.InsertOutward008(pacs);
                base.Response.Redirect("Outward08LongMaker.aspx?OutwardID=" + str);
            }
        }

        protected void ddListReceivingBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindBranches(this.ddListReceivingBank.SelectedValue);
        }

        protected void ddListBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtRoutingNo.Text = this.ddListBranch.SelectedValue;
        }

        private void Banks()
        {
            BanksDB banksDB = new BanksDB();
            this.ddListReceivingBank.DataSource = banksDB.GetSendBanks();
            this.ddListReceivingBank.DataBind();
        }

        private void BindSendBranch()
        {
            BranchesDB branchesDB = new BranchesDB();
            this.ddlSendBranch.DataSource = branchesDB.GetSendBranches();
            this.ddlSendBranch.DataBind();
            if (base.Request.Cookies["AllBranch"].Value != "False")
            {
                this.ddlSendBranch.SelectedValue = "0";
            }
            else
            {
                this.ddlSendBranch.SelectedValue = base.Request.Cookies["RoutingNo"].Value;
                this.ddlSendBranch.Enabled = false;
            }
        }

        private void BindBranches(string BIC)
        {
            BranchesDB branchesDB = new BranchesDB();
            this.ddListBranch.DataSource = branchesDB.GetBranchesByBIC(BIC);
            this.ddListBranch.DataBind();
            this.txtRoutingNo.Text = this.ddListBranch.SelectedValue;
        }

        protected void btnGetInfo_Click(object sender, EventArgs e)
        {
            CustomerInfo customerInfo = new CustomerInfo();
            decimal amount = 0m;
            try
            {
                amount = decimal.Parse(this.txtSettlmentAmount.Text);
            }
            catch
            {
            }
            Service1 service = new Service1();
            string accountInfo = service.GetAccountInfo(this.txtAccountNo.Text, amount, this.ddlCurrency.SelectedValue);
            DataTable customerTable = customerInfo.GetCustomerTable(accountInfo);
            this.AccountInfoGrid.DataSource = customerTable;
            this.AccountInfoGrid.DataBind();
            if (customerTable.Rows.Count > 4)
            {
                this.lblAccountNo.Text = customerTable.Rows[0][1].ToString();
                this.lblAccountName.Text = customerTable.Rows[1][1].ToString();
                this.lblCCY.Text = customerTable.Rows[2][1].ToString();
                this.lblCurrentBalance.Text = customerTable.Rows[3][1].ToString();
            }
            customerTable.Dispose();
        }

        protected void txtSettlmentAmount_txtChanged(object sender, EventArgs e)
        {
            NumberToWordConverter numberToWordConverter = new NumberToWordConverter();
            try
            {
                this.lblAmount.Text = numberToWordConverter.GetAmountInWords(this.txtSettlmentAmount.Text.Replace(",", ""));
            }
            catch
            {
            }
        }

        protected void btnCancelTrans_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("../OutwardListMaker.aspx");
        }

        private void BindTransType()
        {
            TransCodeDB transCodeDB = new TransCodeDB();
            this.ddlCtgyPurpPrtry.DataSource = transCodeDB.GetTransCode("Pacs08");
            this.ddlCtgyPurpPrtry.DataBind();
        }

        protected void ddlCtgyPurpPrtry_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (this.ddlCtgyPurpPrtry.SelectedValue == "041")
            //{
            //    this.CustomDutyPanel.Visible = true;
            //    this.UnstructDiv.Visible = false;
            //    this.ddListReceivingBank.SelectedValue = ConfigurationManager.AppSettings["CustomDutyReceivingBankBIC"];
            //    this.BindBranches(this.ddListReceivingBank.SelectedValue);
            //    this.ddListBranch.SelectedValue = ConfigurationManager.AppSettings["CustomDutyReceivingBranch"];
            //    this.txtReceiverName.Text = ConfigurationManager.AppSettings["CustomDutyReceiverName"];
            //    this.txtReceiverAccountNo.Text = ConfigurationManager.AppSettings["CustomDutyReceiverAccountNo"];
            //}
            //else
            //{
                this.CustomDutyPanel.Visible = false;
                this.UnstructDiv.Visible = true;
                this.ddListReceivingBank.SelectedIndex = 0;
                this.BindBranches(this.ddListReceivingBank.SelectedValue);
                this.txtReceiverName.Text = "";
                this.txtReceiverAccountNo.Text = "";
            //}
        }
    }
}
//        protected void Page_Init(object sender, EventArgs e)
//        {
//            try
//            {
//                if (!Global.cancontinue)
//                {
//                    HttpContext.Current.Response.End();
//                }
//            }
//            catch
//            {
//                HttpContext.Current.Response.End();
//            }
//        }

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            FloraSoft.BankSettingsDB db0 = new FloraSoft.BankSettingsDB();
//            FloraSoft.BankSettings bs = db0.GetBankSettings();

//            if (bs.SkipCBS == true)
//            {
//                ActNameDiv.Visible = true;
//                btnGetInfo.Visible = false;
//            }
//            else
//            {
//                ActNameDiv.Visible = false;
//                btnGetInfo.Visible = true;
//            }

//            if (Request.Cookies["RoleCD"].Value != "RTMK")
//            {
//                Response.Redirect("../AccessDenied.aspx");
//            }

//            if (!IsPostBack)
//            {
//                CustomDutyPanel.Visible = false;
//                UnstructDiv.Visible = true;
//            }

//            if (!IsPostBack)
//            {
//                BindTransType();
//                BindSendBranch();
//                Banks();
//                BindBranches(ddListReceivingBank.SelectedValue);
//            }

//            lblMsg.Text = "";
//        }

//        private bool ValidateCBSAccount()
//        {
//            string branchCD = Request.Cookies["BranchCD"].Value;
//            string AcctNo = txtAccountNo.Text.Trim().Substring(0, 3);
//            string CCY = ddlCurrency.SelectedValue;

//            if ((CCY != "BDT") && (branchCD != AcctNo))
//            {
//                lblMsg.Text = "Foreign Currency transaction is not allowed from this Branch";
//                return false;
//            }
//            if (lblCCY.Text == "")
//            {
//                lblMsg.Text = "Account Verification Needed.";
//                return false;
//            }
//            if (lblAccountNo.Text != txtAccountNo.Text)
//            {
//                lblMsg.Text = "Account Verification Needed.";
//                return false;
//            }
//            if (lblCCY.Text != ddlCurrency.SelectedValue)
//            {
//                lblMsg.Text = "Wrong Currency.";
//                return false;
//            }
//            Decimal balance = Decimal.Parse(lblCurrentBalance.Text);
//            Decimal sendingamount = Decimal.Parse(txtSettlmentAmount.Text);
//            if (sendingamount > balance)
//            {
//                lblMsg.Text = "Insufficient Balance.";
//                return false;
//            }
//            return true;
//        }

//        protected void btnSave_Click(object sender, EventArgs e)
//        {
//            FloraSoft.BankSettingsDB db0 = new FloraSoft.BankSettingsDB();
//            FloraSoft.BankSettings bs = db0.GetBankSettings();
//            if (bs.SkipCBS != true)
//            {
//                if (!ValidateCBSAccount())
//                {
//                    return;
//                }
//            }
//            if (!Page.IsValid)
//            {
//                return;
//            }
//            if ((ddlCtgyPurpPrtry.SelectedValue != "031") && (ddlCtgyPurpPrtry.SelectedItem.Value != "041") && (ddlCtgyPurpPrtry.SelectedItem.Value != "040"))
//            {
//                CCYDB dbccy = new CCYDB();
//                decimal minlimit = dbccy.GetMinLimit(ddlCurrency.SelectedValue, "pacs.008");
//                decimal sttlmmtAmt = Decimal.Parse(txtSettlmentAmount.Text);
//                if (sttlmmtAmt < minlimit)
//                {
//                    lblMsg.Text = "Sending amount is less then Minimum Limit";
//                    return;
//                }

//                if (sttlmmtAmt > (decimal)100000000000.00)
//                {
//                    lblMsg.Text = "Maximum Amount: 1000 Crore.";
//                    return;
//                }
//            }
//            else
//            {
//                if (!ChkChargeWaived.Checked)
//                {
//                    lblMsg.Text = "Charges must be waived for Govt Payment, Custom Duty Payment, TAX Payment And VAT Payment";
//                    return;
//                }
//            }


//            RTGSImporter.Pacs008 pacs = new RTGSImporter.Pacs008();
//            string Credt = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

//            pacs.FrBICFI = bs.BIC;
//            pacs.ToBICFI = "BBHOBDDHRTG";
//            pacs.MsgDefIdr = "pacs.008.001.04";
//            pacs.BizSvc = "RTGS";
//            pacs.CreDt = Credt + "Z";
//            pacs.CreDtTm = Credt;
//            pacs.BtchBookg = "false";
//            pacs.NbOfTxs = 1;
//            pacs.ClrChanl = "RTGS";
//            pacs.SvcLvlPrtry = 75;
//            pacs.LclInstrmPrtry = "RTGS_SSCT";
//            pacs.CtgyPurpPrtry = ddlCtgyPurpPrtry.SelectedValue;
//            pacs.Ccy = ddlCurrency.SelectedValue;
//            pacs.IntrBkSttlmAmt = Decimal.Parse(txtSettlmentAmount.Text);
//            pacs.IntrBkSttlmDt = System.DateTime.Today.ToString("yyyy-MM-dd");
//            pacs.ChrgBr = radioChargeBearer1.SelectedValue;
//            pacs.InstgAgtBICFI = bs.BIC;
//            pacs.InstgAgtNm = bs.BIC;
//            pacs.InstgAgtBranchId = ddlSendBranch.SelectedValue;
//            pacs.InstdAgtBICFI = ddListReceivingBank.SelectedValue;
//            pacs.InstdAgtNm = ddListReceivingBank.SelectedValue;
//            pacs.InstdAgtBranchId = ddListBranch.SelectedValue;


//            if (bs.SkipCBS != true)
//            {
//                pacs.DbtrNm = lblAccountName.Text;
//            }
//            else
//            {
//                pacs.DbtrNm = txtAccountName.Text;
//            }
//            pacs.DbtrAcctOthrId = txtAccountNo.Text; //.Replace(".","");
//            pacs.DbtrAgtBICFI = bs.BIC;
//            pacs.DbtrAgtNm = bs.BIC;
//            pacs.DbtrAgtBranchId = ddlSendBranch.SelectedValue;
//            pacs.CdtrAgtBICFI = ddListReceivingBank.SelectedValue;
//            pacs.CdtrAgtNm = ddListReceivingBank.SelectedValue;
//            pacs.CdtrAgtBranchId = ddListBranch.SelectedValue;
//            pacs.CdtrNm = txtReceiverName.Text;
//            pacs.CdtrAcctOthrId = txtReceiverAccountNo.Text;
//            if (pacs.CtgyPurpPrtry == "041")
//            {
//                pacs.Ustrd = txtCustomOfficeCD.Text + " " + txtRegYr.Text + " " + txRegNumber.Text + " " + txtDeclarantCD.Text + " " + txtCustomerMobile.Text;
//            }
//            else
//            {
//                pacs.Ustrd = txtReasonForPayment.Text;
//            }
//            pacs.DeptId = Int32.Parse(Request.Cookies["DeptID"].Value);
//            pacs.Maker = Request.Cookies["UserName"].Value;
//            pacs.MakerIP = HttpContext.Current.Request.UserHostAddress;
//            pacs.BrnchCD = Request.Cookies["BranchCD"].Value;
//            pacs.ChargeWaived = ChkChargeWaived.Checked;

//            RTGSImporter.TeamRedDB db = new RTGSImporter.TeamRedDB();
//            string OutwardID = db.InsertOutward008(pacs);

//            Response.Redirect("Outward08LongMaker.aspx?OutwardID=" + OutwardID);
//        }

//        protected void ddListReceivingBank_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            BindBranches(ddListReceivingBank.SelectedValue);

//        }

//        protected void ddListBranch_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            txtRoutingNo.Text = ddListBranch.SelectedValue;
//        }

//        private void Banks()
//        {
//            BanksDB bankDB = new BanksDB();
//            ddListReceivingBank.DataSource = bankDB.GetSendBanks();
//            ddListReceivingBank.DataBind();
//        }

//        private void BindSendBranch()
//        {
//            BranchesDB db = new BranchesDB();
//            ddlSendBranch.DataSource = db.GetSendBranches();
//            ddlSendBranch.DataBind();
//            if (Request.Cookies["AllBranch"].Value != "False")
//            {
//                ddlSendBranch.SelectedValue = "0";
//            }
//            else
//            {
//                ddlSendBranch.SelectedValue = Request.Cookies["RoutingNo"].Value;
//                ddlSendBranch.Enabled = false;
//            }
//        }

//        private void BindBranches(string BIC)
//        {
//            BranchesDB db = new BranchesDB();
//            ddListBranch.DataSource = db.GetBranchesByBIC(BIC);
//            ddListBranch.DataBind();
//            txtRoutingNo.Text = ddListBranch.SelectedValue;
//        }

//        protected void btnGetInfo_Click(object sender, EventArgs e)
//        {
//            //Dictionary<string, string> names = new Dictionary<string, string>();
//            CustomerInfo ci = new CustomerInfo();
//            decimal amnt = 0;
//            try
//            {
//                amnt = Decimal.Parse(txtSettlmentAmount.Text);
//            }
//            catch { }

//            RTGSWS.Service1 ws = new RTGSWS.Service1();
//            string str = ws.GetAccountInfo(txtAccountNo.Text, amnt, ddlCurrency.SelectedValue);
//            //names = Utilities.SplitNameValuePair(str);
//            DataTable dt = ci.GetCustomerTable(str);

//            AccountInfoGrid.DataSource = dt;
//            AccountInfoGrid.DataBind();

//            if (dt.Rows.Count > 4)
//            {
//                lblAccountNo.Text = dt.Rows[0][1].ToString();
//                lblAccountName.Text = dt.Rows[1][1].ToString();
//                lblCCY.Text = dt.Rows[2][1].ToString();
//                lblCurrentBalance.Text = dt.Rows[3][1].ToString();
//            }

//            dt.Dispose();
//            //lblAccountName.Text     = names["name"];
//            //lblAccountStatus.Text   = names["status"];
//            //lblCurrentBalance.Text  = names["balance"];
//            //lblSI.Text              = names["instruction"];
//            //lblAddress.Text         = names["address"];
//        }

//        protected void txtSettlmentAmount_txtChanged(object sender, EventArgs e)
//        {
//            FMPS.BLL.NumberToWordConverter conv = new FMPS.BLL.NumberToWordConverter();
//            try
//            {
//                lblAmount.Text = conv.GetAmountInWords(txtSettlmentAmount.Text.Replace(",", ""));
//            }
//            catch { }
//        }

//        protected void btnCancelTrans_Click(object sender, EventArgs e)
//        {
//            Response.Redirect("../OutwardListMaker.aspx");
//        }
//        private void BindTransType()
//        {
//            TransCodeDB db = new TransCodeDB();
//            ddlCtgyPurpPrtry.DataSource = db.GetTransCode("Pacs08");
//            ddlCtgyPurpPrtry.DataBind();
//        }

//        protected void ddlCtgyPurpPrtry_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (ddlCtgyPurpPrtry.SelectedValue == "041")
//            {
//                CustomDutyPanel.Visible = true;
//                UnstructDiv.Visible = false;
//                ddListReceivingBank.SelectedValue = ConfigurationManager.AppSettings["CustomDutyReceivingBankBIC"];
//                BindBranches(ddListReceivingBank.SelectedValue);
//                ddListBranch.SelectedValue = ConfigurationManager.AppSettings["CustomDutyReceivingBranch"];
//                txtReceiverName.Text = ConfigurationManager.AppSettings["CustomDutyReceiverName"];
//                txtReceiverAccountNo.Text = ConfigurationManager.AppSettings["CustomDutyReceiverAccountNo"];
//            }
//            else
//            {
//                CustomDutyPanel.Visible = false;
//                UnstructDiv.Visible = true;
//                ddListReceivingBank.SelectedIndex = 0;
//                BindBranches(ddListReceivingBank.SelectedValue);
//                txtReceiverName.Text = "";
//                txtReceiverAccountNo.Text = "";
//            }

//        }

        
//    }
//}