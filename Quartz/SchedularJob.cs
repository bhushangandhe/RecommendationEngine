using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RecommendationEngine.Classes;

namespace RecommendationEngine.Quartz
{
    public class SchedularJob
    {
        public static void Start()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                IScheduler scheduler = schedFact.GetScheduler().GetAwaiter().GetResult();
                scheduler.Start();

                IJobDetail RealCMJob = JobBuilder.Create<RealTimeJob>().Build();
                
                ITrigger RealtrgCMJob = TriggerBuilder.Create()
               .WithIdentity("RealTrigger")
               .StartNow()
               .WithSimpleSchedule(a=>a.WithIntervalInMinutes(5).RepeatForever())
               //.WithCronSchedule("" + ConfigurationManager.AppSettings["RealtimeCronTrigger"] + "")
               .Build();

                scheduler.ScheduleJob(RealCMJob, RealtrgCMJob);
            }
            catch (Exception ex)
            {
                ExceptionLogger.InsertError("Schedular Job", "Start", ex.Message, ex.Source, "Exception", ex.StackTrace);
            }
        }
    }
}