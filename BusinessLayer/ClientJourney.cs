using log4net.Repository.Hierarchy;
using RecommendationEngine.Classes;
using RecommendationEngine.DataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.BusinessLayer
{
    public class ClientJourney
    {
        public static string UploadRealTimeData(string flag/*, string AppType*/)
        {
            ExceptionLogger.WriteLogToFile(LogLevelL4N.INFO, "UploadRealTimeData : " + flag + "- Start" + DateTime.Now);
            DateTime startTime = DateTime.Now;
            string response = "";
            string responseMsg = "";
            int loopCount = 0;
            int batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["BatchSize"]);
            try
            {
                int successCount;
                int success = 0;
                int failed = 0;

                DataTable dataTable = ClientJourneyRepository.CallRecommendationRealTime();

                DataTable dt = CommonRepository.GetPageCount(flag, batchSize/*, AppType*/);

                if (dt.Rows.Count > 0)
                {
                    int pageCount = Convert.ToInt32(dt.Rows[0]["Page_Count"]);

                    for (int count = 1; count <= pageCount; count++)
                    {
                        loopCount = count;
                        DataTable pageData = new DataTable();
                        pageData = CommonRepository.GetPageData(count, batchSize, flag);
                        if (pageData != null && pageData.Rows.Count > 0)
                        {
                            List<ParametersCleverTap> listObj = new List<ParametersCleverTap>();
                            listObj = (from DataRow data in pageData.AsEnumerable()
                                       select new ParametersCleverTap
                                       {
                                           ClientCode = data.Field<string>("CLIENT_CODE"),
                                           JSONDATA = data.Field<string>("JSONDATA")
                                       }).ToList();

                            string[] finalJson = pageData.AsEnumerable().Select(r => r.Field<string>("JSONDATA")).ToArray();

                            string s = string.Join(",", finalJson.Select(p => p.ToString()).ToArray());

                            response = CleverTapAPICall.UploadUserProfile("{\"d\": [" + s.Trim() + "]}", flag);

                            successCount = Common.ResponseProcessedCount(response);

                            if (response != "")
                            {
                                responseMsg = Common.ResponseMsg(response);
                            }
                            if (responseMsg == "success")
                            {
                                success += successCount;
                                failed += pageData.Rows.Count - successCount;

                                var results = from myRow in pageData.AsEnumerable()
                                              select myRow;

                                DataTable table = results.CopyToDataTable();
                                ClientJourneyRepository.UpdateRecommendationRealTimeFlag(table);
                            }
                        }
                    }
                    ExceptionLogger.WriteLogToFile(LogLevelL4N.INFO, "UploadRealTimeData(" + flag + ") - END" + DateTime.Now);
                    DateTime endTime = DateTime.Now;
                    //Common.Insert_TriggerMail(flag, startTime, endTime, DateTime.Now, success, failed);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "Common : UploadRealTimeData: LoopCount " + loopCount + " Flag:" + flag + " - error" + ex.Message);
                ExceptionLogger.InsertError("ClientJourney", flag, ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            return response;
        }
    }
}