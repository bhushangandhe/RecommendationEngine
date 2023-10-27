using Quartz;
using RecommendationEngine.BusinessLayer;
using RecommendationEngine.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Quartz
{
    public class RealTimeJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => CallJob());
        }
        public static void CallJob()
        {
            try
            {
                if (ConfigurationManager.AppSettings["RealTimeActive"] == "Y")
                {
                    ClientJourney.UploadRealTimeData("Recommendation_RealTime");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.InsertError("RealTimeJob", "Call Job", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
        }
    }
}