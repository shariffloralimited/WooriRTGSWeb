using ClosedXML.Excel;
using FloraSoft;
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace RTGS.Forms
{
    public partial class FinInstitutionCreditTransferBulk : System.Web.UI.Page
    {
        public HttpResponse httpResponse
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.Request.Cookies["RoleCD"].Value != "RTMK")
            {
                base.Response.Redirect("../AccessDenied.aspx");
            }
            if (!this.Page.IsPostBack)
            {
                this.DeletePastDocuments();
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("../OutwardListMaker.aspx");
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            this.LoadExcel();
        }

        private void LoadExcel()
        {
            if (this.upload1.HasFile)
            {
                this.HdnTick.Value = DateTime.Now.Ticks.ToString();
                HUBFile hUBFile = new HUBFile();
                string value = base.Request.Cookies["UserName"].Value;
                string userHostAddress = HttpContext.Current.Request.UserHostAddress;
                string value2 = base.Request.Cookies["BranchCD"].Value;
                string value3 = this.HdnTick.Value;
                string fileName = this.upload1.FileName;
                string text = base.Server.MapPath(".").Replace("\\Forms", "");
                string text2 = string.Concat(new string[]
				{
					text,
					"/Documents/",
					DateTime.Today.Day.ToString(),
					"-",
					value3,
					".xls"
				});
                this.upload1.SaveAs(text2);
                hUBFile.ExecuteSQL("DELETE RTGSD.dbo.Outward09 WHERE Day <> " + DateTime.Today.Day.ToString());
                string text3 = hUBFile.BulkExcelUpload(value, userHostAddress, value2, value3, text2, "RTGSD.dbo.Outward09");
                int num = hUBFile.MoveToMain09(value3);
                this.Msg.Text = text3;
                this.BindInvalidList(value3);
                int count = this.InvalidList.Rows.Count;
                this.Msg.Text = num.ToString() + " rows uploaded. " + count.ToString() + " rows invalid.";
                this.btnProcess.Visible = true;
            }
        }

        private void DeletePastDocuments()
        {
            string path = base.Server.MapPath(".").Replace("\\Forms", "") + "/Documents";
            string[] files = Directory.GetFiles(path);
            string[] array = files;
            for (int i = 0; i < array.Length; i++)
            {
                string path2 = array[i];
                if (File.GetCreationTime(path2).Day != DateTime.Today.Day)
                {
                    try
                    {
                        File.Delete(path2);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void BindInvalidList(string Tick)
        {
            TransSearchDB transSearchDB = new TransSearchDB();
            this.InvalidList.DataSource = transSearchDB.GetInvalidList("Pacs.009", this.HdnTick.Value);
            this.InvalidList.DataBind();
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("../OutwardListMaker.aspx");
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            TransSearchDB db = new TransSearchDB();
            DataTable dt = db.GetInvalidList("Pacs.009", HdnTick.Value);

            dt.Columns.RemoveAt(4);
            dt.Columns.RemoveAt(3);
            dt.Columns.RemoveAt(2);
            dt.Columns.RemoveAt(1);
            dt.Columns.RemoveAt(0);

            if (dt.Rows.Count > 0)
            {
                string xlsFileName = "Invalid_List" + System.DateTime.Today.ToString("yyyyMMdd") + "-T" + System.DateTime.Now.ToString("HHmmss") + ".xlsx";

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
    }
}

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (Request.Cookies["RoleCD"].Value != "RTMK")
//            {
//                Response.Redirect("../AccessDenied.aspx");
//            }
//            if (!Page.IsPostBack)
//            {
//                DeletePastDocuments();
//            }
//        }

//        protected void Cancel_Click(object sender, EventArgs e)
//        {
//            Response.Redirect("../OutwardListMaker.aspx");
//        }

//        protected void Submit_Click(object sender, EventArgs e)
//        {
//            LoadExcel();
//        }
//        private void LoadExcel()
//        {
//            if (upload1.HasFile)
//            {
//                HdnTick.Value = System.DateTime.Now.Ticks.ToString();

//                FloraSoft.HUBFile hf = new FloraSoft.HUBFile();

//                //string ContentType = upload1.PostedFile.ContentType;
//                //if (ContentType != "text/plain")
//                //{
//                //    Msg.Text = "Uploaded File is not an excel file.";
//                //    return;
//                //}

//                string Maker = Request.Cookies["UserName"].Value;
//                string MakerIP = HttpContext.Current.Request.UserHostAddress;
//                string BranchCD = Request.Cookies["BranchCD"].Value;
//                string Tick = HdnTick.Value;

//                string postedfilename = upload1.FileName;
//                string AppPath = Server.MapPath(".").Replace("\\Forms", "");

//                string FileName = AppPath + "/Documents/" + DateTime.Today.Day.ToString() + "-" + Tick + ".xls";
//                upload1.SaveAs(FileName);

//                hf.ExecuteSQL("DELETE RTGSD.dbo.Outward09 WHERE Day <> " + System.DateTime.Today.Day.ToString());

//                string returnmessage = hf.BulkExcelUpload(Maker, MakerIP, BranchCD, Tick, FileName, "RTGSD.dbo.Outward09");

//                int RowsUploaded = hf.MoveToMain09(Tick);

//                Msg.Text = returnmessage;

//                BindInvalidList(Tick);

//                int RowsFailed = InvalidList.Rows.Count;

//                Msg.Text = RowsUploaded.ToString() + " rows uploaded. " + RowsFailed.ToString() + " rows invalid.";
//                btnProcess.Visible = true;
//            }
//        }

//        private void DeletePastDocuments()
//        {
//            string AppPath = Server.MapPath(".").Replace("\\Forms", "") + "/Documents";

//            string[] files = Directory.GetFiles(AppPath);
//            foreach (string file in files)
//            {
//                if (File.GetCreationTime(file).Day != System.DateTime.Today.Day)
//                {
//                    try
//                    {
//                        File.Delete(file);
//                    }
//                    catch (Exception ex)
//                    {
//                        System.Console.WriteLine(ex.Message);
//                    }
//                }
//            }
//        }
//        private void BindInvalidList(string Tick)
//        {
//            TransSearchDB db = new TransSearchDB();
//            InvalidList.DataSource = db.GetInvalidList("Pacs.009", HdnTick.Value);
//            InvalidList.DataBind();
//        }

//        protected void btnProcess_Click(object sender, EventArgs e)
//        {
//            Response.Redirect("../OutwardListMaker.aspx");
//        }

//        protected void btnExcel_Click(object sender, EventArgs e)
//        {
//            TransSearchDB db = new TransSearchDB();
//            DataTable dt = db.GetInvalidList("Pacs.009", HdnTick.Value);

//            dt.Columns.RemoveAt(4);
//            dt.Columns.RemoveAt(3);
//            dt.Columns.RemoveAt(2);
//            dt.Columns.RemoveAt(1);
//            dt.Columns.RemoveAt(0);

//            if (dt.Rows.Count > 0)
//            {
//                string xlsFileName = "Invalid_List" + System.DateTime.Today.ToString("yyyyMMdd") + "-T" + System.DateTime.Now.ToString("HHmmss") + ".xlsx";

//                XLWorkbook workbook = new XLWorkbook();
//                workbook.Worksheets.Add(dt, "Sheet1");

//                // Prepare the response
//                httpResponse = Response;
//                httpResponse.Clear();
//                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
//                string attachment = "attachment; filename=" + xlsFileName;
//                httpResponse.AddHeader("content-disposition", attachment);

//                // Flush the workbook to the Response.OutputStream
//                using (MemoryStream memoryStream = new MemoryStream())
//                {
//                    workbook.SaveAs(memoryStream);
//                    memoryStream.WriteTo(httpResponse.OutputStream);
//                    memoryStream.Close();
//                }
//                dt.Dispose();
//                httpResponse.End();

//            }
//        }

//        public HttpResponse httpResponse { get; set; }
//    }
//}