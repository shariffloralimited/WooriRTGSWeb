using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

using FloraSoft;
using UTILITY;

namespace RTGS
{
    public partial class Login : System.Web.UI.Page
    {
        protected string BranchNumonic = "";
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
            if (!this.Page.IsPostBack)
            {
            }
        }

        public bool IsAuthenticated(string srvr, string usr, string pwd)
        {
            bool result = false;
            SearchResult searchResult = DirectoryHelper.serachLogin(usr, pwd);
            if (searchResult != null)
            {
                ResultPropertyCollection properties = searchResult.Properties;
                foreach (string text in properties.PropertyNames)
                {
                    string text2 = "";
                    foreach (object current in properties[text])
                    {
                        text2 += current;
                    }
                    if (text == "physicaldeliveryofficename")
                    {
                        this.BranchNumonic = text2;
                    }
                }
                result = true;
            }
            return result;
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            string text = "0";
            UserDB userDB = new UserDB();
            UserInfo userInfo = new UserInfo();
            string a = ConfigurationManager.AppSettings["ADLogin"].ToUpper();
            if (a != "TRUE")
            {
                userInfo = userDB.Login(this.UserName.Text, this.Pass.Text, base.Request.UserHostAddress);
                text = userInfo.UserID;
            }
            else if (this.IsAuthenticated(ConfigurationManager.AppSettings["ADServer"], this.UserName.Text, this.Pass.Text))
            {
                userInfo = userDB.ADLogin(this.UserName.Text, this.BranchNumonic, base.Request.UserHostAddress);
                text = userInfo.UserID;
            }
            if (text == "0")
            {
                string text2 = this.Tried.Value;
                if (text2 == "")
                {
                    text2 = "0";
                }
                int num = int.Parse(text2) + 1;
                this.Tried.Value = num.ToString();
                if (num > 2)
                {
                    userDB.LockUser(this.UserName.Text.Trim());
                    this.MyMessage.Text = this.UserName.Text + " account has been locked.";
                }
                else
                {
                    this.MyMessage.Text = userInfo.ExpMsg;
                }
            }
            else
            {
                FormsAuthentication.SetAuthCookie(text, false);
                base.Response.Cookies["RoleID"].Value = "";
                base.Response.Cookies["RoleName"].Value = "";
                base.Response.Cookies["RoleCount"].Value = userInfo.RoleID;
                base.Response.Cookies["UserName"].Value = userInfo.UserName;
                base.Response.Cookies["DeptID"].Value = userInfo.DeptID;
                base.Response.Cookies["RoutingNo"].Value = userInfo.RoutingNo;
                base.Response.Cookies["BranchCD"].Value = userInfo.BranchCD;
                base.Response.Cookies["AllBranch"].Value = userInfo.AllBranch.ToString();
                base.Response.Cookies["DeptName"].Value = userInfo.DeptName;
                base.Response.Cookies["BranchName"].Value = userInfo.BranchName;
                base.Response.Cookies["BankName"].Value = userInfo.BankName;
                base.Response.Cookies["DaysRemaining"].Value = userInfo.DaysRemaining.ToString();
                base.Response.Cookies["ChangePwdNow"].Value = userInfo.ChangePwdNow.ToString();
                base.Response.Cookies["ExpMsg"].Value = userInfo.ExpMsg;
                base.Response.Cookies["PrevPageURL"].Value = HttpContext.Current.Request.Url.AbsolutePath;
                BankSettingsDB bankSettingsDB = new BankSettingsDB();
                BankSettings bankSettings = bankSettingsDB.GetBankSettings();
                base.Response.Cookies["DeptBanking"].Value = bankSettings.DeptBanking.ToString();
                base.Response.Cookies["AuthorizerEnabled"].Value = bankSettings.AuthorizerEnabled.ToString();
                if (userInfo.ChangePwdNow.ToString() == "TRUE")
                {
                    base.Response.Redirect("ChangePassword.aspx");
                }
                if (userInfo.RoleCount != "1")
                {
                    base.Response.Redirect("SelectRole.aspx");
                }
                else
                {
                    base.Response.Cookies["RoleID"].Value = userInfo.RoleID;
                    base.Response.Cookies["RoleCD"].Value = userInfo.RoleCD;
                    base.Response.Cookies["RoleName"].Value = userInfo.RoleName;
                    base.Response.Cookies["TransLimit"].Value = userInfo.TransLimit.ToString();
                }
                if (userInfo.RoleCD == "RTRV" || userInfo.RoleCD == "RTSA")
                {
                    base.Response.Redirect("ReportViewerMenu.aspx");
                }
                if (userInfo.RoleCD == "RTMK" || userInfo.RoleCD == "RTCK" || userInfo.RoleCD == "RTAU")
                {
                    base.Response.Redirect("BranchMessages.aspx");
                }
                if (userInfo.RoleCD == "RTAD" || userInfo.RoleCD == "RTFM")
                {
                    base.Response.Redirect("Default.aspx");
                }
                if (userInfo.RoleCD == "RTSA")
                {
                    base.Response.Redirect("AuditLog.aspx");
                }
            }
        }
    }
}
//        protected void Page_Init(object sender, EventArgs e)
//        {
//           try
//           {
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
//            if (!Page.IsPostBack)
//            {
//                //Response.Buffer = true;
//                //Response.CacheControl = "no-cache";
//                //Response.AddHeader("Pragma", "no-cache");
//                //Response.Expires = -1441;

