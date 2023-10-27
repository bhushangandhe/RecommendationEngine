using RecommendationEngine.BusinessLayer;
using RecommendationEngine.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3
                                                            | System.Net.SecurityProtocolType.Tls
                                                            | System.Net.SecurityProtocolType.Tls11
                                                            | System.Net.SecurityProtocolType.Tls12;

                //Manually Trigger Client master upload 
                ClientJourney.UploadRealTimeData("Recommendation_RealTime");

                //Actual Code
                //ServiceBase[] ServicesToRun;
                //ServicesToRun = new ServiceBase[]
                //{
                //    new RecommendationEngineServices()
                //};
                //ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "Program : Main Method End - SqlException");
                ExceptionLogger.InsertError("Program", "Main Method", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
        }
    }
}