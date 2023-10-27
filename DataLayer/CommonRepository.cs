using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Classes;

namespace RecommendationEngine.DataLayer
{
    public class CommonRepository
    {
        static readonly string conStr = ConfigurationManager.ConnectionStrings["CleverTap_Integration"].ConnectionString;
        public static DataTable GetPageCount(string flag, int rowOfPage/*, string AppType*/)
        {
            DataTable dt = null;
            SqlConnection sqlCon = new SqlConnection(conStr);
            try
            {
                DataSet ds = new DataSet();
                sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand("USP_GET_PAGECOUNT", sqlCon);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.CommandTimeout = 0;
                sql_cmnd.Parameters.AddWithValue("@Flag", flag);
                sql_cmnd.Parameters.AddWithValue("@p_RowsOfPage", rowOfPage);
                //sql_cmnd.Parameters.AddWithValue("@AppType", AppType);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = sql_cmnd;
                adapter.Fill(ds);
                if (ds != null)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "CommonRepository : GetPageCount Method Error" + ex.Message);
                ExceptionLogger.InsertError("CommonRepository", "GetPageCount", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            finally
            {
                sqlCon.Close();
            }
            return dt;
        }
        public static DataTable GetPageData(int pageNo, int rowOfPage, string flag)
        {
            DataTable dt = null;
            SqlConnection sqlCon = new SqlConnection(conStr);
            try
            {
                DataSet ds = new DataSet();

                sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand("USP_GET_PAGEDATA", sqlCon);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.CommandTimeout = 0;
                sql_cmnd.Parameters.AddWithValue("@p_PageNumber", pageNo);
                sql_cmnd.Parameters.AddWithValue("@p_RowsOfPage", rowOfPage);
                sql_cmnd.Parameters.AddWithValue("@Flag", flag);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = sql_cmnd;
                adapter.Fill(ds);
                if (ds != null)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "CommonRepository : GetPageData Method Error" + ex.Message);
                ExceptionLogger.InsertError("CommonRepository", "GetPageData", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            finally
            {
                sqlCon.Close();
            }
            return dt;
        }
        public static int InsertSuccessFailCount(string JourneyName, DateTime date, int success, int fail)
        {
            SqlConnection sqlCon = new SqlConnection(conStr);

            int i = 0;
            try
            {
                SqlCommand sprocCmd = new SqlCommand("USP_INS_SUCCESSFAIL_COUNT");
                sprocCmd.CommandType = CommandType.StoredProcedure;
                sprocCmd.Connection = sqlCon;
                sprocCmd.CommandTimeout = 0;
                sprocCmd.Parameters.AddWithValue("@Journey", JourneyName);
                sprocCmd.Parameters.AddWithValue("@Date", date);
                sprocCmd.Parameters.AddWithValue("@Success", success);
                sprocCmd.Parameters.AddWithValue("@Fail", fail);
                sqlCon.Open();
                i = sprocCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "CommonRepository : InsertSuccessFailCount Method Error" + ex.Message);
                ExceptionLogger.InsertError("CommonRepository", "InsertSuccessFailCount", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            finally
            {
                sqlCon.Close();
            }
            return i;
        }
    }
}