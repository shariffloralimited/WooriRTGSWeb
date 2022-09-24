﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RTGS.DAC
{
    public class InwardDB
    {
        public DataTable GetInwardList(string RoutingNo, int StatusID, string Form, decimal Amount)
        {
            SqlConnection myConnection = new SqlConnection(AppVariables.ConStr);
            SqlDataAdapter myCommand = new SqlDataAdapter("GetListInward", myConnection);
            myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterBranchId = new SqlParameter("@BranchId", SqlDbType.VarChar, 9);
            parameterBranchId.Value = RoutingNo;
            myCommand.SelectCommand.Parameters.Add(parameterBranchId);

            SqlParameter parameterStatusID = new SqlParameter("@StatusID", SqlDbType.Int, 4);
            parameterStatusID.Value = StatusID;
            myCommand.SelectCommand.Parameters.Add(parameterStatusID);

            SqlParameter parameterForm = new SqlParameter("@Form", SqlDbType.VarChar, 10);
            parameterForm.Value = Form;
            myCommand.SelectCommand.Parameters.Add(parameterForm);
            myConnection.Open();

            SqlParameter parameterAmount = new SqlParameter("@Amount", SqlDbType.Money);
            parameterAmount.Value = Amount;
            myCommand.SelectCommand.Parameters.Add(parameterAmount);

            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();
            return dt;
        }

        public DataTable GetInwardDeptList(int DeptID, int StatusID)
        {
            SqlConnection myConnection = new SqlConnection(AppVariables.ConStr);
            SqlDataAdapter myCommand = new SqlDataAdapter("GetListDeptInward", myConnection);
            myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterDeptID = new SqlParameter("@DeptID", SqlDbType.Int, 4);
            parameterDeptID.Value = DeptID;
            myCommand.SelectCommand.Parameters.Add(parameterDeptID);

            SqlParameter parameterStatusID = new SqlParameter("@StatusID", SqlDbType.Int, 4);
            parameterStatusID.Value = StatusID;
            myCommand.SelectCommand.Parameters.Add(parameterStatusID);

            myConnection.Open();

            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();
            return dt;
        }

        public string GetOutwardIDForInward04(string InwardID)
        {
            Guid Inid = new Guid(InwardID);
            SqlConnection myConnection = new SqlConnection(AppVariables.ConStr);
            SqlCommand myCommand = new SqlCommand("GetOutwardIDForInward04", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
         
            SqlParameter parameterInwardID = new SqlParameter("@InwardID", SqlDbType.UniqueIdentifier);
            parameterInwardID.Value = Inid;
            myCommand.Parameters.Add(parameterInwardID);

            SqlParameter parameterOutwardID = new SqlParameter("@OutwardID", SqlDbType.UniqueIdentifier);
            parameterOutwardID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterOutwardID);

            myConnection.Open();

            //DataTable dt = new DataTable();
            //myCommand.Fill(dt);
            myCommand.ExecuteNonQuery();
            string  OutwardID= parameterOutwardID.Value.ToString();
            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();
            return OutwardID;
        }
        public DataTable GetInwardSettlement(int BranchId, string CCy, int StatusID, string SttlmDt)
        {
            SqlConnection myConnection = new SqlConnection(AppVariables.ConStr);
            SqlDataAdapter myCommand = new SqlDataAdapter("RTGS_GetInwardSettlement", myConnection);
            myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterBranchId = new SqlParameter("@BranchId", SqlDbType.Int, 4);
            parameterBranchId.Value = BranchId;
            myCommand.SelectCommand.Parameters.Add(parameterBranchId);

            SqlParameter parameterCCy = new SqlParameter("@CCy", SqlDbType.VarChar, 3);
            parameterCCy.Value = CCy;
            myCommand.SelectCommand.Parameters.Add(parameterCCy);

            SqlParameter parameterStatusID = new SqlParameter("@StatusID", SqlDbType.Int, 4);
            parameterStatusID.Value = StatusID;
            myCommand.SelectCommand.Parameters.Add(parameterStatusID);

            SqlParameter parameterSttlmDt = new SqlParameter("@SttlmDt", SqlDbType.VarChar, 10);
            parameterSttlmDt.Value = SttlmDt;
            myCommand.SelectCommand.Parameters.Add(parameterSttlmDt);
            myConnection.Open();

            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();
            return dt;
        }

        public DataTable GetInwardDeptSettlement(int DeptID, string CCy, int StatusID, string SttlmDt)
        {
            SqlConnection myConnection = new SqlConnection(AppVariables.ConStr);
            SqlDataAdapter myCommand = new SqlDataAdapter("RTGS_GetInwardDeptSettlement", myConnection);
            myCommand.SelectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parameterDeptID = new SqlParameter("@DeptID", SqlDbType.Int, 4);
            parameterDeptID.Value = DeptID;
            myCommand.SelectCommand.Parameters.Add(parameterDeptID);

            SqlParameter parameterCCy = new SqlParameter("@CCy", SqlDbType.VarChar, 3);
            parameterCCy.Value = CCy;
            myCommand.SelectCommand.Parameters.Add(parameterCCy);

            SqlParameter parameterStatusID = new SqlParameter("@StatusID", SqlDbType.Int, 4);
            parameterStatusID.Value = StatusID;
            myCommand.SelectCommand.Parameters.Add(parameterStatusID);

            SqlParameter parameterSttlmDt = new SqlParameter("@SttlmDt", SqlDbType.VarChar, 10);
            parameterSttlmDt.Value = SttlmDt;
            myCommand.SelectCommand.Parameters.Add(parameterSttlmDt);
            myConnection.Open();

            DataTable dt = new DataTable();
            myCommand.Fill(dt);

            myConnection.Close();
            myCommand.Dispose();
            myConnection.Dispose();
            return dt;
        }
    }
}