//                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
//                //Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
//                //Response.Cache.SetNoStore();

//                //LogOut();
//            }
//        }

//        public bool IsAuthenticated(string srvr, string usr, string pwd)
//        {
//            //string department = "";
//            bool authenticated = false;
//            SearchResult result = UTILITY.DirectoryHelper.serachLogin(usr, pwd);

//            if (result != null)
//            {
//                ResultPropertyCollection col = result.Properties;
//                foreach (string key in col.PropertyNames)
//                {
//                    string str = "";
//                    foreach (object curcol in col[key])
//                    {
//                        str = str + curcol;
//                    }
//                    if(key == "physicaldeliveryofficename")
//                    {
//                        BranchNumonic = str;
//                    }
//                    //if(key == "department")
//                    //{
//                    //    department = str;
//                    //}

//                    //MessageBox.Show(str);
//                }
//                authenticated = true;
//            }
//            //else
//            //{
//            //    MessageBox.Show("Login Failed");
//            //}
//            //string DeptList = ",IT,FIT,ICCD,CBT,FD,CCD,GSSP,OPS,HRM,";
//            //if ((BranchNumonic == "HO") && (DeptList.IndexOf(","+department+",") > -1))
//            //{
//            //    BranchNumonic = "ANY";
//            //}
//            return authenticated;
//        }
//        protected void Login_Click(object sender, EventArgs e)
//        {
//            string UserID     = "0";

//            UserDB db       = new UserDB();
//            UserInfo uinfo  = new UserInfo();

//            //Checking if Bank is using AD Login or not.

//            //string ADLogin = ConfigurationManager.AppSettings["ADLogin"].ToUpper();
//            //if (ADLogin != "TRUE")
//            //{
//            //    uinfo = db.Login(UserName.Text, Pass.Text);
//            //    UserID = uinfo.UserID;
//            //}
//            //else
//            //{
//            //    if (IsAuthenticated(ConfigurationManager.AppSettings["ADServer"], UserName.Text, Pass.Text))
//            //    {
//            //        uinfo = db.ADLogin(UserName.Text, BranchNumonic);
//            //        UserID = uinfo.UserID;
//            //    }
//            //    else
//            //    {
//            //        uinfo.UserID = "0";
//            //        uinfo.ExpMsg = "Login(AD) Failed.";
//            //    }
//            //}

//            string ADLogin = ConfigurationManager.AppSettings["ADLogin"].ToUpper();
//            if (ADLogin != "TRUE")
//            {
//                uinfo = db.Login(UserName.Text, Pass.Text, Request.UserHostAddress);
//                UserID = uinfo.UserID;
//            }
//            else
//            {
//                if (IsAuthenticated(ConfigurationManager.AppSettings["ADServer"], UserName.Text, Pass.Text))
//                {
//                    uinfo = db.ADLogin(UserName.Text, BranchNumonic, Request.UserHostAddress);
//                    UserID = uinfo.UserID;
//                }
//            }

//            // if login failed.
//            if (UserID == "0")
//            {
//                string LoginTries = Tried.Value;
//                if (LoginTries == "")
//                {
//                    LoginTries = "0";
//                }
//                int NewVal = Int32.Parse(LoginTries) + 1;
//                Tried.Value = NewVal.ToString();
//                if (NewVal > 2)
//                {
//                    db.LockUser(UserName.Text.Trim());
//                    MyMessage.Text = UserName.Text + " account has been locked.";
//                }
//                else
//                {
//                    MyMessage.Text = uinfo.ExpMsg;
//                }
//            }
//            else
//            {
//                // when login is successfull.
//                FormsAuthentication.SetAuthCookie(UserID, false);

//                Response.Cookies["RoleID"].Value = "";
//                Response.Cookies["RoleName"].Value = "";

