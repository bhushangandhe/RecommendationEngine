using log4net.Repository.Hierarchy;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace RecommendationEngine.Classes
{
    public enum LogLevelL4N
    {
        DEBUG = 1,
        ERROR,
        FATAL,
        INFO,
        WARN
    }
    public static class ExceptionLogger
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Logger));
        static ExceptionLogger()
        {
            XmlConfigurator.Configure();
        }
        public static void WriteLogToFile(LogLevelL4N logLevel, string log)
        {
            if (logLevel.Equals(LogLevelL4N.DEBUG))
            {
                logger.Debug(log);
            }
            else if (logLevel.Equals(LogLevelL4N.ERROR))
            {
                logger.Error(log);
            }
            else if (logLevel.Equals(LogLevelL4N.FATAL))
            {
                logger.Fatal(log);
            }
            else if (logLevel.Equals(LogLevelL4N.INFO))
            {
                logger.Info(log);
            }
            else if (logLevel.Equals(LogLevelL4N.WARN))
            {
                logger.Warn(log);
            }
        }
        public static void InsertError(string ClassName, string MethodName, string Message, string Source, string SourceUrl, string StackTrace)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CleverTap_Integration"].ConnectionString);

                SqlCommand sprocCmd = new SqlCommand("USP_INS_LOGEXCEPTION");
                sprocCmd.CommandType = CommandType.StoredProcedure;
                sprocCmd.Connection = conn;
                sprocCmd.CommandTimeout = 0;
                sprocCmd.Parameters.AddWithValue("@ClassName", ClassName);
                sprocCmd.Parameters.AddWithValue("@MethodName", MethodName);
                sprocCmd.Parameters.AddWithValue("@ExceptionMessage", Message);
                sprocCmd.Parameters.AddWithValue("@Source", Source);
                sprocCmd.Parameters.AddWithValue("@SourceUrl", SourceUrl);
                sprocCmd.Parameters.AddWithValue("@Trace", StackTrace);
                conn.Open();
                sprocCmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (SqlException exs)
            {
                WriteLogToFile(LogLevelL4N.ERROR, "ExceptionLogger InsertError End - SqlException  " + exs.Message);
            }
            catch (Exception exs)
            {
                WriteLogToFile(LogLevelL4N.ERROR, "ExceptionLogger InsertError End " + exs.Message);
            }
        }
    }
}