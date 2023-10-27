using log4net.Repository.Hierarchy;
using RecommendationEngine.Classes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.BusinessLayer
{
    public class CleverTapAPICall
    {
        public static string UploadUserProfile(string dt, string flag/*, string appType*/)
        {
            string pagesource = string.Empty;
            string accountId = string.Empty;
            string passCode = string.Empty;

            if (Convert.ToString(ConfigurationManager.AppSettings["CleverTapApiFlag"]) == "Y")
            {
                var client = new RestClient(Convert.ToString(ConfigurationManager.AppSettings["CleverTapApi"]));
                //Uncomment later for dynamic changes
                //accountId = (appType == "investor") ? Convert.ToString(ConfigurationManager.AppSettings["INV_ACCOUNT_ID_UAT"]) : (appType == "trader") ? Convert.ToString(ConfigurationManager.AppSettings["TRD_ACCOUNT_ID_UAT"]) : "";
                //passCode = (appType == "investor") ? Convert.ToString(ConfigurationManager.AppSettings["INV_PASSCODE_UAT"]) : (appType == "trader") ? Convert.ToString(ConfigurationManager.AppSettings["TRD_PASSCODE_UAT"]) : "";
                //End
                accountId = Convert.ToString(ConfigurationManager.AppSettings["INV_ACCOUNT_ID_PROD"]);
                passCode = Convert.ToString(ConfigurationManager.AppSettings["INV_PASSCODE_PROD"]);
                try
                {
                    var request = new RestRequest("1/upload", Method.Post);
                    request.AddHeader("X-CleverTap-Account-Id", accountId);
                    request.AddHeader("X-CleverTap-Passcode", passCode);
                    request.AddHeader("Content-Type", "application/json; charset=utf-8");

                    var body = dt;

                    request.AddParameter("application/json", body, ParameterType.RequestBody);

                    ExceptionLogger.WriteLogToFile(LogLevelL4N.INFO, "CleverTapAPICall : UploadUserProfile : API START :- " + DateTime.Now);

                    RestResponse response = client.Execute(request);

                    ExceptionLogger.WriteLogToFile(LogLevelL4N.INFO, "CleverTapAPICall : UploadUserProfile : response:" + response.Content + " " + DateTime.Now);

                    ExceptionLogger.WriteLogToFile(LogLevelL4N.INFO, "CleverTapAPICall : UploadUserProfile : API END :- " + DateTime.Now);

                    pagesource = Convert.ToString(response.Content);

                    if (response.ErrorException != null)
                    {
                        ExceptionLogger.WriteLogToFile(LogLevelL4N.INFO, "CleverTapAPICall : UploadUserProfile : RequestInnerExceptionException :- " + response.ErrorException.InnerException.Message + " StatusCode: " + response.StatusCode);
                        ExceptionLogger.InsertError("CleverTapAPICall", "UploadUserProfile", response.ErrorException.InnerException.Message, response.ErrorException.Source, " Web Request Error", response.ErrorException.InnerException.StackTrace);
                    }
                }
                catch (WebException ex)
                {
                    ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "CleverTapAPICall : UploadUserProfile - Web Error" + ex.Message);
                    ExceptionLogger.InsertError("CleverTapAPICall", "UploadUserProfile", ex.Message, ex.Source, " Web Error", ex.StackTrace);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "CleverTapAPICall : UploadUserProfile - Error" + ex.Message);
                    ExceptionLogger.InsertError("CleverTapAPICall", "UploadUserProfile", ex.Message, ex.Source, "Exception", ex.StackTrace);
                }
                finally
                {
                    client.Dispose();
                }
            }
            return pagesource;
        }
    }
}