//                Response.Cookies["RoleCount"].Value = uinfo.RoleID;
//                Response.Cookies["UserName"].Value      = uinfo.UserName;
//                //Response.Cookies["UserName"].Value      = EncryptDecrypt(uinfo.UserName);
//                Response.Cookies["DeptID"].Value        = uinfo.DeptID;
//                Response.Cookies["RoutingNo"].Value     = uinfo.RoutingNo;
//                Response.Cookies["BranchCD"].Value      = uinfo.BranchCD;
//                Response.Cookies["AllBranch"].Value     = uinfo.AllBranch.ToString();  //False
//                Response.Cookies["DeptName"].Value      = uinfo.DeptName;
//                Response.Cookies["BranchName"].Value    = uinfo.BranchName;
//                Response.Cookies["BankName"].Value      = uinfo.BankName;
//                Response.Cookies["DaysRemaining"].Value = uinfo.DaysRemaining.ToString();
//                Response.Cookies["ChangePwdNow"].Value  = uinfo.ChangePwdNow.ToString();
//                Response.Cookies["ExpMsg"].Value        = uinfo.ExpMsg;
//                Response.Cookies["PrevPageURL"].Value   = HttpContext.Current.Request.Url.AbsolutePath;

//                BankSettingsDB bs = new BankSettingsDB();
//                BankSettings settings = bs.GetBankSettings();
//                Response.Cookies["DeptBanking"].Value       = settings.DeptBanking.ToString();
//                Response.Cookies["AuthorizerEnabled"].Value = settings.AuthorizerEnabled.ToString();

//                if (uinfo.ChangePwdNow.ToString() == "TRUE")
//                {
//                    Response.Redirect("ChangePassword.aspx");
//                }

//                if (uinfo.RoleCount != "1")
//                {
//                    Response.Redirect("SelectRole.aspx");
//                }
//                else
//                {
//                    Response.Cookies["RoleID"].Value    = uinfo.RoleID;
//                    Response.Cookies["RoleCD"].Value    = uinfo.RoleCD;
//                    Response.Cookies["RoleName"].Value  = uinfo.RoleName;
//                    Response.Cookies["TransLimit"].Value= uinfo.TransLimit.ToString();
//                }

//                if ((uinfo.RoleCD == "RTRV") || (uinfo.RoleCD == "RTSA")) 
//                {
//                    Response.Redirect("ReportViewerMenu.aspx");
//                }

//                //if (uinfo.RoleCD == "RTSA")
//                //{
//                //    Response.Redirect("ReportViewerMenu.aspx");
//                //}

//                if ((uinfo.RoleCD == "RTMK") || (uinfo.RoleCD == "RTCK") || (uinfo.RoleCD == "RTAU"))
//                {
//                    Response.Redirect("BranchMessages.aspx");
//                }

//                if ((uinfo.RoleCD == "RTAD") || (uinfo.RoleCD == "RTFM"))
//                {
//                    Response.Redirect("Default.aspx");
//                }

//                if (uinfo.RoleCD == "RTSA")
//                {
//                    Response.Redirect("AuditLog.aspx");
//                }


//                uinfo = null;
//            }
//        }

//        //private void SignOut()
//        //{
//        //    try
//        //    {
//        //        UserDB db = new UserDB();
//        //        int UserID = Int32.Parse(Context.User.Identity.Name);
//        //        db.LogOut(UserID);
//        //    }
//        //    catch { }

//        //    FormsAuthentication.SignOut();
//        //    Session.Abandon();

//        //    // clear authentication cookie
//        //    HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
//        //    cookie1.Expires = DateTime.Now.AddYears(-1);
//        //    Response.Cookies.Add(cookie1);

//        //    // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
//        //    HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
//        //    cookie2.Expires = DateTime.Now.AddYears(-1);
//        //    Response.Cookies.Add(cookie2);
//        //    MyMessage.Text = System.DateTime.Now.ToLongTimeString();
//        //}



//        //private void SignOut()
//        //{
//        //    try
//        //    {
//        //        UserDB db = new UserDB();
//        //        int UserID = Int32.Parse(Context.User.Identity.Name);
//        //        db.LogOut(UserID);
//        //    }
//        //    catch { }

//        //    FormsAuthentication.SignOut();
//        //    Session.Abandon();

//        //     clear authentication cookie
//        //    HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
//        //    cookie1.Expires = DateTime.Now.AddYears(-1);
//        //    Response.Cookies.Add(cookie1);

//        //     clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
//        //    HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
//        //    cookie2.Expires = DateTime.Now.AddYears(-1);
//        //    Response.Cookies.Add(cookie2);
//        //    MyMessage.Text = System.DateTime.Now.ToLongTimeString();
//        //}
//    }
//}

