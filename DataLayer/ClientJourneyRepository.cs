using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RecommendationEngine.Classes;

namespace RecommendationEngine.DataLayer
{
    public class ClientJourneyRepository
    {
        static readonly string conStr = ConfigurationManager.ConnectionStrings["CleverTap_Integration"].ConnectionString;
        public static DataTable CallRecommendationRealTime()
        {
            DataTable dt = null;
            SqlConnection sqlCon = new SqlConnection(conStr);
            try
            {
                DataSet ds = new DataSet();
                sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand("REC_USP_INS_RSCH_CALLS_LOGS", sqlCon);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.CommandTimeout = 0;
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = sql_cmnd;
                adapter.Fill(ds);
                if (ds != null)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "ClientJourneyRepository : CallNewClientRealTimeJob Method Error: " + ex.Message);
                ExceptionLogger.InsertError("ClientJourneyRepository", "CallNewClientRealTimeJob", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            finally
            {
                sqlCon.Close();
            }
            return dt;
        }
        public static DataTable UpdateRecommendationRealTimeFlag(DataTable dt)
        {
            SqlConnection sqlCon = new SqlConnection(conStr);
            try
            {
                DataSet ds = new DataSet();

                SqlCommand sprocCmd = new SqlCommand("REC_USP_UPDATE_RSCH_REALTIME_SENTFLAG");
                sprocCmd.CommandType = CommandType.StoredProcedure;
                sprocCmd.Connection = sqlCon;
                sprocCmd.CommandTimeout = 0;
                sprocCmd.Parameters.AddWithValue("@tblUpdateRealTimeSentFlag", dt);
                sqlCon.Open();
                sprocCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "ClientJourneyRepository : UpdateRealTimeFlag Method Error" + ex.Message);
                ExceptionLogger.InsertError("ClientJourneyRepository", "UpdateRealTimeFlag", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            finally
            {
                sqlCon.Close();
            }
            return dt;
        }
    }
}