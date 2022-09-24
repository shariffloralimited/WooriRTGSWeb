using RTGS.DAC;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RTGS
{
    public partial class DailyDeptTransactions : System.Web.UI.Page
    {
        private System.Web.HttpResponse httpResponse;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindDept();
                ddlDay.SelectedValue = System.DateTime.Today.Day.ToString().PadLeft(2, '0');
                ddlMonth.SelectedValue = System.DateTime.Today.Month.ToString().PadLeft(2, '0');
                ddlYear.SelectedValue = System.DateTime.Today.Year.ToString();             
            }
        }
      
        protected void ExportToPdfBtn_Click(object sender, EventArgs e)
        {
            string FileName = "InwardImages(" + DeptList.SelectedValue + ")-D" + System.DateTime.Now.ToString("yyyyMMdd") + "-T" + System.DateTime.Now.ToString("HHmmss") + ".pdf";
        }
        private void BindData()
        {
            DataTable dt = GetData();

            BatchChecksGrid.DataSource = dt;
            try
            {
                BatchChecksGrid.Columns[7].FooterText = Utilities.ToMoney(dt.Compute("SUM(SttlmAmt)", "").ToString());
                BatchChecksGrid.Columns[6].FooterText = "Total";
            }
            catch
            {
                BatchChecksGrid.Columns[6].FooterText = "";
                BatchChecksGrid.Columns[7].FooterText = "";
            }
            try
            {
                BatchChecksGrid.DataBind();
            }
            catch
            {
                BatchChecksGrid.CurrentPageIndex = 0;
                BatchChecksGrid.DataBind();
            }

            dt.Dispose();
            BatchChecksGrid.Dispose();
        }

        private DataTable GetData()
        {
            DataTable dt;
            int DeptID = Int32.Parse(DeptList.SelectedValue);
            string Ccy = CCYList.SelectedValue;
            int StatusID = Int32.Parse(StatusList.SelectedValue);
            string SttlmDt = ddlYear.SelectedValue + "-" + ddlMonth.SelectedValue + "-" + ddlDay.SelectedValue;

            if (TypeList.SelectedValue == "0")
            {
                OutwardDB db = new OutwardDB();
                dt = db.GetOutwardDeptSettlement(DeptID, Ccy, StatusID, SttlmDt);
            }
            else
            {
                InwardDB db = new InwardDB();
                dt = db.GetInwardDeptSettlement(DeptID, Ccy, StatusID, SttlmDt);
            }
            return dt;
        }
        private void BindDept()
        {
            DeptDB db = new DeptDB();
            DeptList.DataSource = db.GetDepts();
            DeptList.DataBind();
            DeptList.Items.Add(new System.Web.UI.WebControls.ListItem("All", "0"));
            DeptList.SelectedValue = "0";
        }


        protected void BtnRun_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            DataTable dt = GetData();

            if (dt.Rows.Count > 0)
            {
                string SettlementDate = ddlYear.SelectedValue + "-" + ddlMonth.SelectedValue + "-" + ddlDay.SelectedValue;
                string xlsFileName = "DailyTransaction(" + SettlementDate + ")-D" + System.DateTime.Today.ToString("yyyyMMdd") + "-T" + System.DateTime.Now.ToString("HHmmss") + ".xlsx";

                XLWorkbook workbook = new XLWorkbook();
                workbook.Worksheets.Add(dt, "Sheet1");

                // Prepare the response
                httpResponse = Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string attachment = "attachment; filename=" + xlsFileName;
                httpResponse.AddHeader("content-disposition", attachment);

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                dt.Dispose();
                httpResponse.End();

            }
        }

        protected void ExportToPdfBtn_Click1(object sender, EventArgs e)
        {
            PrintPDF("DailyTransReport.pdf");
        }
        private void PrintPDF(string FileName)
        {
            DataTable dt = GetData();

            if (dt.Rows.Count == 0)
            {
                return;
            }
            string BankName = Request.Cookies["BankName"].Value;
            string UserName = Request.Cookies["UserName"].Value;
            string LogoImage = Server.MapPath("images") + "\\Bank Logo\\" + BankName + ".gif";

            Response.ClearContent();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);

            Document document = new Document(PageSize.A4.Rotate(), 10, 10, 8, 8);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            Font fnt = new Font(Font.HELVETICA, 6);
            Font fntblue = new Font(Font.HELVETICA, 6);
            fntblue.Color = new Color(0, 0, 255);
            Font fntbld = new Font(Font.HELVETICA, 6);
            fntbld.SetStyle(Font.BOLD);

            string spacer = "            -              ";

            string str = UserName + spacer;
            str = str + BankName + ": All Rights Reserved" + spacer;
            str = str + "Confidential: internal use only" + spacer;
            str = str + "Powered By Flora Limited";

            HeaderFooter footer = new HeaderFooter(new Phrase(str, fnt), false);
            footer.Alignment = Element.ALIGN_CENTER;

            document.Footer = footer;
            document.Open();

            iTextSharp.text.Image jpeg = iTextSharp.text.Image.GetInstance(LogoImage);
            jpeg.Alignment = Element.ALIGN_RIGHT;
            Cell logocell = new Cell();
            logocell.Add(jpeg);

            iTextSharp.text.Table headertable = new iTextSharp.text.Table(3);
            headertable.DefaultCell.Border = Rectangle.NO_BORDER;
            headertable.DefaultCell.VerticalAlignment = Cell.ALIGN_BOTTOM;
            headertable.Cellpadding = 0;
            headertable.Cellspacing = 0;
            headertable.Border = 0;
            headertable.Width = 100;

            string ReportType = "Inward";
          
            //if (data.FormID < 11)     
            if (TypeList.SelectedValue == "0")
            {
                ReportType = "Outward";
            }
            string headphrase = ReportType + " Search Result";

            headertable.AddCell(new Phrase(headphrase, fntblue));

            headphrase = "Report Date and Time: " + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm");

            //string headphrase = "Report Date and Time: " + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm");

            headertable.AddCell(new Phrase(headphrase, fntblue));

            string SettlementDate = ddlYear.SelectedValue + "-" + ddlMonth.SelectedValue + "-" + ddlDay.SelectedValue;

            headertable.AddCell("Settlement Report for " + SettlementDate + "\n");
            headertable.AddCell(logocell);
            

            document.Add(headertable);

            document.Add(new iTextSharp.text.Paragraph(" "));
            iTextSharp.text.pdf.PdfPTable datatable = new iTextSharp.text.pdf.PdfPTable(11);
            datatable.DefaultCell.Padding = 4;
            datatable.DefaultCell.BorderColor = new iTextSharp.text.Color(200, 200, 200);
            float[] headerwidths = { 6, 6, 10, 8, 12, 8, 12, 8, 6, 8, 8};
            datatable.SetWidths(headerwidths);
            datatable.WidthPercentage = 99;

            iTextSharp.text.pdf.BaseFont bf = iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.WINANSI, iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);

            datatable.DefaultCell.BorderWidth = 0.5f;
            datatable.DefaultCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
            datatable.DefaultCell.BackgroundColor = new iTextSharp.text.Color(200, 200, 200);
            //------------------------------------------

            datatable.AddCell(new iTextSharp.text.Phrase("Form", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("SetlDate", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Dept", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Senders A/C", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Sender", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Receivers A/C", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Receivers", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Amount", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("CCY", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Bank", fnt));
            datatable.AddCell(new iTextSharp.text.Phrase("Status", fnt));

            datatable.HeaderRows = 1;
            datatable.DefaultCell.BackgroundColor = new iTextSharp.text.Color(255, 255, 255);
            datatable.DefaultCell.BorderWidth = 0.25f;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["FormName"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["SttlmDt"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["DeptName"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["DbtrAcctId"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["DbtrNm"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["CdtrAcctId"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["CdtrNm"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase(((decimal)dt.Rows[i]["SttlmAmt"]).ToString("0.00"), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["CCY"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["Bank"]).ToString(), fnt));
                datatable.AddCell(new iTextSharp.text.Phrase((dt.Rows[i]["StatusName"]).ToString(), fnt));

            }

            //-------------TOTAL IN FOOTER --------------------
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase(Utilities.ToMoney(dt.Compute("SUM(SttlmAmt)", "").ToString()), fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld)); 
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));
            datatable.AddCell(new iTextSharp.text.Phrase("", fntbld));   
            //-------------END TOTAL -------------------------

            document.Add(datatable);

            //////////////////////////////////////////////

            document.Close();
            dt.Dispose();
            Response.End();
        }

    }
}
