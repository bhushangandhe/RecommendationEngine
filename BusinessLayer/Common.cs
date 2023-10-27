using log4net.Repository.Hierarchy;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RecommendationEngine.DataLayer;
using RecommendationEngine.Classes;

namespace RecommendationEngine.BusinessLayer
{
    public class Common
    {
        public static string ResponseMsg(string flag)
        {
            string responseMsg = "";
            try
            {
                if (flag != "")
                {
                    var obj = JObject.Parse(flag);
                    responseMsg = Convert.ToString(obj.Properties().Select(p => p.Value).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "Common : ResponseMsg: " + flag + " - error" + ex.Message);
                ExceptionLogger.InsertError("Common", "ResponseMsg", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            return responseMsg;
        }
        public static int ResponseProcessedCount(string flag)
        {
            int processedCount = 0;
            try
            {
                if (flag != "")
                {
                    var obj = JObject.Parse(flag);
                    processedCount = Convert.ToInt32(obj["processed"]);
                }
                return processedCount;
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLogToFile(LogLevelL4N.ERROR, "Common : ResponseProcessedCount: " + flag + " - error" + ex.Message);
                ExceptionLogger.InsertError("Common", "ResponseProcessedCount", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
            return processedCount;
        }
        public static void Insert_TriggerMail(string JourneyName, DateTime startDate, DateTime endDate, DateTime date, int success, int fail)
        {
            int count = CommonRepository.InsertSuccessFailCount(JourneyName, date, success, fail);

            if (count > 0)
            {
                string Host = Convert.ToString(ConfigurationManager.AppSettings["Host"]);
                string Port = Convert.ToString(ConfigurationManager.AppSettings["Port"]);
                string Username = Convert.ToString(ConfigurationManager.AppSettings["User"]);
                string Password = Convert.ToString(ConfigurationManager.AppSettings["Password"]);
                string Sender = Convert.ToString(ConfigurationManager.AppSettings["Sender"]);
                var Receipent = new List<string>(ConfigurationManager.AppSettings["Receipent"].Split(new char[] { ',' }));
                var Bcc = new List<string>(ConfigurationManager.AppSettings["BCC"].Split(new char[] { ',' }));

                using (MailMessage mm = new MailMessage())
                {
                    mm.From = new MailAddress(Sender);
                    foreach (string Recipt in Receipent)
                    {
                        mm.To.Add(new MailAddress(Recipt));
                    }
                    foreach (string Bccmail in Bcc)
                    {
                        mm.Bcc.Add(new MailAddress(Bccmail));
                    }

                    mm.Subject = "CleverTap Report";
                    mm.Body = "<div>Hi,<br/> Journey Name : " + JourneyName + "<br/> " +
                                             "Start Time : " + startDate + "<br/>" +
                                             "End Time : " + endDate + "<br/>" +
                                             "Success Records : " + success + "<br/>" +
                                             "Failed Records : " + fail + "<br/>" +
                        "</div>";
                    mm.IsBodyHtml = true;

                    using (var client = new SmtpClient())
                    {
                        client.Credentials = new NetworkCredential(Username, Password);
                        client.Port = Convert.ToInt32(Port);
                        client.Host = Host;
                        client.EnableSsl = true;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Send(mm);
                    }
                }
            }
        }
    }
}